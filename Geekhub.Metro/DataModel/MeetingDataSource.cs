using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using Geekhub.Metro.DataModel;
using Newtonsoft.Json;
using Windows.UI.Popups;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace Geekhub.Metro.Data
{
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
                _meetingDataSource.AllGroups.Where((group) => group.MonthName.Equals(uniqueId));
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
                var month = new MeetingDataGroup(group.Key);

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