using System;
using System.Text;
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
using EZFAC.PAD.src.Model;
using Windows.UI;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EZFAC.PAD
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ApprovalDetail : Page
    {
        private JsonObject checkRecordData = new JsonObject();
        private StorageFile record_file;//UWP 采用StorageFile来读写文件
        private StorageFolder record_folder;//folder来读写文件夹        
        private string checkfilename = "Unknown";
        private string checkgroup = "A";
        private string checkline = "01";
        private string checkdate = "2017-06-10";
        private string userLevel = "2";
        private string checker = "zhaoyi";
        private CommonOperation commonOperation = new CommonOperation();
        private MessDialog messDialog = new MessDialog();

        public ApprovalDetail()
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
                ApprovalUser.Text = getdata["username"];
                userLevel = getdata["userlevel"];
                checkdate = getdata["date"];
                checker = getdata["checker"];
                checkgroup = getdata["group"];
                checkline = getdata["line"];

                string ltemp1 = getdata["temp1"];
                string ltemp2 = getdata["temp2"];
                string ltemp3 = getdata["temp3"];
                string lloop1 = getdata["loop1"];
                string lloop2 = getdata["loop2"];
                string lloop3 = getdata["loop3"];
                string lselect1 = getdata["select1"];
                string lplat1 = getdata["plat1"];
                string editContent = getdata["editContent"];

                // 获取职位
                ApprovalPosition.Text = commonOperation.getJobByLevel(userLevel);
                date.Text = DateTime.Now.ToString("yyyy-MM-dd");
                reviewInfor.Text = "";
                checkfilename = "ykk_record_" + checkgroup + "_" + checkline + "_" + checkdate + ".ykk";

                // 确定组和线
                judgeGroup(checkgroup);
                judgeLine(checkline);

                Temp1.IsOn = ltemp1 == "○" ? true : false;
                Temp2.IsOn = ltemp2 == "○" ? true : false;
                Temp3.IsOn = ltemp3 == "○" ? true : false;

                Loop1.IsOn = lloop1 == "○" ? true : false;
                Loop2.IsOn = lloop2 == "○" ? true : false;
                Loop3.IsOn = lloop3 == "○" ? true : false;

                Select1.IsOn = lselect1 == "○" ? true : false;
                Plat1.IsOn = lplat1 == "○" ? true : false;

                // 获取审批信息并显示在多行Texkbox
                commonOperation.getStateText(reviewInfor, userLevel, checkfilename, "PointCheck");

                // 将上一级用户修改的内容标红

                string[] strs = editContent.Split('+');
                for (int i = 1; i < strs.Length; i++)
                {
                    SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Red);
                    if (strs[i] == "temp1") Temp1Text.Foreground = solidColorBrush;
                    if (strs[i] == "temp2") Temp2Text.Foreground = solidColorBrush;
                    if (strs[i] == "temp3") Temp3Text.Foreground = solidColorBrush;
                    if (strs[i] == "loop1") Loop1Text.Foreground = solidColorBrush;
                    if (strs[i] == "loop2") Loop2Text.Foreground = solidColorBrush;
                    if (strs[i] == "loop3") Loop3Text.Foreground = solidColorBrush;
                    if (strs[i] == "select1") Select1Text.Foreground = solidColorBrush;
                    if (strs[i] == "plat1") Plat1Text.Foreground = solidColorBrush;
                }
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("username", ApprovalPosition.Text);
            data.Add("level", userLevel);
            this.Frame.Navigate(typeof(ApprovalList), data);
        }

        private async void Confirm_Click(object sender, RoutedEventArgs e)
        {
            JsonValue good = JsonValue.CreateStringValue("good");
            JsonValue bad = JsonValue.CreateStringValue("bad");
            PointCheck pointCheck = new PointCheck();
            string edit = "0";

            StorageFolder folder =await KnownFolders.PicturesLibrary.CreateFolderAsync("PointCheck",CreationCollisionOption.OpenIfExists);
            if (folder != null)
            {
                // 获取目录下的文件列表
                var subFiles = await folder.GetFilesAsync();
                // 向ListView中添加文件
                foreach (StorageFile file in subFiles)
                {
                    string extensionname = Path.GetExtension(file.Name);
                    if (file.Name == checkfilename)
                    {
                        string jsonText = await FileIO.ReadTextAsync(file);
                        JsonObject jsonObject = JsonObject.Parse(jsonText);

                        // 判断点检信息是否被更改
                        edit = editContnet(jsonObject);

                        pointCheck.level2check = jsonObject["level2check"].GetString();
                        pointCheck.level2edit = jsonObject["level2edit"].GetString();
                        pointCheck.level2approvaler = jsonObject["level2approvaler"].GetString();
                        pointCheck.level2date = jsonObject["level2date"].GetString();
                        pointCheck.level3check = jsonObject["level3check"].GetString();
                        pointCheck.level3edit = jsonObject["level3edit"].GetString();
                        pointCheck.level3approvaler = jsonObject["level3approvaler"].GetString();
                        pointCheck.level3date = jsonObject["level3date"].GetString();
                        pointCheck.level4check = jsonObject["level4check"].GetString();
                        pointCheck.level4edit = jsonObject["level4edit"].GetString();
                        pointCheck.level4approvaler = jsonObject["level4approvaler"].GetString();
                        pointCheck.level4date = jsonObject["level4date"].GetString();
                        pointCheck.level5check = jsonObject["level5check"].GetString();
                        pointCheck.level5edit = jsonObject["level5edit"].GetString();
                        pointCheck.level5approvaler = jsonObject["level5approvaler"].GetString();
                        pointCheck.level5date = jsonObject["level5date"].GetString();
                    }
                }
            }

            // 实例化JsonObject对象
            // 设置各字段的值

            checkRecordData["checker"] = JsonValue.CreateStringValue(checker);
            checkRecordData["date"] = JsonValue.CreateStringValue(checkdate);
            checkRecordData["group"] = JsonValue.CreateStringValue(checkgroup);
            checkRecordData["line"] = JsonValue.CreateStringValue(checkline);

            checkRecordData["temp1"] = Temp1.IsOn == true ? good : bad;
            checkRecordData["temp2"] = Temp2.IsOn == true ? good : bad;
            checkRecordData["temp3"] = Temp3.IsOn == true ? good : bad;
            checkRecordData["loop1"] = Loop1.IsOn == true ? good : bad;
            checkRecordData["loop2"] = Loop2.IsOn == true ? good : bad;
            checkRecordData["loop3"] = Loop3.IsOn == true ? good : bad;
            checkRecordData["select1"] = Select1.IsOn == true ? good : bad;
            checkRecordData["plat1"] = Plat1.IsOn == true ? good : bad;

            if (userLevel == "2")
            {
                pointCheck.level2check = "1";
                pointCheck.level2edit = edit;
                pointCheck.level2date = date.Text;
                pointCheck.level2approvaler = ApprovalUser.Text;
                
            }
            else if (userLevel == "3")
            {
                pointCheck.level3check = "1";
                pointCheck.level3edit = edit;
                pointCheck.level3date = date.Text;
                pointCheck.level3approvaler = ApprovalUser.Text;
            }
            else if (userLevel == "4")
            {
                pointCheck.level4check = "1";
                pointCheck.level4edit = edit;
                pointCheck.level4date = date.Text;
                pointCheck.level4approvaler = ApprovalUser.Text;
            }
            else if (userLevel == "5")
            {
                pointCheck.level5check = "1";
                pointCheck.level5edit = edit;
                pointCheck.level5date = date.Text;
                pointCheck.level5approvaler = ApprovalUser.Text;
            }
            checkRecordData["level2check"] = JsonValue.CreateStringValue(pointCheck.level2check);
            checkRecordData["level2edit"] = JsonValue.CreateStringValue(pointCheck.level2edit);
            checkRecordData["level2date"] = JsonValue.CreateStringValue(pointCheck.level2date);
            checkRecordData["level2approvaler"] = JsonValue.CreateStringValue(pointCheck.level2approvaler);
            checkRecordData["level3check"] = JsonValue.CreateStringValue(pointCheck.level3check);
            checkRecordData["level3edit"] = JsonValue.CreateStringValue(pointCheck.level3edit);
            checkRecordData["level3date"] = JsonValue.CreateStringValue(pointCheck.level3date);
            checkRecordData["level3approvaler"] = JsonValue.CreateStringValue(pointCheck.level3approvaler);
            checkRecordData["level4check"] = JsonValue.CreateStringValue(pointCheck.level4check);
            checkRecordData["level4edit"] = JsonValue.CreateStringValue(pointCheck.level4edit);
            checkRecordData["level4date"] = JsonValue.CreateStringValue(pointCheck.level4date);
            checkRecordData["level4approvaler"] = JsonValue.CreateStringValue(pointCheck.level4approvaler);
            checkRecordData["level5check"] = JsonValue.CreateStringValue(pointCheck.level5check);
            checkRecordData["level5edit"] = JsonValue.CreateStringValue(pointCheck.level5edit);
            checkRecordData["level5date"] = JsonValue.CreateStringValue(pointCheck.level5date);
            checkRecordData["level5approvaler"] = JsonValue.CreateStringValue(pointCheck.level5approvaler);

            // 将json数据写入对应文件中
            commonOperation.writeJsonToFile(checkRecordData, checkfilename, KnownFolders.PicturesLibrary, "PointCheck");
            // 设置提示框
            messDialog.showDialog("审批成功！");
        }

        private void OnMachineGroupChanged(object sender, RoutedEventArgs e)
        {
        }

        private void OnLineChecked(object sender, RoutedEventArgs e)
        {
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

        private void Select4_Toggled(object sender, RoutedEventArgs e)
        {
            Select1.IsOn = Select4.IsOn;
        }

        private void Plat4_Toggled(object sender, RoutedEventArgs e)
        {
            Plat1.IsOn = Plat4.IsOn;
        }

        
        private void judgeGroup(string checkgroup)
        {
            if (checkgroup == "A")
            {
                groupa.IsEnabled = true;
                groupa.IsChecked = true;
                groupb.IsEnabled = false;
                groupb.IsChecked = false;
                groupc.IsEnabled = false;
                groupc.IsChecked = false;
                groupd.IsEnabled = false;
                groupd.IsChecked = false;
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
                line01.IsEnabled = false;
                line02.IsEnabled = false;
                line03.IsEnabled = false;
                line04.IsEnabled = false;
                line05.IsEnabled = false;
                line06.IsEnabled = false;
                line07.IsEnabled = false;
                line08.IsEnabled = false;
                line09.IsEnabled = false;
                line10.IsEnabled = false;
                line11.IsEnabled = false;
                line12.IsEnabled = false;
                line13.IsEnabled = false;
                line14.IsEnabled = false;
                line15.IsEnabled = false;
                line16.IsEnabled = false;
            }
            else if (checkgroup == "B")
            {
                groupa.IsEnabled = false;
                groupa.IsChecked = false;
                groupb.IsEnabled = true;
                groupb.IsChecked = true;
                groupc.IsEnabled = false;
                groupc.IsChecked = false;
                groupd.IsEnabled = false;
                groupd.IsChecked = false;
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
                line01.IsEnabled = false;
                line02.IsEnabled = false;
                line03.IsEnabled = false;
                line04.IsEnabled = false;
                line05.IsEnabled = false;
                line06.IsEnabled = false;
                line07.IsEnabled = false;
                line08.IsEnabled = false;
                line09.IsEnabled = false;
                line10.IsEnabled = false;
                line11.IsEnabled = false;
                line12.IsEnabled = false;
                line13.IsEnabled = false;
                line14.IsEnabled = false;
                line15.IsEnabled = false;
                line16.IsEnabled = false;
            }
            else if (checkgroup == "C")
            {
                groupa.IsEnabled = false;
                groupa.IsChecked = false;
                groupb.IsEnabled = false;
                groupb.IsChecked = false;
                groupc.IsEnabled = true;
                groupc.IsChecked = true;
                groupd.IsEnabled = false;
                groupd.IsChecked = false;
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
                line01.IsEnabled = false;
                line02.IsEnabled = false;
                line03.IsEnabled = false;
                line04.IsEnabled = false;
                line05.IsEnabled = false;
                line06.IsEnabled = false;
                line07.IsEnabled = false;
                line08.IsEnabled = false;
                line09.IsEnabled = false;
                line10.IsEnabled = false;
                line11.IsEnabled = false;
                line12.IsEnabled = false;
                line13.IsEnabled = false;
                line14.IsEnabled = false;
                line15.IsEnabled = false;
                line16.IsEnabled = false;
            }
            else if (checkgroup == "D")
            {
                groupa.IsEnabled = false;
                groupa.IsChecked = false;
                groupb.IsEnabled = false;
                groupb.IsChecked = false;
                groupc.IsEnabled = false;
                groupc.IsChecked = false;
                groupd.IsEnabled = true;
                groupd.IsChecked = true;
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
                line01.IsEnabled = false;
                line02.IsEnabled = false;
                line03.IsEnabled = false;
                line04.IsEnabled = false;
                line05.IsEnabled = false;
                line06.IsEnabled = false;
                line07.IsEnabled = false;
                line08.IsEnabled = false;
                line09.IsEnabled = false;
                line10.IsEnabled = false;
                line11.IsEnabled = false;
                line12.IsEnabled = false;
                line13.IsEnabled = false;
                line14.IsEnabled = false;
                line15.IsEnabled = false;
                line16.IsEnabled = false;
            }
        }

        private void judgeLine(string checkline)
        {
            if (checkline == "01")
            {
                line01.IsEnabled = true;
                line01.IsChecked = true;
            }
            else if (checkline == "02")
            {
                line02.IsEnabled = true;
                line02.IsChecked = true;
            }
            else if (checkline == "03")
            {
                line03.IsEnabled = true;
                line03.IsChecked = true;
            }
            else if (checkline == "04")
            {
                line04.IsEnabled = true;
                line04.IsChecked = true;
            }
            else if (checkline == "05")
            {
                line05.IsEnabled = true;
                line05.IsChecked = true;
            }
            else if (checkline == "06")
            {
                line06.IsEnabled = true;
                line06.IsChecked = true;
            }
            else if (checkline == "07")
            {
                line07.IsEnabled = true;
                line07.IsChecked = true;
            }
            else if (checkline == "08")
            {
                line08.IsEnabled = true;
                line08.IsChecked = true;
            }
            else if (checkline == "09")
            {
                line09.IsEnabled = true;
                line09.IsChecked = true;
            }
            else if (checkline == "10")
            {
                line10.IsEnabled = true;
                line10.IsChecked = true;
            }
            else if (checkline == "11")
            {
                line11.IsEnabled = true;
                line11.IsChecked = true;
            }
            else if (checkline == "12")
            {
                line12.IsEnabled = true;
                line12.IsChecked = true;
            }
            else if (checkline == "13")
            {
                line13.IsEnabled = true;
                line13.IsChecked = true;
            }
            else if (checkline == "14")
            {
                line14.IsEnabled = true;
                line14.IsChecked = true;
            }
            else if (checkline == "15")
            {
                line15.IsEnabled = true;
                line15.IsChecked = true;
            }
            else if (checkline == "16")
            {
                line16.IsEnabled = true;
                line16.IsChecked = true;
            }
        }

        /*
         * 判断点检信息是否被更改
        */
        private string editContnet(JsonObject jsonObject)
        {
            string edit = "0";
            bool temp1 = jsonObject["temp1"].GetString().Equals("good");
            bool temp2 = jsonObject["temp2"].GetString().Equals("good");
            bool temp3 = jsonObject["temp3"].GetString().Equals("good");
            bool loop1 = jsonObject["loop1"].GetString().Equals("good");
            bool loop2 = jsonObject["loop2"].GetString().Equals("good");
            bool loop3 = jsonObject["loop3"].GetString().Equals("good");
            bool select1 = jsonObject["select1"].GetString().Equals("good");
            bool plat1 = jsonObject["plat1"].GetString().Equals("good");

            if (temp1 != Temp1.IsOn) edit = edit + "+temp1";
            if (temp2 != Temp2.IsOn) edit = edit + "+temp2";
            if (temp3 != Temp3.IsOn) edit = edit + "+temp3";
            if (loop1 != Loop1.IsOn) edit = edit + "+loop1";
            if (loop1 != Loop1.IsOn) edit = edit + "+loop2";
            if (loop1 != Loop1.IsOn) edit = edit + "+loop3";
            if (select1 != Select1.IsOn) edit = edit + "+select1";
            if (plat1 != Plat1.IsOn) edit = edit + "+plat1";

            return edit;
        }
    }
}
