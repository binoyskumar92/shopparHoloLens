using System;

namespace AssemblyCSharp
{
    [Serializable]
    public class ProductImage
	{
		public string thumbnailImage { get; set; }
		public string mediumImage { get; set; }
		public string largeImage { get; set; }
		public string entityType { get; set; }
	}
}

