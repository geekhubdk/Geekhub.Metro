using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Geekhub.Metro.Common;

namespace Geekhub.Metro.Data
{
    /// <summary>
    ///     Generic group data model.
    /// </summary>
    public class MeetingDataGroup : BindableBase
    {
        private string _monthName;
        private readonly ObservableCollection<MeetingDataItem> _items = new ObservableCollection<MeetingDataItem>();

        public MeetingDataGroup(string monthName)
        {
            _monthName = monthName;
        }

        public ObservableCollection<MeetingDataItem> Items
        {
            get { return _items; }
        }

        public string MonthName
        {
            get { return _monthName; }
            set { SetProperty(ref _monthName, value); }
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
}