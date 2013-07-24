﻿using ICSharpCode.AvalonEdit.Document;
using System;

namespace ICSharpCode.AvalonEdit.Rendering
{
    /// <summary>
    /// EventArgs for the <see cref="TextView.VisualLineConstructionStarting"/> event.
    /// </summary>
    public class VisualLineConstructionStartEventArgs : EventArgs
    {
        /// <summary>
        /// Gets/Sets the first line that is visible in the TextView.
        /// </summary>
        public DocumentLine FirstLineInView { get; private set; }

        /// <summary>
        /// Creates a new VisualLineConstructionStartEventArgs instance.
        /// </summary>
        public VisualLineConstructionStartEventArgs(DocumentLine firstLineInView)
        {
            if (firstLineInView == null)
                throw new ArgumentNullException("firstLineInView");
            this.FirstLineInView = firstLineInView;
        }
    }
}