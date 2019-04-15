using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordClock.Core
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ScreenSaverBase : IScreenSaver
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
        public void Run(string[] args)
        {
            if (args.Length > 0)
            {
                string firstArgument = args[0].ToLower().Trim();
                string secondArgument = null;

                // Handle cases where arguments are separated by colon. 
                // Examples: /c:1234567 or /P:1234567
                if (firstArgument.Length > 2)
                {
                    secondArgument = firstArgument.Substring(3).Trim();
                    firstArgument = firstArgument.Substring(0, 2);
                }
                else if (args.Length > 1)
                    secondArgument = args[1];

                if (firstArgument == "/c")           // Configuration mode
                {
                    Run_c();
                }
                else if (firstArgument == "/p")      // Preview mode
                {
                    if (secondArgument == null)
                    {
                        MessageBox.Show("Sorry, but the expected window handle was not provided.",
                            "ScreenSaver", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    Run_p(new IntPtr(long.Parse(secondArgument)));
                }

                else if (firstArgument == "/s")      // Full-screen mode
                {
                    Run_s();
                }
                else if (firstArgument == "/w") // if executable, windowed mode.
                {
                    Run_w();
                }
                else    // Undefined argument
                {
                    MessageBox.Show("Sorry, but the command line argument \"" + firstArgument +
                        "\" is not valid.", "ScreenSaver",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                if (System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.EndsWith("exe")) // treat like /w
                {
                    Run_exe();
                }
                else // No arguments - treat like /c
                {
                    Run_c();
                }
            }
        }
        protected abstract void Run_c();

        protected abstract void Run_w();

        protected abstract void Run_p(IntPtr sa);

        protected abstract void Run_s();

        protected abstract void Run_exe();

    }
}
