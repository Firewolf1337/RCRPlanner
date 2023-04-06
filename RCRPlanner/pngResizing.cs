using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RCRPlanner
{
    /// Thanks to https://github.com/camlcase for this image resizing code!
    public interface IpngResizing
    {
        pngResizing Resize(int width, int height);
        void Save(string path, bool dispose);
        MemoryStream ToStream();
    }
    public class pngResizing : IpngResizing, IDisposable
    {
        #region Fields
        private PngBitmapEncoder _pngEncoder;
        private Stream _sourceImageStream;
        private BitmapFrame _firstImageBitmapFrame;
        #endregion

        #region Constructors
        public pngResizing(string sourceImagePath)
            : this(new MemoryStream(File.ReadAllBytes(sourceImagePath)))
        { }

        public pngResizing(byte[] sourceImageBytes)
            : this(new MemoryStream(sourceImageBytes))
        { }

        public pngResizing(Stream sourceImageStream)
        {
            _sourceImageStream = sourceImageStream;
            _firstImageBitmapFrame = GetFirstBitmapFrame(_sourceImageStream);

            _pngEncoder = new PngBitmapEncoder();
            _pngEncoder.Frames.Add(_firstImageBitmapFrame);
        }
        #endregion

        #region IImageResizer Members
        /// <summary>
        /// Resizes the image source using the supplied width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public pngResizing Resize(int width, int height)
        {
            var resizedBitmapFrame = Resize(_firstImageBitmapFrame, width, height);

            _pngEncoder.Frames.Clear();
            _pngEncoder.Frames.Add(resizedBitmapFrame);

            // Make this method chainable
            return this;
        }

        /// <summary>
        /// Saves the modified image source to the supplied destination path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dispose"></param>
        public void Save(string path, bool dispose = true)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                _pngEncoder.Save(fs);
            }

            if (dispose)
            {
                Dispose();
            }
        }

        /// <summary>
        /// Encodes the modified image to an image stream,
        /// using the current jpeg encoder.
        /// </summary>
        /// <returns></returns>
        public MemoryStream ToStream()
        {
            var memStream = new MemoryStream();
            _pngEncoder.Save(memStream);
            return memStream;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _sourceImageStream.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

        #region Private Helpers
        /// <summary>
        /// The image resizing method.
        /// </summary>
        /// <param name="bitmapFrame"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static BitmapFrame Resize(BitmapFrame bitmapFrame, int width, int height)
        {
            double scaleWidth, scaleHeight;

            // Resize proportionally to the width
            if (height == 0)
            {
                scaleWidth = width;
                scaleHeight = (((double)width / bitmapFrame.PixelWidth) * bitmapFrame.PixelHeight);
            }
            // Resize proportionally to the height
            else if (width == 0)
            {
                scaleHeight = height;
                scaleWidth = (((double)height / bitmapFrame.PixelHeight) * bitmapFrame.PixelWidth);
            }
            // Resize using the supplied width and height
            else
            {
                scaleWidth = width;
                scaleHeight = height;
            }

            // Create the scale transform
            var scaleTransform = new ScaleTransform(scaleWidth / bitmapFrame.PixelWidth, scaleHeight / bitmapFrame.PixelHeight, 0, 0);

            // Transform the bitmap frame
            var transformedBitmap = new TransformedBitmap(bitmapFrame, scaleTransform);

            return BitmapFrame.Create(transformedBitmap);
        }

        /// <summary>
        /// Reads out the first <see cref="BitmapFrame"/> from the supplied image stream.
        /// </summary>
        /// <param name="imageStream"></param>
        /// <returns></returns>
        private static BitmapFrame GetFirstBitmapFrame(Stream imageStream)
        {
            var bitmapDecoder = BitmapDecoder.Create(imageStream,
                BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            return bitmapDecoder.Frames[0];
        }
        #endregion
    }
}

