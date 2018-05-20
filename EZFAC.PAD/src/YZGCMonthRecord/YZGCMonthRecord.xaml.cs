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
    public sealed partial class YZGCMonthRecord : Page
    {
        //private JsonObject checkRecordData = new JsonObject();
       // private StorageFile record_file;//UWP 采用StorageFile来读写文件
        //private StorageFolder record_folder;//folder来读写文件夹        
        private string groupName = "A";
        private string lineName = "01";
        private CommonOperation commonOperation = new CommonOperation();
        private CommomSetting commomSetting = new CommomSetting();
        private string type = "YZGCMonthRecord";
        private Dictionary<string, string> data = new Dictionary<string, string>();
        private JsonValue good = JsonValue.CreateStringValue("good");
        private JsonValue bad = JsonValue.CreateStringValue("bad");
        private MessDialog messDialog = new MessDialog();

        public YZGCMonthRecord()
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
                date.Text = DateTime.Now.ToString("yyyy-MM-dd");
                // 获取导航参数
                Dictionary<string, string> getdata = (Dictionary<string, string>)e.Parameter;
                // 显示内容          
               
            }
        }

        private void OnCommitData(object sender, RoutedEventArgs e)
        {
            JsonObject checkRecordData = new JsonObject();
            ToggleSwitch[] toggleSwitch = { Temp1, Temp2, Temp3, Temp4, Temp5, Temp6, Temp7, Temp8 };
            ToggleSwitch[] maintenanceSwitch = { Maintenance1, Maintenance2, Maintenance3, Maintenance4, Maintenance5, Maintenance6, Maintenance7, Maintenance8 };
            // 初始化检查的json信息
            checkRecordData.Add("checkInfo", commonOperation.initCheckJsonArray(type, groupName, lineName));
            // 设置检查内容的json信息
            JsonArray content = new JsonArray();
            for(int i = 0; i < toggleSwitch.Length; i++)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(toggleSwitch[i].Name);
                string toggle = toggleSwitch[i].IsOn == true ? "good" : "bad";         
                String maintenance = maintenanceSwitch[i].IsOn == true ? "good" : "bad";
                contentItem["status"] = JsonValue.CreateStringValue(toggle + "-" + maintenance);
                contentItem["edit"] = JsonValue.CreateStringValue("0");
                content.Add(contentItem);
            }
            checkRecordData.Add("content", content);
            // 初始化各级别用户的json信息
            checkRecordData.Add("checkerInfo", commonOperation.initCheckerJsonArray(username.Text, date.Text, reviewInfor.Text));
            string fileName = "ykk_record_" + groupName + "_" + lineName + "_" + date.Text + ".ykk";
            // 将json数据写入对应文件中
            commonOperation.writeJsonToFile(checkRecordData, fileName, KnownFolders.PicturesLibrary, data["folderName"]);
            // 设置提示框
            messDialog.showDialog("点检成功！");
        }

        private void OnMachineGroupChanged(object sender, RoutedEventArgs e)
        {
            //  初始化线数组
            RadioButton[] radioButton = { line01,line02,line03,line04,line05,line06,line07,line08,line09,
                                          line10,line11,line12,line13,line14,line15,line16,line17,line18
                                        };
            ToggleSwitch[] toggleSwitch = { Temp1, Temp2, Temp3, Temp4,Temp5,Temp6,Temp7,Temp8};
            ToggleSwitch[] maintenanceSwitch = { Maintenance1, Maintenance2, Maintenance3, Maintenance4, Maintenance5, Maintenance6, Maintenance7, Maintenance8 };

            line01.IsChecked = true;
            //  根据机组选定情况设置groupName的值
            if (groupa.IsChecked == true)
            {
                groupName = "A";
            }
            else if (groupb.IsChecked == true)
            {
                groupName = "B";
            }
            else if (groupc.IsChecked == true)
            {
                groupName = "C";
            }
            else if (groupd.IsChecked == true)
            {
                groupName = "D";
            }
            // 根据组名确定显示的机番以及其名称
            commomSetting.setLineByGroup(groupName, radioButton);
            // 初始化点检内容
            for (int i = 0; i < toggleSwitch.Length; i++)
            {
                toggleSwitch[i].IsOn = false;
            }
            for(int i = 0; i < maintenanceSwitch.Length; i++)
            {
                maintenanceSwitch[i].IsOn = false;
            }
            reviewInfor.Text = "";

        }

        private void OnLineChecked(object sender, RoutedEventArgs e)
        {
            //  初始化线数组
            RadioButton[] radioButton = { line01,line02,line03,line04,line05,line06,line07,line08,line09,
                                          line10,line11,line12,line13,line14,line15,line16,line17,line18
                                        };
            //  初始化内容数组
            ToggleSwitch[] toggleSwitch = { Temp1, Temp2, Temp3, Temp4,Temp5,Temp6,Temp7,Temp8};
            ToggleSwitch[] maintenanceSwitch = { Maintenance1,Maintenance2,Maintenance3,Maintenance4,Maintenance5,Maintenance6,Maintenance7,Maintenance8};
            //  根据机番选定情况设置lineName的值
            for (int i = 0; i < radioButton.Length; i++)
            {
                if (radioButton[i].IsChecked == true)
                {
                    //  若（i+1）为个位数则在前面加个0
                    lineName = (i + 1) < 10 ? "0" + Convert.ToString(i + 1) : Convert.ToString(i + 1);
                }
            }
            // 初始化点检内容
            for (int i = 0; i < toggleSwitch.Length; i++)
            {
                toggleSwitch[i].IsOn = false;
            }
            for(int j = 0; j < maintenanceSwitch.Length; j++)
            {
                maintenanceSwitch[j].IsOn = false;
            }
            reviewInfor.Text = "";
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AuthorityNavigation), data);
        }

    }
}
