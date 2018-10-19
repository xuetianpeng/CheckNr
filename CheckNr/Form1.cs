using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Net;

namespace CheckNr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            
            InitializeComponent();
            
        }

        private const uint WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_STYLE = (-16);
        private const int GWL_EXSTYLE = (-20);
        private const int LWA_ALPHA = 0;
        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong(
        IntPtr hwnd,
        int nIndex,
        uint dwNewLong
        );
        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong(
        IntPtr hwnd,
        int nIndex
        );
        [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
        private static extern int SetLayeredWindowAttributes(
        IntPtr hwnd,
        int crKey,
        int bAlpha,
        int dwFlags
        );

        string path;
        string FileDX;
        string iplist;
        string[] ipls;
        bool ed, nr, jt, pc;
        
        /// <summary>
        /// 设置窗体具有鼠标穿透效果
        /// </summary>
        public void SetPenetrate()
        {
            GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED);
        }     
        private void Form1_Load(object sender, EventArgs e)
        {
            ed = nr = jt = pc = true;
            GetPingIP();
            FirstCon();
            SetXY();
            SetPenetrate();
            CED();
            CheckNRFile();
            CheckPC();
            Setminilo();
        }
        void Setminilo() 
        {
            //ed = nr = jt = pc = true;
            //jt = true;
            if (ed && nr && jt && pc)
            {
                Set1XY();
                timer1.Interval = 600000;
            }
            else 
            {
                SetXY();
                timer1.Interval = 30000;
            }
            toolStripStatusLabel1.BackColor = ed ? Color.Green : Color.Red;
            toolStripStatusLabel2.BackColor = nr ? Color.Green : Color.Red;
            toolStripStatusLabel3.BackColor = jt ? Color.Green : Color.Red;
            toolStripStatusLabel4.BackColor = pc ? Color.Green : Color.Red;
        }
        void FirstCon() 
        {
            if (File.Exists("path.txt"))
            {
                path = File.ReadAllText("path.txt", Encoding.UTF8);
            }
            else
            {
                if (MessageBox.Show("未发现备份截图路径的配置文件，请选择路径后保存", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    folderBrowserDialog1.ShowDialog();
                    using (File.CreateText("path.txt"))
                    {

                    }
                    File.WriteAllText("path.txt", folderBrowserDialog1.SelectedPath);
                    path = folderBrowserDialog1.SelectedPath;
                }
                else
                {
                    this.Close();
                }
            }
        }
        void GetPingIP() 
        {
            if (File.Exists("IP.txt"))
            {
                iplist = File.ReadAllText("IP.txt");
                ipls = iplist.Split("\r\n".ToCharArray());
            }
            int i = 0;
            if (ipls.Length > 0)
            {
                foreach (string s in ipls)
                {
                    if (s != "")
                    {
                        if (i % 2 == 0)
                        {
                            this.Height = this.Height + 35;
                            Button btn = new Button();
                            this.Controls.Add(btn);
                            btn.Name = s;
                            btn.Location = new System.Drawing.Point(5, this.Height -53);
                            btn.Size = new System.Drawing.Size(120, 27);
                            btn.BackColor = Color.Green;
                            btn.ForeColor = Color.White;
                            btn.Font = new Font("宋体", 16);
                            btn.Text = s.Split(',')[0];
                            btn.Tag = s.Split(',')[1];
                            //btn.UseVisualStyleBackColor = true;
                            
                        }
                        else
                        {
                            Button btn = new Button();
                            this.Controls.Add(btn);
                            btn.Name = s;
                            btn.Location = new Point(135, this.Height -53);
                            btn.Size = new System.Drawing.Size(120, 27);
                            btn.BackColor = Color.Red;
                            btn.ForeColor = Color.White;
                            btn.Font = new Font("宋体", 16);
                            btn.Text = s.Split(',')[0];
                            btn.Tag = s.Split(',')[1];
                            //btn.UseVisualStyleBackColor = true;
                        }
                        i++;
                    }
                }
            }

        }
        void SetXY() 
        {
            int x = SystemInformation.PrimaryMonitorSize.Width - this.Width;
            int y = 0;//SystemInformation.PrimaryMonitorSize.Height - this.Height;//要让窗体往上走 只需改变 Y的坐标
            this.Location = new Point(x, y);

        }
        void Set1XY() 
        {
            int x = SystemInformation.PrimaryMonitorSize.Width - this.Width;
            int y = 0;//SystemInformation.PrimaryMonitorSize.Height - this.Height;//要让窗体往上走 只需改变 Y的坐标
            this.Location = new Point(x, y - this.Height + 22);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            CED();
            CheckNRFile();
            CheckPC();
            Setminilo();
        }
        void CED() 
        {
            bool nr = System.IO.Directory.Exists(@"\\172.16.16.10\nr");
            if (!nr)
            {
                label4.ForeColor = Color.Red;
                label4.Text = "NR共享文件夹不可用，请检查！！！！";
                ed = false;
            }
            else
            {
                label4.ForeColor = Color.Black;
                label4.Text = "NR共享文件夹正常。";
                ed = true;
            }
        }
        void CheckNRFile() 
        {
            label5.Text = label6.Text = label7.Text = label8.Text = label9.Text = label10.Text = "";
            DateTime x = DateTime.Now;
            if (x.Hour >= 8)
            {
                //8点钟
                bool n8 = ExFile(@"\\172.16.16.10\nr\" + GetWeekInt( DateTime.Now.DayOfWeek).ToString()+@"\", DateTime.Now.ToString("expyyMMdd") + "08", ".dump.gz");
                nr = true;
                if (x.Hour == 8)
                {
                    //超过8点半
                    if (x.Minute > 30)
                    {
                        //备份文件已经存在
                        if (n8)
                        {
                            label7.ForeColor = Color.Black;
                            label7.Text = "8点备份正常。" + FileDX;
                            //截图文件是否存在
                            bool j8 = ExFile(path + @"\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\", DateTime.Now.ToString("yyyyMMdd"), "-1.jpg");
                            //已存在
                            if (j8)
                            {
                                label5.ForeColor = Color.Black;
                                label5.Text = "8点截图已保存。";
                                jt = true;
                            }
                                //不存在
                            else
                            {
                                label5.ForeColor = Color.Red;
                                label5.Text = "8点截图未保存";
                                jt = false;
                            }
                            nr = true;
                        }
                            //备份文件不存在
                        else
                        {
                            label7.ForeColor = Color.Red;
                            label7.Text = "8点备份异常，请检查。";
                            bool j8 = ExFile(path + @"\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\", DateTime.Now.ToString("yyyyMMdd"), "-1.jpg");
                            //已存在
                            if (j8)
                            {
                                label5.ForeColor = Color.Black;
                                label5.Text = "8点截图已保存。";
                                jt = true;
                            }
                            //不存在
                            else
                            {
                                label5.ForeColor = Color.Red;
                                label5.Text = "8点截图未保存";
                                jt = false;
                            }
                            nr = false;
                        }
                    }
                }
                    //9.10.11点
                else
                {
                    //备份文件存在
                    if (n8)
                    {
                        label7.ForeColor = Color.Black;
                        label7.Text = "8点备份正常。" + FileDX;
                        nr = true;
                        bool j8 = ExFile(path + @"\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\", DateTime.Now.ToString("yyyyMMdd"), "-1.jpg");
                        //截图文件存在
                        if (j8)
                        {
                            label5.ForeColor = Color.Black;
                            label5.Text = "8点截图已保存。";
                            jt = true;
                        }
                            //截图文件不存在
                        else
                        {
                            label5.ForeColor = Color.Red;
                            label5.Text = "8点截图未保存";
                            jt = false;
                        }
                    }
                        //备份文件不存在
                    else
                    {
                        label7.ForeColor = Color.Red;
                        label7.Text = "8点备份异常，请检查。";
                        bool j8 = ExFile(path + @"\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\", DateTime.Now.ToString("yyyyMMdd"), "-1.jpg");
                        //截图文件存在
                        if (j8)
                        {
                            label5.ForeColor = Color.Black;
                            label5.Text = "8点截图已保存。";
                            jt = true;
                        }
                        //截图文件不存在
                        else
                        {
                            label5.ForeColor = Color.Red;
                            label5.Text = "8点截图未保存";
                            jt = false;
                        }
                        nr = false;
                    }
                }

            }
            if (x.Hour >= 12)
            {
                label9.Text = label10.Text = "";
                bool n12 = ExFile(@"\\172.16.16.10\nr\" + GetWeekInt(DateTime.Now.DayOfWeek).ToString() + @"\", DateTime.Now.ToString("expyyMMdd") + "12", ".dump.gz");
                if (x.Hour == 12)
                {
                    if (x.Minute > 30)
                    {
                        
                        if (n12)
                        {
                            label8.ForeColor = Color.Black;
                            label8.Text = "12点备份正常。" + FileDX;
                            nr = true;
                        }
                        else
                        {
                            label8.ForeColor = Color.Red;
                            label8.Text = "12点备份异常，请检查。";
                            nr = false;
                        }
                    }
                }
                else
                {
                    if (n12)
                    {
                        label8.ForeColor = Color.Black;
                        label8.Text = "12点备份正常。" + FileDX;
                        nr = true;
                    }
                    else
                    {
                        label8.ForeColor = Color.Red;
                        label8.Text = "12点备份异常，请检查。";
                        nr = false;
                    }
                }
            }
            if (x.Hour >= 16)
            {
                label10.Text = "";
                bool n16 = ExFile(@"\\172.16.16.10\nr\" + GetWeekInt(DateTime.Now.DayOfWeek).ToString() + @"\", DateTime.Now.ToString("expyyMMdd") + "16", ".dump.gz");
                if (x.Hour == 16)
                {
                    if (x.Minute > 30)
                    {
                        if (n16)
                        {
                            label9.ForeColor = Color.Black;
                            label9.Text = "16点备份正常。" + FileDX;
                            nr = true;
                        }
                        else
                        {
                            label9.ForeColor = Color.Red;
                            label9.Text = "16点备份异常，请检查。";
                            nr = false;
                        }
                    }
                }
                else
                {
                    if (n16)
                    {
                        label9.ForeColor = Color.Black;
                        label9.Text = "16点备份正常。" + FileDX;
                        nr = true;
                    }
                    else
                    {
                        label9.ForeColor = Color.Red;
                        label9.Text = "16点备份异常，请检查。";
                        nr = false;
                    }
                }
            }
            if (x.Hour >= 20)
            {
                
                bool n20 = ExFile(@"\\172.16.16.10\nr\" + GetWeekInt(DateTime.Now.DayOfWeek).ToString() + @"\", DateTime.Now.ToString("expyyMMdd") + "20", ".dump.gz");
                if (x.Hour == 20)
                {
                    if (x.Minute > 30)
                    {
                        
                        if (n20)
                        {
                            label10.ForeColor = Color.Black;
                            label10.Text = "20点备份正常。" + FileDX;
                            bool j8 = ExFile(path + @"\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\", DateTime.Now.ToString("yyyyMMdd"), "-2.jpg");
                            if (j8)
                            {
                                label6.ForeColor = Color.Black;
                                label6.Text = "20点截图已保存。";
                                jt = true;
                            }
                            else
                            {
                                label6.ForeColor = Color.Red;
                                label6.Text = "20点截图未保存";
                                jt = false;
                            }
                            nr = true;
                        }
                        else
                        {
                            label10.ForeColor = Color.Red;
                            label10.Text = "20点备份异常，请检查。";
                            bool j8 = ExFile(path + @"\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\", DateTime.Now.ToString("yyyyMMdd"), "-2.jpg");
                            if (j8)
                            {
                                label6.ForeColor = Color.Black;
                                label6.Text = "20点截图已保存。";
                                jt = true;
                            }
                            else
                            {
                                label6.ForeColor = Color.Red;
                                label6.Text = "20点截图未保存";
                                jt = false;
                            }
                            nr = false;
                        }
                    }
                }
                else
                {
                    if (n20)
                    {
                        label10.ForeColor = Color.Black;
                        label10.Text = "20点备份正常。" + FileDX;
                        bool j8 = ExFile(path + @"\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\", DateTime.Now.ToString("yyyyMMdd"), "-2.jpg");
                        if (j8)
                        {
                            label6.ForeColor = Color.Black;
                            label6.Text = "20点截图已保存。";
                            jt = true;
                        }
                        else
                        {
                            label6.ForeColor = Color.Red;
                            label6.Text = "20点截图未保存";
                            jt = false;
                        }
                        nr = true;
                    }
                    else
                    {
                        label10.ForeColor = Color.Red;
                        label10.Text = "20点备份异常，请检查。";
                        bool j8 = ExFile(path + @"\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\", DateTime.Now.ToString("yyyyMMdd"), "-2.jpg");
                        if (j8)
                        {
                            label6.ForeColor = Color.Black;
                            label6.Text = "20点截图已保存。";
                            jt = true;
                        }
                        else
                        {
                            label6.ForeColor = Color.Red;
                            label6.Text = "20点截图未保存";
                            jt = false;
                        }
                        nr = true;
                        nr = false;
                    }
                }

            }
            if (x.Hour < 8)
            {
                label5.Text = label6.Text = label7.Text = label8.Text = label9.Text = label10.Text = "";
                nr = true;
                jt = true;
            }
        }
        void CheckPC() 
        {
            bool has = false;
            foreach (Control c in this.Controls) 
            {
                if (c.GetType().Name == "Button")
                {
                    if (PingPC(((Button)c).Tag.ToString()))
                    {
                        if (((Button)c).Tag.ToString() != "")
                        {
                            ((Button)c).BackColor = Color.Green;
                            if (!has)
                            {
                                pc = true;
                            }
                        }
                    }
                    else
                    {
                        if (((Button)c).Tag.ToString() != "")
                        {
                            ((Button)c).BackColor = Color.Red;
                            has = true;
                            pc = false;
                        }
                    }
                }
            }
        }
        bool PingPC(string IP) 
        {
            Ping pin = new Ping();
            PingReply pr = pin.Send(IP);
            if (pr.Status == IPStatus.Success)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        //检测文件大小
        bool ExFile(string paths, string startstr, string endstr) 
        {
            if (System.IO.Directory.Exists(paths))
            {
                string[] Files = Directory.GetFiles(paths);
                foreach (string i in Files)
                {
                    if (i.StartsWith(paths + startstr) && i.ToLower().EndsWith(endstr))
                    {
                        FileInfo fi = new FileInfo(i);
                        FileDX = (fi.Length / 1024 / 1024).ToString() + "MB";
                        return true;
                    }
                }
            }
            return false;
        }
        //转换日期
        int GetWeekInt(DayOfWeek dw) 
        {
            switch (dw) 
            {
                case DayOfWeek.Monday :
                    return 1;
                case DayOfWeek.Tuesday :
                    return 2;
                case DayOfWeek.Wednesday:
                    return 3;
                case DayOfWeek.Thursday:
                    return 4;
                case DayOfWeek.Friday:
                    return 5;
                case DayOfWeek.Saturday:
                    return 6;
                default :
                    return 0;
            }
        }
    }
}
