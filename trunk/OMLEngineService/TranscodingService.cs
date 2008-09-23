using System;
using System.ServiceModel;

using OMLEngine;

namespace OMLEngineService
{
    public class TranscodingService : ITranscodingService
    {
        public void Transcode(OMLEngine.MediaSource source)
        {
            Utilities.DebugLine("Transcode: {0}", source);
        }

        public void Cancel(string key)
        {
            Utilities.DebugLine("Cancel: {0}", key);
        }

        public TranscodingStatus GetStatus(string key)
        {
            Utilities.DebugLine("GetStatus: {0}", key);
            return TranscodingStatus.Error;
        }
    }
}
