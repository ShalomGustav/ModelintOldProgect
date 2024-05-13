using CoreLibrary.Models;
using CoreLibrary.Services;
using CoreLibrary.Utils;
using Microsoft.AspNetCore.Mvc;

namespace MODELINT.Controllers
{
    [Route("Authorization")]

    public class AuthorizationController : Controller
    {
        private readonly ValidationService _validationService;
        private readonly AuthorizationService _authorizationService;
        public AuthorizationController(ValidationService validationService, AuthorizationService authorizationService)
        {
            _validationService = validationService;
            _authorizationService = authorizationService;
        }

        [HttpGet("regmock")]
        public bool RegMock()
        {
            _authorizationService.SetFileName("test.json");

            var credentials = new Credentials
            {
                Client = new Client
                {
                    FirstName = "maxfjfjfh",
                    LastName = "fargegeget",
                    UserName = "kfkegegegegegegegefkf",
                    Password = "383gegeg736"
                }
            };

            var responce = _authorizationService.Register(credentials);

            return false;
        }

        [HttpGet("author")]

        public bool Author()
        {
            _authorizationService.SetFileName("test.json");

            var credentials = new Credentials
            {
                Client = new Client
                {
                    UserName = "kfkegeg",
                    Password = "некорректный пароль"
                }
            };

            var responce = _authorizationService.Authorization(credentials);

            return false;
        }

        public IActionResult Authorization(Credentials credentials)
        {
            if (!_validationService.ValideAuthorizationRequest(credentials))
            {

            }

            var responce = _authorizationService.Authorization(credentials);

            return View();
        }

        public IActionResult Register(Credentials credentials)
        {
            if (!_validationService.ValideRegisterRequest(credentials))
            {

            }

            var responce = _authorizationService.Register(credentials);

            return View();
        }

    }
}
