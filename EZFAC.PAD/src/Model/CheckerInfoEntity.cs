using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZFAC.PAD.src.Model
{
    class CheckerInfoEntity
    {
        public CheckerInfoEntity()
        {

        }
        public CheckerInfoEntity(string name, string level, string check, string edit, string date, string comments)
        {
            this.name = name;
            this.level = level;
            this.check = check;
            this.edit = edit;
            this.date = date;
            this.comments = comments;
        }

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
