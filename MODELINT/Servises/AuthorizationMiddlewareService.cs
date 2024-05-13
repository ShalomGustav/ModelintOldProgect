using CoreLibrary.Interfaces;
using CoreLibrary.Models;
using System.Collections.Generic;

namespace MODELINT.Servises
{
    public class AuthorizationMiddlewareService : IAuthorizationMiddlewareService
    {
        private readonly ILoggerService _loggerService;
        readonly Dictionary<string, string> Credentions;

        public AuthorizationMiddlewareService(ILoggerService loggerService)
        {
            _loggerService = loggerService;
            Credentions = new Dictionary<string, string>();
            Credentions.Add("Maksim", "384a18ae-effb-4c31-ad80-5701337b3a6d");
        }

        public bool CanAccess(Request request)
        {

            if (request.Credentials.Client == null)
            {
                _loggerService.Logger.Error($"Авторизация не удалась, данные пришли пустые. Middleware: {nameof(AuthorizationMiddlewareService)}");
                return false;
            }

            var userName = request.Credentials.Client.UserName;
            var key = request.Credentials.Client.Key;

            if (!Credentions.ContainsKey(userName))//метод есть в коллекции, работает по стрингу так как метод такой, внутрь передаем значение , если внутри переменной есть совпадение то вернет true, иначе false
            {
                _loggerService.Logger.Error($"Авторизация не удалась, юзера {userName}, нет в базе. Middleware: {nameof(AuthorizationMiddlewareService)} ");
                return false;
            }

            if (Credentions[userName] != key)
            {
                _loggerService.Logger.Error($"Авторизация не удалась, связка логин: {userName}, ключ: {key} не верна.  Middleware: {nameof(AuthorizationMiddlewareService)} ");
                return false;
            }

            return true;
        }
    }
}
