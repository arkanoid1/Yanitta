﻿using ICSharpCode.AvalonEdit.Document;
using System;
using System.Globalization;

namespace ICSharpCode.AvalonEdit
{
    /// <summary>
    /// Represents a text location with a visual column.
    /// </summary>
    public struct TextViewPosition : IEquatable<TextViewPosition>
    {
        int line, column, visualColumn;

        /// <summary>
        /// Gets/Sets Location.
        /// </summary>
        public TextLocation Location
        {
            get { return new TextLocation(line, column); }
            set
            {
                line   = value.Line;
                column = value.Column;
            }
        }

        /// <summary>
        /// Gets/Sets the line number.
        /// </summary>
        public int Line
        {
            get { return line; }
            set { line = value; }
        }

        /// <summary>
        /// Gets/Sets the (text) column number.
        /// </summary>
        public int Column
        {
            get { return column; }
            set { column = value; }
        }

        /// <summary>
        /// Gets/Sets the visual column number.
        /// Can be -1 (meaning unknown visual column).
        /// </summary>
        public int VisualColumn
        {
            get { return visualColumn; }
            set { visualColumn = value; }
        }

        /// <summary>
        /// Creates a new TextViewPosition instance.
        /// </summary>
        public TextViewPosition(int line, int column, int visualColumn)
        {
            this.line = line;
            this.column = column;
            this.visualColumn = visualColumn;
        }

        /// <summary>
        /// Creates a new TextViewPosition instance.
        /// </summary>
        public TextViewPosition(int line, int column)
            : this(line, column, -1)
        {
        }

        /// <summary>
        /// Creates a new TextViewPosition instance.
        /// </summary>
        public TextViewPosition(TextLocation location, int visualColumn)
        {
            line = location.Line;
            column = location.Column;
            this.visualColumn = visualColumn;
        }

        /// <summary>
        /// Creates a new TextViewPosition instance.
        /// </summary>
        public TextViewPosition(TextLocation location)
            : this(location, -1)
        {
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                                 "[TextViewPosition Line={0} Column={1} VisualColumn={2}]",
                                 line, column, visualColumn);
        }

        #region Equals and GetHashCode implementation

        // The code in this region is useful if you want to use this structure in collections.
        // If you don't need it, you can just remove the region and the ": IEquatable<Struct1>" declaration.

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is TextViewPosition)
                return Equals((TextViewPosition)obj); // use Equals method below
            else
                return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 0;
            unchecked
            {
                hashCode += 1000000007 * Line.GetHashCode();
                hashCode += 1000000009 * Column.GetHashCode();
                hashCode += 1000000021 * VisualColumn.GetHashCode();
            }
            return hashCode;
        }

        /// <summary>
        /// Equality test.
        /// </summary>
        public bool Equals(TextViewPosition other)
        {
            return Line == other.Line && Column == other.Column && VisualColumn == other.VisualColumn;
        }

        /// <summary>
        /// Equality test.
        /// </summary>
        public static bool operator ==(TextViewPosition left, TextViewPosition right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Inequality test.
        /// </summary>
        public static bool operator !=(TextViewPosition left, TextViewPosition right)
        {
            return !(left.Equals(right)); // use operator == and negate result
        }

        #endregion Equals and GetHashCode implementation
    }
}