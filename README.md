# ASP. NET Core Templarbit Plugin

[![Build Status](https://travis-ci.org/templarbit/aspnet-plugin.svg?branch=master)](https://travis-ci.org/templarbit/aspnet-plugin)

# Usage
1. Create account at https://app.templarbit.com/signup
2. Login with your credentials
3. Create first property
4. Automatic approach: Copy Plugin API token and Property ID and install Templarbit Plugin for ASP.NET Core
5. Templarbit continuously updates the generated Content-Security-Policy header based on the traffic patterns we see.

# Installation
Install Nuget package from https://preview.nuget.org/packages/templarbit
Or
```sh
PM> Install-Package Templarbit -Version 0.1.4
  ```
  
Add following properties to app.config
```sh
<appSettings>
    <add key="TB_API_KEY" value="Your API Token" />
    <add key="TB_PROPERTY_ID" value="Your Property ID" />
 </appSettings>
  ```
  
  In Startup.cs of your Core app include the following in Configure method:
```sh
app.UseTemplarbit(new TemplarbitMiddlewareModel()
            {
                TemplarbitApiToken = ConfigurationManager.AppSettings["TB_API_KEY"],
                TemplarbitPropertyId = ConfigurationManager.AppSettings["TB_PROPERTY_ID"]
            }, new Logger());
```
You can additionally set fetch interval (default 5 seconds) and API URL (Default to https://api.templarbit.com/v1) by adding properties to app.config
  ```sh
  <appSettings>
    <add key="TB_POLL_INTERVAL" value="Interval in seconds" />
    <add key="TB_API_URL" value="API URL" />
 </appSettings>
  ```
  Note that Property ID is a UUID 36 characters long and Token is random string a-zA-Z0-9, max 64 characters.
