using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace X330Backlight
{

    public class ColorConfig
    {
        public Color FontColor { get; set; } = Color.Black;

        public Color MarginStartColor { get; set; } = Color.FromArgb(212, 212, 212);//Color.FromArgb(113, 113, 113);

        public Color MarginEndColor { get; set; } = Color.FromArgb(212, 212, 212); //Color.FromArgb(58, 58, 58);

        public Color DropDownItemBackColor { get; set; } = Color.FromArgb(212, 212, 212); //Color.FromArgb(34, 34, 34);

        public Color DropDownItemStartColor { get; set; } = Color.Orange;

        public Color DropDownItemEndColor { get; set; } = Color.FromArgb(160, 100, 20);

        public Color MenuItemStartColor { get; set; } = Color.FromArgb(52, 106, 159);

        public Color MenuItemEndColor { get; set; } = Color.FromArgb(212, 212, 212);//Color.FromArgb(73, 124, 174);

        public Color SeparatorColor { get; set; } = Color.Gray;

        public Color MainMenuStartColor { get; set; } = Color.FromArgb(93, 93, 93);

        public Color MainMenuEndColor { get; set; } = Color.FromArgb(34, 34, 34);

        public Color DropDownBorder { get; set; } = Color.FromArgb(40, 96, 151);
    }


    public class MenuRenderer : ToolStripProfessionalRenderer
    {
        private readonly ColorConfig _colorconfig = new ColorConfig();

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            e.ToolStrip.ForeColor = _colorconfig.FontColor;
            if (e.ToolStrip is ToolStripDropDown)
            {
                e.Graphics.FillRectangle(new SolidBrush(_colorconfig.DropDownItemBackColor), e.AffectedBounds);
            }
            else if (e.ToolStrip is MenuStrip)
            {
                var blend = new Blend();
                float[] fs = {0f, 0.3f, 0.5f, 0.8f, 1f};
                float[] f = {0f, 0.5f, 0.9f, 0.5f, 0f};
                blend.Positions = fs;
                blend.Factors = f;
                FillLineGradient(e.Graphics, e.AffectedBounds, _colorconfig.MainMenuStartColor,
                    _colorconfig.MainMenuEndColor, 90f, blend);
            }
            else
            {
                base.OnRenderToolStripBackground(e);
            }
        }


        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            FillLineGradient(e.Graphics, e.AffectedBounds, _colorconfig.MarginStartColor, _colorconfig.MarginEndColor, 0f,
                null);
        }


        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.ToolStrip is MenuStrip)
            {
                if (e.Item.Selected || e.Item.Pressed)
                {
                    Blend blend = new Blend();
                    float[] fs = {0f, 0.3f, 0.5f, 0.8f, 1f};
                    float[] f = {0f, 0.5f, 1f, 0.5f, 0f};
                    blend.Positions = fs;
                    blend.Factors = f;
                    FillLineGradient(e.Graphics, new Rectangle(0, 0, e.Item.Size.Width, e.Item.Size.Height),
                        _colorconfig.MenuItemStartColor, _colorconfig.MenuItemEndColor, 90f, blend);
                }
                else
                    base.OnRenderMenuItemBackground(e);
            }
            else if (e.ToolStrip is ToolStripDropDown)
            {
                if (e.Item.Selected)
                {
                    FillLineGradient(e.Graphics, new Rectangle(0, 0, e.Item.Size.Width, e.Item.Size.Height),
                        _colorconfig.DropDownItemStartColor, _colorconfig.DropDownItemEndColor, 90f, null);
                }
            }
            else
            {
                base.OnRenderMenuItemBackground(e);
            }
        }


        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(_colorconfig.SeparatorColor), 0, 2, e.Item.Width, 2);
        }


        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip is ToolStripDropDown)
            {
                e.Graphics.DrawRectangle(new Pen(_colorconfig.DropDownBorder),
                    new Rectangle(0, 0, e.AffectedBounds.Width - 1, e.AffectedBounds.Height - 1));
            }
            else
            {
                base.OnRenderToolStripBorder(e);
            }
        }


        private void FillLineGradient(Graphics g, Rectangle rect, Color startcolor, Color endcolor, float angle,
            Blend blend)
        {
            LinearGradientBrush linebrush = new LinearGradientBrush(rect, startcolor, endcolor, angle);
            if (blend != null)
            {
                linebrush.Blend = blend;
            }

            var path = new GraphicsPath();
            path.AddRectangle(rect);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPath(linebrush, path);
        }
    }

}
