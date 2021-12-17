﻿using System.Windows;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace SkyEdge
{
    /// <summary>
    /// The secondary window for info display.
    /// </summary>
    public class SideWindow : Window
    {
        WebView2 webvw;

        internal async void CreateWebView2(string uri)
        {
            
            
            webvw = new WebView2()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            
            AddChild(webvw);

            if (webvw != null && webvw.CoreWebView2 == null)
            {
                var env = await CoreWebView2Environment.CreateAsync(null, "data");

                await webvw.EnsureCoreWebView2Async(env);
            }
            webvw.CoreWebView2.Navigate(uri);
            webvw.CoreWebView2.OpenDevToolsWindow();

            // suppress new window being opened
            webvw.CoreWebView2.NewWindowRequested += (obj, args) =>
            {
                args.NewWindow = (CoreWebView2) obj;
                args.Handled = true;
            };
        }
    }
}