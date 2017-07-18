# log4net.AspNetCore
log4net extensions for AspNetCore

## Usage:
```csharp
using log4net.AspNetCore;
```

```csharp
public void Configure(IApplicationBuilder app,
    IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    var cfg = new Log4NetConfig
    {
      MinLevel = "DEBUG",
      ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"
    };
    
    loggerFactory.AddLog4Net(cfg);

    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    app.UseMvcWithDefaultRoute();
}
```
