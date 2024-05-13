using CoreLibrary.Models;
using CoreLibrary.ServerModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MODELINT.Servises
{
    public interface IValuteService
    {
        Task<ResponceLocal> ValuteAsync(List<ValuteEnum> valutes);
        Task<ResponceLocal> CryptsAsync(CryptsEnum crypts);
        Task<ResponceLocal> ListCryptsAsync(List<CryptsEnum> crypts);
        Task<Responce> GetValuteAsync();
        Task<ResponceConvert> ResponceConvertAsync(double rub, ValuteEnum valuteEnum);
        Task<ResponceConvert> ConvertCryptAsync(CryptsEnum crypts);
    }
}
