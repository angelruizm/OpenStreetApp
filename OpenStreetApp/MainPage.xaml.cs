﻿using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace OpenStreetApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Konstruktor
        public MainPage()
        {
            InitializeComponent();
        }

        public void TestTiles(Point coord, double zoom, string path)
        {
            //this.picture.Source = new BitmapImage(new Uri(path));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            /*TileDownloadManager tdm = new TileDownloadManager();
            for (int i = 0; i < 10; i++)
            {
                System.Threading.Thread.Sleep(5000);
                Point p = new Point(46.8 + (i * 3), 10.1 + (i * 5));
                tdm.fetch(p, 12, new Action<Point,double,string>(TestTiles));
            }*/
        }

        private void openButton_Click(object sender, EventArgs e)
        {

        }

        private void showFavoritesButton_Click(object sender, EventArgs e)
        {

        }

        private void preferencesButton_Click(object sender, EventArgs e)
        {

        }

        private void favoriteButton_Click(object sender, EventArgs e)
        {

        }

        private void POIButton_Click(object sender, EventArgs e)
        {

        }
    }
}