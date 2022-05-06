using System;
using System.IO;
using FileProcessorOMS.Services;
using NUnit.Framework;

namespace FileProcessorTests
{
    public class FileProcessorServiceTests
    {
        string securitiesPath;
        string portfolioPath;
        string transactionsPath;
        string outputPath;

        [SetUp]
        public void Setup()
        {
            //Change this variable to chose FOLDER location to store outputs
            string outputLocation = "";

            //Set Input File Locations for tests 
            securitiesPath = Path.Combine(Environment.CurrentDirectory, "securities.csv");
            portfolioPath = Path.Combine(Environment.CurrentDirectory, "portfolios.csv");
            transactionsPath = Path.Combine(Environment.CurrentDirectory, "transactions.csv");
            outputPath = Path.Combine(Environment.CurrentDirectory, outputLocation);
        }

        [Test]
        public void RunFileProcessor()
        {
            FileProcessorService fileProcessorService = new FileProcessorService();
            fileProcessorService.ProcessFiles(securitiesPath, portfolioPath, transactionsPath, outputPath);
            Assert.Pass();
        }

        [Test]
        public void Fail_File_Not_Found()
        {
            FileProcessorService fileProcessorService = new FileProcessorService();

            securitiesPath = Path.Combine(Environment.CurrentDirectory, "zzzsecurities.csv");

            Assert.Throws<FileNotFoundException>(() => fileProcessorService.ProcessFiles(securitiesPath, portfolioPath, transactionsPath, outputPath));
        }
    }
}
