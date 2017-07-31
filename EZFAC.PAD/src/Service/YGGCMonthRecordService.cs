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
    class YZGCMonthRecordService
    {
        public string good = "○";
        public string bad = "×";
        public CommonOperation commonOperation = new CommonOperation();


        /*
        * 获取当前用户的点检审批信息
        * @param 显示信息列表  用户等级
        */
        public async void getYZGCMonthList(ListView lvFiles, string userlevel)
        {
            lvFiles.Items.Clear();
            StorageFolder record_folder = KnownFolders.PicturesLibrary;
            StorageFolder folder = await record_folder.CreateFolderAsync("YZGCMonthRecord", CreationCollisionOption.OpenIfExists);
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
                        string color = "Black", contentEdit = null;
                        // 判断当前用户的下级是否审批过信息
                        if (commonOperation.isJudged(checkerInfo, userlevel) == false)
                        {
                            continue;
                        }
                        // 判断当前用户的下级是否修改过信息
                        if (commonOperation.isEditContent(checkerInfo, userlevel))
                        {
                            color = "Red";
                        }
                        // 获取检查内容修改信息
                        for (int i = 0; i < content.Count; i++)
                        {
                            contentEdit = contentEdit + content[i].GetObject()["edit"].GetString();
                        }
                        lvFiles.Items.Add(new
                        {
                            // 设置检查信息
                            Group = checkInfo[0].GetObject()["group"].GetString(),
                            Line = checkInfo[0].GetObject()["number"].GetString(),
                            MachineName = "压铸线" + checkInfo[0].GetObject()["group"].GetString() + "-" + checkInfo[0].GetObject()["number"].GetString(),
                            // 设置检查内容
                            Temp1Name = content[0].GetObject()["status"].GetString() == "good" ? good : bad,
                            Temp2Name = content[1].GetObject()["status"].GetString() == "good" ? good : bad,
                            Temp3Name = content[2].GetObject()["status"].GetString() == "good" ? good : bad,
                            Loop1Name = content[3].GetObject()["status"].GetString() == "good" ? good : bad,
                            Loop2Name = content[4].GetObject()["status"].GetString() == "good" ? good : bad,
                            Loop3Name = content[5].GetObject()["status"].GetString() == "good" ? good : bad,
                            Select1Name = content[6].GetObject()["status"].GetString() == "good" ? good : bad,
                            Plat1Name = content[7].GetObject()["status"].GetString() == "good" ? good : bad,
                            // 设置用户信息
                            CheckerName = checkerInfo[0].GetObject()["name"].GetString(),
                            CheckDate = checkerInfo[0].GetObject()["date"].GetString(),
                            ContentEdit = contentEdit,
                            Count = Convert.ToString(count++),
                            Color = color
                        });
                    }
                }
            }
        }

        /*
        * 获取点检审批信息详情
        * @param 信息详情json字符串
        */
        public Dictionary<string, string> getDetail(String jsonText)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            // 将信息字符串转换成字符串数组
            string[] strs = jsonText.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Split("{,=}".ToCharArray());
            // 获取组 ，机番, 时间信息
            for (int i = 1; i < strs.Length - 1; i += 2)
            {
                if (strs[i].Equals("Group"))
                {
                    data.Add("group", strs[i + 1]);
                }
                else if (strs[i].Equals("Line"))
                {
                    data.Add("line", strs[i + 1]);
                }
                else if (strs[i].Equals("CheckDate"))
                {
                    data.Add("date", strs[i + 1]);
                }
                else if (strs[i].Equals("CheckerName"))
                {
                    data.Add("checker", strs[i + 1]);
                }
                else if (strs[i].Equals("Temp1Name"))
                {
                    data.Add("temp1", strs[i + 1]);
                }
                else if (strs[i].Equals("Temp2Name"))
                {
                    data.Add("temp2", strs[i + 1]);
                }
                else if (strs[i].Equals("Temp3Name"))
                {
                    data.Add("temp3", strs[i + 1]);
                }
                else if (strs[i].Equals("Temp4Name"))
                {
                    data.Add("temp4", strs[i + 1]);
                }
                else if (strs[i].Equals("Temp5Name"))
                {
                    data.Add("temp5", strs[i + 1]);
                }
                else if (strs[i].Equals("Temp6Name"))
                {
                    data.Add("temp6", strs[i + 1]);
                }
                else if (strs[i].Equals("Temp7Name"))
                {
                    data.Add("temp7", strs[i + 1]);
                }
                else if (strs[i].Equals("Temp8Name"))
                {
                    data.Add("temp8", strs[i + 1]);
                }
                else if (strs[i].Equals("ContentEdit"))
                {
                    data.Add("contentEdit", strs[i + 1]);
                }
            }
            return data;
        }

        /*
         * 审批多条信息
         * @param 信息详情列表 审批人信息 文件所在目录
         */
        public async void mulYZGCMonth(ListView listView, CheckerInfoEntity checkerInfoEntity, string folderName)
        {
            for (int i = 0; i < listView.SelectedItems.Count; i++)
            {
                string itemContent = listView.SelectedItems[i].ToString();
                string groupName = null, lineName = null, date = null;
                string[] strs = itemContent.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Split("{,=}".ToCharArray());
                for (int j = 1; j < strs.Length - 1; j += 2)
                {
                    if (strs[j].Equals("CheckDate")) date = strs[j + 1];
                    if (strs[j].Equals("Group")) groupName = strs[j + 1];
                    if (strs[j].Equals("Line")) lineName = strs[j + 1];
                }
                string fileName = "ykk_record_" + groupName + "_" + lineName + "_" + date + ".ykk";
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
    }
}
