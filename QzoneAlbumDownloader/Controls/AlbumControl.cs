﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MaterialSkin.Animations;
using System.Drawing.Drawing2D;
using MaterialSkin;
using System.ComponentModel;
using System.Runtime.InteropServices;
using MaterialSkin.Controls;

/// <summary>
/// QQ空间相册控件
/// </summary>
namespace QzoneAlbumDownloader.Controls
{
    public class AlbumControl : Control, IMaterialControl
    {

        #region 属性

        private Bitmap image = null;
        public Bitmap Image
        {
            get
            {
                return image;
            }

            set
            {
                value = GetSquareBitmap(value);
                image = value;
                Invalidate();
            }
        }

        private Bitmap loadingImage = null;
        public Bitmap LoadingImage
        {
            get => loadingImage;
            set
            {
                value = GetSquareBitmap(value);
                loadingImage = value;
                Invalidate();
            }
        }

        private bool isLoading = true;
        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;
                Invalidate();
            }
        }

        private string imageURL = string.Empty;
        public string ImageURL
        {
            get
            {
                return imageURL;
            }

            set
            {
                imageURL = value;
            }
        }

        private string title = string.Empty;
        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
                Invalidate();
            }
        }

        private Font hintFont = DefaultFont;
        public Font HintFont { get => hintFont; set => hintFont = value; }

        private string hintString = string.Empty;
        public string HintString { get => hintString; set => hintString = value; }

        private Color hintForeColor = DefaultForeColor;
        public Color HintForeColor { get => hintForeColor; set => hintForeColor = value; }

        #endregion

        #region 初始化

        [Browsable(false)]
        public int Depth { get; set; }
        [Browsable(false)]
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        [Browsable(false)]
        public MouseState MouseState { get; set; }

        public AlbumControl()
        {
            Padding = new Padding(10);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            MaterialContextMenuStrip cms = new AlbumControlContextMenuStrip();
            cms.Opening += ContextMenuStripOnOpening;
            cms.OnItemClickStart += ContextMenuStripOnItemClickStart;
            ContextMenuStrip = cms;

            animationManager = new AnimationManager(false)
            {
                Increment = 0.03,
                AnimationType = AnimationType.EaseOut
            };
            animationManager.OnAnimationProgress += sender => Invalidate();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            MouseDown += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left || args.Button == MouseButtons.Right)
                {
                    MouseState = MouseState.DOWN;

                    animationManager.StartNewAnimation(AnimationDirection.In, args.Location);
                    Invalidate();
                }
            };
            MouseUp += (sender, args) =>
            {
                MouseState = MouseState.HOVER;

                Invalidate();
            };

        }

        #endregion

        #region 右键菜单

        private class AlbumControlContextMenuStrip : MaterialContextMenuStrip
        {
            public readonly ToolStripItem undo = new MaterialToolStripMenuItem { Text = "撤销" };
            public readonly ToolStripItem seperator1 = new ToolStripSeparator();
            public readonly ToolStripItem cut = new MaterialToolStripMenuItem { Text = "剪切" };
            public readonly ToolStripItem copy = new MaterialToolStripMenuItem { Text = "复制" };
            public readonly ToolStripItem paste = new MaterialToolStripMenuItem { Text = "粘贴" };
            public readonly ToolStripItem delete = new MaterialToolStripMenuItem { Text = "删除" };
            public readonly ToolStripItem seperator2 = new ToolStripSeparator();
            public readonly ToolStripItem selectAll = new MaterialToolStripMenuItem { Text = "全选" };

            public AlbumControlContextMenuStrip()
            {
                Items.AddRange(new[]
                {
                    undo,
                    seperator1,
                    cut,
                    copy,
                    paste,
                    delete,
                    seperator2,
                    selectAll
                });
            }
        }

        private void ContextMenuStripOnItemClickStart(object sender, ToolStripItemClickedEventArgs toolStripItemClickedEventArgs)
        {
            switch (toolStripItemClickedEventArgs.ClickedItem.Text)
            {
                case "撤销":
                    break;
                case "剪切":
                    break;
                case "复制":
                    break;
                case "粘贴":
                    break;
                case "删除":
                    break;
                case "全选":
                    break;
            }
        }
        private void ContextMenuStripOnOpening(object sender, CancelEventArgs cancelEventArgs)
        {
            if (sender is AlbumControlContextMenuStrip strip)
            {

            }
        }

        #endregion

        #region 绘制

        private readonly AnimationManager animationManager;

        protected override void OnPaint(PaintEventArgs e)
        {
            ReloadSize();
            int a = Width - Padding.Left - Padding.Right;
            var g = e.Graphics;
            g.Clear(BackColor);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            //Draw Image
            if (!IsLoading && Image != null)
                g.DrawImage(Image, new Rectangle(Padding.Left, Padding.Top, a, a));
            else if (IsLoading && LoadingImage != null)
                g.DrawImage(LoadingImage, new Rectangle(Padding.Left, Padding.Top, a, a));
            //Draw Border
            Pen p = new Pen(Color.FromArgb(68, 69, 70));
            g.DrawLine(p, new Point(Padding.Left - 1, Padding.Top - 1), new Point(Padding.Left + a, Padding.Top - 1));
            g.DrawLine(p, new Point(Padding.Left + a, Padding.Top - 1), new Point(Padding.Left + a, Padding.Top + a));
            g.DrawLine(p, new Point(Padding.Left - 1, Padding.Top + a), new Point(Padding.Left + a, Padding.Top + a));
            g.DrawLine(p, new Point(Padding.Left - 1, Padding.Top + a), new Point(Padding.Left - 1, Padding.Top - 1));
            //Draw Ripple
            if (animationManager.IsAnimating())
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                for (int i = 0; i < animationManager.GetAnimationCount(); i++)
                {
                    var animationValue = animationManager.GetProgress(i);
                    var animationSource = animationManager.GetSource(i);

                    using (Brush rippleBrush = new SolidBrush(Color.FromArgb((int)(101 - (animationValue * 100)), Color.Black)))
                    {
                        var rippleSize = (int)(animationValue * Width * 2);
                        g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                    }
                }
                g.SmoothingMode = SmoothingMode.None;
            }
            //Draw Title
            StringFormat sf = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(Title, Font, new SolidBrush(ForeColor),
                new Rectangle(Padding.Left, a + Padding.Top + Padding.Bottom,
                a, (int)Font.Size + Padding.Bottom), sf);
            g.DrawString(HintString, HintFont, new SolidBrush(HintForeColor),
                new Rectangle(Padding.Left, a + Padding.Top + Padding.Bottom * 2 + (int)Font.Size + 2,
                a, (int)HintFont.Size + Padding.Bottom), sf);
            base.OnPaint(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            ReloadSize();
            base.OnSizeChanged(e);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            Invalidate();
            base.OnFontChanged(e);
        }

        #endregion

        #region 函数

        /// <summary>
        /// 获取方形照片
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private Bitmap GetSquareBitmap(Bitmap img)
        {
            int temp = 0;
            if (img == null)
                return null;
            if (img.Width > img.Height)
            {
                temp = (img.Width - img.Height) / 2;
                return CutImage(img, new Rectangle(temp, 0, img.Height, img.Height));
            }
            else if (img.Width < img.Height)
            {
                temp = (img.Height - img.Width) / 2;
                return CutImage(img, new Rectangle(0, temp, img.Width, img.Width));
            }
            else
                return img;
        }

        /// <summary>
        /// 裁切照片
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <param name="rc"></param>
        /// <returns></returns>
        private Bitmap CutImage(Bitmap sourceBitmap, Rectangle rc)
        {
            if (rc.Bottom < 0)
                return null;
            Bitmap TempsourceBitmap = new Bitmap(rc.Right - rc.Left, rc.Bottom - rc.Top);
            Graphics gr = Graphics.FromImage(TempsourceBitmap);
            gr.DrawImage(sourceBitmap, 0, 0, new RectangleF(rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top), GraphicsUnit.Pixel);
            gr.Dispose();
            return TempsourceBitmap;
        }

        /// <summary>
        /// 重新计算 Size
        /// </summary>
        public void ReloadSize()
        {
            Height = Width + Padding.Top + Padding.Bottom * 2 + (int)Font.Size + (int)HintFont.Size;
            return;
        }

        #endregion

    }
}
