using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WordClock.Core;

namespace WordClock
{
    internal class Program : ScreenSaverBase
    {

        /// <summary>
        /// Arguments for any Windows 98+ screensaver:
        /// 
        ///   ScreenSaver.scr           - Show the Settings dialog box.
        ///   ScreenSaver.scr /c        - Show the Settings dialog box, modal to the foreground window.
        ///   ScreenSaver.scr /p <HWND> - Preview Screen Saver as child of window <HWND>.
        ///   ScreenSaver.scr /s        - Run the Screen Saver.
        /// 
        /// Custom arguments:
        /// 
        ///   ScreenSaver.scr /w        - Run in normal resizable window mode.
        ///   ScreenSaver.exe           - Run in normal resizable window mode.
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            new Program().Run(args);
        }

        protected override void Run_c()
        {
            var settings = new SettingsForm();
            Application.Run(settings);
        }

        protected override void Run_p(IntPtr sa)
        {
            Application.Run(new ScreenSaver(sa));
        }

        protected override void Run_s()
        {
            Application.Run(new ScreenSaver());
        }

        protected override void Run_w()
        {
            Application.Run(new ScreenSaver(WindowMode: true));
        }
        protected override void Run_exe()
        {
            Application.Run(new ScreenSaver(WindowMode: true));
        }
    }
}
