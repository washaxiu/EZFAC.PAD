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
using Windows.UI.Xaml.Documents;
using Windows.UI;


// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EZFAC.PAD
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AuthorityNavigation : Page
    {

        private Dictionary<string, string> data = new Dictionary<string, string>();
        private SolidColorBrush black = new SolidColorBrush(Colors.Black);
        private SolidColorBrush gray = new SolidColorBrush(Colors.Gray);
        private string[] checkMenus = {  "CheckRecord",
                                    "UnqualifiedReport",
                                    "",
                                    "",
                                    "",
                                    "DailyCheckNoon",
                                    "",
                                    "YZGCMonthRecord"
                                 };
        private string[] approvalcMenus = {  "ApprovalList",
                                             "UnqualifiedReportList",
                                             "",
                                             "",
                                             "",
                                             "DailyCheckNoonList",
                                             "",
                                             "YZGCMonthRecordList"
                                           };

        public AuthorityNavigation()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            //username.Text = "用户名/员工号/邮箱地址";
            timetag.Text = DateTime.Now.ToString();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RadioButton[] radioButton = { line01, line02, line03, line04, line05, line06, line07, line08 };
            Run[] run = { lineText01, lineText02, lineText03, lineText04, lineText05, lineText06, lineText07, lineText08 };
            for(int i = 0; i < radioButton.Length; i++)
            {
                run[i].Foreground = gray;
                radioButton[i].IsEnabled = false;
            }
            bool flag = false;
            data = (Dictionary<string, string>)e.Parameter;
            string[] authority = data["authority"].Split(',');
            if ("1".Equals(data["userlevel"]))
            {
                button.Content = "检    查";
                menu.Text = "检 查 菜 单";
                UserNameTag.Text = "点检人：";
                DateTag.Text = "点检日期 : ";
                for (int i = 0; i < authority.Length; i++)
                {
                    for(int j=0;j< checkMenus.Length; j++)
                    {
                        if(authority[i] == checkMenus[j])
                        {
                            if(flag == false)
                            {
                                flag = true;
                                radioButton[j].IsChecked = true;
                            }
                            run[j].Foreground = black;
                            radioButton[j].IsEnabled = true;
                        }
                    }
                }
            }
            else
            {
                button.Content = "审    批";
                menu.Text = "审 批 菜 单";
                UserNameTag.Text = "审批人：";
                DateTag.Text = "审批日期 : ";
                for (int i = 0; i < authority.Length; i++)
                {
                    for (int j = 0; j < approvalcMenus.Length; j++)
                    {
                        if (authority[i] == approvalcMenus[j])
                        {
                            if (flag == false)
                            {
                                flag = true;
                                radioButton[j].IsChecked = true;
                            }
                            run[j].Foreground = black;
                            radioButton[j].IsEnabled = true;
                        }
                    }
                }
            }
            username.Text = data["username"];
          /*  List<String> items = new List<string>();
            for (int i = 0; i < authority.Length; i++)
            {
                items.Add(authority[i]);
            }
            module.ItemsSource = items;
            if (authority.Length > 0) module.SelectedIndex = 0;*/
        }

        private void SignIn(object sender, RoutedEventArgs e)
        {
            string level = data["userlevel"];
            //  点检的检查和审批界面
            if (line01.IsChecked == true)
            {
                if(level == "1")
                {
                    this.Frame.Navigate(typeof(CheckRecord), data);
                }
                else
                {
                    this.Frame.Navigate(typeof(ApprovalList), data);
                }
            }
            // 不合格报告的检查和审批界面 
            else if (line02.IsChecked == true)
            {
                if (level == "1")
                {
                    this.Frame.Navigate(typeof(UnqualifiedReport), data);
                }
                else
                {
                    this.Frame.Navigate(typeof(UnqualifiedReportList), data);
                }
            }
            // DC品质异常报告书的检查和审批界面 
            else if(line03.IsChecked == true)
            {

            }
            // DC研磨前半制品的检查和审批界面 
            else if (line04.IsChecked == true)
            {

            }
            // 压铸工程日常点检（早班）的检查和审批界面 
            else if (line05.IsChecked == true)
            {

            }
            // 压铸工程日常点检（中班)的检查和审批界面 
            else if (line06.IsChecked == true)
            {
                if (level == "1")
                {
                    this.Frame.Navigate(typeof(DailyCheckNoon), data);
                }
                else
                {
                    this.Frame.Navigate(typeof(DailyCheckNoonList), data);
                }
            }
            // 压铸工程型维修记录表的检查和审批界面 
            else if (line07.IsChecked == true)
            {

            }
            // 压铸工程月度机械漏油点检记录表的检查和审批界面 
            else if (line08.IsChecked == true)
            {
                if (level == "1")
                {
                    this.Frame.Navigate(typeof(YZGCMonthRecord), data);
                }
                else
                {
                    this.Frame.Navigate(typeof(YZGCMonthRecord), data);
                }
            }
        }



        private void OnLineChecked(object sender, RoutedEventArgs e)
        {
        }

        private void loginOut_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage), "");
        }

        private void lineBlock01_Tapped(object sender, TappedRoutedEventArgs e)
        {
            line01.IsChecked = line01.IsEnabled;
        }

        private void lineBlock02_Tapped(object sender, TappedRoutedEventArgs e)
        {
            line02.IsChecked = line02.IsEnabled;
        }

        private void lineBlock03_Tapped(object sender, TappedRoutedEventArgs e)
        {
            line03.IsChecked = line03.IsEnabled;
        }

        private void lineBlock04_Tapped(object sender, TappedRoutedEventArgs e)
        {
            line04.IsChecked = line04.IsEnabled;
        }

        private void lineBlock05_Tapped(object sender, TappedRoutedEventArgs e)
        {
            line05.IsChecked = line05.IsEnabled;
        }

        private void lineBlock06_Tapped(object sender, TappedRoutedEventArgs e)
        {
            line06.IsChecked = line06.IsEnabled;
        }

        private void lineBlock07_Tapped(object sender, TappedRoutedEventArgs e)
        {
            line07.IsChecked = line07.IsEnabled;
        }

        private void lineBlock08_Tapped(object sender, TappedRoutedEventArgs e)
        {
            line08.IsChecked = line08.IsEnabled;
        }
    }

}
