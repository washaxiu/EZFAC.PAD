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
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace EZFAC.PAD.src.Service
{
    class SemiFinishedCheckService
    {
        public string good = "○";
        public string bad = "×";
        public CommonOperation commonOperation = new CommonOperation();
        public SolidColorBrush red = new SolidColorBrush(Colors.Red);
        public SolidColorBrush black = new SolidColorBrush(Colors.Black);

        /*
        * 获取当前用户的不合格报告审批信息
        * @param 显示信息列表  用户等级
        */
        public async void getApprovalList(ListView lvFiles, string userlevel, string fileName)
        {
            lvFiles.Items.Clear();
            StorageFolder record_folder = KnownFolders.PicturesLibrary;
            StorageFolder folder = await record_folder.CreateFolderAsync(fileName, CreationCollisionOption.OpenIfExists);
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
                        string color = "Black";
                        if (commonOperation.isJudged(checkerInfo, userlevel) == false)
                        {
                            continue;
                        }
                        // 判断当前用户的下级是否修改过信息
                        if (commonOperation.isEditContent(checkerInfo, userlevel))
                        {
                            color = "Red";
                        }
                        lvFiles.Items.Add(new
                        {
                            // 设置文本内容
                            group = checkInfo[0].GetObject()["group"].GetString(),
                            line = checkInfo[0].GetObject()["number"].GetString(),
                            // 下拉框内容
                            separateStatus = content[0].GetObject()["status"].GetString(),
                            gneck = content[1].GetObject()["status"].GetString(),
                            // 文本内容
                            item = content[2].GetObject()["status"].GetString(),
                            personInCharge = content[3].GetObject()["status"].GetString(),
                            // 设置检查用户信息
                            checkName = checkerInfo[0].GetObject()["name"].GetString(),
                            checkDate = checkerInfo[0].GetObject()["date"].GetString(),
                            // 其它帮助信息
                            fileName = file.Name,
                            Color = color,
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
        public async void getApprovalDetail(string folderName, string fileName, TextBox[] textBox, ToggleSwitch[] toggleSwitch, TextBlock[] textBlock)
        {
            StorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            if (folder != null)
            {
                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                if (file != null)
                {
                    var jsonText = await FileIO.ReadTextAsync(file);
                    JsonObject jsonObject = JsonObject.Parse(jsonText);
                    // 获取点检信息
                    JsonArray checkInof = jsonObject["checkInfo"].GetArray();
                    // 获取内容信息
                    JsonArray content = jsonObject["content"].GetArray();

                    int index = 0;
                    textBox[0].Text = "压轴线"+checkInof[0].GetObject()["group"].GetString();
                    textBox[1].Text = checkInof[0].GetObject()["group"].GetString() + " - "+checkInof[0].GetObject()["number"].GetString();
                    for(int i=0;i< textBox.Length; i++)
                    {
                        textBox[i].IsReadOnly = true;
                        if (i > 1)
                        {
                            textBox[i].Text = content[index++].GetObject()["status"].GetString();
                        }
                    }
                    for (int i = 0; i < toggleSwitch.Length; i++)
                    {
                        string status = content[index].GetObject()["status"].GetString();
                        string edit = content[index++].GetObject()["edit"].GetString();
                        toggleSwitch[i].IsOn = status.Equals("good");
                        textBlock[i].Foreground = "1".Equals(edit) ? red : black;
                    }
                }
            }
        }
    }
}
