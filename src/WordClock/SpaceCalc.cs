// <copyright file="RoomCalc.cs" company="zondy">
//		Copyright (c) Zondy. All rights reserved.
// </copyright>
// <author>WeiWenGang</author>
// <date>2019/4/11 0:01:27</date>
// <summary>文件功能描述</summary>
// <modify>
//		修改人:		
//		修改时间:	
//		修改描述:	
//		版本: 1.0	
// </modify>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordClock
{
    /// <summary>
    /// 空间计算辅助类
    /// </summary>
    public class SpaceCalc
    {
        int _w;
        int _h;

        int _rScale = 10;
        int _circleNum = 6;

        int r;
        int d;

        public SpaceCalc(int w, int h)
        {
            _w = w;
            _h = h;

            Init();
        }

        void Init()
        {
            int R = Math.Min(_h, _w);
            if (_w > _h)
            {
                r = _h / _rScale;
            }
            else
            {
                r = _w / _rScale;
            }

            d = (R - r) / _circleNum;
        }

        public int GetRi(int i)
        {
            return r + i * d;
        }
    }
}
