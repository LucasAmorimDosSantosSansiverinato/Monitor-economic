using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Monitor_economic._2_Monitor_economic.Application.Entity
{ 
        [Table("ipc")]
        public class IPCEntity : BaseModel
        {
            [PrimaryKey("id", false)]
            public int Id { get; set; }

            [Column("data")]
            public DateTime Data { get; set; }

            [Column("valor")]
            public decimal Valor { get; set; }
        }
    
}
