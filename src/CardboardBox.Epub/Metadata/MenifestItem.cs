namespace CardboardBox.Epub.Metadata
{
	public class ManifestItem : IXmlItem
	{
		public string Id { get; set; }
		public string Href { get; set; }
		public string MediaType { get; set; }

		public string? Properties { get; set; }

		public ManifestItem(string id, string href, string mediaType, string? props = null)
		{
			Id = id;
			Href = href;
			MediaType = mediaType;
			Properties = props;
		}

		public XElement ToElement()
		{
			var el = new XElement("item")
				.AddAttribute("id", Id)
				.AddAttribute("href", Href)
				.AddAttribute("media-type", MediaType);

			if (!string.IsNullOrEmpty(Properties))
				el.AddAttribute("properties", Properties);

			return el;
		}
	}
}
