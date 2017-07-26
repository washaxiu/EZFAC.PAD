using System;
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
    public sealed partial class ApprovalList : Page
    {
        private string userlevel = "2";
        private CommonOperation commonOperation = new CommonOperation();
        private PointCheckService pointCheckService = new PointCheckService();
        private MessDialog messDialog = new MessDialog();

        public ApprovalList()
        {
            this.InitializeComponent();
            timetag.Text = DateTime.Now.ToString();
        }

        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is Dictionary<string, string>)
            {
                // 获取导航参数
                Dictionary<string, string> data = (Dictionary<string, string>)e.Parameter;
                // 显示内容
                ApprovalListUser.Text = data["username"];
                string level = data["level"];
                userlevel = level;
                // 获取职位
                ApprovalListPosition.Text = commonOperation.getJobByLevel(level);
                // 获取审批信息列表
                pointCheckService.getApprovalList(lvFiles, userlevel);
            }
            date.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            this.Frame.Navigate(typeof(LoginPage), data);
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
            Dictionary<string, string> data = pointCheckService.getDetail(lvFiles.Items[courrent].ToString(), ApprovalListUser.Text, userlevel);
            // 导航并传递参数
            this.Frame.Navigate(typeof(ApprovalDetail), data);
        }

        private async void Confirm_Click(object sender, RoutedEventArgs e)
        {
           // string groupName = null, lineName = null, date = null;

          //  string fileName = "ykk_record_" + groupName + "_" + lineName + "_" + date + ".ykk";

            // 设置提示框
        /*    for(int i=0;i<lvFiles.SelectedItems.Count;i++)
            {
                // messDialog.showDialog(lvFiles.SelectedItems[i].ToString());
                ContentDialog ct = new ContentDialog()
                {
                    Content = lvFiles.SelectedItems[i].ToString(),
                    PrimaryButtonText = "确定",
                    SecondaryButtonText = "取消"
                };
                await ct.ShowAsync();
            }*/
            PointCheck pointCheck = new PointCheck();
            pointCheck.checker = ApprovalListUser.Text;
            pointCheck.date = date.Text;

            pointCheckService.mulApproval(lvFiles, userlevel, pointCheck, "PointCheck");
            // 获取审批信息列表

            ContentDialog dialog = new ContentDialog()
            {
                Content = "审批成功",
                PrimaryButtonText = "确定",
                SecondaryButtonText = "取消"
            };
            dialog.PrimaryButtonClick += primaryButtonClick1;
            await dialog.ShowAsync();
           // messDialog.showDialog("审批成功！");
           //  pointCheckService.getApprovalList(lvFiles, userlevel);
        }

        public void primaryButtonClick1(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            pointCheckService.getApprovalList(lvFiles, userlevel);
        }

    }


}
