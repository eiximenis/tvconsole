using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TvConsole.Win32;

namespace TvConsole
{
    public class TvConsoleStream : Stream
    {
        private readonly IntPtr _handle;
        private readonly bool _useFileApi;

        public override bool CanRead { get; }
        public override bool CanSeek => false;
        public override bool CanWrite { get; }

        public TvConsoleStream(IntPtr handle, FileAccess access, bool useFileApi)
        {
            CanRead = ((access & FileAccess.Read) == FileAccess.Read);
            CanWrite = ((access & FileAccess.Write) == FileAccess.Write);
            _handle = handle;
            _useFileApi = useFileApi;
        }


        public override long Length => throw new InvalidOperationException("Can't seek a console");

        public override long Position
        {
            get => throw new InvalidOperationException("Can't seek a console");
            set => throw new InvalidOperationException("Can't seek a console");
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException("Can't seek a console");
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException("Can't seek a console");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {

            var toRead = buffer;

            // TODO: Review if unsafe pointer could make this more efficient
            if (offset != 0)
            {
                toRead = buffer.Skip(offset).ToArray();
            }

            if (_useFileApi)
            {
                FileNative.ReadFile(_handle, toRead, (uint)count, out uint bytesRead, IntPtr.Zero);
                return (int)bytesRead;
            }
            else
            {
                ConsoleNative.ReadConsole(_handle, toRead, (uint)(count / sizeof(char)), out IntPtr charsRead, IntPtr.Zero);
                return charsRead.ToInt32() * sizeof(char);
            }


            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var toWrite = buffer;

            // TODO: Review if unsafe pointer could make this more efficient
            if (offset != 0)
            {
                toWrite = buffer.Skip(offset).ToArray();
            }
            if (_useFileApi)
            {
                FileNative.WriteFile(_handle, toWrite, (uint)count, out uint written, IntPtr.Zero);
            }
            else
            {
                ConsoleNative.WriteConsole(_handle, toWrite, (uint)(count / sizeof(char)), out uint charsWritten, IntPtr.Zero);
            }
        }
    }
}
