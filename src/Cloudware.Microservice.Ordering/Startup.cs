using Cloudware.Microservice.Ordering.Application.Command;
using Cloudware.Microservice.Ordering.Application.Event;
using Cloudware.Microservice.Ordering.Application.Event.Stock;
using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Microservice.Ordering.Infrastructure.Services;
using Cloudware.Utilities.BusTools;
using Cloudware.Utilities.Common.Extension;
using Cloudware.Utilities.Common.Setting;
using Cloudware.Utilities.Configure.Microservice.Middleware;
using Cloudware.Utilities.Configure.Microservice.Services;
using Cloudware.Utilities.Contract.Idp;
using MassTransit;
using MassTransit.Mediator;

//using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering
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
            var currentDomain = AppDomain.CurrentDomain;
            var executingAssembly = Assembly.GetExecutingAssembly();
            Assembly contract = typeof(UserCreatedEvent).Assembly;
            var assembly = AppDomain.CurrentDomain.GetAssemblies();
            _ = assembly.Append(contract);
           //var settings=services.AddConfig<AppSettings>(Configuration);
            services.AddScoped<IUnitOfWork, OrderingWriteDbContext>();
            services.AddScoped<IPaginationService, PaginationService>();
            services.AddScoped<IOrderService, OrderService>();

            var setting = services.AddClwServices<AppSettings>(Configuration, currentDomain, executingAssembly, System.AppContext.BaseDirectory, 2);
            services.AddDbContext<OrderingWriteDbContext>(options => options.UseSqlServer(setting.ConnectionStrings.MsSql));

            #region MyRegion
            //services.AddControllers();

            //services.Configure<OrderingSettings>(Configuration.GetSection("ConnectionStrings"));
            //services.AddScoped<IUnitOfWork, OrderingWriteDbContext>();
            //// services.AddMediatR(typeof(Startup).Assembly);
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //        builder => builder
            //            .AllowAnyMethod()
            //            .AllowAnyHeader()
            //            .SetIsOriginAllowed((host) => true)
            //            .AllowCredentials());
            //});


            //services.AddDbContext<OrderingWriteDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("OrderingConnectionString")));

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cloudware.Microservice.Ordering", Version = "v1" });
            //    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name }.xml";
            //    var filePath = Path.Combine(System.AppContext.BaseDirectory, xmlFile);
            //    c.IncludeXmlComments(filePath);
            //});
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddScoped<IPaginationService, PaginationService>();

            //var assembly = AppDomain.CurrentDomain.GetAssemblies();
            //services.AddMassTransitMediatorClw<IMediatorConsumerType, IRequestType>(assembly);



            //services.AddMassTransitClw<IBusConsumerType, IBusMultiConsumerType, IBusRequestType>(Configuration.GetValue<string>("MassTransit:Host"),
            //    Configuration.GetValue<string>("MassTransit:Username"), Configuration.GetValue<string>("MassTransit:Password"), assembly);

            //#region comment
            ////services.AddMediator(x =>
            ////{
            ////    //consumers
            ////    //x.AddConsumersFromNamespaceContaining<OrderCreatedConsumer>();
            ////    //x.AddConsumersFromNamespaceContaining<GetProductByIdQueryConsumer>();
            ////    x.AddConsumersFromNamespaceContaining<StockConfirmedEventConsumer>();
            ////    x.AddConsumersFromNamespaceContaining<CreateOrderCommandConsumer>();
            ////    x.AddConsumersFromNamespaceContaining<OrderCreatedEventConsumer>();

            ////    //requestClients

            ////    x.AddRequestClient<CreateOrderCommand>();
            ////    //x.AddRequestClient<GetAllProductsByFiltersQuery>();
            ////    //x.AddRequestClient<GetHomePageDataQuery>();
            ////    //x.AddRequestClient<GetAllSearchFiltersQuery>();
            ////    //x.AddRequestClient<SearchProductsQuery>();
            ////    //x.AddRequestClient<CreateProductCommand>();
            ////    //x.AddRequestClient<EditProductCommand>();

            ////});
            ////services.AddMassTransit(x =>
            ////{
            ////    x.AddConsumersFromNamespaceContaining<StockConfirmedEventConsumer>();
            ////    x.AddConsumersFromNamespaceContaining<OrderCreatedEventConsumer>();


            ////    x.SetKebabCaseEndpointNameFormatter();

            ////    x.UsingRabbitMq((context, cfg) =>
            ////    {
            ////        cfg.Host(new Uri(Configuration.GetValue<string>("MassTransit:Host")), d =>
            ////        {
            ////            d.Username(Configuration.GetValue<string>("MassTransit:Username"));
            ////            d.Password(Configuration.GetValue<string>("MassTransit:Password"));
            ////        });

            ////        cfg.ConfigureEndpoints(context);
            ////    });
            ////}); 
            //#endregion
            //services.AddMassTransitHostedService();
            #endregion
            services.AddSingleton<IOrderingReadDbContext, OrderingReadDbContext>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseClw("Cloudware.Microservice.Ordering v1");
           
        }
    }
}
 #region comment
            //app.UseDeveloperExceptionPage();
            //app.UseSwagger();
            //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cloudware.Microservice.Ordering v1"));



            //app.UseRouting();

            //app.UseAuthorization();
            //app.UseCors("CorsPolicy");
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //}); 
            #endregion