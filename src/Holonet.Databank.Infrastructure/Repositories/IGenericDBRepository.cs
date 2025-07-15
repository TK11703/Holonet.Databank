
namespace Holonet.Databank.Infrastructure.Repositories;

public interface IGenericDBRepository
{
    Task<bool> DBReady();
}