namespace CardboardBox.Epub
{
	public static class Extensions
	{
		public static IServiceCollection AddEpubs(this IServiceCollection services)
		{
			return services.AddTransient<IEpubService, EpubService>();
		}

		public static async Task AddCoverImage(this IEpubBuilder builder, string url, IEpubService service)
		{
			var (data, file, _) = await service.GetData(url);
			await builder.AddCoverImage(file, data);
			await data.DisposeAsync();
		}
	}
}