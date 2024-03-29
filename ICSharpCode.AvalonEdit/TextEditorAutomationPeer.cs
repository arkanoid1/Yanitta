﻿using System.Diagnostics;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace ICSharpCode.AvalonEdit
{
    /// <summary>
    /// Exposes <see cref="TextEditor"/> to automation.
    /// </summary>
    public class TextEditorAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
    {
        /// <summary>
        /// Creates a new TextEditorAutomationPeer instance.
        /// </summary>
        public TextEditorAutomationPeer(TextEditor owner)
            : base(owner)
        {
            Debug.WriteLine("TextEditorAutomationPeer was created");
        }

        TextEditor TextEditor
        {
            get { return (TextEditor)base.Owner; }
        }

        void IValueProvider.SetValue(string value)
        {
            TextEditor.Text = value;
        }

        string IValueProvider.Value
        {
            get { return TextEditor.Text; }
        }

        bool IValueProvider.IsReadOnly
        {
            get { return TextEditor.IsReadOnly; }
        }

        /// <inheritdoc/>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Value)
                return this;

            if (patternInterface == PatternInterface.Scroll)
            {
                ScrollViewer scrollViewer = TextEditor.ScrollViewer;
                if (scrollViewer != null)
                    return UIElementAutomationPeer.CreatePeerForElement(scrollViewer);
            }

            return base.GetPattern(patternInterface);
        }

        internal void RaiseIsReadOnlyChanged(bool oldValue, bool newValue)
        {
            RaisePropertyChangedEvent(ValuePatternIdentifiers.IsReadOnlyProperty, oldValue, newValue);
        }
    }
}