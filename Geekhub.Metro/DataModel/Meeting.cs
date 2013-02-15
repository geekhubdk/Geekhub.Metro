using System;

namespace Geekhub.Metro.DataModel
{
    internal class Meeting
    {
        public string Title { get; set; }
        public DateTime Starts_At { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Url { get; set; }
        public string Organizer { get; set; }
        public bool CostsMoney { get; set; }
        public int ID { get; set; }
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
    }
}