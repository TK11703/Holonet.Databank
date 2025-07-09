using Holonet.Databank.Web.Models;
using Holonet.Databank.Web.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace Holonet.Databank.Web.Clients;

public class FunctionAppClient : ClientBase
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<FunctionAppClient> _logger;

	public FunctionAppClient(HttpClient httpClient, ILogger<FunctionAppClient> logger, ITokenAcquisition tokenAcquisition, IOptions<AppSettings> options) : base(tokenAcquisition, options)
	{
		_httpClient = httpClient;
		_logger = logger;
		PerformClientChecks();
	}

	private void PerformClientChecks()
	{
		if (_httpClient.BaseAddress == null)
		{
			_logger.LogError("BaseAddress of FunctionAppClient cannot be null.");
		}
	}

	public async Task<bool> ProcessNewDataRecord(DataRecordModel request)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		var dataRecordFunctionDto = request.ToDataRecordFunctionDto();
		using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"ProcessDataRecordShard", dataRecordFunctionDto);
		if (response.IsSuccessStatusCode)
		{
			return true;
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return false;
	}
}
