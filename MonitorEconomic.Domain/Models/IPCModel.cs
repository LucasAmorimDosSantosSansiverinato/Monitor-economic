using Microsoft.OpenApi.MicrosoftExtensions;
using Monitor_economic._2_Monitor_economic.Application.Exception;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Data;

namespace Monitor_economic.Monitor_economic.Domain.Models
{
    [Table("ipc")]
    public class IPCModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; private set; }

        private DateTime _data;
        

        [Column("data")]
        public DateTime data
        {
            get => _data;
            private set
            {
                if (value > DateTime.UtcNow)
                    throw new DomainException("A data não pode ser futura.");

                _data = value;
            }
        }

        [Column("valor")]
        public decimal valor { get; private set; }

        public IPCModel(DateTime data, decimal valor)
        {
            this.data = data;
            this.valor = valor;
        }

        protected IPCModel() {}

        public void AtualizarValor(decimal novoValor)
        {
            valor = novoValor;
        }

        public void AtualizarData(DateTime novaData)
        {
            data = novaData;
        }

    }
}
