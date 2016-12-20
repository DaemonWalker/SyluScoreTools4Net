using SyluScroeTools4Net.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyluScoreTools4Net.Class
{
    [Serializable]
    public class UserInfo
    {
        public double VIPAvg { get; private set; } = 0;


        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 成绩列表
        /// </summary>
        public List<ClassInfo> ScoreList { get; set; }

        /// <summary>
        /// 学位课列表
        /// </summary>
        public List<ClassInfo> VIPClassList { get; set; }

        /// <summary>
        /// 获取学位课绩点
        /// </summary>
        async public Task GetVipAvg()
        {
            if (VIPAvg != 0)
            {
                return;
            }
            await Task.Run(() =>
            {
                var tasks = new List<Task>();
                if (ScoreList == null || ScoreList.Count == 0)
                {
                    tasks.Add(new Task(() =>
                    {
                        var webPost = new WebPost();
                        webPost.UserName = UserName;
                        webPost.PassWord = Password;
                        this.ScoreList = webPost.GetAllScore();
                    }));
                }
                if (VIPClassList == null || VIPClassList.Count == 0)
                {
                    tasks.Add(new Task(() =>
                    {
                        var webPost = new WebPost();
                        webPost.UserName = UserName;
                        webPost.PassWord = Password;
                        this.VIPClassList = webPost.GetAllVIP();
                    }));
                }
                Task.WaitAll(tasks.ToArray());
            });
            CountVipAvg();
        }
        async public Task RefreshScore()
        {
            await Task.Run(() =>
            {
                var webPost = new WebPost();
                webPost.UserName = UserName;
                webPost.PassWord = Password;
                this.ScoreList = webPost.GetAllScore();
            });
            CountVipAvg();
        }

        private void CountVipAvg()
        {
            for (int i = 0; i < this.ScoreList.Count; i++)
            {
                var classInfo = this.ScoreList[i];
                foreach (var vipInfo in this.VIPClassList)
                {
                    if (classInfo.ClassID == vipInfo.ClassID)
                    {
                        vipInfo.Score = Math.Max(vipInfo.Score, classInfo.Score);
                        vipInfo.NewScore = vipInfo.Score.ToString();
                        break;
                    }
                }
            }
            double allScore = 0;
            double allWeight = 0;
            foreach (var vipInfo in this.VIPClassList)
            {
                allScore += vipInfo.Score * vipInfo.ClassWeight;
                allWeight += vipInfo.ClassWeight;
            }
            VIPAvg = allScore / allWeight;
        }
    }
}
