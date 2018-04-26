using System;

namespace AssemblyCSharp
{
    [Serializable]
    public class FatInfo
	{
		public string total { get; set; }
		public string saturatedfat { get; set; }
		public string polyunsaturated { get; set; }
		public string monounsaturated { get; set; }
	}
}

