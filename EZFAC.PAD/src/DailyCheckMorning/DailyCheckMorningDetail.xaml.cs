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
    public sealed partial class DailyCheckMorningDetail : Page
    {
        private JsonObject checkRecordData = new JsonObject();
        private string type = "DieCasting";
        private string checkfilename = "Unknown";
        private string checkgroup = "A";
        private string checkline = "01";
        private string checkdate = "2017-06-10";
        private string userLevel = "2";
        private string checker = "zhaoyi";
        private string authority = null;
        private CommonOperation commonOperation = new CommonOperation();
        private MessDialog messDialog = new MessDialog();
        private SolidColorBrush red = new SolidColorBrush(Colors.Red);
        JsonValue good = JsonValue.CreateStringValue("good");
        JsonValue bad = JsonValue.CreateStringValue("bad");

        public DailyCheckMorningDetail()
        {
            this.InitializeComponent();
            timetag.Text = DateTime.Now.ToString();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // 获取导航参数
            Dictionary<string, string> getdata = (Dictionary<string, string>)e.Parameter;
            // 显示内容
            ApprovalUser.Text = getdata["username"];
            MachineGroup.Text = "压轴线" + getdata["group"];
            machineNo.Text = getdata["group"] + getdata["line"];
            machineModel.Text = getdata["machineModel"];
            work.Text = getdata["work"];
            userLevel = getdata["userlevel"];
            checkgroup = getdata["group"];
            checkdate = getdata["date"];
            checkline = getdata["line"];
            authority = getdata["authority"];

            ToggleSwitch[] toggleSwitch = { first, two, three, five, six, seven, eight, nine, fourteen, fifteen, sixteen, seventeen };
            TextBox[] textBox = { four, ten, eleven, twelve };
            string[] toggleContents = { getdata["first"] , getdata["two"] , getdata["three"] , 
                                  getdata["five"] , getdata["six"] , getdata["seven"] , getdata["eight"],getdata["nine"] ,
                                  getdata["fourteen"] , getdata["fifteen"] , getdata["sixteen"] , getdata["seventeen"]
                                    };
            string[] textBoxContents = { getdata["four"], getdata["ten"] , getdata["eleven"] , getdata["twelve"] };
            string contentEdit = getdata["contentEdit"];
            // 获取职位
            ApprovalPosition.Text = commonOperation.getJobByLevel(userLevel);
            date.Text = DateTime.Now.ToString("yyyy-MM-dd");
            checkfilename = "ykk_record_" + checkgroup + "_" + checkline + "_" + checkdate + ".ykk";

            // 确定内容，并将修改的内容标红
            for (int i = 0; i < toggleContents.Length; i++)
            {
                toggleSwitch[i].IsOn = toggleContents[i] == "good";
                if (contentEdit[i] == '1')
                {
                    toggleSwitch[i].Foreground = red;
                }
            }
            for (int i = 0; i < textBoxContents.Length; i++)
            {
                textBox[i].Text = textBoxContents[i] ;
                if (contentEdit[i] == '1')
                {
                    textBox[i].Foreground = red;                       }
                }
            }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("username", ApprovalUser.Text);
            data.Add("userlevel", userLevel);
            data.Add("authority", authority);
            this.Frame.Navigate(typeof(DailyCheckNoonList), data);
        }

        private async void OnCommitData(object sender, RoutedEventArgs e)
        {
            ToggleSwitch[] toggleSwitch = { first, two, three, five, six, seven, eight, nine, fourteen, fifteen, sixteen, seventeen };
            TextBox[] textBox = { four, ten, eleven, twelve };
            List<CheckerInfoEntity> checkerList = new List<CheckerInfoEntity>();
            string oldEdit = null, newEdit = null;
            StorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync("DailyCheckNoon", CreationCollisionOption.OpenIfExists);
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
                    for (int i = 0; i < checkerInfo.Count; i++)
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
            // 设置检查信息的json信息
            checkRecordData.Add("checkInfo", commonOperation.initCheckJsonArray(type, checkgroup, checkline));

            // 设置检查内容的json信息
            JsonArray newContent = new JsonArray();
            // 用户是否修改过内容
            string flag = "0";
            for (int i = 0; i < toggleSwitch.Length; i++)
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
                    contentItem["edit"] = JsonValue.CreateStringValue(oldEdit[i] + "");
                }
                newContent.Add(contentItem);
            }
            flag = "0";
            for (int i = 0; i < textBox.Length; i++)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(textBox[i].Name);
                contentItem["status"] = JsonValue.CreateStringValue(textBox[i].Text);
                //  判断内容是否被修改,若修改则设为1，否则等于原来的值
                if (newEdit[i] == '1')
                {
                    flag = "1";
                    contentItem["edit"] = JsonValue.CreateStringValue("1");
                }
                else
                {
                    contentItem["edit"] = JsonValue.CreateStringValue(oldEdit[i] + "");
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
            commonOperation.writeJsonToFile(checkRecordData, checkfilename, KnownFolders.PicturesLibrary, "DailyCheckNoon");
            // 设置提示框
            messDialog.showDialog("审批成功！");
        }

        /*
         * 判断点检信息是否被更改并集成为字符串
        */
        private string editContnet(JsonArray content)
        {
            string edit = null;
            //  初始化内容数组
            ToggleSwitch[] toggleSwitch = { first, two, three, five, six, seven, eight, nine, fourteen, fifteen, sixteen, seventeen };
            TextBox[] textBox = { four, ten, eleven, twelve };
            for (int i = 0; i < toggleSwitch.Length; i++)
            {
                bool flag = content[i+2].GetObject()["status"].GetString().Equals("good");
                string msg = flag == toggleSwitch[i].IsOn ? "0" : "1";
                edit = edit + msg;
            }
            for (int i = 0; i < textBox.Length; i++)
            {
                String flag = content[i+13].GetObject()["status"].GetString();
                string msg = flag == textBox[i].Text ? "0" : "1";
                edit = edit + msg;
            }
            return edit;
        }
    }
}
