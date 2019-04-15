using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Samples;
using SharpDX.Windows;
using WordClock.Core;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;

namespace WorkClockByDx
{

    public class Direct2D1App : Direct2D1DemoApp
    {
        bool _preview = false;
        CalendarBySharpDX _calendar;
        IntPtr _PreviewWndHandle = IntPtr.Zero;

        public Direct2D1App()
        {

        }

        public Direct2D1App(IntPtr PreviewWndHandle) : this()
        {
            _PreviewWndHandle = PreviewWndHandle;

            // 的屏保设置界面预览时,减小字体
            _preview = true;

        }

        protected override void Initialize(DemoConfiguration demoConfiguration)
        {
            base.Initialize(demoConfiguration);

            RenderTarget2D.TextAntialiasMode = TextAntialiasMode.Cleartype;
        }


        /// <summary>
        /// Create Form for this demo.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        protected override Form CreateForm(DemoConfiguration config)
        {
            var f = new RenderForm(config.Title)
            {
                FormBorderStyle = FormBorderStyle.None,
                ShowIcon = false,
                ShowInTaskbar = false,
                WindowState = FormWindowState.Maximized,
                BackColor = System.Drawing.Color.Black,
                ClientSize = new Size(config.Width, config.Height)
            };

            f.KeyDown += F_KeyDown;
            f.MouseDown += F_MouseDown;

            if (_PreviewWndHandle != IntPtr.Zero)
            {
                // Set the preview window as the parent of this window
                NativeMethods.SetParent(f.Handle, _PreviewWndHandle);

                // Make this a child window so it will close when the parent dialog closes
                NativeMethods.SetWindowLong(f.Handle, -16, new IntPtr(NativeMethods.GetWindowLong(f.Handle, -16) | 0x40000000));

                // Place our window inside the parent
                System.Drawing.Rectangle ParentRect;
                NativeMethods.GetClientRect(_PreviewWndHandle, out ParentRect);
                f.ClientSize =new Size(ParentRect.Size.Width + 1, ParentRect.Size.Height + 1);
                f.Location = new System.Drawing.Point(-1, -1);
            }

            return f;
        }

        private void F_MouseDown(object sender, MouseEventArgs e)
        {
            MyExit();
        }

        void MyExit()
        {
            FormClosed = true;
            _calendar.Stop();
            _calendar.Dispose();
            Exit();
        }

        private void F_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Alt && e.KeyCode != Keys.PrintScreen &&
                e.KeyCode != Keys.Menu && e.KeyCode != Keys.Tab)
            {
                MyExit();
            }
        }

        protected override void BeginRun()
        {
            base.BeginRun();
            _calendar = new CalendarBySharpDX(Width, Height, "微软雅黑",
                RenderTarget2D, FactoryDWrite, _swapChain, Device, BackBufferView, _preview);
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
        }

        protected override void BeginDraw()
        {

        }

        protected override void EndDraw()
        {

        }

    }
}
