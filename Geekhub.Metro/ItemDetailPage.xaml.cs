using Bing.Maps;
using Geekhub.Metro.Common;
using Geekhub.Metro.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Geekhub.Metro.DataModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace Geekhub.Metro
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ItemDetailPage : Geekhub.Metro.Common.LayoutAwarePage
    {
        private DataTransferManager dataTransferManager;

		private MeetingDataItem Item;

        public ItemDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += this.DataRequested;

            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.dataTransferManager.DataRequested -= this.DataRequested;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            try
            {   
                // Allow saved page state to override the initial item to display
                if (pageState != null && pageState.ContainsKey("SelectedItem"))
                {
                    navigationParameter = pageState["SelectedItem"];
                }

                // TODO: Create an appropriate data model for your problem domain to replace the sample data
                Item = MeetingDataSource.GetItem((String)navigationParameter);
				this.DefaultViewModel["Item"] = Item;
				this.DefaultViewModel["Group"] = Item.Group;
                this.DefaultViewModel["Items"] = Item.Group.Items;
                this.flipView.SelectedItem = Item;
            }                
            catch(Exception)
            {
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var selectedItem = (MeetingDataItem)this.flipView.SelectedItem;
            pageState["SelectedItem"] = selectedItem.UniqueId;
        }

        private void UxReadMore_Click(object sender, RoutedEventArgs e)
        {
            Windows.System.Launcher.LaunchUriAsync(new Uri(Item.Url));
        }

        private void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            var meeting = (MeetingDataItem) flipView.SelectedItem;
            DataRequest request = e.Request;
            request.Data.Properties.Title = meeting.Title;
            request.Data.Properties.Description = meeting.Description;
            request.Data.Properties.ApplicationName = "Geekhub";
            var link = new Uri("http://geekhub.dk/meetings/" + meeting.UniqueId);
            request.Data.SetUri(link);

        }

        private void UxMap_OnLoaded(object sender, RoutedEventArgs e)
        {
            var map = sender as Bing.Maps.Map;
            map.Center = new Location(Item.Latitude, Item.Longtitude);
            var pushpin = new Bing.Maps.Pushpin();
            MapLayer.SetPosition(pushpin, map.Center);
            map.Children.Add(pushpin);
        }
    }
}
