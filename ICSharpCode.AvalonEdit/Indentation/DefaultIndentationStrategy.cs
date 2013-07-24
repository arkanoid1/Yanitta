﻿using ICSharpCode.AvalonEdit.Document;
using System;

namespace ICSharpCode.AvalonEdit.Indentation
{
    /// <summary>
    /// Handles indentation by copying the indentation from the previous line.
    /// Does not support indenting multiple lines.
    /// </summary>
    public class DefaultIndentationStrategy : IIndentationStrategy
    {
        /// <inheritdoc/>
        public virtual void IndentLine(TextDocument document, DocumentLine line)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (line == null)
                throw new ArgumentNullException("line");

            if (line.PreviousLine != null)
            {
                var indentationSegment = TextUtilities.GetWhitespaceAfter(document, line.PreviousLine.Offset);
                var indentation = document.GetText(indentationSegment);
                // copy indentation to line
                indentationSegment = TextUtilities.GetWhitespaceAfter(document, line.Offset);

                document.Replace(indentationSegment, indentation);
            }
        }

        /// <summary>
        /// Does nothing: indenting multiple lines is useless without a smart indentation strategy.
        /// </summary>
        public virtual void IndentLines(TextDocument document, int beginLine, int endLine)
        {
        }
    }
}