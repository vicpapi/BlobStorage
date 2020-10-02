using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Document.Core.Interfaces;
using Document.Infrastructure.Repository; 
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Document
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
            services.AddControllersWithViews();

            services.AddSingleton<IBlobRepository<CloudBlockBlob>, BlobRepository>(service =>
            {
                return new BlobRepository(Configuration["ConnectionStrings:DocumentStoreConnection"], 
                                          Configuration["ConnectionStrings:DocumentStoreName"]);
            });

            services.AddScoped<ILog4NetRepository, Log4NetRepository>();
            services.AddScoped<ISeriLogRepository, SeriLogRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "text/html";

                        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            LogException(error.Error, context);
                            context.Response.Redirect("/Home/Error");
                        }
                    });
                });
                //app.UseBrowserLink();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();


                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "text/html";

                        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            LogException(error.Error, context);
                            await context.Response.WriteAsync("<h2>An error has occured in the website.</h2>").ConfigureAwait(false);
                        }
                    });
                });
            }

            
            app.UseHttpsRedirection();
            app.UseStaticFiles(); 
            app.UseRouting();
            

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Document}/{action=Index}/{id?}");
            });
        }



        private void LogException(Exception error, Microsoft.AspNetCore.Http.HttpContext context)
        {
            try
            {
    //            var connectionStr = Configuration.GetConnectionString("DefaultConnection");
    //            using (var connection = new System.Data.SqlClient.SqlConnection(connectionStr))
    //            {
    //                var command = connection.CreateCommand();
    //                command.CommandText = @"INSERT INTO ErrorLog (ErrorId, Application, Host, Type, Source, Method, Message, StackTrace, [User],  TimeUtc)
    //VALUES (@ErrorId, @Application, @Host, @Type, @Source, @Method, @Message, @StackTrace, @User,  @TimeUtc)";
    //                connection.Open();

    //                if (error.InnerException != null)
    //                    error = error.InnerException;

    //                command.Parameters.AddWithValue("@ErrorId", Guid.Parse(context.TraceIdentifier));
    //                command.Parameters.AddWithValue("@Application", this.GetType().Namespace);
    //                command.Parameters.AddWithValue("@Host", Environment.MachineName);
    //                command.Parameters.AddWithValue("@Type", error.GetType().FullName);
    //                command.Parameters.AddWithValue("@Source", error.Source);
    //                command.Parameters.AddWithValue("@Method", context.Request.Method);
    //                command.Parameters.AddWithValue("@Message", error.Message);
    //                command.Parameters.AddWithValue("@StackTrace", error.StackTrace);
    //                var user = context.User.Identity?.Name;
    //                if (user == null)
    //                    command.Parameters.AddWithValue("@User", DBNull.Value);
    //                else
    //                    command.Parameters.AddWithValue("@User", user);
    //                command.Parameters.AddWithValue("@TimeUtc", DateTime.Now);

    //                command.ExecuteNonQuery();
    //            }
            }
            catch(Exception exp) { }
        }
    }

}
