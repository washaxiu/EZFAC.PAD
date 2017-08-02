using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml.Controls;

namespace EZFAC.PAD.src.Tools
{
    class CommomSetting
    {
        // 每个分组所含的机番
        public int[] gruopA = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        public int[] gruopB = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        public int[] gruopC = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public int[] gruopD = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        /*
        * 根据组名确定显示的机番以及其名称
        * @param 组名 机番数组
        */
        public JsonObject getGroupAndLine()
        {
            JsonObject group = new JsonObject();
            string groupName = "压轴线A,压轴线B,压轴线C";
            string A = "A-01,A-02,A-03,A-04,A-05,A-06,A-07,A-08,A-09,A-10";
            string B = "B-01,B-02,B-03,B-04,B-05,B-06";
            string C = "C-01,C-02,C-03,C-04,C-05,C-06,C-07,C-08";
            group["group"] = JsonValue.CreateStringValue(groupName);
            group["A"] = JsonValue.CreateStringValue(A.ToString());
            group["B"] = JsonValue.CreateStringValue(B.ToString());
            group["C"] = JsonValue.CreateStringValue(C.ToString());
            return group;
        }




        /*
        * 根据组名确定显示的机番以及其名称
        * @param 组名 机番数组
        */
        public void setLineByGroup(string groupName, RadioButton[] line)
        {
            for(int i = 0; i < line.Length; i++)
            {
                line[i].IsEnabled = false;
                //  若（i+1）为个位数则在前面加个0
                string num = (i + 1) < 10 ? "0" + Convert.ToString(i + 1) : Convert.ToString(i + 1);
                line[i].Content = groupName + "-" + num;
            }
            if ("A".Equals(groupName))
            {
                for(int i = 0; i < gruopA.Length; i++)
                {
                    line[gruopA[i] - 1].IsEnabled = true;
                }
            }
            else if ("B".Equals(groupName))
            {
                for (int i = 0; i < gruopB.Length; i++)
                {
                    line[gruopB[i] - 1].IsEnabled = true;
                }
            }
            else if ("C".Equals(groupName))
            {
                for (int i = 0; i < gruopC.Length; i++)
                {
                    line[gruopC[i] - 1].IsEnabled = true;
                }
            }
            else if ("D".Equals(groupName))
            {
                for (int i = 0; i < gruopD.Length; i++)
                {
                    line[gruopD[i] - 1].IsEnabled = true;
                }
            }
        }
    }
}
