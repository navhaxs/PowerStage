using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string test = @"GET / HTTP/1.1
Host: 127.0.0.1:54688
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:61.0) Gecko/20100101 Firefox/61.0
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
Accept-Language: en-AU,en;q=0.5
Accept-Encoding: gzip, deflate
Cookie: SESS894db45d2315bda1c47d5b502cfa9523=y8SXlzTHaRoxunoo63sxHymv6trr_bny1UD9EVHCaRg
Connection: keep-alive
Upgrade-Insecure-Requests: 1
DNT: 1

";

            string first = new StringReader(test).ReadLine();
            string[] res = first.Split(' ');
            if (res.Length < 2)
            {
                return null;
            }
            string method = res[0];
            string path = res[1];

        }
    }
}
