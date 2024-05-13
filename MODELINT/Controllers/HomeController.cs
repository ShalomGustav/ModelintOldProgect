using CoreLibrary.Models;
using CoreLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using MODELINT.Models;
using MODELINT.Servises;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using YcLibrary.Exceptions;
using YcLibrary.Models.Extentions;

namespace MODELINT.Controllers
{
    public class HomeController : Controller
    {

        private readonly IValuteService _valuteService;
        private readonly YcCredentials _ycCredentials;
        private readonly AuthorizationService _authorizationService;
        private readonly Client _client;

        public HomeController(IValuteService valuteService, YcCredentials ycCredentials, AuthorizationService authorizationService, Client client)
        {
            _valuteService = valuteService;
            _ycCredentials = ycCredentials;
            _authorizationService = authorizationService;

            _authorizationService.SetFileName("accounts.json");
            _client = client;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("valute")]
        public async Task<IActionResult> Valute()
        {
            var valutes = new List<ValuteEnum>();
            valutes.Add(ValuteEnum.USD);
            valutes.Add(ValuteEnum.EUR);
            valutes.Add(ValuteEnum.KRW);
            valutes.Add(ValuteEnum.AMD);
            valutes.Add(ValuteEnum.AUD);

            var responselocal = await _valuteService.ValuteAsync(valutes);
            return View("Valutes", responselocal);
        }


        [HttpGet("crypts")]
        public async Task<IActionResult> Crypts(CryptsEnum crypts = CryptsEnum.NULL)
        {
            var cryptsList = new List<CryptsEnum>();
            switch (crypts)
            {
                case CryptsEnum.NULL:
                    cryptsList.Add(CryptsEnum.BTC);
                    cryptsList.Add(CryptsEnum.ETH);
                    break;
                case CryptsEnum.BTC:
                    cryptsList.Add(CryptsEnum.BTC);
                    break;
                case CryptsEnum.ETH:
                    cryptsList.Add(CryptsEnum.ETH);
                    break;
                default:
                    cryptsList.Add(CryptsEnum.BTC);
                    cryptsList.Add(CryptsEnum.ETH);
                    break;
            }

            var responselocal = await _valuteService.ListCryptsAsync(cryptsList);
            return View("Crypts", responselocal);
        }

        [HttpGet("valute-usd")]
        public async Task<IActionResult> ConvertValuteUSD(double rub)
        {
            var response = await _valuteService.ResponceConvertAsync(rub, ValuteEnum.USD);           
            return View("ValutesConvert", response);
        }

        [HttpGet("valute-eur")]
        public async Task<IActionResult> ConvertValueEUR(double rub)
        {
            var response = await _valuteService.ResponceConvertAsync(rub, ValuteEnum.EUR);
            return View("ValutesConvert", response);
        }

        [HttpGet("valute-krw")]
        public async Task<IActionResult> ConvertValueKRW(double rub)
        {
            var response = await _valuteService.ResponceConvertAsync(rub, ValuteEnum.KRW);
            return View("ValutesConvert", response);
        }

        [HttpGet("valute-amd")]
        public async Task<IActionResult> ConvertValueAMD(double rub)
        {
            var response = await _valuteService.ResponceConvertAsync(rub, ValuteEnum.AMD);
            return View("ValutesConvert", response);
        }

        [HttpGet("valute-aud")]
        public async Task<IActionResult> ConvertValueAUD(double rub)
        {
            var response = await _valuteService.ResponceConvertAsync(rub, ValuteEnum.AUD);
            return View("ValutesConvert", response);
        }

        [HttpGet("crypts-convert")]
        public async Task<IActionResult> ConvertCryptsRUB()
        {
            var response = await _valuteService.ConvertCryptAsync(CryptsEnum.BTC);
            return View("ValutesConvert", response);
        }

        [HttpGet("cryptseth-convert")]
        public async Task<IActionResult> ConvertCryptsETH()
        {
            var response = await _valuteService.ConvertCryptAsync(CryptsEnum.ETH);
            return View("ValutesConvert", response);
        }

        [HttpGet("translate")]
        public IActionResult Translate()
        {
            return View("Translate");
        }

        [HttpGet("translate-result")]
        public async Task<IActionResult> Translate(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                var error = new ResponseTranslateView
                {
                    Error = "Поле не должно быть пустым"

                };
                return View("Translate", error);
            }
            string content;

            var response = new ResponseTranslateView();

            if (string.IsNullOrEmpty(_ycCredentials.YcTranslateBearer))
            {
                return View("Authorization");
            }

            var ycTranslate = new YcTranslateService(_ycCredentials);

            try
            {
                content = await ycTranslate.Translate(text);

                response = new ResponseTranslateView
                {
                    Text = content
                };
            }
            catch(YcResponceException e)
            {
                response = new ResponseTranslateView
                {
                    Code = e.Code,
                    Message = e.Description
                };
            }
            
            return View("Translate", response);
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View("Register");
        }

        [HttpGet("register-result")]
        public IActionResult RegisterResult(string firstName, string lastName, string userName, string password, string folderId, string bearerToken)
        {
            var response = new ResponseAuthorization();

            if (string.IsNullOrEmpty(firstName) ||
                string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(userName) ||
                string.IsNullOrEmpty(password))
            {
                response.IsEmpty = true;

                return View("register", response);
            }

            var credentials = new Credentials
            {
                Client = new Client
                {
                    FirstName = firstName,
                    LastName = lastName,
                    UserName = userName,
                    Password = password,
                    ycCredentials = new YcCredentials(),
                }
            };

            if (!string.IsNullOrEmpty(folderId))
            {
                credentials.Client.ycCredentials.YcFolderId = folderId;
            }

            if (!string.IsNullOrEmpty(bearerToken))
            {
                credentials.Client.ycCredentials.YcTranslateBearer = bearerToken;
            }

            var request = _authorizationService.Register(credentials);

            if (request.Success)
            {
                response.Success = true;

                return View("Register", response);
            }

            response.Success = false;

            return View("Register", response);
        }


        [HttpGet("account")]
        public IActionResult Account()
        {
            if (string.IsNullOrEmpty(_client.UserName) || string.IsNullOrEmpty(_client.Key))
            {
                return View("authorization");
            }

            return View("account", _client);
        }

        [HttpGet("account-update")]
        public IActionResult AccountUpdate(string firstName, string lastName, string userName, string password, string folderId, string bearerToken)
        {
            var response = new ResponseAuthorization();

            if (string.IsNullOrEmpty(firstName) ||
                string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(userName) ||
                string.IsNullOrEmpty(password))
            {
                response.IsEmpty = true;

                return View("account_update", response);
            }

            var credentials = new Credentials
            {
                Client = new Client
                {
                    FirstName = firstName,
                    LastName = lastName,
                    UserName = userName,
                    Password = password,
                    ycCredentials = new YcCredentials(),
                }
            };

            if (!string.IsNullOrEmpty(folderId))
            {
                credentials.Client.ycCredentials.YcFolderId = folderId;
            }

            if (!string.IsNullOrEmpty(bearerToken))
            {
                credentials.Client.ycCredentials.YcTranslateBearer = bearerToken;
            }

            var request = _authorizationService.UpdateAccount(credentials);

            if (request.Success)
            {
                response.Success = true;

                _client.SetClient(request.Client);

                return View("account_update", response);
            }

            response.Success = false;

            return View("account_update", response);
        }


        [HttpGet("authorization")]
        public IActionResult Authorization()
        {
            return View("authorization");
        }

        [HttpGet("authorization-result")]
        public IActionResult AuthorizationResult(string userName, string password)
        {
            var response = new ResponseAuthorization();

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                response.IsEmpty = true;
                response.Success = false;

                return View("Authorization", response);
            }

            var credentials = new Credentials
            {
                Client = new Client
                {
                    UserName = userName,
                    Password = password
                }
            };

            var request = _authorizationService.Authorization(credentials);

            if (request.Success)
            {
                _client.SetClient(request.Client);

                response.Success = true;

                return View("Authorization", response);
            }

            //_ycCredentials.YcTranslateBearer = bearer;

            response.Success = false;

            return View("Authorization", response);
        }
    }

}




