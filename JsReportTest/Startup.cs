using jsreport.AspNetCore;
using jsreport.Binary;
using jsreport.Local;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JsReportTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            #region jsreport
            services.AddJsReport(
                //为jsReport建一个临时文件，可以避免一些意外问题 要确保提供的文件路径存在
                new LocalReporting().TempDirectory(AppDomain.CurrentDomain.BaseDirectory + "Files\\jsReportTemp")   
                .UseBinary(JsReportBinary.GetBinary())
                //自定义端口  默认5488端口
                .Configure((cfg) => { cfg.HttpPort = 8148; return cfg; })
                //在启动一个jsreport程序前 结束正在运行的程序
                .KillRunningJsReportProcesses()
                .AsUtility()
                .Create());
            #endregion
            //mvc支持
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
