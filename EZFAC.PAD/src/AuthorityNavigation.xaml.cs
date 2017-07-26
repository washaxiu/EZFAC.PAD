﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using EZFAC.PAD.src.Model;
using EZFAC.PAD.src.Tools;


// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EZFAC.PAD
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AuthorityNavigation : Page
    {
        public AuthorityNavigation()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            //username.Text = "用户名/员工号/邮箱地址";

            timetag.Text = DateTime.Now.ToString();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Dictionary<string, string> getdata = (Dictionary<string, string>)e.Parameter;
            string[] authority = getdata["authority"].Split(',');
            username.Text = getdata["username"];
            level.Text = getdata["level"];
            List<String> items = new List<string>();
            for (int i = 0; i < authority.Length; i++)
            {
                items.Add(authority[i]);
            }
            module.ItemsSource = items;
            if (items.Count > 0) module.SelectedIndex = 0;
        }

        private void SignIn(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("username", username.Text);
            data.Add("level", level.Text);
            string app = module.SelectedItem.ToString();
            if ("CheckRecord".Equals(app))
            {
                this.Frame.Navigate(typeof(CheckRecord), data);
            }
            else if ("ApprovalList".Equals(app))
            {
                this.Frame.Navigate(typeof(ApprovalList), data);
            }
            else if ("DailyCheckNoon".Equals(app))
            {
                this.Frame.Navigate(typeof(DailyCheckNoon), data);
            }
            else if ("DailyCheckNoonList".Equals(app))
            {
                this.Frame.Navigate(typeof(DailyCheckNoonList), data);
            }
            else if ("ApprovalList".Equals(app))
            {
                this.Frame.Navigate(typeof(ApprovalList), data);
            }
            else if ("ApprovalList".Equals(app))
            {
                this.Frame.Navigate(typeof(ApprovalList), data);
            }
            else if ("ApprovalList".Equals(app))
            {
                this.Frame.Navigate(typeof(ApprovalList), data);
            }
            else if ("ApprovalList".Equals(app))
            {
                this.Frame.Navigate(typeof(ApprovalList), data);
            }

        }
    }

}
