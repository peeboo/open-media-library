using System;
using System.ServiceModel;

using OMLEngine;

namespace OMLEngineService
{
    [ServiceContract(Namespace= "net.tcp://localhost:8321/OMLTS")]
    public interface ITranscodingService
    {
        [OperationContract(IsOneWay = true)]
        void Transcode(MediaSource source, string mcxuser);

        [OperationContract(IsOneWay = true)]
        void Cancel(string key);

        [OperationContract]
        TranscodingStatus GetStatus(string key);

        [OperationContract(IsOneWay = true)]
        void RegisterNotifyer(string url, bool register);
    }

    public enum TranscodingStatus
    {
        Unknown,
        Initializing,
        BufferReady,
        Stopped,
        Done,
        Error,
    }

    [ServiceContract(Namespace= "net.tcp://localhost:8321/OMLTNS")]
    public interface ITranscodingNotifyingService
    {
        [OperationContract(IsOneWay = true)]
        void StatusChanged(string key, TranscodingStatus status);
    }
}
