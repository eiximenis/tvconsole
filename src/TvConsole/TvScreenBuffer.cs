using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TvConsole.Win32;

namespace TvConsole
{
    public class TvScreenBuffer : ISecondaryScreenBuffer, IDisposable
    {
        private IntPtr _hstdout;
        private bool _disposed;
        private readonly bool _isDisposable;

        public TvCursor Cursor { get; }

        public TvConsoleStreamProperties OutProperties { get; }
        public TextWriter Out { get; }
        public TvScreenBuffer(IntPtr handle, TvConsole owner, bool outputRedirected, bool forceFileApi, bool disposable)
        {
            _hstdout = handle;
            _disposed = false;
            _isDisposable = disposable;
            OutProperties = new TvConsoleStreamProperties((int)ConsoleNative.GetConsoleOutputCP(), System.Console.OutputEncoding, outputRedirected, forceFileApi);
            Out = new StreamWriter(new TvConsoleStream(_hstdout, FileAccess.ReadWrite, OutProperties.UseFileApis), OutProperties.Encoding) { AutoFlush = true };
            Cursor = new TvCursor(_hstdout);
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

        ~TvScreenBuffer()
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
            ConsoleNative.SetConsoleCursorPosition(_hstdout, curPos);
        }
    }
}
