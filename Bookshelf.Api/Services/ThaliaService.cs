using System.Diagnostics.CodeAnalysis;
using Bookshelf.DataModel;
using HtmlAgilityPack;

namespace Bookshelf.Api.Services;

public class ThaliaService(HttpClient client) : IExternalBookService
{
    private readonly string _baseUrl = "https://www.thalia.de";

    private record BookParseResult(string Name);
    
    public async Task<Book?> GetBookFromIsbn(string isbn, bool saveThumbnails = false)
    {
        if (await SearchThalia(isbn) is not { } searchPage || GetProductUrl(searchPage) is not { } productUrl)
            return null;

        if (await GetProductDetails(productUrl) is not { } productDetails)
            return null;
        
        return null;
    }

    private async Task<string?> SearchThalia(string isbn)
    {
        var response = await client.GetAsync($"{_baseUrl}/suche?sq={isbn}");
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadAsStringAsync();
    }

    private string? GetProductUrl(string searchResult)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(searchResult);
        
        var results = doc.DocumentNode.SelectSingleNode("//ul[@class='tm-produktliste']");
        if (results.ChildNodes.Count != 1)
            return null;

        return results.SelectSingleNode("//a[@class='tm-produkt-link']").Attributes["href"].Value;
    }

    private async Task<string?> GetProductDetails(string url)
    {
        var response = await client.GetAsync($"{_baseUrl}{url}");
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadAsStringAsync();
    }

    private bool TryParseProductDetails(string details, [NotNullWhen(true)] out BookParseResult? result)
    {
        result = null;
        
        var doc = new HtmlDocument();
        doc.LoadHtml(details);

        var contentWrapper = doc.DocumentNode.SelectSingleNode("//div[@class='content-wrapper']");
        
        return false;
    }
}