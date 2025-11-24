namespace Monitor_economic.Monitor_economic.Application.Dto
{
    public class ItemCotacaoDto
    {
        public decimal cotacaoCompra { get; set; }
        public decimal cotacaoVenda { get; set; }
        public string dataHoraCotacao { get; set; } = string.Empty;
    }
}
