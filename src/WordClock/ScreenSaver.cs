using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordClock
{
    public partial class ScreenSaver : Form
    {
        IDictionary<int, string> numberMap = new Dictionary<int, string>()
        {
            { 0,  "　零" }, { 1,  "　一" }, { 2,  "　二" }, { 3,  "　三" }, { 4,  "　四" },
            { 5,  "　五" }, { 6,  "　六" }, { 7,  "　七" }, { 8,  "　八" }, { 9,  "　九" },
            { 10, "　十" }, { 11, "十一" }, { 12, "十二" }, { 13, "十三" }, { 14, "十四" },
            { 15, "十五" }, { 16, "十六" }, { 17, "十七" }, { 18, "十八" }, { 19, "十九" },
            { 20, "二十" }, { 21, "二十一" }, { 22, "二十二" }, { 23, "二十三" }, { 24, "二十四" },
            { 25, "二十五" }, { 26, "二十六" }, { 27, "二十七" }, { 28, "二十八" }, { 29, "二十九" },
            { 30, "三十" }, { 31, "三十一" }, { 32, "三十二" }, { 33, "三十三" }, { 34, "三十四" },
            { 35, "三十五" }, { 36, "三十六" }, { 37, "三十七" }, { 38, "三十八" }, { 39, "三十九" },
            { 40, "四十" }, { 41, "四十一" }, { 42, "四十二" }, { 43, "四十三" }, { 44, "四十四" },
            { 45, "四十五" }, { 46, "四十六" }, { 47, "四十七" }, { 48, "四十八" }, { 49, "四十九" },
            { 50, "五十" }, { 51, "五十一" }, { 52, "五十二" }, { 53, "五十三" }, { 54, "五十四" },
            { 55, "五十五" }, { 56, "五十六" }, { 57, "五十七" }, { 58, "五十八" }, { 59, "五十九" },
            { 60, "六十" },
        };

        Graphics text;
        Brush brush;
        Brush brushRed;
        Brush brushBackColor;
        Font font;
        Font fontClock;
        Bitmap img;
        Bitmap img1;
        Bitmap img2;
        Bitmap img3;
        Pen pen;
        StringFormat formatFar;
        StringFormat formatNear;
        StringFormat formatCenter;
        PointF center;
        int iof = 0;
        DateTime _time = DateTime.Now;

        void Exit()
        {
            timerMain.Stop();

            text.Dispose();
            brush.Dispose();
            brushRed.Dispose();
            brushBackColor.Dispose();
            font.Dispose();
            fontClock.Dispose();
            img.Dispose();
            img1.Dispose();
            img2.Dispose();
            img3.Dispose();
            pen.Dispose();

            Close();
        }

        public ScreenSaver()
        {
            InitializeComponent();
        }


        public ScreenSaver(Screen s) : this()
        {
            this.Width = s.WorkingArea.Width;
            this.Height = s.WorkingArea.Height;
        }

        /// <summary>
        /// Initiate the form inside window's screen saver settings screen
        /// </summary>
        /// <param name="PreviewWndHandle"></param>
        public ScreenSaver(IntPtr PreviewWndHandle) : this()
        {
            //previewMode = true;

            // Set the preview window as the parent of this window
            NativeMethods.SetParent(this.Handle, PreviewWndHandle);

            // Make this a child window so it will close when the parent dialog closes
            NativeMethods.SetWindowLong(this.Handle, -16, new IntPtr(NativeMethods.GetWindowLong(this.Handle, -16) | 0x40000000));

            // Place our window inside the parent
            Rectangle ParentRect;
            NativeMethods.GetClientRect(PreviewWndHandle, out ParentRect);
            Size = new Size(ParentRect.Size.Width + 1, ParentRect.Size.Height + 1);
            Location = new Point(-1, -1);
        }
        public ScreenSaver(bool WindowMode = false) : this()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            this.BackColor = Color.Transparent;
        }

        void SyncTime()
        {
            while (_time.Second == DateTime.Now.Second)
            {
            }

            _time = DateTime.Now.AddSeconds(-1);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            text = this.CreateGraphics();

            //创建一个画刷，颜色是纯色
            brush = new SolidBrush(Color.FromArgb(0, 255, 255));
            brushRed = new SolidBrush(Color.FromArgb(255, 0, 0));
            brushBackColor = new SolidBrush(Color.Black);
            var g = CreateGraphics();
            //选择字体、字号、风格
            font = new Font("微软雅黑", 18f * 96f / g.DpiX, FontStyle.Regular);
            fontClock = new Font("微软雅黑", 35f * 96f / g.DpiX, FontStyle.Regular);

            img = new Bitmap(Width, Height);
            img1 = new Bitmap(Width, Height);
            img2 = new Bitmap(Width, Height);
            img3 = new Bitmap(Width, Height);

            pen = new Pen(Color.Green, 3);

            formatNear = new StringFormat();
            //指定字符串的水平对齐方式
            formatNear.Alignment = StringAlignment.Near;
            //表示字符串的垂直对齐方式
            formatNear.LineAlignment = StringAlignment.Center;

            formatFar = new StringFormat();
            formatFar.Alignment = StringAlignment.Far;
            formatFar.LineAlignment = StringAlignment.Center;

            formatCenter = new StringFormat();
            formatCenter.Alignment = StringAlignment.Center;
            formatCenter.LineAlignment = StringAlignment.Center;
            center = new PointF(Width / 2, Height / 2);

            timerMain.Tick += TimerMain_Tick;

            SyncTime();

            DrawToImage(img, _time, 0f);
            timerMain.Start();
        }

        void DrawToImage(Bitmap img, DateTime time, float angleOffset)
        {
            Graphics gBmp = Graphics.FromImage(img);

            gBmp.FillRectangle(brushBackColor, 0, 0, Width, Height);

            gBmp.DrawString(DateTime.Now.ToString("dddd\nyyyy-MM-dd\nHH:mm:ss"), fontClock, brush, Width / 2, Height / 2, formatCenter);

            float r = Height / 1.5f;
            float minr = 150;
            float dr = (r - minr) / 44;
            iof++;

            var now = DateTime.Now;

            r -= dr * 8;
            var secondPre = numberMap[time.Second].Length > 2 ? "　　　秒": "　　秒";
            DrawCircle(gBmp, r, 60, secondPre, angleOffset, time.Second);

            r -= dr * 9;
            DrawCircle(gBmp, r, 60, "分", false, true, now.Minute);

            r -= dr * 9;
            DrawCircle(gBmp, r, 24, "时", false, true, now.Hour);

            var dayNum = DateTime.DaysInMonth(time.Year, now.Month);
            r -= dr * 8;
            DrawCircle(gBmp, r, dayNum, "日", false, false, now.Day);

            r -= dr * 8;
            DrawCircle(gBmp, r, 12, "月", false, false, now.Month);
        }

        private void DrawCircle(Graphics gBmp, float r, int num, string tip, float angleOffset, int curIdx)
        {
            gBmp.ResetTransform();

            var offset = new SizeF(r, 0);

            gBmp.DrawString(tip, font, brushRed, center + offset, formatNear);

            Matrix mtxRotate = gBmp.Transform;
            mtxRotate.RotateAt(angleOffset, center);
            gBmp.Transform = mtxRotate;

            var ang = 360f / num;
            gBmp.DrawString(numberMap[curIdx], font, brushRed, center + offset, formatNear);

            for (int i = 1; i < num; i++)
            {
                mtxRotate = gBmp.Transform;
                mtxRotate.RotateAt(ang, center);
                gBmp.Transform = mtxRotate;
                gBmp.DrawString(numberMap[(i + curIdx) % num], font, brush, center + offset, formatNear);
            }
        }

        private void DrawCircle(Graphics gBmp, float r, int num, string tip, bool priTip, bool hasZero, int curIdx)
        {
            var offset = new SizeF(r, 0);

            var f = formatNear;

            var text = $"{numberMap[curIdx % num]}{tip}";
            if (priTip)
            {
                text = $"{tip}{numberMap[curIdx % num]}";
                f = formatFar;
            }

            gBmp.ResetTransform();

            gBmp.DrawString(text, font, brushRed, center + offset, f);

            if (num > 1)
            {
                var ang = 360f / num;
                for (int i = 1; i < num; i++)
                {
                    //旋转角度和平移
                    Matrix mtxRotate = gBmp.Transform;
                    mtxRotate.RotateAt(ang, center);
                    gBmp.Transform = mtxRotate;

                    var idx = (i + curIdx) % num;
                    if (idx == 0 && !hasZero)
                    {
                        idx = num;
                    }

                    gBmp.DrawString(numberMap[idx], font, brush, center + offset, formatNear);
                }
            }
        }

        Mutex mtx = new Mutex(false);

        /// <summary>
        /// 显示当前帧
        /// </summary>
        void DisplayImage()
        {
            var tsk = new Task(() =>
            {
                mtx.WaitOne();
                try
                {
                    text.DrawImage(img, 0, 0);
                    // 当前帧的过渡帧1
                    DrawToImage(img, _time, -2.1f);

                    text.DrawImage(img1, 0, 0);
                    // 当前帧的过渡帧2
                    DrawToImage(img1, _time, -4.2f);

                    text.DrawImage(img2, 0, 0);
                    var tt = _time.AddSeconds(1);
                    // 下一帧的回弹帧
                    DrawToImage(img2, tt, -0.3f);

                    Thread.Sleep(30);
                    text.DrawImage(img3, 0, 0);
                    // 下一帧
                    DrawToImage(img3, tt, 0f);
                }
                catch (Exception)
                {
                    // ignore
                }
                finally
                {
                    mtx.ReleaseMutex();
                }
            });

            tsk.Start();
        }

        private void TimerMain_Tick(object sender, EventArgs e)
        {
            if (mtx.WaitOne(1))
            {
                mtx.ReleaseMutex();
                DisplayImage();
            }
            _time = _time.AddSeconds(1);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode != Keys.Alt && e.KeyCode != Keys.PrintScreen && e.KeyCode != Keys.Menu)
            {
                Exit();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Exit();
        }
    }
}
