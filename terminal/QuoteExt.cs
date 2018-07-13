using System;
using System.Collections.Generic;
using System.Text;

namespace HaiFeng
{
	public partial class QuoteExt : CTPQuote
    {
        public QuoteExt(string pAbsoluteFilePath):base(pAbsoluteFilePath) {  }

		public string Broker { get; internal set; }
		public string Investor { get; internal set; }
		public string Password { get; internal set; }
	}
}
