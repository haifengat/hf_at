using System;
using System.ComponentModel;

namespace HaiFeng
{

	/// <summary>
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class ParameterAttribute : DescriptionAttribute
	{
		/// <summary>
		/// </summary>
		/// <param name="pCategory"> </param>
		/// <param name="pDescription"> </param>
		public ParameterAttribute(string pCategory = "参数", string pDescription = null)
			: base(pDescription)
		{
			this.Category = pCategory;
		}

		/// <summary>
		/// </summary>
		/// <param name="pDescription"> </param>
		public ParameterAttribute(string pDescription)
			: base(pDescription)
		{
			this.Category = "参数";
		}

		/// <summary>
		/// </summary>
		public string Category { get; set; }
	}
}
