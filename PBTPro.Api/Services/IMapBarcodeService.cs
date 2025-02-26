using System.Threading.Tasks;

namespace PBTPro.Api.Services
{
    public interface IMapBarcodeService
    {
        Task<byte[]> FetchGoogleMapsImageAsync(decimal latitude, decimal longitude);
        byte[] GenerateBarcode(string text, int? barcodeWidth = 100, int? barcodeHeight = 20);
    }
}
