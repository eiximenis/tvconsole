﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TvConsole.Win32;

namespace TvConsole
{
    public class Win32TvScreenBuffer : ISecondaryScreenBuffer, IDisposable
    {
        private IntPtr _hstdout;
        private bool _disposed;
        private readonly bool _isDisposable;

        public IConsoleCursor Cursor { get; }

        public IConsoleColor ForeColor(ConsoleColor foreground) => new TvConsoleColor(_hstdout, foreground);
        public IConsoleColor BackColor(ConsoleColor background) => new TvConsoleColor(_hstdout, forecolor: null, backColor: background);
        public IConsoleColor CharacterColor(ConsoleColor foreground, ConsoleColor background) => new TvConsoleColor(_hstdout, foreground, background);
        public IConsoleColor ColorScope => TvConsoleColor.None(_hstdout);

        public ConsoleColor BackgroundColor
        {
            get
            {
                ConsoleNative.GetConsoleScreenBufferInfo(_hstdout, out CONSOLE_SCREEN_BUFFER_INFO info);
                return TvConsoleColor.AttributesToBackConsoleColor((ushort)info.wAttributes);
            }
            set
            {
                ConsoleNative.GetConsoleScreenBufferInfo(_hstdout, out CONSOLE_SCREEN_BUFFER_INFO info);
                var currentAttributes = (ushort)info.wAttributes;
                var backAttributes = TvConsoleColor.BackConsoleColorToAttribute(value);
                ConsoleNative.SetConsoleTextAttribute(_hstdout, (ushort)(backAttributes | TvConsoleColor.OnlyForegroundAttributes(currentAttributes)));
            }
        }

        public ConsoleColor ForegroundColor
        {
            get
            {
                ConsoleNative.GetConsoleScreenBufferInfo(_hstdout, out CONSOLE_SCREEN_BUFFER_INFO info);
                return TvConsoleColor.AttributesToForeConsoleColor((ushort)info.wAttributes);
            }

            set
            {
                ConsoleNative.GetConsoleScreenBufferInfo(_hstdout, out CONSOLE_SCREEN_BUFFER_INFO info);
                var currentAttributes = (ushort)info.wAttributes;
                var foreAttributes = TvConsoleColor.ForeConsoleColorToAttribute(value);
                ConsoleNative.SetConsoleTextAttribute(_hstdout, (ushort)(foreAttributes | TvConsoleColor.OnlyBackgroundAttributes(currentAttributes)));
            }
        }

        public TvConsoleStreamProperties OutProperties { get; }
        public TextWriter Out { get; }
        public Win32TvScreenBuffer(IntPtr handle, bool outputRedirected, bool disposable)
        {
            _hstdout = handle;
            _disposed = false;
            _isDisposable = disposable;
            OutProperties = new TvConsoleStreamProperties((int)ConsoleNative.GetConsoleOutputCP(), System.Console.OutputEncoding, outputRedirected);
            Out = new StreamWriter(new Win32TvConsoleStream(_hstdout, FileAccess.ReadWrite, OutProperties.UseFileApis), OutProperties.Encoding) { AutoFlush = true };
            Cursor = new Win32TvCursor(_hstdout);
        }
        public void WriteLine(string message)
        {
            Out.WriteLine(message);
        }

        public void WriteLine(string format, params object[] @params)
        {
            Out.WriteLine(string.Format(format, @params));
        }

        public void WriteLine() => Out.WriteLine();

        public void Write(string message) => Out.Write(message);

        public bool Activate()
        {
            var ok = ConsoleNative.SetConsoleActiveScreenBuffer(_hstdout);
            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposable)
            {
                throw new InvalidOperationException("Default buffer screen is NOT disposable. Use TvConsole.Default.Close() instead");
            }

            if (!_disposed)
            {
                if (disposing)
                {
                    Out?.Dispose();
                }

                if (_hstdout != IntPtr.Zero)
                {
                    HandleNative.CloseHandle(_hstdout);
                    _hstdout = IntPtr.Zero;
                }
                _disposed = true;
            }
        }

        ~Win32TvScreenBuffer()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            if (_isDisposable)
            {
                Dispose(false);
            }
        }

        public void Dispose()
        {
            if (!_isDisposable) { return; }

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Close() => Dispose();

        public void Cls()
        {
            var info = CONSOLE_SCREEN_BUFFER_INFO_EX.New();
            ConsoleNative.GetConsoleScreenBufferInfoEx(_hstdout, ref info);
            COORD curPos = new COORD(0, 0);
            var numChars = info.dwSize.X * info.dwSize.Y;
            ConsoleNative.FillConsoleOutputCharacter(_hstdout, ' ', (uint)numChars, curPos, out var numWritten);
            ConsoleNative.FillConsoleOutputAttribute(_hstdout, (ushort)info.wAttributes, (uint)numChars, curPos, out var attrsWritten);
            ConsoleNative.SetConsoleCursorPosition(_hstdout, curPos);
        }

        public void WriteCharacterAt(int x, int y, char character, int count = 1)
        {
            ConsoleNative.FillConsoleOutputCharacter(_hstdout, character, (uint)count, new COORD((short)x,(short)y), out var numWritten);
        }

        public void WriteCharacterAt(int x, int y, char character, ConsoleColor foreColor, ConsoleColor backColor, int count = 1)
        {
            var coord = new COORD((short)x, (short)y);

            var attribute = (ushort)(TvConsoleColor.ForeConsoleColorToAttribute(foreColor) | TvConsoleColor.BackConsoleColorToAttribute(backColor));

            ConsoleNative.FillConsoleOutputAttribute(_hstdout, attribute, (uint)count, coord, out var numAttrWritten);
            ConsoleNative.FillConsoleOutputCharacter(_hstdout, character, (uint)count, coord, out var numWritten);
        }
    }
}
