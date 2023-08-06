﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace ChainEdge;

/// <summary>
/// The main window that hosts WebView2.
/// </summary>
public class EdgeWindow : Window
{
    WebView2 webvw;

    Grid grid;

    public EdgeWindow()
    {
        grid = new Grid();

        Content = grid;

        Loaded += OnLoaded;
    }

    async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Icon = BitmapFrame.Create(new Uri("./static/logo.webp", UriKind.Relative));

        webvw = new WebView2
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        grid.Children.Add(webvw);


        if (webvw != null && webvw.CoreWebView2 == null)
        {
            var env = await CoreWebView2Environment.CreateAsync(null, "data");

            await webvw.EnsureCoreWebView2Async(env);
        }
        if (webvw.CoreWebView2 == null)
        {
            MessageBox.Show("获取 CoreWebView2 失败");
            return;
        }

        var settings = webvw.CoreWebView2.Settings;
        settings.AreDevToolsEnabled = false;
        settings.IsZoomControlEnabled = false;
        settings.AreDefaultContextMenusEnabled = false;

        string url = EdgeApp.AppConf[nameof(url)];
        webvw.CoreWebView2.Navigate(url);

        // suppress new window being opened
        webvw.CoreWebView2.NewWindowRequested += (obj, args) =>
        {
            args.NewWindow = (CoreWebView2)obj;
            args.Handled = true;
        };

        webvw.CoreWebView2.AddHostObjectToScript("wrap", EdgeApp.host);

        webvw.NavigationCompleted += AfterNavigation;
    }

    void AfterNavigation(object target, CoreWebView2NavigationCompletedEventArgs e)
    {
        Title = webvw.CoreWebView2.DocumentTitle;
    }


    public void PostMessage(string v)
    {
        webvw.CoreWebView2.PostWebMessageAsJson(v);
    }


    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private const int HOTKEY_ID = 9000;

    //Modifiers:
    private const uint MOD_NONE = 0x0000; //(none)
    private const uint MOD_ALT = 0x0001; //ALT
    private const uint MOD_CONTROL = 0x0002; //CTRL
    private const uint MOD_SHIFT = 0x0004; //SHIFT

    private const uint MOD_WIN = 0x0008; //WINDOWS

    //CAPS LOCK:
    private const uint VK_CAPITAL = 0x14;


    private IntPtr _windowHandle;
    private HwndSource _source;

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        _windowHandle = new WindowInteropHelper(this).Handle;
        _source = HwndSource.FromHwnd(_windowHandle);
        _source.AddHook(HwndHook);

        RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_ALT | MOD_CONTROL, 0x44); //CTRL + CAPS_LOCK
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_HOTKEY = 0x0312;
        switch (msg)
        {
            case WM_HOTKEY:
                switch (wParam.ToInt32())
                {
                    case HOTKEY_ID:
                        int vkey = (((int)lParam >> 16) & 0xFFFF);
                        if (vkey == 0x44)
                        {
                            // set visiblity of the device manager window

                            MessageBox.Show("");
                        }
                        handled = true;
                        break;
                }
                break;
        }
        return IntPtr.Zero;
    }

    protected override void OnClosed(EventArgs e)
    {
        _source.RemoveHook(HwndHook);
        UnregisterHotKey(_windowHandle, HOTKEY_ID);
        base.OnClosed(e);
    }
}