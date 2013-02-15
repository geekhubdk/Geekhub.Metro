using System;
using Geekhub.Metro.Common;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Geekhub.Metro.Data
{
    /// <summary>
    ///     Base class for <see cref="MeetingDataItem" /> and <see cref="MeetingDataGroup" /> that
    ///     defines properties common to both.
    /// </summary>
    [WebHostHidden]
    public abstract class MeetingDataCommon : BindableBase
    {
        private static readonly Uri _baseUri = new Uri("ms-appx:///");
        private string _description = string.Empty;
        private ImageSource _image;
        private String _imagePath;
        private double _latitude;
        private double _longtitude;
        private string _subtitle = string.Empty;
        private string _title = string.Empty;

        private string _uniqueId;

        public MeetingDataCommon(string uniqueId, String title, String subtitle, String imagePath, String description,
                                 double latitude, double longtitude)
        {
            _uniqueId = uniqueId;
            _title = title;
            _subtitle = subtitle;
            _description = description;
            _imagePath = imagePath;
            _latitude = latitude;
            _longtitude = longtitude;
        }

        public string UniqueId
        {
            get { return _uniqueId; }
            set { SetProperty(ref _uniqueId, value); }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string Subtitle
        {
            get { return _subtitle; }
            set { SetProperty(ref _subtitle, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public double Latitude
        {
            get { return _latitude; }
            set { SetProperty(ref _latitude, value); }
        }

        public double Longtitude
        {
            get { return _longtitude; }
            set { SetProperty(ref _longtitude, value); }
        }

        public ImageSource Image
        {
            get
            {
                if (_image == null && _imagePath != null)
                {
                    _image = new BitmapImage(new Uri(_baseUri, _imagePath));
                }
                return _image;
            }

            set
            {
                _imagePath = null;
                SetProperty(ref _image, value);
            }
        }

        public void SetImage(String path)
        {
            _image = null;
            _imagePath = path;
            OnPropertyChanged("Image");
        }
    }
}