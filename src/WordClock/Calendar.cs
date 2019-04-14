// <copyright file="Calendar.cs" company="zondy">
//		Copyright (c) Zondy. All rights reserved.
// </copyright>
// <author>WeiWenGang</author>
// <date>2019/4/11 0:00:58</date>
// <summary>文件功能描述</summary>
// <modify>
//		修改人:		
//		修改时间:	
//		修改描述:	
//		版本: 1.0	
// </modify>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WordClock
{
    public class ClockCalendar : IDisposable
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

        DateTime _time = DateTime.Now;

        int _w;
        int _h;
        string _fontName;

        Font font;
        Font fontClock;

        Bitmap img;
        Graphics _gBmp;

        Brush brush;
        Brush brushRed;
        Brush brushBackColor;

        StringFormat formatNear;
        StringFormat formatCenter;
        Pen pen;

        PointF center;

        public int Width => _w;
        public int Height => _h;

        private System.Windows.Forms.Timer timerMain;

        public class TextInfo
        {
            public float X { get; set; }
            public float Y { get; set; }
            public string Text { get; set; }
            public float Angle { get; set; }

        }
        Graphics text;

        public Action<TextInfo> Drawer;

        public ClockCalendar(int w, int h, int fontScale, string fontName, Graphics textt)
        {
            _w = w;
            _h = h;

            center = new PointF(_w / 2, _h / 2);

            _fontName = fontName;

            timerMain = new System.Windows.Forms.Timer();
            timerMain.Interval = 1000;
            timerMain.Tick += TimerMain_Tick;

            img = new Bitmap(_w, _h);
            _gBmp = Graphics.FromImage(img);

            //选择字体、字号、风格
            font = new Font(fontName, 18f * fontScale / textt.DpiX, FontStyle.Regular);
            fontClock = new Font(/*"微软雅黑"*/fontName, 35f * fontScale / textt.DpiX, FontStyle.Regular);

            //创建一个画刷，颜色是纯色
            brush = new SolidBrush(Color.FromArgb(0, 255, 255));
            brushRed = new SolidBrush(Color.FromArgb(255, 0, 0));
            brushBackColor = new SolidBrush(Color.Black);

            formatNear = new StringFormat();
            //指定字符串的水平对齐方式
            formatNear.Alignment = StringAlignment.Near;
            //表示字符串的垂直对齐方式
            formatNear.LineAlignment = StringAlignment.Center;

            formatCenter = new StringFormat();
            formatCenter.Alignment = StringAlignment.Center;
            formatCenter.LineAlignment = StringAlignment.Center;

            pen = new Pen(Color.Green, 3);

            text = textt;
        }

        public void Start()
        {
            SyncTime();
            timerMain.Start();
            DrawToImage(img, _time, 0f);
        }

        public void Stop()
        {
            timerMain.Stop();
        }

        /// <summary>
        /// 同步系统时间
        /// </summary>
        public void SyncTime()
        {
            while (_time.Second == DateTime.Now.Second)
            {
            }

            _time = DateTime.Now.AddSeconds(-1);
        }


        void DrawToImage(Bitmap img, DateTime time, float angleOffset)
        {
            _gBmp.ResetTransform();
            _gBmp.FillRectangle(brushBackColor, 0, 0, Width, Height);
            _gBmp.DrawString(DateTime.Now.ToString("dddd\nyyyy-MM-dd\nHH:mm:ss"),
                fontClock, brush, Width / 2, Height / 2, formatCenter);

            float r = Height / 1.5f;
            float minr = 150;
            float dr = (r - minr) / 44;

            var now = DateTime.Now;

            r -= dr * 8;
            var secondPre = numberMap[time.Second].Length > 2 ? "　　　秒" : "　　秒";
            DrawCircle(_gBmp, r, 60, secondPre, angleOffset, time.Second);

            r -= dr * 9;
            DrawCircle(_gBmp, r, 60, "分", false, true, now.Minute);

            r -= dr * 9;
            DrawCircle(_gBmp, r, 24, "时", false, true, now.Hour);

            var dayNum = DateTime.DaysInMonth(time.Year, now.Month);
            r -= dr * 8;
            DrawCircle(_gBmp, r, dayNum, "日", false, false, now.Day);

            r -= dr * 8;
            DrawCircle(_gBmp, r, 12, "月", false, false, now.Month);
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
            
            var text = $"{numberMap[curIdx % num]}{tip}";

            gBmp.ResetTransform();

            gBmp.DrawString(text, font, brushRed, center + offset, formatNear);

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
                    // 当前帧的过渡帧1(上一帧提前准备好的数据)
                    text.DrawImage(img, 0, 0);

                    // 当前帧的过渡帧2
                    DrawToImage(img, _time, -4.2f);
                    text.DrawImage(img, 0, 0);

                    var tt = _time.AddSeconds(1);
                    // 当前帧的回弹帧
                    DrawToImage(img, tt, -0.3f);
                    Thread.Sleep(10);
                    text.DrawImage(img, 0, 0);

                    // 当前帧
                    DrawToImage(img, tt, 0);
                    text.DrawImage(img, 0, 0);

                    // 下一帧的过渡帧1
                    DrawToImage(img, tt, -2.1f);

                    // 每一帧结尾进行手动GC,保持帧稳定
                    GC.Collect();
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

        public void Dispose()
        {
            _gBmp.Dispose();
            img.Dispose();
            font.Dispose();
            fontClock.Dispose();
            brush.Dispose();
            brushRed.Dispose();
            brushBackColor.Dispose();
            pen.Dispose();
        }
    }
}
