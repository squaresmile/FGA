﻿using Android.Media;
using CoreAutomata;

namespace FateGrandAutomata
{
    public class ImgListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        readonly object _syncLock = new object();

        IPattern _lastestPattern;
        bool _newImgAvailable;
        readonly ImageReader _imageReader;

        public ImgListener(ImageReader ImageReader)
        {
            _imageReader = ImageReader;
        }

        public void OnImageAvailable(ImageReader Reader)
        {
            lock (_syncLock)
            {
                _newImgAvailable = true;
            }
        }

        public IPattern AcquirePattern()
        {
            lock (_syncLock)
            {
                if (_newImgAvailable)
                {
                    var latestImage = _imageReader.AcquireLatestImage();

                    try
                    {
                        _lastestPattern?.Dispose();
                        _lastestPattern = new DroidCvPattern(latestImage);
                    }
                    finally
                    {
                        // Close is required for ImageReader. Dispose doesn't work.
                        latestImage.Close();
                    }

                    _newImgAvailable = false;
                }

                return _lastestPattern;
            }
        }
    }
}