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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EZFAC.PAD
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class YZGCMonthRecord : Page
    {
        private JsonObject checkRecordData = new JsonObject();
        private StorageFile record_file;//UWP 采用StorageFile来读写文件
        private StorageFolder record_folder;//folder来读写文件夹        
        private string groupName = "A";
        private string lineName = "01";
        public YZGCMonthRecord()
        {
            this.InitializeComponent();
            timetag.Text = DateTime.Now.ToString();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is Dictionary<string, string>)
            {
                // 获取导航参数
                Dictionary<string, string> getdata = (Dictionary<string, string>)e.Parameter;
                // 显示内容
                string level = getdata["level"];
                if (level == "1")
                {
                    ApprovalPosition.Text = "员工";
                }
                else if (level == "2")
                {
                    ApprovalPosition.Text = "出班长";
                }
                else if (level == "3")
                {
                    ApprovalPosition.Text = "保全";
                }
                else if (level == "4")
                {
                    ApprovalPosition.Text = "课长";
                }
                else if (level == "5")
                {
                    ApprovalPosition.Text = "部门长";
                }
                else
                {
                    ApprovalPosition.Text = "未知";
                }
                username.Text = getdata["username"];
            }
            date.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private async void OnCommitData(object sender, RoutedEventArgs e)
        {
            // 实例化JsonObject对象
            // 设置各字段的值
            checkRecordData["checker"] = JsonValue.CreateStringValue(username.Text);
            checkRecordData["date"] = JsonValue.CreateStringValue(date.Text);
            checkRecordData["group"] = JsonValue.CreateStringValue(groupName);
            checkRecordData["line"] = JsonValue.CreateStringValue(lineName);


            if (Temp1.IsOn == true)
            {
                checkRecordData["temp1"] = JsonValue.CreateStringValue("good");
            }
            else
            {
                checkRecordData["temp1"] = JsonValue.CreateStringValue("bad");
            }
            if (Temp2.IsOn == true)
            {
                checkRecordData["temp2"] = JsonValue.CreateStringValue("good");
            }
            else
            {
                checkRecordData["temp2"] = JsonValue.CreateStringValue("bad");
            }
            if (Temp3.IsOn == true)
            {
                checkRecordData["temp3"] = JsonValue.CreateStringValue("good");
            }
            else
            {
                checkRecordData["temp3"] = JsonValue.CreateStringValue("bad");
            }
            if (Temp4.IsOn == true)
            {
                checkRecordData["temp1"] = JsonValue.CreateStringValue("good");
            }
            else
            {
                checkRecordData["temp1"] = JsonValue.CreateStringValue("bad");
            }
            if (Temp5.IsOn == true)
            {
                checkRecordData["temp2"] = JsonValue.CreateStringValue("good");
            }
            else
            {
                checkRecordData["temp2"] = JsonValue.CreateStringValue("bad");
            }
            if (Temp6.IsOn == true)
            {
                checkRecordData["temp3"] = JsonValue.CreateStringValue("good");
            }
            else
            {
                checkRecordData["temp3"] = JsonValue.CreateStringValue("bad");
            }
            if (Temp7.IsOn == true)
            {
                checkRecordData["temp3"] = JsonValue.CreateStringValue("good");
            }
            else
            {
                checkRecordData["temp3"] = JsonValue.CreateStringValue("bad");
            }
            if (Temp8.IsOn == true)
            {
                checkRecordData["temp3"] = JsonValue.CreateStringValue("good");
            }
            else
            {
                checkRecordData["temp3"] = JsonValue.CreateStringValue("bad");
            }

            checkRecordData["level2check"] = JsonValue.CreateStringValue("0");
            checkRecordData["level2date"] = JsonValue.CreateStringValue("");
            checkRecordData["level2approvaler"] = JsonValue.CreateStringValue("");
            checkRecordData["level3check"] = JsonValue.CreateStringValue("0");
            checkRecordData["level3date"] = JsonValue.CreateStringValue("");
            checkRecordData["level3approvaler"] = JsonValue.CreateStringValue("");
            checkRecordData["level4check"] = JsonValue.CreateStringValue("0");
            checkRecordData["level4date"] = JsonValue.CreateStringValue("");
            checkRecordData["level4approvaler"] = JsonValue.CreateStringValue("");
            checkRecordData["level5check"] = JsonValue.CreateStringValue("0");
            checkRecordData["level5date"] = JsonValue.CreateStringValue("");
            checkRecordData["level5approvaler"] = JsonValue.CreateStringValue("");

            // 显示JSON对象的字符串表示形式
            string jstr = checkRecordData.Stringify();
            record_folder = KnownFolders.PicturesLibrary;
            record_file = await record_folder.CreateFileAsync("ykk_YZGCMonthRecord_" + groupName + "_" + lineName + "_" + date.Text + ".ykk", CreationCollisionOption.ReplaceExisting);
            using (Stream file = await record_file.OpenStreamForWriteAsync())
            {
                using (StreamWriter write = new StreamWriter(file))
                {
                    write.Write(jstr);
                }
            }
        }

        private void OnMachineGroupChanged(object sender, RoutedEventArgs e)
        {
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
                line01.IsEnabled = true;
                line02.IsEnabled = true;
                line03.IsEnabled = true;
                line04.IsEnabled = true;
                line05.IsEnabled = true;
                line06.IsEnabled = true;
                line07.IsEnabled = true;
                line08.IsEnabled = true;
                line09.IsEnabled = true;
                line10.IsEnabled = true;
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
                line01.IsEnabled = true;
                line02.IsEnabled = true;
                line03.IsEnabled = true;
                line04.IsEnabled = true;
                line05.IsEnabled = true;
                line06.IsEnabled = true;
                line07.IsEnabled = true;
                line08.IsEnabled = true;
                line09.IsEnabled = true;
                line10.IsEnabled = true;
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
                line01.IsEnabled = true;
                line02.IsEnabled = true;
                line03.IsEnabled = true;
                line04.IsEnabled = true;
                line05.IsEnabled = true;
                line06.IsEnabled = true;
                line07.IsEnabled = true;
                line08.IsEnabled = true;
                line09.IsEnabled = true;
                line10.IsEnabled = true;
                line11.IsEnabled = true;
                line12.IsEnabled = true;
                line13.IsEnabled = true;
                line14.IsEnabled = true;
                line15.IsEnabled = true;
                line16.IsEnabled = true;
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
                line01.IsEnabled = true;
                line02.IsEnabled = true;
                line03.IsEnabled = true;
                line04.IsEnabled = true;
                line05.IsEnabled = true;
                line06.IsEnabled = true;
                line07.IsEnabled = true;
                line08.IsEnabled = true;
                line09.IsEnabled = true;
                line10.IsEnabled = true;
                line11.IsEnabled = true;
                line12.IsEnabled = true;
                line13.IsEnabled = true;
                line14.IsEnabled = true;
                line15.IsEnabled = true;
                line16.IsEnabled = true;
            }

            Temp1.IsOn = false;
            Temp2.IsOn = false;
            Temp3.IsOn = false;
            Temp4.IsOn = false;
            Temp5.IsOn = false;
            Temp6.IsOn = false;
            Temp7.IsOn = false;
            Temp8.IsOn = false;


        }

        private void OnLineChecked(object sender, RoutedEventArgs e)
        {
            if (line01.IsChecked == true)
            {
                lineName = "01";
            }
            else if (line02.IsChecked == true)
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
            Temp1.IsOn = false;
            Temp2.IsOn = false;
            Temp3.IsOn = false;
            Temp4.IsOn = false;
            Temp5.IsOn = false;
            Temp6.IsOn = false;
            Temp7.IsOn = false;
            Temp8.IsOn = false;
        }
    }
}
