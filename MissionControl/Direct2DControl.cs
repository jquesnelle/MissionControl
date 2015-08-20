using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX.Direct2D1;
using SharpDX;

namespace MissionControl
{
    public partial class Direct2DControl : Control
    {
        public Direct2DControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw  | ControlStyles.UserPaint, true);

        }

        protected Factory factory;
        protected WindowRenderTarget renderTarget;
        private bool renderObjectsCreated;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if(!DesignMode)
            {
                factory = new Factory(FactoryType.SingleThreaded, DebugLevel.None);
                CreateRenderObjects();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (Disposing)
            {
                DisposeRenderObjects();
                if (factory != null)
                {
                    factory.Dispose();
                    factory = null;
                }
            }
            base.Dispose(disposing);
        }

        protected override void OnResize(EventArgs e)
        {
            if (renderTarget != null && !renderTarget.IsDisposed)
                renderTarget.Resize(new Size2(Width, Height));
            base.OnResize(e);
        }

        public delegate void RenderEventHandler(object sender, EventArgs e, RenderTarget target);

        public event EventHandler RendererCreated;
        public event EventHandler RendererDestroyed;
        public event RenderEventHandler Render;
        public event EventHandler RenderError;

        protected void OnCreateRenderObjects(EventArgs e)
        {
            if (RendererCreated != null)
                RendererCreated(this, e);
        }

        protected void OnDisposeRenderObjects(EventArgs e)
        {
            if (RendererDestroyed != null)
                RendererDestroyed(this, e);
        }

        protected void OnRender(EventArgs e)
        {
            if (Render != null)
                Render(this, e, renderTarget);
        }

        private void CreateRenderObjects()
        {
            RenderTargetProperties renderTargetProperties = new RenderTargetProperties();
            HwndRenderTargetProperties hwRenderTargetProperties = new HwndRenderTargetProperties()
            {
                Hwnd = this.Handle,
                PixelSize = new Size2(ClientSize.Width, ClientSize.Height)
            };
            renderTarget = new WindowRenderTarget(factory, renderTargetProperties, hwRenderTargetProperties);
            renderObjectsCreated = true;
            OnCreateRenderObjects(EventArgs.Empty);
        }

        private void DisposeRenderObjects()
        {
            if(renderObjectsCreated)
            {
                renderTarget.Dispose();
                renderTarget = null;
                OnDisposeRenderObjects(EventArgs.Empty);
                renderObjectsCreated = false;
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (renderObjectsCreated)
            {
                StartRendering();
            }
            else
            {
                pe.Graphics.Clear(System.Drawing.Color.CornflowerBlue);
                pe.Graphics.DrawString(Name, Font, Brushes.White, PointF.Empty);
            }
        }
        private void StartRendering()
        {
            try
            {
                if (renderTarget == null)
                    CreateRenderObjects();
                renderTarget.BeginDraw();

                try
                {
                    OnRender(EventArgs.Empty);
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    renderTarget.EndDraw();
                }
            }
            catch (SharpDXException ex)
            {
                if (ex.ResultCode.Code == unchecked((int)0x8899000C))
                {
                    DisposeRenderObjects();
                    CreateRenderObjects();
                    Invalidate();
                }
            }
        }
    }
}
