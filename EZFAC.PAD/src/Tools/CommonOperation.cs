using EZFAC.PAD.src.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace EZFAC.PAD.src.Tools
{
    class CommonOperation
    {

        public string good = "○";
        public string bad = "×";

        /*
          * 初始化JsonObject并设置用户级别信息
          */
        public JsonObject initJsonObject()
        {
            JsonObject checkRecordData = new JsonObject();
            // 设置用户级别
            checkRecordData["level2check"] = JsonValue.CreateStringValue("0");
            checkRecordData["level2edit"] = JsonValue.CreateStringValue("0");
            checkRecordData["level2date"] = JsonValue.CreateStringValue("");
            checkRecordData["level2approvaler"] = JsonValue.CreateStringValue("");
            checkRecordData["level3check"] = JsonValue.CreateStringValue("0");
            checkRecordData["level3edit"] = JsonValue.CreateStringValue("0");
            checkRecordData["level3date"] = JsonValue.CreateStringValue("");
            checkRecordData["level3approvaler"] = JsonValue.CreateStringValue("");
            checkRecordData["level4check"] = JsonValue.CreateStringValue("0");
            checkRecordData["level4edit"] = JsonValue.CreateStringValue("0");
            checkRecordData["level4date"] = JsonValue.CreateStringValue("");
            checkRecordData["level4approvaler"] = JsonValue.CreateStringValue("");
            checkRecordData["level5check"] = JsonValue.CreateStringValue("0");
            checkRecordData["level5edit"] = JsonValue.CreateStringValue("0");
            checkRecordData["level5date"] = JsonValue.CreateStringValue("");
            checkRecordData["level5approvaler"] = JsonValue.CreateStringValue("");
            return checkRecordData;
        }

        /*
         * 将json数据写入对应文件中
         * @param Json数据 文件名 文件所在目录 文件夹名
         */
        public async void writeJsonToFile(JsonObject content,String fileName, StorageFolder record_folder,string folderName)
        {
            // 显示JSON对象的字符串表示形式
            string jstr = content.Stringify();
            StorageFolder newFolder = await record_folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            StorageFile record_file = await newFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (Stream file = await record_file.OpenStreamForWriteAsync())
            {
                using (StreamWriter write = new StreamWriter(file))
                {
                    string[] strs = jstr.Split(',');
                    for (int i = 0; i < strs.Length; i++)
                    {
                        if (i == 0)
                        {
                            write.Write(strs[i].Substring(0, 1));
                            write.WriteLine();
                            write.Write("   " + strs[i].Substring(1, strs[i].Length - 1)+",");
                            write.WriteLine();
                        }
                        else if (i == strs.Length - 1)
                        {
                            write.Write("   " + strs[i].Substring(0, strs[i].Length - 1));
                            write.WriteLine();
                            write.Write(strs[i].Substring(strs[i].Length - 1, 1));
                            write.WriteLine();
                        }
                        else
                        {
                            write.Write("   " + strs[i]+",");
                            write.WriteLine();
                        }
                    }
                }
            }

        }

        /*
         * 判断当前用户的下级是否审批过信息，若没有则返回false
         */
        public bool isJudged(JsonObject jsonObject,string userlevel)
        {
            string checked_flag;
            string lowlevel_flag;

            if (userlevel == "2")
            {
                lowlevel_flag = "1";
                checked_flag = jsonObject["level2check"].GetString();
            }
            else if (userlevel == "3")
            {
                lowlevel_flag = jsonObject["level2check"].GetString();
                checked_flag = jsonObject["level3check"].GetString();
            }
            else if (userlevel == "4")
            {
                lowlevel_flag = jsonObject["level3check"].GetString();
                checked_flag = jsonObject["level4check"].GetString();
            }
            else if (userlevel == "5")
            {
                lowlevel_flag = jsonObject["level4check"].GetString();
                checked_flag = jsonObject["level5check"].GetString();
            }
            else
            {
                return false;
            }

            if (!((lowlevel_flag == "1") && (checked_flag == "0")))
            {
                return false;
            }
            return true;
        }


        /*
        * 判断当前用户的下级是否修改过信息，若没有则返回false
        */
        public string getEditContent(JsonObject jsonObject, string userlevel)
        {
            string lowlevel_edit = "0";
            if (userlevel == "3")
            {
                lowlevel_edit = jsonObject["level2edit"].GetString();
            }
            else if (userlevel == "4")
            {
                lowlevel_edit = jsonObject["level3edit"].GetString();
            }
            else if (userlevel == "5")
            {
                lowlevel_edit = jsonObject["level4edit"].GetString();
            }
            return lowlevel_edit;
        }

        /*
         * 根据用户等级获取用户职位
         * @param 用户等级
         */
        public string getJobByLevel(String userLevel)
        {
            string job = "";
            if (userLevel == "1")
            {
                job = "员工";
            }
            else if (userLevel == "2")
            {
                job = "出班长";
            }
            else if (userLevel == "3")
            {
                job = "保全";
            }
            else if (userLevel == "4")
            {
                job = "课长";
            }
            else if (userLevel == "5")
            {
                job = "部门长";
            }
            else
            {
                job = "未知";
            }
            return job;
        }

         /*
          * 获取各级用户的审批信息
          * @param 信息显示文本 用户等级  文件名 文件所在目录
          */
        public async void getStateText(TextBox reviewInfor,String userLevel,string checkfilename,string folderName)
        {
            if (userLevel == "1") return;
            StorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            if (folder != null)
            {
                // 获取目录下的文件列表
                var subFiles = await folder.GetFilesAsync();
                // 向ListView中添加文件
                foreach (StorageFile file in subFiles)
                {
                    string extensionname = Path.GetExtension(file.Name);
                    if (file.Name == checkfilename)
                    {
                        
                        string jsonText = await FileIO.ReadTextAsync(file);
                        JsonObject jsonObject = JsonObject.Parse(jsonText);

                        reviewInfor.Text = "点检人：" + jsonObject["checker"].GetString()+", "+ jsonObject["date"].GetString()+" 提交\n";
                        if (userLevel == "2") return;

                        string level2edit = jsonObject["level2edit"].GetString();
                        string level2approvaler = jsonObject["level2approvaler"].GetString();
                        string level2date = jsonObject["level2date"].GetString();

                        reviewInfor.Text = reviewInfor.Text + getJobByLevel("2") + "：" + level2approvaler + ", " + level2date;
                        if (level2edit == "0")
                        {
                            reviewInfor.Text = reviewInfor.Text + " 提交\n";
                        }
                        else
                        {
                            reviewInfor.Text = reviewInfor.Text + " 修改并提交\n";
                        }
                        if (userLevel == "3") return;
                        
                        string level3edit = jsonObject["level3edit"].GetString();
                        string level3approvaler = jsonObject["level3approvaler"].GetString();
                        string level3date = jsonObject["level3date"].GetString();

                        reviewInfor.Text = reviewInfor.Text + getJobByLevel("3") + "：" + level3approvaler + ", " + level3date;
                        if (level3edit == "0")
                        {
                            reviewInfor.Text = reviewInfor.Text + " 提交\n";
                        }
                        else
                        {
                            reviewInfor.Text = reviewInfor.Text + " 修改并提交\n";
                        }
                        if (userLevel == "4") return;

                        string level4edit = jsonObject["level4edit"].GetString();
                        string level4approvaler = jsonObject["level4approvaler"].GetString();
                        string level4date = jsonObject["level4date"].GetString();

                        reviewInfor.Text = reviewInfor.Text + getJobByLevel("4") + "：" + level4approvaler + ", " + level4date;
                        if (level4edit == "0")
                        {
                            reviewInfor.Text = reviewInfor.Text + " 提交\n";
                        }
                        else
                        {
                            reviewInfor.Text = reviewInfor.Text  + " 修改并提交\n";
                        }
                        if (userLevel == "5") return;
                    }
                }
            }
        }

        
    }
}
