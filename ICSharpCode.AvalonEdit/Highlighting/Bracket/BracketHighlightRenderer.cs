﻿using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Highlighting.Bracket
{
    public class BracketHighlightRenderer : IBackgroundRenderer
    {
        private BracketSearchResult result;
        private Pen borderPen;
        private Brush backgroundBrush;
        private TextView textView;

        public void SetHighlight(BracketSearchResult result)
        {
            if (this.result != result)
            {
                this.result = result;
                textView.InvalidateLayer(this.Layer);
            }
        }

        public BracketHighlightRenderer(TextView textView)
        {
            if (textView == null)
                throw new ArgumentNullException("textView");

            this.textView = textView;
            this.textView.BackgroundRenderers.Add(this);

            this.borderPen = new Pen(new SolidColorBrush(Color.FromArgb(0xFF, 0x71, 0x0B, 0xCB)), 1);
            this.backgroundBrush = new SolidColorBrush(Color.FromArgb(0xFF,0x71,0x0B,0xCB));
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Selection; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (this.result == null)
                return;

            var builder = new BackgroundGeometryBuilder();
            builder.AlignToMiddleOfPixels = true;
            builder.AddSegment(textView, new TextSegment { StartOffset = result.OpeningOffset, Length = 1 });
            builder.CloseFigure(); // prevent connecting the two segments
            builder.AddSegment(textView, new TextSegment { StartOffset = result.ClosingOffset, Length = 1 });

            var geometry = builder.CreateGeometry();
            if (geometry != null)
                drawingContext.DrawGeometry(backgroundBrush, borderPen, geometry);
        }
    }
}
