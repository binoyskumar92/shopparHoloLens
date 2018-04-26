using System;

namespace AssemblyCSharp
{
    [Serializable]
    public class Product
	{
		public string _id { get; set; }
		public string name { get; set; }
		public string maker { get; set; }
		public int price { get; set; }
		public string category { get; set; }
		public string upc { get; set; }
		public string size { get; set; } 
		public NutritionalInfo nutrition { get; set; } 
		public Array ProductImage { get; set; }
	}
}

