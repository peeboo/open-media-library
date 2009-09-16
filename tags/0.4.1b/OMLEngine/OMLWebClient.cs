using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
namespace OMLEngine {
    public class OMLWebClient : WebClient {
        private System.Net.CookieContainer cookieContainer;
        private string userAgent;
        private int timeout;

        public System.Net.CookieContainer CookieContainer {
            get { return cookieContainer; }
            set { cookieContainer = value; }
        }

        public string UserAgent {
            get { return userAgent; }
            set { userAgent = value; }
        }

        public int Timeout {
            get { return timeout; }
            set { timeout = value; }
        }

        public OMLWebClient() {
            timeout = -1;
            userAgent = @"Mozilla/4.0 (compatible; MS 7.0; Windows NT 5.1; .NET CLR 2.0.50727)";
            cookieContainer = new CookieContainer();
        }

        protected override WebRequest GetWebRequest(Uri address) {
            WebRequest request = base.GetWebRequest(address);

            if (request.GetType() == typeof(HttpWebRequest)) {
                ((HttpWebRequest)request).CookieContainer = cookieContainer;
                ((HttpWebRequest)request).UserAgent = userAgent;
                ((HttpWebRequest)request).Timeout = timeout;
            }
            return request;
        }
    }
}
