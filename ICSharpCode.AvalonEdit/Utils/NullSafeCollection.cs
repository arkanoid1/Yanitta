﻿using System;
using System.Collections.ObjectModel;

namespace ICSharpCode.AvalonEdit.Utils
{
    /// <summary>
    /// A collection that cannot contain null values.
    /// </summary>
    [Serializable]
    public class NullSafeCollection<T> : Collection<T> where T : class
    {
        /// <inheritdoc/>
        protected override void InsertItem(int index, T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            base.InsertItem(index, item);
        }

        /// <inheritdoc/>
        protected override void SetItem(int index, T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            base.SetItem(index, item);
        }
    }
}