﻿using System.Reflection;
using Amazon.DynamoDBv2;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using App.Api.Shared.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Api.DeleteEvent;

public class Startup
{
  public static IConfiguration Configuration = FunctionConfiguration.Get();

  public static IServiceCollection ConfigureServices()
  {
    var services = new ServiceCollection();

    AWSSDKHandler.RegisterXRayForAllServices();
#if DEBUG
    AWSXRayRecorder.Instance.XRayOptions.IsXRayTracingDisabled = true;
#endif

    var dynamoDbConfig = Configuration.GetSection("DynamoDB");

    services
      .AddDefaultAWSOptions(Configuration.GetAWSOptions())
      .AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient());
    services.AddMediatR(Assembly.GetCallingAssembly());
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));

    services.AddTransient<IResponse, Response>();
    services.AddTransient<IRequestHandler<DeleteEventCommand.Command, IResponse>, DeleteEventCommand.CommandHandler>();
    services.AddTransient<IValidator<DeleteEventCommand.Command>, DeleteEventCommand.CommandValidator>();

    return services;
  }
}
