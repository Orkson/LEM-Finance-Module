using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/currency")]
    public class CurrencyController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public CurrencyController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetExchangeRate(string code)
        {
            var url = $"https://api.nbp.pl/api/exchangerates/rates/a/{code}/?format=json";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound($"Nie znaleziono kodu waluty: {code}.");
            }

            var json = await response.Content.ReadAsStringAsync();
            var nbpResponse = JsonSerializer.Deserialize<NbpResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var rate = nbpResponse.Rates.FirstOrDefault()?.Mid;
            if (rate == null)
                return NotFound("Exchange rate not found.");

            return Ok(new { Currency = code.ToUpper(), Rate = rate });
        }

        private class NbpResponse
        {
            public List<NbpRate> Rates { get; set; }
        }

        private class NbpRate
        {
            public decimal Mid { get; set; }
        }
    }
}
