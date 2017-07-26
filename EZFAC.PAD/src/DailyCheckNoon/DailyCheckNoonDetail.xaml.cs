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
    public sealed partial class DailyCheckNoonDetail : Page
    {
        private CommonOperation commonOperation = new CommonOperation();
        private MessDialog messDialog = new MessDialog();

        public DailyCheckNoonDetail()
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
                ApprovalPosition.Text = getdata["username"];


            }
            date.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void OnCommitData(object sender, RoutedEventArgs e)
        {
            // 实例化JsonObject对象并设置用户级别信息
            JsonObject checkRecordData = commonOperation.initJsonObject();
            // 设置各字段的值
            checkRecordData["checker"] = JsonValue.CreateStringValue("");
            checkRecordData["date"] = JsonValue.CreateStringValue(date.Text);
            checkRecordData["group"] = JsonValue.CreateStringValue(MachineGroup.SelectedItem.ToString());
            checkRecordData["line"] = JsonValue.CreateStringValue(machineNo.SelectedItem.ToString());

            JsonValue good = JsonValue.CreateStringValue("good");
            JsonValue bad = JsonValue.CreateStringValue("bad");

            checkRecordData["first"] = first.IsOn == true ? good : bad;
            checkRecordData["two"] = two.IsOn == true ? good : bad;
            checkRecordData["three"] = three.IsOn == true ? good : bad;
            checkRecordData["four"] = four.IsOn == true ? good : bad;
            checkRecordData["five"] = five.IsOn == true ? good : bad;
            checkRecordData["six"] = six.IsOn == true ? good : bad;
            checkRecordData["seven"] = seven.IsOn == true ? good : bad;
            checkRecordData["eight"] = eight.IsOn == true ? good : bad;
            checkRecordData["nine"] = nine.IsOn == true ? good : bad;
            checkRecordData["ten"] = ten.IsOn == true ? good : bad;
            checkRecordData["eleven"] = eleven.IsOn == true ? good : bad;
            checkRecordData["twelve"] = twelve.IsOn == true ? good : bad;
            checkRecordData["fourteen"] = fourteen.IsOn == true ? good : bad;
            checkRecordData["fifteen"] = fifteen.IsOn == true ? good : bad;
            checkRecordData["sixteen"] = sixteen.IsOn == true ? good : bad;
            checkRecordData["seventeen"] = seventeen.IsOn == true ? good : bad;

            string fileName = "ykk_record_" + MachineGroup.SelectedItem.ToString() + "_" + machineNo.SelectedItem.ToString() + "_" + date.Text + ".ykk";
            // 将json数据写入对应文件中
            commonOperation.writeJsonToFile(checkRecordData, fileName, KnownFolders.PicturesLibrary, "DailyCheckNoon");
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
            }
            first.IsOn = false;
            two.IsOn = false;
            three.IsOn = false;
            four.IsOn = false;
            six.IsOn = false;
            seven.IsOn = false;
            eight.IsOn = false;
            nine.IsOn = false;
            ten.IsOn = false;
            eleven.IsOn = false;
            twelve.IsOn = false;
            fourteen.IsOn = false;
            fifteen.IsOn = false;
            sixteen.IsOn = false;
            seventeen.IsOn = false;
        }

        private void machineNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            first.IsOn = false;
            two.IsOn = false;
            three.IsOn = false;
            four.IsOn = false;
            six.IsOn = false;
            seven.IsOn = false;
            eight.IsOn = false;
            nine.IsOn = false;
            ten.IsOn = false;
            eleven.IsOn = false;
            twelve.IsOn = false;
            fourteen.IsOn = false;
            fifteen.IsOn = false;
            sixteen.IsOn = false;
            seventeen.IsOn = false;
        }
    }
}
