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
using WordClock.Core;

namespace WordClock
{
    /// <summary>
    /// WinForm日历
    /// </summary>
    public class ClockCalendar : CalenderBase, IDisposable
    {
        Bitmap img;
        Graphics _gBmp;
        Brush brush;
        Brush brushRed;
        Brush brushBackColor;

        StringFormat formatNear;
        StringFormat formatCenter;
        Pen pen;

        Font font;
        Font fontClock;
        Graphics text;


        public ClockCalendar(int w, int h, int fontScale, string fontName, Graphics textt)
            : base(w, h, fontName)
        {

            img = new Bitmap(Width, Height);
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

        protected override void ResetTransform()
        {
            _gBmp.ResetTransform();
        }

        protected override void Clear()
        {
            _gBmp.FillRectangle(brushBackColor, 0, 0, Width, Height);
        }

        protected override void DrawClockString(string s, float x, float y)
        {
            _gBmp.DrawString(s, fontClock, brush, x, y, formatCenter);

        }
        protected override void DrawString(string s, float x, float y)
        {
            _gBmp.DrawString(s, font, brush, x, y, formatNear);
        }

        protected override void DrawCurrentTimeString(string s, float x, float y)
        {
            _gBmp.DrawString(s, font, brushRed, x, y, formatNear);
        }


        protected override void RotateAt(float angle, PointF point)
        {
            Matrix mtxRotate = _gBmp.Transform;
            mtxRotate.RotateAt(angle, point);
            _gBmp.Transform = mtxRotate;
        }

        protected override void Present()
        {
            text.DrawImage(img, 0, 0);
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
