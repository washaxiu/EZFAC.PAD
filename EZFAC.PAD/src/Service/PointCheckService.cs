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
    class PointCheckService
    {
        public string good = "○";
        public string bad = "×";
        public CommonOperation commonOperation = new CommonOperation();


        /*
        * 获取当前用户的点检审批信息
        * @param 显示信息列表  用户等级
        */
        public async void getApprovalList(ListView lvFiles, string userlevel)
        {
            lvFiles.Items.Clear();
            StorageFolder record_folder = KnownFolders.PicturesLibrary;
            StorageFolder folder = await record_folder.CreateFolderAsync("PointCheck", CreationCollisionOption.OpenIfExists);
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
                        jsonText = jsonText.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
                        JsonObject jsonObject = JsonObject.Parse(jsonText);
                        string color = "Black";

                        // 判断当前用户的下级是否审批过信息
                        if (commonOperation.isJudged(jsonObject, userlevel) == false)
                        {
                            continue;
                        }
                        string editContent = commonOperation.getEditContent(jsonObject, userlevel);
                        // 判断当前用户的下级是否修改过信息
                        if (!editContent.Equals("0"))
                        {
                            color = "Red";
                        }

                        lvFiles.Items.Add(new
                        {
                            Group = jsonObject["group"].GetString(),
                            Line = jsonObject["line"].GetString(),
                            MachineName = "压铸线" + jsonObject["group"].GetString() + "-" + jsonObject["line"].GetString(),
                            Temp1Name = jsonObject["temp1"].GetString() == "good" ? good : bad,
                            Temp2Name = jsonObject["temp2"].GetString() == "good" ? good : bad,
                            Temp3Name = jsonObject["temp3"].GetString() == "good" ? good : bad,
                            Loop1Name = jsonObject["loop1"].GetString() == "good" ? good : bad,
                            Loop2Name = jsonObject["loop2"].GetString() == "good" ? good : bad,
                            Loop3Name = jsonObject["loop3"].GetString() == "good" ? good : bad,
                            Select1Name = jsonObject["select1"].GetString() == "good" ? good : bad,
                            Plat1Name = jsonObject["plat1"].GetString() == "good" ? good : bad,
                            CheckerName = jsonObject["checker"].GetString(),
                            CheckDate = jsonObject["date"].GetString(),
                            Count = Convert.ToString(count++),
                            CheckerLevel = userlevel,
                            EditContent = editContent,
                            Color = color
                        });
                    }
                }
            }
        }

        /*
        * 获取点检审批信息详情
        * @param 信息详情json字符串 审批人名 审批人等级
        */
        public Dictionary<string,string> getDetail(String jsonText,string username,string userlevel)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            // 将信息字符串转换成字符串数组
            string[] strs = jsonText.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Split("{,=}".ToCharArray());

            // 将点检信息写入
            for (int i = 1; i < strs.Length-1; i += 2)
            {
                string msg = null;
                if (strs[i].Equals("CheckerName")) msg = "checker";
                if (strs[i].Equals("CheckDate")) msg = "date";
                if (strs[i].Equals("Group")) msg = "group";
                if (strs[i].Equals("Line")) msg = "line";
                if (strs[i].Equals("MachineName")) msg = "MachineName";
                if (strs[i].Equals("Temp1Name")) msg = "temp1";
                if (strs[i].Equals("Temp2Name")) msg = "temp2";
                if (strs[i].Equals("Temp3Name")) msg = "temp3";
                if (strs[i].Equals("Loop1Name")) msg = "loop1";
                if (strs[i].Equals("Loop2Name")) msg = "loop2";
                if (strs[i].Equals("Loop3Name")) msg = "loop3";
                if (strs[i].Equals("Select1Name")) msg = "select1";
                if (strs[i].Equals("Plat1Name")) msg = "plat1";
                if (strs[i].Equals("EditContent")) msg = "editContent";
                if (msg!=null) data.Add(msg, strs[i+1]);
            }
           
            data.Add("username", username);
            data.Add("userlevel", userlevel);
            return data;
        }

        public async void mulApproval(ListView listView,string userlevel, PointCheckEntity pointCheck,string folderName)
        {
            for (int i = 0; i < listView.SelectedItems.Count; i++)
            {
                string content = listView.SelectedItems[i].ToString();
                string groupName = null, lineName = null, date = null;
                string[] strs = content.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Split("{,=}".ToCharArray());
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
                    StorageFile file =await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    if(file != null)
                    {
                        var jsonText = await FileIO.ReadTextAsync(file);
                        jsonText = jsonText.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
                        JsonObject jsonObject = JsonObject.Parse(jsonText);

                        string levelCheck = "level" + userlevel + "check";
                        string levelDate = "level" + userlevel + "date";
                        string levelApprovaler = "level" + userlevel + "approvaler";

                        jsonObject.Remove(levelCheck);
                        jsonObject.Remove(levelDate);
                        jsonObject.Remove(levelApprovaler);

                        jsonObject.Add(levelCheck, JsonValue.CreateStringValue("1"));
                        jsonObject.Add(levelDate, JsonValue.CreateStringValue(pointCheck.date));
                        jsonObject.Add(levelApprovaler, JsonValue.CreateStringValue(pointCheck.checker));
                        commonOperation.writeJsonToFile(jsonObject, fileName, KnownFolders.PicturesLibrary, folderName);
                    }
                }
            }
        }
    }
}
