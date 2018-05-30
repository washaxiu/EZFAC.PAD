using EZFAC.PAD.src.Model;
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

namespace EZFAC.PAD
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SemiFinishedCheckDetail : Page
    {
        private CommonOperation commonOperation = new CommonOperation();
        private SemiFinishedCheckService semiFinishedCheckService = new SemiFinishedCheckService();
        private MessDialog messDialog = new MessDialog();
        private Dictionary<string, string> data = new Dictionary<string, string>();
        private JsonValue good = JsonValue.CreateStringValue("good");
        private JsonValue bad = JsonValue.CreateStringValue("bad");
        private string userLevel = null;

        public SemiFinishedCheckDetail()
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
                userLevel = data["userlevel"];
                username.Text = data["username"];
                //初始化文本框
                TextBox[] textBox = { MachineGroup, machineNo, separateStatus, gneck, item,
                    personInCharge, HS_Num, remark };
                //初始化转换框
                ToggleSwitch[] toggleSwitch = { surface , damage_SB171 , PINDamage ,
                    damage_SB251 , filling , xingpian , b3_b4_b5_b7 , b6 , c8_c9_c10 , coreWash};
                //初始化转换框描述文本
                TextBlock[] textBlock = { surfaceText,damageText,PINDamageText,damage_SB251Text,fillingText,
                    xingpianText,b3_b4_b5_b7Text,b6Text,c8_c9_c10Text,coreWashText};
                // 获取审批信息并回填
                semiFinishedCheckService.getApprovalDetail(data["folderName"], data["fileName"], textBox, toggleSwitch, textBlock);
            }
            date.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SemiFinishedCheckList), data);
        }

        private async void Confirm_Click(object sender, RoutedEventArgs e)
        {
            //初始化文本框
            TextBox[] textBox = { separateStatus, gneck, item, personInCharge, HS_Num, remark };
            //初始化转换框
            ToggleSwitch[] toggleSwitch = { surface , damage_SB171 , PINDamage ,
                    damage_SB251 , filling , xingpian , b3_b4_b5_b7 , b6 , c8_c9_c10 , coreWash};
            List<CheckerInfoEntity> checkerList = new List<CheckerInfoEntity>();
            string oldEdit = null, newEdit = null;
            StorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(data["folderName"], CreationCollisionOption.OpenIfExists);
            if (folder != null)
            {
                StorageFile file = await folder.CreateFileAsync(data["fileName"], CreationCollisionOption.OpenIfExists);
                if (file != null)
                {
                    var jsonText = await FileIO.ReadTextAsync(file);
                    JsonObject jsonObject = JsonObject.Parse(jsonText);
                    // 获取检查信息
                    JsonArray checkInfo = jsonObject["checkInfo"].GetArray();
                    // 获取检查内容
                    JsonArray content = jsonObject["content"].GetArray();
                    // 获取用户信息
                    JsonArray checkerInfo = jsonObject["checkerInfo"].GetArray();
                    // 判断信息是否被更改并集成为字符串
                    newEdit = editContnet(toggleSwitch,content);
                    for (int i = 6; i < content.Count; i++)
                    {
                        oldEdit = oldEdit + content[i].GetObject()["edit"].GetString();
                    }
                    // 获取各级用户信息
                    for (int i = 0; i < checkerInfo.Count; i++)
                    {
                        checkerList.Add(new CheckerInfoEntity(
                                 checkerInfo[i].GetObject()["name"].GetString(),
                                 checkerInfo[i].GetObject()["level"].GetString(),
                                 checkerInfo[i].GetObject()["check"].GetString(),
                                 checkerInfo[i].GetObject()["edit"].GetString(),
                                 checkerInfo[i].GetObject()["date"].GetString(),
                                 checkerInfo[i].GetObject()["comments"].GetString()
                            ));
                    }
                }
            }
           
            JsonObject checkRecordData = new JsonObject();
            // 设置检查信息的json信息
            string groupName = MachineGroup.Text[MachineGroup.Text.Length - 1] + "";
            string lineName = machineNo.Text.Substring(machineNo.Text.Length - 2, 2);
            checkRecordData.Add("checkInfo", commonOperation.initCheckJsonArray("SemiFinishedCheck", groupName, lineName));
            
            // 设置检查内容的json信息
            JsonArray newContent = new JsonArray();
            // 初始化文本内容
            foreach (TextBox item in textBox)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(item.Name);
                contentItem["status"] = JsonValue.CreateStringValue(item.Text);
                contentItem["edit"] = JsonValue.CreateStringValue("0");
                newContent.Add(contentItem);
            }
            // 初始化转换框
            string flag = "0";
            for (int i = 0; i < toggleSwitch.Length; i++)
            {
                JsonObject contentItem = new JsonObject();
                contentItem["name"] = JsonValue.CreateStringValue(toggleSwitch[i].Name);
                contentItem["status"] = toggleSwitch[i].IsOn == true ? good : bad;
                //  判断内容是否被修改,若修改则设为1，否则等于原来的值
                if (newEdit[i] == '1')
                {
                    flag = "1";
                    contentItem["edit"] = JsonValue.CreateStringValue("1");
                }
                else
                {
                    contentItem["edit"] = JsonValue.CreateStringValue(oldEdit[i] + "");
                }
                newContent.Add(contentItem);
            }
            checkRecordData.Add("content", newContent);

            // 设置各级别用户的json信息
            JsonArray newCheckerInfo = new JsonArray();
            for (int i = 0; i < checkerList.Count; i++)
            {
                string level = Convert.ToString(i + 1);
                JsonObject item = new JsonObject();
                item["name"] = level == userLevel ? JsonValue.CreateStringValue(username.Text) : JsonValue.CreateStringValue(checkerList[i].name);
                item["level"] = level == userLevel ? JsonValue.CreateStringValue(level) : JsonValue.CreateStringValue(checkerList[i].level);
                item["check"] = level == userLevel ? JsonValue.CreateStringValue("1") : JsonValue.CreateStringValue(checkerList[i].check);
                item["edit"] = level == userLevel ? JsonValue.CreateStringValue(flag) : JsonValue.CreateStringValue(checkerList[i].edit);
                item["date"] = level == userLevel ? JsonValue.CreateStringValue(date.Text) : JsonValue.CreateStringValue(checkerList[i].date);
                item["comments"] = level == userLevel ? JsonValue.CreateStringValue("") : JsonValue.CreateStringValue(checkerList[i].comments);
                newCheckerInfo.Add(item);
            }
            checkRecordData.Add("checkerInfo", newCheckerInfo);
            // 将json数据写入对应文件中
            commonOperation.writeJsonToFile(checkRecordData, data["fileName"], KnownFolders.PicturesLibrary, data["folderName"]);
            // 设置提示框
            messDialog.showDialog("审批成功！");
        }

        /*
        * 判断点检信息是否被更改并集成为字符串
       */
        private string editContnet(ToggleSwitch[] toggleSwitch,JsonArray content)
        {
            string edit = null;
            //  初始化内容数组
            for (int i = 0; i < toggleSwitch.Length; i++)
            {
                bool flag = content[i+6].GetObject()["status"].GetString().Equals("good");
                string msg = flag == toggleSwitch[i].IsOn ? "0" : "1";
                edit = edit + msg;
            }
            return edit;
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
                FullSizeDesired = false,
            };
            await showImage.ShowAsync();
        }
    }
}
