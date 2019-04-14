using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WordClock.Core
{
    /// <summary>
    /// 日历基类,封装日历中的部分通用代码
    /// </summary>
    public abstract class CalenderBase : ICalendar
    {
        protected IDictionary<int, string> numberMap = new Dictionary<int, string>()
        {
            { 0,  "　〇" }, { 1,  "　一" }, { 2,  "　二" }, { 3,  "　三" }, { 4,  "　四" },
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

        PointF center;
        System.Windows.Forms.Timer timerMain;

        public virtual PointF Center => center;

        public virtual int Width => _w;
        public virtual int Height => _h;
        public virtual string FontName => _fontName;

        public virtual DateTime Now => _time;

        public CalenderBase(int w, int h, string fontName)
        {
            _w = w;
            _h = h;
            center = new PointF(_w / 2, _h / 2);
            _fontName = fontName;

            timerMain = new System.Windows.Forms.Timer();
            timerMain.Interval = 1000;
            timerMain.Tick += TimerMain_Tick;
        }

        /// <summary>
        /// 同步系统时间
        /// </summary>
        public virtual void SyncTime()
        {
            while (_time.Second == DateTime.Now.Second)
            {
            }

            _time = DateTime.Now.AddSeconds(-1);
        }



        Mutex mtx = new Mutex(false);

        /// <summary>
        /// 显示当前帧
        /// </summary>
        protected virtual void Display()
        {
            var tsk = new Task(() =>
            {
                mtx.WaitOne();
                try
                {
                    // 当前帧的过渡帧1(上一帧提前准备好的数据)
                    Present();

                    // 当前帧的过渡帧2
                    PreDraw();
                    DrawTime(Now, -4.2f);
                    Present();

                    var tt = Now.AddSeconds(1);
                    // 当前帧的回弹帧
                    PreDraw();
                    DrawTime(tt, -0.3f);
                    Thread.Sleep(10);
                    Present();

                    // 当前帧
                    PreDraw();
                    DrawTime(tt, 0);
                    Present();

                    // 下一帧的过渡帧1
                    PreDraw();
                    DrawTime(tt, -2.1f);

                    // 每一帧结尾进行手动GC,保持帧稳定
                    GC.Collect();
                }
                catch (Exception e)
                {
                    throw e;
                    // ignore
                }
                finally
                {
                    mtx.ReleaseMutex();
                }
            });

            tsk.Start();
        }

        /// <summary>
        /// 定时器回调,绘制当前帧,准备下一帧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerMain_Tick(object sender, EventArgs e)
        {
            if (mtx.WaitOne(1))
            {
                mtx.ReleaseMutex();
                Display();
            }
            _time = _time.AddSeconds(1);
        }

        public virtual void Start()
        {
            SyncTime();
            timerMain.Start();

            PreDraw();
            DrawTime(Now, 0f);
        }

        public virtual void Stop()
        {
            timerMain.Stop();
            // 等待最后一帧绘制完成
            mtx.WaitOne();
            mtx.ReleaseMutex();
        }

        /// <summary>
        /// 重置画布变换参数
        /// </summary>
        protected abstract void ResetTransform();

        /// <summary>
        /// 清空画布
        /// </summary>
        protected abstract void Clear();

        /// <summary>
        /// 绘制Clock中心部分文字
        /// </summary>
        /// <param name="s"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected abstract void DrawClockString(string s, float x, float y);

        /// <summary>
        /// 普通文字绘制
        /// </summary>
        /// <param name="s"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected abstract void DrawString(string s, float x, float y);

        /// <summary>
        /// 当前时间绘制
        /// </summary>
        /// <param name="s"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected abstract void DrawCurrentTimeString(string s, float x, float y);

        /// <summary>
        /// 围绕point旋转画布angle角度(单位度)
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="point"></param>
        protected abstract void RotateAt(float angle, PointF point);

        /// <summary>
        /// 绘制秒圈
        /// </summary>
        /// <param name="r"></param>
        /// <param name="num"></param>
        /// <param name="tip"></param>
        /// <param name="angleOffset"></param>
        /// <param name="curIdx"></param>
        protected virtual void DrawCircle(float r, int num, string tip, float angleOffset, int curIdx)
        {
            ResetTransform();

            var offset = new SizeF(r, 0);

            DrawCurrentTimeString(tip, Center.X + r, Center.Y);

            RotateAt(angleOffset, Center);

            var ang = 360f / num;
            DrawCurrentTimeString(numberMap[curIdx], Center.X + r, Center.Y);

            for (int i = 1; i < num; i++)
            {
                RotateAt(ang, Center);
                DrawString(numberMap[(i + curIdx) % num], Center.X + r, Center.Y);
            }
        }

        /// <summary>
        /// 绘制其他圈(分,时,日,月)
        /// </summary>
        /// <param name="r"></param>
        /// <param name="num"></param>
        /// <param name="tip"></param>
        /// <param name="priTip"></param>
        /// <param name="hasZero"></param>
        /// <param name="curIdx"></param>
        protected virtual void DrawCircle(float r, int num, string tip, bool priTip, bool hasZero, int curIdx)
        {
            var offset = new SizeF(r, 0);

            var text = $"{numberMap[curIdx % num]}{tip}";

            ResetTransform();

            DrawCurrentTimeString(text, Center.X + r, Center.Y);

            if (num > 1)
            {
                var ang = 360f / num;
                for (int i = 1; i < num; i++)
                {
                    //旋转角度和平移
                    RotateAt(ang, Center);

                    var idx = (i + curIdx) % num;
                    if (idx == 0 && !hasZero)
                    {
                        idx = num;
                    }

                    DrawString(numberMap[idx], Center.X + r, Center.Y);
                }
            }
        }

        public virtual float Radius => Height / 1.5f;

        /// <summary>
        /// 绘制指定的时间到缓存中
        /// </summary>
        /// <param name="time"></param>
        /// <param name="angleOffset">秒圈偏移角度(正数为顺时针,负数为逆时针)</param>
        protected virtual void DrawTime(DateTime time, float angleOffset)
        {
            ResetTransform();
            Clear();

            DrawClockString(DateTime.Now.ToString("dddd\nyyyy-MM-dd\nHH:mm:ss"), Width / 2, Height / 2);

            float r = Radius;
            float minr = 150;
            float dr = (r - minr) / 44;

            var now = DateTime.Now;

            r -= dr * 8;
            var secondPre = numberMap[time.Second].Length > 2 ? "　　　秒" : "　　秒";
            DrawCircle(r, 60, secondPre, angleOffset, time.Second);

            r -= dr * 9;
            DrawCircle(r, 60, "分", false, true, now.Minute);

            r -= dr * 9;
            DrawCircle(r, 24, "时", false, true, now.Hour);

            var dayNum = DateTime.DaysInMonth(time.Year, now.Month);
            r -= dr * 8;
            DrawCircle(r, dayNum, "日", false, false, now.Day);

            r -= dr * 8;
            DrawCircle(r, 12, "月", false, false, now.Month);
        }

        /// <summary>
        /// 呈现缓存中的时间
        /// </summary>
        protected abstract void Present();

        protected abstract void PreDraw();
    }
}
