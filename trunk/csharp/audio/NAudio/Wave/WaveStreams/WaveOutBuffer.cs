using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NAudio.Wave 
{
    /// <summary>
    /// A buffer of Wave samples for streaming to a Wave Output device
    /// </summary>
    class WaveOutBuffer : IDisposable
    {
        private WaveHeader header;
        private Int32 bufferSize; // allocated bytes, may not be the same as bytes read
        private WaveBuffer buffer;
        private GCHandle hBuffer;
        private IntPtr hWaveOut;
        private GCHandle hHeader; // we need to pin the header structure
        private GCHandle hThis; // for the user callback
        private IWaveProvider waveStream;
        private object waveOutLock;

        /// <summary>
        /// creates a new wavebuffer
        /// </summary>
        /// <param name="hWaveOut">WaveOut device to write to</param>
        /// <param name="bufferSize">Buffer size in bytes</param>
        /// <param name="bufferFillStream">Stream to provide more data</param>
        /// <param name="waveOutLock">Lock to protect WaveOut API's from being called on >1 thread</param>
        public WaveOutBuffer(IntPtr hWaveOut, Int32 bufferSize, IWaveProvider bufferFillStream, object waveOutLock)
        {
            this.bufferSize = bufferSize;
            this.buffer = new WaveBuffer(bufferSize);
            this.hBuffer = GCHandle.Alloc(buffer.ByteBuffer, GCHandleType.Pinned);
            this.hWaveOut = hWaveOut;
            this.waveStream = bufferFillStream;
            this.waveOutLock = waveOutLock;

            header = new WaveHeader();
            hHeader = GCHandle.Alloc(header);
            header.dataBuffer = hBuffer.AddrOfPinnedObject();
            header.bufferLength = bufferSize;
            header.loops = 1;
            hThis = GCHandle.Alloc(this);
            header.userData = (IntPtr)hThis;
            lock (waveOutLock)
            {
                MmException.Try(WaveInterop.waveOutPrepareHeader(hWaveOut, header, Marshal.SizeOf(header)), "waveOutPrepareHeader");
            }
        }

        #region Dispose Pattern

        /// <summary>
        /// Finalizer for this wave buffer
        /// </summary>
        ~WaveOutBuffer()
        {
            Dispose(false);
            System.Diagnostics.Debug.Assert(true, "WaveBuffer was not disposed");
        }

        /// <summary>
        /// Releases resources held by this WaveBuffer
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// Releases resources held by this WaveBuffer
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
            // free unmanaged resources
            if (hHeader.IsAllocated)
                hHeader.Free();
            if (hBuffer.IsAllocated)
                hBuffer.Free();
            if (hThis.IsAllocated)
                hThis.Free();
            if (hWaveOut != IntPtr.Zero)
            {
                lock (waveOutLock)
                {
                    WaveInterop.waveOutUnprepareHeader(hWaveOut, header, Marshal.SizeOf(header));
                }
                hWaveOut = IntPtr.Zero;
            }
        }

        #endregion

        /// this is called by the WAVE callback and should be used to refill the buffer
        internal bool OnDone()
        {
            int bytes;
            lock (waveStream)
            {
                buffer.ByteBufferCount = buffer.ByteBuffer.Length;
                bytes = waveStream.Read(buffer);
            }
            if (bytes == 0)
            {
                return false;
            }
            else
            {
                for (int n = bytes; n < buffer.ByteBufferCount; n++)
                {
                    buffer.ByteBuffer[n] = 0;
                }
            }
            WriteToWaveOut();
            return true;
        }

        /// <summary>
        /// Whether the header's in queue flag is set
        /// </summary>
        public bool InQueue
        {
            get
            {
                return (header.flags & WaveHeaderFlags.InQueue) == WaveHeaderFlags.InQueue;
            }
        }

        /// <summary>
        /// The buffer size in bytes
        /// </summary>
        public Int32 BufferSize
        {
            get
            {
                return bufferSize;
            }
        }

        private void WriteToWaveOut()
        {
            MmResult result;

            lock (waveOutLock)
            {
                result = WaveInterop.waveOutWrite(hWaveOut, header, Marshal.SizeOf(header));
            }
            if (result != MmResult.NoError)
            {
                throw new MmException(result, "waveOutWrite");
            }

            GC.KeepAlive(this);
        }

    }
}
