using CoreLibrary.Interfaces;
using CoreLibrary.Models;
using CoreLibrary.ServerModels;
using CoreLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MODELINT.Servises
{
    public class ValuteService : IValuteService
    {
        public string Url { get; set; } = "https://www.cbr-xml-daily.ru/daily_json.js";
        public string Urlbtc { get; set; } = "https://api.cryptonator.com/api/ticker/btc-usd";
        public string Urleth { get; set; } = "https://api.cryptonator.com/api/ticker/eth-usd";

        private readonly ValidationService _validationService; // в библиотеке переименовать
        private readonly IExternalValuteService _externalValuteService;

        public ValuteService(ValidationService validationService, IExternalValuteService externalValuteService)
        {
            _validationService = validationService;
            _externalValuteService = externalValuteService;
        }

        #region Block converts methods
        public async Task<ResponceConvert> ResponceConvertAsync(double rub, ValuteEnum valuteEnum)
        {
            var content = await _externalValuteService.GetValuteAsync(Url);

            switch (valuteEnum)
            {
                case ValuteEnum.USD:
                    var valueUsd = content.Valute.USD.Value;
                    var resultConvert = rub / valueUsd;
                    return ConvertValutesToResponce(content, resultConvert, valuteEnum);

                case ValuteEnum.EUR:
                    var valueEur = content.Valute.EUR.Value;
                    var EUR = rub / valueEur;
                    return ConvertValutesToResponce(content, EUR, valuteEnum);

                case ValuteEnum.KRW:
                    var valueKrw = content.Valute.EUR.Value;
                    var KRW = rub / valueKrw;
                    return ConvertValutesToResponce(content, KRW, valuteEnum);

                case ValuteEnum.AMD:
                    var valueAmd = content.Valute.AMD.Value;
                    var AMD = rub / valueAmd;
                    return ConvertValutesToResponce(content, AMD, valuteEnum);

                case ValuteEnum.AUD:
                    var valueAud = content.Valute.AUD.Value;
                    var AUD = rub / valueAud;
                    return ConvertValutesToResponce(content, AUD, valuteEnum);
            }

            return null;
        }

        public async Task<ResponceConvert> ConvertCryptAsync(CryptsEnum crypts)
        {
            ResponceLocal content;

            switch (crypts)
            {
                case CryptsEnum.BTC:
                    content = await CryptsAsync(CryptsEnum.BTC);
                    break;

                case CryptsEnum.ETH:
                    content = await CryptsAsync(CryptsEnum.ETH);
                    break;

                default:
                    content = await CryptsAsync(CryptsEnum.BTC);
                    break;
            }

            var valuteList = new List<ValuteEnum>();
            valuteList.Add(ValuteEnum.USD);
            var valutes = await ValuteAsync(valuteList);

            var valueUsd = double.Parse(valutes.Valutesloc.FirstOrDefault().Value);
            var valueBtc = double.Parse(content.Valutesloc.FirstOrDefault().Value, CultureInfo.InvariantCulture);

            var response = new ResponceConvert
            {
                Value = content.Valutesloc.FirstOrDefault().Value,
                Data = content.Data,
                CharCode = "rub",
                NameValue = content.Valutesloc.FirstOrDefault().NameValue,
                Result = (valueBtc * valueUsd).ToString()
            };

            return response;
        }

        private ResponceConvert ConvertValutesToResponce(Responce content, double resultConvert, ValuteEnum valuteEnum)
        {
            var response = new ResponceConvert
            {
                Data = content.Date,
                Result = resultConvert.ToString()
            };

            switch (valuteEnum)
            {
                case ValuteEnum.USD:
                    response.CharCode = content.Valute.USD.CharCode;
                    response.NameValue = content.Valute.USD.Name;
                    response.Value = content.Valute.USD.Value.ToString();
                    break;

                case ValuteEnum.EUR:
                    response.CharCode = content.Valute.EUR.CharCode;
                    response.NameValue = content.Valute.EUR.Name;
                    response.Value = content.Valute.EUR.Value.ToString();
                    break;

                case ValuteEnum.KRW:
                    response.CharCode = content.Valute.KRW.CharCode;
                    response.NameValue = content.Valute.KRW.Name;
                    response.Value = content.Valute.KRW.Value.ToString();
                    break;

                case ValuteEnum.AMD:
                    response.CharCode = content.Valute.AMD.CharCode;
                    response.NameValue = content.Valute.AMD.Name;
                    response.Value = content.Valute.AMD.Value.ToString();
                    break;

                case ValuteEnum.AUD:
                    response.CharCode = content.Valute.AUD.CharCode;
                    response.NameValue = content.Valute.AUD.Name;
                    response.Value = content.Valute.AUD.Value.ToString();
                    break;
            }

            return response;
        }
        #endregion

        #region Block internal gets methods
        public async Task<ResponceLocal> CryptsAsync(CryptsEnum crypts)
        {
            ResponceCrypt content;

            switch (crypts)
            {
                case CryptsEnum.BTC:
                    content = await _externalValuteService.GetCryptsAsync(Urlbtc);
                    break;

                case CryptsEnum.ETH:
                    content = await _externalValuteService.GetCryptsAsync(Urleth);
                    break;

                default:
                    content = await _externalValuteService.GetCryptsAsync(Urlbtc);
                    break;
            }

            var date = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var datebts = date.AddSeconds(content.Timestamp);
            var responselocal = new ResponceLocal
            {
                Data = datebts.ToLocalTime().ToString(),
                Valutesloc = new List<ValutesLocal>()
            };

            responselocal.Valutesloc.Add(new ValutesLocal
            {
                NameValue = content.Ticker.Base,
                Value = content.Ticker.Price,
                Target = content.Ticker.Target,
            });

            return responselocal;
        }

        public async Task<ResponceLocal> ValuteAsync(List<ValuteEnum> valutes)
        {
            var content = await _externalValuteService.GetValuteAsync(Url);

            if (!_validationService.ValideLocalResponce(new ResponceLocal {Errors = content.Errors}, valutes))
            {
                return new ResponceLocal()
                {
                    Errors = content.Errors
                };
            }


            var responselocal = new ResponceLocal
            {
                Data = content.Date,
                Valutesloc = new List<ValutesLocal>()
            };

            foreach (var valute in valutes)
            {
                switch (valute)
                {
                    case ValuteEnum.USD:

                        responselocal.Valutesloc.Add(new ValutesLocal
                        {
                            NameValue = content.Valute.USD.Name,
                            Value = content.Valute.USD.Value.ToString(),
                            CharCode = content.Valute.USD.CharCode
                        });

                        break;

                    case ValuteEnum.EUR:
                        responselocal.Valutesloc.Add(new ValutesLocal
                        {
                            NameValue = content.Valute.EUR.Name,
                            Value = content.Valute.EUR.Value.ToString(),
                            CharCode = content.Valute.EUR.CharCode
                        });

                        break;

                    case ValuteEnum.KRW:
                        responselocal.Valutesloc.Add(new ValutesLocal
                        {
                            NameValue = content.Valute.KRW.Name,
                            Value = content.Valute.KRW.Value.ToString(),
                            CharCode = content.Valute.KRW.CharCode
                        });

                        break;

                    case ValuteEnum.AMD:
                        responselocal.Valutesloc.Add(new ValutesLocal
                        {
                            NameValue = content.Valute.AMD.Name,
                            Value = content.Valute.AMD.Value.ToString(),
                            CharCode = content.Valute.AMD.CharCode
                        });

                        break;

                    case ValuteEnum.AUD:
                        responselocal.Valutesloc.Add(new ValutesLocal
                        {
                            NameValue = content.Valute.AUD.Name,
                            Value = content.Valute.AUD.Value.ToString(),
                            CharCode = content.Valute.AUD.CharCode
                        });

                        break;
                }
            }

            return responselocal;
        }
        public async Task<ResponceLocal> ListCryptsAsync(List<CryptsEnum> crypts)
        {
            ResponceCrypt content = null;

            var responselocal = new ResponceLocal
            {
                Valutesloc = new List<ValutesLocal>(),
                Errors = new List<Error>()
               
            };

            foreach (var crypt in crypts)
            {
                switch (crypt)
                {
                    case CryptsEnum.BTC:
                        content = await _externalValuteService.GetCryptsAsync(Urlbtc);
                        break;

                    case CryptsEnum.ETH:
                        content = await _externalValuteService.GetCryptsAsync(Urleth);
                        break;

                    default:
                        content = await _externalValuteService.GetCryptsAsync(Urlbtc);
                        break;
                }

                if (_validationService.ValideLocalResponce(responselocal, null, crypts))
                {
                    responselocal.Valutesloc.Add(new ValutesLocal
                    {
                        NameValue = content.Ticker.Base,
                        Value = content.Ticker.Price,
                        Target = content.Ticker.Target,
                    });
                }
                else
                {
                    responselocal.Errors.Add(new Error
                    {
                        Cod = content.Errors.FirstOrDefault().Cod,
                        Message = content.Errors.FirstOrDefault().Message,
                        Target = crypt.ToString()
                    });
                }

            }
            var date = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var dateCrypts = content != null ? date.AddSeconds(content.Timestamp) : date;

            responselocal.Data = dateCrypts.ToString();

            

            return responselocal;
        }

        Task<Responce> IValuteService.GetValuteAsync()
        {
            throw new NotImplementedException();
        }

        Task<ResponceConvert> IValuteService.ResponceConvertAsync(double rub, ValuteEnum valuteEnum)
        {
            throw new NotImplementedException();
        }

        Task<ResponceConvert> IValuteService.ConvertCryptAsync(CryptsEnum crypts)
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
