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
        public async void getUserInfo()
        {
            string url = "http://180.161.134.61:8800/get-userInfo";
            //string url = "http://example.com/datalist.aspx";
            JsonObject checkRecordData = new JsonObject();
            CommonOperation commonOperation = new CommonOperation();
            string fileName = "user.json";
            var httpClient = new HttpClient();
            var resourceUri = new Uri(url);
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(resourceUri);
                String content = response.Content.ToString();
                String newContent = content.Replace("\\",String.Empty);          
                checkRecordData.Add("Users", JsonValue.CreateStringValue(newContent));
                commonOperation.writeJsonToFileForUser(checkRecordData, fileName, KnownFolders.PicturesLibrary);
            }
            catch (TaskCanceledException ex)
            { // Handle request being canceled due to timeout. } catch (HttpRequestException ex) { // Handle other possible exceptions. }
            }
            httpClient.Dispose();
        }

        public async void getInfo(string tableName, string level)
        {
            string url = "http://180.161.134.61:8800/get-checkRecord-list?table=" + tableName + "&level=" + level;
            //string url = "http://example.com/datalist.aspx";
            JsonObject checkRecordData = new JsonObject();
            CommonOperation commonOperation = new CommonOperation();
            string fileName = "user.json";
            var httpClient = new HttpClient();
            var resourceUri = new Uri(url);
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(resourceUri);
                String content = response.Content.ToString();
                String newContent = content.Replace("\\", String.Empty);
                checkRecordData.Add("Users", JsonValue.CreateStringValue(newContent));
                commonOperation.writeJsonToFileForUser(checkRecordData, fileName, KnownFolders.PicturesLibrary);
            }
            catch (TaskCanceledException ex)
            { // Handle request being canceled due to timeout. } catch (HttpRequestException ex) { // Handle other possible exceptions. }
            }
            httpClient.Dispose();
        }
    }
}
