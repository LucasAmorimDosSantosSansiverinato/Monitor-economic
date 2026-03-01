using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MonitorEconomic.Infra.Data.Models;

[Table("ipc")]
public class IPCModel : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("data")]
    public DateTime Data { get; set; }

    [Column("valor")]
    public decimal Valor { get; set; }
}