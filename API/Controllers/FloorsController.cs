using BusinessLogic.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class FloorsController : BaseApiController
    {
        private readonly IFloorService floorService;

        public FloorsController(IFloorService floorService)
        {
            this.floorService = floorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Floor>>> GetFloors()
        {
            var floors = await floorService.GetFloorsAsync();
            return Ok(floors);
        }

        [HttpDelete]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult> DeleteFloor(int id)
        {
            await floorService.DeleteAsync(id);
            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<Floor>> CreateFloor(int floorNumber)
        {
            var floor = await floorService.CreateAsync(floorNumber);
            return Ok(floor);
        }
    }
}
