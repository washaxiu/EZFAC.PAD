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
using EZFAC.PAD.src.Tools;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EZFAC.PAD
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UnqualifiedReport : Page
    {
        private JsonObject checkRecordData = new JsonObject();
        private Dictionary<string, string> data = new Dictionary<string, string>();
        private CommonOperation commonOperation = new CommonOperation();
        private MessDialog messDialog = new MessDialog();

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
                data = (Dictionary<string, string>)e.Parameter;
                // 显示内容
                username.Text = data["username"];
                underTaker.Text = data["username"];
            }
            date.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            // 初始化文本数组
            TextBox[] textBox = { underTaker, unqualifiedContent, classificationNo, description, material, finish, weight,
                                  founder, ExceptionDiscription,ExceptionObject,reasonProcess,machineNo,modelNo,
                                  unfitNo,reason,solution};
            // 初始化日期选择数组
            CalendarDatePicker[] calendarDatePicker = { foundDate, badProductDate, signInDate };
            // 初始化单选数组
            RadioButton[] radioButton = { groupa, groupb, groupc, groupd, groupe, groupf,
                                          group1,group2,group3,
                                          groupWork1,groupWork2,
                                          group4M1,group4M2,group4M3,group4M4};
       /*     if (!isOK(textBox, calendarDatePicker))
            {
                messDialog.showDialog("请完善表格！");
                return;
            }*/

            JsonObject checkRecordData = new JsonObject();
            // 初始化检查的json信息
            checkRecordData.Add("checkInfo", commonOperation.initCheckJsonArray("UnqualifiedReport", "", ""));

            // 设置检查内容的json信息
            JsonArray content = new JsonArray();
            // 初始化文本内容
            foreach (TextBox item in textBox)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(item.Name);
                contentItem["status"] = JsonValue.CreateStringValue(item.Text);
                contentItem["edit"] = JsonValue.CreateStringValue("0");
                content.Add(contentItem);
            }
            // 初始化日期选择内容
            foreach (CalendarDatePicker item in calendarDatePicker)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(item.Name);
                contentItem["status"] = JsonValue.CreateStringValue(item.Date.ToString().Split(' ')[0]);
                contentItem["edit"] = JsonValue.CreateStringValue("0");
                content.Add(contentItem);
            }
            // 初始化单选内容
            foreach (RadioButton item in radioButton)
            {
                if(item.IsChecked == true)
                {
                    JsonObject contentItem = new JsonObject();
                    contentItem["name"] = JsonValue.CreateStringValue(item.GroupName);
                    contentItem["status"] = JsonValue.CreateStringValue(item.Name);
                    contentItem["edit"] = JsonValue.CreateStringValue("0");
                    content.Add(contentItem);
                }
            }
            checkRecordData.Add("content", content);
            // 初始化各级别用户的json信息
            checkRecordData.Add("checkerInfo", commonOperation.initCheckerJsonArray(username.Text, date.Text, ""));
            string fileName = "UnqualifiedReport_" + classificationNo.Text + "_" + date.Text + ".ykk";
            // 将json数据写入对应文件中
            commonOperation.writeJsonToFile(checkRecordData, fileName, KnownFolders.PicturesLibrary, data["folderName"]);
            // 设置提示框
            messDialog.showDialog("点检成功！");
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AuthorityNavigation), data);
        }

        //  判断常规文本和时间文本是否填入值
        private bool isOK(TextBox[] textBox, CalendarDatePicker[] calendarDatePicker)
        {
            for(int i = 0; i < textBox.Length; i++)
            {
                if ("".Equals(textBox[i].Text.Trim()))
                {
                    return false;
                }
            }
            for (int i = 0; i < calendarDatePicker.Length; i++)
            {
                if ("".Equals(calendarDatePicker[i].Date.ToString().Trim()))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
