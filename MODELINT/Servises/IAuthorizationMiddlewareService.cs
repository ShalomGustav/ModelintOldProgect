using CoreLibrary.Models;

namespace MODELINT.Servises
{
    public interface IAuthorizationMiddlewareService
    {
        bool CanAccess(Request request);
    }
}
