using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MonitorEconomic.Infra.Data.Models;

[Table("ipc")]
public class IPCModel : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("data")]
    public new DateTime Data { get; set; }

    [Column("valor")]
    public new decimal Valor { get; set; }

    public IPCModel() { }
    public IPCModel(DateTime data, decimal valor)
       
    {
        Data = data;
        Valor = valor;
    }

}