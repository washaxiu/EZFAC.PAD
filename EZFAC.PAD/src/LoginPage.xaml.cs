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
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using System.Net;
using System.Text;
using System.Threading;



// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EZFAC.PAD
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private string jsonfile = "user.json";
        private CommonOperation commonOperation = new CommonOperation();
        private MessDialog mess = new MessDialog();
        private DataInfo userInfo = new DataInfo();
        public string msg = "";
        private Object thisLock = new Object();
        public DispatcherTimer timer;
        public DispatcherTimer loginTimer;

        public LoginPage()
        {
            // 判断是否存在 StatusBar
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                commonOperation.hide();
            }
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            //username.Text = "用户名/员工号/邮箱地址";
            username.Text = "000002";
            password.Password = "123456";
            information.Text = "";
            timetag.Text = DateTime.Now.ToString();

            // 设置定时器，每秒刷新一次时间
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();

            initFile();

        }

        private void Timer_Tick(object sender, object e)
        {
            timetag.Text = DateTime.Now.ToString();
        }

        private async void initFile()
        {
            StorageFile file = await KnownFolders.PicturesLibrary.TryGetItemAsync(jsonfile) as StorageFile;
            if (file == null)
            {
                userInfo.getUserList();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is Dictionary<string, string>)
            {
                // 获取导航参数
                Dictionary<string, string> data = (Dictionary<string, string>)e.Parameter;
 
                if (data.ContainsKey("loginError"))
                {
                    information.Text = data["loginError"];
                }
                else
                {
                    information.Text = "";
                }
            }           
            //password.ClearValue(PasswordBox.PasswordProperty);
        }

        private async void LoginClick(object sender, RoutedEventArgs e)
        {
            // 对输入内容进行验证
            Dictionary<string, string> data = new Dictionary<string, string>();
            bool isValidUser = false;
            bool isChecked = false;
            string userLevel = "1";
            StorageFolder folder_demonstration = KnownFolders.PicturesLibrary;
            StorageFile file = await folder_demonstration.TryGetItemAsync(jsonfile) as StorageFile;
            if (file != null)
            {
                //string jsonText = await FileIO.ReadTextAsync(file);
                var jsonText = await FileIO.ReadTextAsync(file);
                JsonObject jsonObject = JsonObject.Parse(jsonText);
                JsonArray jsonArray = jsonObject["Users"].GetArray();

                foreach (JsonValue userInfo in jsonArray)
                {
                    JsonObject userObject = userInfo.GetObject();
                    string uniqueId = userObject["UniqueId"].GetString();
                    string jusername = userObject["UserName"].GetString();
                    string eMail = userObject["e-mail"].GetString();
                    string jpassword = userObject["Password"].GetString();
                    string jlevel = userObject["level"].GetString();
                    string authority = userObject["authority"].GetString();
                    //string jposation = userObject["Password"].GetString();                 
                    if (username.Text == uniqueId || username.Text == jusername || username.Text == eMail)
                    {
                        isValidUser = true;
                        if (password.Password == jpassword)
                        {
                            data.Add("username", jusername);
                            data.Add("authority", authority);
                            //data.Add("position", jposation);
                            data.Add("userlevel", jlevel);
                            userLevel = jlevel;
                            isChecked = true;
                            break;
                        }
                        else
                        {
                            msg = "密码不正确";
                            isChecked = false;
                            break;
                        }
                    }
                }

                if (!isValidUser)
                {
                    // 如果用户名不存在，就重新向服务器获取数据
                    userInfo.getUserList();
                    // 设置定时器
                    loginTimer = new DispatcherTimer();
                    loginTimer.Interval = new TimeSpan(0, 0, 2);
                    loginTimer.Tick += LoginTimer_Tick; ;
                    loginTimer.Start();
                }
                // 导航并传递参数
                if (!isChecked)
                {
                    data["loginError"] = msg;
                    this.Frame.Navigate(typeof(LoginPage), data);
                }
                else
                {
                    this.Frame.Navigate(typeof(AuthorityNavigation), data);
                }
            }
            else
            {
                // 如果用户文件不存在，就重新向服务器获取数据
                userInfo.getUserList();
                // 设置定时器
                loginTimer = new DispatcherTimer();
                loginTimer.Interval = new TimeSpan(0, 0, 2);
                loginTimer.Tick += LoginTimer_Tick; ;
                loginTimer.Start();
            }
        }

        private async void LoginTimer_Tick(object sender, object e)
        {
            // 对输入内容进行验证
            Dictionary<string, string> data = new Dictionary<string, string>();
            bool isChecked = false;
            string userLevel = "1";
            StorageFolder folder_demonstration = KnownFolders.PicturesLibrary;
            StorageFile file = await folder_demonstration.TryGetItemAsync(jsonfile) as StorageFile;
            if (file != null)
            {
                //string jsonText = await FileIO.ReadTextAsync(file);
                var jsonText = await FileIO.ReadTextAsync(file);
                JsonObject jsonObject = JsonObject.Parse(jsonText);
                JsonArray jsonArray = jsonObject["Users"].GetArray();

                foreach (JsonValue userInfo in jsonArray)
                {
                    JsonObject userObject = userInfo.GetObject();
                    string uniqueId = userObject["UniqueId"].GetString();
                    string jusername = userObject["UserName"].GetString();
                    string eMail = userObject["e-mail"].GetString();
                    string jpassword = userObject["Password"].GetString();
                    string jlevel = userObject["level"].GetString();
                    string authority = userObject["authority"].GetString();
                    //string jposation = userObject["Password"].GetString();                 
                    if (username.Text == uniqueId || username.Text == jusername || username.Text == eMail)
                    {
                        if (password.Password == jpassword)
                        {
                            data.Add("username", jusername);
                            data.Add("authority", authority);
                            data.Add("userlevel", jlevel);
                            userLevel = jlevel;
                            isChecked = true;
                            break;
                        }
                    }
                }
                // 导航并传递参数
                if (!isChecked)
                {
                    data["loginError"] = "用户名或密码出错";
                    loginTimer.Stop();
                    this.Frame.Navigate(typeof(LoginPage), data);
                }
                else
                {
                    loginTimer.Stop();
                    this.Frame.Navigate(typeof(AuthorityNavigation), data);
                }
            }
            else
            {
                data["loginError"] = "网络出错，请联网后，重启应用";
                loginTimer.Stop();
                this.Frame.Navigate(typeof(LoginPage), data);
            }
        }
    }

}
