using System;
using System.Globalization;
using DirectShowLib;
using DirectShowLib.Dvd;

namespace DVDMediaInfo
{
    public class DVDInfo
    {
        IDvdInfo2 dvdInfo = (IDvdInfo2)new DVDNavigator();
        string dirName;

        public DVDInfo(string directoryPath)
        {
            dirName = directoryPath;
        }

        public string GetDiscID()
        {
            long id;
            DsError.ThrowExceptionForHR(dvdInfo.GetDiscID(dirName, out id));
            return Convert.ToString(id, 16);
        }

        public int DefaultMenuLanguageId()
        {
            int languageId;
            DsError.ThrowExceptionForHR(dvdInfo.GetDefaultMenuLanguage(out languageId));
            return languageId;
        }

        public int GetNumberOfChaptersForTitle(int titleNumber)
        {
            int numChapters;
            DsError.ThrowExceptionForHR(dvdInfo.GetNumberOfChapters(titleNumber, out numChapters));
            return numChapters;
        }

        public void GetTitleAttributes(int titleNumber)
        {
            DvdMenuAttributes menuAttributes;
            DvdTitleAttributes titleAttributes = new DvdTitleAttributes();
            DsError.ThrowExceptionForHR(dvdInfo.GetTitleAttributes(titleNumber, out menuAttributes, titleAttributes));

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

        public DvdHMSFTimeCode GetTotalTitleLength(int titleNumber)
        {
            DvdHMSFTimeCode tCode = new DvdHMSFTimeCode();
            DvdTimeCodeFlags flag;
            DsError.ThrowExceptionForHR(dvdInfo.GetTotalTitleTime(tCode, out flag));
            return tCode;
        }

        public AudioStream GetAudioAttributes(int audioStreamId)
        {
            DvdAudioAttributes attributes = new DvdAudioAttributes();
            DsError.ThrowExceptionForHR(dvdInfo.GetAudioAttributes(audioStreamId, out attributes));

            return new AudioStream()
            {
                Format = attributes.AudioFormat.ToString(),
                NumberOfChannels = attributes.bNumberOfChannels,
                LanguageId = attributes.Language,
                LanguageExtension = attributes.LanguageExtension.ToString(),
            };
        }

        public string GetAudioLanguageName(int audioStreamId)
        {
            int lcid;
            DsError.ThrowExceptionForHR(dvdInfo.GetAudioLanguage(audioStreamId, out lcid));
            return new CultureInfo(lcid).Name;
        }

        public string GetDefaultAudioLanguage()
        {
            int lcid;
            DvdAudioLangExt ext;
            DsError.ThrowExceptionForHR(dvdInfo.GetDefaultAudioLanguage(out lcid, out ext));
            return new CultureInfo(lcid).Name;
        }

        public string GetDefaultSubpictureLanguage()
        {
            int lcid;
            DvdSubPictureLangExt ext;
            DsError.ThrowExceptionForHR(dvdInfo.GetDefaultSubpictureLanguage(out lcid, out ext));
            return new CultureInfo(lcid).Name;
        }

        public SubpictureStream GetSubpictureAttributes(int subpictureStreamId)
        {
            DvdSubpictureAttributes attrs;
            DsError.ThrowExceptionForHR(dvdInfo.GetSubpictureAttributes(subpictureStreamId, out attrs));

            return new SubpictureStream()
            {
                Coding = attrs.CodingMode.ToString(),
                LanguageId = attrs.Language,
                LanguageExtension = attrs.LanguageExtension.ToString(),
                Type = attrs.Type.ToString()
            };
        }

        public string GetSubpictureLanguageName(int subpictureStreamId)
        {
            int lcid;
            DsError.ThrowExceptionForHR(dvdInfo.GetSubpictureLanguage(subpictureStreamId, out lcid));
            return new CultureInfo(lcid).Name;
        }

        public DvdParentalLevel GetTitleParentalLevel(int titleNumber)
        {
            DvdParentalLevel level;
            DsError.ThrowExceptionForHR(dvdInfo.GetTitleParentalLevels(titleNumber, out level));
            return level;
        }

        public IDvdState GetCurrentState()
        {
            IDvdState state;
            DsError.ThrowExceptionForHR(dvdInfo.GetState(out state));
            return state;
        }

    }
}
