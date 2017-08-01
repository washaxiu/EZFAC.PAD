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
    class SemiFinishedCheckService
    {
        public string good = "○";
        public string bad = "×";
        public CommonOperation commonOperation = new CommonOperation();


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
    }
}
