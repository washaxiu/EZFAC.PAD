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
    public sealed partial class SemiFinishedCheck : Page
    {
        private JsonObject checkRecordData = new JsonObject();
        private StorageFile record_file;//UWP 采用StorageFile来读写文件
        private StorageFolder record_folder;//folder来读写文件夹        
        private string groupName = "A";
        private string lineName = "01";
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
                Dictionary<string, string> getdata = (Dictionary<string, string>)e.Parameter;
                // 显示内容
                string level = getdata["level"];
               
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


            if (surface.IsOn)
            {
                checkRecordData["surface"] = JsonValue.CreateStringValue(surface.OnContent.ToString());
            }
            else
            {
                checkRecordData["surface"] = JsonValue.CreateStringValue(surface.OffContent.ToString());
            }
            if (damage_SB171.IsOn)
            {
                checkRecordData["damage_SB171"] = JsonValue.CreateStringValue(damage_SB171.OnContent.ToString());
            }
            else
            {
                checkRecordData["damage_SB171"] = JsonValue.CreateStringValue(damage_SB171.OffContent.ToString());
            }
            if (PINDamage.IsOn)
            {
                checkRecordData["PINDamage"] = JsonValue.CreateStringValue(PINDamage.OnContent.ToString());
            }
            else
            {
                checkRecordData["PINDamage"] = JsonValue.CreateStringValue(PINDamage.OffContent.ToString());
            }
            if (damage_SB251.IsOn)
            {
                checkRecordData["damage_SB251"] = JsonValue.CreateStringValue(damage_SB251.OnContent.ToString());
            }
            else
            {
                checkRecordData["damage_SB251"] = JsonValue.CreateStringValue(damage_SB251.OffContent.ToString());
            }
            if (filling.IsOn)
            {
                checkRecordData["filling"] = JsonValue.CreateStringValue(filling.OnContent.ToString());
            }
            else
            {
                checkRecordData["filling"] = JsonValue.CreateStringValue(filling.OffContent.ToString());
            }
            if (xingpian.IsOn)
            {
                checkRecordData["xingpian"] = JsonValue.CreateStringValue(xingpian.OnContent.ToString());
            }
            else
            {
                checkRecordData["xingpian"] = JsonValue.CreateStringValue(xingpian.OffContent.ToString());
            }
            if (b3_b4_b5_b7.IsOn)
            {
                checkRecordData["b3_b4_b5_b7"] = JsonValue.CreateStringValue(b3_b4_b5_b7.OnContent.ToString());
            }
            else
            {
                checkRecordData["b3_b4_b5_b7"] = JsonValue.CreateStringValue(b3_b4_b5_b7.OffContent.ToString());
            }
            if (b6.IsOn)
            {
                checkRecordData["b6"] = JsonValue.CreateStringValue(b6.OnContent.ToString());
            }
            else
            {
                checkRecordData["b6"] = JsonValue.CreateStringValue(b6.OffContent.ToString());
            }
            if (c8_c9_c10.IsOn)
            {
                checkRecordData["c8_c9_c10"] = JsonValue.CreateStringValue(c8_c9_c10.OnContent.ToString());
            }
            else
            {
                checkRecordData["c8_c9_c10"] = JsonValue.CreateStringValue(c8_c9_c10.OffContent.ToString());
            }
            if (b6.IsOn)
            {
                checkRecordData["coreWash"] = JsonValue.CreateStringValue(coreWash.OnContent.ToString());
            }
            else
            {
                checkRecordData["coreWash"] = JsonValue.CreateStringValue(coreWash.OffContent.ToString());
            }
            // 显示JSON对象的字符串表示形式
            string jstr = checkRecordData.Stringify();
            record_folder = KnownFolders.PicturesLibrary;
            record_file = await record_folder.CreateFileAsync("ykk_SemiFinishedCheck_" + groupName + "_" + lineName + "_" + date.Text + ".ykk", CreationCollisionOption.ReplaceExisting);
            using (Stream file = await record_file.OpenStreamForWriteAsync())
            {
                using (StreamWriter write = new StreamWriter(file))
                {
                    write.Write(jstr);
                }
            }
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
    }
}
