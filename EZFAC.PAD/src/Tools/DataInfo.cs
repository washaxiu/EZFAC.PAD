using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

namespace EZFAC.PAD.src.Tools
{
    public class DataInfo
    {
        public static string[] checkRecordDb = { "temp1", "temp2", "temp3" , "loop1", "loop2", "loop3", "select1", "plat1" };
        public static string[] checkRecordText = { "Temp1", "Temp2", "Temp3", "Loop1", "Loop2", "Loop3", "select1", "Plat1" };

        MessDialog messDialog =new MessDialog();
        CommonOperation commonOperation = new CommonOperation();

        public async void getUserInfo()
        {
            string url = "http://180.161.134.61:8800/get-userInfo";
            JsonObject checkRecordData = new JsonObject();
            CommonOperation commonOperation = new CommonOperation();
            string fileName = "user.json";
            var httpClient = new HttpClient();
            var resourceUri = new Uri(url);
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(resourceUri);
                String content = response.Content.ToString();
                checkRecordData.Add("Users", JsonValue.CreateStringValue(content));
                commonOperation.writeJsonToFileForUser(checkRecordData, fileName, KnownFolders.PicturesLibrary);
            }
            catch(Exception e)
            {
                fileName = "111";
            }
            finally
            {
                httpClient.Dispose();
            }
        }

        public async void getInfo(string tableName, string level,string folderName, ContentDialog dialog)
        {
            string url = "http://192.168.2.110:8800/get-checkRecord-list?table_name=" + tableName + "&level=" + level;
           // string url = "http://180.161.134.61:8800/get-checkRecord-list?table_name=CHECK_RECORD&level=2";
            // string url = "http://192.168.2.110:8800/get-checkRecord-list?table_name=CHECK_RECORD&level=1";
            JsonObject checkRecordData = new JsonObject();
            CommonOperation commonOperation = new CommonOperation();
            var httpClient = new HttpClient();
            var resourceUri = new Uri(url);
            string msg = null;
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(resourceUri);
                String content = response.Content.ToString();
                if (content != null)
                {
                    content = unicodeToString(content);
                  //  messDialog.showDialog(content);
                    List<Dictionary<string,string>> list = dataHandle(content);
                    dataToFile(list,folderName);
                    msg = "获取数据成功";
                }
                else
                {
                    msg = "服务器数据没有更新";
                }
            }
            catch (Exception ex)
            {
                msg = "获取数据失败，请稍后再试";
            }
            finally
            {
                httpClient.Dispose();
                // 设置提示框
                dialog.Content = msg;
                await dialog.ShowAsync();
            }
        }

        public void dataToFile(List<Dictionary<string, string>> list,string folderName)
        {
            JsonObject checkRecordData = null;
            foreach (Dictionary<string, string> dic in list)
            {
                checkRecordData = new JsonObject();
                checkRecordData.Add("checkInfo", getCheckInfo(dic));
                checkRecordData.Add("content", getContent(dic,folderName));
                checkRecordData.Add("checkerInfo", getCheckerInfo(dic));
                commonOperation.writeJsonToFile(checkRecordData, dic["fileName"], KnownFolders.PicturesLibrary, folderName);
            }
        }

        // 初始化点检信息
        public static JsonArray getCheckInfo(Dictionary<string, string> dic)
        {
            string type = dic["type"];
            JsonArray checkInfo = new JsonArray();
            JsonObject check = new JsonObject();
            if ("MaintenanceLog".Equals(type))
            {
                check["type"] = JsonValue.CreateStringValue(dic["type"]);
                check["lineName"] = JsonValue.CreateStringValue(dic["lineName"]);
                check["elementName"] = JsonValue.CreateStringValue(dic["elementName"]);
                check["SHOTNumber"] = JsonValue.CreateStringValue(dic["SHOTNumber"]);
            }
            else
            {
                check["type"] = JsonValue.CreateStringValue(dic["type"]);
                check["group"] = JsonValue.CreateStringValue(dic["group1"]);
                check["number"] = JsonValue.CreateStringValue(dic["number"]);
            }
            checkInfo.Add(check);
            return checkInfo;
        }

        private static JsonArray getContent(Dictionary<string, string> dic, string folderName)
        {
            JsonArray contents = new JsonArray();
            JsonObject content = null;
            string[] db = null , text = null;
            if (folderName.Equals("CheckRecord"))
            {
                db = checkRecordDb;
                text = checkRecordText;
            }
            string edit = dic["checkEdit"];
            for (int i = 0; i < db.Length; i++)
            {
                content = new JsonObject();
                content["name"] = JsonValue.CreateStringValue(text[i]);
                content["status"] = JsonValue.CreateStringValue(dic[db[i]]);
                content["edit"] = JsonValue.CreateStringValue(edit[i]+"");
                contents.Add(content);
            }
            return contents;
        }

        // 初始化审批信息
        private static JsonArray getCheckerInfo(Dictionary<string, string> dic)
        {
            string checkerEdit = dic["checkerEdit"];
            string check = dic["isCheck"];
            int level = int.Parse(dic["level"]);
            JsonArray checkerInfo = new JsonArray();
            JsonObject checker = null;
            for (int i = 1; i <= 5; i++)
            {
                checker = new JsonObject();
                checker["name"] = JsonValue.CreateStringValue(dic["name" + i]);
                checker["level"] = JsonValue.CreateStringValue(i + "");
                checker["check"] = JsonValue.CreateStringValue(check[i - 1] + "");
                checker["edit"] = JsonValue.CreateStringValue(checkerEdit[i - 1] + "");
                checker["date"] = JsonValue.CreateStringValue(dic["date" + i]);
                checker["comments"] = JsonValue.CreateStringValue(dic["comments" + i]);
                checkerInfo.Add(checker);
            }
            return checkerInfo;
        }

        // 将获取的数据转换成对应的List<dictionary>
        public static List<Dictionary<string, string>> dataHandle(string content)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            Dictionary<string, string> dic = null;
            string[] detail = content.Split('}'), rowDetails = null, keyAndValue = null;
            string rowContent = null;
            for (int i = 0; i < detail.Length - 1; i++)
            {
                dic = new Dictionary<string, string>();
                rowContent = detail[i].Substring(2).Replace("\"", "");
                rowDetails = rowContent.Split(',');
                foreach (string rowDetail in rowDetails)
                {
                    keyAndValue = rowDetail.Split(':');
                    if ("null".Equals(keyAndValue[1].Trim()) || keyAndValue[1].Trim() == null)
                    {
                        keyAndValue[1] = "";
                    }
                    dic.Add(keyAndValue[0].Trim(), keyAndValue[1].Trim());
                }
                list.Add(dic);
            }
            return list;
        }

        // 将转义字符转换层对应的中文
        public static string unicodeToString(string unicode)
        {
            StringBuilder sb = new StringBuilder();
            Byte[] bytes = new byte[2];
            string st = null;
            for (int i = 0; i < unicode.Length; i++)
            {
                if (i + 5 <= unicode.Length && unicode[i] == '\\' && unicode[i + 1] == 'u')
                {
                    st = unicode.Substring(i + 2, 4);
                    bytes[1] = byte.Parse(int.Parse(st.Substring(0, 2), System.Globalization.NumberStyles.HexNumber).ToString());
                    bytes[0] = byte.Parse(int.Parse(st.Substring(2, 2), System.Globalization.NumberStyles.HexNumber).ToString());
                    sb.Append(Encoding.Unicode.GetString(bytes));
                    i += 5;
                }
                else
                {
                    sb.Append(unicode[i]);
                }
            }
            return sb.ToString();
        }
    }
}
