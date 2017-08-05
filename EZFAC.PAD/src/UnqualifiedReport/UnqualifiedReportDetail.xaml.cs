using EZFAC.PAD.src.Service;
using EZFAC.PAD.src.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EZFAC.PAD.src.UnqualifiedReport
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UnqualifiedReportDetail : Page
    {
        private CommonOperation commonOperation = new CommonOperation();
        private UnqualifiedReportService unqualifiedReportService = new UnqualifiedReportService();
        private MessDialog messDialog = new MessDialog();
        private Dictionary<string, string> data = new Dictionary<string, string>();

        public UnqualifiedReportDetail()
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
                ApprovalListUser.Text = data["username"];
                // 获取职位
                ApprovalListPosition.Text = commonOperation.getJobByLevel(data["userlevel"]);
                
                // 初始化文本数组
                TextBox[] textBox = {  unqualifiedContent, classificationNo, description, material, finish, weight,
                                  founder, ExceptionDiscription,ExceptionObject,reasonProcess,machineNo,modelNo,
                                  unfitNo,reason,solution};
                // 初始化日期选择数组
                CalendarDatePicker[] calendarDatePicker = { foundDate, badProductDate, signInDate };
                // 初始化单选数组
                RadioButton[] radioButton = { groupa, groupb, groupc, groupd, groupe, groupf,
                                          group1,group2,group3,
                                          groupWork1,groupWork2,
                                          group4M1,group4M2,group4M3,group4M4};
                // 获取审批信息并回填
                unqualifiedReportService.getApprovalDetail(data["floderName"], data["fileName"], textBox, calendarDatePicker, radioButton);
            }
            date.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UnqualifiedReportList), data);
        }

        private async void Confirm_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(data["floderName"], CreationCollisionOption.OpenIfExists);
            if (folder != null)
            {
                StorageFile file = await folder.CreateFileAsync(data["fileName"], CreationCollisionOption.OpenIfExists);
                if (file != null)
                {
                    var jsonText = await FileIO.ReadTextAsync(file);
                    JsonObject jsonObject = JsonObject.Parse(jsonText);
                    // 获取用户信息
                    JsonArray checkerInfo = jsonObject["checkerInfo"].GetArray();

                    JsonObject item = new JsonObject();
                    item["name"] = JsonValue.CreateStringValue(data["username"]);
                    item["level"] = JsonValue.CreateStringValue(data["userlevel"]);
                    item["check"] = JsonValue.CreateStringValue("1");
                    item["edit"] = JsonValue.CreateStringValue("0");
                    item["date"] = JsonValue.CreateStringValue(date.Text);
                    item["comments"] = JsonValue.CreateStringValue("");

                    for (int j = 0; j < checkerInfo.Count; j++)
                    {
                        string level = Convert.ToString(j + 1);
                        if (level == data["userlevel"])
                        {
                            checkerInfo[j] = item;
                        }
                    }
                    commonOperation.writeJsonToFile(jsonObject, data["fileName"], KnownFolders.PicturesLibrary, data["floderName"]);
                }
            }
            messDialog.showDialog("审批成功！");
        }
    }
}
