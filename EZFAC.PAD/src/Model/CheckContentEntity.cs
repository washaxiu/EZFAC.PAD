using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZFAC.PAD.src.Model
{
    class CheckContentEntity
    {
        //  检查内容的名称
        public string name { get; set; }
        // 检查内容的状态
        public string status { get; set; }
        // 内容状态是否被修改
        public string edit { get; set; }
    }
}
