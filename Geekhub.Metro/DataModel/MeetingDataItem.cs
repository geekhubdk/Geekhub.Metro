using System;
using System.Globalization;

namespace Geekhub.Metro.Data
{
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
}