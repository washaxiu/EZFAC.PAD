using EZFAC.PAD.src.Model;
using EZFAC.PAD.src.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EZFAC.PAD.src.MaintenanceLog
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MaintenanceLogDetail : Page
    {
        private JsonObject checkRecordData = new JsonObject();
        private string type = "MaintenanceLog";
        private string checkfilename = "Unknown";
        private string checkjifan = "";
        private string checkpinming = "";
        private string checkdate = "2017-06-10";
        private string userLevel = "2";
        private string checker = "zhaoyi";
        private string authority = null;
        private string folderName = null;
        private CommonOperation commonOperation = new CommonOperation();
        private MessDialog messDialog = new MessDialog();
        private SolidColorBrush red = new SolidColorBrush(Colors.Red);
        public MaintenanceLogDetail()
        {
            this.InitializeComponent();
            TimeLog.Text = DateTime.Now.ToString();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is Dictionary<string, string>)
            {
                // 获取导航参数
                Dictionary<string, string> getdata = (Dictionary<string, string>)e.Parameter;
                // 显示内容
                username.Text = getdata["username"];
                userLevel = getdata["userlevel"];

                // 获取职位
                position.Text = commonOperation.getJobByLevel(userLevel);

                date.Text = DateTime.Now.ToString("yyyy-MM-dd");
                jifan.Text = getdata["jifan"];
                pinming.Text = getdata["pinming"];
                SHOT.Text = getdata["SHOT"];
                folderName = getdata["folderName"];
                //element1.SelectedValue = getdata["record1"];
                //element2.SelectedValue = getdata["record2"];
                //element3.SelectedValue = getdata["record3"];
                //element4.SelectedValue = getdata["record4"];
                //element5.SelectedValue = getdata["record5"];
                //element6.SelectedValue = getdata["record6"];
                //element7.SelectedValue = getdata["record7"];
                //element8.SelectedValue = getdata["record8"];
                //element9.SelectedValue = getdata["record9"];
                //element10.SelectedValue = getdata["record10"];
                //element11.SelectedValue = getdata["record11"];
                //element12.SelectedValue = getdata["record12"].ToString();
                string[] value = { getdata["record1"], getdata["record2"],getdata["record3"],getdata["record4"],getdata["record5"],getdata["record6"],getdata["record7"],getdata["record8"],getdata["record9"],getdata["record10"],getdata["record11"],getdata["record12"]};
                //得到部品下拉框内容
                String productNum = getProductNum();
                String[] productNameArray = Regex.Split(productNum, ",", RegexOptions.IgnoreCase);

                ComboBox[] comboBox = { element1, element2, element3, element4, element5, element6, element7, element8, element9, element10, element11, element12 };
                for (int i = 0; i < comboBox.Length; i++)
                {
                    comboBox[i].ItemsSource = productNameArray;
                    string j = value[i].Substring(0, value[i].Length-1).ToString();
                    comboBox[i].SelectedIndex = int.Parse(j)-1;
                    
                }

                String[] reason = Regex.Split(getdata["maintainReason"], "_", RegexOptions.IgnoreCase);
                CheckBox[] checkbox = { maintain_A, maintain_B, maintain_C, maintain_D, maintain_E, maintain_F, maintain_G, maintain_H, maintain_J, maintain_K, maintain_M, maintain_N, maintain_P, maintain_S, maintain_T, maintain_U };
                for (int i = 0; i < reason.Length; i++)
                {
                    for (int j = 0; j < checkbox.Length; j++)
                    {
                        if ((reason[i]).Equals(checkbox[j].Content))
                        {
                            checkbox[j].IsChecked = true;
                            break;
                        }
                    }
                }

                reviewInfor.Text = getdata["content"];
                MaintenResult.SelectedValue = getdata["result"];

                checkdate = getdata["date"];
                checker = getdata["CheckerName"];

                authority = getdata["authority"];
                string contentEdit = getdata["ContentEdit"];

                checkfilename = "ykk_record_" + "MaintenanceLog_" + getdata["jifan"] + "_" + getdata["pinming"] + "_" + checkdate + ".ykk";
                // 确定组和线
                //judgeGroup(checkgroup);
                // judgeLine(checkline);
                // 确定内容，并将修改的内容标红  
                for (int i = 0; i < contentEdit.Length; i++)
                {
                    if (contentEdit[i] == '1')
                    {
                        changeColor(i);
                    }
                }

                // 获取审批信息并显示在多行Texkbox
                commonOperation.getStateText(approvalreviewInfor, userLevel, checkfilename, folderName);
            }
        }
        private void back_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("username", username.Text);
            data.Add("userlevel", userLevel);
            data.Add("authority", authority);
            data.Add("folderName", folderName);
            this.Frame.Navigate(typeof(MaintenanceLogList), data);
        }

        private async void Confirm_Click(object sender, RoutedEventArgs e)
        {
            JsonObject checkRecordData = new JsonObject();
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
            checkRecordData.Add("checkInfo", commonOperation.initCheckJsonArray(type, jifan.Text, pinming.Text, SHOT.Text));

            // 设置检查内容的json信息
            JsonArray newContent = new JsonArray();
            // 用户是否修改过内容

            string flag = "0";
            List<string> elemenText = getElement();
            List<string> elementTagText = getElementTag();
            for (int i = 0; i < 12; i++)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(elementTagText[i]);
                contentItem["status"] = JsonValue.CreateStringValue(elemenText[i]);
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

            JsonObject contentItem1 = new JsonObject();
            CheckBox[] checkbox = { maintain_A, maintain_B, maintain_C, maintain_D, maintain_E, maintain_F, maintain_G, maintain_H, maintain_J, maintain_K, maintain_M, maintain_N, maintain_P, maintain_S, maintain_T, maintain_U };
            String reason = "";
            for (int i = 0; i < checkbox.Length; i++)
            {
                if (checkbox[i].IsChecked == true)
                {
                    reason += checkbox[i].Content;
                    reason += "_";
                }
            }
            contentItem1["name"] = JsonValue.CreateStringValue("maintainReason");
            contentItem1["status"] = JsonValue.CreateStringValue(reason);
            if (newEdit[12] == '1')
            {
                flag = "1";
                contentItem1["edit"] = JsonValue.CreateStringValue("1");
            }
            else
            {
                contentItem1["edit"] = JsonValue.CreateStringValue(oldEdit[12] + "");
            }
            newContent.Add(contentItem1);

            contentItem1 = new JsonObject();
            contentItem1["name"] = JsonValue.CreateStringValue("reviewInfor");
            contentItem1["status"] = JsonValue.CreateStringValue(reviewInfor.Text);

            if (newEdit[13] == '1')
            {
                flag = "1";
                contentItem1["edit"] = JsonValue.CreateStringValue("1");
            }
            else
            {
                contentItem1["edit"] = JsonValue.CreateStringValue(oldEdit[13] + "");
            }
            newContent.Add(contentItem1);

            contentItem1 = new JsonObject();
            contentItem1["name"] = JsonValue.CreateStringValue("MaintainResult");
            contentItem1["status"] = JsonValue.CreateStringValue(MaintenResult.SelectedValue.ToString());
            if (newEdit[14] == '1')
            {
                flag = "1";
                contentItem1["edit"] = JsonValue.CreateStringValue("1");
            }
            else
            {
                contentItem1["edit"] = JsonValue.CreateStringValue(oldEdit[14] + "");
            }
            newContent.Add(contentItem1);

            checkRecordData.Add("content", newContent);



            // 设置各级别用户的json信息
            JsonArray newCheckerInfo = new JsonArray();
            for (int i = 0; i < checkerList.Count; i++)
            {
                string level = Convert.ToString(i + 1);
                JsonObject item = new JsonObject();
                item["name"] = level == userLevel ? JsonValue.CreateStringValue(username.Text) : JsonValue.CreateStringValue(checkerList[i].name);
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
            List<string> examInfo = getElement();
            for (int i = 0; i < 12; i++)
            {
                string msg1 = content[i].GetObject()["status"].GetString() == examInfo[i] ? "0" : "1";
                edit = edit + msg1;
            }
            String[] reason1 = Regex.Split(content[12].GetObject()["status"].GetString(), ";", RegexOptions.IgnoreCase);
            CheckBox[] checkbox = { maintain_A, maintain_B, maintain_C, maintain_D, maintain_E, maintain_F, maintain_G, maintain_H, maintain_J, maintain_K, maintain_M, maintain_N, maintain_P, maintain_S, maintain_T, maintain_U };
            String msg = "0";
            for (int i = 0; i < checkbox.Length; i++)
            {
                for (int j = 0; j < reason1.Length; j++)
                {
                    if (checkbox[i].Content.Equals(reason1[j]))
                    {
                        if (checkbox[i].IsChecked == false)
                        {
                            msg = "1";
                        }
                        else
                        {
                            msg = "0";
                        }
                        break;
                    }
                    else
                    {
                        if (checkbox[i].IsChecked == false)
                        {
                            msg = "0";
                        }
                        else
                        {
                            msg = "1";
                            break;
                        }
                    }
                }
                if (msg == "1")
                {
                    break;
                }
            }
            edit = edit + msg;
            msg = content[13].GetObject()["status"].GetString() == reviewInfor.Text ? "0" : "1";
            edit = edit + msg;
            msg = content[14].GetObject()["status"].GetString() == MaintenResult.SelectedValue.ToString() ? "0" : "1";
            edit = edit + msg;
            return edit;
        }

        private List<String> getElement()
        {
            List<String> elementList = new List<string>();
            elementList.Add(element1.SelectedValue.ToString());
            elementList.Add(element2.SelectedValue.ToString());
            elementList.Add(element3.SelectedValue.ToString());
            elementList.Add(element4.SelectedValue.ToString());
            elementList.Add(element5.SelectedValue.ToString());
            elementList.Add(element6.SelectedValue.ToString());
            elementList.Add(element7.SelectedValue.ToString());
            elementList.Add(element8.SelectedValue.ToString());
            elementList.Add(element9.SelectedValue.ToString());
            elementList.Add(element10.SelectedValue.ToString());
            elementList.Add(element11.SelectedValue.ToString());
            elementList.Add(element12.SelectedValue.ToString());

            return elementList;
        }
        private List<String> getElementTag()
        {
            List<String> elementNumTag = new List<string>();
            elementNumTag.Add(element1Tag.Text);
            elementNumTag.Add(element2Tag.Text);
            elementNumTag.Add(element3Tag.Text);
            elementNumTag.Add(element4Tag.Text);
            elementNumTag.Add(element5Tag.Text);
            elementNumTag.Add(element6Tag.Text);
            elementNumTag.Add(element7Tag.Text);
            elementNumTag.Add(element8Tag.Text);
            elementNumTag.Add(element9Tag.Text);
            elementNumTag.Add(element10Tag.Text);
            elementNumTag.Add(element11Tag.Text);
            elementNumTag.Add(element12Tag.Text);
            return elementNumTag;
        }
        private void changeColor(int i)
        {
            if (i == 0)
            {
                element1Tag.Foreground = red;
            }
            else if (i == 1)
            {
                element2Tag.Foreground = red;
            }
            else if (i == 2)
            {
                element3Tag.Foreground = red;
            }
            else if (i == 3)
            {
                element4Tag.Foreground = red;
            }
            else if (i == 4)
            {
                element5Tag.Foreground = red;
            }
            else if (i == 5)
            {
                element6Tag.Foreground = red;
            }
            else if (i == 6)
            {
                element7Tag.Foreground = red;
            }
            else if (i == 7)
            {
                element8Tag.Foreground = red;
            }
            else if (i == 8)
            {
                element9Tag.Foreground = red;
            }
            else if (i == 9)
            {
                element10Tag.Foreground = red;
            }
            else if (i == 10)
            {
                element11Tag.Foreground = red;
            }
            else if (i == 11)
            {
                element12Tag.Foreground = red;
            }
            else if (i == 12)
            {
                maintainReason.Foreground = red;
            }
            else if (i == 13)
            {
                content.Foreground = red;
            }
            else if (i == 14)
            {
                result.Foreground = red;
            }
        }
        //部品下拉框赋值
        public String getProductNum()
        {
            String productNum = null;
            for (int i = 1; i <= 200; i++)
            {
                if (i != 1) productNum = productNum + ",";
                productNum = productNum + i + "个";
            }
            return productNum;
        }
    }
}
