using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DrawTimeLine
{
    public partial class Form1 : Form
    {
        List<TimeState> tss;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap(panel1.Width - 22, (tss.Count + 1) * 100);
            Graphics g = Graphics.FromImage(bitmap);
            g.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Width);
            //Graphics g = pbBack.CreateGraphics();
            Font fontText = new Font("微软雅黑", 10);
            Font fontHour = new Font("微软雅黑", 14);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            Pen penBlack = new Pen(Color.Black, 8);
            Pen penOrange = new Pen(Color.Orange, 8);
            Pen penRed = new Pen(Color.FromArgb(255,0,0), 8);
            Pen penGreen = new Pen(Color.SeaGreen, 8);
            float l, t, r;
            l = 100;
            t = 20;
            r = bitmap.Width - 20;
            int i;
            Pen pen = penBlack;
            for (i = 0; i < tss.Count; i++)
            {
                g.DrawString(tss[i].Date, fontText, Brushes.Black, new RectangleF(l - 100, t - 10, 100, 20), sf);
                g.DrawLine(penBlack, l, t, r, t);
                float x;
                int hour = 0;
                for (x = l; x <= r + 1; x += (r - l) / 24)
                {
                    g.DrawLine(Pens.Black, x, t - 8, x, t);
                    g.DrawString(hour.ToString(), fontHour, Brushes.Black, new RectangleF(x - 15, t + 5, 30, 20), sf);
                    hour++;
                }
                float x1 = 0;
                float x2 = 0;
                Pen PrePen = null;
                foreach (State s in tss[i].State)
                {
                    PrePen = pen;
                    if (s.state == "吃")
                    {
                        pen = penRed;
                    }
                    else if (s.state == "醒")
                    {
                        pen = penOrange;
                    }
                    else if (s.state == "睡")
                    {
                        pen = penGreen;
                    }
                    int h = Convert.ToInt32(s.Time.Substring(0, 2));
                    int m = Convert.ToInt32(s.Time.Substring(2, 2));
                    x2 = (r - l) / 24 * h + m * (r - l) / 24 / 60 + l;
                    if (x1 == 0)
                    {
                        x1 = x2;
                        g.DrawLine(PrePen, l, t, x2, t);
                        continue;
                    }
                    //if(tss[i].Date == "20200503")
                    //Thread.Sleep(1200);
                    g.DrawLine(PrePen, x1, t, x2, t);
                    x1 = x2;
                }
                if (i != tss.Count - 1)
                {
                    g.DrawLine(pen, x1, t, r, t);
                }
                t += 100;
            }

            g.DrawLine(penRed, l, t, l + (r - l) / 24, t);
            g.DrawString("吃", fontText, Brushes.Black, new RectangleF(l, t + 5, (r - l) / 24, 20), sf);
            g.DrawLine(penGreen, l + (r - l) / 24 * 2, t, l + (r - l) / 24 * 3, t);
            g.DrawString("睡", fontText, Brushes.Black, new RectangleF(l + (r - l) / 24 * 2, t + 5, (r - l) / 24, 20), sf);
            g.DrawLine(penOrange, l + (r - l) / 24 * 4, t, l + (r - l) / 24 * 5, t);
            g.DrawString("醒", fontText, Brushes.Black, new RectangleF(l + (r - l) / 24 * 4, t + 5, (r - l) / 24, 20), sf);
            g.DrawLine(penBlack, l + (r - l) / 24 * 6, t, l + (r - l) / 24 * 7, t);
            g.DrawString("未知", fontText, Brushes.Black, new RectangleF(l + (r - l) / 24 * 6, t + 5, (r - l) / 24, 20), sf);

            pbBack.Size = bitmap.Size;
            pbBack.Image = bitmap;

            //Bitmap b = bitmap.Clone() as Bitmap;
            //b.RotateFlip(RotateFlipType.Rotate90FlipNone);
            //Clipboard.SetImage(b);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //pbBack.Image = Image.FromFile(@"E:\VS2017\BingWallpaper\BingWallpaper\bin\Debug\Pics\20191103.jpg");
            tss = new List<TimeState>();
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件(*.txt)|*.txt";
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (ofd.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            string str;
            using (StreamReader sr = new StreamReader(ofd.FileName, Encoding.Default))
            {
                str = sr.ReadToEnd().Replace("----", "-");
            }
            str = str.Replace(Environment.NewLine, "醒").Replace("醒吃", "吃").Replace("睡醒", "睡").Replace("醒醒", "醒");
            str = str.Replace("吃醒", "吃");//测试情况
            string[] days = str.Split('-');
            tss.Clear();
            foreach (string s in days)
            {
                if (s.Length > 0)
                {
                    TimeState ts = new TimeState();
                    ts.Date = s.Substring(0, 8);
                    //State state = new State();
                    //state.Time = s.Substring(9, 4);
                    //state.state = s.Substring(10, 1);
                    //ts.State.Add()
                    State state = new State();
                    int i;
                    for (i = 9; i < s.Length; i++)
                    {
                        if (char.IsDigit(s[i]))
                        {
                            state.Time = string.Format("{0}{1}", s[i], s.Substring(i + 1, 3));
                            i += 3;
                        }
                        else
                        {
                            state.state = s.Substring(i, 1);
                            ts.State.Add(new State(state));
                            if (i >= s.Length - 1)
                            {
                                break;
                            }
                            //if (!char.IsDigit(s[i + 1]))
                            //{
                            //    i++;
                            //}
                        }
                    }
                    tss.Add(ts);
                }
            }

            string msg = "";
            foreach (var k in tss)
            {
                msg = k.Date + Environment.NewLine;
                foreach (var kk in k.State)
                {
                    msg += string.Format("{0} {1}", kk.Time, kk.state) + Environment.NewLine;
                }
                //MessageBox.Show(msg);
            }
        }
    }
}
