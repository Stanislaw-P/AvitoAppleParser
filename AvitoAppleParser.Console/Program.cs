using AvitoAppleParser.Console.Services;

var parser = new AvitoParsingService();
string url = "https://www.avito.ru/rossiya/noutbuki/apple-ASgCAQICAUCo5A0U9Nlm?user=1";

Console.WriteLine("Начало парсинга...");
var products = await parser.ParsePageAsync(url);

Console.WriteLine($"Найдено объявлений: {products.Count}");
