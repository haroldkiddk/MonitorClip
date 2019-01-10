using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 获取剪贴板内容
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll")]
        protected static extern int SetClipboardViewer(int hWndViewer);
        [DllImport("user32.dll")]
        protected static extern bool ChangeClipboardChain(IntPtr hWndResume, IntPtr hWndNext);
        [DllImport("user32.dll")]
        protected static extern int SendMessage(IntPtr hWnd, int nMsg, IntPtr wParam, IntPtr lParam);
        IntPtr hNextClipboardViewer;

        private void Form1_Load(object sender, EventArgs e)
        {
            hNextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
        }

        void ShowNotify()
        {
            dataGridView1.DataSource = null;
            richTextBox1.Clear();
            string Str = Clipboard.GetText();
            if (Str.Contains("\t"))
            {
                DataTable dt = new DataTable();
                string[] StrSplitEnter = Regex.Split(Str, "\r\n");
                foreach(string s1 in StrSplitEnter)
                {
                    if (s1 != "")
                    {
                        DataRow row = dt.NewRow();
                        dt.Rows.Add(row);
                        string[] StrSplitTab = Regex.Split(s1, "\t");
                        int cnt = 0;
                        foreach (string s2 in StrSplitTab)
                        {
                            cnt++;
                            if (cnt > dt.Columns.Count)
                            {
                                dt.Columns.Add(cnt.ToString());
                            }
                            dt.Rows[dt.Rows.Count - 1][cnt - 1] = s2;
                        }
                    }
                }
                dataGridView1.DataSource = dt;
            }
            else
            {
                richTextBox1.AppendText(Str);
            }
        }

        void ShowImage()
        {
            pictureBox1.Image = Clipboard.GetImage();
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_DRAWCLPBORAD = 0x308;
            const int WM_CHANGCBCHAIN = 0X030D;
            switch (m.Msg)

            {
                case WM_DRAWCLPBORAD:
                    if (Clipboard.ContainsText())
                    {
                        ShowNotify();
                    }
                    else if (Clipboard.ContainsImage())
                    {
                        ShowImage();
                    }
                    SendMessage(hNextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                case WM_CHANGCBCHAIN:
                    if (hNextClipboardViewer == m.WParam)
                    {
                        hNextClipboardViewer = m.LParam;
                    }
                    else
                    {
                        SendMessage(hNextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    }
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
