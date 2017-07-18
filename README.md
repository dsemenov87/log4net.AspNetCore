# log4net.AspNetCore
log4net extensions for AspNetCore

## Usage:

```csharp
public void Configure(IApplicationBuilder app,
    IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    loggerFactory.AddLog4Net();

    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    app.UseMvcWithDefaultRoute();
}
```
