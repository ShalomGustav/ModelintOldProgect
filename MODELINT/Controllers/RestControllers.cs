using CoreLibrary.Interfaces;
using CoreLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using MODELINT.Models;
using MODELINT.Servises;
using System.Collections.Generic;
using System.Threading.Tasks;
using YcLibrary.Models;
using YcLibrary.Models.Extentions;

namespace MODELINT.Controllers
{
    [Route("rest")]
    public class RestControllers : Controller
    {
        private readonly string YcTranslateUrl = "https://translate.api.cloud.yandex.net/translate/v2/translate";
        private readonly string YcTranslateBearer = "t1.9euelZqOkovNysvIz8jOnJTHjI2PkO3rnpWam52Qm52MnpXPmZ3MicyXmMvl8_c-CCdn-e8TThVy_t3z9342JGf57xNOFXL-.EWXCs721BjBkLO4Jd2h4U9P0nxxddk9ZX-K6PcrrT1gDFoqRsRCHOm4rAA1ekMyEXx99jFH2kZ4l_adBTl4YAQ";

        private readonly IValuteService _valuteService;
        private readonly ILoggerService _loggerService;
        private readonly IAuthorizationMiddlewareService _authorizationMiddlewareService;
        private readonly YcCredentials _ycCredentials;
        public RestControllers(IValuteService valuteService, ILoggerService loggerService, IAuthorizationMiddlewareService authorizationMiddlewareService, YcCredentials ycCredentials)
        {
            _valuteService = valuteService;
            _loggerService = loggerService;
            _authorizationMiddlewareService = authorizationMiddlewareService;
            _ycCredentials = ycCredentials;
        }

        [HttpGet("index")]
        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        [Route("translate")]
        public async Task<ResponseTranslateView> Translate([FromBody] Request request)
        {
            if (string.IsNullOrEmpty(request.Text))
            {
                var error = new ResponseTranslateView
                {
                    Error = "Поле не должно быть пустым"

                };
                return error;
            }

            var ycTranslate = new YcTranslateService(_ycCredentials);

            //var ycTranslate = new YcTranslateService(YcTranslateUrl,YcTranslateBearer);

            var content = await ycTranslate.Translate(request.Text);

            var responce = new ResponseTranslateView
            {
                Text = content
            };

            return responce;
        }

                
        [HttpPost]
        [Route("valutes")]
        public async Task<ActionResult<ResponceLocal>> GetValutes([FromBody] Request request)//переменная валютес реализует класс реквест , лежат в ней валюты которые надо вернуть пользователю, просто new будет пустой
        {
            if (!_authorizationMiddlewareService.CanAccess(request))
            {
                return new ResponceLocal()
                {
                    Errors = new List<Error>()
                    {
                        new Error
                        {
                            Cod = "1",
                            Message = "Авторизация не удалась",
                            Target = " "
                        }
                    }
                };
            }

            _loggerService.Logger.Info($"UserName: {request.Credentials.Client.UserName}, {request.Credentials.Client.Key}");
            //_loggerService.Logger.Info("test");пример как написать слово тест в файл
            var response = await _valuteService.ValuteAsync(request.Valutes);
            
            return response;
        }

        [HttpPost]
        [Route("listcrypts")]
        public async Task<ActionResult<ResponceLocal>> GetCrypts([FromBody] Request request)
        {
            if (!_authorizationMiddlewareService.CanAccess(request))
            {
                return new ResponceLocal()
                {
                    Errors = new List<Error>()
                    {
                        new Error
                        {
                            Cod = "1",
                            Message = "Авторизация не удалась",
                            Target = " "
                        }
                    }
                };
            }
            _loggerService.Logger.Info($"UserName: {request.Credentials.Client.UserName}, {request.Credentials.Client.Key}");
            
            var response = await _valuteService.ListCryptsAsync(request.Crypts);

            return response;

        }

        
    }

}
