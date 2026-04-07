using CoreBankingSystem.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace CoreBankingSystem.API.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result, int successStatusCode = 200)
    {
        if (!result.IsSuccess)
            return new BadRequestObjectResult(new { error = result.Error });

        return successStatusCode switch
        {
            201 => new ObjectResult(result.Value) { StatusCode = 201 },
            _ => new OkObjectResult(result.Value)
        };
    }

    public static IActionResult ToActionResult(this Result result)
    {
        if (!result.IsSuccess)
            return new BadRequestObjectResult(new { error = result.Error });

        return new NoContentResult();
    }
}