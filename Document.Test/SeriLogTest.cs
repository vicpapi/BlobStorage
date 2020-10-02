using Document.Core.Interfaces;
using Document.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Document.Test
{
    public class SeriLogTest
    {
        private ISeriLogRepository _loggerRepository = null;

        public SeriLogTest()
        {
            _loggerRepository = new SeriLogRepository();
        }

        [Fact]
        public void SaveInfo()
        {
            string message = "Test logger";
            bool exists = false;
            string path = @"C:\Users\Victor Ayala\Documents\Visual Studio 2019\Projects\Document\Document.Test\bin\Debug\netcoreapp3.1\log-20200929.txt";

            _loggerRepository.LogInfo(message);
            _loggerRepository.LogInfo(message);

            var logText = File.ReadAllText(path);

            if (logText.Contains(message))
            {
                exists = true;
            }

            Assert.True(exists);
        }
    }
}
