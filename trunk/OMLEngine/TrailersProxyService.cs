using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace OMLEngine
{
    public delegate void delReceiveWebRequest(HttpListenerContext Context);

    public class TrailersProxyService
    {
        public HttpServer Server = null;

        public TrailersProxyService()
        {
            Server = new TrailersProxyServer();
            Server.Start("http://127.0.0.1:8484/3f0850a7-0fd7-4cbf-b8dc-c7f7ea31534e/");
        }
    }
    /// <summary>
    /// Wrapper class for the HTTPListener to allow easier access to the
    /// server, for start and stop management and event routing of the actual
    /// inbound requests.
    /// </summary>
    public class HttpServer
    {
        protected HttpListener Listener;
        protected bool IsStarted = false;

        public event delReceiveWebRequest ReceiveWebRequest;

        public HttpServer()
        {
        }

        /// <summary>
        /// Starts the Web Service
        /// </summary>
        /// <param name="UrlBase">
        /// A Uri that acts as the base that the server is listening on. 
        /// Format should be: http://127.0.0.1:8080/ or http://127.0.0.1:8080/somevirtual/ 
        /// Note: the trailing backslash is required! For more info see the 
        /// HttpListener.Prefixes property on MSDN.
        /// </param>
        public void Start(string UrlBase)
        {
            // *** Already running - just leave it in place
            if (this.IsStarted)
                return;

            if (this.Listener == null)
            {
                this.Listener = new HttpListener();
            }

            this.Listener.Prefixes.Add(UrlBase);
            this.IsStarted = true;
            this.Listener.Start();

            IAsyncResult result = this.Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), this.Listener);
        }

        /// <summary>
        /// Shut down the Web Service
        /// </summary>
        public void Stop()
        {
            if (Listener != null)
            {
                this.Listener.Abort();
                this.Listener.Close();

                this.Listener = null;

                this.IsStarted = false;
            }
        }

        protected void WebRequestCallback(IAsyncResult result)
        {
            try
            {
                if (this.Listener == null)
                    return;

                // Get out the context object
                HttpListenerContext context = this.Listener.EndGetContext(result);

                // *** Immediately set up the next context
                this.Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), this.Listener);

                if (this.ReceiveWebRequest != null)
                    this.ReceiveWebRequest(context);

                this.ProcessRequest(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Overridable method that can be used to implement a custom hnandler
        /// </summary>
        /// <param name="Context"></param>
        protected virtual void ProcessRequest(HttpListenerContext Context)
        {
        }
    }

    public class TrailersProxyServer : HttpServer
    {
        protected override void ProcessRequest(System.Net.HttpListenerContext Context)
        {
            HttpListenerRequest Request = Context.Request;
            HttpListenerResponse Response = Context.Response;
            HttpWebRequest req = null;
            WebResponse res = null;
            try
            {
                int bytesDone = 0;
                Stream stream = Response.OutputStream;
                Stream readStream = null;
                Response.KeepAlive = true;

                req = (HttpWebRequest)System.Net.WebRequest.Create(Request.RawUrl.Replace("/3f0850a7-0fd7-4cbf-b8dc-c7f7ea31534e/", "http://"));
                //Console.WriteLine("Loading :" + Request.RawUrl.Replace("/3f0850a7-0fd7-4cbf-b8dc-c7f7ea31534e/", "http://"));
                req.UserAgent = "QuickTime/7.6.2";
                req.Timeout = 1200000;
                if (req != null)
                {
                    res = req.GetResponse();
                    if (res != null)
                    {

                        Response.StatusCode = 200;
                        Response.StatusDescription = "OK";
                        Response.ContentLength64 = res.ContentLength;
                        Response.ContentType = "application/octet-stream";
                        readStream = res.GetResponseStream();
                        byte[] buffer = new byte[1024];
                        int bytesRead = 0;
                        Stream OutputStream = Response.OutputStream;
                        do
                        {
                            bytesRead = readStream.Read(buffer, 0, buffer.Length);

                            if (OutputStream.CanWrite)
                            {
                                OutputStream.Write(buffer, 0, bytesRead);
                                OutputStream.Flush();
                            }
                            else
                            {
                                req.Abort();
                            }
                            bytesDone += bytesRead;
                        } while (bytesRead > 0);
                        OutputStream.Close();
                    }
                }
                Response.Close();
            }
            catch (Exception)
            {
                //eat it
            }
            finally
            {
                if (req != null)
                    req.Abort();
                if (res != null)
                    res.Close();
                if (Response != null)
                    Response.Close();
                req = null;
                res = null;
                Response = null;
            }
        }
    }
}
