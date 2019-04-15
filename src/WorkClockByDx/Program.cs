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
using WordClock.Core;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;

namespace WorkClockByDx
{
    public class Program : ScreenSaverBase
    {
        [STAThread]
        static void Main(string[] args)
        {
            new Program().Run(args);
        }

        protected override void Run_c()
        {
            var settings = new SettingsForm();
            Application.Run(settings);
        }

        protected override void Run_exe()
        {
            var bnd = Screen.PrimaryScreen.Bounds;
            var app = new Direct2D1App();
            app.Run(new DemoConfiguration("SharpDX DirectWrite Text Rendering Demo", bnd.Width, bnd.Height));
        }

        protected override void Run_p(IntPtr sa)
        {
            var bnd = Screen.PrimaryScreen.Bounds;
            var app = new Direct2D1App(sa);
            app.Run(new DemoConfiguration("SharpDX DirectWrite Text Rendering Demo", bnd.Width, bnd.Height));

        }

        protected override void Run_s()
        {
            Run_exe();
        }

        protected override void Run_w()
        {
            Run_exe();
        }
    }
}
