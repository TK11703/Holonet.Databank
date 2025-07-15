using Holonet.Databank.Infrastructure.Repositories;

namespace Holonet.Databank.Application.Services;
public class GenericDBService(IGenericDBRepository genericDBRepository) : IGenericDBService
{
    private readonly IGenericDBRepository _genericDBRepository = genericDBRepository;
    public async Task<bool> DBReady()
    {
        return await _genericDBRepository.DBReady();
    }
}
