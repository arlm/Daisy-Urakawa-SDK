using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Microsoft.DirectX.DirectSound;

namespace AudioEngine.DirectX9
{
	/// <summary>
	/// Implementation of <see cref="IPlaybackAudioDevice"/> using DirectX 9
	/// </summary>
	public class PlaybackAudioDevice : AudioDevice, IPlaybackAudioDevice
	{


		/// <summary>
		/// The length of the playback buffer in milliseconds
		/// </summary>
		public static long BUFFER_MS = 2000;

		/// <summary>
		/// The time to wait between updates while playback
		/// </summary>
		public static int WAIT_TIME_MS = 10;

		private Device mPlaybackDevice;
		
		/// <summary>
		/// Default constructor - uses the default playback <see cref="Device"/>
		/// </summary>
		public PlaybackAudioDevice(System.Windows.Forms.Control owner)
		{
			mPlaybackDevice = new Device();
			mPlaybackDevice.SetCooperativeLevel(owner, CooperativeLevel.Priority);
		}

		/// <summary>
		/// Destructor, abouts any running threads
		/// </summary>
		~PlaybackAudioDevice()
		{
			if (mPlaybackThread != null)
			{
				if (mPlaybackThread.IsAlive) mPlaybackThread.Abort();
			}
		}

		private struct OverloadPosition
		{
			public long Position;
			public ushort Channel;
		}

		private bool IsTrue(bool val) { return val == true; }

		private void UpdateTime(int newPlayCursor)
		{
			List<OverloadPosition> olPos = new List<OverloadPosition>();
			ushort bps = BytesPerSample;
			int bufferSamples = mBufferLength / bps;
			ushort noc = getNumberOfChannels();
			double[] maxDbs = new double[noc];
			for (int i = 0; i < noc; i++)
			{
				maxDbs[i] = 0;
			}
			if (getState() == AudioDeviceState.Playing)
			{
				int len = ((newPlayCursor - mPlayCursor) % mBufferLength) / bps;
				for (int i = 0; i < len; i += noc)
				{
					for (ushort c = 0; c < noc; c++)
					{
						int sampleIndex = ((mPlayCursor/bps) + i + c) % bufferSamples;
						if (maxDbs[c] < mAbsVals[sampleIndex])
						{
							maxDbs[c] = mAbsVals[sampleIndex];
						}
						if (mOverloads[sampleIndex])
						{
							if (mOverloads[(sampleIndex-1) % bufferSamples])
							{
								OverloadPosition p;
								p.Channel = c;
								p.Position = (mCycleCount * mBufferLength) + (sampleIndex * bps);
								olPos.Add(p);
							}
						}
					}
				}
			}
			if (newPlayCursor != mPlayCursor)
			{
				mPlayCursor = newPlayCursor;
				TimeSpan time = getTimeEquivalent(mPlayCursor + (mCycleCount * mBufferLength));
				for (int c = 0; c < noc; c++)
				{
					maxDbs[c] = calculateDbValue(maxDbs[c]);
				}
				FireTime(getTimeEquivalent(mPlayCursor + (mCycleCount * mBufferLength)), maxDbs);
				long[] prevOLPos = new long[noc];
				for (int c = 0; c < noc; c++)
				{
					prevOLPos[c] = Int64.MinValue;
				}
				foreach (OverloadPosition pos in olPos)
				{
					if (pos.Position - prevOLPos[pos.Channel] != bps * noc)
					{
						FireOverloadOccured(pos.Channel, getTimeEquivalent(pos.Position));
					}
					prevOLPos[pos.Channel] = pos.Position;
				}
			}

		}


		/// <summary>
		/// Kill the playback and update worker <see cref="Thread"/>s ensuring that no more events are raised
		/// </summary>
		public void killPlaybackWorker()
		{
			if (mPlaybackThread!=null)
			{
				if (mPlaybackThread.IsAlive) mPlaybackThread.Abort();
			}
		}

		#region IPlaybackAudioDevice Members

		/// <summary>
		/// Event fired when a playback session ends
		/// </summary>
		public event EventHandler<EndedEventArgs> PlayEnded;

		private void FirePlayEnded(TimeSpan playEnd)
		{
			if (PlayEnded != null) PlayEnded(this, new EndedEventArgs(playEnd, mPCMInputStream));
		}

		/// <summary>
		/// Plays a given pcm input <see cref="Stream"/>
		/// </summary>
		/// <param name="pcmInputStream">The pcm input <see cref="Stream"/></param>
		/// <remarks>Playback starts at the current position in the pcm input <see cref="Stream"/></remarks>
		public void play(Stream pcmInputStream)
		{
			play(pcmInputStream, new TimeSpan());
		}

		/// <summary>
		/// Plays a clip of a given pcm input <see cref="Stream"/>.
		/// </summary>
		/// <param name="pcmInputStream">The pcm input <see cref="Stream"/></param>
		/// <param name="clipBegin">
		/// The begin time of the clip to play, the end of the pcm input <see cref="Stream"/>
		/// determines the end of the clip
		/// </param>
		/// <remarks>Clip times are calculated from the current position of the pcm input <see cref="Stream"/></remarks>
		public void play(Stream pcmInputStream, TimeSpan clipBegin)
		{
			TimeSpan clipEnd = new TimeSpan(
				(TimeSpan.TicksPerSecond * (pcmInputStream.Length - pcmInputStream.Position)) / getByteRate());
			play(pcmInputStream, clipBegin, clipEnd);
		}

		/// <summary>
		/// Plays a clip of a given pcm input <see cref="Stream"/>.
		/// </summary>
		/// <param name="pcmInputStream">The pcm input <see cref="Stream"/></param>
		/// <param name="clipBegin">The begin time of the clip to play</param>
		/// <param name="clipEnd">The end time of the clip to play</param>
		/// <remarks>Clip times are calculated from the current position of the pcm input <see cref="Stream"/></remarks>
		public virtual void play(Stream pcmInputStream, TimeSpan clipBegin, TimeSpan clipEnd)
		{
			if (getState() != AudioDeviceState.Stopped)
			{
				throw new ApplicationException("Can only play from Stopped state");
			}
			mClipBegin = clipBegin;
			mClipEnd = clipEnd;
			mPCMInputStream = pcmInputStream;
			mIsPlaying = true;
			InitializeBuffer();
			ThreadStart ts = new ThreadStart(PlaybackWorker);
			mPlaybackThread = new Thread(ts);
			mPlaybackThread.Start();
		}


		private long mOriginPos;
		private TimeSpan mClipBegin, mClipEnd;
		private Stream mPCMInputStream;
		private bool mIsPlaying;
		private int mBufferLength;
		private int mCycleCount;
		private long mEndOffset;
		private SecondaryBuffer mBuffer;
		private double[] mAbsVals;
		private bool[] mOverloads;
		private int mPlayCursor;

		private Thread mPlaybackThread;

		private void InitializeBuffer()
		{
			mOriginPos = mPCMInputStream.Position;
			long startOffset = getPositionEquivalent(mClipBegin);
			startOffset -= startOffset % getBlockAlign();
			mEndOffset = getPositionEquivalent(mClipEnd);
			mEndOffset += mEndOffset % getBlockAlign();
			if (startOffset + mPCMInputStream.Position > mPCMInputStream.Length)
			{
				throw new ApplicationException("Start position is beyond the end of the PCM input stream");
			}
			if (mEndOffset + mPCMInputStream.Position > mPCMInputStream.Length)
			{
				mEndOffset = mPCMInputStream.Length;
			}

			//Seeks to the clip start position in the PCM input Stream
			mPCMInputStream.Seek(startOffset, SeekOrigin.Current);
			BufferDescription bd = new BufferDescription(getWaveFormat());

			//Create a SecondaryBuffer of 2s length
			mBufferLength = (int)getPositionEquivalent(new TimeSpan(BUFFER_MS*TimeSpan.TicksPerMillisecond));
			bd.BufferBytes = mBufferLength;
			//TODO: Optimize buffer and write lengths
			bd.Flags =
				BufferDescriptionFlags.CanGetCurrentPosition
				| BufferDescriptionFlags.ControlVolume
				| BufferDescriptionFlags.LocateInSoftware
				| BufferDescriptionFlags.GlobalFocus
				| BufferDescriptionFlags.ControlFrequency;
			mBuffer = new SecondaryBuffer(bd, mPlaybackDevice);
			mBuffer.Frequency = (int)(getPlaybackSpeed() * getSampleRate());
			mAbsVals = new double[mBufferLength/BytesPerSample];
			mOverloads = new bool[mBufferLength/BytesPerSample];
			for (int i = 0; i < mAbsVals.Length; i++)
			{
				mAbsVals[i] = 0;
				mOverloads[i] = false;
			}
			//mBuffer.PlayPosition = 0;
			mCycleCount = 0;
			mPlayCursor = 0;
		}

		private void WriteToBuffer(byte[] pcmData, int startCursor, long sourcePos)
		{
			mBuffer.Write(startCursor, pcmData, LockFlag.None);
			int bps = BytesPerSample;
			int pcmSamples = pcmData.Length / bps;
			int bufferSamples = mBufferLength / bps;
			int sampleStartCursor = startCursor / bps;
			int sampleIndex;
			double absVal;
			bool overload;
			for (int i = 0; i < pcmSamples; i++)
			{
				sampleIndex = (sampleStartCursor + i) % bufferSamples;

				calculateSampleValAndMaxMin(pcmData, i * bps, out absVal, out overload);
				mAbsVals[sampleIndex] = absVal;
				mOverloads[sampleIndex] = overload;
			}
		}



		private void PlaybackWorker()
		{
			TimeSpan endTime = TimeSpan.Zero;
			int writeLength = mBufferLength / 4;//Write aprox. 0.5s at a time
			int latestWritePos = 0;
			int latestPlayCursor = 0;
			bool playbackInitiated = false;
			byte[] buf = new byte[writeLength];
			try
			{
				while (mIsPlaying)
				{
					int pp = mBuffer.PlayPosition;
					UpdateTime(pp);
					if (pp < latestPlayCursor) mCycleCount++;
					latestPlayCursor = pp;
					int curPlayPos = pp + (mCycleCount * mBufferLength);
					TimeSpan curPlayTime = getTimeEquivalent(curPlayPos);
					if (mClipBegin + curPlayTime > mClipEnd)
					{
						endTime = curPlayTime;
						mBuffer.Stop();
						setState(AudioDeviceState.Stopped);
						mIsPlaying = false;
						break;
					}
					if (latestWritePos + writeLength < curPlayPos + mBufferLength)
					{
						long sourcePos = mPCMInputStream.Position;
						if (mPCMInputStream.Position + writeLength < mOriginPos + mEndOffset)
						{
							if (mPCMInputStream.Read(buf, 0, buf.Length) != buf.Length)
							{
								throw new ApplicationException("Not enough data in PCM Input Stream");
							}
							WriteToBuffer(buf, latestWritePos % mBufferLength, sourcePos);
						}
						else
						{
							int restWriteLength = 0;
							
							if (mPCMInputStream.Position < mOriginPos + mEndOffset - 1)
							{
								restWriteLength = (int)(mOriginPos + mEndOffset - mPCMInputStream.Position - 1);
							}
							if (mPCMInputStream.Read(buf, 0, restWriteLength) != restWriteLength)
							{
								throw new ApplicationException("Not enough data in PCM Input Stream");
							}
							Array.Clear(buf, restWriteLength, buf.Length - restWriteLength);
							WriteToBuffer(buf, latestWritePos % mBufferLength, sourcePos);
						}
						latestWritePos += writeLength;
					}
					if (!playbackInitiated)
					{
						mBuffer.Play(0, BufferPlayFlags.Looping);
						setState(AudioDeviceState.Playing);
						playbackInitiated = true;
					}
					Thread.Sleep(WAIT_TIME_MS);
				}
				//UpdateTime(latestPlayCursor);
				mBuffer.Dispose();
				FirePlayEnded(endTime);
			}
			catch (ThreadAbortException)
			{
				if (mBuffer != null)
				{
					if (!mBuffer.Disposed)
					{
						UpdateTime(0);
						mBuffer.Dispose();
						mIsPlaying = false;
						FirePlayEnded(endTime);
					}
				}
			}
		}

		/// <summary>
		/// Gets the current time during playback and recording
		/// </summary>
		/// <returns>The current time - if stopped, <see cref="TimeSpan.Zero"/> is returned</returns>
		public override TimeSpan getCurrentTime()
		{
			switch (getState())
			{
				case AudioDeviceState.Playing:
				case AudioDeviceState.PausedPlay:
					return getTimeEquivalent(((long)mBuffer.PlayPosition)+(long)(mCycleCount*mBufferLength));
				default:
					return TimeSpan.Zero;
			}
		}

		/// <summary>
		/// Stops playback
		/// </summary>
		public void stopPlayback()
		{
			if (mIsPlaying)
			{
				mBuffer.Stop();
				mIsPlaying = false;
				setState(AudioDeviceState.Stopped);
				mPCMInputStream = null;
			}
		}

		/// <summary>
		/// Pauses playback - the <see cref="AudioDeviceState"/> must be <see cref="AudioDeviceState.Playing"/>
		/// </summary>
		public void pausePlayback()
		{
			if (getState() != AudioDeviceState.Playing)
			{
				throw new ApplicationException("Can only pause playback while playing");
			}
			mBuffer.Stop();
			setState(AudioDeviceState.PausedPlay);
		}

		/// <summary>
		/// Resumes playback after playing has been paused
		/// </summary>
		public void resumePlayback()
		{
			if (getState() != AudioDeviceState.PausedPlay)
			{
				throw new ApplicationException("Can not resume playback when not paused");
			}
			mBuffer.Play(0, BufferPlayFlags.Looping);
			setState(AudioDeviceState.Playing);
		}

		/// <summary>
		/// Stores the current playback speed (ex. 0.5 for half speed, 1 for normal speed and 2 for double speed)
		/// </summary>
		protected double mPlaybackSpeed = 1;

		/// <summary>
		/// Sets the playback speed of the <see cref="PlaybackAudioDevice"/>
		/// </summary>
		/// <param name="newSpeed">The new speed must be in the interval
		/// <c>[<see cref="getMinPlaybackSpeed"/>();<see cref="getMaxPlaybackSpeed"/>()]</c></param>
		public virtual void setPlaybackSpeed(double newSpeed)
		{
			if (newSpeed < getMinPlaybackSpeed() || getMaxPlaybackSpeed() < newSpeed)
			{
				throw new ApplicationException("The new playback speed is out of the currently supported range");
			}
			mPlaybackSpeed = newSpeed;
			if (mBuffer != null)
			{
				mBuffer.Frequency = (int)(mPlaybackSpeed * getSampleRate());
			}
		}

		/// <summary>
		/// Gets the current playback speed
		/// </summary>
		/// <returns>The current playback speed</returns>
		public virtual double getPlaybackSpeed()
		{
			if (mPlaybackSpeed < getMinPlaybackSpeed()) mPlaybackSpeed = getMinPlaybackSpeed();
			if (mPlaybackSpeed > getMaxPlaybackSpeed()) mPlaybackSpeed = getMaxPlaybackSpeed();
			return mPlaybackSpeed;
		}

		/// <summary>
		/// Gets the minimal playback speed supported by the <see cref="IPlaybackAudioDevice"/>. 
		/// May vary with sample rate or other WAVE PCM parameters.
		/// </summary>
		/// <returns>The minimal supported playback speed (in ]0;1])</returns>
		public virtual double getMinPlaybackSpeed()
		{
			return ((double)mPlaybackDevice.Caps.MinSecondarySampleRate) / ((double)getSampleRate());
		}

		/// <summary>
		/// Get maximal playback speed speed supported by the <see cref="IPlaybackAudioDevice"/>
		/// May vary with sample rate or other WAVE PCM parameters.
		/// </summary>
		/// <returns>The minimal supported playback speed </returns>
		public virtual double getMaxPlaybackSpeed()
		{
			return ((double)mPlaybackDevice.Caps.MaxSecondarySampleRate) / ((double)getSampleRate());
		}

		#endregion
	}
}