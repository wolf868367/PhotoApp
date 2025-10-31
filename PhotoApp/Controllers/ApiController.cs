using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoApp.Models;
using PhotoApp.Services;

namespace PhotoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiController : ControllerBase
    {
        protected readonly DataService _dataService;
        protected readonly AuthService _authService;

        public ApiController(DataService dataService, AuthService authService)
        {
            _dataService = dataService;
            _authService = authService;
        }

        protected bool CheckAdminAccess()
        {
            return _authService.IsAdmin;
        }

        protected bool CheckOperatorAccess()
        {
            return _authService.IsOperator || _authService.IsAdmin;
        }
    }
}