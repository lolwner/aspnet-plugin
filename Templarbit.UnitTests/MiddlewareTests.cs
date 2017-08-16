using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Threading;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Templarbit.Core;

namespace Templarbit.UnitTests
{
    public class MiddlewareTests
    {
        private readonly int _timeOut = 0;
        public MiddlewareTests()
        {

        }
        private static int endTestCount = 0;
        private void IsEnd()
        {
            endTestCount++;
            if (endTestCount == 6)
            {
                TemplarbitMiddleware.Digest = false;
            }
        }
        [Fact]
        public void ResponseTest()
        {
            var result = true;
            var manualEvent = new ManualResetEvent(false);
            manualEvent.Reset();
            try
            {
                var thread = new Thread(async () =>
                {
                    var logger = new TestLogger();
                    var startup = new Startup();
                    startup.Logger = logger;
                    startup.TemplarbitApiToken = "b46b86317e73d423ba8a802f33837b46ce0ba64a0bca55dcba5bcf4bc5cd4a01";
                    startup.TemplarbitPropertyId = "cccb512f-0aa0-4931-89f4-76cda8602a56";
                    startup.TemplarbitApiUrl = "https://api.templarbit.com/v1";
                    var builder = new WebHostBuilder()
                             .ConfigureServices(services =>
                             {
                                 services.AddSingleton<Startup>(startup);
                             });

                    var server = new TestServer(builder.UseStartup<Startup>());
                    var client = server.CreateClient();
                    var response = await client.GetAsync("/");
                    response.EnsureSuccessStatusCode();

                    System.Threading.Thread.Sleep(1000);
                    var responseString = await response.Content.ReadAsStringAsync();

                    // Assert
                    if (logger.Logs.Count != 0)
                    {
                        Assert.True(false);
                    }
                    Assert.True(true);
                    server.Dispose();
                    manualEvent.Set();
                });
                thread.Start();
                manualEvent.WaitOne();
                thread.Abort();
                Assert.True(result);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
            finally
            {
                IsEnd();
            }
        }
        [Fact]
        public void return_500ResponseTest()
        {
            var result = true;
            var manualEvent = new ManualResetEvent(false);
            manualEvent.Reset();
            try
            {
                var thread = new Thread(async () =>
                {
                    var logger = new TestLogger();
                    var startup = new Startup();
                    startup.Logger = logger;
                    startup.TemplarbitApiToken = "return_500";
                    startup.TemplarbitPropertyId = "571f4f43-ad7a-415d-894b-1a1f234899db";
                    startup.TemplarbitApiUrl = "https://api.tb-stag-01.net/v1";
                    var builder = new WebHostBuilder()
                             .ConfigureServices(services =>
                             {
                                 services.AddSingleton<Startup>(startup);
                             });
                    var server = new TestServer(builder.UseStartup<Startup>());
                    var client = server.CreateClient();
                    var response = await client.GetAsync("/");
                    response.EnsureSuccessStatusCode();
                    if (logger.Logs.Count != 0)
                    {
                        result = false;
                    }
                    if (logger.Logs.Count > 0 && "\nTemplarbitMiddlewareError: Fetch failed, returned status InternalServerError\n" != logger.Logs[logger.Logs.Count - 1])
                    {
                        result = false;
                    }
                    server.Dispose();
                    manualEvent.Set();
                });
                thread.Start();
                manualEvent.WaitOne();
                thread.Abort();
                Assert.True(result);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
            finally
            {
                IsEnd();
            }
        }
        [Fact]
        public void return_validResponseTest()
        {
            var result = true;
            var manualEvent = new ManualResetEvent(false);
            manualEvent.Reset();
            try
            {
                var thread = new Thread(async () =>
                {
                    var logger = new TestLogger();
                    var startup = new Startup();
                    startup.Logger = logger;
                    startup.TemplarbitApiToken = "return_valid";
                    startup.TemplarbitPropertyId = "571f4f43-ad7a-415d-894b-1a1f234899db";
                    startup.TemplarbitApiUrl = "https://api.tb-stag-01.net/v1";

                    var builder = new WebHostBuilder()
                              .ConfigureServices(services =>
                              {
                                  services.AddSingleton<Startup>(startup);
                              });
                    var server = new TestServer(builder.UseStartup<Startup>());
                    var client = server.CreateClient();
                    System.Threading.Thread.Sleep(3000);
                    var response = await client.GetAsync("/");
                    response.EnsureSuccessStatusCode();
                    if (logger.Logs.Count != 0)
                    {
                        Assert.True(false, logger.Logs[0]);
                    }
                    Assert.True(true);
                    server.Dispose();
                    manualEvent.Set();
                });
                thread.Start();
                manualEvent.WaitOne();
                thread.Abort();
                Assert.True(result);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
            finally
            {
                IsEnd();
            }
        }
        [Fact]
        public void return_invalidResponseTest()
        {
            var result = true;
            var manualEvent = new ManualResetEvent(false);
            manualEvent.Reset();
            try
            {
                var thread = new Thread(async () =>
                {
                    var logger = new TestLogger();
                    var startup = new Startup();
                    startup.Logger = logger;
                    startup.TemplarbitApiToken = "return_invalid";
                    startup.TemplarbitPropertyId = "571f4f43-ad7a-415d-894b-1a1f234899db";
                    startup.TemplarbitApiUrl = "https://api.tb-stag-01.net/v1";
                    var builder = new WebHostBuilder()
                              .ConfigureServices(services =>
                              {
                                  services.AddSingleton<Startup>(startup);
                              });
                    var server = new TestServer(builder.UseStartup<Startup>());
                    var client = server.CreateClient();
                    System.Threading.Thread.Sleep(4000);
                    var response = await client.GetAsync("/");
                    response.EnsureSuccessStatusCode();
                    Thread.Sleep(_timeOut);
                    if (logger.Logs.Count == 0)
                    {
                        Assert.True(false);
                    }
                    Assert.Equal("\nTemplarbitMiddlewareError: Fetch successful, but Content-Security-Policy was empty.\n", logger.Logs[logger.Logs.Count - 1]);
                    Assert.True(true);
                    server.Dispose();
                    manualEvent.Set();
                });
                thread.Start();
                manualEvent.WaitOne();
                thread.Abort();
                Assert.True(result);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
            finally
            {
                IsEnd();
            }
        }
        [Fact]
        public void return_errorResponseTest()
        {
            var result = true;
            var manualEvent = new ManualResetEvent(false);
            manualEvent.Reset();
            try
            {
                var thread = new Thread(async () =>
                {
                    var logger = new TestLogger();
                    var startup = new Startup();
                    startup.Logger = logger;
                    startup.TemplarbitApiToken = "return_error";
                    startup.TemplarbitPropertyId = "571f4f43-ad7a-415d-894b-1a1f234899db";
                    startup.TemplarbitApiUrl = "https://api.tb-stag-01.net/v1";
                    var builder = new WebHostBuilder()
                             .ConfigureServices(services =>
                             {
                                 services.AddSingleton<Startup>(startup);
                             });
                    var server = new TestServer(builder.UseStartup<Startup>());
                    var client = server.CreateClient();
                    System.Threading.Thread.Sleep(1000);
                    var response = await client.GetAsync("/");
                    response.EnsureSuccessStatusCode();
                    if (logger.Logs.Count == 0)
                    {
                        Assert.True(false);
                    }
                    Assert.True(logger.Logs[logger.Logs.Count - 1].StartsWith("\nTemplarbitMiddlewareError: Fetch failed: "));
                    server.Dispose();
                    manualEvent.Set();
                });
                thread.Start();
                manualEvent.WaitOne();
                thread.Abort();
                Assert.True(result);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
            finally
            {
                IsEnd();
            }
        }
        [Fact]
        public void return_401ResponseTest()
        {
            var result = true;
            var manualEvent = new ManualResetEvent(false);
            manualEvent.Reset();
            try
            {
                var thread = new Thread(async () =>
                {
                    var logger = new TestLogger();
                    var startup = new Startup();
                    startup.Logger = logger;
                    startup.TemplarbitApiToken = "return_401";
                    startup.TemplarbitPropertyId = "571f4f43-ad7a-415d-894b-1a1f234899db";
                    startup.TemplarbitApiUrl = "https://api.tb-stag-01.net/v1";
                    var builder = new WebHostBuilder()
                            .ConfigureServices(services =>
                            {
                                services.AddSingleton<Startup>(startup);
                            });
                    var server = new TestServer(builder.UseStartup<Startup>());
                    var client = server.CreateClient();
                    System.Threading.Thread.Sleep(6000);
                    var response = await client.GetAsync("/");
                    response.EnsureSuccessStatusCode();
                    Thread.Sleep(_timeOut);
                    if (logger.Logs.Count == 0)
                    {
                        Assert.True(false);
                    }
                    Assert.Equal("\nTemplarbitMiddlewareError: invalid templarbit_api_token and/or templarbit_property_id\n", logger.Logs[logger.Logs.Count - 1]);
                    Assert.True(true);
                    server.Dispose();
                    manualEvent.Set();
                });
                thread.Start();
                manualEvent.WaitOne();
                thread.Abort();
                Assert.True(result);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
            finally
            {
                IsEnd();
            }
        }
    }
}
