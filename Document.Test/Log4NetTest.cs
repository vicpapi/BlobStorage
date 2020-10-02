using Document.Core.Interfaces;
using Document.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Document.Test
{
    public class Log4NetTest
    {
        private ILog4NetRepository _loggerRepository = null;
        private string path = @"C:\Users\Victor Ayala\Documents\Visual Studio 2019\Projects\Document\Document.Test\bin\Debug\netcoreapp3.1\Logs\2020-09-29.txt";

        public Log4NetTest()
        {
            _loggerRepository = new Log4NetRepository();
        }

        [Fact]
        public void SaveInfo()
        {
            string message = "Test logger1";
            bool exists = false;       

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
