﻿using System.Windows;
using ChainEdge.Features;

namespace ChainEdge.Drivers
{
    public class GiantLedDriver : Driver, IGiantLed
    {
        MediaPlayWindow sidewin;


        public override void Test()
        {
        }

        public void open(string uri)
        {
        }
    }


    public class MediaPlayWindow : Window
    {
        // MediaElement webvw;
    }
}