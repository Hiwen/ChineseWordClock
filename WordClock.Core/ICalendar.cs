using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordClock.Core
{
    /// <summary>
    /// 日历接口
    /// </summary>
    public interface ICalendar
    {
        /// <summary>
        /// 开启日历
        /// </summary>
        void Start();

        /// <summary>
        /// 结束日历
        /// </summary>
        void Stop();

        /// <summary>
        /// 日历宽
        /// </summary>
        int Width { get; }

        /// <summary>
        /// 日历高
        /// </summary>
        int Height { get; }

        /// <summary>
        /// 日历字体
        /// </summary>
        string FontName { get; }

        /// <summary>
        /// 当前时间
        /// </summary>
        DateTime Now { get; }
    }
}
