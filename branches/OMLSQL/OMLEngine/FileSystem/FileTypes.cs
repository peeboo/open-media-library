using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace OMLEngine.FileSystem
{
    public enum DirectoryType
    {
        HDDvd,
        BluRay,
        DVD,
        Normal
    }

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
            ".WPL" // windows media playlist
        };
    }
}
