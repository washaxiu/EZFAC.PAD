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
using EZFAC.PAD.src;
using Windows.UI.Xaml.Media.Imaging;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EZFAC.PAD
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DailyCheckMorning : Page
    {
        private CommonOperation commonOperation = new CommonOperation();
        private CommomSetting commomSetting = new CommomSetting();
        private MessDialog messDialog = new MessDialog();
        private Dictionary<string, string> data = new Dictionary<string, string>();
        private JsonValue good = JsonValue.CreateStringValue("good");
        private JsonValue bad = JsonValue.CreateStringValue("bad");
        private string type = "DieCasting";
        private string groupName = "A";
        private string lineName = "01";
        private JsonObject group = new JsonObject();
        List<string[]> lineList = new List<string[]>();

        public DailyCheckMorning()
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
                ApprovalPosition.Text = data["username"];
            }
            date.Text = DateTime.Now.ToString("yyyy-MM-dd");

            group = commomSetting.getGroupAndLine();
            string [] groupList = group["group"].ToString().Replace("\"","").Split(',');
            MachineGroup.ItemsSource = groupList;
            MachineGroup.SelectedIndex = 0;                  
        }

        private void OnCommitData(object sender, RoutedEventArgs e)
        {
            JsonObject checkRecordData = new JsonObject();
            ToggleSwitch[] toggleSwitch = { first, two, three, five, six, seven, eight, fourteen, fifteen, sixteen, seventeen, eighteen };
            TextBox[] textBox = { four, zhouqi, nozzleTemp, GOOSENECKTemp, fuTemp1, fuTemp2 };
            // 初始化检查的json信息
            checkRecordData.Add("checkInfo", commonOperation.initCheckJsonArray(type, groupName, lineName));
            // 设置检查内容的json信息
            JsonArray content = new JsonArray();

            JsonObject contentItem1 = new JsonObject();
            contentItem1["name"] = JsonValue.CreateStringValue(machineModel.Name);
            contentItem1["status"] = JsonValue.CreateStringValue(machineModel.SelectedItem.ToString());
            contentItem1["edit"] = JsonValue.CreateStringValue("0");
            content.Add(contentItem1);

            JsonObject contentItem2 = new JsonObject();
            contentItem2["name"] = JsonValue.CreateStringValue(work.Name);
            contentItem2["status"] = JsonValue.CreateStringValue(work.SelectedItem.ToString());
            contentItem2["edit"] = JsonValue.CreateStringValue("0");
            content.Add(contentItem2);

            foreach (ToggleSwitch item in toggleSwitch)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(item.Name);
                contentItem["status"] = item.IsOn == true ? good : bad;
                contentItem["edit"] = JsonValue.CreateStringValue("0");
                content.Add(contentItem);
            }
            foreach (TextBox text in textBox)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(text.Name);
                contentItem["status"] = JsonValue.CreateStringValue(text.Text);
                contentItem["edit"] = JsonValue.CreateStringValue("0");
                content.Add(contentItem);
            }
            checkRecordData.Add("content", content);
            // 初始化各级别用户的json信息
            checkRecordData.Add("checkerInfo", commonOperation.initCheckerJsonArray(username.Text, date.Text, ""));
            string fileName = "ykk_record_" + groupName + "_" + lineName + "_" + date.Text + ".ykk";
            // 将json数据写入对应文件中
            commonOperation.writeJsonToFile(checkRecordData, fileName, KnownFolders.PicturesLibrary, "DailyCheckMorning");
            // 设置提示框
            messDialog.showDialog("点检成功！");
        }

        private void MachineGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string contnet = (String)MachineGroup.SelectedItem;
            groupName = contnet[contnet.Length - 1] + "";
            string[] line = group[groupName].ToString().Replace("\"", "").Split(',');
            machineNo.ItemsSource = line;
            machineNo.SelectedIndex = 0;

            ToggleSwitch[] toggleSwitch = { first, two, three, five, six, seven, eight, fourteen, fifteen, sixteen, seventeen, eighteen };
            TextBox[] textBox = { four, zhouqi, nozzleTemp, GOOSENECKTemp, fuTemp1, fuTemp2 };
            // 初始化点检内容
            for (int i = 0; i < toggleSwitch.Length; i++)
            {
                toggleSwitch[i].IsOn = false;
            }
            for (int i = 0; i < textBox.Length; i++)
            {
                textBox[i].Text = "";
            }
        }

        private void machineNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (machineNo.SelectedItem == null)
            {
                machineNo.SelectedIndex = 0;
            }
            string content = machineNo.SelectedValue.ToString();
            lineName = content.Substring(1,3);
            ToggleSwitch[] toggleSwitch = { first, two, three, five, six, seven, eight, fourteen, fifteen, sixteen, seventeen, eighteen };
            TextBox[] textBox = { four, zhouqi, nozzleTemp, GOOSENECKTemp, fuTemp1, fuTemp2 };
            // 初始化点检内容
            for (int i = 0; i < toggleSwitch.Length; i++)
            {
                toggleSwitch[i].IsOn = false;
            }
            for (int i = 0; i < textBox.Length; i++)
            {
                textBox[i].Text = "";
            }
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AuthorityNavigation), data);
        }


        private async void Image_Click(object sender, PointerRoutedEventArgs e)
        {
            Image image = new Image();
            Image image1 = (Image)sender;
            image.Source = image1.Source;
            ContentDialog showImage = new ContentDialog()
            {
                Title = "消息提示",
                Content = image,
                PrimaryButtonText = "确定",
                SecondaryButtonText = "取消",
                FullSizeDesired = false,
            };
            await showImage.ShowAsync();
        }
    }
}
