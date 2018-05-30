using EZFAC.PAD.src.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace EZFAC.PAD.src.Tools
{
    class CommonOperation
    {
        private int userLevelCount = 5;
        public string good = "○";
        public string bad = "×";
        public MessDialog messDialog = new MessDialog();

        public JsonObject initJsonObject()
        {
            JsonObject s = new JsonObject();
            return s;
        }

        public async void hide()
        {
            await StatusBar.GetForCurrentView().HideAsync();
        }

        /*
          * 初始化检查的json信息
          * @param检查类型 检查内容分类信息
          */
        public JsonArray initCheckJsonArray(string type, string group, string number)
        {
            JsonArray checkerInfo = new JsonArray();
            JsonObject checker = new JsonObject();
            checker["type"] = JsonValue.CreateStringValue(type);
            checker["group"] = JsonValue.CreateStringValue(group);
            checker["number"] = JsonValue.CreateStringValue(number);
            checkerInfo.Add(checker);
            return checkerInfo;
        }
        /*
         * MaintenanceOperation checkInfo内容
         * @param检查类型 检查内容分类信息
         */

        public JsonArray initCheckJsonArray(string type, string lineName, string elementName, String SHOTNumber)
        {
            JsonArray checkerInfo = new JsonArray();
            JsonObject checker = new JsonObject();
            checker["type"] = JsonValue.CreateStringValue(type);
            checker["lineName"] = JsonValue.CreateStringValue(lineName);
            checker["elementName"] = JsonValue.CreateStringValue(elementName);
            checker["SHOTNumber"] = JsonValue.CreateStringValue(SHOTNumber);
            checkerInfo.Add(checker);
            return checkerInfo;
        }
        /*
          * 初始化各级别用户的json信息
          * @param 点检人姓名(level=1) 检查时间 备注
          */
        public JsonArray initCheckerJsonArray(string userName,string date, string comments)
        {
            JsonArray checkerInfo = new JsonArray();

            for(int i=1;i<= userLevelCount; i++)
            {
                JsonObject checker = new JsonObject();
                checker["name"] = i == 1 ? JsonValue.CreateStringValue(userName) : JsonValue.CreateStringValue("");
                checker["level"] = JsonValue.CreateStringValue(Convert.ToString(i));
                checker["check"] = i == 1 ? JsonValue.CreateStringValue("1") : JsonValue.CreateStringValue("0");
                checker["edit"] = JsonValue.CreateStringValue("0");
                checker["date"] = i == 1 ? JsonValue.CreateStringValue(date) : JsonValue.CreateStringValue("");
                checker["comments"] = i == 1 ? JsonValue.CreateStringValue(comments) : JsonValue.CreateStringValue("");
                checkerInfo.Add(checker);
            }
            return checkerInfo;
        }

        /*
        * 将json数据写入对应文件中
        * @param Json数据 文件名 文件所在目录 文件夹名
        */
        public async void writeJsonToFile(JsonObject content, String fileName, StorageFolder record_folder, string folderName)
        {
            // 从json数据里面提前对应的json数组
            string[] jsonString = {  "checkInfo",content["checkInfo"].Stringify(),
                                "content",content["content"].Stringify(),
                                "checkerInfo",content["checkerInfo"].Stringify()
                              };
            StorageFolder newFolder = await record_folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            StorageFile record_file = await newFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (Stream file = await record_file.OpenStreamForWriteAsync())
            {
                using (StreamWriter write = new StreamWriter(file))
                {
                    write.Write("{");
                    write.WriteLine();
                    // 将数组中数据写入对应文件中
                    for (int i = 0; i < jsonString.Length; i += 2)
                    {
                        // 一个json数组的开始
                        write.Write("   \"" + jsonString[i] + "\":[");
                        write.WriteLine();
                        //  循环数组内容
                        string strs = jsonString[i + 1];
                        int count = 2; // tab次数
                        // 去掉头尾的中括号
                        for (int j = 1; j < strs.Length-1; j++)
                        {
                            // 统计双引号的个数
                            int num = 0;
                            for (int k = 0; k < count; k++)
                            {
                                write.Write("   ");
                            }
                            while (true)
                            {
                                char str = strs[j];
                                write.Write(str);
                                count = str == '{' ? count + 1 : count;
                                num = str == '\"' ? num + 1 : num;
                                if (str == '{' || str == '}' || num >= 4)
                                {
                                    if (strs[j + 1] == ',')
                                    {
                                        j++;
                                        write.Write(strs[j]);
                                    }
                                    count = strs[j + 1] == '}' ? count - 1 : count;
                                    break;
                                }
                                j++;
                            }
                            write.WriteLine();
                        }
                        // 一个json数组的结束,若为最后一个json数据，则]后面不加,
                        string isAdd2 = i == jsonString.Length - 2 ? "" : ",";
                        write.Write("   ]" + isAdd2);
                        write.WriteLine();
                    }
                    write.Write("}");
                    write.Dispose();
                }
                file.Dispose();
            }
        }

        /*
        * 将json数据写入对应文件中
        * @param Json数据 文件名 文件所在目录 文件夹名
        */
        public async void writeJsonToFileForUser(JsonObject content, String fileName, StorageFolder record_folder)
        {
            string userInfo = content.ToString().Replace("\\", "");
            StringBuilder sb = new StringBuilder();
            int len = userInfo.Length;
            for (int i = 0; i < len; i++)
            {
                //  去掉[前面的双引号
                if(i < len - 1 && userInfo[i]=='"' && userInfo[i+1] == '[')
                {
                    continue;
                }
                sb.Append(userInfo[i]);
                // 去掉]后面的双引号
                if (userInfo[i] == ']')
                {
                    i++;
                }
            }
            StorageFile record_file = await record_folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (Stream file = await record_file.OpenStreamForWriteAsync())
            {
                using (StreamWriter write = new StreamWriter(file))
                {
                    write.Write(JsonTree(sb.ToString()));
                    write.Dispose();
                }
                file.Dispose();
            }
        }


        /// <summary>
        /// JSON字符串格式化
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string JsonTree(string json)
        {
            int level = 0;
            var jsonArr = json.ToArray();  // Using System.Linq;
            string jsonTree = string.Empty;
            for (int i = 0; i < json.Length; i++)
            {
                char c = jsonArr[i];
                if (level > 0 && '\n' == jsonTree.ToArray()[jsonTree.Length - 1])
                {
                    jsonTree += TreeLevel(level);
                }
                switch (c)
                {
                    case '[':
                        jsonTree += c + "\n";
                        level++;
                        break;
                    case ',':
                        jsonTree += c + "\n";
                        break;
                    case ']':
                        jsonTree += "\n";
                        level--;
                        jsonTree += TreeLevel(level);
                        jsonTree += c;
                        break;
                    default:
                        jsonTree += c;
                        break;
                }
            }
            return jsonTree;
        }
        /// <summary>
        /// 树等级
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static string TreeLevel(int level)
        {
            string leaf = string.Empty;
            for (int t = 0; t < level; t++)
            {
                leaf += "\t";
            }
            return leaf;
        }

        /*
         * 判断当前用户的下级是否审批过信息，若没有则返回false
         */
        public bool isJudged(JsonArray jsonArray, string userlevel)
        {
            string checked_flag = "0";
            string lowlevel_flag = "0";

            if (userlevel == "2")
            {
                lowlevel_flag = "1";
                checked_flag = jsonArray[1].GetObject()["check"].GetString();
            }
            else if (userlevel == "3")
            {
                lowlevel_flag = jsonArray[1].GetObject()["check"].GetString();
                checked_flag = jsonArray[2].GetObject()["check"].GetString();
            }
            else if (userlevel == "4")
            {
                lowlevel_flag = jsonArray[2].GetObject()["check"].GetString();
                checked_flag = jsonArray[3].GetObject()["check"].GetString();
            }
            else if (userlevel == "5")
            {
                lowlevel_flag = jsonArray[3].GetObject()["check"].GetString();
                checked_flag = jsonArray[4].GetObject()["check"].GetString();
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
        public bool isEditContent(JsonArray jsonArray, string userlevel)
        {
            string lowlevel_edit = "0";
            if (userlevel == "3")
            {
                lowlevel_edit = jsonArray[1].GetObject()["edit"].GetString();
            }
            else if (userlevel == "4")
            {
                lowlevel_edit = jsonArray[2].GetObject()["edit"].GetString();
            }
            else if (userlevel == "5")
            {
                lowlevel_edit = jsonArray[3].GetObject()["edit"].GetString();
            }
            return lowlevel_edit.Equals("1");
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
          * 获取改用户等级以下的用户的审批信息
          * @param 信息显示文本 用户等级  文件名 文件所在目录
          */
        public async void getStateText(TextBox reviewInfor,String userLevel,string checkfilename,string folderName)
        {
            if (userLevel == "1") return;
            StorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            if (folder != null)
            {
                StorageFile file = await folder.CreateFileAsync(checkfilename, CreationCollisionOption.OpenIfExists);
                if (file != null)
                {
                    var jsonText = await FileIO.ReadTextAsync(file);
                    JsonObject jsonObject = JsonObject.Parse(jsonText);
                    // 获取用户信息
                    JsonArray checkerInfo = jsonObject["checkerInfo"].GetArray();

                    reviewInfor.Text = "点检人：" + checkerInfo[0].GetObject()["name"].GetString() + ", "+ checkerInfo[0].GetObject()["date"].GetString() + " 提交\n";
                    if (userLevel == "2") return;

                    for(int i=1;i< userLevelCount; i++)
                    {
                        string level = Convert.ToString(i+1);
                        //  若等级与当前等级相等，则退出
                        if (level == userLevel) return;
                        reviewInfor.Text = reviewInfor.Text + getJobByLevel(level) + "：" + checkerInfo[i].GetObject()["name"].GetString() + ", " + checkerInfo[i].GetObject()["date"].GetString();
                        if (checkerInfo[i].GetObject()["edit"].GetString() == "0")
                        {
                            reviewInfor.Text = reviewInfor.Text + " 确认\n";
                        }
                        else
                        {
                            reviewInfor.Text = reviewInfor.Text + " 修改并确认\n";
                        }
                    }
                }
            }
        }

        /*
         * 将json数据写入对应文件中
         * @param Json数据 文件名 文件所在目录 文件夹名
         */
        public async void old_writeJsonToFile(JsonObject content, String fileName, StorageFolder record_folder, string folderName)
        {
            // 从json数据里面提前对应的json数组
            string[] jsonString = {  "checkInfo",content["checkInfo"].Stringify(),
                                "content",content["content"].Stringify(),
                                "checkerInfo",content["checkerInfo"].Stringify()
                              };
            StorageFolder newFolder = await record_folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            StorageFile record_file = await newFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (Stream file = await record_file.OpenStreamForWriteAsync())
            {
                using (StreamWriter write = new StreamWriter(file))
                {
                    write.Write("{");
                    write.WriteLine();
                    // 将数组中数据写入对应文件中
                    for (int i = 0; i < jsonString.Length; i += 2)
                    {
                        string[] strs = jsonString[i + 1].Replace("[", "").Replace("]", "").Split(',');
                        // 一个json数组的开始
                        write.Write("   \"" + jsonString[i] + "\":[");
                        write.WriteLine();
                        for (int j = 0; j < strs.Length; j++)
                        {
                            // 数据的开始点
                            if (strs[j][0] == '{')
                            {
                                write.Write("       {");
                                write.WriteLine();
                                write.Write("           " + strs[j].Substring(1, strs[j].Length - 1) + ",");
                                write.WriteLine();
                            }
                            // 数据的结束点
                            else if (strs[j][strs[j].Length - 1] == '}')
                            {
                                if (strs[j].Contains("comments"))
                                {
                                    strs[j] = strs[j].Replace("+", ",");
                                }
                                write.Write("           " + strs[j].Substring(0, strs[j].Length - 1));
                                write.WriteLine();
                                //  若为最后一条数据，则}后面不加,
                                string isAdd1 = j == strs.Length - 1 ? "" : ",";
                                write.Write("       }" + isAdd1);
                                write.WriteLine();
                            }
                            else
                            {
                                write.Write("           " + strs[j] + ",");
                                write.WriteLine();
                            }
                        }
                        // 一个json数组的结束,若为最后一个json数据，则]后面不加,
                        string isAdd2 = i == jsonString.Length - 2 ? "" : ",";
                        write.Write("   ]" + isAdd2);
                        write.WriteLine();
                    }
                    write.Write("}");
                }
            }
        }

    }
}
