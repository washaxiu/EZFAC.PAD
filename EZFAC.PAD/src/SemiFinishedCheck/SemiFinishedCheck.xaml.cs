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
        private string groupName = "A";
        private string lineName = "01";
        private JsonValue good = JsonValue.CreateStringValue("good");
        private JsonValue bad = JsonValue.CreateStringValue("bad");
        private MessDialog messDialog = new MessDialog();
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
            checkRecordData.Add("checkInfo", commonOperation.initCheckJsonArray("SemiFinishedCheck", groupName, lineName));

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
            // 初始化下拉框
            foreach (ComboBox item in comboBox)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(item.Name);
                contentItem["status"] = JsonValue.CreateStringValue(item.SelectedItem.ToString());
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
            string fileName = "ykk_record_" + groupName + "_" + lineName + "_" + date.Text + ".ykk";
            // 将json数据写入对应文件中
            commonOperation.writeJsonToFile(checkRecordData, fileName, KnownFolders.PicturesLibrary, "SemiFinishedCheck");
            // 设置提示框
            messDialog.showDialog("点检成功！");

        }
        private void MachineGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if ((String)MachineGroup.SelectedItem == "压轴线A")
            {
                List<String> items = new List<string>();
                items.Add("A - 01");
                items.Add("A - 02");
                items.Add("A - 03");
                items.Add("A - 04");
                items.Add("A - 05");
                items.Add("A - 06");
                items.Add("A - 07");
                items.Add("A - 08");
                items.Add("A - 09");
                machineNo.ItemsSource = items;
                groupName = "A";

            }
            else if ((String)MachineGroup.SelectedItem == "压轴线B")
            {
                List<String> items = new List<string>();
                items.Add("B - 01");
                items.Add("B - 02");
                items.Add("B - 03");
                items.Add("B - 04");
                items.Add("B - 05");
                items.Add("B - 06");
                items.Add("B - 07");
                items.Add("B - 08");
                items.Add("B - 09");
                machineNo.ItemsSource = items;
                groupName = "B";
            }
            else if ((String)MachineGroup.SelectedItem == "压轴线C")
            {
                List<String> items = new List<string>();
                items.Add("C - 01");
                items.Add("C - 02");
                items.Add("C - 03");
                items.Add("C - 04");
                items.Add("C - 05");
                items.Add("C - 06");
                items.Add("C - 07");
                items.Add("C - 08");
                items.Add("C - 09");
                machineNo.ItemsSource = items;
                groupName = "C";

            }
            else if ((String)MachineGroup.SelectedItem == "压轴线D")
            {
                List<String> items = new List<string>();
                items.Add("D - 01");
                items.Add("D - 02");
                items.Add("D - 03");
                items.Add("D - 04");
                items.Add("D - 05");
                items.Add("D - 06");
                items.Add("D - 07");
                items.Add("D - 08");
                items.Add("D - 09");
                machineNo.ItemsSource = items;
                groupName = "D";
            }
            surface.IsOn = false;
            damage_SB171.IsOn = false;
            PINDamage.IsOn = false;
            damage_SB251.IsOn = false;
            filling.IsOn = false;
            xingpian.IsOn = false;
            b3_b4_b5_b7.IsOn = false;
            b6.IsOn = false;
            c8_c9_c10.IsOn = false;
            coreWash.IsOn = false;

        }

        private void machineNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string content = machineNo.SelectedItem.ToString();
            lineName = content.Substring(content.Length-2,2);
            surface.IsOn = false;
            damage_SB171.IsOn = false;
            PINDamage.IsOn = false;
            damage_SB251.IsOn = false;
            filling.IsOn = false;
            xingpian.IsOn = false;
            b3_b4_b5_b7.IsOn = false;
            b6.IsOn = false;
            c8_c9_c10.IsOn = false;
            coreWash.IsOn = false;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AuthorityNavigation), data);
        }
    }
}
