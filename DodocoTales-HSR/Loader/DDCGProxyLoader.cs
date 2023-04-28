using DodocoTales.SR.Common;
using DodocoTales.SR.Library.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;
namespace DodocoTales.SR.Loader
{
    public class DDCGProxyLoader
    {
        public ProxyServer PServer { get; set; }
        private ExplicitProxyEndPoint EndPoint;

        public string Authkey { get; set; }
        public DDCLGameClientType CapturedClientType { get; set; }

        public DDCGProxyLoader()
        {
            PServer = null;
            Authkey = null;
            CapturedClientType = DDCLGameClientType.Unknown;
        }

        public bool InitializeProxy()
        {
            if (PServer == null)
            {
                PServer = new ProxyServer();
                PServer.CertificateManager.EnsureRootCertificate();
                PServer.BeforeRequest += BeforeRequest;
                EndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 12199);
                //DDCLog.Info(DCLN.Loader, "Proxy server initialized");
                return true;
            }
            return false;
        }

        public void StartProxy()
        {
            Authkey = null;
            CapturedClientType = DDCLGameClientType.Unknown;

            PServer.AddEndPoint(EndPoint);
            PServer.Start();
            PServer.SetAsSystemHttpProxy(EndPoint);
            PServer.SetAsSystemHttpsProxy(EndPoint);
            //DDCLog.Info(DCLN.Loader, "Proxy Mode On");
        }

        public void EndProxy()
        {
            if (PServer.ProxyRunning)
            {
                PServer.Stop();
            }
            PServer.DisableAllSystemProxies();
            //DDCLog.Info(DCLN.Loader, "Proxy Mode Off");
        }

        private Task BeforeRequest(object sender, SessionEventArgs e)
        {
            var request = e.HttpClient.Request;
            string authkey = null;
            if ((request.Host == "api-takumi.mihoyo.com") && request.RequestUri.AbsolutePath == "/common/gacha_record/api/getGachaLog")
            {
                string pattern = @".+\?(\S+&game_biz=hkrpg_cn)";
                var result = Regex.Matches(request.Url, pattern);
                Regex regex = new Regex(@"lang=.+&authkey=");
                authkey = regex.Replace(result[result.Count - 1].Groups[1].Value, "lang=zh-cn&device_type");
            }
            if (authkey != null)
            {
                Authkey = authkey;
                if (Authkey.Contains("game_biz=hkrpg_cn"))
                    CapturedClientType = DDCLGameClientType.CN;
                else
                    CapturedClientType = DDCLGameClientType.Unknown;

                //DDCLog.Info(DCLN.Loader, "Authkey captured by proxy server.");
                DDCS.Emit_ProxyCaptured();
            }
            return Task.CompletedTask;
        }
    }
}
