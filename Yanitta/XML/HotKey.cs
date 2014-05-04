﻿using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Xml.Serialization;

namespace System.Windows.Input
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class HotKey : IDisposable, INotifyPropertyChanged
    {
        // Track whether Dispose has been called.
        private bool disposed = false;

        [XmlIgnore]
        private Key key;

        [XmlIgnore]
        private ModifierKeys modifier;

        [NonSerialized]
        private HwndSource m_HandleSource = null;

        #region Win API

        private const int WM_HOTKEY = 0x312;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("user32.dll", EntryPoint = "RegisterHotKey", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool apiRegisterHotKey(IntPtr hWnd, int id, ModifierKeys fsModifiers, int vk);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("user32.dll", EntryPoint = "UnregisterHotKey", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool apiUnregisterHotKey(IntPtr hWnd, int id);

        #endregion Win API

        /// <summary>
        ///
        /// </summary>
        public event EventHandler<HandledEventArgs> Pressed;

        /// <summary>
        ///
        /// </summary>
        public HotKey()
            : this(Key.None, ModifierKeys.None)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifier"></param>
        public HotKey(Key key, ModifierKeys modifier)
        {
            this.Key = key;
            this.Modifier = modifier;

            this.m_HandleSource = new HwndSource(new HwndSourceParameters());
            this.m_HandleSource.AddHook(HwndSourceHook);
            this.m_HandleSource.Disposed += (o, e) =>
            {
                if (!this.IsEmpty)
                {
                    apiUnregisterHotKey(this.m_HandleSource.Handle, this.RawHotKey);
                }
                this.IsRegistered = false;
            };
        }

        private IntPtr HwndSourceHook(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (message == WM_HOTKEY)
            {
                if (lParam.ToInt32() == this.RawHotKey)
                {
                    var handledEventArgs = new HandledEventArgs();

                    if (this.Pressed != null)
                    {
                        this.Pressed(this, handledEventArgs);
                        handled = handledEventArgs.Handled;
                    }

                    return new IntPtr(1);
                }
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Defines a system-wide hot key.
        /// </summary>
        /// <param name="window">The window that will receive messages generated by the hot key.</param>
        public void Register()
        {
            if (this.IsEmpty)
                throw new Exception("HotKey is empty!");

            this.Unregister();

            var intKey = KeyInterop.VirtualKeyFromKey(this.Key);
            this.IsRegistered = apiRegisterHotKey(this.m_HandleSource.Handle, this.RawHotKey, this.Modifier, intKey);

            if (!this.IsRegistered)
                throw new ApplicationException("HotKey (" + this + ") already in use");

            Debug.WriteLine("Registered HotKey: " + this);
        }

        /// <summary>
        /// Frees a hot key previously registered by the calling thread.
        /// </summary>
        public void Unregister()
        {
            if (this.m_HandleSource != null && !this.m_HandleSource.IsDisposed && !this.IsEmpty)
            {
                Debug.WriteLine("Unregister HotKey: " + this);
                apiUnregisterHotKey(this.m_HandleSource.Handle, this.RawHotKey);
            }

            this.IsRegistered = false;
        }

        /// <summary>
        ///
        /// </summary>
        [XmlAttribute]
        [NotifyParentProperty(true)]
        public Key Key
        {
            get { return this.key; }
            set
            {
                if (this.key != value)
                {
                    this.key = value;
                    OnPropertyChanged("Key");
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        [XmlAttribute]
        public ModifierKeys Modifier
        {
            get { return this.modifier; }
            set
            {
                if (this.Modifier != value)
                {
                    this.modifier = value;
                    OnPropertyChanged("Modifier");
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public bool IsRegistered { get; private set; }

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public int RawHotKey
        {
            get { return KeyInterop.VirtualKeyFromKey(this.Key) << 16 | (int)this.Modifier & 0xFFFF; }
        }

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public object Tag { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsEmpty
        {
            get { return this.Key == Key.None || this.Modifier == ModifierKeys.None; }
        }

        [XmlIgnore]
        public bool Control
        {
            get { return (this.modifier & ModifierKeys.Control) != 0; }
            set
            {
                if (value)
                    this.modifier |= ModifierKeys.Control;
                else
                    this.modifier &= ~ModifierKeys.Control;

                OnPropertyChanged("Control");
                OnPropertyChanged("Modifier");
            }
        }

        [XmlIgnore]
        public bool Shift
        {
            get { return (this.modifier & ModifierKeys.Shift) != 0; }
            set
            {
                if (value)
                    this.modifier |= ModifierKeys.Shift;
                else
                    this.modifier &= ~ModifierKeys.Shift;

                OnPropertyChanged("Shift");
                OnPropertyChanged("Modifier");
            }
        }

        [XmlIgnore]
        public bool Alt
        {
            get { return (this.modifier & ModifierKeys.Alt) != 0; }
            set
            {
                if (value)
                    this.modifier |= ModifierKeys.Alt;
                else
                    this.modifier &= ~ModifierKeys.Alt;

                OnPropertyChanged("Alt");
                OnPropertyChanged("Modifier");
            }
        }

        [XmlIgnore]
        public bool Windows
        {
            get { return (this.modifier & ModifierKeys.Windows) != 0; }
            set
            {
                if (value)
                    this.modifier |= ModifierKeys.Windows;
                else
                    this.modifier &= ~ModifierKeys.Windows;

                OnPropertyChanged("Windows");
                OnPropertyChanged("Modifier");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is HotKey))
                return false;
            var mhotKey = obj as HotKey;
            return (this.Key == mhotKey.Key && this.Modifier == mhotKey.Modifier);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.RawHotKey ^ this.RawHotKey;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Key == Key.None)
                return "(Empty)";

            if (this.Modifier == ModifierKeys.None)
                return this.Key.ToString();

            var modstr = this.Modifier.ToString()
                .Replace(',', '+')
                .Replace(" ",  "");
            return modstr + "+" + this.Key;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        ~HotKey()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                this.Unregister();
                if (disposing)
                {
                    if (this.m_HandleSource != null && !this.m_HandleSource.IsDisposed)
                    {
                        this.m_HandleSource.RemoveHook(HwndSourceHook);
                        this.m_HandleSource.Dispose();
                    }
                }

                this.m_HandleSource = null;
                this.Modifier = ModifierKeys.None;
                this.Key      = Key.None;
                this.Tag      = null;
                this.Pressed  = null;

                // Note disposing has been done.
                disposed = true;
            }
        }
    }
}