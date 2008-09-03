using System;
using System.Globalization;
using DirectShowLib;
using DirectShowLib.Dvd;

namespace DVDMediaInfo
{
    public class DVDInfo
    {
        IDvdInfo2 dvdInfo;
        string dirName;
        int hr = 0;

        public DVDInfo(string directoryPath)
        {
            DVDNavigator nav = new DVDNavigator();
            dvdInfo = (IDvdInfo2)nav;
            dirName = directoryPath;
        }

        public string GetDiscID()
        {
            long id = 0;
            hr = dvdInfo.GetDiscID(dirName, out id);
            DsError.ThrowExceptionForHR(hr);

            if (hr == 0)
                return Convert.ToString(id, 16);

            return null;
        }

        public int DefaultMenuLanguageId()
        {
            int languageId = 0;
            hr = dvdInfo.GetDefaultMenuLanguage(out languageId);
            DsError.ThrowExceptionForHR(hr);

            if (hr == 0)
                return languageId;

            return 0;
        }

        public int GetNumberOfChaptersForTitle(int titleNumber)
        {
            int numChapters = 0;
            hr = dvdInfo.GetNumberOfChapters(titleNumber, out numChapters);
            DsError.ThrowExceptionForHR(hr);

            if (hr == 0)
                return numChapters;

            return 0;
        }

        public void GetTitleAttributes(int titleNumber)
        {
            DvdMenuAttributes menuAttributes;
            DvdTitleAttributes titleAttributes = new DvdTitleAttributes();
            hr = dvdInfo.GetTitleAttributes(titleNumber, out menuAttributes, titleAttributes);
            DsError.ThrowExceptionForHR(hr);

            if (hr == 0)
            {
                DvdAudioAttributes[] audioAttributes = titleAttributes.AudioAttributes;
                DvdSubpictureAttributes[] subpictureAttributes = titleAttributes.SubpictureAttributes;
                DvdMultichannelAudioAttributes[] mcAudioAttributes = titleAttributes.MultichannelAudioAttributes;
                int numAudioStreams = titleAttributes.ulNumberOfAudioStreams;
                int numSubpictureStreams = titleAttributes.ulNumberOfSubpictureStreams;

                DvdVideoAttributes videoAttributes = titleAttributes.VideoAttributes;

                for (int i = 0; i < numAudioStreams; i++)
                {
                    DvdAudioFormat format = audioAttributes[i].AudioFormat;
                    byte numChannels = audioAttributes[i].bNumberOfChannels;
                    bool hasMultiChannelInfo = audioAttributes[i].fHasMultichannelInfo;
                    int languageId = audioAttributes[i].Language;
                    DvdAudioLangExt ext = audioAttributes[i].LanguageExtension;
                    if (languageId > 0)
                    {
                        CultureInfo cInfo = new CultureInfo(languageId);
                        string languageName = cInfo.Name;
                    }

                    if (hasMultiChannelInfo)
                    {
                        DvdMUACoeff[] huh = mcAudioAttributes[i].Coeff;
                        DvdMUAMixingInfo[] info = mcAudioAttributes[i].Info;
                    }
                }

                for (int i = 0; i < numSubpictureStreams; i++)
                {
                    DvdSubPictureCoding coding = subpictureAttributes[i].CodingMode;
                    int languageId = subpictureAttributes[i].Language;
                    DvdSubPictureLangExt ext = subpictureAttributes[i].LanguageExtension;
                    DvdSubPictureType type = subpictureAttributes[i].Type;
                }

                int x = videoAttributes.aspectX;
                int y = videoAttributes.aspectY;
                DvdVideoCompression compression = videoAttributes.compression;
                int height = videoAttributes.frameHeight;
                int fRate = videoAttributes.frameRate;
                bool isFilmMode = videoAttributes.isFilmMode;
                bool isLetterbox = videoAttributes.isSourceLetterboxed;
                bool isLetterboxPermitted = videoAttributes.letterboxPermitted;
                bool isPanAndScanPermitted = videoAttributes.panscanPermitted;
                int sourceX = videoAttributes.sourceResolutionX;
                int sourceY = videoAttributes.sourceResolutionY;
            }
        }

        public DvdHMSFTimeCode GetTotalTitleLength(int titleNumber)
        {
            DvdHMSFTimeCode tCode = new DvdHMSFTimeCode();
            DvdTimeCodeFlags flag;
            hr = dvdInfo.GetTotalTitleTime(tCode, out flag);
            DsError.ThrowExceptionForHR(hr);

            if (hr == 0)
                return tCode;

            return null;
        }

        public AudioStream GetAudioAttributes(int audioStreamId)
        {
            DvdAudioAttributes attributes = new DvdAudioAttributes();
            hr = dvdInfo.GetAudioAttributes(audioStreamId, out attributes);
            DsError.ThrowExceptionForHR(hr);

            if (hr == 0)
            {
                DvdAudioFormat format = attributes.AudioFormat;
                byte numChannels = attributes.bNumberOfChannels;
                int languageId = attributes.Language;
                DvdAudioLangExt ext = attributes.LanguageExtension;

                AudioStream aStream = new AudioStream();
                aStream.Format = format.ToString();
                aStream.NumberOfChannels = numChannels;
                aStream.LanguageId = languageId;
                aStream.LanguageExtension = ext.ToString();

                return aStream;
            }
            return null;
        }

        public string GetAudioLanguageName(int audioStreamId)
        {
            int lcid = 0;
            hr = dvdInfo.GetAudioLanguage(audioStreamId, out lcid);
            DsError.ThrowExceptionForHR(hr);

            if (hr == 0)
            {
                CultureInfo cInfo = new CultureInfo(lcid);
                if (cInfo != null)
                    return cInfo.Name;
            }
            return null;
        }

        public string GetDefaultAudioLanguage()
        {
            int lcid = 0;
            DvdAudioLangExt ext = new DvdAudioLangExt();
            hr = dvdInfo.GetDefaultAudioLanguage(out lcid, out ext);
            DsError.ThrowExceptionForHR(hr);

            if (hr == 0)
            {
                CultureInfo cInfo = new CultureInfo(lcid);
                if (cInfo != null)
                    return cInfo.Name;
            }
            return null;
        }

        public string GetDefaultSubpictureLanguage()
        {
            int lcid = 0;
            DvdSubPictureLangExt ext = new DvdSubPictureLangExt();
            hr = dvdInfo.GetDefaultSubpictureLanguage(out lcid, out ext);
            DsError.ThrowExceptionForHR(hr);

            if (hr == 0)
            {
                CultureInfo cInfo = new CultureInfo(lcid);
                if (cInfo != null)
                    return cInfo.Name;
            }
            return null;
        }

        public SubpictureStream GetSubpictureAttributes(int subpictureStreamId)
        {
            DvdSubpictureAttributes attrs = new DvdSubpictureAttributes();
            hr = dvdInfo.GetSubpictureAttributes(subpictureStreamId, out attrs);
            DsError.ThrowExceptionForHR(hr);

            if (hr == 0)
            {
                DvdSubPictureCoding coding = attrs.CodingMode;
                int languageLCID = attrs.Language;
                DvdSubPictureLangExt langExt = attrs.LanguageExtension;
                DvdSubPictureType type = attrs.Type;

                SubpictureStream sStream = new SubpictureStream();
                sStream.Coding = coding.ToString();
                sStream.LanguageId = languageLCID;
                sStream.LanguageExtension = langExt.ToString();
                sStream.Type = type.ToString();

                return sStream;
            }
            return null;
        }

        public string GetSubpictureLanguageName(int subpictureStreamId)
        {
            int lcid = 0;
            hr = dvdInfo.GetSubpictureLanguage(subpictureStreamId, out lcid);
            DsError.ThrowExceptionForHR(hr);

            if (hr == 0)
            {
                CultureInfo cInfo = new CultureInfo(lcid);
                if (cInfo != null)
                    return cInfo.Name;
            }
            return null;
        }

        public DvdParentalLevel GetTitleParentalLevel(int titleNumber)
        {
            DvdParentalLevel level = DvdParentalLevel.None;
            hr = dvdInfo.GetTitleParentalLevels(titleNumber, out level);
            DsError.ThrowExceptionForHR(hr);

            return level;
        }

        public IDvdState GetCurrentState()
        {
            IDvdState state;
            hr = dvdInfo.GetState(out state);
            DsError.ThrowExceptionForHR(hr);
            return state;
        }


    }
}
