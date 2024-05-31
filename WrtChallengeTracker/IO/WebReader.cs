using System.Net;
using System.Security.Policy;
using HtmlAgilityPack;

namespace WrtChallengeTracker.IO
{
    public static class WebReader
    {
        static public string Site { get { return "https://textes-a-la-pelle.fr"; } }

        static private DateTime? lastActualisation = null;
        static private TimeSpan minimumAfterLast = TimeSpan.FromDays(1);
        static public TimeSpan MinimumAfter { get { return minimumAfterLast; } set => minimumAfterLast = value.TotalDays < 1 ? TimeSpan.FromDays(1) : value; }
        static public TimeSpan DurationSinceLastTime { get => !lastActualisation.HasValue ? TimeSpan.MaxValue : DateTime.Now.Subtract(lastActualisation.Value); }
        static public DateTime? LastActualisation { get => lastActualisation; }
        static public bool IsOkToActualise() => DurationSinceLastTime >= MinimumAfter;
        static public void Load()
        {
            Program.AddText("WebReader.Load\n");
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

            Program.AddText("WebReader.LoadHTML\n");
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
                    lastActualisation = DateTime.Today;
                    Stream stream = httpResponseMessage.Content.ReadAsStream();
                    StreamReader reader = new StreamReader(stream);
                    text = reader.ReadToEnd();
                    reader.Close();



                }



            }
            return text;


        }
        static private void ReadHTML(HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            Program.AddText("WebReader.ReadHTML\n");
            HtmlNode htmlNode = htmlDocument.DocumentNode.FirstChild;
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
            IEnumerable<HtmlNode> articles = htmlNode.ChildNodes.Where(c => c.Name == "article");
            foreach (HtmlNode child in articles)
            {
                Challenge.ReadHTML(child);

            }



        }

    }
}
