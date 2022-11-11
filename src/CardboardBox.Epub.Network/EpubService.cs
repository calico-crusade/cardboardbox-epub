namespace CardboardBox.Epub
{
	public interface IEpubService
	{
		Task<string> Generate(string title, Func<IEpubBuilder, Task> builder);

		Task Generate(string path, string title, Func<IEpubBuilder, Task> builder);

		Task<FileData> GetData(string url);
	}

	public class EpubService : IEpubService
	{
		private readonly IFileCacheService _file;

		public EpubService(IFileCacheService file)
		{
			_file = file;
		}

		public async Task Generate(string path, string title, Func<IEpubBuilder, Task> builder)
		{
			await using var epub = EpubBuilder.Create(title, path);
			var bob = await epub.Start();
			await builder(bob);
		}

		public async Task<string> Generate(string title, Func<IEpubBuilder, Task> builder)
		{
			var path = Path.GetTempFileName();
			await Generate(path, title, builder);
			return path;
		}

		public async Task<FileData> GetData(string url)
		{
			if (url.ToLower().StartsWith("file://")) return await GetDataFromFile(url.Remove(0, 7));
			var (stream, name, type) = await _file.GetFile(url);

			name = DetermineName(url, name, type);
			return new(stream, name, type);
		}

		public Task<FileData> GetDataFromFile(string path)
		{
			var name = Path.GetFileName(path);
			var ext = Path.GetExtension(path).ToLower().TrimStart('.');
			var type = ext.MimeType();
			if (string.IsNullOrEmpty(type)) throw new NotSupportedException($"File Extension \"{ext}\" mime-type is unknown");

			var stream = (Stream)File.OpenRead(path);
			return Task.FromResult(new FileData(stream, name, type));
		}

		public string RandomBits(int size, string? chars = null)
		{
			chars ??= "abcdefghijklmnopqrstuvwxyz0123456789";
			var r = new Random();

			var output = "";
			for (var i = 0; i < size; i++)
				output += chars[r.Next(0, chars.Length)];

			return output;
		}

		public string DetermineName(string url, string name, string type)
		{
			if (!string.IsNullOrEmpty(name)) return RandomBits(5) + name;

			name = url.Split('/').Last();
			var ext = Path.GetExtension(name);
			if (!string.IsNullOrEmpty(ext)) return RandomBits(5) + name;

			ext = type.Extension();
			if (string.IsNullOrEmpty(ext)) throw new NotSupportedException($"`{type}` is not a known media-type");

			return Path.GetRandomFileName() + "." + ext;
		}
	}

	public record class FileData(Stream stream, string filename, string type);
}
