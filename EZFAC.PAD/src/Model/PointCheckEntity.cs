using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZFAC.PAD.src.Model
{
    class PointCheckEntity
    {
        //  点检人名
        public string checker { get; set; }
        //  点检日期
        public string date { get; set; }

        // 点检机番信息
        public string group { get; set; }
        public string line { get; set; }

        // 点检内容 1 铸机温调针运转状况
        public string temp1 { get; set; }
        public string temp2 { get; set; }
        public string temp3 { get; set; }

        // 点检内容 2 铸机主副回路设定压力
        public string loop1 { get; set; }
        public string loop2 { get; set; }
        public string loop3 { get; set; }

        // 点检内容 3 自动研磨选别机
        public string select1 { get; set; }

        // 点检内容 4 制御盘内
        public string plat1 { get; set; }

        // 用户级别信息
        public string level2check { get; set; }
        public string level2edit { get; set; }
        public string level2date { get; set; }
        public string level2approvaler { get; set; }
        public string level3check { get; set; }
        public string level3edit { get; set; }
        public string level3date { get; set; }
        public string level3approvaler { get; set; }
        public string level4check { get; set; }
        public string level4edit { get; set; }
        public string level4date { get; set; }
        public string level4approvaler { get; set; }
        public string level5check { get; set; }
        public string level5edit { get; set; }
        public string level5date { get; set; }
        public string level5approvaler { get; set; }
    }
}
