using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UserController(BicycleSharingContext context) : ControllerBase
{
    private const int DefaultRentalTime = 1;

    [HttpPut("Rental/{id:guid}/{time:int?}")]
    public async Task<IActionResult> RentalAsync(Guid id, int time = DefaultRentalTime)
    {
        var dbBicycle = context.Bicycles.FirstOrDefault(x => x.BicycleId == id);

        if (dbBicycle is null)
        {
            return BadRequest();
        }

        var startDateTime = DateTime.Now;
        var expireDateTime = startDateTime + TimeSpan.FromMinutes(time);

        dbBicycle.StartRentalTime = startDateTime;
        dbBicycle.ExpireRentalTime = expireDateTime;

        return await context.SaveChangesAsync().ConfigureAwait(false) > 0
            ? Accepted()
            : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("Return/{officeId:guid}/{bicycleId:guid}")]
    public async Task<IActionResult> ReturnAsync(Guid officeId, Guid bicycleId)
    {
        var dbBicycle = context.Bicycles.FirstOrDefault(x => x.BicycleId == bicycleId);

        if (dbBicycle is null)
        {
            return BadRequest();
        }

        dbBicycle.RentalOfficeId = officeId;
        dbBicycle.StartRentalTime = default;
        dbBicycle.ExpireRentalTime = DateTime.Now;

        return await context.SaveChangesAsync().ConfigureAwait(false) > 0
            ? Accepted()
            : StatusCode(StatusCodes.Status500InternalServerError);
    }
}