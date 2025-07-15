
namespace Holonet.Databank.Application.Services;

public interface IGenericDBService
{
    Task<bool> DBReady();
}