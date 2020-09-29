using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DotnetSpider.DataFlow;
using DotnetSpider.DataFlow.Parser;
using DotnetSpider.DataFlow.Storage;
using DotnetSpider.Downloader;
using DotnetSpider.Http;
using DotnetSpider.Infrastructure;
using DotnetSpider.Scheduler.Component;
using DotnetSpider.Selector;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace DotnetSpider.Sample.samples
{
	public class SpeedSpider : Spider
	{
		public static async Task RunAsync()
		{
			var builder = Builder.CreateDefaultBuilder<SpeedSpider>(options =>
			{
				options.Speed = 100;
			});
			builder.UseSerilog();
			builder.UseDownloader<EmptyDownloader>();
			builder.UseQueueDistinctBfsScheduler<HashSetDuplicateRemover>();
			await builder.Build().RunAsync();
		}

		public SpeedSpider(IOptions<SpiderOptions> options,
			DependenceServices services,
			ILogger<Spider> logger) : base(
			options, services, logger)
		{
		}

		protected override async Task InitializeAsync(CancellationToken stoppingToken = default)
		{
			for (var i = 0; i < 100000; ++i)
			{
				await AddRequestsAsync(new Request("https://news.cnblogs.com/n/page/" + i)
				{
					Downloader = DownloaderNames.Empty
				});
			}

			AddDataFlow(new MyDataFlow());
		}

		protected override (string Id, string Name) GetIdAndName()
		{
			return (ObjectId.NewId().ToString(), "speed");
		}

		protected class MyDataFlow : DataFlowBase
		{
			private int _downloadCount;

			public override Task HandleAsync(DataContext context)
			{
				Interlocked.Increment(ref _downloadCount);
				if ((_downloadCount % 100) == 0)
				{
					Logger.LogInformation($"Complete {_downloadCount}");
				}

				return Task.CompletedTask;
			}
		}
	}
}