﻿using System.Windows;
using Microsoft.Phone.Controls;

namespace OpenStreetApp
{
    public partial class AddFavorite : PhoneApplicationPage
    {
        private Location current = null;

        public AddFavorite()
        {
            OSMHelpers.GeoPositionToLocation(MainPage.currentPosition, onLocationReceived);
            InitializeComponent();
        }

        private void onLocationReceived(Location current)
        {
            this.LocationInfo.Text = current.ToString();
            this.current = current;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            OSA_Configuration.Instance.addFavorite(this.current);
            NavigationService.GoBack();
        }
    }
}