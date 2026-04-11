using Microsoft.Extensions.Options;
using MonitorEconomic.Domain.Enums;
using MonitorEconomic.Infra.Data.Services;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace MonitorEconomic.Tests.Infra;

public class BacenHttpServiceTests
{
    [Fact]
    public async Task ObterBacenAsync_ParsesResponseUsingInvariantCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("pt-BR");
            CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");

            var handler = new FakeHttpMessageHandler("""
                [{"data":"11/04/2026","valor":"0.65"}]
                """);
            var httpClient = new HttpClient(handler);
            var options = Options.Create(new BacenApiOptions
            {
                SeriesUrlTemplate = "https://fake/{codigo}?dataInicial={dataInicial}&dataFinal={dataFinal}"
            });
            var service = new BacenHttpService(httpClient, options, new[] { new IpcBacenSerieStrategy() });

            var result = await service.obterBacenAsync(BacenSerie.Ipc, "10/04/2026", "11/04/2026");

            Assert.Single(result);
            Assert.Equal(new DateTime(2026, 4, 11), result[0].Data);
            Assert.Equal(0.65m, result[0].Valor);
            Assert.Equal(BacenSerie.Ipc, result[0].Serie);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _content;

        public FakeHttpMessageHandler(string content)
        {
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_content, Encoding.UTF8, "application/json")
            };

            return Task.FromResult(response);
        }
    }
}