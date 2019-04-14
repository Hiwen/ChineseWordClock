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

        Graphics text;

        ClockCalendar _calender;

        void Exit()
        {
            _calender.Stop();
            _calender.Dispose();
            text.Dispose();

            Close();
        }

        public ScreenSaver()
        {
            fontScale = 96;
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
            // Set the preview window as the parent of this window
            NativeMethods.SetParent(this.Handle, PreviewWndHandle);

            // Make this a child window so it will close when the parent dialog closes
            NativeMethods.SetWindowLong(this.Handle, -16, new IntPtr(NativeMethods.GetWindowLong(this.Handle, -16) | 0x40000000));

            // Place our window inside the parent
            Rectangle ParentRect;
            NativeMethods.GetClientRect(PreviewWndHandle, out ParentRect);
            Size = new Size(ParentRect.Size.Width + 1, ParentRect.Size.Height + 1);
            Location = new Point(-1, -1);

            // 的屏保设置界面预览时,减小字体
            fontScale = 48;
        }

        public ScreenSaver(bool WindowMode = false) : this()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            this.BackColor = Color.Transparent;
        }


        int fontScale;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            text = this.CreateGraphics();

            _calender = new ClockCalendar(Width, Height, fontScale, "微软雅黑", text);
            _calender.Start();
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
