using System;
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

        private Dictionary<string, string> data = new Dictionary<string, string>();

        public AuthorityNavigation()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            //username.Text = "用户名/员工号/邮箱地址";
            timetag.Text = DateTime.Now.ToString();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            data = (Dictionary<string, string>)e.Parameter;
            string[] authority = data["authority"].Split(',');
            if ("1".Equals(data["userlevel"]))
            {
                button.Content = "检    查";
            }
            else
            {
                button.Content = "审    批";
            }
            List<String> items = new List<string>();
            for (int i = 0; i < authority.Length; i++)
            {
                items.Add(authority[i]);
            }
            module.ItemsSource = items;
            if (authority.Length > 0) module.SelectedIndex = 0;
        }

        private void SignIn(object sender, RoutedEventArgs e)
        {
            string app = module.SelectedItem.ToString();

            //  点检的检查和审批界面
            if ("CheckRecord".Equals(app))
            {
                this.Frame.Navigate(typeof(CheckRecord), data);
            }
            else if ("ApprovalList".Equals(app))
            {
                this.Frame.Navigate(typeof(ApprovalList), data);
            }

            // 中班和检查和审批界面 
            else if ("DailyCheckNoon".Equals(app))
            {
                this.Frame.Navigate(typeof(DailyCheckNoon), data);
            }
            else if ("DailyCheckNoonList".Equals(app))
            {
                this.Frame.Navigate(typeof(ApprovalList), data);
            }
        }

        private void loginOut_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage), "");
        }
    }

}
