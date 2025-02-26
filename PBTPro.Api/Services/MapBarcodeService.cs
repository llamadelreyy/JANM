using ZXing;
using SkiaSharp;
using System.IO;
using ZXing.SkiaSharp;

namespace PBTPro.Api.Services
{
    public class MapBarcodeService : IMapBarcodeService
    {
        private readonly string _googleMapsApiKey;

        public MapBarcodeService(IConfiguration configuration)
        {
            _googleMapsApiKey = configuration["GoogleMap:ApiKey"];
        }

        public async Task<byte[]> FetchGoogleMapsImageAsync(decimal latitude, decimal longitude)
        {
            try
            {
                var url = $"https://maps.googleapis.com/maps/api/staticmap?center={latitude},{longitude}&zoom=15&size=600x400&markers=color:red%7Clabel:S%7C{latitude},{longitude}&key={_googleMapsApiKey}";

                using (var httpClient = new HttpClient())
                {
                    return await httpClient.GetByteArrayAsync(url);
                }
            }catch(Exception ex)
            {
                throw;
            }
        }

        public byte[] GenerateBarcode(string text, int? barcodeWidth = 100, int? barcodeHeight = 20)
        {
            try
            {
                var barcodeWriter = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Margin = 0,
                        Width = barcodeWidth.Value,
                        Height = barcodeHeight.Value,
                        PureBarcode = true
                    }
                };

                var barcodeBitmap = barcodeWriter.Write(text);
                // if need label extent heigth for the label
                //using (var skiaBitmap = new SKBitmap(barcodeBitmap.Width, barcodeBitmap.Height+20))
                using (var skiaBitmap = new SKBitmap(barcodeBitmap.Width, barcodeBitmap.Height))
                {
                    using (var canvas = new SKCanvas(skiaBitmap))
                    {
                        canvas.Clear(SKColors.White);

                        using (var paint = new SKPaint())
                        {
                            paint.IsAntialias = false;
                            paint.Style = SKPaintStyle.Fill;
                            paint.Color = SKColors.Black;

                            for (int x = 0; x < barcodeBitmap.Width; x++)
                            {
                                for (int y = 0; y < barcodeBitmap.Height; y++)
                                {
                                    if (barcodeBitmap.GetPixel(x, y).Red == 0)
                                    {
                                        canvas.DrawPoint(x, y, paint);
                                    }
                                }
                            }
                        }
                        //Lable Below Barcode
                        //using (var textPaint = new SKPaint())
                        //{
                        //    textPaint.Color = SKColors.Black;
                        //    textPaint.TextSize = 10;
                        //    textPaint.IsAntialias = true;
                        //    textPaint.TextAlign = SKTextAlign.Center;
                        //    textPaint.Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold);

                        //    float xPos = barcodeBitmap.Width / 2f;
                        //    float yPos = barcodeBitmap.Height + 10;

                        //    canvas.DrawText(text, xPos, yPos, textPaint);
                        //}
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        skiaBitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }


}
