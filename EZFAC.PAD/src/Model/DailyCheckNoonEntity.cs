using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZFAC.PAD.src.Model
{
    class DailyCheckNoonEntity
    {
        //  点检人名
        public string checker { get; set; }



        public string group { get; set; }
        public string line { get; set; }

        public string first { get; set; }
        public string two { get; set; }
        public string three { get; set; }
        public string four { get; set; }
        public string five { get; set; }
        public string six { get; set; }
        public string seven { get; set; }
        public string eight { get; set; }
        public string nine { get; set; }
        public string ten { get; set; }
        public string eleven { get; set; }
        public string twelve { get; set; }
        public string fourteen { get; set; }
        public string fifteen { get; set; }
        public string sixteen { get; set; }
        public string seventeen { get; set; }

        //  用户姓名
        public string name { get; set; }
        // 用户等级
        public string level { get; set; }
        // 用户是否检查或审批过
        public string check { get; set; }
        // 用户是否对内容进行过修改
        public string edit { get; set; }
        // 用户检查或审批时间
        public string date { get; set; }
        // 用户备注信息
        public string comments { get; set; }
    }
}
