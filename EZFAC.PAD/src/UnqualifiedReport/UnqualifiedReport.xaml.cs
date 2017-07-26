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
    public sealed partial class UnqualifiedReport : Page
    {
        private JsonObject checkRecordData = new JsonObject();
        private StorageFile record_file;//UWP 采用StorageFile来读写文件
        private StorageFolder record_folder;//folder来读写文件夹        
        private string groupName = "A";
        private string lineName = "01";
        public UnqualifiedReport()
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
                username.Text = getdata["username"];

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


            checkRecordData["unqualifiedContent"] = JsonValue.CreateStringValue(unqualifiedContent.Text);
            checkRecordData["classificationNo"] = JsonValue.CreateStringValue(classificationNo.Text);
            checkRecordData["description"] = JsonValue.CreateStringValue(description.Text);
            String messRateValue = "";
            if (groupa.IsChecked == true)
            {
                messRateValue = (String)groupa.Content;
            }
            else if(groupb.IsChecked == true)
            {
                messRateValue = (String)groupb.Content;
            }
            else if (groupc.IsChecked == true)
            {
                messRateValue = (String)groupc.Content;
            }
            else if (groupd.IsChecked == true)
            {
                messRateValue = (String)groupd.Content;
            }
            else if (groupe.IsChecked == true)
            {
                messRateValue = (String)groupe.Content;
            }
            else if (groupf.IsChecked == true)
            {
                messRateValue = (String)groupf.Content;
            }
            checkRecordData["messRate"] = JsonValue.CreateStringValue(messRateValue);
            checkRecordData["material"] = JsonValue.CreateStringValue(material.Text);
            checkRecordData["finish"] = JsonValue.CreateStringValue(finish.Text);
            checkRecordData["weight"] = JsonValue.CreateStringValue(weight.Text);
            checkRecordData["foundDate"] = JsonValue.CreateStringValue(foundDate.Date.ToString());
            String foundProjectValue = "";
            if (group1.IsChecked == true)
            {
                foundProjectValue = (String)group1.Content;
            }
            else if (group2.IsChecked == true)
            {
                foundProjectValue = (String)group2.Content;
            }
            else if (group3.IsChecked == true)
            {
                foundProjectValue = (String)group3.Content;
            }
            checkRecordData["foundProject"] = JsonValue.CreateStringValue(foundProjectValue);
            checkRecordData["founder"] = JsonValue.CreateStringValue(founder.Text);
            checkRecordData["ExceptionDiscription"] = JsonValue.CreateStringValue(ExceptionDiscription.Text);
            checkRecordData["ExceptionObject"] = JsonValue.CreateStringValue(ExceptionObject.Text);
            checkRecordData["reasonProcess"] = JsonValue.CreateStringValue(reasonProcess.Text);
            String workValue = "";
            if (groupWork1.IsChecked == true)
            {
                workValue = (String)groupWork1.Content;
            }
            else if (groupWork2.IsChecked == true)
            {
                workValue = (String)groupWork2.Content;
            }
            checkRecordData["workValue"] = JsonValue.CreateStringValue(workValue);
            checkRecordData["badProductDate"] = JsonValue.CreateStringValue(badProductDate.Date.ToString());
            String _4MValue = "";
            if (group4M1.IsChecked == true)
            {
                _4MValue = (String)group4M1.Content;
            }
            else if (group4M2.IsChecked == true)
            {
                _4MValue = (String)group4M2.Content;
            }
            else if (group4M3.IsChecked == true)
            {
                _4MValue = (String)group4M3.Content;
            }
            checkRecordData["4M"] = JsonValue.CreateStringValue(_4MValue);
            checkRecordData["machineNo"] = JsonValue.CreateStringValue(machineNo.Text);
            checkRecordData["unfitNo"] = JsonValue.CreateStringValue(unfitNo.Text);
            checkRecordData["modelNo"] = JsonValue.CreateStringValue(modelNo.Text);
            checkRecordData["signInDate"] = JsonValue.CreateStringValue(signInDate.Date.ToString());
            checkRecordData["reason"] = JsonValue.CreateStringValue(reason.Text);
            checkRecordData["solution"] = JsonValue.CreateStringValue(solution.Text);



            // 显示JSON对象的字符串表示形式
            string jstr = checkRecordData.Stringify();
            record_folder = KnownFolders.PicturesLibrary;
            record_file = await record_folder.CreateFileAsync("ykk_UnqualifiedReport_" + groupName + "_" + lineName + "_" + date.Text + ".ykk", CreationCollisionOption.ReplaceExisting);
            using (Stream file = await record_file.OpenStreamForWriteAsync())
            {
                using (StreamWriter write = new StreamWriter(file))
                {
                    write.Write(jstr);
                }
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {

        }

        private void group4M1_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
