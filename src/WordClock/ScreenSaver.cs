using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordClock
{
    public partial class ScreenSaver : Form
    {
        IDictionary<int, string> numberMap = new Dictionary<int, string>()
        {
            { 0, "零" },      { 1, "一" },      { 2, "二" },      { 3, "三" },      { 4, "四" },
            { 5, "五" },      { 6, "六" },      { 7, "七" },      { 8, "八" },      { 9, "九" },
            { 10, "十" },     { 11, "十一" },   { 12, "十二" },   { 13, "十三" },   { 14, "十四" },
            { 15, "十五" },   { 16, "十六" },   { 17, "十七" },   { 18, "十八" },   { 19, "十九" },
            { 20, "二十" },   { 21, "二十一" }, { 22, "二十二" }, { 23, "二十三" }, { 24, "二十四" },
            { 25, "二十五" }, { 26, "二十六" }, { 27, "二十七" }, { 28, "二十八" }, { 29, "二十九" },
            { 30, "三十" },   { 31, "三十一" }, { 32, "三十二" }, { 33, "三十三" }, { 34, "三十四" },
            { 35, "三十五" }, { 36, "三十六" }, { 37, "三十七" }, { 38, "三十八" }, { 39, "三十九" },
            { 40, "四十" },   { 41, "四十一" }, { 42, "四十二" }, { 43, "四十三" }, { 44, "四十四" },
            { 45, "四十五" }, { 46, "四十六" }, { 47, "四十七" }, { 48, "四十八" }, { 49, "四十九" },
            { 50, "五十" },   { 51, "五十一" }, { 52, "五十二" }, { 53, "五十三" }, { 54, "五十四" },
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
        Pen pen;
        StringFormat formatFar;
        StringFormat formatNear;
        StringFormat formatCenter;
        PointF center;
        int iof = 0;

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


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            text = this.CreateGraphics();

            //创建一个画刷，颜色是纯色
            brush = new SolidBrush(Color.FromArgb(0, 255, 255));
            brushRed = new SolidBrush(Color.FromArgb(255, 0, 0));
            brushBackColor = new SolidBrush(Color.Black);

            //选择字体、字号、风格
            font = new Font("Adobe Gothic Std", 18f, FontStyle.Regular);
            fontClock = new Font("Adobe Gothic Std", 35f, FontStyle.Regular);
            
            img = new Bitmap(Width, Height);

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
            timerMain.Start();
        }
        

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            text.DrawString($"请您稍候... ...", fontClock, brush, center, formatCenter);
        }
        
        
        void DrawToImage(DateTime time, float angleOffset)
        {
            Graphics gBmp = Graphics.FromImage(img);

            gBmp.FillRectangle(brushBackColor, 0, 0, Width, Height);
            //在位置（150，200）处绘制文字
            gBmp.DrawString(time.ToString("HH:mm:ss"), fontClock, brush, Width / 2, Height / 2, formatCenter);
            
            float r = Height / 1.5f;
            float minr = 150;
            float dr = (r - minr) / 44;
            //gBmp.DrawLine(p1, center.X, center.Y, center.X + r, center.Y);
            iof++;
            
            r -= dr * 8;
            DrawCircle(gBmp, r, 60, "秒", angleOffset, time.Second);

            r -= dr * 9;
            DrawCircle(gBmp, r, 60, "分", false, true, time.Minute);

            r -= dr * 9;
            DrawCircle(gBmp, r, 24, "时", false, true, time.Hour);

            var dayNum = DateTime.DaysInMonth(time.Year, time.Month);
            r -= dr * 8;
            DrawCircle(gBmp, r, dayNum, "日", false, false, time.Day);

            r -= dr * 6;
            DrawCircle(gBmp, r, 12, "月", false, false, time.Month);

            r -= dr * 3;
            DrawCircle(gBmp, r, 7, (int)time.DayOfWeek);
        }

        private void DrawCircle(Graphics gBmp, float r, int num, string tip, float angleOffset, int curIdx)
        {
            gBmp.ResetTransform();

            //旋转角度和平移
            Matrix mtxRotate = gBmp.Transform;
            mtxRotate.RotateAt(angleOffset, center);
            gBmp.Transform = mtxRotate;

            var offset = new SizeF(r, 0);
            var ang = 360f / num;
            gBmp.DrawString($"{numberMap[curIdx]}{tip}", font, brushRed, center + offset, formatNear);
            
            for (int i = 1; i < num; i++)
            {
                //旋转角度和平移
                mtxRotate = gBmp.Transform;
                mtxRotate.RotateAt(ang, center);
                gBmp.Transform = mtxRotate;

                gBmp.DrawString(numberMap[(i + curIdx) % num], font, brush, center + offset, formatNear);
            }
        }

        private void DrawCircle(Graphics gBmp, float r, int num, string tip, bool priTip, bool hasZero, int curIdx)
        {
            var offset = new SizeF(r, 0);
            var ang = 360f / num;

            int i = 0;
            int fix = 0;
            if (!hasZero)
            {
                fix = 1;
            }

            var f = formatNear;

            var text = $"{numberMap[(i + curIdx) % num]}{tip}";
            if (priTip)
            {
                text = $"{tip}{numberMap[(i + curIdx) % num]}";
                f = formatFar;
            }

            if (text == "周七")
            {
                text = "周日";
            }

            gBmp.ResetTransform();

            gBmp.DrawString(text, font, brushRed, center + offset, f);

            i++;

            for (; i < num; i++)
            {
                //旋转角度和平移
                Matrix mtxRotate = gBmp.Transform;
                mtxRotate.RotateAt(ang, center);
                gBmp.Transform = mtxRotate;
                gBmp.DrawString(numberMap[(i + curIdx) % num + fix], font, brush, center + offset, formatNear);
            }
        }

        // 绘制周
        private void DrawCircle(Graphics gBmp, float r, int num, int curIdx)
        {
            var offset = new SizeF(r, 0);
            var ang = 360f / num;

            int i = 0;
            int fix = 1;

            var f = formatFar;
            var ori = numberMap[7];
            numberMap[7] = "日";

            var text = $"周{numberMap[(i + curIdx) % num]}";

            gBmp.ResetTransform();
            gBmp.DrawString(text, font, brushRed, center + offset, f);
            
            for (i++; i < num; i++)
            {
                //旋转角度和平移
                Matrix mtxRotate = gBmp.Transform;
                mtxRotate.RotateAt(ang, center);
                gBmp.Transform = mtxRotate;
                gBmp.DrawString(numberMap[(i + curIdx) % num + fix], font, brush, center + offset, formatNear);
            }

            numberMap[7] = ori;
        }
        
        private void TimerMain_Tick(object sender, EventArgs e)
        {
            text.DrawImage(img, 0, 0);

            new Action(() => DrawToImage(DateTime.Now.AddSeconds(1), 0)).BeginInvoke(null, null);
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            Exit();
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Exit();
        }


        void RefrencesCode()
        {
            var img = new Bitmap(500, 500);

            Graphics gBmp = Graphics.FromImage(img);

            //添加文字
            String str1 = "1ABCDE";
            String str2 = "2ABCDEABCDE";
            String str3 = "3ABCDEABCDEABCDE";
            Font font = new Font("微软雅黑", 16);
            SolidBrush sbrush1 = new SolidBrush(Color.Green);
            SolidBrush sbrush2 = new SolidBrush(Color.Red);

            //笔刷渐变效果
            //LinearGradientBrush sbrush3 = new LinearGradientBrush(new PointF(100, 100), new PointF(250,250), Color.Red ,Color.Black); 
            SolidBrush sbrush3 = new SolidBrush(Color.Blue);

            //绘制饼图
            //SolidBrush s1 = new SolidBrush(Color.Pink);
            //gBmp.FillPie(s1, 150, 150, 200, 150, 90, 200);
            //SolidBrush s2 = new SolidBrush(Color.Purple);
            //gBmp.FillPie(s2, 150, 150, 200, 150, 290, 70);
            //SolidBrush s3 = new SolidBrush(Color.Green);
            //gBmp.FillPie(s3, 150, 150, 200, 150, 0, 60);
            //SolidBrush s4 = new SolidBrush(Color.Brown);
            //gBmp.FillPie(s4, 150, 150, 200, 150, 60, 30);

            //文字位置中心点坐标
            Pen p1 = new Pen(Color.Green, 3);
            gBmp.DrawLine(p1, 0, 100, 200, 100);
            gBmp.DrawLine(p1, 100, 0, 100, 200);
            // Rectangle rect = new Rectangle(200,200,100,70);
            // gBmp.DrawEllipse(p1,rect);


            StringFormat format1 = new StringFormat();
            //指定字符串的水平对齐方式
            format1.Alignment = StringAlignment.Far;
            //表示字符串的垂直对齐方式
            format1.LineAlignment = StringAlignment.Center;
            StringFormat format2 = new StringFormat();
            //指定字符串的水平对齐方式
            format2.Alignment = StringAlignment.Center;
            //表示字符串的垂直对齐方式
            format2.LineAlignment = StringAlignment.Center;
            StringFormat format3 = new StringFormat();
            //指定字符串的水平对齐方式
            format3.Alignment = StringAlignment.Near;
            //表示字符串的垂直对齐方式
            format3.LineAlignment = StringAlignment.Center;

            //旋转角度和平移
            Matrix mtxRotate = gBmp.Transform;
            mtxRotate.RotateAt(30, new PointF(100, 100));
            gBmp.Transform = mtxRotate;

            //gBmp.DrawString(str1, font, sbrush1, new PointF(100, 100), format1);
            //gBmp.DrawString(str2, font, sbrush2, new PointF(100, 100), format2);
            gBmp.DrawString(str3, font, sbrush3, new PointF(100, 100), format3);


            var g = this.CreateGraphics();
            g.DrawImage(img, 0, 0);

            gBmp.Dispose();
        }
    }
}
