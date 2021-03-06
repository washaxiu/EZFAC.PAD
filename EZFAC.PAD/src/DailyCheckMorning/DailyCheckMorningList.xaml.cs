﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;
using EZFAC.PAD.src.Model;
using EZFAC.PAD.src.Tools;
using EZFAC.PAD.src.Service;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EZFAC.PAD
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DailyCheckMorningList : Page
    {
        private CommonOperation commonOperation = new CommonOperation();
        private DailyCheckMorningService dailyCheckMorningService = new DailyCheckMorningService();
        private MessDialog messDialog = new MessDialog();
        private DataInfo dataInfo = new DataInfo();
        private Dictionary<string, string> data = new Dictionary<string, string>();
        public DispatcherTimer timer;

        public DailyCheckMorningList()
        {
            this.InitializeComponent();
            timetag.Text = DateTime.Now.ToString();
            // 设置定时器，每秒刷新一次时间
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            timetag.Text = DateTime.Now.ToString();
        }

        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is Dictionary<string, string>)
            {
                // 获取导航参数
                data = (Dictionary<string, string>)e.Parameter;
                // 显示内容
                ApprovalListUser.Text = data["username"];
                // 获取职位
                ApprovalListPosition.Text = commonOperation.getJobByLevel(data["userlevel"]);
                // 获取审批信息列表
                dailyCheckMorningService.getApprovalList(lvFiles, data["userlevel"], data["folderName"]);
            }
            date.Text = DateTime.Now.ToString("yyyy-MM-dd");

        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AuthorityNavigation), data);
        }

        private void checkBox_Changed(object sender, RoutedEventArgs e)
        {      
            if (checkBox.IsChecked == true)
            {
                checkBox.Content = "反选";
                lvFiles.SelectAll();
            }
            else
            {
                checkBox.Content = "全选";
                lvFiles.SelectedItems.Clear();
            } 
        }

        private void onLookClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            // 获取当前所在行
            int courrent = Convert.ToInt32(btn.CommandParameter);
            // 获取信息详情
            Dictionary<string, string> getData = dailyCheckMorningService.getDetail(lvFiles.Items[courrent].ToString());
            getData.Add("authority", data["authority"]);
            getData.Add("username", data["username"]);
            getData.Add("userlevel", data["userlevel"]);
            getData.Add("folderName",data["folderName"]);
            // 导航并传递参数
            this.Frame.Navigate(typeof(DailyCheckMorningDetail), getData);
        }

        private async void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (lvFiles.SelectedItems.Count > 0)
            {
                CheckerInfoEntity checkerInfo = new CheckerInfoEntity(ApprovalListUser.Text, data["userlevel"], "1", "0", date.Text, "");
                //  审批所选信息
                dailyCheckMorningService.mulApproval(lvFiles, checkerInfo, data["folderName"]);
                // 设置提示框
                ContentDialog dialog = new ContentDialog()
                {
                    Content = "审批成功！",
                    PrimaryButtonText = "确定",
                };
                dialog.PrimaryButtonClick += primaryButtonClick1;
                await dialog.ShowAsync();
            }
            else
            {
                messDialog.showDialog("请至少选择一条数据进行审批！");
            }
        }

        public void primaryButtonClick1(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // 获取审批信息列表
            dailyCheckMorningService.getApprovalList(lvFiles, data["userlevel"], data["folderName"]);
            checkBox.Content = "全选";
            checkBox.IsChecked = false;
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Content = "获取数据成功！",
                PrimaryButtonText = "确定",
            };
            dialog.PrimaryButtonClick += primaryButtonClick2;
            dataInfo.getInfo("DAILY_CHECK_MORNING", (int.Parse(data["userlevel"]) - 1).ToString(), data["folderName"], dialog);
        }

        public void primaryButtonClick2(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // 获取审批信息列表
            dailyCheckMorningService.getApprovalList(lvFiles, data["userlevel"], data["folderName"]);
        }
    }


}
