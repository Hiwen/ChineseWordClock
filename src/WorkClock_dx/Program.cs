// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Threading;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Samples;
using SharpDX.Windows;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;

namespace WorkClock_dx
{
    public class Program : Direct2D1DemoApp
    {
        public TextFormat TextFormat { get; private set; }
        public TextLayout TextLayout { get; private set; }

        protected override void Initialize(DemoConfiguration demoConfiguration)
        {
            base.Initialize(demoConfiguration);

            // Initialize a TextFormat
            TextFormat = new TextFormat(FactoryDWrite, "微软雅黑", 32)
            {
                TextAlignment = TextAlignment.Center,
                ParagraphAlignment = ParagraphAlignment.Center
            };

            RenderTarget2D.TextAntialiasMode = TextAntialiasMode.Cleartype;

            //RenderTarget2D.Transform = Matrix3x2.Rotation(3.14f / 4, new Vector2(300, 300));

            // Initialize a TextLayout
            TextLayout = new TextLayout(FactoryDWrite, "eeeee", TextFormat, demoConfiguration.Width, demoConfiguration.Height);

        }

        CalendarBySharpDX _calendar;

        /// <summary>
        /// Create Form for this demo.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        protected override Form CreateForm(DemoConfiguration config)
        {
            return new RenderForm(config.Title)
            {
                FormBorderStyle = FormBorderStyle.None,
                ShowIcon = false,
                ShowInTaskbar = false,
                WindowState = FormWindowState.Maximized,
                BackColor = System.Drawing.Color.Black,
                ClientSize = new System.Drawing.Size(config.Width, config.Height)
            };
        }

        protected override void BeginRun()
        {
            base.BeginRun();
            _calendar = new CalendarBySharpDX(Width, Height, "微软雅黑",
                RenderTarget2D, FactoryDWrite, _swapChain, Device, BackBufferView);

            _calendar.OnPresent += s =>
            {
                // RenderTarget2D.DrawTextLayout(new Vector2(0, 0), new TextLayout(FactoryDWrite, s, TextFormat, Width, Height), SceneColorBrush, DrawTextOptions.None);
            };
        }

        private void _calendar_OnPresent()
        {
            RenderTarget2D.DrawTextLayout(new Vector2(0, 0), TextLayout, SceneColorBrush, DrawTextOptions.None);
        }

        protected override void EndRun()
        {
            _calendar.Stop();
            base.EndRun();
        }

        bool firstDraw = true;

        protected override void Draw(DemoTime time)
        {
            base.Draw(time);

            if (firstDraw)
            {
                firstDraw = false;
                _calendar.Start();
            }

            Thread.Sleep(100);

            // Draw the TextLayout
            // RenderTarget2D.DrawTextLayout(new Vector2(0, 0), TextLayout, SceneColorBrush, DrawTextOptions.None);

            //TextLayout.SetLocaleName("xxxxxx", new TextRange());
            //RenderTarget2D.DrawTextLayout(new Vector2(0, 100), TextLayout, SceneColorBrush, DrawTextOptions.None);
        }

        protected override void BeginDraw()
        {

        }

        protected override void EndDraw()
        {

        }

        [STAThread]
        static void Main(string[] args)
        {
            Program program = new Program();

            var bnd = Screen.PrimaryScreen.Bounds;

            program.Run(new DemoConfiguration("SharpDX DirectWrite Text Rendering Demo", bnd.Width, bnd.Height));
        }
    }
}
