# CardboardBox.Epub.Network
Adds some extra functionality to the Epub builder like adding files from network URLs.

## Installation
You can install the nuget package using Visual Studio. It targets .net 6.0 to take advantage of most of the new features within c# and .net.

```
PM> Install-Package CardboardBox.Epub.Network
```

## Setup
You can setup the Epub Network services using the following code:

Where ever you register your services with Dependency Injection, you can add:
```csharp
using CardboardBox.Http;
using CardboardBox.Epub;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddEpubs()
		.AddCardboardHttp(); //Epub Network Services requires CardboardBox.Http to function
```

You can see more about `CardboardBox.Http` [here](https://github.com/calico-crusade/CardboardBox/tree/master/CardboardBox.Http)

## Usage
Once `CardboardBox.Epub.Network` is registered with your service collection you can inject the `IEpubService` into any of your services:

```csharp
using CardboardBox.Epub;

namespace ExampleThing
{
	public class SomeService 
	{
		private readonly IEpubService _epub;

		public SomeService(IEpubService epub)
		{
			_epub = epub;
		}

		public async Task GenerateEpub(string title, string path)
		{
			await _epub.Generate(path, title, async builder => 
			{
				//Add all of your epub data here.

				//Add a cover image from a URL:
				await builder.AddCoverImage("https://example.org/example.png", epub);

				//You can also fetch file streams from a URL:
				var (stream, filename, mimetype) = await _epub.GetData("https://example.org/example.png");
				await builder.AddImage(filename, stream);
			});
		}
	}
}

```