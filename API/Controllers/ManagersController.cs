using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ManagersController : BaseApiController
    {
        private readonly IManagerService managerService;

        public ManagersController(IManagerService managerService)
        {
            this.managerService = managerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManagerDto>>> GetManagers()
        {
            var managers = await managerService.GetManagers();

            return Ok(managers);
        }

    }
}
