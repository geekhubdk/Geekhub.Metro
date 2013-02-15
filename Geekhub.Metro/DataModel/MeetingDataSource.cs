using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using Geekhub.Metro.Common;
using Geekhub.Metro.DataModel;
using Newtonsoft.Json;
using Windows.Foundation.Metadata;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

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

    /// <summary>
    ///     Generic item data model.
    /// </summary>
    public class MeetingDataItem : MeetingDataCommon
    {
        private string _content = string.Empty;
        private MeetingDataGroup _group;
        private DateTime _startsAt;
        private string _url = string.Empty;

        public MeetingDataItem(string uniqueId, String title, String subtitle, String imagePath, String description,
                               String content, MeetingDataGroup group, DateTime startsAt, string url, double latitude,
                               double longtitude)
            : base(uniqueId, title, subtitle, imagePath, description, latitude, longtitude)
        {
            _content = content;
            _group = group;
            _startsAt = startsAt;
            _url = url;
        }

        public DateTime StartsAt
        {
            get { return _startsAt; }
            set { SetProperty(ref _startsAt, value); }
        }

        public string LongDate
        {
            get { return StartsAt.ToString("dddd 'den' dd. MMMM", new CultureInfo("da-dk")); }
        }

        public string ShortDateTime
        {
            get { return StartsAt.ToString("dd MMMM', kl:' HH:mm", new CultureInfo("da-dk")); }
        }

        public string LongDateTime
        {
            get { return StartsAt.ToString("dddd 'den' dd. MMMM', kl:' HH:mm", new CultureInfo("da-dk")); }
        }

        public string Day
        {
            get { return StartsAt.ToString("dd", new CultureInfo("da-dk")); }
        }

        public string ShortMonth
        {
            get { return StartsAt.ToString("MMM", new CultureInfo("da-dk")); }
        }

        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }


        public MeetingDataGroup Group
        {
            get { return _group; }
            set { SetProperty(ref _group, value); }
        }


        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }
    }

    /// <summary>
    ///     Generic group data model.
    /// </summary>
    public class MeetingDataGroup : MeetingDataCommon
    {
        private readonly ObservableCollection<MeetingDataItem> _items = new ObservableCollection<MeetingDataItem>();

        public MeetingDataGroup(string uniqueId, String title, String subtitle, String imagePath, String description,
                                double latitude, double longtitude)
            : base(uniqueId, title, subtitle, imagePath, description, latitude, longtitude)
        {
        }

        public ObservableCollection<MeetingDataItem> Items
        {
            get { return _items; }
        }

        public IEnumerable<MeetingDataItem> TopItems
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed
            get { return _items; }
        }
    }

    /// <summary>
    ///     Creates a collection of groups and items with hard-coded content.
    /// </summary>
    public sealed class MeetingDataSource
    {
        private static readonly MeetingDataSource _meetingDataSource = new MeetingDataSource();

        private readonly ObservableCollection<MeetingDataGroup> _allGroups =
            new ObservableCollection<MeetingDataGroup>();

        public MeetingDataSource()
        {
            LoadData();
        }

        public ObservableCollection<MeetingDataGroup> AllGroups
        {
            get { return _allGroups; }
        }

        public static IEnumerable<MeetingDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups"))
                throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _meetingDataSource.AllGroups;
        }

        public static MeetingDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            IEnumerable<MeetingDataGroup> matches =
                _meetingDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static MeetingDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            IEnumerable<MeetingDataItem> matches =
                _meetingDataSource.AllGroups.SelectMany(group => group.Items)
                                  .Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static MeetingDataItem[] Search(string text)
        {
            string ltext = text.ToLower();
            // Simple linear search is acceptable for small data sets
            IEnumerable<MeetingDataItem> matches =
                _meetingDataSource.AllGroups.SelectMany(group => group.Items).Where(
                    (item) => item.Title.ToLower().Contains(ltext));
            return matches.ToArray();
        }

        private async void LoadData()
        {
            WebRequest request = WebRequest.Create("http://geekhub.herokuapp.com/api/v1/meetings");

            try
            {
                WebResponse response = await request.GetResponseAsync();

                string json = "";
                using (Stream stream = response.GetResponseStream())
                {
                    using (var streamreader = new StreamReader(stream))
                    {
                        json = streamreader.ReadToEnd();
                        HandleData(json);
                    }
                }
            }
            catch (Exception)
            {
                new MessageDialog("Kunne ikke hente indhold fra server.", "Der skete en fejl").ShowAsync();
                return;
            }

            //var tile = TileUpdateManager.CreateTileUpdaterForApplication();
            //tile.EnableNotificationQueue(true);


            //foreach(var e in AllGroups.SelectMany(x=>x.Items).OrderBy(x=>x.Starts_At).Take(4))
            //{
            //    var tileContent = TileContentFactory.CreateTileWideText01();
            //    tileContent.TextHeading.Text = e.Title;
            //    tileContent.TextBody3.Text = e.Subtitle;
            //    tileContent.TextBody1.Text = e.LongDateTime;

            //    var squareContent = TileContentFactory.CreateTileSquareText04();
            //    squareContent.TextBodyWrap.Text = e.Title;
            //    tileContent.SquareContent = squareContent;

            //    var notification = tileContent.CreateNotification();
            //    notification.ExpirationTime = new DateTimeOffset(e.Starts_At);

            //    tile.Update(notification);
            //}
        }

        private void HandleData(string json)
        {
            var meetings = JsonConvert.DeserializeObject<Meeting[]>(json);

            foreach (var group in meetings.GroupBy(x => GetGroupKey(x)))
            {
                var month = new MeetingDataGroup(group.Key, group.Key, null, null, null, 0, 0);

                foreach (Meeting meeting in group)
                {
                    month.Items.Add(new MeetingDataItem(meeting.ID.ToString(), meeting.Title,
                                                        meeting.Location + " - " + meeting.Organizer, null,
                                                        meeting.Description, meeting.Description, month,
                                                        meeting.Starts_At, meeting.Url, meeting.Latitude,
                                                        meeting.Longtitude));
                }

                AllGroups.Add(month);
            }
        }

        private string GetGroupKey(Meeting meeting)
        {
            if (meeting.Starts_At.Year == DateTime.Now.Year)
            {
                return meeting.Starts_At.ToString("MMMM", new CultureInfo("da-dk"));
            }

            return meeting.Starts_At.ToString("MMMM yyyy", new CultureInfo("da-dk"));
        }
    }
}