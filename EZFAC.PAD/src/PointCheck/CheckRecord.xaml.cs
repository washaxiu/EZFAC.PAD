using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Windows.UI.Xaml.Navigation;
using EZFAC.PAD.src.Tools;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EZFAC.PAD
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CheckRecord : Page
    {
        private CommonOperation commonOperation = new CommonOperation();
        private MessDialog messDialog = new MessDialog();
        private Dictionary<string, string> data = new Dictionary<string, string>();
        private string groupName = "A";
        private string lineName = "01";

        public CheckRecord()
        {
            this.InitializeComponent();
            timetag.Text = DateTime.Now.ToString();
       }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is Dictionary<string, string>)
            {
                // 获取导航参数
                data = (Dictionary<string, string>)e.Parameter;
                // 显示内容
                username.Text = data["username"];
            }
            date.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        /*
         *初始化点检内容
         */
        private void initialContent()
        {
            Temp1.IsOn = false;
            Temp2.IsOn = false;
            Temp3.IsOn = false;
            Temp4.IsOn = false;

            Loop1.IsOn = false;
            Loop2.IsOn = false;
            Loop3.IsOn = false;
            Loop4.IsOn = false;

            select1.IsOn = false;
            select4.IsOn = false;

            Plat1.IsOn = false;
            Plat4.IsOn = false;
        }

        private void OnCommitData(object sender, RoutedEventArgs e)
        {
            
            JsonValue good = JsonValue.CreateStringValue("good");
            JsonValue bad = JsonValue.CreateStringValue("bad");

            // 实例化JsonObject对象并设置用户级别信息
            JsonObject checkRecordData = commonOperation.initJsonObject();
            
            // 设置点检用户信息及时间
            checkRecordData["checker"] = JsonValue.CreateStringValue(username.Text);
            checkRecordData["date"] = JsonValue.CreateStringValue(date.Text);

            //  设置用户点检内容
            checkRecordData["group"] = JsonValue.CreateStringValue(groupName);
            checkRecordData["line"] = JsonValue.CreateStringValue(lineName);
            checkRecordData["temp1"] = Temp1.IsOn == true ? good : bad;
            checkRecordData["temp2"] = Temp2.IsOn == true ? good : bad;
            checkRecordData["temp3"] = Temp3.IsOn == true ? good : bad;
            checkRecordData["loop1"] = Loop1.IsOn == true ? good : bad;
            checkRecordData["loop2"] = Loop2.IsOn == true ? good : bad;
            checkRecordData["loop3"] = Loop3.IsOn == true ? good : bad;
            checkRecordData["select1"] = select1.IsOn == true ? good : bad;
            checkRecordData["plat1"] = Plat1.IsOn == true ? good : bad;
            
            string fileName = "ykk_record_" + groupName + "_" + lineName + "_" + date.Text + ".ykk";
            // 将json数据写入对应文件中
            commonOperation.writeJsonToFile(checkRecordData,fileName, KnownFolders.PicturesLibrary, "PointCheck");
            // 设置提示框
            messDialog.showDialog("点检成功！");
        }

        private void OnMachineGroupChanged(object sender, RoutedEventArgs e)
        {
            line01.IsChecked = true;
            if (groupa.IsChecked == true)
            {
                groupName = "A";
                line01.Content = "A-01";
                line02.Content = "A-02";
                line03.Content = "A-03";
                line04.Content = "A-04";
                line05.Content = "A-05";
                line06.Content = "A-06";
                line07.Content = "A-07";
                line08.Content = "A-08";
                line09.Content = "A-09";
                line10.Content = "A-10";
                line11.Content = "A-11";
                line12.Content = "A-12";
                line13.Content = "A-13";
                line14.Content = "A-14";
                line15.Content = "A-15";
                line16.Content = "A-16";
                line11.IsEnabled = true;
                line12.IsEnabled = true;
                line13.IsEnabled = true;
                line14.IsEnabled = true;
                line15.IsEnabled = true;
                line16.IsEnabled = true;
            }
            else if (groupb.IsChecked == true)
            {
                groupName = "B";
                line01.Content = "B-01";
                line02.Content = "B-02";
                line03.Content = "B-03";
                line04.Content = "B-04";
                line05.Content = "B-05";
                line06.Content = "B-06";
                line07.Content = "B-07";
                line08.Content = "B-08";
                line09.Content = "B-09";
                line10.Content = "B-10";
                line11.Content = "B-11";
                line12.Content = "B-12";
                line13.Content = "B-13";
                line14.Content = "B-14";
                line15.Content = "B-15";
                line16.Content = "B-16";
                line11.IsEnabled = true;
                line12.IsEnabled = true;
                line13.IsEnabled = true;
                line14.IsEnabled = true;
                line15.IsEnabled = true;
                line16.IsEnabled = true;
            }
            else if (groupc.IsChecked == true)
            {
                groupName = "C";
                line01.Content = "C-01";
                line02.Content = "C-02";
                line03.Content = "C-03";
                line04.Content = "C-04";
                line05.Content = "C-05";
                line06.Content = "C-06";
                line07.Content = "C-07";
                line08.Content = "C-08";
                line09.Content = "C-09";
                line10.Content = "C-10";
                line11.Content = "C-11";
                line12.Content = "C-12";
                line13.Content = "C-13";
                line14.Content = "C-14";
                line15.Content = "C-15";
                line16.Content = "C-16";
                line11.IsEnabled = false;
                line12.IsEnabled = false;
                line13.IsEnabled = false;
                line14.IsEnabled = false;
                line15.IsEnabled = false;
                line16.IsEnabled = false;
            }
            else if (groupd.IsChecked == true)
            {
                groupName = "D";
                line01.Content = "D-01";
                line02.Content = "D-02";
                line03.Content = "D-03";
                line04.Content = "D-04";
                line05.Content = "D-05";
                line06.Content = "D-06";
                line07.Content = "D-07";
                line08.Content = "D-08";
                line09.Content = "D-09";
                line10.Content = "D-10";
                line11.Content = "D-11";
                line12.Content = "D-12";
                line13.Content = "D-13";
                line14.Content = "D-14";
                line15.Content = "D-15";
                line16.Content = "D-16";
                line11.IsEnabled = false;
                line12.IsEnabled = false;
                line13.IsEnabled = false;
                line14.IsEnabled = false;
                line15.IsEnabled = false;
                line16.IsEnabled = false;
            }
            this.initialContent();
            
        }

        private void OnLineChecked(object sender, RoutedEventArgs e)
        {
            if(line01.IsChecked == true)
            {
                lineName = "01";
            }
            else if(line02.IsChecked == true)
            {
                lineName = "02";
            }
            else if (line03.IsChecked == true)
            {
                lineName = "03";
            }
            else if (line04.IsChecked == true)
            {
                lineName = "04";
            }
            else if (line05.IsChecked == true)
            {
                lineName = "05";
            }
            else if (line06.IsChecked == true)
            {
                lineName = "06";
            }
            else if (line07.IsChecked == true)
            {
                lineName = "07";
            }
            else if (line08.IsChecked == true)
            {
                lineName = "08";
            }
            else if (line09.IsChecked == true)
            {
                lineName = "09";
            }
            else if (line10.IsChecked == true)
            {
                lineName = "10";
            }
            else if (line11.IsChecked == true)
            {
                lineName = "11";
            }
            else if (line12.IsChecked == true)
            {
                lineName = "12";
            }
            else if (line13.IsChecked == true)
            {
                lineName = "13";
            }
            else if (line14.IsChecked == true)
            {
                lineName = "14";
            }
            else if (line15.IsChecked == true)
            {
                lineName = "15";
            }
            else if (line16.IsChecked == true)
            {
                lineName = "16";
            }
            this.initialContent();
        }

        private void Temp4_Toggled(object sender, RoutedEventArgs e)
        {
            Temp1.IsOn = Temp4.IsOn;
            Temp2.IsOn = Temp4.IsOn;
            Temp3.IsOn = Temp4.IsOn;
        }

        private void Loop4_Toggled(object sender, RoutedEventArgs e)
        {
            Loop1.IsOn = Loop4.IsOn;
            Loop2.IsOn = Loop4.IsOn;
            Loop3.IsOn = Loop4.IsOn;
        }

        private void select4_Toggled(object sender, RoutedEventArgs e)
        {
            select1.IsOn = select4.IsOn;
        }

        private void Plat4_Toggled(object sender, RoutedEventArgs e)
        {
            Plat1.IsOn = Plat4.IsOn;
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AuthorityNavigation), data);
        }
    }
}
