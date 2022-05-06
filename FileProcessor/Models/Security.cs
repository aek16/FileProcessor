using System;
namespace FileProcessorOMS.Models
{
    public class Security
    {
        public int SecurityId { get; set; }
        public string ISIN { get; set; }
        public string Ticker { get; set; }
        public string CUSIP { get; set; }
    }
}