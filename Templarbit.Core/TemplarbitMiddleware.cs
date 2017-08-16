using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Net;
using Templarbit.Core;

namespace Templarbit.Core
{
    public class TemplarbitMiddleware
    {
        public static bool Digest { get; set; } = true;
        private const int MILLISECONDS_IN_SECOND = 1000;
        private readonly RequestDelegate _next;
        private readonly TemplarbitMiddlewareModel _model;
        private readonly ITemplarbitLogger _logger;
        private string contentSecurityPolicy = "";
        private string contentSecurityPolicyReportOnly = "";
        public TemplarbitMiddleware(RequestDelegate next, TemplarbitMiddlewareModel model, ITemplarbitLogger logger)
        {
            _next = next;
            _model = model;
            _logger = logger;

            Initizalization();
            if (!String.IsNullOrEmpty(_model.TemplarbitPropertyId) && !String.IsNullOrEmpty(_model.TemplarbitApiToken))
            {
                Task.Run(() => StartLoop());
            }
        }

        private async Task StartLoop()
        {
            while (Digest)
            {
                try
                {
                    var data = new
                    {
                        property_id = _model.TemplarbitPropertyId,
                        token = _model.TemplarbitApiToken
                    };
                    var jsonData = JsonConvert.SerializeObject(data);
                    var result = await HttpHelper.DataPost(_model.TemplarbitApiUrl + "/csp", jsonData);
                    if (result.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        _logger.Log("invalid templarbit_api_token and/or templarbit_property_id");

                        System.Threading.Thread.Sleep((int)(MILLISECONDS_IN_SECOND * _model.TemplarbitFetchInterval));
                        continue;
                    }
                    if (result.StatusCode != HttpStatusCode.OK)
                    {
                        _logger.Log("Fetch failed, returned status " + result.StatusCode.ToString());

                        System.Threading.Thread.Sleep((int)(MILLISECONDS_IN_SECOND * _model.TemplarbitFetchInterval));
                        continue;
                    }
                    var response = JsonConvert.DeserializeObject<ResponseFromTemplarbit>(result.Response);
                    if (!String.IsNullOrEmpty(response.error))
                    {
                        _logger.Log("Fetch failed: " + response.error);

                        System.Threading.Thread.Sleep((int)(MILLISECONDS_IN_SECOND * _model.TemplarbitFetchInterval));
                        continue;
                    }
                    if (String.IsNullOrEmpty(response.csp) && String.IsNullOrEmpty(response.csp_report_only))
                    {
                        _logger.Log("Fetch successful, but Content-Security-Policy was empty.");

                        System.Threading.Thread.Sleep((int)(MILLISECONDS_IN_SECOND * _model.TemplarbitFetchInterval));
                        continue;
                    }

                    System.Threading.Thread.Sleep((int)(MILLISECONDS_IN_SECOND * _model.TemplarbitFetchInterval));
                    contentSecurityPolicy = response.csp;
                    contentSecurityPolicyReportOnly = response.csp_report_only;
                }
                catch (Exception e)
                {
                    _logger.Log("Fetch failed with error " + e.Message);

                    System.Threading.Thread.Sleep((int)(MILLISECONDS_IN_SECOND * _model.TemplarbitFetchInterval));
                    continue;
                }
            }
        }

        public Task Invoke(HttpContext context)
        {
            try
            {
                if (!String.IsNullOrEmpty(_model.TemplarbitPropertyId) && !String.IsNullOrEmpty(_model.TemplarbitApiToken))
                {
                    Task.Run(() => Run(context));
                }
                // Call the next delegate/middleware in the pipeline
                return this._next(context);
            }
            catch (Exception)
            {
                return this._next(context);
            }

        }

        private void Run(HttpContext context)
        {
            if (!string.IsNullOrEmpty(contentSecurityPolicy))
            {
                try
                {
                    var headers = context.Response.Headers;
                    headers["Content-Security-Policy"] = contentSecurityPolicy;
                }
                catch (Exception e)
                {
                    _logger.Log("Failed to set Content-Security-Policy response header: " + e.Message);
                }
            }
            if (!string.IsNullOrEmpty(contentSecurityPolicyReportOnly))
            {
                try
                {
                    var headers = context.Response.Headers;
                    headers["Content-Security-Policy-Report-Only"] = contentSecurityPolicyReportOnly;
                }
                catch (Exception e)
                {
                    _logger.Log("Failed to set Content-Security-Policy-Report-Only response header: " + e.Message);
                }
            }
        }

        private void Initizalization()
        {
            if (String.IsNullOrEmpty(_model.TemplarbitPropertyId))
            {
                _logger.Log("templarbit_property_id not set");
            }
            if (String.IsNullOrEmpty(_model.TemplarbitApiToken))
            {
                _logger.Log("templarbit_api_token not set");
            }
            if (_model.TemplarbitFetchInterval == 0)
            {
                _model.TemplarbitFetchInterval = 5;
            }
            if (String.IsNullOrEmpty(_model.TemplarbitApiUrl))
            {
                _model.TemplarbitApiUrl = "https://api.templarbit.com/v1";
            }
        }
    }
}
