using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Maps;

namespace Geekhub.Metro.DataModel
{
    class Meeting
    {
        public string Title { get; set; }
        public DateTime starts_at { get; set; }
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
