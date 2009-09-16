/***************************************************************************
 * This code taken from a provided sample in the DirectShowLib.Net project *
 ***************************************************************************/

using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using DirectShowLib;
using DirectShowLib.DES;

namespace OMLEngine
{
    public class MediaInfo
    {
        private string fileName;
        private Guid audioSubType;
        private int channels;
        private float samplesPerSec;
        private int bitsPerSample;
        private TimeSpan audioLength;
        private Guid videoSubType;
        private Size resolution;
        private int bitsPerPixel;
        private string fourCC;
        private TimeSpan videoLength;

        public MediaInfo()
        {
        }

        public MediaInfo(string strFilename)
        {
            if (File.Exists(strFilename))
            {
                int hr = 0;
                IMediaDet mediaDet = null;

                try
                {
                    mediaDet = (IMediaDet)new MediaDet();

                    hr = mediaDet.put_Filename(strFilename);
                    if (hr > 0)
                    {
                        DsError.ThrowExceptionForHR(hr);
                    }

                    this.fileName = strFilename;

                    int streamCount;
                    hr = mediaDet.get_OutputStreams(out streamCount);
                    DsError.ThrowExceptionForHR(hr);

                    for (int i = 0; i < streamCount; i++)
                    {
                        hr = mediaDet.put_CurrentStream(i);
                        DsError.ThrowExceptionForHR(hr);

                        Guid streamType;
                        hr = mediaDet.get_StreamType(out streamType);
                        DsError.ThrowExceptionForHR(hr);

                        if (streamType == MediaType.Audio)
                            UpdateAudioPart(mediaDet);
                        else if (streamType == MediaType.Video)
                            UpdateVideoPart(mediaDet);
                        else
                            continue;
                    }
                }
                finally
                {
                    if (mediaDet != null)
                        Marshal.ReleaseComObject(mediaDet);
                }
            }
        }

        private void UpdateAudioPart(IMediaDet mediaDet)
        {
            int hr = 0;
            AMMediaType mediaType = new AMMediaType();

            hr = mediaDet.get_StreamMediaType(mediaType);
            DsError.ThrowExceptionForHR(hr);

            this.audioSubType = mediaType.subType;

            double streamLength;
            hr = mediaDet.get_StreamLength(out streamLength);
            DsError.ThrowExceptionForHR(hr);

            this.audioLength = TimeSpan.FromSeconds(streamLength);

            if (mediaType.formatType == FormatType.WaveEx)
            {
                WaveFormatEx waveFormatEx = (WaveFormatEx)Marshal.PtrToStructure(mediaType.formatPtr, typeof(WaveFormatEx));
                channels = waveFormatEx.nChannels;
                samplesPerSec = ((float)waveFormatEx.nSamplesPerSec) / 1000;
                bitsPerSample = waveFormatEx.wBitsPerSample;
            }
        }

        public void UpdateVideoPart(IMediaDet mediaDet)
        {
            int hr = 0;
            AMMediaType mediaType = new AMMediaType();

            hr = mediaDet.get_StreamMediaType(mediaType);
            DsError.ThrowExceptionForHR(hr);

            videoSubType = mediaType.subType;

            double streamLength;
            hr = mediaDet.get_StreamLength(out streamLength);
            DsError.ThrowExceptionForHR(hr);

            videoLength = TimeSpan.FromSeconds(streamLength);

            if (mediaType.formatType == FormatType.VideoInfo)
            {
                VideoInfoHeader videoHeader = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.formatPtr, typeof(VideoInfoHeader));

                resolution = new Size(videoHeader.BmiHeader.Width, videoHeader.BmiHeader.Height);
                bitsPerPixel = videoHeader.BmiHeader.BitCount;
                fourCC = FourCCToString(videoHeader.BmiHeader.Compression);
            }
        }

        public string FourCCToString(int fourcc)
        {
            byte[] bytes = new byte[4];

            bytes[0] = (byte)(fourcc & 0x000000ff); fourcc = fourcc >> 8;
            bytes[1] = (byte)(fourcc & 0x000000ff); fourcc = fourcc >> 8;
            bytes[2] = (byte)(fourcc & 0x000000ff); fourcc = fourcc >> 8;
            bytes[3] = (byte)(fourcc & 0x000000ff);

            return Encoding.ASCII.GetString(bytes);
        }
    }
}
