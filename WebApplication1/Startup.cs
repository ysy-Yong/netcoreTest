﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore;

namespace WebApplication1
{
    public class Startup
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// configureservices方法时用来把services(各种服务，例identity,ef,mvc等等包括第三方的，
        /// 或者自己写的)加入(register)到container(asp.netcore的容器)中去，并配置这些services.
        /// 这个container是用来进行dependency injection的(依赖注入). 
        /// 所有注入的services(此外还包括一些框架已经注册好的services) 在以后写代码的时候, 都可以将它们注入(inject)进去. 
        /// 例如上面的Configure方法的参数, app, env, loggerFactory都是注入进去的services.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //注册mvc到container
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //netcore默认提供的是json格式，
            //配置输出为xml格式
            //services.AddMvc(options =>
            //{
            //    options.ReturnHttpNotAcceptable = true;
            //    options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            //});

            #region  Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version="v1",
                    Title="netcoreTest",
                    Description="instruction doc",
                    //TermsOfService=new Uri("None"),
                    //Contact=new Microsoft.OpenApi.Models.OpenApiContact { Name="nerCore Test",Email="2395188058@qq.com", Url =new Uri("https://www.jianshu.com/u/94102b59cc2a") }
                });

                //导出
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "TestAPI.xml");
                c.IncludeXmlComments(xmlPath,true);//第二个参数是controller的注释，默认为false

                //model层的xml文件
                var xmlModelPath = Path.Combine(basePath,"TestModel.xml");
                c.IncludeXmlComments(xmlModelPath);

                #region  Token绑定到ConfigureServices
                //添加header验证消息
                //var security = new Dictionary<string, IEnumerable<string>> { { "TestAPI", new string[] { } }, };
                ////c.AddSecurityRequirement(security);

                //c.AddSecurityDefinition("TestAPI", new ApiSchema
                //{
                //    Description="",
                    
                //    //Name="",
                //    //In="",
                //    Type=""
                //});
                #endregion
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //判断是否时环境变量
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            //处理异常的middleware
            //使用mvc中间件middleware

            #region  Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                var apiName = "Values";
                var version = "v1";
                c.SwaggerEndpoint($"/swagger/{version}/swagger.json",$"{apiName} {version}");
               // c.SwaggerEndpoint("/swagger/v1/swagger.json", "NetCoreTestWebAPI");
                c.RoutePrefix = string.Empty;//设置启动浏览器为swagger页面
            });
            #endregion

            app.UseMvc();
        }
        //public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        //{
        //    if (env.IsDevelopment())
        //    {
        //        // 在开发环境中，使用异常页面，这样可以暴露错误堆栈信息，所以不要放在生产环境。
        //        app.UseDeveloperExceptionPage();
        //    }
        //    else
        //    {
        //        app.UseExceptionHandler("/Error");
        //        // 在非开发环境中，使用HTTP严格安全传输(or HSTS) 对于保护web安全是非常重要的。
        //        // 强制实施 HTTPS 在 ASP.NET Core，配合 app.UseHttpsRedirection
        //        //app.UseHsts();

        //    }

        //    #region Swagger
        //    app.UseSwagger();
        //    app.UseSwaggerUI(c =>
        //    {
        //        //之前是写死的
        //        //c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1");
        //        //c.RoutePrefix = "";//路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉

        //        //根据版本名称倒序 遍历展示
        //        typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
        //        {                    // 注意这个 ApiName 和 要和上边 ConfigureServices 中配置swagger的name要大小写一致，具体查看我的blog.core源码
        //            c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiName} {version}");
        //        });
        //    });
        //    #endregion

        //    #region Authen
        //    //app.UseMiddleware<JwtTokenAuth>();//注意此授权方法已经放弃，请使用下边的官方验证方法。但是如果你还想传User的全局变量，还是可以继续使用中间件
        //    app.UseAuthentication();
        //    #endregion

        //    #region CORS
        //    //跨域第二种方法，使用策略，详细策略信息在ConfigureService中
        //    app.UseCors("LimitRequests");//将 CORS 中间件添加到 web 应用程序管线中, 以允许跨域请求。


        //    //跨域第一种版本，请要ConfigureService中配置服务 services.AddCors();
        //    //    app.UseCors(options => options.WithOrigins("http://localhost:8021").AllowAnyHeader()
        //    //.AllowAnyMethod()); 
        //    #endregion

        //    // 跳转https
        //    app.UseHttpsRedirection();
        //    // 使用静态文件
        //    app.UseStaticFiles();
        //    // 使用cookie
        //    app.UseCookiePolicy();
        //    // 返回错误码
        //    app.UseStatusCodePages();//把错误码返回前台，比如是404

        //    app.UseMvc();
        //}
    }
}
