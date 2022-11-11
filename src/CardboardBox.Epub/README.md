# CardboardBox.Epub
EPub file build for generating ebooks usable with Amazon Kindle and other ebook reader applications

## Installation
You can install the nuget package using Visual Studio. It targets .net 6.0 to take advantage of most of the new features within c# and .net.

```
PM> Install-Package CardboardBox.Epub
```

## Usage
Once the `CardboardBox.Epub` package is installed; you can start an epub generator using the `EpubBuilder` class:

```csharp
using CardboardBox.Epub;

var filePath = "C:\\Users\\Cardboard\\Desktop\\my-epub.epub";

await using (var epub = EpubBuilder.Create("This is my first book title", filePath))
{
	var builder = await epub.Start();

	//Add the epub to a specific series / set
	builder.BelongsTo("Series Title", 1);

	//Add a style sheet to the epub
	await builder.AddStylesheetFromFile("stylesheet.css", "stylesheet.css");

	//Add a cover page to the epub
	await builder.AddCoverImage("cover.png", "C:\\Users\\Cardboard\\Desktop\\my-book-cover.png");

	//Add a new chapter to the epub
	await builder.AddChapter("My first Chapter", async (cb) => 
	{
		//Add a new page to the chapter (this creates all of the encompassing HTML for you)
		await cb.AddRawPage("page-1", "<p>This is some chapter content</p>");

		//Add a new full HTML page to chapter (this requires the <html> and <body> tags)
		using (var io = File.OpenRead("C:\\Users\\Cardboard\\Desktop\\page-2.html"))
			await cb.AddPage("page-2", io);

		//You can also use:
		await cb.AddPageFromFile("page-2-clone", "C:\\Users\\Cardboard\\Desktop\\page-2.html");

		//Add an image to the chapter
		await cb.AddImage("my-image.png", "C:\\Users\\Cardboard\\Desktop\\page-3.png");
	});

	//Add some meta data
	builder.Author("Cardboard")
		   .Illustrator("Definitely not cardboard cause he sucks at drawing")
	       .Editor("Cardboard's Dog")
		   .Translator("Google")
		   .Language("troll-speak")
		   .Date(DateTime.Now());
}

//At this point `filePath` contains the written epub file.
```

Note: `EpubBuilder` does not actually write the content of the epub to the file until the instance is disposed of, 
so it is best _not_ to use using declarations, otherwise you might run into issues where the file isn't written correctly.