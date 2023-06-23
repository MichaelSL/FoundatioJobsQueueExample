using Foundatio.Jobs;
using Foundatio.Messaging;
using Foundatio.Queues;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Xml.Linq;

namespace JobsQueueExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateSlimBuilder(args);

            builder.Services.AddHttpClient();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddTransient<TodoShareWorkItemHandler>();

            // Register the handlers with dependency injection.
            builder.Services.AddSingleton(sp =>
            {
                var handlers = new WorkItemHandlers();
                handlers.Register<TodoShareWorkItem>(sp.GetRequiredService<TodoShareWorkItemHandler>());
                return handlers;
            });

            // Register the queue for WorkItemData.
            builder.Services.AddSingleton<IQueue<WorkItemData>>(s => new InMemoryQueue<WorkItemData>());

            builder.Services.AddSingleton<IMessagePublisher, InMemoryMessageBus>();
            builder.Services.AddTransient<WorkItemJob>();

            var app = builder.Build();

            var contentApi = app.MapGroup("/content");

            contentApi.MapPost("/share/{TenantId}", async ([FromServices]IQueue <WorkItemData> queue, int TenantId, List<Content> contents) =>
            {
                await queue.EnqueueAsync(new TodoShareWorkItem { TenantId = TenantId, Contents = contents });
            });

            contentApi.MapPut("/share/{TenantId}", async (int TenantId, List<Content> contents, [FromServices]HttpClient httpClient) =>
            {
                if (contents != null)
                {
                    TenantRepository.GetById(TenantId)?.Todos.AddRange(contents);
                }

                var children = TenantRepository.GetChildren(TenantId);
                foreach (var item in children)
                {
                    await httpClient.PutAsJsonAsync($"http://localhost:5253/content/share/{item.Id}", contents);
                }
            });

            var tenantsApi = app.MapGroup("/tenants");
            tenantsApi.MapGet("/", () => TenantRepository.GetAll());

            new JobRunner(app.Services.GetRequiredService<WorkItemJob>(), instanceCount: 2).RunInBackground();

            app.Run();
        }
    }
}