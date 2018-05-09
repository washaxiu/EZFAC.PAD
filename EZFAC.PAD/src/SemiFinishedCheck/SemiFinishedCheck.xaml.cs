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
    public sealed partial class SemiFinishedCheck : Page
    {
        private Dictionary<string, string> data = new Dictionary<string, string>();
        private CommonOperation commonOperation = new CommonOperation();
        private CommomSetting commomSetting = new CommomSetting();
        private string groupName = "A";
        private string lineName = "01";
        private JsonValue good = JsonValue.CreateStringValue("good");
        private JsonValue bad = JsonValue.CreateStringValue("bad");
        private MessDialog messDialog = new MessDialog();
        private JsonObject group = new JsonObject();

        public SemiFinishedCheck()
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
                string level = data["userlevel"];
                username.Text = data["username"];
            }
            date.Text = DateTime.Now.ToString("yyyy-MM-dd");
            group = commomSetting.getGroupAndLine();
            string[] groupList = group["group"].ToString().Replace("\"", "").Split(',');
            MachineGroup.ItemsSource = groupList;
            MachineGroup.SelectedIndex = 0;
        }

        private void OnCommitData(object sender, RoutedEventArgs e)
        {
            //初始化下拉框
            ComboBox[] comboBox = { separateStatus, gneck };
            //初始化文本框
            TextBox[] textBox = { item , personInCharge , HS_Num , remark };
            //初始化转换框
            ToggleSwitch[] toggleSwitch = { surface , damage_SB171 , PINDamage ,
                damage_SB251 , filling , xingpian , b3_b4_b5_b7 , b6 , c8_c9_c10 , coreWash};

            JsonObject checkRecordData = new JsonObject();
            // 初始化检查的json信息
            checkRecordData.Add("checkInfo", commonOperation.initCheckJsonArray(data["folderName"], groupName, lineName));

            // 设置检查内容的json信息
            JsonArray content = new JsonArray();
            // 初始化下拉框
            foreach (ComboBox item in comboBox)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(item.Name);
                contentItem["status"] = JsonValue.CreateStringValue(item.SelectedItem.ToString());
                contentItem["edit"] = JsonValue.CreateStringValue("0");
                content.Add(contentItem);
            }
            // 初始化文本内容
            foreach (TextBox item in textBox)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(item.Name);
                contentItem["status"] = JsonValue.CreateStringValue(item.Text);
                contentItem["edit"] = JsonValue.CreateStringValue("0");
                content.Add(contentItem);
            }
            // 初始化转换框
            foreach (ToggleSwitch item in toggleSwitch)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(item.Name);
                contentItem["status"] = item.IsOn == true ? good : bad ;
                contentItem["edit"] = JsonValue.CreateStringValue("0");
                content.Add(contentItem);
            }
            checkRecordData.Add("content", content);
            // 初始化各级别用户的json信息
            checkRecordData.Add("checkerInfo", commonOperation.initCheckerJsonArray(username.Text, date.Text, ""));
            string fileName = "ykk_record_" + "SemiFinishedCheck_" + groupName + "_" + lineName + "_" + date.Text + ".ykk";
            // 将json数据写入对应文件中
            commonOperation.writeJsonToFile(checkRecordData, fileName, KnownFolders.PicturesLibrary, data["folderName"]);
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

            //初始化文本框
            TextBox[] textBox = { item, personInCharge, HS_Num, remark };
            //初始化转换框
            ToggleSwitch[] toggleSwitch = { surface , damage_SB171 , PINDamage ,
                damage_SB251 , filling , xingpian , b3_b4_b5_b7 , b6 , c8_c9_c10 , coreWash};

            foreach (TextBox item in textBox)
            {
                item.Text = "";
            }
            foreach(ToggleSwitch item in toggleSwitch)
            {
                item.IsOn = false;
            }
        }

        private void machineNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (machineNo.SelectedItem == null)
            {
                machineNo.SelectedIndex = 0;
            }
            string content = machineNo.SelectedValue.ToString();
            lineName = content.Substring(content.Length-2,2);

            //初始化文本框
            TextBox[] textBox = { item, personInCharge, HS_Num, remark };
            //初始化转换框
            ToggleSwitch[] toggleSwitch = { surface , damage_SB171 , PINDamage ,
                damage_SB251 , filling , xingpian , b3_b4_b5_b7 , b6 , c8_c9_c10 , coreWash};

            foreach (TextBox item in textBox)
            {
                item.Text = "";
            }
            foreach (ToggleSwitch item in toggleSwitch)
            {
                item.IsOn = false;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AuthorityNavigation), data);
        }

        // 点击查看大图
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
