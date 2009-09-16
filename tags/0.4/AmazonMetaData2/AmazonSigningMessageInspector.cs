using System;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Text.RegularExpressions;

namespace AmazonMetaData2
{
    class AmazonSigningMessageInspector : IClientMessageInspector
    {
        public AmazonSigningMessageInspector()
        {
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            string operation = Regex.Match(request.Headers.Action, "[^/]+$").ToString();
            DateTime now = DateTime.UtcNow;
            string timestamp = now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            string signMe = operation + timestamp;
            byte[] bytesToSign = Encoding.UTF8.GetBytes(signMe);

            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(Properties.Settings.Default.AWEAccessKey);
            HMAC hmacSha256 = new HMACSHA256(secretKeyBytes);
            byte[] hashBytes = hmacSha256.ComputeHash(bytesToSign);
            string signature = Convert.ToBase64String(hashBytes);

            request.Headers.Add(new AmazonHeader("AWSAccessKeyId", Properties.Settings.Default.AWEAccessKeyId));
            request.Headers.Add(new AmazonHeader("Timestamp", timestamp));
            request.Headers.Add(new AmazonHeader("Signature", signature));

            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState) { }
    }
}
