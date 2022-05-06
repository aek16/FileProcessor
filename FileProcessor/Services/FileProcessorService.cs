using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using FileProcessorOMS.Models;

namespace FileProcessorOMS.Services
{
    public class FileProcessorService
    {


        public FileProcessorService()
        {
        }

        public void ProcessFiles(string securityFile, string portfolioFile, string transactionFile, string outputFolder)
        {
            List<Transaction> transactions;
            List<Security> securities;
            List<Portfolio> portfolios;

            try
            {
                using (var reader = new StreamReader(securityFile))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    securities = csv.GetRecords<Security>().ToList();
                }

                using (var reader = new StreamReader(portfolioFile))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    portfolios = csv.GetRecords<Portfolio>().ToList();
                }

                using (var reader = new StreamReader(transactionFile))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    //Adding CSV map to ignore additional parameters in Transaction Class
                    csv.Context.RegisterClassMap<TransactionMap>();
                    transactions = csv.GetRecords<Transaction>().ToList();
                }

                foreach (var transaction in transactions)
                {
                    //Assumption made that only 1 security & portfolio per transaction
                    transaction.Security = securities.Where(x => x.SecurityId == transaction.SecurityId).FirstOrDefault();
                    transaction.Portfolio = portfolios.Where(x => x.PortfolioId == transaction.PortfolioId).FirstOrDefault();
                }
            }catch(FileNotFoundException ex)
            {
                Console.WriteLine($"Location of input file is incorrect: {ex.Message}");
                throw ex;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an error parsing input files: {ex.Message}");
                throw ex;
            }

            try
            {
                WriteToFile<CsvAAAMap>(outputFolder, transactions, ",", true, "aaa");
                WriteToFile<CsvBBBMap>(outputFolder, transactions, "|", true, "bbb");
                WriteToFile<CsvCCCMap>(outputFolder, transactions, ",", false, "ccc");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an error writing to files: {ex.Message}");
                throw ex;
            }
        }

        public void WriteToFile<T>(string outputFolder, List<Transaction> transactions, string delimiter, bool header, string extention) where T : ClassMap
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter,
                HasHeaderRecord = header
            };

            using (var writer = new StreamWriter(Path.Combine(outputFolder, "oms." + extention)))
            using (var csvWriter = new CsvWriter(writer, config))
            {
                csvWriter.Context.RegisterClassMap<T>();
                csvWriter.WriteRecords(transactions);
            }

        }
    }
}
