namespace Holonet.Databank.API.Endpoints;

public interface IEndpoint
{
	void MapEndpoint(IEndpointRouteBuilder app);
}