using System.Drawing.Imaging;

namespace UpdatePlugin.Classes
{

    public static class ButtonExtensions
    {
        private const int ImageCacheCapacity = 100;
        private static readonly Dictionary<(Image, float), LinkedListNode<CacheItem>> _imageCache = new();
        private static readonly LinkedList<CacheItem> _lruList = new();
        private static readonly object _cacheLock = new();

        private class CacheItem
        {
            public (Image, float) Key { get; }
            public Image FadedImage { get; }

            public CacheItem((Image, float) key, Image fadedImage)
            {
                Key = key;
                FadedImage = fadedImage;
            }
        }

        public static void SetOpacity(this Button button, Image image, float opacity, ImageLayout layout = ImageLayout.Stretch)
        {
            if (button == null || image == null) return;

            opacity = Math.Clamp(opacity, 0f, 1f);
            Image fadedImage;

            lock (_cacheLock)
            {
                var key = (image, opacity);

                if (_imageCache.TryGetValue(key, out var node))
                {
                    // Cache hit: move to front
                    _lruList.Remove(node);
                    _lruList.AddFirst(node);
                    fadedImage = node.Value.FadedImage;
                }
                else
                {
                    // Cache miss: create faded image
                    fadedImage = CreateFadedImage(image, opacity);

                    // Evict if needed
                    if (_imageCache.Count >= ImageCacheCapacity)
                    {
                        var oldest = _lruList.Last;
                        if (oldest != null)
                        {
                            _imageCache.Remove(oldest.Value.Key);
                            _lruList.RemoveLast();

                            if (oldest.Value.FadedImage is IDisposable disposable)
                                disposable.Dispose();
                        }
                    }

                    var item = new CacheItem(key, fadedImage);
                    var newNode = new LinkedListNode<CacheItem>(item);
                    _lruList.AddFirst(newNode);
                    _imageCache[key] = newNode;
                }
            }

            button.BackgroundImage = fadedImage;
            button.BackgroundImageLayout = layout;
        }

        private static Image CreateFadedImage(Image source, float opacity)
        {
            var faded = new Bitmap(source.Width, source.Height);
            using (Graphics g = Graphics.FromImage(faded))
            {
                var matrix = new ColorMatrix { Matrix33 = opacity };
                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                g.DrawImage(source, new Rectangle(0, 0, faded.Width, faded.Height),
                    0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);
            }
            return faded;
        }

        // Optional: Clear cache manually if needed
        public static void ClearImageCache()
        {
            lock (_cacheLock)
            {
                foreach (var node in _lruList)
                {
                    if (node.FadedImage is IDisposable disposable)
                        disposable.Dispose();
                }
                _imageCache.Clear();
                _lruList.Clear();
            }
        }
    }
}
