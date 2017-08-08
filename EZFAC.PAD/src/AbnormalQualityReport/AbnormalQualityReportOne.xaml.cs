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
using EZFAC.PAD.src.Service;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EZFAC.PAD
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AbnormalQualityReportOne : Page
    {
        private Dictionary<string, string> data = new Dictionary<string, string>();
        private AbnormalQualityReportService abnormalQualityReportService = new AbnormalQualityReportService();
        private CommonOperation commonOperation = new CommonOperation();
        private MessDialog messDialog = new MessDialog();
        private JsonValue good = JsonValue.CreateStringValue("good");
        private JsonValue bad = JsonValue.CreateStringValue("bad");

        public AbnormalQualityReportOne()
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
                ApprovalUser.Text = data["username"];
                ApprovalPosition.Text = commonOperation.getJobByLevel(data["userlevel"]);

                string[] strs = { "早班", "中班" };
                comboBox1.ItemsSource = strs;
                comboBox1.SelectedIndex = 0;

                if(data["userlevel"] == "1" || data["userlevel"] == "2")
                {
                    Confirm.Content = "确   认";
                }
                else
                {
                    Confirm.Content = "下 一 页";
                }

                if (data.ContainsKey("fileName"))
                {
                    // 初始化控件文本
                    TextBox[] textBox = { thingName ,numberProject , other , machineNo , badContent , badRate , handle ,
                            operater,grindMachine,grindTime,grind,DCJ_DCX,
                            count1,founder1,slot1,pcs1,count2,founder2,slot2,pcs2,
                            count3,founder3,slot3,pcs3,count4,founder4,slot4,pcs4,
                            count5,founder5,slot5,pcs5,count6,founder6,slot6,pcs6,
                            project,number,judgeReason
                    };
                    RadioButton[] radioButton = { groupCheckPointa, groupCheckPointb, groupCheckPointc ,
                            groupJudgera ,groupJudgerb};
                    ComboBox[] comboBox = { comboBox1, productShift ,
                            card1 ,result1 ,come1 ,card2 ,result2 ,come2 ,
                            card3 ,result3 ,come3 ,card4 ,result4 ,come4 ,
                            card5 ,result5 ,come5 ,card6 ,result6 ,come6 ,
                            result7 ,come7
                    };
                    CalendarDatePicker[] calendarDatePicker = { productDate, date1, date2, date3, date4, date5, date6 };
                    abnormalQualityReportService.getApprovalDetailOne(data["userlevel"],data["folderName"], data["fileName"], textBox, radioButton, comboBox, calendarDatePicker);
                }
                else
                {
                    //  根据用户等级初始化界面
                    this.initial(data["userlevel"]);
                }

            }
            date.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void OnCommitData(object sender, RoutedEventArgs e)
        {
            // 如果时等级高的用户，该按钮的功能为下一页
            if (data["userlevel"] == "3" || data["userlevel"] == "4" || data["userlevel"] == "5")
            {
                this.Frame.Navigate(typeof(AbnormalQualityReportTwo), data);
                return;
            }

            // 初始化控件文本
            TextBox[] textBox = { thingName ,numberProject , other , machineNo , badContent , badRate , handle ,
                    operater,grindMachine,grindTime,grind,DCJ_DCX,
                    count1,founder1,slot1,pcs1,count2,founder2,slot2,pcs2,
                    count3,founder3,slot3,pcs3,count4,founder4,slot4,pcs4,
                    count5,founder5,slot5,pcs5,count6,founder6,slot6,pcs6,
                    project,number,judgeReason
            };
            RadioButton[] radioButton = { groupCheckPointa, groupCheckPointb, groupCheckPointc ,
                    groupJudgera ,groupJudgerb};
            ComboBox[] comboBox = { comboBox1, productShift ,
                    card1 ,result1 ,come1 ,card2 ,result2 ,come2 ,
                    card3 ,result3 ,come3 ,card4 ,result4 ,come4 ,
                    card5 ,result5 ,come5 ,card6 ,result6 ,come6 ,
                    result7 ,come7
            };
            CalendarDatePicker[] calendarDatePicker = { productDate, date1, date2, date3, date4, date5, date6 };

           

            JsonObject checkRecordData = new JsonObject();
            // 初始化检查的json信息
            checkRecordData.Add("checkInfo", commonOperation.initCheckJsonArray(data["folderName"], "", ""));
            // 设置检查内容的json信息
            JsonArray content = new JsonArray();
            // 初始化文本框
            for (int i = 0; i < textBox.Length; i++)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(textBox[i].Name);
                contentItem["status"] = JsonValue.CreateStringValue(textBox[i].Text);
                contentItem["edit"] = JsonValue.CreateStringValue("0");
                content.Add(contentItem);
            }
            // 初始化单选框
            for (int i = 0; i < radioButton.Length; i++)
            {
                if (radioButton[i].IsChecked == true)
                {
                    JsonObject contentItem = new JsonObject();
                    contentItem["name"] = JsonValue.CreateStringValue(radioButton[i].GroupName);
                    contentItem["status"] = JsonValue.CreateStringValue(radioButton[i].Name);
                    contentItem["edit"] = JsonValue.CreateStringValue("0");
                    content.Add(contentItem);
                }
            }
            // 初始化下拉框
            for (int i = 0; i < comboBox.Length; i++)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(comboBox[i].Name);
                contentItem["status"] = JsonValue.CreateStringValue(comboBox[i].SelectedItem.ToString());
                contentItem["edit"] = JsonValue.CreateStringValue("0");
                content.Add(contentItem);
            }
            // 初始化日历框
            for (int i = 0; i < calendarDatePicker.Length; i++)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(calendarDatePicker[i].Name);
                contentItem["status"] = JsonValue.CreateStringValue(calendarDatePicker[i].Date.ToString().Split(' ')[0]);
                contentItem["edit"] = JsonValue.CreateStringValue("0");
                content.Add(contentItem);
            }
            checkRecordData.Add("content", content);
            checkRecordData.Add("checkerInfo", commonOperation.initCheckerJsonArray(data["username"], date.Text, ""));
            string time = comboBox1.SelectedIndex == 0 ? "morning" : "noon";
            string fileName = data["folderName"] + "_" + thingName.Text + "_" + time + "_" + date.Text + ".ykk";
            // 将json数据写入对应文件中
            commonOperation.writeJsonToFile(checkRecordData, fileName, KnownFolders.PicturesLibrary, data["folderName"]);
            // 设置提示框
            messDialog.showDialog("提交成功！");

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.initial(data["userlevel"]);
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            if (data["userlevel"] == "1")
            {
                this.Frame.Navigate(typeof(AuthorityNavigation), data);
            }
            else
            {
                this.Frame.Navigate(typeof(AbnormalQualityReportList), data);
            }
        }

        private void initial(string userlevel)
        {
            // 初始化控件文本
            TextBox[] textBox = { numberProject , other , thingName , machineNo , badContent , badRate , handle ,
                    operater,grindMachine,grindTime,grind,DCJ_DCX,
                    count1,founder1,slot1,pcs1,count2,founder2,slot2,pcs2,
                    count3,founder3,slot3,pcs3,count4,founder4,slot4,pcs4,
                    count5,founder5,slot5,pcs5,count6,founder6,slot6,pcs6,
                    project,number,judgeReason
            };
            RadioButton[] radioButton = {  groupCheckPointb, groupCheckPointc ,groupJudgerb};
            ComboBox[] comboBox = {  productShift ,
                    card1 ,result1 ,come1 ,card2 ,result2 ,come2 ,
                    card3 ,result3 ,come3 ,card4 ,result4 ,come4 ,
                    card5 ,result5 ,come5 ,card6 ,result6 ,come6 ,
                    result7 ,come7
            };
            CalendarDatePicker[] calendarDatePicker = { productDate, date1, date2, date3, date4, date5, date6 };

            for(int i=0;i< textBox.Length; i++)
            {
                textBox[i].Text = ""; 
                textBox[i].IsReadOnly = true;
                if ((userlevel == "2") || ( (userlevel == "1") && ( i < 12) ))
                {
                    textBox[i].IsReadOnly = false;
                }
            }
            groupCheckPointa.IsChecked = true;
            groupJudgera.IsChecked = true;
            for (int i = 0; i < radioButton.Length; i++)
            {
                radioButton[i].IsChecked = false;
                radioButton[i].IsEnabled = false;
                if ((userlevel == "2") || (userlevel == "1"))
                {
                    radioButton[i].IsEnabled = true;
                }
            }
            for (int i = 0; i < comboBox.Length; i++)
            {
                comboBox[i].SelectedIndex = 0;
                comboBox[i].IsEnabled = false;
                if ((userlevel == "2") || ((userlevel == "1") && (i < 2)))
                {
                    comboBox[i].IsEnabled = true;
                }
            }
            for (int i = 0; i < calendarDatePicker.Length; i++)
            {
                calendarDatePicker[i].IsEnabled = false;
                if ((userlevel == "2") || ((userlevel == "1") && (i < 1)))
                {
                    calendarDatePicker[i].IsEnabled = true;
                }
            }
        }

        
    }
}
