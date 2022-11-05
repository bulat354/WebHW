using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyServer.Attributes;
using MyServer.TemplateEngines;

namespace MyServer.Pages
{
    [FileController("/index.html")]
    public class GamePage
    {
        [DataSource]
        public MainPageContent GetPage()
        {
            return new MainPageContent();
        }
    }

    public class MainPageContent
    {
        public Article[] Games { get; set; } = new Article[2]
        {
            new Article("War of Gods", "anvbdbvbsvbuosvbuodvbbovdboobividaisoovi", "22.10.2022", "05.11.2022", "/img/war-of-god.jpg", "black"),
            new Article("KyberPunk 3077", "anvbdbvbsvbuosvbuodvbbovdboobividaisoovi", "22.10.2022", "05.11.2022", "/img/cyberpunk.jpg", "blue")
        };
        public Article[] Articles { get; set; } = new Article[2]
        {
            new Article("Programmers", "anvbdbvbsvbuosvbuodvbbovdboobividaisoovi", "22.10.2022", "05.11.2022", "/img/programmers.jpg", "black"),
            new Article("Во что поиграть?", "anvbdbvbsvbuosvbuodvbbovdboobividaisoovi", "22.10.2022", "05.11.2022", "/img/what-to-play.jpg", "green")
        };
    }

    public class Article
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string FirstDate { get; set; }
        public string LastDate { get; set; }
        public string ImageUrl { get; set; }
        public string ImageColor { get; set; }

        public Article(string title, string description, string firstDate, string lastDate, string imageUrl, string imageColor)
        {
            Title = title;
            Description = description;
            FirstDate = firstDate;
            LastDate = lastDate;
            ImageUrl = imageUrl;
            ImageColor = imageColor;
        }
    }
}
