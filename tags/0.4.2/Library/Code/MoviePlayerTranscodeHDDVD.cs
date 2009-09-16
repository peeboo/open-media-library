using System;
using OMLEngine;

namespace Library
{
    public class TranscodeHDDVDPlayer: IPlayMovie
    {
        /*******************************************************************************
         * Either AnyHDDVD is required OR the disc must have already been ripped to HD *
         * A VERY powerful machine is required for realtime transcoding                *
         *******************************************************************************/

// command to transcode hddvd is:
// mencoder <filename.evo> -o <output filename> -oac copy -ovc lavc -lavcopts codec=???:vbitrate=???? -fps <30|60> -vf -scale=1280:720 (note: -scale=1920:1080 is not a good idea, encoding would really suck)
// The audio streams will have to be reviewed for NON HD/TRU audio channels, we need either DTSHD, DTS, DD, or something else useful.
// NOTE: DTSHD is completely compatible with DTS, while Dolby Digital HD is NOT compatible with Dolby Digital

        private MediaSource _source;

        public TranscodeHDDVDPlayer(MediaSource source)
        {
            _source = source;
        }

        public bool PlayMovie()
        {
            return false;
        }

    }
}
