using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZFAC.PAD.src.Model
{
    class CheckInfoEntity
    {
        //  检查类型
        public string type { get; set; }
        
        // 点检机番信息
        public string group { get; set; }
        public string number { get; set; }
    }
}
