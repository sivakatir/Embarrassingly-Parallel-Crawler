using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Embarrassingly_Parallel_Crawler
{
    class Program
    {

        static void Main(string[] args)
        {
            var crawler = new Crawler();

            crawler.Links.Add(new LinkModel
            {
                SiteUrl = new Uri("http://codingfields.com/")
            });

            for (int i = 0; i < 100; i++)
            {
                crawler.Links.Add(new LinkModel
                {
                    SiteUrl = new Uri("http://codingfields.com/" + i)
                });
            }

            foreach (var link in crawler.Links)
            {
                crawler.CrawlQueue.Enqueue(link);
            }

            Action action = () =>
            {
                LinkModel link;
                while (crawler.CrawlQueue.TryDequeue(out link))
                {
                    crawler.Crawl(link);
                }
            };

            var actions = new Action[12];
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i] = action;
            }

            Parallel.Invoke(actions);

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }

    public class Crawler
    {
        public ConcurrentBag<LinkModel> Links = new ConcurrentBag<LinkModel>();
        public ConcurrentBag<LinkModel> CrawledLinks = new ConcurrentBag<LinkModel>();
        public ConcurrentQueue<LinkModel> CrawlQueue = new ConcurrentQueue<LinkModel>();

        public void Crawl(LinkModel link)
        {
            var browser = new ScrapingBrowser
            {
                AllowAutoRedirect = false,
                AllowMetaRedirect = false
            };

            try
            {
                var page = browser.NavigateToPage(link.SiteUrl);

                var title = page.Html.CssSelect("h1").FirstOrDefault();

                link.PageTitle = title != null ? title.InnerText : "H1 Not Found";

                CrawledLinks.Add(link);

                Console.WriteLine("{0} | {1} | {2}", CrawlQueue.Count, CrawledLinks.Count, link.PageTitle);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Crawl Failed: {0}", ex.Message);
            }
        }
    }
}
