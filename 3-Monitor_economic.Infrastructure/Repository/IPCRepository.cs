using Monitor_economic._1_Monitor_econimic.Domain.Interfaces.IRepository;
using Monitor_economic._2_Monitor_economic.Application.Entity;
using Monitor_economic._2_Monitor_economic.Application.Mapper;
using Monitor_economic.Monitor_economic.Domain.Models;



namespace Monitor_economic._3_Monitor_economic.Infrastructure.Repository
{
    public class IPCRepository : IIPCRepository
    {
        private readonly Supabase.Client _client;

        public IPCRepository(Supabase.Client client)
        {
            _client = client;
        }

        public async Task salvarAsync(IPCModel ipcModel)
        {
            IPCEntity entity = IPCMapper.ToEntity(ipcModel);
            await _client.From<IPCEntity>().Insert(entity);
        }

        public async Task<List<IPCModel>> obterTodosAsync()
        {
            var response = await _client.From<IPCEntity>().Get();

            List<IPCModel> lista = new List<IPCModel>();

            foreach (var entity in response.Models)
            {
                IPCModel model = IPCMapper.ToDomain(entity);
                lista.Add(model);
            }

            return lista;
        }
    }
}
