﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Phone.Reactive;

namespace OpenStreetApp
{
    public partial class MapControl : UserControl
    {
        Point lastMouseLogicalPos = new Point();
        Point lastMouseViewPort = new Point();
        Point lastOSMPoint = new Point();
        double zoom = 1;
        double zoomCount = 1.0;

        public MultiScaleTileSource Source
        {
            get { return this.OSM_Map.Source; }
            set { this.OSM_Map.Source = value; }
        }

        public MapControl()
        {
            InitializeComponent();

            // Implement double click
            Microsoft.Phone.Reactive.Observable.FromEvent<MouseButtonEventArgs>(this.OSM_Map, "MouseLeftButtonUp")
            .BufferWithTimeOrCount(TimeSpan.FromSeconds(1), 2)
            .Subscribe(new Action<IList<IEvent<MouseButtonEventArgs>>>(
                eventList =>
                {
                    if (eventList.Count >= 2)
                        // subscribing directly on that dispatcher didn't work...
                        this.Dispatcher.BeginInvoke(OSM_Map_OnDoubleClick);
                }));

            this.OSM_Map.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(OSM_Map_ManipulationDelta);
        }

        public void navigateToInputAdress(String inputAdressString)
        {
            String encoded = System.Net.HttpUtility.UrlEncode(inputAdressString);
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.DownloadStringCompleted += (sender, e) =>
            {
                Console.WriteLine(e.Result);
            };
            Uri adress = new Uri("http://geocoding.cloudmade.com/" + CloudeMadeService.ApiKey + "/geocoding/v2/find.js?query="
                + encoded + "&token=" + CloudeMadeService.Token);
            Console.WriteLine(adress);
            wc.DownloadStringAsync(adress);
        }

        private void OSM_Map_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.lastMouseLogicalPos = e.GetPosition(this.OSM_Map);
            this.lastMouseViewPort = this.OSM_Map.ViewportOrigin;
            this.lastOSMPoint = this.OSM_Map.ElementToLogicalPoint(this.lastMouseLogicalPos);
        }

        private void OSM_Map_OnDoubleClick()
        {
            var newzoom = zoom / 2.0;
            zoomCount *= 2;
            Point logicalPoint = this.OSM_Map.ElementToLogicalPoint(this.lastMouseLogicalPos);
            this.OSM_Map.ZoomAboutLogicalPoint(zoom / newzoom, logicalPoint.X, logicalPoint.Y);
            zoom = newzoom;
        }

        //Multi-Touch working
        void OSM_Map_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            // Zoom
            if (e.DeltaManipulation.Scale.X != 0 || e.DeltaManipulation.Scale.Y != 0)
            {
                // zoom by average of X and Y scaling
                var zoom = (Math.Abs(e.DeltaManipulation.Scale.X) + Math.Abs(e.DeltaManipulation.Scale.Y)) / 2.0;

                Point logicalPoint = this.OSM_Map.ElementToLogicalPoint(this.lastMouseLogicalPos);
                this.OSM_Map.ZoomAboutLogicalPoint(zoom, logicalPoint.X, logicalPoint.Y);

                if (this.OSM_Map.ViewportWidth > 1)
                    this.OSM_Map.ViewportWidth = 1;
            }
            // Pinch
            else
            {
                Point newPoint = lastMouseViewPort;
                newPoint.X -= e.CumulativeManipulation.Translation.X / this.OSM_Map.ActualWidth * this.OSM_Map.ViewportWidth;
                newPoint.Y -= e.CumulativeManipulation.Translation.Y / this.OSM_Map.ActualWidth * this.OSM_Map.ViewportWidth;
                this.OSM_Map.ViewportOrigin = newPoint;
            }
        }

        public void navigateToCoordinate(System.Device.Location.GeoCoordinate geoCoordinate, int zoom)
        {
            Point p = OSMHelpers.WorldToTilePos(geoCoordinate.Longitude, geoCoordinate.Latitude, zoom);
            double fixX = (this.OSM_Map.ActualWidth / 256.0) / 4.0;
            double fixY = (this.OSM_Map.ActualHeight / 256.0) / 4.0;
            double xRelative = (p.X - fixX) / Math.Pow(2, zoom);
            double yRelative = (p.Y - fixY) / Math.Pow(2, zoom);

            this.OSM_Map.ViewportWidth = (1.0/Math.Pow(2,zoom));
            this.OSM_Map.ViewportOrigin = new Point(xRelative, yRelative);
            // THINK ABOUT THIS zoomCount *= 12;
        }

        public void zoomToWorldView()
        {
            // USED FOR UNZOOM 
            this.OSM_Map.ViewportOrigin = new Point(0.0, 0.0);
            this.OSM_Map.ZoomAboutLogicalPoint(1.0 / zoomCount, 0, 0);
            zoomCount = 1;
        }
    }
}
