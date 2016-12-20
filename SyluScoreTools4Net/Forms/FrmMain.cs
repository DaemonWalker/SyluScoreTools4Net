using SyluScoreTools4Net.Class;
using SyluScroeTools4Net.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace SyluScoreTools4Net.Forms
{
    /// <summary>
    /// 主窗口
    /// </summary>
    public partial class FrmMain : Form
    {
        #region private var
        /// <summary>
        /// 账户窗口
        /// </summary>
        private FrmAcc FrmAcc = new FrmAcc();

        /// <summary>
        /// 存档路径
        /// </summary>
        private string DirPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\ScroeTools\";

        /// <summary>
        /// 缓存路径
        /// </summary>
        private string FilePath;

        /// <summary>
        /// 序列化
        /// </summary>
        private JavaScriptSerializer Json = new JavaScriptSerializer();

        /// <summary>
        /// 账号信息
        /// </summary>
        private Dictionary<string, UserInfo> UserDict;

        /// <summary>
        /// 绩点模拟器窗口
        /// </summary>
        private FrmVipSim FrmSim = new FrmVipSim();
        #endregion
        #region public var
        /// <summary>
        /// 当前用户
        /// </summary>
        internal UserInfo NowUser
        {
            get
            {
                if (!UserDict.ContainsKey("Now"))
                {
                    UserDict.Add("Now", new UserInfo());
                }
                return UserDict["Now"];
            }
            set
            {
                if (!UserDict.ContainsKey(value.UserName))
                {
                    UserDict.Add(value.UserName, value);
                }
                UserDict["Now"] = value;
            }
        }

        /// <summary>
        /// 当前用户名
        /// </summary>
        public string UserName
        {
            get
            {
                var dictName = NowUser.UserName;
                if (string.IsNullOrWhiteSpace(dictName))
                {
                    var frmName = FrmAcc.UserName;
                    if (string.IsNullOrWhiteSpace(frmName))
                    {
                        throw new BusinessException("你还未设置账号密码!");
                    }
                    else
                    {
                        NowUser.UserName = frmName;
                        return frmName;
                    }
                }
                else
                {
                    return dictName;
                }
            }
            set
            {
                NowUser.UserName = value;
            }
        }

        /// <summary>
        /// 当前密码
        /// </summary>
        public string Password
        {
            get
            {
                var dictPwd = NowUser.Password;
                if (string.IsNullOrWhiteSpace(dictPwd))
                {
                    var frmPwd = FrmAcc.Password;
                    if (string.IsNullOrWhiteSpace(frmPwd))
                    {
                        return null;
                    }
                    else
                    {
                        NowUser.Password = frmPwd;
                        return frmPwd;
                    }
                }
                else
                {
                    return dictPwd;
                }
            }
            set
            {
                NowUser.Password = value;
            }
        }
        #endregion
        #region public foo
        /// <summary>
        /// 创建缓存文件夹
        /// </summary>
        public FrmMain()
        {
            InitializeComponent();
            if (!Directory.Exists(DirPath))
            {
                Directory.CreateDirectory(DirPath);
            }
            FilePath = DirPath + "data.json";
        }
        #endregion
        #region events
        private void FrmMain_Load(object sender, EventArgs e)
        {
            dgvData.AutoGenerateColumns = false;
            labVIPScore.Alignment = ToolStripItemAlignment.Right;
            if (File.Exists(FilePath))
            {
                try
                {
                    UserDict = Json.Deserialize<Dictionary<string, UserInfo>>(File.ReadAllText(FilePath));
                    AddTSMIBtn(UserDict.Keys);
                    dgvData.DataSource = NowUser.ScoreList;
                    ShowVIPClass();
                }
                catch
                {
                    UserDict = new Dictionary<string, UserInfo>();
                }
            }
            else
            {
                UserDict = new Dictionary<string, UserInfo>();
            }
        }

        async private void btnAddAcc_Click(object sender, EventArgs e)
        {
            FrmAcc.DialogResult = DialogResult.No;
            if (FrmAcc.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            UserName = FrmAcc.UserName;
            Password = FrmAcc.Password;
            if (UserDict.ContainsKey(FrmAcc.UserName))
            {
                MessageBox.Show("你已经添加了该帐号!");
                NowUser = UserDict[UserName];
                return;
            }
            AddTSMIBtn(new List<string>() { FrmAcc.UserName });
            NowUser = new UserInfo() { UserName = this.UserName, Password = this.Password };
            MessageBox.Show("添加成功!");
            ChangeStatusLabel("正在计算中...");
            await NowUser.GetVipAvg();
            ShowVIPClass();
            SaveData();
            ChangeStatusLabel("成绩获取成功!");
        }

        /// <summary>
        /// 刷新成绩点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void btnRefreshSocre_Click(object sender, EventArgs e)
        {
            ChangeStatusLabel("正在获取成绩中...");
            ChangeDgvDataSource(null);
            await NowUser.RefreshScore();
            ChangeDgvDataSource(NowUser.ScoreList);
            ChangeVIPAvgLabel(NowUser.VIPAvg);
            ShowVIPClass();
            SaveData();
            ChangeStatusLabel("成绩刷新成功!");
        }

        /// <summary>
        /// 添加账号点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAcc_Click(object sender, EventArgs e)
        {
            var btn = sender as ToolStripMenuItem;
            NowUser = UserDict[btn.Text];
            ChangeDgvDataSource(NowUser);
            ShowVIPClass();
            SaveData();
        }

        /// <summary>
        /// 绩点模拟器点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSim_Click(object sender, EventArgs e)
        {
            FrmSim.NowUser = this.NowUser;
            FrmSim.ShowDialog();
        }
        #endregion
        #region private foo
        /// <summary>
        /// 更改dgv数据源
        /// </summary>
        /// <param name="data"></param>
        private void ChangeDgvDataSource(object data)
        {
            if (this.dgvData.InvokeRequired)
            {
                this.Invoke(new ChangeControlData(ChangeDgvDataSource), data);
            }
            else
            {
                this.dgvData.DataSource = data;
            }
        }

        /// <summary>
        /// 更改状态label
        /// </summary>
        /// <param name="data"></param>
        private void ChangeStatusLabel(object data)
        {
            if (this.statusStrip.InvokeRequired)
            {
                this.Invoke(new ChangeControlData(ChangeStatusLabel), data);
            }
            else
            {
                tslFrmStatus.Text = data.ToString();
            }
        }

        /// <summary>
        /// 更改学位课绩点label
        /// </summary>
        /// <param name="data"></param>
        private void ChangeVIPAvgLabel(object data)
        {
            if (this.statusStrip.InvokeRequired)
            {
                this.Invoke(new ChangeControlData(ChangeVIPAvgLabel), data);
            }
            else
            {
                labVIPScore.Text = "当前用户学位课绩点:" + data.ToString();
            }
        }

        /// <summary>
        /// 更改dgv行颜色
        /// </summary>
        /// <param name="data"></param>
        private void ChangeRowColor(object data)
        {
            if (this.dgvData.InvokeRequired)
            {
                this.Invoke(new ChangeControlData(ChangeRowColor), data);
            }
            else
            {
                var i = (int)data;
                this.dgvData.Rows[i].DefaultCellStyle.BackColor = Color.Red;
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        private void SaveData()
        {
            UserDict[NowUser.UserName] = NowUser;
            using (var file = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(file))
                {
                    sw.WriteLine(Json.Serialize(UserDict));
                    sw.Flush();
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// 显示学位课
        /// </summary>
        private void ShowVIPClass()
        {
            foreach (var info in NowUser.VIPClassList)
            {
                for (int i = 0; i < NowUser.ScoreList.Count; i++)
                {
                    var score = NowUser.ScoreList[i];
                    if (score.ClassID == info.ClassID)
                    {
                        ChangeRowColor(i);
                    }
                }
            }
            ChangeVIPAvgLabel(NowUser.VIPAvg);
        }

        /// <summary>
        /// 添加账号按钮
        /// </summary>
        /// <param name="texts"></param>
        private void AddTSMIBtn(IEnumerable<string> texts)
        {
            var list = new List<ToolStripMenuItem>();
            foreach (var text in texts)
            {
                if (text == "Now")
                {
                    continue;
                }
                var tsmi = new ToolStripMenuItem();
                tsmi.Text = text;
                tsmi.Click += btnAcc_Click;
                if (text == NowUser.UserName)
                {
                    tsmi.Checked = true;
                }
                list.Add(tsmi);
            }
            btnSelectNowAcc.DropDownItems.AddRange(list.ToArray());
        }
        #endregion
    }
}
