using AvitoAppleParser.Db.Models;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace AvitoAppleParser.Console.Services
{
    public class AvitoParsingService
    {
        readonly HttpClient _httpClient;

        public AvitoParsingService()
        {
            _httpClient = new HttpClient();

            // Нужно чтобы avito не блокировал запрос, прячемся под пользователя
            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36");
        }

        public async Task<List<Product>> ParsePageAsync(string url)
        {
            var html = await _httpClient.GetStringAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var products = new List<Product>();

            // Находим все блоки объявлений
            var items = doc.DocumentNode.SelectNodes("//div[@data-marker='item']");

            if (items == null)
                return products;

            foreach (var item in items)
            {
                try
                {
                    var product = new Product
                    {
                        ExternalId = item.GetAttributeValue("data-item-id", ""),
                        // Ссылка на объявление (полный URL)
                        Url = GetFullUrl(item.SelectSingleNode(".//a[@data-marker='item-title']")?.GetAttributeValue("href", "") ?? ""),
                        Title = item.SelectSingleNode(".//a[@data-marker='item-title']")?.InnerText.Trim() ?? "",
                        // Цена
                        Price = ExtractPrice(item),
                        // Город
                        City = ExtractCity(item),
                        // Фото
                        PhotoUrl = ExtractPhotoUrl(item),
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    if (product.ExternalId != null)
                        products.Add(product);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Ошибка парсинга: {ex.Message}");
                }
            }

            return products;
        }

        private string GetFullUrl(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
                return "";
            if (relativeUrl.StartsWith("http"))
                return relativeUrl;
            return $"https://www.avito.ru{relativeUrl}";
        }

        private decimal ExtractPrice(HtmlNode item)
        {
            // Способ 1: через meta тег
            var metaPrice = item.SelectSingleNode(".//meta[@itemprop='price']");
            if (metaPrice != null)
            {
                var priceStr = metaPrice.GetAttributeValue("content", "");
                if (decimal.TryParse(priceStr, out var price))
                    return price;
            }

            // Способ 2: через текст с ценой
            var priceText = item.SelectSingleNode(".//span[@data-marker='item-price-value']");
            if (priceText != null)
            {
                var cleanPrice = Regex.Replace(priceText.InnerText, "[^0-9]", "");
                if (decimal.TryParse(cleanPrice, out var price))
                    return price;
            }

            return 0;
        }

        private string ExtractCity(HtmlNode item)
        {
            var cityNode = item.SelectSingleNode(".//div[@data-marker='item-location']//span");
            if (cityNode != null)
            {
                var text = cityNode.InnerText.Trim();
                // Убираем иконку, оставляем только текст города
                var match = Regex.Match(text, @"[А-Яа-я].+$");
                return match.Success ? match.Value.Trim() : text;
            }
            return "";
        }

        private string ExtractPhotoUrl(HtmlNode item)
        {
            // Берём первое фото из слайдера
            var img = item.SelectSingleNode(".//img[@class='photo-slider-image-cD891']");
            if (img != null)
            {
                var src = img.GetAttributeValue("src", "");
                // Очищаем URL от параметров
                var cleanUrl = src.Split('?')[0];
                return cleanUrl;
            }
            return "";
        }
    }
}
