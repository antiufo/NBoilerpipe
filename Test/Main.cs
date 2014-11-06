using NBoilerpipe.Extractors;
using Shaman.Dom;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Test
{
    class MainClass
    {
        public static void Main(string[] args)
        {

            String url = "http://magazine.good.is/features/the-nordic-food-lab-cooks-with-blood";
            //String url = "http://www.dn.se/nyheter/vetenskap/annu-godare-choklad-med-hjalp-av-dna-teknik";

            String page = String.Empty;
            WebRequest request = WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                page = streamReader.ReadToEnd();
            }
            var doc = new  HtmlDocument();
            doc.LoadHtml(page);
            var d = ArticleExtractor.INSTANCE.GetDocument(doc.DocumentNode);
            //String text = ArticleExtractor.INSTANCE.GetText(doc.DocumentNode);
            Console.WriteLine("Text: \n" + d.GetText(true, false));
        }
    }
}
