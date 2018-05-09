using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Web.Http;

namespace EZFAC.PAD.src.Tools
{
    public class DataInfo
    {
        MessDialog messDialog =new MessDialog();

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

        public async void getInfo(string tableName, string level,string folderName)
        {
            // string url = "http://192.168.2.110:8800/get-checkRecord-list?table_name=" + tableName + "&level=" + level;
            string url = "http://180.161.134.61:8800/get-checkRecord-list?table_name=CHECK_RECORD&level=2";
            //string url = "http://example.com/datalist.aspx";
            JsonObject checkRecordData = new JsonObject();
            CommonOperation commonOperation = new CommonOperation();
            var httpClient = new HttpClient();
            var resourceUri = new Uri(url);
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(resourceUri);
                String content = response.Content.ToString();
                messDialog.showDialog(unicodeToString(content));
            }
            catch (Exception ex)
            { 
                url = "1111";
            }
            finally
            {
                httpClient.Dispose();
            }
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
