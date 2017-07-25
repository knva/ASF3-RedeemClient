using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedeembyASF3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.button1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String info = HttpGet("http://127.0.0.1:1242/IPC", "command=Version");
            if (info == null)
            {
                MessageBox.Show("请先启动ASF!或者ASF版本不匹配!使用ASF3.0","提示信息");
            }
            else
            {
                if (textBox1.Text.Length == 0)
                {
                    MessageBox.Show(string.Format("没有获取到KEY!"));
                }
                else
                {
                    //String rinfo = HttpGet("http://127.0.0.1:1242/IPC", string.Format("command={0}",ExtractKeysAndReg(textBox1.Text)));

                    Form2 a = new Form2(string.Format("http://127.0.0.1:1242/IPC?command={0}", ExtractKeysAndReg(textBox1.Text)));
                    a.ShowDialog();

                }
            }
        }

        /// <summary>  
        /// GET请求与获取结果  
        /// </summary>  
        public static string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            string retString;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
            }
            catch (Exception)
            {
                return null;
                throw;
            }



            return retString;
        }

        private string ExtractKeysAndReg(string plainText)
        {
            List<string> listStrKeys = ExtractKeysFromString(plainText);
            if (listStrKeys.Count > 0)
            {
                string strKeys;

                string stra;
                strKeys = string.Join(",", listStrKeys.ToArray());
                try
                {

                    MessageBox.Show(string.Format("{0} Key被获取", listStrKeys.Count));
                    stra = (string.Format("redeem {0}", strKeys));
                }
                catch
                {
                    return null;
                }
                return stra;
            }
            else
            {

                return null;
            }

        }

        private List<string> ExtractKeysFromString(string source)
        {
            MatchCollection m = Regex.Matches(source, "([0-9A-Z]{5})(?:\\-[0-9A-Z]{5}){2,3}",
                  RegexOptions.IgnoreCase | RegexOptions.Singleline);
            List<string> result = new List<string>();
            if (m.Count > 0)
            {
                foreach (Match v in m)
                {
                    result.Add(v.Value);
                }
            }
            return result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "./";//初始目录，不赋值也可以
            openFileDialog1.Filter = "ArchiSteamFarm.exe(ArchiSteamFarm.exe) | ArchiSteamFarm.exe";//文件类型
            openFileDialog1.ShowDialog();//弹出选择框
            
            try
            {
                String strOpenFileName = openFileDialog1.FileName;//打开的文件的全限定名
                System.Diagnostics.Process process = new System.Diagnostics.Process();

                process.StartInfo.FileName = strOpenFileName;   //asf
                process.StartInfo.Arguments = "--server";

                process.Start();
                if (process.Handle != null)
                {
                    this.button1.Enabled = true;
                }
            }
            catch (System.InvalidOperationException)
            {
                //MessageBox.Show("没有发现ASF,请放在ASF目录下使用.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string str = Interaction.InputBox("输入服务器ip以及端口,例如 127.0.0.1:1242 ,若服务器没有使用--server 模式,则无法使用该连接进行激活操作.", "请输入服务器地址", "", 100, 100);
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show(string.Format("没有获取到KEY!"));
                return;
            }
            if (!ValidateIPAddress(str.Split(':')[0]))
            {
                MessageBox.Show(string.Format("服务器地址错误!"));
                return;
            }
            string url = string.Format("{0}/IPC?command={1}", str, ExtractKeysAndReg(textBox1.Text));
            Interaction.InputBox("输入服务器ip以及端口,例如 127.0.0.1:1242 ,若服务器没有使用--server 模式,则无法使用该连接进行激活操作.", "ipc连接", url, 100, 100);
        }
        public static bool ValidateIPAddress(string ipAddress)
        {
            Regex validipregex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
            return (ipAddress != "" && validipregex.IsMatch(ipAddress.Trim())) ? true : false;
        }
    }


}
