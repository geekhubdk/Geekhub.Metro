using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using Geekhub.Metro.DataModel;
using Newtonsoft.Json;
using NotificationsExtensions.TileContent;
using Windows.ApplicationModel.Resources.Core;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Data;
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
    /// Base class for <see cref="MeetingDataItem"/> and <see cref="MeetingDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class MeetingDataCommon : Geekhub.Metro.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public MeetingDataCommon(string uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(MeetingDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class MeetingDataItem : MeetingDataCommon
    {
        public MeetingDataItem(string uniqueId, String title, String subtitle, String imagePath, String description, String content, MeetingDataGroup group, DateTime startsAt, string url)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
            this._startsAt = startsAt;
            this._url = url;
        }

        private DateTime _startsAt;
        public DateTime StartsAt
        {
            get { return this._startsAt; }
            set { this.SetProperty(ref this._startsAt, value); }
        }

        public string LongDate
        {
            get { return this.StartsAt.ToString("dddd 'den' dd. MMMM", new CultureInfo("da-dk")); }
        }

        public string LongDateTime
        {
            get { return this.StartsAt.ToString("dddd 'den' dd. MMMM', kl:' HH:mm", new CultureInfo("da-dk")); }
        }
        
        public string Day
        {
            get { return this.StartsAt.ToString("dd", new CultureInfo("da-dk")); }
        }

        public string ShortMonth
        {
            get { return this.StartsAt.ToString("MMM", new CultureInfo("da-dk")); }
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }


        private MeetingDataGroup _group;
        public MeetingDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }


        private string _url = string.Empty;
        public string Url
        {
            get { return this._url; }
            set { this.SetProperty(ref this._url, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class MeetingDataGroup : MeetingDataCommon
    {
        public MeetingDataGroup(string uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
        }

        private ObservableCollection<MeetingDataItem> _items = new ObservableCollection<MeetingDataItem>();
        public ObservableCollection<MeetingDataItem> Items
        {
            get { return this._items; }
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
            get { return this._items; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<MeetingDataGroup> _allGroups = new ObservableCollection<MeetingDataGroup>();
        public ObservableCollection<MeetingDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<MeetingDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _sampleDataSource.AllGroups;
        }

        public static MeetingDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static MeetingDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static MeetingDataItem[] Search(string text)
        {
            var ltext = text.ToLower();
            // Simple linear search is acceptable for small data sets
            var matches =
                _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where(
                    (item) => item.Title.ToLower().Contains(ltext));
            return matches.ToArray();
        }

        public SampleDataSource()
        {
            
            LoadData();

        }

        private async void LoadData()
        {
            var request = WebRequest.Create("http://geekhub.herokuapp.com/api/v1/meetings");

            var response = await request.GetResponseAsync();

            var json = "";
            using(var stream = response.GetResponseStream())
            {
                using(var streamreader = new StreamReader(stream))
                {
                    json = streamreader.ReadToEnd();
                    HandleData(json);
                }
            }


            var tile = TileUpdateManager.CreateTileUpdaterForApplication();
            tile.EnableNotificationQueue(true);


            foreach(var e in AllGroups.SelectMany(x=>x.Items).OrderBy(x=>x.StartsAt).Take(4))
            {
                var tileContent = TileContentFactory.CreateTileWideText01();
                tileContent.TextHeading.Text = e.Title;
                tileContent.TextBody3.Text = e.Subtitle;
                tileContent.TextBody1.Text = e.LongDateTime;

                var squareContent = TileContentFactory.CreateTileSquareText04();
                squareContent.TextBodyWrap.Text = e.Title;
                tileContent.SquareContent = squareContent;

                var notification = tileContent.CreateNotification();
                notification.ExpirationTime = new DateTimeOffset(e.StartsAt);

                tile.Update(notification);
            }

        }

        private void HandleData(string json)
        {
            var meetings = JsonConvert.DeserializeObject<Meeting[]>(json);

            foreach (var group in meetings.GroupBy(x => GetGroupKey(x)))
            {
                var month = new MeetingDataGroup(group.Key, group.Key, null, null, null);

                foreach(var meeting in group)
                {
                    month.Items.Add(new MeetingDataItem(meeting.ID.ToString(), meeting.Title, meeting.Location + " - " + meeting.Organizer, null, meeting.Description, meeting.Description, month, meeting.starts_at, meeting.Url));
                }

                AllGroups.Add(month);
            }
        }

        private string GetGroupKey(Meeting meeting)
        {
            if(meeting.starts_at.Year == DateTime.Now.Year)
            {
                return meeting.starts_at.ToString("MMMM", new CultureInfo("da-dk"));
            }

            return meeting.starts_at.ToString("MMMM yyyy", new CultureInfo("da-dk"));
        }
    }
}
