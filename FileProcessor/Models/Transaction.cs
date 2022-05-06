using System;
using System.Globalization;
using CsvHelper.Configuration;

namespace FileProcessorOMS.Models
{
    public class Transaction
    {
        public int SecurityId { get; set; }
        public int PortfolioId { get; set; }
        public decimal Nominal { get; set; }
        public string OMS { get; set; }
        public TransactionType TransactionType { get; set; }
        public Security Security { get; set; }
        public Portfolio Portfolio { get; set; }       
    }

    public enum TransactionType
    {
        BUY,
        SELL
    }

    //Used to Read in CSV
    public sealed class TransactionMap : ClassMap<Transaction>
    {
        public TransactionMap()
        {
            Map(m => m.SecurityId);
            Map(m => m.PortfolioId);
            Map(m => m.Nominal);
            Map(m => m.OMS);
            Map(m => m.TransactionType);
        }
    }

    //Used to Write to CSV
    public sealed class CsvAAAMap: ClassMap<Transaction>
    {
        public CsvAAAMap()
        {
            Map(m => m.Security.ISIN).Index(0).Name("ISIN");
            Map(m => m.Portfolio.PortfolioCode).Index(1).Name("PortfolioCode");
            Map(m => m.Nominal).Index(2).Name("Nominal");
            Map(m => m.TransactionType).Index(3).Name("TransactionType");
        }
    }

    public sealed class CsvBBBMap : ClassMap<Transaction>
    {
        public CsvBBBMap()
        {
            Map(m => m.Security.CUSIP).Index(0).Name("Cusip");
            Map(m => m.Portfolio.PortfolioCode).Index(1).Name("PortfolioCode");
            Map(m => m.Nominal).Index(2).Name("Nominal");
            Map(m => m.TransactionType).Index(3).Name("TransactionType");
        }
    }

    public sealed class CsvCCCMap : ClassMap<Transaction>
    {
        public CsvCCCMap()
        {
            Map(m => m.Portfolio.PortfolioCode).Index(0);
            Map(m => m.Security.Ticker).Index(1);
            Map(m => m.Nominal).Index(2);
            Map(m => m.TransactionType).Index(3);
        }
    }
}