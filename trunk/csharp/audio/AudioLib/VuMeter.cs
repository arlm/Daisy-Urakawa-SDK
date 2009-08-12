using System;

namespace AudioLib
{
    // This class underwent a major cleanup and simplification at revision 1486.
    // See:
    // http://daisy.trac.cvsdude.com/urakawa-sdk/changeset/1486#file6
    // Just in case we need to restore some functionality:
    // http://daisy.trac.cvsdude.com/urakawa-sdk/browser/trunk/csharp/audio/AudioLib/VuMeter.cs?rev=1485
    public class VuMeter
    {
        public event Events.VuMeter.PeakOverloadHandler PeakOverload;
        public event Events.VuMeter.UpdatePeakMeterHandler UpdatePeakMeter;

        private AudioPlayer mPlayer;
        private AudioRecorder mRecorder;

        private byte[] m_arUpdatedVM;

        public VuMeter(AudioPlayer player, AudioRecorder recorder)
        {
            mPlayer = player;
            mRecorder = recorder;

            mPlayer.UpdateVuMeter += new Events.Player.UpdateVuMeterHandler(OnPlayerUpdateVuMeter);
            mRecorder.UpdateVuMeter += new Events.Recorder.UpdateVuMeterHandler(OnRecorderUpdateVuMeter);
        }

        public void OnPlayerUpdateVuMeter(object sender, Events.Player.UpdateVuMeterEventArgs Update)
        {
            AudioPlayer ob_AudioPlayer = sender as AudioPlayer;
            if (ob_AudioPlayer == null || ob_AudioPlayer != mPlayer)
            {
                return;
            }

            if (m_arUpdatedVM == null || m_arUpdatedVM.Length != mPlayer.arUpdateVM.Length)
            {
                Console.WriteLine("*** creating mPlayer buffer");
                m_arUpdatedVM = new byte[mPlayer.arUpdateVM.Length];
            }
            Array.Copy(mPlayer.arUpdateVM, m_arUpdatedVM, mPlayer.arUpdateVM.Length);

            ComputePeakDbValue(mPlayer.CurrentAudioPCMFormat);

            int index = 0;
            foreach (double peak in m_PeakDbValue)
            {
                index++;
                if (peak < 0)
                {
                    continue;
                }
                if (PeakOverload != null)
                {
                    PeakOverload(this,
                          new Events.VuMeter.PeakOverloadEventArgs(index, mPlayer.CurrentTimePosition));
                }
            }
        }

        public void OnRecorderUpdateVuMeter(object sender, Events.Recorder.UpdateVuMeterEventArgs UpdateVuMeter)
        {
            AudioRecorder ob_AudioRecorder = sender as AudioRecorder;
            if (ob_AudioRecorder == null || ob_AudioRecorder != mRecorder)
            {
                return;
            }

            if (m_arUpdatedVM == null || m_arUpdatedVM.Length != mRecorder.arUpdateVM.Length)
            {
                Console.WriteLine("*** creating mRecorder buffer");
                m_arUpdatedVM = new byte[mRecorder.arUpdateVM.Length];
            }
            Array.Copy(mRecorder.arUpdateVM, m_arUpdatedVM, mRecorder.arUpdateVM.Length);

            ComputePeakDbValue(mRecorder.RecordingPCMFormat);

            int index = 0;
            foreach (double peak in m_PeakDbValue)
            {
                index++;
                if (peak < 0)
                {
                    continue;
                }
                if (PeakOverload != null)
                {
                    PeakOverload(this,
                        new Events.VuMeter.PeakOverloadEventArgs(index, mRecorder.TimeOfAsset));
                }
            }
        }

        private double[] m_PeakDbValue;
        private void ComputePeakDbValue(AudioLibPCMFormat pcmFormat)
        {
            if (m_PeakDbValue == null || m_PeakDbValue.Length != pcmFormat.NumberOfChannels)
            {
                Console.WriteLine("*** creating PeakDbValue buffer");
                m_PeakDbValue = new double[pcmFormat.NumberOfChannels];
            }

            double full = Math.Pow(2, pcmFormat.BitDepth);
            double halfFull = full / 2;

            int bytesPerSample = pcmFormat.BitDepth / 8;

            for (int byteOffsetOfFrame = 0; byteOffsetOfFrame < m_arUpdatedVM.Length; byteOffsetOfFrame += pcmFormat.BlockAlign)
            {
                for (int channelIndex = 0; channelIndex < pcmFormat.NumberOfChannels; channelIndex++)
                {
                    double val = 0;
                    for (int byteOffsetInSample = 0; byteOffsetInSample < bytesPerSample; byteOffsetInSample++)
                    {
                        int arrayIndex = byteOffsetOfFrame + (channelIndex*bytesPerSample) + byteOffsetInSample;
                        val += Math.Pow(2, 8 * byteOffsetInSample)
                            * m_arUpdatedVM[arrayIndex];
                    }

                    if (val > halfFull)
                    {
                        val = full - val;
                    }

                    if (val > m_PeakDbValue[channelIndex])
                    {
                        m_PeakDbValue[channelIndex] = val;
                    }
                }
            }

            for (int channelIndex = 0; channelIndex < pcmFormat.NumberOfChannels; channelIndex++)
            {
                m_PeakDbValue[channelIndex] = 20 * Math.Log10(m_PeakDbValue[channelIndex] / halfFull);
            }

            if (UpdatePeakMeter != null)
                UpdatePeakMeter(this, new Events.VuMeter.UpdatePeakMeter(m_PeakDbValue));
        }
    }
}
