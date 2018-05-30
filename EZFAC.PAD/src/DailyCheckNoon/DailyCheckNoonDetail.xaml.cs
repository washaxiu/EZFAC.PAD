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
using System.Text.RegularExpressions;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EZFAC.PAD
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DailyCheckNoonDetail : Page
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
        private string folderName = null;
        private CommonOperation commonOperation = new CommonOperation();
        private MessDialog messDialog = new MessDialog();
        private SolidColorBrush red = new SolidColorBrush(Colors.Red);
        private SolidColorBrush black = new SolidColorBrush(Colors.Black);
        JsonValue good = JsonValue.CreateStringValue("good");
        JsonValue bad = JsonValue.CreateStringValue("bad");

        public DailyCheckNoonDetail()
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
            folderName = getdata["folderName"];

            ToggleSwitch[] toggleSwitch = { first, two, three, five, six, seven, eight, nine, fourteen, fifteen, sixteen, seventeen };
            TextBox[] textBox = { four, ten, eleven, twelve };
            TextBlock[] toggleText = { firstText, twoText, threeText, fiveText, sixText, sevenText, eightText, nineText, fourteenText, fifteenText, sixteenText, seventeenText };
            TextBlock[] textBlock = { fourText, tenText, elevenText, twelveText };
            string[] toggleContents = { getdata["first"] , getdata["two"] , getdata["three"] ,
                                  getdata["five"] , getdata["six"] , getdata["seven"] , getdata["eight"],getdata["nine"] ,
                                  getdata["fourteen"] , getdata["fifteen"] , getdata["sixteen"] , getdata["seventeen"]
                                    };
            string[] textBoxContents = { getdata["four"], getdata["ten"], getdata["eleven"], getdata["twelve"] };
            string contentEdit = getdata["contentEdit"];
            // 获取职位
            ApprovalPosition.Text = commonOperation.getJobByLevel(userLevel);
            date.Text = DateTime.Now.ToString("yyyy-MM-dd");
            checkfilename = "ykk_record_" + "DailyCheckNoon_" + checkgroup + "_" + checkline + "_" + checkdate + ".ykk";

            // 确定内容，并将修改的内容标红
            for (int i = 0; i < toggleContents.Length; i++)
            {
                toggleSwitch[i].IsOn = toggleContents[i] == "good";
                if (contentEdit[i] == '1')
                {
                    toggleText[i].Foreground = red;
                }
            }
            for (int i = 0; i < textBoxContents.Length; i++)
            {
                textBox[i].Text = textBoxContents[i];
                if (contentEdit[i + 12] == '1')
                {
                    textBlock[i].Foreground = red;

                }

            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("username", ApprovalUser.Text);
            data.Add("userlevel", userLevel);
            data.Add("authority", authority);
            data.Add("folderName", folderName);
            this.Frame.Navigate(typeof(DailyCheckNoonList), data);
        }

        private async void OnCommitData(object sender, RoutedEventArgs e)
        {
            JsonObject checkRecordData = new JsonObject();
            ToggleSwitch[] toggleSwitch = { first, two, three, five, six, seven, eight, nine, fourteen, fifteen, sixteen, seventeen };
            TextBox[] textBox = { four, ten, eleven, twelve };
            TextBlock[] textBlock = { fourText, tenText, elevenText, twelveText };
            bool flagRequiredFields = true;
            string tips = "";
            for (int i = 0; i < textBox.Length; i++)
            {
                textBlock[i].Foreground = black;
                if ("".Equals(textBox[i].Text.Trim()))
                {
                    tips = "必填项不能为空！";
                    textBlock[i].Foreground = red;
                    flagRequiredFields = false;
                }
                else if (!Regex.IsMatch(textBox[i].Text, @"^(\-|\+)?\d+(\.\d+)?$"))
                {
                    tips = "输入必须为数字!  ";
                    textBlock[i].Foreground = red;
                    flagRequiredFields = false;
                }
            }
            if (!flagRequiredFields)
            {
                messDialog.showDialog(tips);
                return;
            }
            List<CheckerInfoEntity> checkerList = new List<CheckerInfoEntity>();
            string oldEdit = null, newEdit = null;
            StorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
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
                    for (int i = 2; i < content.Count; i++)
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

            // 设置机器的型号以及稼动情况  审批时这两个字段不可更改
            JsonObject contentItem1 = new JsonObject();
            contentItem1["name"] = JsonValue.CreateStringValue(machineModel.Name);
            contentItem1["status"] = JsonValue.CreateStringValue(machineModel.Text.ToString());
            contentItem1["edit"] = JsonValue.CreateStringValue("0");
            newContent.Add(contentItem1);

            JsonObject contentItem2 = new JsonObject();
            contentItem2["name"] = JsonValue.CreateStringValue(work.Name);
            contentItem2["status"] = JsonValue.CreateStringValue(work.Text.ToString());
            contentItem2["edit"] = JsonValue.CreateStringValue("0");
            newContent.Add(contentItem2);

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
            for (int i = 0; i < textBox.Length; i++)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(textBox[i].Name);
                contentItem["status"] = JsonValue.CreateStringValue(textBox[i].Text);
                //  判断内容是否被修改,若修改则设为1，否则等于原来的值
                if (newEdit[i+12] == '1')
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
            commonOperation.writeJsonToFile(checkRecordData, checkfilename, KnownFolders.PicturesLibrary, folderName);
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
                String flag = content[i+14].GetObject()["status"].GetString();
                string msg = flag == textBox[i].Text ? "0" : "1";
                edit = edit + msg;
            }
            return edit;
        }

        // 点击查看大图
        private async void Image_Click(object sender, PointerRoutedEventArgs e)
        {
            Image image = new Image();
            Image image1 = (Image)sender;
            image.Source = image1.Source;
            ContentDialog showImage = new ContentDialog()
            {
                Title = "消息提示",
                Content = image,
                PrimaryButtonText = "确定",
                FullSizeDesired = false
            };
            await showImage.ShowAsync();
        }
    }
}
