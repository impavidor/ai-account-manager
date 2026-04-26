using AccountManager.Application;
using AccountManager.Common.Errors;
using AccountManager.Domain.Administration;
using AccountManager.Domain.SelfService;
using AccountManager.WebAPI;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.Application.Tests;

[TestFixture]
public class ResultMapperTests
{
    private ResultMapper _mapper = null!;

    [SetUp]
    public void SetUp() => _mapper = new ResultMapper();

    [Test]
    public void Map_OkResult_Returns204()
    {
        var result = _mapper.Map(CommandResult.Ok());

        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public void Map_CreatedResult_Returns201()
    {
        var id = Guid.NewGuid();

        var result = _mapper.Map(CommandResult.Created(id));

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(201);
    }

    [Test]
    public void MapError_ContactNotFoundError_Returns404()
    {
        var result = _mapper.MapError(new ContactNotFoundError(new ContactId(Guid.NewGuid())));

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(404);
    }

    [Test]
    public void MapError_InvalidStatusTransitionError_Returns409()
    {
        var result = _mapper.MapError(new InvalidStatusTransitionError("Pending", "Deleted"));

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(409);
    }

    [Test]
    public void MapError_SelfActionForbiddenError_Returns403()
    {
        var result = _mapper.MapError(new SelfActionForbiddenError());

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(403);
    }

    [Test]
    public void MapError_InvalidNpiError_Returns422()
    {
        var result = _mapper.MapError(new InvalidNpiError("bad"));

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(422);
    }

    [Test]
    public void MapError_InvalidProviderNameError_Returns422()
    {
        var result = _mapper.MapError(new InvalidProviderNameError("first name", ""));

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(422);
    }

    [Test]
    public void MapError_UnknownError_Returns500()
    {
        var result = _mapper.MapError(new UnknownTestError());

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(500);
    }
}

file sealed record UnknownTestError() : Error("unknown");
