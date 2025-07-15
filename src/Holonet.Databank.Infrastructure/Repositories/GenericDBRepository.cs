
using Holonet.Databank.Infrastructure.Data;

namespace Holonet.Databank.Infrastructure.Repositories;
public class GenericDBRepository(ISqlDataAccess dataAccess) : IGenericDBRepository
{
    private readonly ISqlDataAccess _dataAccess = dataAccess;

    public async Task<bool> DBReady()
    {        
        return await _dataAccess.ExecuteHealthCheckAsync();
    }
}
