using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyluScoreTools4Net.Class
{
    /// <summary>
    /// 用于记录课程信息
    /// </summary>
    class ClassInfo
    {
        /// <summary>
        /// 课程编码
        /// </summary>
        public string ClassID { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 学期
        /// </summary>
        public string Term { get; set; }

        /// <summary>
        /// 课程名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 课程性质
        /// </summary>
        public string ClassType { get; set; }

        /// <summary>
        /// 绩点
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// 学分
        /// </summary>
        public double ClassWeight { get; set; }

        /// <summary>
        /// 辅修标记
        /// </summary>
        public bool IsMinor { get; set; }

        /// <summary>
        /// 重修标记
        /// </summary>
        public bool IsRestudy { get; set; }

        /// <summary>
        /// 假设的绩点
        /// </summary>
        public string NewScore { get; set; }
    }
}
