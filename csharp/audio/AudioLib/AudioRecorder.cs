using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX.DirectSound;
using System.Threading;

namespace AudioLib
{
    public class AudioRecorder
    {
        private const int NOTIFICATIONS = 16;

        private byte[] m_PcmDataBuffer;

        private CaptureBuffer m_CircularBuffer;
        private int m_CircularBufferReadPositon;
        private AutoResetEvent m_CircularBufferNotificationEvent;

        private Thread m_CircularBufferRefreshThread;

        private Notify m_Notify;

        private long m_TotalRecordedBytes;


        public AudioRecorder()
        {
            CurrentState = State.NotReady;

            //m_CircularBufferNotificationEventCheckTimer.Enabled = false;
            //m_CircularBufferNotificationEventCheckTimer.Interval = 200;
            //m_CircularBufferNotificationEventCheckTimer.Tick += new EventHandler(onCircularBufferNotificationEventCheckTimerTick);
        }

        /// <summary>
        /// The three states of the audio recorder.
        /// NotReady: the recorder is not ready to record, for whatever reason.
        /// Stopped: the recorder is stopped and ready to record or monitor.
        /// Monitoring: the recording is listening but not writing any data.
        /// Recording: sound is currently being recorded.
        /// </summary>
        public enum State { NotReady, Stopped, Monitoring, Recording };

        private State m_PreviousState;
        private State m_State;
        public State CurrentState
        {
            get
            {
                return m_State;
            }
            private set
            {
                if (m_State == value)
                {
                    return;
                }

                m_PreviousState = m_State;
                m_State = value;

                if (StateChanged != null)
                    StateChanged(this, new StateChangedEventArgs(m_PreviousState));
            }
        }

        public event StateChangedHandler StateChanged;
        public delegate void StateChangedHandler(object sender, StateChangedEventArgs e);
        public class StateChangedEventArgs : EventArgs
        {
            private State m_OldState;
            public State OldState
            {
                get
                {
                    return m_OldState;
                }
            }

            public StateChangedEventArgs(State oldState)
            {
                m_OldState = oldState;
            }
        }

        private string m_RecordedFilePath;

        public event AudioRecordingFinishHandler AudioRecordingFinished;
        public delegate void AudioRecordingFinishHandler(object sender, AudioRecordingFinishEventArgs e);
        public class AudioRecordingFinishEventArgs : EventArgs
        {
            private string m_RecordedFilePath;
            public string RecordedFilePath
            {
                get { return m_RecordedFilePath; }
            }

            public AudioRecordingFinishEventArgs(string recordedFilePath)
            {
                m_RecordedFilePath = recordedFilePath;
            }
        }

        private readonly PcmDataBufferAvailableEventArgs m_PcmDataBufferAvailableEventArgs = new PcmDataBufferAvailableEventArgs(new byte[] { 0, 0, 0, 0 });
        public event PcmDataBufferAvailableHandler PcmDataBufferAvailable;
        public delegate void PcmDataBufferAvailableHandler(object sender, PcmDataBufferAvailableEventArgs e);
        public class PcmDataBufferAvailableEventArgs : EventArgs
        {
            private byte[] m_PcmDataBuffer;
            public byte[] PcmDataBuffer
            {
                get
                {
                    return m_PcmDataBuffer;
                }
                set
                {
                    m_PcmDataBuffer = value;
                }
            }

            public PcmDataBufferAvailableEventArgs(byte[] pcmDataBuffer)
            {
                m_PcmDataBuffer = pcmDataBuffer;
            }
        }


        public void SetDevice(string name)
        {
            List<InputDevice> devices = InputDevices;
            InputDevice found = devices.Find(delegate(InputDevice d) { return d.Name == name; });
            if (found != null)
            {
                InputDevice = found;
            }
            else if (devices.Count > 0)
            {
                InputDevice = devices[0]; //devices.Count-1
            }
            else
            {
                throw new Exception("No input device available.");
            }
        }

        InputDevice m_InputDevice;
        public InputDevice InputDevice
        {
            get { return m_InputDevice; }
            set
            {
                m_InputDevice = value;

                CurrentState = State.Stopped;
            }
        }

        private List<InputDevice> m_InputDevices;
        public List<InputDevice> InputDevices
        {
            get
            {
                if (m_InputDevices != null)
                {
                    return m_InputDevices;
                }

                CaptureDevicesCollection devices = new CaptureDevicesCollection();
                m_InputDevices = new List<InputDevice>(devices.Count);
                foreach (DeviceInformation info in devices)
                {
                    m_InputDevices.Add(new InputDevice(info));
                }

                return m_InputDevices;
            }
        }


        private AudioLibPCMFormat m_RecordingPCMFormat;
        public AudioLibPCMFormat RecordingPCMFormat
        {
            get { return m_RecordingPCMFormat; }
            private set { m_RecordingPCMFormat = value; }
        }

        private ulong m_RecordedFileRiffHeaderSize;


        private string m_RecordingDirectory;
        public string RecordingDirectory
        {
            get
            {
                return m_RecordingDirectory;
            }
            set
            {
                m_RecordingDirectory = value;
                if (!Directory.Exists(m_RecordingDirectory))
                {
                    Directory.CreateDirectory(m_RecordingDirectory);
                }
            }
        }

        public double CurrentDuration
        {
            get
            {
                if (CurrentState == State.NotReady)
                {
                    return 0;
                }

                if (CurrentState != State.Monitoring && CurrentState != State.Recording)
                {
                    return 0;
                }

                return RecordingPCMFormat.ConvertBytesToTime(m_TotalRecordedBytes);
            }
        }

        public void StartMonitoring(AudioLibPCMFormat pcmFormat)
        {
            if (CurrentState == State.NotReady)
            {
                return;
            }

            if (CurrentState != State.Stopped)
            {
                return;
            }

            startRecordingOrMonitoring(pcmFormat, false);
        }


        public void StartRecording(AudioLibPCMFormat pcmFormat)
        {
            if (CurrentState == State.NotReady)
            {
                return;
            }

            if (CurrentState != State.Stopped)
            {
                return;
            }

            startRecordingOrMonitoring(pcmFormat, true);
        }

        private const int REFRESH_INTERVAL_MS = 75; //ms interval for refreshing PCM data
        private void startRecordingOrMonitoring(AudioLibPCMFormat pcmFormat, bool recordingToFile)
        {
            RecordingPCMFormat = pcmFormat;

            WaveFormat waveFormat = new WaveFormat();
            waveFormat.FormatTag = WaveFormatTag.Pcm;
            waveFormat.Channels = (short)RecordingPCMFormat.NumberOfChannels;
            waveFormat.SamplesPerSecond = (int)RecordingPCMFormat.SampleRate;
            waveFormat.BitsPerSample = (short)RecordingPCMFormat.BitDepth;
            waveFormat.AverageBytesPerSecond = (int)RecordingPCMFormat.SampleRate * RecordingPCMFormat.BlockAlign;
            waveFormat.BlockAlign = (short)RecordingPCMFormat.BlockAlign;

            uint byteRate = RecordingPCMFormat.SampleRate * RecordingPCMFormat.BlockAlign;

            int pcmDataBufferSize = (int)(byteRate * REFRESH_INTERVAL_MS / 1000.0);
            pcmDataBufferSize -= pcmDataBufferSize % RecordingPCMFormat.BlockAlign;

            m_PcmDataBuffer = new byte[pcmDataBufferSize];

            int circularBufferSize = pcmDataBufferSize * NOTIFICATIONS;

            CaptureBufferDescription bufferDescription = new CaptureBufferDescription();
            bufferDescription.BufferBytes = circularBufferSize;
            bufferDescription.Format = waveFormat;

            m_CircularBuffer = new CaptureBuffer(bufferDescription, InputDevice.Capture);

            m_CircularBufferNotificationEvent = new AutoResetEvent(false);

            BufferPositionNotify[] m_BufferPositionNotify = new BufferPositionNotify[NOTIFICATIONS];
            for (int i = 0; i < NOTIFICATIONS; i++)
            {
                m_BufferPositionNotify[i].Offset = (pcmDataBufferSize * i) + pcmDataBufferSize - 1;
                m_BufferPositionNotify[i].EventNotifyHandle = m_CircularBufferNotificationEvent.SafeWaitHandle.DangerousGetHandle();
            }

            m_Notify = new Notify(m_CircularBuffer);
            m_Notify.SetNotificationPositions(m_BufferPositionNotify, NOTIFICATIONS);

            m_CircularBufferReadPositon = 0;
            m_RecordingFileWriter = null;
            m_TotalRecordedBytes = 0;

            if (recordingToFile)
            {
                int i = -1;
                do
                {
                    i++;
                    m_RecordedFilePath = RecordingDirectory + Path.DirectorySeparatorChar + i.ToString() + ".wav";

                } while (File.Exists(m_RecordedFilePath));

                Stream stream = File.Create(m_RecordedFilePath);
                try
                {
                    m_RecordedFileRiffHeaderSize = RecordingPCMFormat.RiffHeaderWrite(stream, 0);
                }
                finally
                {
                    stream.Close();
                }
            }

            m_CircularBufferRefreshThread = new Thread(new ThreadStart(circularBufferRefreshThreadMethod));
            m_CircularBufferRefreshThread.Name = "Recorder Notify Thread";
            m_CircularBufferRefreshThread.Priority = ThreadPriority.Highest;
            m_CircularBufferRefreshThread.IsBackground = true;


            CurrentState = (recordingToFile ? State.Recording : State.Monitoring);

            m_CircularBuffer.Start(true);

            m_CircularBufferRefreshThread.Start();


            Console.WriteLine("Recorder notify thread start.");

            //m_PreviousTotalRecordedBytes = 0;
            //m_CircularBufferNotificationEventCheckTimer.Start();
        }

        //private System.Windows.Forms.Timer m_CircularBufferNotificationEventCheckTimer = new System.Windows.Forms.Timer();
        //long m_PreviousTotalRecordedBytes;
        //private void onCircularBufferNotificationEventCheckTimerTick(object sender, EventArgs e)
        //{
        //    if (m_PreviousTotalRecordedBytes == m_TotalRecordedBytes
        //        && CurrentState == State.Recording)
        //    {
        //        m_CircularBufferNotificationEvent.WaitOne(1);
        //    }
        //    m_PreviousTotalRecordedBytes = m_TotalRecordedBytes;
        //}

        private BinaryWriter m_RecordingFileWriter;

        private readonly Object LOCK = new object();
        private void circularBufferRefreshThreadMethod()
        {
            while (true)
            {
                m_CircularBufferNotificationEvent.WaitOne(Timeout.Infinite, true);

                Monitor.Enter(LOCK);
                try
                {
                    if (m_CircularBuffer == null || !m_CircularBuffer.Capturing || CurrentState == State.Stopped)
                    {
                        Console.WriteLine("circularBufferRefreshThreadMethod EXIT while");
                        break;
                    }
                    else
                    {
                        circularBufferTransferData();
                    }
                }
                finally
                {
                    Monitor.Exit(LOCK);
                }
            }

            Console.WriteLine("Recorder notify thread exit.");
        }

        private int circularBufferTransferData()
        {
            int circularBufferCapturePosition;
            int readPosition;
            m_CircularBuffer.GetCurrentPosition(out circularBufferCapturePosition, out readPosition);


            int circularBufferBytesAvailableForReading = (circularBufferCapturePosition == m_CircularBufferReadPositon ? 0
                                    : (circularBufferCapturePosition < m_CircularBufferReadPositon
                              ? circularBufferCapturePosition + (m_CircularBuffer.Caps.BufferBytes - m_CircularBufferReadPositon)
                              : circularBufferCapturePosition - m_CircularBufferReadPositon));

            circularBufferBytesAvailableForReading -= (circularBufferBytesAvailableForReading % (m_CircularBuffer.Caps.BufferBytes / NOTIFICATIONS));

            int circularBufferBytesAvailableForCapturing = m_CircularBuffer.Caps.BufferBytes - circularBufferBytesAvailableForReading;

            if (circularBufferBytesAvailableForReading <= 0)
            {
                Console.WriteLine(
                    string.Format("circularBufferTransferData: no more bytes to fetch {0}", circularBufferBytesAvailableForReading));
                return circularBufferBytesAvailableForReading;
            }

            //int toRead = readPosition - m_CircularBufferReadPositon;
            //if (toRead < 0)
            //    toRead += m_CircularBuffer.Caps.BufferBytes;

            //toRead -= (toRead % (m_CircularBuffer.Caps.BufferBytes / NOTIFICATIONS));
            //if (toRead <= 0)
            //{
            //    Console.WriteLine(string.Format("BAD toRead {0}", toRead));
            //    continue;
            //}

            byte[] incomingPcmData =
                (byte[])
                m_CircularBuffer.Read(m_CircularBufferReadPositon, typeof(byte), LockFlag.None, circularBufferBytesAvailableForReading);

            /*
             * 
    if (m_CircularBuffer != null && m_CircularBuffer.Capturing)
    {
        int capturePosition;
        int readPosition;
        m_CircularBuffer.GetCurrentPosition(out capturePosition, out readPosition);

        return
            RecordingPCMFormat.ConvertBytesToTime(m_TotalRecordedBytes + capturePosition -
                                                    m_CircularBufferReadPositon);
    }
             */

            m_TotalRecordedBytes += incomingPcmData.Length;

            m_CircularBufferReadPositon += incomingPcmData.Length;
            m_CircularBufferReadPositon %= m_CircularBuffer.Caps.BufferBytes;

            if (CurrentState == State.Recording)
            {
                if (m_RecordingFileWriter == null)
                {
                    FileInfo fi = new FileInfo(m_RecordedFilePath);
                    m_RecordingFileWriter = new BinaryWriter(File.OpenWrite(fi.FullName));
                }

                m_RecordingFileWriter.BaseStream.Position = m_TotalRecordedBytes +
                                                            (long)m_RecordedFileRiffHeaderSize;
                // m_RecordingFileWriter.BaseStream.Length;
                m_RecordingFileWriter.Write(incomingPcmData, 0, incomingPcmData.Length);
            }


            if (m_PcmDataBuffer.Length != incomingPcmData.Length)
            {
                Console.WriteLine(
                    string.Format(">>>>> Resizing buffer: m_PcmDataBuffer = {0}, incomingPcmData = {1}",
                                  m_PcmDataBuffer.Length, incomingPcmData.Length));

                Array.Resize(ref m_PcmDataBuffer, incomingPcmData.Length);
            }

            Array.Copy(incomingPcmData, m_PcmDataBuffer, m_PcmDataBuffer.Length);

            if (PcmDataBufferAvailable != null)
            {
                m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
                PcmDataBufferAvailable(this, m_PcmDataBufferAvailableEventArgs);
            }

            return circularBufferBytesAvailableForReading;
        }

        public void StopRecording()
        {
            if (CurrentState == State.NotReady)
            {
                return;
            }

            if (CurrentState != State.Recording && CurrentState != State.Monitoring)
            {
                return;
            }

            bool wasRecording = CurrentState == State.Recording;

            Monitor.Enter(LOCK);
            try
            {
                m_CircularBuffer.Stop();
            }
            finally
            {
                Monitor.Exit(LOCK);
            }

            m_CircularBufferNotificationEvent.Set();
            m_CircularBufferNotificationEvent.Close();

            m_CircularBufferRefreshThread = null;

            int remainingBytesToRead = 0;
            do
            {
                remainingBytesToRead = circularBufferTransferData();
                Console.WriteLine(
                    string.Format("circularBufferTransferData: fetched remaining bytes: {0}", remainingBytesToRead));
            } while (remainingBytesToRead > 0);

            if (m_RecordingFileWriter != null)
            {
                m_RecordingFileWriter.Close();

                FileInfo fileInfo = new FileInfo(m_RecordedFilePath);
                Stream stream = File.OpenWrite(m_RecordedFilePath);
                try
                {
                    // overriding the existing RIFF header, this time with correct data length
                    m_RecordedFileRiffHeaderSize = RecordingPCMFormat.RiffHeaderWrite(stream,
                                                            (uint)
                                                            (fileInfo.Length -
                                                             (long)
                                                             m_RecordedFileRiffHeaderSize));
                }
                finally
                {
                    stream.Close();
                }

                if (fileInfo.Length == (long) m_RecordedFileRiffHeaderSize) // no PCM data, just RIFF header
                {
                    File.Delete(m_RecordedFilePath);
                    m_RecordedFilePath = null;
                }
            }

            m_Notify.Dispose();
            m_Notify = null;

            m_CircularBuffer.Dispose();
            m_CircularBuffer = null;

            CurrentState = State.Stopped;

            if (wasRecording && AudioRecordingFinished != null)
                AudioRecordingFinished(this, new AudioRecordingFinishEventArgs(m_RecordedFilePath));
        }
    }
}



//BinaryWriter Writer = new BinaryWriter(File.OpenWrite(RecordedFilePath));
//FileInfo RecordedFile = new FileInfo(RecordedFilePath);

//long Audiolength = RecordedFile.Length - 8;
//for (int i = 0; i < 4; i++)
//{
//    Writer.BaseStream.Position = i + 4;
//    Writer.Write(Convert.ToByte(CalculationFunctions.ConvertFromDecimal(Audiolength)[i]));
//}
//Audiolength = Audiolength - 36;
//for (int i = 0; i < 4; i++)
//{
//    Writer.BaseStream.Position = i + 40;
//    Writer.Write(Convert.ToByte(CalculationFunctions.ConvertFromDecimal(Audiolength)[i]));
//}
//Writer.Close();