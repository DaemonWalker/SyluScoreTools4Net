using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyluScoreTools4Net.Class
{
    [Serializable]
    class UserInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<ClassInfo> ScoreList { get; set; }
        public List<ClassInfo> VIPClassList { get; set; }
        public double VIPAvg { get; set; }
    }
}
