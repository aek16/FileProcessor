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
        List<Transaction> transactions;
        List<Security> securities;
        List<Portfolio> portfolios;

        public FileProcessorService()
        {
        }

        public void ProcessFiles(string securityFile, string portfolioFile, string transactionFile, string outputFolder)
        {
            try {

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
                WriteAAAFile(outputFolder);

                WriteBBBFile(outputFolder);

                WriteCCCFile(outputFolder);
            }
            catch (Exception ex) {
                Console.WriteLine($"There was an error writing to files: {ex.Message}");
                throw ex;
            }
        }

        public void WriteAAAFile(string outputFolder)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true
            };

            using (var writer = new StreamWriter(outputFolder + "/oms.aaa"))
            using (var csvWriter = new CsvWriter(writer, config))
            {
                csvWriter.Context.RegisterClassMap<CsvAAAMap>();
                csvWriter.WriteRecords(transactions);
            }

        }

        public void WriteBBBFile(string outputFolder)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "|",
                HasHeaderRecord = true
            };

            using (var writer = new StreamWriter(outputFolder + "/oms.bbb"))
            using (var csvWriter = new CsvWriter(writer, config))
            {
                csvWriter.Context.RegisterClassMap<CsvBBBMap>();
                csvWriter.WriteRecords(transactions);
            }
        }

        public void WriteCCCFile(string outputFolder)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = false
            };

            using (var writer = new StreamWriter(outputFolder + "/oms.ccc"))
            using (var csvWriter = new CsvWriter(writer, config))
            {
                csvWriter.Context.RegisterClassMap<CsvCCCMap>();
                csvWriter.WriteRecords(transactions);
            }

        }

    }
}
