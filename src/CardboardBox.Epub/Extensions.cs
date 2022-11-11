namespace CardboardBox.Epub
{
	public static class Extensions
	{
		public static (string extension, string mimeType)[] MimeMaps()
		{
			return new[]
			{
				("jpg", "image/jpeg"),
				("jpeg", "image/jpeg"),
				("png", "image/png"),
				("webp", "image/webp"),
				("gif", "image/gif"),
				("ncx", "application/x-dtbncx+xml"),
				("css", "text/css"),
				("html", "text/html"),
				("xhtml", "application/xhtml+xml")
			};
		}

		public static string MimeType(this string extension)
		{
			return MimeMaps().FirstOrDefault(t => t.extension.ToLower() == extension.ToLower()).mimeType;
		}

		public static string Extension(this string mimeType)
		{
			return MimeMaps().FirstOrDefault(t => t.mimeType.ToLower() == mimeType.ToLower()).extension;
		}

		public static string Serialize(this IXmlItem item)
		{
			var doc = new XDocument
			{
				Declaration = new XDeclaration("1.0", "utf-8", null)
			};
			var el = item.ToElement();
			doc.Add(el);

			using var writer = new Utf8StringWriter();
			doc.Save(writer);

			return writer.ToString();
		}

		public static string SnakeName(this string text)
		{
			var ar = text
				.PurgePathChars()
				.ToLower()
				.ToCharArray()
				.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-')
				.ToArray();
			var cur = new string(ar).Replace(" ", "-").ToLower();

			while (cur.Contains("--"))
				cur = cur.Replace("--", "-");

			return cur;
		}

		public static string FixFileName(this string text)
		{
			var ext = Path.GetExtension(text).TrimStart('.');
			var name = Path.GetFileNameWithoutExtension(text).TrimEnd('.').SnakeName();

			return $"{name.Replace("-", "")}.{ext}";
		}

		public static string CleanId(this string text)
		{
			if (text.Length == 0) return text;

			if (char.IsDigit(text[0]))
				return "a" + text;

			return text;
		}

		public static string FixMissingHtmlTags(this string content, string tag)
		{
			string start = $"<{tag}>",
				stop = $"</{tag}";

			int i = 0;
			while (i < content.Length)
			{
				var fis = content.IndexOf(start, i);
				if (fis == -1) break;

				var nis = content.IndexOf(start, fis + start.Length);
				var fie = content.IndexOf(stop, fis);

				if (nis == -1) break;

				if (nis < fie)
					content = content.Insert(nis, stop);

				if (fie == -1)
				{
					content += stop;
					break;
				}

				i = fie + 1;
			}
			return content;
		}
	}
}
