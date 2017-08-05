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

namespace EZFAC.PAD.src.MaintenanceLog
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MaintenanceLog : Page
    {
        private CommonOperation commonOperation = new CommonOperation();
        private CommomSetting commomSetting = new CommomSetting();
        private MessDialog messDialog = new MessDialog();
        private Dictionary<string, string> data = new Dictionary<string, string>();
        private string type = "MaintenanceLog";
        private List<String> elementNum = new List<String>();
        private List<String> elementNumTag = new List<String>();
        private String lineName = "";
        private String elementName = "";
        private String SHOTNumber = "";


        public MaintenanceLog()
        {
            this.InitializeComponent();
            TimeLog.Text = DateTime.Now.ToString();

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is Dictionary<string, string>)
            {
                // 获取导航参数
                data = (Dictionary<string, string>)e.Parameter;
                // 显示内容
                username.Text = data["username"];
                position.Text = commonOperation.getJobByLevel(data["userlevel"]);
                date.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }

        }
        private void Return_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AuthorityNavigation), data);
        }
        private void OnCommitData(object sender, RoutedEventArgs e)
        {
            JsonObject checkRecordData = new JsonObject();
            // 初始化检查的json信息
            checkRecordData.Add("checkInfo", commonOperation.initCheckJsonArray(type, jiFan.Text, pinMing.Text, SHOT.Text));
            // 设置检查内容的json信息
            JsonArray content = new JsonArray();

            getElement();
            for (int i = 0; i < elementNum.Count; i++)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(elementNumTag[i]);
                contentItem["status"] = JsonValue.CreateStringValue(elementNum[i]);
                contentItem["edit"] = JsonValue.CreateStringValue("0");
                content.Add(contentItem);
            }
            checkRecordData.Add("content", content);

            // 初始化各级别用户的json信息
            checkRecordData.Add("checkerInfo", commonOperation.initCheckerJsonArray(username.Text, date.Text, reviewInfor.Text));
            string fileName = "ykk_record_" + jiFan.Text + "_" + pinMing.Text + "_" + date.Text + ".ykk";
            // 将json数据写入对应文件中
            commonOperation.writeJsonToFile(checkRecordData, fileName, KnownFolders.PicturesLibrary, data["folderName"]);
            // 设置提示框
            messDialog.showDialog("提交成功！");


        }
        private void getElement()
        {

            elementNum.Add(element1.Text);
            elementNum.Add(element2.Text);
            elementNum.Add(element3.Text);
            elementNum.Add(element4.Text);
            elementNum.Add(element5.Text);
            elementNum.Add(element6.Text);
            elementNum.Add(element7.Text);
            elementNum.Add(element8.Text);
            elementNum.Add(element9.Text);
            elementNum.Add(element10.Text);
            elementNum.Add(element11.Text);
            elementNum.Add(element12.Text);
            CheckBox[] checkbox = { maintain_A, maintain_B, maintain_C, maintain_D, maintain_E, maintain_F, maintain_G, maintain_H, maintain_J, maintain_K, maintain_M, maintain_N, maintain_P, maintain_S, maintain_T, maintain_U };
            String reason = "";
            for (int i = 0; i < checkbox.Length; i++)
            {
                if (checkbox[i].IsChecked == true)
                {
                    reason += checkbox[i].Content;
                    reason += ";";
                }
            }
            elementNum.Add(reason);
            elementNum.Add(reviewInfor.Text);
            elementNum.Add(MaintenResult.SelectedValue.ToString());


            elementNumTag.Add(element1Tag.Text);
            elementNumTag.Add(element2Tag.Text);
            elementNumTag.Add(element3Tag.Text);
            elementNumTag.Add(element4Tag.Text);
            elementNumTag.Add(element5Tag.Text);
            elementNumTag.Add(element6Tag.Text);
            elementNumTag.Add(element7Tag.Text);
            elementNumTag.Add(element8Tag.Text);
            elementNumTag.Add(element9Tag.Text);
            elementNumTag.Add(element10Tag.Text);
            elementNumTag.Add(element11Tag.Text);
            elementNumTag.Add(element12Tag.Text);
            elementNumTag.Add("维修原因");
            elementNumTag.Add("维修内容");
            elementNumTag.Add("维修结果");
        }
    }
}
