using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EZFAC.PAD
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AbnormalQualityReportTwo : Page
    {
        private JsonObject checkRecordData = new JsonObject();
        private StorageFile record_file;//UWP 采用StorageFile来读写文件
        private StorageFolder record_folder;//folder来读写文件夹        
        private string groupName = "A";
        private string lineName = "01";
        public AbnormalQualityReportTwo()
        {
            this.InitializeComponent();
            timetag.Text = DateTime.Now.ToString();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is Dictionary<string, string>)
            {
                // 获取导航参数
                Dictionary<string, string> getdata = (Dictionary<string, string>)e.Parameter;
                // 显示内容
                string level = getdata["level"];
                if (level == "1")
                {
                    ApprovalPosition.Text = "员工";
                }
                else if (level == "2")
                {
                    ApprovalPosition.Text = "出班长";
                }
                else if (level == "3")
                {
                    ApprovalPosition.Text = "保全";
                }
                else if (level == "4")
                {
                    ApprovalPosition.Text = "课长";
                }
                else if (level == "5")
                {
                    ApprovalPosition.Text = "部门长";
                }
                else
                {
                    ApprovalPosition.Text = "未知";
                }
            }
            date.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private async void OnCommitData(object sender, RoutedEventArgs e)
        {
            // 实例化JsonObject对象
            // 设置各字段的值
            checkRecordData["checker"] = JsonValue.CreateStringValue("");
            checkRecordData["date"] = JsonValue.CreateStringValue(date.Text);
            checkRecordData["group"] = JsonValue.CreateStringValue(groupName);
            checkRecordData["line"] = JsonValue.CreateStringValue(lineName);


            
            checkRecordData["level2check"] = JsonValue.CreateStringValue("0");
            checkRecordData["level2date"] = JsonValue.CreateStringValue("");
            checkRecordData["level2approvaler"] = JsonValue.CreateStringValue("");
            checkRecordData["level3check"] = JsonValue.CreateStringValue("0");
            checkRecordData["level3date"] = JsonValue.CreateStringValue("");
            checkRecordData["level3approvaler"] = JsonValue.CreateStringValue("");
            checkRecordData["level4check"] = JsonValue.CreateStringValue("0");
            checkRecordData["level4date"] = JsonValue.CreateStringValue("");
            checkRecordData["level4approvaler"] = JsonValue.CreateStringValue("");
            checkRecordData["level5check"] = JsonValue.CreateStringValue("0");
            checkRecordData["level5date"] = JsonValue.CreateStringValue("");
            checkRecordData["level5approvaler"] = JsonValue.CreateStringValue("");

            // 显示JSON对象的字符串表示形式
            string jstr = checkRecordData.Stringify();
            record_folder = KnownFolders.PicturesLibrary;
            record_file = await record_folder.CreateFileAsync("ykk_record_" + groupName + "_" + lineName + "_" + date.Text + ".ykk", CreationCollisionOption.ReplaceExisting);
            using (Stream file = await record_file.OpenStreamForWriteAsync())
            {
                using (StreamWriter write = new StreamWriter(file))
                {
                    write.Write(jstr);
                }
            }
        }

       

        
    }
}
