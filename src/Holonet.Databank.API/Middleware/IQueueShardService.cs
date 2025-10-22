using Holonet.Databank.Core.Dtos;

namespace Holonet.Databank.API.Middleware
{
    public interface IQueueShardService
    {
        Task QueueShardItem(DataRecordFunctionDto record);
    }
}