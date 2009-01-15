using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace OMLEngine
{
    public enum DirectoryType
    {
        HDDVD,
        BluRay,
        DVD,
        Normal
    }

    /// <summary>
    /// Enumerator for Video Fromats
    /// </summary>
    public enum VideoFormat : int
    {
        // DO NOT MODIFY ORDER, INSERT IN THE MIDDLE, OR REMOVE ENTRIES, JUST ADD TO THE END!

        ASF = 1, // WMV style
        AVC = 2, // AVC H264
        AVI = 3, // DivX, Xvid, etc
        B5T = 4, // BlindWrite image
        B6T = 5, // BlindWrite image
        BIN = 6, // using an image loader lib and load/play this as a DVD
        BLURAY = 7, // detect which drive supports this and request the disc
        BWT = 8, // BlindWrite image
        CCD = 9, // CloneCD image
        CDI = 10, // DiscJuggler Image
        CUE = 11, // cue sheet
        DVD = 12, // detect which drive supports this and request the disc
        DVRMS = 13, // MPG
        H264 = 14, // AVC OR MP4
        HDDVD = 15, // detect which drive supports this and request the disc
        IFO = 16, // Online DVD
        IMG = 17, // using an image loader lib and load/play this as a DVD
        ISO = 18, // Standard ISO image
        ISZ = 19, // Compressed ISO image
        M2TS = 20, // mpeg2 transport stream
        MDF = 21, // using an image loader lib and load/play this as a DVD
        MDS = 22, // Media Descriptor file
        MKV = 23, // Likely h264
        MOV = 24, // Quicktime
        MPG = 25,
        MPEG = 26,
        MP4 = 27, // DivX, AVC, or H264
        NRG = 28, // Nero image
        OFFLINEBLURAY = 29, // detect which drive supports this and request the disc
        OFFLINEDVD = 30, // detect which drive supports this and request the disc
        OFFLINEHDDVD = 31, // detect which drive supports this and request the disc
        OGM = 32, // Similar to MKV
        PDI = 33, // Instant CD/DVD image
        TS = 34, // MPEG2
        UIF = 35,
        UNKNOWN = 36,
        URL = 37, // this is used for online content (such as streaming trailers)
        WMV = 38,
        VOB = 39, // MPEG2
        WVX = 40, // wtf is this?
        ASX = 41, // like WPL
        WPL = 42, // playlist file?
        WTV = 43, // new dvr format in vista (introduced in the tv pack 2008)
        DIVX = 44,

        ALL = 2147483647, // meaning all format types - used for setting video format to external player
    };

    public static class FileTypes
    {        
        public static readonly HashSet<string> SupportedVideoExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) 
        {
            ".ASF",
            ".AVC",
            ".AVI", // DivX, Xvid, etc
            ".B5T", // BlindWrite image
            ".B6T", // BlindWrite image
            ".BIN", // using an image loader lib and load/play this as a DVD
            ".BWT", // BlindWrite image
            ".CCD", // CloneCD image
            ".CDI", // DiscJuggler Image
            ".CUE", // cue sheet
            ".DVR-MS", // MPG
            ".DVRMS", // MPG
            ".H264", // AVC OR MP4
            ".IMG", // using an image loader lib and load/play this as a DVD
            ".ISO", // Standard ISO image
            ".ISZ", // Compressed ISO image
            ".M2TS", // mpeg2 transport stream
            ".MDF", // using an image loader lib and load/play this as a DVD
            ".MDS", // Media Descriptor file
            ".MKV", // Likely h264
            ".MOV", // Quicktime
            ".MPG",
            ".MPEG",
            ".MP4", // DivX, AVC, or H264
            ".NRG", // Nero image
            ".OGM", // Similar to MKV
            ".PDI", // Instant CD/DVD image
            ".TS", // MPEG2
            ".UIF",
            ".WMV",
            //"VOB", // MPEG2 - make sure it's not part of a DVD
            ".WVX", // windows playlist file
            ".ASX", // windows playlist file
            ".WPL", // windows media playlist
            ".WTV",
            ".DIVX"
        };
    }
}
