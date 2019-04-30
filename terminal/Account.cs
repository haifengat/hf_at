using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
    public class Account
    {
        public string ServerName { get; set; } = string.Empty;
        public string Investor { get; set; } = string.Empty;
        public string AppID { get; set; } = string.Empty;
        public string AuthCode { get; set; } = string.Empty;
        public string ProductInfo { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Investor}@{ServerName}";
        }
    }
}
