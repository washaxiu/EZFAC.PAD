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
    class AbnormalQualityReportService
    {
        public string good = "○";
        public string bad = "×";
        public CommonOperation commonOperation = new CommonOperation();

        /*
        * 获取当前用户的不合格报告审批信息
        * @param 显示信息列表  用户等级 检查信息所在目录
        */
        public async void getApprovalList(ListView lvFiles, string userlevel, string floderName)
        {
            lvFiles.Items.Clear();
            StorageFolder record_folder = KnownFolders.PicturesLibrary;
            StorageFolder folder = await record_folder.CreateFolderAsync(floderName, CreationCollisionOption.OpenIfExists);
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
                        if (commonOperation.isJudged(checkerInfo, userlevel) == false)
                        {
                            continue;
                        }
                        lvFiles.Items.Add(new
                        {
                            // 设置检查内容
                            thingName = content[0].GetObject()["status"].GetString(),
                            badContent = content[4].GetObject()["status"].GetString(),
                            comboBox1 = content[41].GetObject()["status"].GetString(),
                            // 设置用户信息
                            checkName = checkerInfo[0].GetObject()["name"].GetString(),
                            checkDate = checkerInfo[0].GetObject()["date"].GetString(),
                            fileName = file.Name,
                            Count = Convert.ToString(count++)
                        });
                    }
                }
            }
        }


        /*
         * 审批信息回填
         * @param 文件所在目录 文件名 界面的控件数组
         */
        public async void getApprovalDetailOne(string userlevel,string folderName, string fileName, TextBox[] textBox, RadioButton[] radioButton, ComboBox[] comboBox, CalendarDatePicker[] calendarDatePicker)
        {
            StorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            if (folder != null)
            {
                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                if (file != null)
                {
                    var jsonText = await FileIO.ReadTextAsync(file);
                    JsonObject jsonObject = JsonObject.Parse(jsonText);
                    // 获取内容信息
                    JsonArray content = jsonObject["content"].GetArray();

                    int index = 0;

                    foreach (TextBox item in textBox)
                    {
                        if (!"2".Equals(userlevel)) item.IsReadOnly = true;
                        item.Text = content[index++].GetObject()["status"].GetString();
                    }
                    foreach (RadioButton item in radioButton)
                    {
                        if (index < content.Count && item.Name == content[index].GetObject()["status"].GetString())
                        {
                            item.IsChecked = true;
                            index++;
                        }
                        else
                        {
                            if (!"2".Equals(userlevel)) item.IsEnabled = false;
                        }
                    }
                    foreach (ComboBox item in comboBox)
                    {
                        if (!"2".Equals(userlevel)) item.IsEnabled = false;
                        item.SelectedItem = content[index++].GetObject()["status"].GetString();
                    }
                    foreach (CalendarDatePicker item in calendarDatePicker)
                    {
                        string dateContent = content[index++].GetObject()["status"].GetString();
                        if (!"".Equals(dateContent))
                        {
                            item.Date = Convert.ToDateTime(dateContent);
                        }
                        if (!"2".Equals(userlevel)) item.IsEnabled = false;
                    }
                    
                }
            }
        }
    }
}
