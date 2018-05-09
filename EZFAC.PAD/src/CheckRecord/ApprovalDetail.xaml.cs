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
        private string type = "DieCasting";
        private string checkfilename = "Unknown";
        private string checkgroup = "A";
        private string checkline = "01";
        private string checkdate = "2017-06-10";
        private string userLevel = "2";
        private string checker = "zhaoyi";
        private string folderName = null;
        private string authority = null;
        private CommonOperation commonOperation = new CommonOperation();
        private MessDialog messDialog = new MessDialog();
        private SolidColorBrush red = new SolidColorBrush(Colors.Red);
        JsonValue good = JsonValue.CreateStringValue("good");
        JsonValue bad = JsonValue.CreateStringValue("bad");

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
                authority = getdata["authority"];
                folderName = getdata["folderName"];

                ToggleSwitch[] toggleSwitch = { Temp1, Temp2, Temp3, Loop1, Loop2, Loop3, Select1, Plat1 };
                TextBlock[] toggleText = { Temp1Text, Temp2Text, Temp3Text, Loop1Text, Loop2Text, Loop3Text, Select1Text, Plat1Text };
                string[] contents = { getdata["temp1"] , getdata["temp2"] , getdata["temp3"] , getdata["loop1"],
                                      getdata["loop2"] , getdata["loop3"] , getdata["select1"] , getdata["plat1"]
                                    };
                string contentEdit = getdata["contentEdit"];
                // 获取职位
                ApprovalPosition.Text = commonOperation.getJobByLevel(userLevel);
                date.Text = DateTime.Now.ToString("yyyy-MM-dd");
                reviewInfor.Text = "";
                checkfilename = "ykk_record_" + "CheckRecord_" + checkgroup + "_" + checkline + "_" + checkdate + ".ykk";
                // 确定组和线
                judgeGroup(checkgroup);
                judgeLine(checkline);
                // 确定内容，并将修改的内容标红
                for (int i = 0; i < contentEdit.Length; i++)
                {
                    if (contentEdit[i] == '1')
                    {
                        toggleText[i].Foreground = red;
                    }
                    toggleSwitch[i].IsOn = contents[i] == "○" ? true : false;
                }
                // 获取审批信息并显示在多行Texkbox
               commonOperation.getStateText(reviewInfor, userLevel, checkfilename, folderName);
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("username", ApprovalUser.Text);
            data.Add("userlevel", userLevel);
            data.Add("authority", authority);
            data.Add("folderName", folderName);
            this.Frame.Navigate(typeof(ApprovalList), data);
        }

        private async void Confirm_Click(object sender, RoutedEventArgs e)
        {
            ToggleSwitch[] toggleSwitch = { Temp1, Temp2, Temp3, Loop1, Loop2, Loop3, Select1, Plat1 };
            List<CheckerInfoEntity> checkerList = new List<CheckerInfoEntity>();
            string oldEdit = null, newEdit = null;
            StorageFolder folder =await KnownFolders.PicturesLibrary.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            if (folder != null)
            {
                StorageFile file = await folder.CreateFileAsync(checkfilename, CreationCollisionOption.OpenIfExists);
                if (file != null)
                {
                    var jsonText = await FileIO.ReadTextAsync(file);
                    JsonObject jsonObject = JsonObject.Parse(jsonText);
                    // 获取检查信息
                    JsonArray checkInfo = jsonObject["checkInfo"].GetArray();
                    // 获取检查内容
                    JsonArray content = jsonObject["content"].GetArray();
                    // 获取用户信息
                    JsonArray checkerInfo = jsonObject["checkerInfo"].GetArray();
                    // 判断信息是否被更改并集成为字符串
                    newEdit = editContnet(content);
                    for (int i = 0; i < content.Count; i++)
                    {
                        oldEdit = oldEdit + content[i].GetObject()["edit"].GetString();
                    }
                    // 获取各级用户信息
                    for(int i = 0; i < checkerInfo.Count; i++)
                    {
                        checkerList.Add(new CheckerInfoEntity(
                                 checkerInfo[i].GetObject()["name"].GetString(),
                                 checkerInfo[i].GetObject()["level"].GetString(),
                                 checkerInfo[i].GetObject()["check"].GetString(),
                                 checkerInfo[i].GetObject()["edit"].GetString(),
                                 checkerInfo[i].GetObject()["date"].GetString(),
                                 checkerInfo[i].GetObject()["comments"].GetString()
                            ));
                    }
                }
            }
            JsonObject checkRecordData = new JsonObject();
            // 设置检查信息的json信息
            checkRecordData.Add("checkInfo", commonOperation.initCheckJsonArray(type, checkgroup, checkline));

            // 设置检查内容的json信息
            JsonArray newContent = new JsonArray();
            // 用户是否修改过内容
            string flag = "0";
            for (int i=0;i <toggleSwitch.Length;i++)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(toggleSwitch[i].Name);
                contentItem["status"] = toggleSwitch[i].IsOn == true ? good : bad;
                //  判断内容是否被修改,若修改则设为1，否则等于原来的值
                if (newEdit[i] == '1')
                {
                    flag = "1";
                    contentItem["edit"] = JsonValue.CreateStringValue("1");
                }
                else
                {
                    contentItem["edit"] = JsonValue.CreateStringValue(oldEdit[i]+"");
                }
                newContent.Add(contentItem);
            }
            checkRecordData.Add("content", newContent);

            // 设置各级别用户的json信息
            JsonArray newCheckerInfo = new JsonArray();
            for (int i = 0; i < checkerList.Count; i++)
            {
                string level = Convert.ToString(i + 1);
                JsonObject item = new JsonObject();
                item["name"] = level == userLevel ? JsonValue.CreateStringValue(ApprovalUser.Text) : JsonValue.CreateStringValue(checkerList[i].name);
                item["level"] = level == userLevel ? JsonValue.CreateStringValue(level) : JsonValue.CreateStringValue(checkerList[i].level);
                item["check"] = level == userLevel ? JsonValue.CreateStringValue("1") : JsonValue.CreateStringValue(checkerList[i].check);
                item["edit"] = level == userLevel ? JsonValue.CreateStringValue(flag) : JsonValue.CreateStringValue(checkerList[i].edit);
                item["date"] = level == userLevel ? JsonValue.CreateStringValue(date.Text) : JsonValue.CreateStringValue(checkerList[i].date);
                item["comments"] = level == userLevel ? JsonValue.CreateStringValue("") : JsonValue.CreateStringValue(checkerList[i].comments);
                newCheckerInfo.Add(item);
            }
            checkRecordData.Add("checkerInfo", newCheckerInfo);
            // 将json数据写入对应文件中
            commonOperation.writeJsonToFile(checkRecordData, checkfilename, KnownFolders.PicturesLibrary, folderName);
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

        /*
         * 确定机组
        */
        private void judgeGroup(string checkgroup)
        {
            //  初始化线数组
            RadioButton[] radioButton = { line01,line02,line03,line04,line05,line06,line07,line08,line09,
                                          line10,line11,line12,line13,line14,line15,line16,line17,line18
                                        };
            //  根据机组设置机番名称
            for (int i = 0; i < radioButton.Length; i++)
            {
                //  若（i+1）为个位数则在前面加个0
                string num = (i + 1) < 10 ? "0" + Convert.ToString(i + 1) : Convert.ToString(i + 1);
                radioButton[i].Content = checkgroup + "-" + num;
                radioButton[i].IsEnabled = false;
            }
            //  设置机组
            groupa.IsEnabled = false;
            groupa.IsChecked = false;
            groupb.IsEnabled = false;
            groupb.IsChecked = false;
            groupc.IsEnabled = false;
            groupc.IsChecked = false;
            groupd.IsEnabled = false;
            groupd.IsChecked = false;
            if (checkgroup == "A")
            {
                groupa.IsEnabled = true;
                groupa.IsChecked = true;
            }
            else if (checkgroup == "B")
            {
                groupb.IsEnabled = true;
                groupb.IsChecked = true;
            }
            else if (checkgroup == "C")
            {
                groupc.IsEnabled = true;
                groupc.IsChecked = true;
            }
            else if (checkgroup == "D")
            {
                groupd.IsEnabled = true;
                groupd.IsChecked = true;
            }
        }

        /*
         * 确定机番
        */
        private void judgeLine(string checkline)
        {
            //  初始化线数组
            RadioButton[] radioButton = { line01,line02,line03,line04,line05,line06,line07,line08,line09,
                                          line10,line11,line12,line13,line14,line15,line16,line17,line18
                                        };
            for (int i = 0; i < radioButton.Length; i++)
            {
                //  若（i+1）为个位数则在前面加个0
                string num = (i + 1) < 10 ? "0" + Convert.ToString(i + 1) : Convert.ToString(i + 1);
                if(num == checkline)
                {
                    radioButton[i].IsEnabled = true;
                    radioButton[i].IsChecked = true;
                }
            }
        }

        /*
         * 判断点检信息是否被更改并集成为字符串
        */
        private string editContnet(JsonArray content)
        {
            string edit = null;
            //  初始化内容数组
            ToggleSwitch[] toggleSwitch = { Temp1, Temp2, Temp3, Loop1, Loop2, Loop3, Select1, Plat1 };
            for(int i = 0; i < toggleSwitch.Length; i++)
            {
                bool flag = content[i].GetObject()["status"].GetString().Equals("good");
                string msg = flag == toggleSwitch[i].IsOn ? "0" : "1";
                edit = edit + msg;
            }
            return edit;
        }
    }
}
