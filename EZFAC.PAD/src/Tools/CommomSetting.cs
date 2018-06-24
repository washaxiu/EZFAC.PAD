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
        public int[] gruopA = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 12, 13, 14, 15, 16,17,18,19};
        public int[] gruopB = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 12, 13, 14, 15, 16,17,18,19};
        public int[] gruopC = { 1, 2, 3, 4, 5, 6};
        public int[] gruopD = { 1, 2, 3, 4, 5, 6};

        /*
        * 将数字转换成对应的字符串
        * @param 数字
        */
        public string numTostring(int num)
        {
            if (num > 10) return Convert.ToString(num);
            return "0" + Convert.ToString(num);
        }
        
        /*
        * 根据组名确定显示的机番以及其名称
        * @param 组名 机番数组
        */
        public JsonObject getGroupAndLine()
        {
            JsonObject group = new JsonObject();
            string groupName = "压轴线A,压轴线B,压轴线C,压轴线D";
            string A = null, B = null, C = null, D = null;
            // 初始化机番A
            for(int i=0;i< gruopA.Length; i++)
            {
                if (i != 0) A = A + ",";
                A = A + "A-" + numTostring(gruopA[i]);
            }
            // 初始化机番B
            for (int i = 0; i < gruopB.Length; i++)
            {
                if (i != 0) B = B + ",";
                B = B + "B-" + numTostring(gruopB[i]);
            }
            // 初始化机番C
            for (int i = 0; i < gruopC.Length; i++)
            {
                if (i != 0) C = C + ",";
                C = C + "C-" + numTostring(gruopC[i]);
            }
            // 初始化机番D
            for (int i = 0; i < gruopD.Length; i++)
            {
                if (i != 0) D = D + ",";
                D = D + "D-" + numTostring(gruopD[i]);
            }

            group["group"] = JsonValue.CreateStringValue(groupName);
            group["A"] = JsonValue.CreateStringValue(A);
            group["B"] = JsonValue.CreateStringValue(B);
            group["C"] = JsonValue.CreateStringValue(C);
            group["D"] = JsonValue.CreateStringValue(D);
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
