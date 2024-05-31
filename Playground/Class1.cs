using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground
{
    static public class Class1
    {
        static public void Load()
        {
            Console.Out.WriteLine("WebReader.Load");
            string? doc = LoadHTML();

            if (doc != null)
            {
                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                html.LoadHtml(doc);
                ReadHTML(html);

            }
        }
        static string? LoadHTML()
        {
            StreamReader stream1 = new StreamReader("html.html");
            string html = stream1.ReadToEnd();
            stream1.Close();
            return html;/*
            Console.Out.WriteLine("WebReader.LoadHTML\n");
            string? text = null;
            if (IsOkToActualise())
            {


                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Site);
                HttpRequestMessage request = new HttpRequestMessage();
                request.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(Site);
                HttpResponseMessage httpResponseMessage = client.Send(request);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Stream stream = httpResponseMessage.Content.ReadAsStream();
                    StreamReader reader = new StreamReader(stream);
                    text = reader.ReadToEnd();
                    reader.Close();



                }



            }
            return text;
            */

        }
        static private void ReadHTML(HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            Console.Out.WriteLine("WebReader.ReadHTML\n");
            HtmlAgilityPack.HtmlNode htmlNode = htmlDocument.DocumentNode.FirstChild;
            while (htmlNode.Name != "html")
            {
                htmlNode = htmlNode.NextSibling;
            }
            htmlNode = htmlNode.FirstChild;
            while (htmlNode.Name != "body")
            {
                htmlNode = htmlNode.NextSibling;
            }

            htmlNode = htmlNode.FirstChild;
            while (htmlNode.Name != "main")
            {
                htmlNode = htmlNode.NextSibling;
            }
            htmlNode = htmlNode.FirstChild;
            while (htmlNode.Id != "contests")
            { htmlNode = htmlNode.NextSibling; }
            ChallengeReadHTML(htmlNode.ChildNodes.First(h=>h.Name=="article"));/*
            IEnumerable<HtmlNode> articles = htmlNode.ChildNodes.Where(c => c.Name == "article");
            foreach (HtmlNode child in articles)
            {
                

            }*/



        }

        static private void ChallengeReadHTML(HtmlNode html)
        {
            Console.Out.WriteLine("\nChallenge.ReadHTML\n");
            Dictionary<string,string> data= new Dictionary<string,string>();
            data.Add("id", html.Id);
            html = html.ChildNodes.First(h => h.Name == "div").ChildNodes.Last(h => h.Name == "div").ChildNodes.First(h => h.Name == "a");
            data.Add("site", html.GetAttributeValue("href", ""));
            html = html.ChildNodes.First(h => h.Name == "h2");
            data.Add("publisher", html.InnerText.Replace("&ensp;", ""));
            html = html.ParentNode.ParentNode;
            html = html.ChildNodes.First(h => h.Name == "h3");
            data.Add("name", html.InnerText);
            html = html.ParentNode;
            html = html.ChildNodes.First(h => h.Name == "div");
            List<string> tags =html.ChildNodes.Where(h=>h.Name=="a").Select(html=>html.InnerText).ToList();
            html = html.ParentNode;
            html = html.ChildNodes.Last(h => h.Name == "div");
            foreach (HtmlNode node in html.ChildNodes)
            {
                if(node.Name=="p")
                {
                    string[] text = { "" };
                    foreach (HtmlNode child in node.ChildNodes)
                    {
                        text[0 ] += child.InnerText;
                        
                    }
                    text = text[0].Trim().Split("&#8239;:&ensp;");
                    data.Add(text[0], text[1]);
                }

            }
            foreach(KeyValuePair<string, string> pair in data)
            {
                Console.WriteLine(pair.Key);
                Console.WriteLine("\t"+pair.Value);
            }
        }

    }
}
