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
using EZFAC.PAD.src.MaintenanceLog;


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
                                    "SemiFinishedCheck",
                                    "DailyCheckMorning",
                                    "DailyCheckNoon",
                                    "MaintenanceLog",
                                    "YZGCMonthRecord"
                                 };
        private string[] approvalcMenus = {  "ApprovalList",
                                             "SemiFinishedCheckList",
                                             "DailyCheckMorningList",
                                             "DailyCheckNoonList",
                                             "MaintenanceLogList",
                                             "YZGCMonthRecordList"
                                           };
        public DispatcherTimer timer;

        public AuthorityNavigation()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            //username.Text = "用户名/员工号/邮箱地址";
            timetag.Text = DateTime.Now.ToString();
            date.Text = DateTime.Now.ToString("yyyy-MM-dd");

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RadioButton[] radioButton = { line01, line04, line05, line06, line07, line08 };
            Run[] run = { lineText01, lineText04, lineText05, lineText06, lineText07, lineText08 };
            for(int i = 0; i < radioButton.Length; i++)
            {
                run[i].Foreground = gray;
                radioButton[i].IsEnabled = false;
            }
            bool flag = false;
            data = (Dictionary<string, string>)e.Parameter;
            string[] authority = data["authority"].Split('_');
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
                        string value = (j + 1) + "";
                        if(authority[i] == value)
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
                        string value = (j + 1) + "";
                        if (authority[i] == value)
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
        }

        private void SignIn(object sender, RoutedEventArgs e)
        {
            string level = data["userlevel"];
            //  点检的检查和审批界面
            if (line01.IsChecked == true)
            {
                data["folderName"] = checkMenus[0];
                if (level == "1")
                {
                    this.Frame.Navigate(typeof(CheckRecord), data);
                }
                else
                {
                    this.Frame.Navigate(typeof(ApprovalList), data);
                }
            }

            // DC研磨前半制品的检查和审批界面 
            else if (line04.IsChecked == true)
            {
                data["folderName"] = checkMenus[1];
                if (level == "1")
                {
                    this.Frame.Navigate(typeof(SemiFinishedCheck), data);
                }
                else
                {
                    this.Frame.Navigate(typeof(SemiFinishedCheckList), data);
                }
            }
            // 压铸工程日常点检（早班）的检查和审批界面 
            else if (line05.IsChecked == true)
            {
                data["folderName"] = checkMenus[2];
                if (level == "1")
                {
                    this.Frame.Navigate(typeof(DailyCheckMorning), data);
                }
                else
                {
                    this.Frame.Navigate(typeof(DailyCheckMorningList), data);
                }
            }
            // 压铸工程日常点检（中班)的检查和审批界面 
            else if (line06.IsChecked == true)
            {
                data["folderName"] = checkMenus[3];
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
                data["folderName"] = checkMenus[4];
                if (level == "1")
                {
                    this.Frame.Navigate(typeof(MaintenanceLog), data);
                }
                else
                {
                    this.Frame.Navigate(typeof(MaintenanceLogList), data);
                }
            }
            // 压铸工程月度机械漏油点检记录表的检查和审批界面 
            else if (line08.IsChecked == true)
            {
                data["folderName"] = checkMenus[5];
                if (level == "1")
                {
                    this.Frame.Navigate(typeof(YZGCMonthRecord), data);
                }
                else
                {
                    this.Frame.Navigate(typeof(YZGCMonthRecordList), data);
                }
            }
        }



        private void OnLineChecked(object sender, RoutedEventArgs e)
        {
        }

        private void loginOut_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage), data);
        }

        private void lineBlock01_Tapped(object sender, TappedRoutedEventArgs e)
        {
            line01.IsChecked = line01.IsEnabled;
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
