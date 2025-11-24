namespace Monitor_economic.Domain.Models
{
    public class CotacaoModel
    {
        public decimal cotacaoCompra { get; set; }
        public decimal cotacaoVenda { get; set; }
        public string dataHoraCotacao { get; set; } = string.Empty;
    }

}