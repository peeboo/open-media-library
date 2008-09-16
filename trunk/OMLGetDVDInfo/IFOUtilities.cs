using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace OMLGetDVDInfo
{
    public static class IFOUtilities
    {
        public static byte[] GetFileBlock(string strFile, long pos, int count)
        {
            using (FileStream stream = new FileStream(strFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] buf = new byte[count];
                stream.Seek(pos, SeekOrigin.Begin);
                if (stream.Read(buf, 0, count) != count)
                    return buf;
                return buf;
            }
        }
        public static short ToInt16(byte[] bytes) { return (short)((bytes[0] << 8) + bytes[1]); }
        public static uint ToInt32(byte[] bytes) { return (uint)((bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3]); }
        public static short ToShort(byte[] bytes) { return ToInt16(bytes); }
        public static long ToFilePosition(byte[] bytes) { return ToInt32(bytes) * 0x800L; }
        public static long GetTotalFrames(TimeSpan time, int fps)
        {
            return (long)Math.Round(fps * time.TotalSeconds);
        }
        
        static string TwoLong(int val) { return string.Format("{0:D2}", val); }

        static int AsHex(int val)
        {
            int ret;
            int.TryParse(string.Format("{0:X2}", val), out ret);
            return ret;
        }

        internal static short? GetFrames(byte val)
        {
            int byte0_high = val >> 4;
            int byte0_low = val & 0x0F;
            if (byte0_high > 11)
                return (short)(((byte0_high - 12) * 10) + byte0_low);
            if ((byte0_high <= 3) || (byte0_high >= 8))
                return null;
            return (short)(((byte0_high - 4) * 10) + byte0_low);
        }

        internal static int GetFrames(TimeSpan time, int fps)
        {
            return (int)Math.Round(fps * time.Milliseconds / 1000.0);
        }
        internal static long GetPCGIP_Position(string ifoFile)
        {
            return ToFilePosition(GetFileBlock(ifoFile, 0xCC, 4));
        }

        internal static int GetProgramChains(string ifoFile, long pcgitPosition)
        {
            return ToInt16(GetFileBlock(ifoFile, pcgitPosition, 2));
        }

        internal static uint GetChainOffset(string ifoFile, long pcgitPosition, int programChain)
        {
            return ToInt32(GetFileBlock(ifoFile, (pcgitPosition + (8 * programChain)) + 4, 4));
        }

        internal static int GetNumberOfPrograms(string ifoFile, long pcgitPosition, uint chainOffset)
        {
            return GetFileBlock(ifoFile, (pcgitPosition + chainOffset) + 2, 1)[0];
        }

        internal static TimeSpan? ReadTimeSpan(string ifoFile, long pcgitPosition, uint chainOffset, out int fps)
        {
            return ReadTimeSpan(GetFileBlock(ifoFile, (pcgitPosition + chainOffset) + 4, 4), out fps);
        }
        internal static TimeSpan? ReadTimeSpan(byte[] playbackBytes, out int fps)
        {
            short? frames = GetFrames(playbackBytes[3]);
            int fpsMask = playbackBytes[3] >> 6;
            fps = fpsMask == 0x01 ? 25 : fpsMask == 0x03 ? 30 : 0;
            if (frames == null)
                return null;

            try
            {
                int hours = AsHex(playbackBytes[0]);
                int minutes = AsHex(playbackBytes[1]);
                int seconds = AsHex(playbackBytes[2]);
                TimeSpan ret = new TimeSpan(hours, minutes, seconds);
                if (fps != 0)
                    ret = ret.Add(TimeSpan.FromSeconds((double)frames / (double)fps));
                return ret;
            }
            catch { return null; }
        }

        public static string GetLongestIFO(string directory)
        {
            if (Directory.Exists(directory))
            {
                string longestFile = null;
                TimeSpan max = TimeSpan.Zero;
                foreach (string ifoFile in Directory.GetFiles(directory, "VTS_*.IFO"))
                {
                    List<DVDChapter> chapters = Chapters(Path.Combine(directory, ifoFile), -1);
                    if (chapters != null)
                    {
                        TimeSpan time = DVDTitle.GetTotalTimeSpan(chapters);
                        if (time > max)
                        {
                            longestFile = ifoFile;
                            max = time;
                        }
                    }
                }
                return longestFile;
            }
            return null;
        }

        public static List<DVDChapter> Chapters(string ifoFile, int programChain)
        {
            List<DVDChapter> chapters = new List<DVDChapter>();
            long pcgITPosition = GetPCGIP_Position(ifoFile);
            int programChainPrograms = -1;
            TimeSpan programTime = TimeSpan.Zero;
            if (programChain >= 0)
            {
                int FPS;
                uint chainOffset = GetChainOffset(ifoFile, pcgITPosition, programChain);
                programTime = ReadTimeSpan(ifoFile, pcgITPosition, chainOffset, out FPS) ?? TimeSpan.Zero;
                programChainPrograms = GetNumberOfPrograms(ifoFile, pcgITPosition, chainOffset);
            }
            else
            {
                int programChains = GetProgramChains(ifoFile, pcgITPosition);
                for (int curChain = 1; curChain <= programChains; curChain++)
                {
                    int FPS;
                    uint chainOffset = GetChainOffset(ifoFile, pcgITPosition, curChain);
                    TimeSpan? time = ReadTimeSpan(ifoFile, pcgITPosition, chainOffset, out FPS);
                    if (time == null)
                        break;

                    if (time.Value > programTime)
                    {
                        programChain = curChain;
                        programChainPrograms = GetNumberOfPrograms(ifoFile, pcgITPosition, chainOffset);
                        programTime = time.Value;
                    }
                }
            }
            if (programChain < 0)
                return null;

            uint longestChainOffset = GetChainOffset(ifoFile, pcgITPosition, programChain);
            int programMapOffset = ToInt16(GetFileBlock(ifoFile, (pcgITPosition + longestChainOffset) + 230, 2));
            int cellTableOffset = ToInt16(GetFileBlock(ifoFile, (pcgITPosition + longestChainOffset) + 0xE8, 2));
            for (int currentProgram = 0; currentProgram < programChainPrograms; ++currentProgram)
            {
                int entryCell = GetFileBlock(ifoFile, ((pcgITPosition + longestChainOffset) + programMapOffset) + currentProgram, 1)[0];
                int exitCell = entryCell;
                if (currentProgram < (programChainPrograms - 1))
                    exitCell = GetFileBlock(ifoFile, ((pcgITPosition + longestChainOffset) + programMapOffset) + (currentProgram + 1), 1)[0] - 1;

                TimeSpan totalTime = TimeSpan.Zero;
                int fps = 0;
                for (int currentCell = entryCell; currentCell <= exitCell; currentCell++)
                {
                    int cellStart = cellTableOffset + ((currentCell - 1) * 0x18);
                    byte[] bytes = GetFileBlock(ifoFile, (pcgITPosition + longestChainOffset) + cellStart, 4);
                    int cellType = bytes[0] >> 6;
                    if (cellType == 0x00 || cellType == 0x01)
                    {
                        bytes = GetFileBlock(ifoFile, ((pcgITPosition + longestChainOffset) + cellStart) + 4, 4);
                        TimeSpan time = ReadTimeSpan(bytes, out fps) ?? TimeSpan.Zero;
                        totalTime += time;
                    }
                }

                totalTime += TimeSpan.FromMilliseconds(fps != 0 ? (double)1000 / fps / 8 : 0);
                chapters.Add(new DVDChapter() { ChapterNumber = currentProgram + 1, Duration = totalTime, FPS = fps });
            }
            return chapters;
        }

        internal static List<DVDAudioTrack> AudioTracks(string ifoFile)
        {
            List<DVDAudioTrack> ret = new List<DVDAudioTrack>();
            int audioStreams = IFOUtilities.GetFileBlock(ifoFile, 0x203, 1)[0];
            for (int currentAudioStream = 1; currentAudioStream <= audioStreams; currentAudioStream++)
            {
                byte[] bytes = IFOUtilities.GetFileBlock(ifoFile, (long)(0x204 + (8 * (currentAudioStream - 1))), 8);
                AudioValues audioValues = AudioValues.ReadAudioSpecs(bytes[0], bytes[1], bytes[5]);
                string languageCode = "";
                if (audioValues.LanguageTypePresent)
                    languageCode = "" + (char)bytes[2] + (char)bytes[3];

                ret.Add(new DVDAudioTrack()
                {
                    Frequency = audioValues.SampleRate * 1000, 
                    Language = Language(languageCode), 
                    TrackNumber = currentAudioStream,
                    SubFormat = SubFormat(audioValues.Channels),
                    Format = audioValues.Encoding.ToString(),
                    Extension = audioValues.Extension,
                    // Bitrate = ,
                });
            }
            return ret;
        }

        static string Language(string languageCode)
        {
            switch (languageCode)
            {
                case "en": return "English";
                case "es": return "Espanol";
                case "fr": return "Francais";
                case "de": return "Deutsch";
                case "ru": return "Россия";
                case "": return "Unknown";
            }
            return null;
        }

        static string SubFormat(int channels)
        {
            switch (channels)
            {
                case 1: return "1.0 ch";
                case 2: return "2.0 ch";
                case 6: return "5.1 ch";
                case 7: return "6.1 ch";
                case 8: return "7.1 ch";
            }
            return null;
        }

        internal static List<DVDSubtitle> SubTitleTracks(string ifoFile)
        {
            List<DVDSubtitle> ret = new List<DVDSubtitle>();
            int subPictureStreams = IFOUtilities.GetFileBlock(ifoFile, 0x255, 1)[0];
            for (int currentSubPictureStream = 1; currentSubPictureStream <= subPictureStreams; currentSubPictureStream++)
            {
                byte[] bytes = IFOUtilities.GetFileBlock(ifoFile, (long)(0x256 + (6 * (currentSubPictureStream - 1))), 6);
                if (bytes[2] == 0 || bytes[3] == 0)
                    continue;

                string languageCode = "" + (char)bytes[2] + (char)bytes[3];
                ret.Add(new DVDSubtitle() { Language = Language(languageCode), TrackNumber = currentSubPictureStream });
            }
            return ret;
        }

        public static List<DVDTitle> Titles(string videoTSDir)
        {
            List<DVDTitle> ret = new List<DVDTitle>();

            string videoIFO = Path.Combine(videoTSDir, "VIDEO_TS.IFO");
            if (File.Exists(videoIFO))
            {
                byte[] bytRead = new byte[4];
                long VMG_PTT_STPT_Position = ToFilePosition(GetFileBlock(videoIFO, 0xC4, 4));
                int titlePlayMaps = ToInt16(GetFileBlock(videoIFO, VMG_PTT_STPT_Position, 2));
                string longestIfo = GetLongestIFO(videoTSDir);
                for (int currentTitle = 1; currentTitle <= titlePlayMaps; ++currentTitle)
                {
                    DVDTitle title = new DVDTitle() { TitleNumber = currentTitle };
                    long titleInfoStart = 8 + ((currentTitle - 1) * 12);
                    int titleSetNumber = GetFileBlock(videoIFO, (VMG_PTT_STPT_Position + titleInfoStart) + 6L, 1)[0];
                    int titleSetTitleNumber = IFOUtilities.GetFileBlock(videoIFO, (VMG_PTT_STPT_Position + titleInfoStart) + 7L, 1)[0];
                    string vtsIFO = Path.Combine(videoTSDir, string.Format("VTS_{0:D2}_0.IFO", titleSetNumber));
                    if (File.Exists(vtsIFO) == false)
                    {
                        Trace.WriteLine(string.Format("IFOUtils.Titles: VTS IFO file missing: {0}", Path.GetFileName(vtsIFO)));
                        continue;
                    }
                    byte[] b = IFOUtilities.GetFileBlock(vtsIFO, 0x200, 2);
                    VideoValues video = VideoValues.ReadVideoSpecs(b[0], b[1]);
                    title.Main = longestIfo == Path.GetFileName(vtsIFO);
                    title.File = "vts " + titleSetNumber;
                    title.AspectRatio = video.AspectRatio;
                    title.Resolution = video.Resolution;
                    title.Chapters = Chapters(vtsIFO, titleSetTitleNumber);
                    title.AudioTracks = AudioTracks(vtsIFO);
                    title.Subtitles = SubTitleTracks(vtsIFO);
                    title.Duration = DVDTitle.GetTotalTimeSpan(title.Chapters);
                    if (title.Duration.TotalSeconds > 10)
                        ret.Add(title);
                    else
                        Trace.WriteLine(string.Format("IFOUtils.Titles: Duration < 10s, ignoring: {0}", Path.GetFileName(vtsIFO)));
                }
            }
            else
                Trace.WriteLine(string.Format("IFOUtilities.Titles: Cannot analyze DVD, file {0} not found", videoIFO));
            return ret;
        }

    }
}
