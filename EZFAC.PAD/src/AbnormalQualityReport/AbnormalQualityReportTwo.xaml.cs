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
using EZFAC.PAD.src.Service;
using EZFAC.PAD.src.Tools;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EZFAC.PAD
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AbnormalQualityReportTwo : Page
    {
        private Dictionary<string, string> data = new Dictionary<string, string>();
        private AbnormalQualityReportService abnormalQualityReportService = new AbnormalQualityReportService();
        private CommonOperation commonOperation = new CommonOperation();
        private MessDialog messDialog = new MessDialog();
        private JsonValue good = JsonValue.CreateStringValue("good");
        private JsonValue bad = JsonValue.CreateStringValue("bad");

        public AbnormalQualityReportTwo()
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
                TextBox[] textLevel3 = { badNum , other , judger , personInCharge , beikao};
                RadioButton[] radioLevel3 = { groupHandleMethoda , groupHandleMethodb };
                TextBox[] textLevel4 = { happenReason , shift1 , repairPer1 , repairContent1 , shift2 , repairPer2 , repairContent2 , reply };
                CalendarDatePicker[] calendarLevel4 = { repaieDate1 , repaieDate2 };
                abnormalQualityReportService.getApprovalDetailTwo(data["userlevel"], data["folderName"], data["fileName"], textLevel3, radioLevel3, textLevel4, calendarLevel4);
            }
            date.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AbnormalQualityReportList), data);
        }

        private void prePage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AbnormalQualityReportOne), data);
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if(data["userlevel"] == "3")
            {
                TextBox[] textLevel3 = { badNum, other, judger, personInCharge, beikao };
                RadioButton[] radioLevel3 = { groupHandleMethoda, groupHandleMethodb };
                abnormalQualityReportService.level3Approval(data, date, textLevel3, radioLevel3);
            }
            else if (data["userlevel"] == "4")
            {
                TextBox[] textLevel4 = { happenReason, shift1, repairPer1, repairContent1, shift2, repairPer2, repairContent2, reply };
                CalendarDatePicker[] calendarLevel4 = { repaieDate1, repaieDate2 };
                abnormalQualityReportService.level4Approval(data, date, textLevel4, calendarLevel4);
            }
            else if (data["userlevel"] == "5")
            {
                abnormalQualityReportService.level5Approval(data, date);
            }
            messDialog.showDialog("审批成功！");
        }
    }
}
