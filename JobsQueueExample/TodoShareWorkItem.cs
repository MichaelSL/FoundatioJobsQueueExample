using Foundatio.Jobs;
using Foundatio.Queues;

namespace JobsQueueExample
{
    public class TodoShareWorkItemHandler : WorkItemHandlerBase
    {
        private readonly IQueue<WorkItemData> queue;

        public TodoShareWorkItemHandler(IQueue<WorkItemData> queue)
        {
            this.queue = queue ?? throw new ArgumentNullException(nameof(queue));
        }

        public override async Task HandleItemAsync(WorkItemContext ctx)
        {
            var workItem = ctx.GetData<TodoShareWorkItem>();

            // We can report the progress over the message bus easily.
            // To receive these messages just inject IMessageSubscriber
            // and Subscribe to messages of type WorkItemStatus
            await ctx.ReportProgressAsync(0, "Starting ToDo sharing");
            await Task.Delay(TimeSpan.FromSeconds(1.5));
            
            await ctx.ReportProgressAsync(20, "Saving ToDo");
            if (workItem.Contents != null)
            {
                TenantRepository.GetById(workItem.TenantId)?.Todos.AddRange(workItem.Contents);
            }

            await ctx.ReportProgressAsync(50, "Getting child tenants");
            var children = TenantRepository.GetChildren(workItem.TenantId);
            if (children.Any())
            {
                await ctx.ReportProgressAsync(75, "Sharing down the tree");
                foreach (var item in children)
                {
                    await queue.EnqueueAsync(new TodoShareWorkItem { TenantId = item.Id, Contents = workItem.Contents });
                }
            }

            await ctx.ReportProgressAsync(100, $"ToDo sharing done for {workItem.TenantId}");
        }
    }

    public class TodoShareWorkItem
    {
        public int TenantId { get; set; }
        public IEnumerable<Content>? Contents { get; set; }
    }

}
