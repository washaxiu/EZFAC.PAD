using EZFAC.PAD.src.Model;
using EZFAC.PAD.src.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace EZFAC.PAD.src.Service
{
    class AbnormalQualityReportService
    {
        public string good = "○";
        public string bad = "×";
        public CommonOperation commonOperation = new CommonOperation();

        /*
        * 获取当前用户的不合格报告审批信息
        * @param 显示信息列表  用户等级 检查信息所在目录
        */
        public async void getApprovalList(ListView lvFiles, string userlevel, string floderName)
        {
            lvFiles.Items.Clear();
            StorageFolder record_folder = KnownFolders.PicturesLibrary;
            StorageFolder folder = await record_folder.CreateFolderAsync(floderName, CreationCollisionOption.OpenIfExists);
            int count = 0;
            if (folder != null)
            {
                // 获取目录下的文件列表
                var subFiles = await folder.GetFilesAsync();
                foreach (StorageFile file in subFiles)
                {
                    string extensionname = Path.GetExtension(file.Name);
                    if (extensionname == ".ykk")
                    {
                        var jsonText = await FileIO.ReadTextAsync(file);
                        JsonObject jsonObject = JsonObject.Parse(jsonText);
                        // 获取检查信息
                        JsonArray checkInfo = jsonObject["checkInfo"].GetArray();
                        // 获取检查内容
                        JsonArray content = jsonObject["content"].GetArray();
                        // 获取用户信息
                        JsonArray checkerInfo = jsonObject["checkerInfo"].GetArray();
                        // 判断当前用户的下级是否审批过信息
                        if (commonOperation.isJudged(checkerInfo, userlevel) == false)
                        {
                            continue;
                        }
                        lvFiles.Items.Add(new
                        {
                            // 设置检查内容
                            thingName = content[0].GetObject()["status"].GetString(),
                            badContent = content[4].GetObject()["status"].GetString(),
                            comboBox1 = content[41].GetObject()["status"].GetString(),
                            // 设置用户信息
                            checkName = checkerInfo[0].GetObject()["name"].GetString(),
                            checkDate = checkerInfo[0].GetObject()["date"].GetString(),
                            fileName = file.Name,
                            Count = Convert.ToString(count++)
                        });
                    }
                }
            }
        }

        /*
         * 审批多条信息
         * @param 信息详情列表 审批人信息 文件所在目录
         */
        public async void mulApproval(ListView listView, CheckerInfoEntity checkerInfoEntity, string folderName)
        {
            for (int i = 0; i < listView.SelectedItems.Count; i++)
            {
                string itemContent = listView.SelectedItems[i].ToString();
                string fileName = null;
                string[] strs = itemContent.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Split("{,=}".ToCharArray());
                for (int j = 1; j < strs.Length - 1; j += 2)
                {
                    if (strs[j].Equals("fileName"))
                    {
                        fileName = strs[j + 1];
                        break;
                    }
                }
                StorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
                if (folder != null)
                {
                    StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    if (file != null)
                    {
                        var jsonText = await FileIO.ReadTextAsync(file);
                        JsonObject jsonObject = JsonObject.Parse(jsonText);
                        // 获取用户信息
                        JsonArray checkerInfo = jsonObject["checkerInfo"].GetArray();

                        JsonObject item = new JsonObject();
                        item["name"] = JsonValue.CreateStringValue(checkerInfoEntity.name);
                        item["level"] = JsonValue.CreateStringValue(checkerInfoEntity.level);
                        item["check"] = JsonValue.CreateStringValue(checkerInfoEntity.check);
                        item["edit"] = JsonValue.CreateStringValue(checkerInfoEntity.edit);
                        item["date"] = JsonValue.CreateStringValue(checkerInfoEntity.date);
                        item["comments"] = JsonValue.CreateStringValue(checkerInfoEntity.comments);

                        for (int j = 0; j < checkerInfo.Count; j++)
                        {
                            string level = Convert.ToString(j + 1);
                            if (level == checkerInfoEntity.level)
                            {
                                checkerInfo[j] = item;
                            }
                        }
                        commonOperation.writeJsonToFile(jsonObject, fileName, KnownFolders.PicturesLibrary, folderName);
                    }
                }
            }
        }


        /*
         * 审批信息回填 界面1
         * @param 文件所在目录 文件名 界面的控件数组
         */
        public async void getApprovalDetailOne(string userlevel, string folderName, string fileName, 
                        TextBox[] textBox, RadioButton[] radioButton, ComboBox[] comboBox, 
                        CalendarDatePicker[] calendarDatePicker)
        {
            StorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            if (folder != null)
            {
                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                if (file != null)
                {
                    var jsonText = await FileIO.ReadTextAsync(file);
                    JsonObject jsonObject = JsonObject.Parse(jsonText);
                    // 获取内容信息
                    JsonArray content = jsonObject["content"].GetArray();
                    // 因为存在下拉本文改变事件，先初始化下拉框，起始位置为 文本框数量+单选框数量。
                    int index = textBox.Length + 2;
                    
                    foreach (ComboBox item in comboBox)
                    {
                        if (!"2".Equals(userlevel)) item.IsEnabled = false;
                        item.SelectedItem = content[index++].GetObject()["status"].GetString();
                    }
                    foreach (CalendarDatePicker item in calendarDatePicker)
                    {
                        string dateContent = content[index++].GetObject()["status"].GetString();
                        if (!"".Equals(dateContent))
                        {
                            item.Date = Convert.ToDateTime(dateContent);
                        }
                        if (!"2".Equals(userlevel)) item.IsEnabled = false;
                    }
                    // 最后初始化文本框和单选框
                    index = 0;

                    foreach (TextBox item in textBox)
                    {
                        if (!"2".Equals(userlevel)) item.IsReadOnly = true;
                        item.Text = content[index++].GetObject()["status"].GetString();
                    }
                    foreach (RadioButton item in radioButton)
                    {
                        if (index < content.Count && item.Name == content[index].GetObject()["status"].GetString())
                        {
                            item.IsChecked = true;
                            index++;
                        }
                        else
                        {
                            if (!"2".Equals(userlevel)) item.IsEnabled = false;
                        }
                    }
                }
            }
        }

        /*
         * 审批信息回填 界面2
         * @param 文件所在目录 文件名 界面的控件数组
         */
        public async void getApprovalDetailTwo(string userlevel, string folderName, string fileName,
                        TextBox[] textLevel3, RadioButton[] radioLevel3, TextBox[] textLevel4,
                        CalendarDatePicker[] calendarLevel4)
        {
            StorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            if (folder != null)
            {
                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                if (file != null)
                {
                    var jsonText = await FileIO.ReadTextAsync(file);
                    JsonObject jsonObject = JsonObject.Parse(jsonText);
                    // 获取内容信息
                    JsonArray content = jsonObject["content"].GetArray();
                    // 上一个界面的控件数量为70
                    int index = 70,count = content.Count;

                    foreach (TextBox item in textLevel3)
                    {
                        if (!"3".Equals(userlevel)) item.IsReadOnly = true;
                        item.Text = index < count? content[index++].GetObject()["status"].GetString():"";
                    }
                    foreach (RadioButton item in radioLevel3)
                    {
                        if (index < count && item.Name == content[index].GetObject()["status"].GetString())
                        {
                            item.IsChecked = true;
                            index++;
                        }
                        else
                        {
                            if (!"3".Equals(userlevel)) item.IsEnabled = false;
                        }
                    }
                    foreach (TextBox item in textLevel4)
                    {
                        if (!"4".Equals(userlevel)) item.IsReadOnly = true;
                        item.Text = index < count ? content[index++].GetObject()["status"].GetString() : "";
                    }
                    foreach (CalendarDatePicker item in calendarLevel4)
                    {
                        string dateContent = index < count ? content[index++].GetObject()["status"].GetString():"";
                        if (!"".Equals(dateContent))
                        {
                            item.Date = Convert.ToDateTime(dateContent);
                        }
                        if (!"4".Equals(userlevel)) item.IsEnabled = false;
                    }

                }
            }
        }

        /*
       * level 3 审批信息
       * @param 信息存储结构 日期控件
       */
        public async void level3Approval(Dictionary<string, string> data, TextBlock date, TextBox[] textLevel3, RadioButton[] radioLevel3)
        {
            StorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(data["folderName"], CreationCollisionOption.OpenIfExists);
            if (folder != null)
            {
                StorageFile file = await folder.CreateFileAsync(data["fileName"], CreationCollisionOption.OpenIfExists);
                if (file != null)
                {
                    var jsonText = await FileIO.ReadTextAsync(file);
                    JsonObject jsonObject = JsonObject.Parse(jsonText);
                    // 获取用户信息
                    JsonArray checkerInfo = jsonObject["checkerInfo"].GetArray();
                    // 获取内容信息
                    JsonArray content = jsonObject["content"].GetArray();

                    //  增加level3所填信息
                    foreach (TextBox textBox in textLevel3)
                    {
                        JsonObject contentItem = new JsonObject();
                        contentItem["name"] = JsonValue.CreateStringValue(textBox.Name);
                        contentItem["status"] = JsonValue.CreateStringValue(textBox.Text);
                        contentItem["edit"] = JsonValue.CreateStringValue("0");
                        content.Add(contentItem);
                    }
                    foreach (RadioButton radioButton in radioLevel3)
                    {
                        if (radioButton.IsChecked == true)
                        {
                            JsonObject contentItem = new JsonObject();
                            contentItem["name"] = JsonValue.CreateStringValue(radioButton.GroupName);
                            contentItem["status"] = JsonValue.CreateStringValue(radioButton.Name);
                            contentItem["edit"] = JsonValue.CreateStringValue("0");
                            content.Add(contentItem);
                        }
                    }

                    // 设置level3已审批
                    JsonObject item = new JsonObject();
                    item["name"] = JsonValue.CreateStringValue(data["username"]);
                    item["level"] = JsonValue.CreateStringValue(data["userlevel"]);
                    item["check"] = JsonValue.CreateStringValue("1");
                    item["edit"] = JsonValue.CreateStringValue("0");
                    item["date"] = JsonValue.CreateStringValue(date.Text);
                    item["comments"] = JsonValue.CreateStringValue("");

                    checkerInfo[2] = item;

                    commonOperation.writeJsonToFile(jsonObject, data["fileName"], KnownFolders.PicturesLibrary, data["folderName"]);
                }
            }
        }

        /*
       * level 4 审批信息
       * @param 信息存储结构 日期控件
       */
        public async void level4Approval(Dictionary<string, string> data, TextBlock date, TextBox[] textLevel4, CalendarDatePicker[] calendarLevel4)
        {
            StorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(data["folderName"], CreationCollisionOption.OpenIfExists);
            if (folder != null)
            {
                StorageFile file = await folder.CreateFileAsync(data["fileName"], CreationCollisionOption.OpenIfExists);
                if (file != null)
                {
                    var jsonText = await FileIO.ReadTextAsync(file);
                    JsonObject jsonObject = JsonObject.Parse(jsonText);
                    // 获取用户信息
                    JsonArray checkerInfo = jsonObject["checkerInfo"].GetArray();
                    // 获取内容信息
                    JsonArray content = jsonObject["content"].GetArray();
                    //  增加level4所填信息
                    foreach (TextBox textBox in textLevel4)
                    {
                        JsonObject contentItem = new JsonObject();
                        contentItem["name"] = JsonValue.CreateStringValue(textBox.Name);
                        contentItem["status"] = JsonValue.CreateStringValue(textBox.Text);
                        contentItem["edit"] = JsonValue.CreateStringValue("0");
                        content.Add(contentItem);
                    }
                    foreach (CalendarDatePicker calendarDatePicker in calendarLevel4)
                    {
                        JsonObject contentItem = new JsonObject();
                        contentItem["name"] = JsonValue.CreateStringValue(calendarDatePicker.Name);
                        contentItem["status"] = JsonValue.CreateStringValue(calendarDatePicker.Date.ToString().Split(' ')[0]);
                        contentItem["edit"] = JsonValue.CreateStringValue("0");
                        content.Add(contentItem);
                    }

                    // 设置level4已审批
                    JsonObject item = new JsonObject();
                    item["name"] = JsonValue.CreateStringValue(data["username"]);
                    item["level"] = JsonValue.CreateStringValue(data["userlevel"]);
                    item["check"] = JsonValue.CreateStringValue("1");
                    item["edit"] = JsonValue.CreateStringValue("0");
                    item["date"] = JsonValue.CreateStringValue(date.Text);
                    item["comments"] = JsonValue.CreateStringValue("");

                    checkerInfo[3] = item;

                    commonOperation.writeJsonToFile(jsonObject, data["fileName"], KnownFolders.PicturesLibrary, data["folderName"]);
                }
            }
        }


        /*
        * level 5 审批信息
        * @param 信息存储结构 日期控件
        */
        public async void level5Approval(Dictionary<string, string> data,TextBlock date)
        {
            StorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(data["folderName"], CreationCollisionOption.OpenIfExists);
            if (folder != null)
            {
                StorageFile file = await folder.CreateFileAsync(data["fileName"], CreationCollisionOption.OpenIfExists);
                if (file != null)
                {
                    var jsonText = await FileIO.ReadTextAsync(file);
                    JsonObject jsonObject = JsonObject.Parse(jsonText);
                    // 获取用户信息
                    JsonArray checkerInfo = jsonObject["checkerInfo"].GetArray();

                    JsonObject item = new JsonObject();
                    item["name"] = JsonValue.CreateStringValue(data["username"]);
                    item["level"] = JsonValue.CreateStringValue(data["userlevel"]);
                    item["check"] = JsonValue.CreateStringValue("1");
                    item["edit"] = JsonValue.CreateStringValue("0");
                    item["date"] = JsonValue.CreateStringValue(date.Text);
                    item["comments"] = JsonValue.CreateStringValue("");

                    checkerInfo[4] = item;
                   
                    commonOperation.writeJsonToFile(jsonObject, data["fileName"], KnownFolders.PicturesLibrary, data["folderName"]);
                }
            }
        }
    }
}
