using EZFAC.PAD.src.Model;
using EZFAC.PAD.src.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace EZFAC.PAD.src.Service
{
    class MaintenanceLogService
    {
        public CommonOperation commonOperation = new CommonOperation();
        /*
        * 获取当前用户的点检审批信息
        * @param 显示信息列表  用户等级
        */
        public async void getMaintenanceLogList(ListView lvFiles, string userlevel)
        {
            lvFiles.Items.Clear();
            StorageFolder record_folder = KnownFolders.PicturesLibrary;
            StorageFolder folder = await record_folder.CreateFolderAsync("MaintenanceLog", CreationCollisionOption.OpenIfExists);
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
                            lineName = checkInfo[0].GetObject()["lineName"].GetString(),
                            elementName = checkInfo[0].GetObject()["elementName"].GetString(),
                            SHOTNumber = checkInfo[0].GetObject()["SHOTNumber"].GetString(),
                            record1 = content[0].GetObject()["status"].GetString(),                           
                            record2 = content[1].GetObject()["status"].GetString(),
                            record3 = content[2].GetObject()["status"].GetString(),
                            record4 = content[3].GetObject()["status"].GetString(),
                            record5 = content[4].GetObject()["status"].GetString(),
                            record6 = content[5].GetObject()["status"].GetString(),
                            record7 = content[6].GetObject()["status"].GetString(),
                            record8 = content[7].GetObject()["status"].GetString(),
                            record9 = content[8].GetObject()["status"].GetString(),
                            record10 = content[9].GetObject()["status"].GetString(),
                            record11 = content[10].GetObject()["status"].GetString(),
                            record12 = content[11].GetObject()["status"].GetString(),
                            reason = content[12].GetObject()["status"].GetString(),
                            content = content[13].GetObject()["status"].GetString(),
                            result = content[14].GetObject()["status"].GetString(),
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
       * 审批多条信息
       * @param 信息详情列表 审批人信息 文件所在目录
       */
        public async void mulMaintenanceLog(ListView listView, CheckerInfoEntity checkerInfoEntity, string folderName)
        {
            for (int i = 0; i < listView.SelectedItems.Count; i++)
            {
                string itemContent = listView.SelectedItems[i].ToString();
                
                string jifan = null, piming = null, date = null;
                string[] strs = itemContent.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Split("{,=}".ToCharArray());
                
                for (int j = 1; j < strs.Length - 1; j += 2)
                {
                    if (strs[j].Equals("CheckDate")) date = strs[j + 1];
                    if (strs[j].Equals("lineName")) jifan = strs[j + 1];
                    if (strs[j].Equals("elementName")) piming = strs[j + 1];
                }
                string fileName = "ykk_record_" + jifan + "_" + piming + "_" + date + ".ykk";
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
       * 获取点检审批信息详情
       * @param 信息详情json字符串
       */
        public Dictionary<string, string> getDetail(String jsonText)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            // 将信息字符串转换成字符串数组
            string[] strs = jsonText.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Split("{,=}".ToCharArray());
            // 获取组 ，机番, 时间信息
            for (int i = 1; i < strs.Length - 1; i += 1)
            {
                if (strs[i].Equals("lineName"))
                {
                    data.Add("jifan", strs[i + 1]);
                }
                else if (strs[i].Equals("elementName"))
                {
                    data.Add("pinming", strs[i + 1]);
                } 
                else if (strs[i].Equals("CheckDate"))
                {
                    data.Add("date", strs[i + 1]);
                }
                else if (strs[i].Equals("SHOTNumber"))
                {
                    data.Add("SHOT", strs[i + 1]);
                }
                else if (strs[i].Equals("record1"))
                {
                    data.Add("record1", strs[i + 1]);
                }
                else if (strs[i].Equals("record2"))
                {
                    data.Add("record2", strs[i + 1]);
                }
                else if (strs[i].Equals("record3"))
                {
                    data.Add("record3",strs[i+1]);
                }
                else if (strs[i].Equals("record4"))
                {
                    data.Add("record4", strs[i + 1]);
                }
                else if (strs[i].Equals("record5"))
                {
                    data.Add("record5", strs[i + 1]);
                }
                else if (strs[i].Equals("record6"))
                {
                    data.Add("record6", strs[i + 1]);
                }
                else if (strs[i].Equals("record7"))
                {
                    data.Add("record7", strs[i + 1]);
                }
                else if (strs[i].Equals("record8"))
                {
                    data.Add("record8", strs[i + 1]);
                }
                else if (strs[i].Equals("record9"))
                {
                    data.Add("record9", strs[i + 1]);
                }
                else if (strs[i].Equals("record10"))
                {
                    data.Add("record10", strs[i + 1]);
                }
                else if (strs[i].Equals("record11"))
                {
                    data.Add("record11", strs[i + 1]);
                }
                else if (strs[i].Equals("record12"))
                {
                    data.Add("record12", strs[i + 1]);
                }        
                else if (strs[i].Equals("reason"))
                {
                    data.Add("reason", strs[i + 1]);
                }
                else if (strs[i].Equals("content"))
                {
                    data.Add("content", strs[i + 1]);
                }
                else if (strs[i].Equals("result"))
                {
                    data.Add("result", strs[i + 1]);
                }
                else if (strs[i].Equals("CheckerName"))
                {
                    data.Add("CheckerName", strs[i + 1]);
                }
                else if (strs[i].Equals("ContentEdit"))
                {
                    data.Add("ContentEdit", strs[i + 1]);
                }              
            }
            return data;
        }
       
    }
}
