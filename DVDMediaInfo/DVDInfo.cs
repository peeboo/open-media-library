using System;
using System.Globalization;
using DirectShowLib;
using DirectShowLib.Dvd;

namespace DVDMediaInfo
{
    public class DVDInfo
    {
        IDvdGraphBuilder dvdGraph;
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

        public int DefaultMenuLanguage()
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

        public void GetTotalTitleLength(int titleNumber)
        {
        }

        public void GetAudioAttributes(int audioStreamId)
        {
        }

        public void GetAudioLanguage(int audioStreamId)
        {
        }

        public void GetDefaultAudioLanguage()
        {
        }

        public void GetDefaultSubpictureLanguage()
        {
        }

        public void GetSubpictureAttributes(int subpictureStreamId)
        {
        }

        public void GetSubpictureLanguage(int subpictureStreamId)
        {
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
