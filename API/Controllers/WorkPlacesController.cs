using BusinessLogic.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class WorkPlacesController : BaseApiController
    {
        private readonly IWorkPlaceService workPlaceService;

        public WorkPlacesController(IWorkPlaceService workPlaceService)
        {
            this.workPlaceService = workPlaceService;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<WorkPlace>>> CreateWorkPlaces(int quantity, int floorId)
        {
            var workPlaces = await workPlaceService.CreateWorkPlacesAsync(quantity, floorId);
            return Ok(workPlaces);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteWorkPlace(int id)
        {
            await workPlaceService.DeleteWorkPlaceAsync(id);
            return Ok();
        }
    }
}
