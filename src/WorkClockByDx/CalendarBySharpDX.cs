using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WordClock.Core;

namespace WorkClockByDx
{
    public class CalendarBySharpDX : CalenderBase, IDisposable
    {
        RenderTarget _rt;
        SharpDX.DirectWrite.Factory _fdw;
        SwapChain _sc;
        SharpDX.Direct3D11.Device _device;
        RenderTargetView _backBufferView;

        // inner resourse
        SolidColorBrush brush;
        SolidColorBrush brushRed;
        TextFormat formatNear;
        TextFormat formatCenter;
        TextLayout _tyNear;
        TextLayout _tyCenter;
        RawMatrix3x2 _oriTransform;
        bool _preView;


        public override float Radius => Height / (_preView ? 1f : 1.1f);

        public CalendarBySharpDX(int w, int h, string fontName,
            RenderTarget rt, SharpDX.DirectWrite.Factory fdw, SwapChain sc, 
            SharpDX.Direct3D11.Device device, RenderTargetView backBufferView, bool preView) 
            : base(w,h, fontName)
        {
            _rt = rt;
            _fdw = fdw;
            _sc = sc;
            _device = device;
            _backBufferView = backBufferView;
            _preView = preView;
            Init();
        }

        public override float Scale => _rt.Size.Width / 1920;

        void Init()
        {
            _oriTransform = _rt.Transform;

            brush = new SolidColorBrush(_rt, new SharpDX.Color(0, 255, 255));
            brushRed = new SolidColorBrush(_rt, SharpDX.Color.Red);

            formatNear = new TextFormat(_fdw, FontName, 32 * Scale)
            {
                TextAlignment = TextAlignment.Leading,
                ParagraphAlignment = ParagraphAlignment.Center
            };
            formatCenter = new TextFormat(_fdw, FontName, 65 * Scale)
            {
                TextAlignment = TextAlignment.Center,
                ParagraphAlignment = ParagraphAlignment.Center
            };

            _tyNear = new TextLayout(_fdw, "", formatNear, Width, Height);
            _tyCenter = new TextLayout(_fdw, "", formatCenter, Width, Height);
        }

        public void Dispose()
        {
            brush.Dispose();
            brushRed.Dispose();
            _tyNear.Dispose();
            _tyCenter.Dispose();
            formatNear.Dispose();
            formatCenter.Dispose();
        }

        protected override void Clear()
        {
            _rt.Clear(new SharpDX.Mathematics.Interop.RawColor4(0,0,0,0));
        }

        protected override void DrawClockString(string s, float x, float y)
        {
            var tl = new TextLayout(_fdw, s, formatCenter, Width, Height);
            _rt.DrawTextLayout(new Vector2(Width / 2 - x, Height / 2 - y), tl, brush, DrawTextOptions.None);
            tl.Dispose();
        }

        protected override void DrawCurrentTimeString(string s, float x, float y)
        {
            var tl = new TextLayout(_fdw, s, formatNear, Width, Height);
            _rt.DrawTextLayout(new Vector2(x - Radius + Width / 2 , 0), tl, brushRed, DrawTextOptions.None);
            tl.Dispose();
        }

        protected override void DrawString(string s, float x, float y)
        {
            var tl = new TextLayout(_fdw, s, formatNear, Width, Height);
            _rt.DrawTextLayout(new Vector2(x - Radius + Width / 2, 0), tl, brush, DrawTextOptions.None);
            tl.Dispose();
        }


        protected override void PreDraw()
        {
            Thread.Sleep(20);
            _device.ImmediateContext.Rasterizer.SetViewport(new Viewport(0, 0, Width, Height));
            _device.ImmediateContext.OutputMerger.SetTargets(_backBufferView);
            _rt.BeginDraw();
        }

        public event Action<string> OnPresent;
        protected override void Present()
        {
            ResetTransform();

            OnPresent?.Invoke(DateTime.Now.ToString("dddd\nyyyy-MM-dd\nHH:mm:ss"));

            _rt.EndDraw();
            _sc.Present(false ? 1 : 0, PresentFlags.None);
        }

        float rotatedAngle = 0;

        protected override void ResetTransform()
        {
            _rt.Transform = _oriTransform;
            rotatedAngle = 0;
        }

        protected override void RotateAt(float angle, PointF point)
        {
            rotatedAngle += angle;
            _rt.Transform = Matrix3x2.Rotation(rotatedAngle / 180 * (float)Math.PI, new Vector2(point.X, point.Y));
        }

    }
}
