using Dapper;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace Holonet.Databank.Infrastructure.Data;
public class SqlDataAccess : ISqlDataAccess
{
	private readonly IConfiguration _configuration;

	public SqlDataAccess(IConfiguration configuration)
	{
		_configuration = configuration;
	}

    public async Task<bool> ExecuteHealthCheckAsync(string connectionId = "DefaultConnection")
    {
        try
        {
            using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));
            await connection.ExecuteScalarAsync("SELECT 1");
            return true;
        }
        catch (DbException e)
        {
            throw new InvalidOperationException("Execute health check failed.", e);
        }
    }

    public async Task<IEnumerable<T>> LoadDataAsync<T, U>(string storedProcedure, U parameters, string connectionId = "DefaultConnection")
	{
		using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));

		return await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
	}

	/// <summary>
	/// Executes a SPROC with parameters and returns the number of rows affected
	/// </summary>
	/// <typeparam name="T">Object being stored</typeparam>
	/// <param name="storedProcedure">Name of SPROC to execute</param>
	/// <param name="parameters">Generic parameters to use in the SPROC</param>
	/// <param name="connectionId"></param>
	/// <returns>The number of rows affected</returns>
	public async Task<int> SaveDataAsync<T>(string storedProcedure, T parameters, string connectionId = "DefaultConnection")
	{
		using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));

		return await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
	}

	/// <summary>
	/// Executes a SPROC with parameters and returns the number of rows affected
	/// </summary>
	/// <typeparam name="T">Object being stored</typeparam>
	/// <param name="storedProcedure">Name of SPROC to execute</param>
	/// <param name="parameters">Generic parameters to use in the SPROC</param>
	/// <param name="connectionId"></param>
	/// <returns>The number of rows affected</returns>
	public int SaveData<T>(string storedProcedure, T parameters, string connectionId = "DefaultConnection")
	{
		using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));

		return connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
	}

    //public async Task<bool> TransactionalExecuteAsync<T>(IEnumerable<string> storedProcedure, IEnumerable<DynamicParameters> parameters, string connectionId = "DefaultConnection")
    //{
    //	bool completed = false;
    //	if (!storedProcedure.Any())
    //	{
    //		throw new ArgumentException("No stored procedures provided", nameof(storedProcedure));
    //	}
    //	if (!parameters.Any())
    //	{
    //		throw new ArgumentException("No dynamic parameters provided", nameof(parameters));
    //	}

    //	using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));
    //	connection.Open();
    //	using (var transaction = connection.BeginTransaction())
    //	{
    //		for (int i = 0; i < storedProcedure.Count() - 1; i++)
    //		{
    //			await connection.ExecuteAsync(storedProcedure.ElementAt(i), parameters.ElementAt(i), commandType: CommandType.StoredProcedure, transaction: transaction);
    //		}

    //		transaction.Commit();
    //		completed = true;
    //	}
    //	return completed;
    //}

    //public bool TransactionalExecute<T>(IEnumerable<string> storedProcedure, IEnumerable<DynamicParameters> parameters, string connectionId = "DefaultConnection")
    //{
    //	bool completed = false;
    //	if (!storedProcedure.Any())
    //	{
    //		throw new ArgumentException("No stored procedures provided", nameof(storedProcedure));
    //	}
    //	if(!parameters.Any())
    //	{
    //		throw new ArgumentException("No dynamic parameters provided", nameof(parameters));
    //	}
    //	using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));
    //	connection.Open();
    //	using (var transaction = connection.BeginTransaction())
    //	{
    //		for (int i = 0; i < storedProcedure.Count() - 1; i++)
    //		{
    //			connection.Execute(storedProcedure.ElementAt(i), parameters.ElementAt(i), commandType: CommandType.StoredProcedure, transaction: transaction);
    //		}

    //		transaction.Commit();
    //		completed = true;
    //	}
    //	return completed;
    //}
}
