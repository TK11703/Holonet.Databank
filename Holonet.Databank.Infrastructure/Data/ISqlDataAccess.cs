
namespace Holonet.Databank.Infrastructure.Data;

public interface ISqlDataAccess
{
	Task<IEnumerable<T>> LoadDataAsync<T, U>(string storedProcedure, U parameters, string connectionId = "DefaultConnection");
	int SaveData<T>(string storedProcedure, T parameters, string connectionId = "DefaultConnection");
	Task<int> SaveDataAsync<T>(string storedProcedure, T parameters, string connectionId = "DefaultConnection");
}