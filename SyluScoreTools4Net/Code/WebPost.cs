using SyluScoreTools4Net.Class;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SyluScroeTools4Net.Code
{
    class WebPost
    {
        #region private vars
        /// <summary>
        /// 获取到的url
        /// </summary>
        private string URL = "";

        /// <summary>
        /// 默认编码
        /// </summary>
        private static Encoding encoding = Encoding.GetEncoding("GB2312");

        /// <summary>
        /// 成绩信息
        /// </summary>
        private List<ClassInfo> ScoreInfo;

        /// <summary>
        /// Cookie容器
        /// </summary>
        private CookieContainer cc;

        /// <summary>
        /// 用于POST
        /// </summary>
        private string __VIEWSTATE = "";

        /// <summary>
        /// 功能模块代码 用于POST
        /// </summary>
        private string gnmkdm = "";
        #endregion
        #region public vars
        /// <summary>
        /// 学位课列表
        /// </summary>
        public List<ClassInfo> VIPClassList { get; set; } = new List<ClassInfo>();

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }
        #endregion
        #region public foos
        /// <summary>
        /// 获取全部信息
        /// </summary>
        /// <returns></returns>
        public List<ClassInfo> GetAllScore()
        {
            GetURL();
            EnterSchoolSite(UserName, PassWord);
            EnterScorePage(UserName);
            PostScoreData(new[] { UserName, "", "", "btn_zcj", "历年成绩" });
            return ScoreInfo;
        }

        /// <summary>
        /// 获取全部学位课课表
        /// </summary>
        /// <returns></returns>
        public List<ClassInfo> GetAllVIP()
        {
            GetURL();
            EnterSchoolSite(UserName, PassWord);
            GetVIPClass(UserName);
            return VIPClassList;
        }

        /// <summary>
        /// 检查账号密码
        /// </summary>
        public void CheckAccPsw()
        {
            GetURL();
            EnterSchoolSite(UserName, PassWord);
        }
        #endregion
        #region private foos
        private string Trans(Hashtable ht)
        {
            string ReturnHtml = "";
            Encoding GB2313 = Encoding.UTF8;
            foreach (string s in ht.Keys)
            {
                ReturnHtml = ReturnHtml + HttpUtility.UrlEncode(s, encoding) + "=" + HttpUtility.UrlEncode((string)ht[s], encoding) + "&";
            }
            ReturnHtml = ReturnHtml.Substring(0, ReturnHtml.Length - 1);
            return ReturnHtml;
        }


        private void GetURL()
        {
            HttpWebRequest h1 = (HttpWebRequest)WebRequest.Create(new Uri("http://218.25.35.27:8080"));
            h1.Method = "Get";
            HttpWebResponse h2 = (HttpWebResponse)h1.GetResponse();
            Stream back = h2.GetResponseStream();
            URL = h2.ResponseUri.ToString();
            URL = URL.Substring(0, URL.IndexOf("default2.aspx"));
            var sr = new StreamReader(h2.GetResponseStream(), encoding);
            var ret = sr.ReadToEnd();
            var temp1 = ret.IndexOf("\"__VIEWSTATE\" value=\"");
            ret = ret.Remove(0, temp1 + 21);
            var temp2 = ret.IndexOf("\"");
            __VIEWSTATE = ret.Substring(0, temp2);
            temp1 = ret.IndexOf("用户名");
            ret = ret.Remove(0, temp1);
            ret = ret.Remove(0, ret.IndexOf("name=\"") + 6);

        }

        /// <summary>
        /// 进入主站
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PassWord"></param>
        /// <returns></returns>
        private void EnterSchoolSite(string UserName, string PassWord)
        {
            HttpWebRequest LoginReq = null;
            HttpWebResponse LoginRes;
            ScoreInfo = null;
            cc = new CookieContainer();
            string ReturnHtml = string.Empty;
            Hashtable ht = new Hashtable();
            ht.Add("RadioButtonList1", "学生");
            ht.Add("TextBox1", UserName);
            ht.Add("TextBox2", PassWord);
            ht.Add("__VIEWSTATE", __VIEWSTATE);
            ht.Add("Button1", "");
            ht.Add("lbLanguage", "");
            string s = Trans(ht);
            byte[] byteArray = encoding.GetBytes(s);
            LoginReq = (HttpWebRequest)WebRequest.Create(new Uri(URL + "default2.aspx"));
            LoginReq.CookieContainer = cc;
            LoginReq.Method = "POST";
            LoginReq.ContentType = "application/x-www-form-urlencoded";
            LoginReq.Referer = URL + "default2.aspx";
            LoginReq.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";
            LoginReq.Timeout = 10000;
            LoginReq.ContentLength = byteArray.Length;
            Stream newStream = LoginReq.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);//写入参数
            newStream.Close();
            LoginRes = (HttpWebResponse)LoginReq.GetResponse();
            StreamReader sr = new StreamReader(LoginRes.GetResponseStream(), Encoding.Default);
            ReturnHtml = sr.ReadToEnd();
            sr.Close();
            LoginRes.Close();
            newStream.Close();
            if (ReturnHtml.Contains("出错啦"))
            {
                ThrowServerEx();
            }
            if (ReturnHtml.Contains("用户名"))
            {
                ThrowAccEx();
            }
        }
        private void EnterScorePage(string UserName)
        {
            int begin, end;
            HttpWebRequest EnterScorePageReq = (HttpWebRequest)WebRequest.Create(URL + "xscjcx.aspx?xh=" + UserName + "&gnmkdm=121605");
            EnterScorePageReq.Referer = URL + "xs_main.aspx?xh=" + UserName;
            EnterScorePageReq.CookieContainer = cc;
            EnterScorePageReq.Method = "GET";
            HttpWebResponse EnterScorePageRes = (HttpWebResponse)EnterScorePageReq.GetResponse();
            StreamReader sr = new StreamReader(EnterScorePageRes.GetResponseStream(), Encoding.Default);
            string ReturnHtml = sr.ReadToEnd();
            sr.Close();

            if (ReturnHtml.Contains("请登录"))
            {
                ThrowServerEx();
            }

            begin = ReturnHtml.IndexOf("\"__VIEWSTATE\" value=\"");
            ReturnHtml = ReturnHtml.Remove(0, begin + 21);
            end = ReturnHtml.IndexOf("\"");
            __VIEWSTATE = ReturnHtml.Substring(0, end);

        }
        private void PostScoreData(string[] CalcOption)
        {
            string s, ReturnHtml;
            byte[] byteArray;
            StreamReader sr;
            HttpWebRequest ScoreDataReq = (HttpWebRequest)WebRequest.Create(URL + "xscjcx.aspx?xh=" + CalcOption[0] + "&gnmkdm=" + gnmkdm);
            ScoreDataReq.CookieContainer = cc;
            ScoreDataReq.Referer = URL + "xscjcx.aspx?xh=" + CalcOption[0] + "&gnmkdm=" + gnmkdm;
            Hashtable h2 = new Hashtable();
            h2.Add("__VIEWSTATE", __VIEWSTATE);
            h2.Add("ddlXN", CalcOption[1]);
            h2.Add("ddlXQ", CalcOption[2]);
            h2.Add(CalcOption[3], CalcOption[4]);
            h2.Add("ddl_kcxz", "");
            ScoreDataReq.Method = "post";
            ScoreDataReq.ContentType = "application/x-www-form-urlencoded";
            s = Trans(h2);
            byteArray = encoding.GetBytes(s);
            ScoreDataReq.ContentLength = byteArray.Length;
            Stream s1 = ScoreDataReq.GetRequestStream();
            s1.Write(byteArray, 0, byteArray.Length);
            s1.Close();
            HttpWebResponse ScoreDataRes = (HttpWebResponse)ScoreDataReq.GetResponse();
            sr = new StreamReader(ScoreDataRes.GetResponseStream(), Encoding.Default);
            ReturnHtml = sr.ReadToEnd();
            sr.Close();
            ScoreDataRes.Close();
            s1.Close();
            if (ReturnHtml.Contains("请登录"))
            {
                ThrowServerEx();
            }
            GetData(CalcOption[1], ReturnHtml);
        }

        private void GetData(string Year, string ReturnHtml)
        {
            var list = new List<ClassInfo>();
            int begin = ReturnHtml.IndexOf("重修标记");
            ReturnHtml = ReturnHtml.Remove(0, begin + 14);
            ReturnHtml = ReturnHtml.Replace("&nbsp;", "");
            while (ReturnHtml.IndexOf("</table>") >= 20)
            {
                begin = ReturnHtml.IndexOf("<td>" + Year);
                string ss = "	</tr>";
                int end = ReturnHtml.IndexOf(ss);
                string temp = (ReturnHtml.Substring(begin, end - begin)).Replace("<td>", "");
                ReturnHtml = ReturnHtml.Remove(0, end + ss.Length);
                string[] data = Regex.Split(temp, "</td>");
                list.Add(new ClassInfo()
                {
                    Year = data[0],
                    Term = data[1],
                    ClassID = data[2],
                    ClassName = data[3],
                    ClassType = data[4],
                    ClassWeight = double.Parse(data[6]),
                    Score = double.Parse(data[7]),
                    IsMinor = string.IsNullOrWhiteSpace(data[13]) ? false : (data[13] == "0" ? false : true),
                    IsRestudy = !string.IsNullOrWhiteSpace(data[19])
                });
            }
            ScoreInfo = list;
        }

        private void GetVIPClass(string UserName)
        {
            StreamReader sr;
            int begin, end;
            string ReturnHtml;
            HttpWebRequest GetAllPlanPagesReq = (HttpWebRequest)WebRequest.Create(URL + "pyjh.aspx?xh=" + UserName + "&gnmkdm=N121607");
            GetAllPlanPagesReq.Referer = URL + "xs_main.aspx?xh=" + UserName;
            GetAllPlanPagesReq.CookieContainer = cc;
            GetAllPlanPagesReq.Method = "GET";
            HttpWebResponse response2 = (HttpWebResponse)GetAllPlanPagesReq.GetResponse();
            sr = new StreamReader(response2.GetResponseStream(), Encoding.Default);
            ReturnHtml = sr.ReadToEnd();
            begin = ReturnHtml.IndexOf("\"__VIEWSTATE\" value=\"");
            ReturnHtml = ReturnHtml.Remove(0, begin + 21);
            end = ReturnHtml.IndexOf("\"");
            __VIEWSTATE = ReturnHtml.Substring(0, end);
            for (int i = 1; i < 10; i++)
            {
                Hashtable ht = new Hashtable();

                ht.Add("__EVENTTARGET", "xq");
                ht.Add("__EVENTARGUMENT", "");
                ht.Add("__VIEWSTATE", __VIEWSTATE);
                ht.Add("xq", i.ToString());
                ht.Add("kcxz", "全部");

                byte[] PostByteArray = encoding.GetBytes(Trans(ht));
                HttpWebRequest SingPlanPageReq = (HttpWebRequest)WebRequest.Create(URL + "pyjh.aspx?xh=" + UserName + "&gnmkdm=N121607");
                SingPlanPageReq.Method = "POST";
                SingPlanPageReq.ContentType = "application/x-www-form-urlencoded";
                SingPlanPageReq.Referer = URL + "pyjh.aspx?xh=" + UserName + "&gnmkdm=N121607";
                SingPlanPageReq.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";
                SingPlanPageReq.Timeout = 10000;
                SingPlanPageReq.ContentLength = PostByteArray.Length;
                SingPlanPageReq.CookieContainer = cc;
                Stream PostStream = SingPlanPageReq.GetRequestStream();
                PostStream.Write(PostByteArray, 0, PostByteArray.Length);//写入参数
                PostStream.Close();
                HttpWebResponse SingPlanPageRes = (HttpWebResponse)SingPlanPageReq.GetResponse();
                sr = new StreamReader(SingPlanPageRes.GetResponseStream(), Encoding.Default);
                ReturnHtml = sr.ReadToEnd();
                begin = ReturnHtml.IndexOf("是否学位课</td>");
                end = ReturnHtml.IndexOf("<td colspan=\"28\"><b><span>1");
                string tempStr = ReturnHtml.Substring(begin, end - begin);
                string[] ClassContext = Regex.Split(tempStr, "</tr>");
                List<string> list = new List<string>();
                foreach (string ss in ClassContext)
                {
                    if (ss.Contains(">是<"))
                        list.Add(ss);
                }
                foreach (string ss in list)
                {
                    var TempSA = Regex.Split(ss, "</td>");
                    TempSA[0] = TempSA[0].Replace("</font>", "");
                    TempSA[1] = TempSA[1].Replace("</font>", "");
                    TempSA[2] = TempSA[2].Replace("</font>", "");
                    VIPClassList.Add(new ClassInfo()
                    {
                        ClassID = TempSA[0].Substring(TempSA[0].LastIndexOf(">") + 1),
                        ClassName = TempSA[1].Substring(TempSA[1].LastIndexOf(">") + 1),
                        ClassWeight = double.Parse(TempSA[2].Substring(TempSA[2].LastIndexOf(">") + 1))
                    });
                }
            }
        }
        private void ThrowServerEx()
        {
            throw new ServerErrorException("教学网挂了,请重试");
        }
        private void ThrowAccEx()
        {
            throw new WrongAccException("用户名密码错误");
        }
        #endregion
    }

}