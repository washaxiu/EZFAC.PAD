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
    class UnqualifiedReportService
    {
        public string good = "○";
        public string bad = "×";
        public CommonOperation commonOperation = new CommonOperation();


        /*
        * 获取当前用户的不合格报告审批信息
        * @param 显示信息列表  用户等级 检查信息所在目录
        */
        public async void getApprovalList(ListView lvFiles, string userlevel,string floderName)
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
                            classificationNo = content[2].GetObject()["status"].GetString(),
                            unqualifiedContent = content[1].GetObject()["status"].GetString(),
                            underTaker = content[0].GetObject()["status"].GetString(),
                            // 设置用户信息
                            username = checkerInfo[0].GetObject()["name"].GetString(),
                            date = checkerInfo[0].GetObject()["date"].GetString(),
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
         * 审批信息回填
         * @param 文件所在目录 文件名 界面的控件数组
         */
        public async void getApprovalDetail(string folderName, string fileName, TextBox[] textBox, CalendarDatePicker[] calendarDatePicker, RadioButton[] radioButton)
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

                    int index = 1;

                    foreach (TextBox item in textBox)
                    {
                        item.IsReadOnly = true;
                        item.Text = content[index++].GetObject()["status"].GetString();
                    }
                    foreach (CalendarDatePicker item in calendarDatePicker)
                    {
                        string dateContent = content[index++].GetObject()["status"].GetString();
                        if (!"".Equals(dateContent))
                        {
                            item.Date = Convert.ToDateTime(dateContent);
                        }
                        item.IsEnabled = false;
                    }
                    foreach (RadioButton item in radioButton)
                    {
                        if(index<content.Count && item.Name == content[index].GetObject()["status"].GetString())
                        {
                            item.IsChecked = true;
                            index++;
                        }
                        else
                        {
                            item.IsEnabled = false;
                        }
                    }
                }
            }
        }
    }
}
