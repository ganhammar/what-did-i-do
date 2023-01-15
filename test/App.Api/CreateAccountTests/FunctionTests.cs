﻿using System.Net;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using App.Api.CreateAccount;
using App.Api.Shared.Models;
using FluentValidation.Results;
using TestBase;
using Xunit;

namespace CreateAccountTests;

[Collection(Constants.DatabaseCollection)]
public class FunctionTests
{
  [Fact]
  public async Task Should_ReturnAccount_When_InputIsValid()
  {
    var function = new Function();
    var context = new TestLambdaContext();
    var data = new CreateAccountCommand.Command
    {
      Name = "Testing Testing",
    };
    var request = new APIGatewayProxyRequest
    {
      HttpMethod = HttpMethod.Post.Method,
      Body = JsonSerializer.Serialize(data),
    };
    var response = await function.FunctionHandler(request, context);

    Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);

    var body = JsonSerializer.Deserialize<AccountDto>(response.Body);

    Assert.NotNull(body);
    Assert.Equal(data.Name, body!.Name);
    Assert.NotNull(body!.Id);
  }

  [Fact]
  public async Task Should_ReturnBadRequest_When_NameIsNotSet()
  {
    var function = new Function();
    var context = new TestLambdaContext();
    var data = new CreateAccountCommand.Command();
    var request = new APIGatewayProxyRequest
    {
      HttpMethod = HttpMethod.Post.Method,
      Body = JsonSerializer.Serialize(data),
    };
    var response = await function.FunctionHandler(request, context);

    Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);

    var errors = JsonSerializer.Deserialize<List<ValidationFailure>>(response.Body);

    Assert.NotNull(errors);
    Assert.Contains(errors, error => error.PropertyName == nameof(CreateAccountCommand.Command.Name)
      && error.ErrorCode == "NotEmptyValidator");
  }
}