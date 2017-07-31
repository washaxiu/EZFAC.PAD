using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
