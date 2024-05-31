using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace WrtChallengeTracker
{
    public class Challenge
    {
        const string freeTopic= "libre", freePrice= "0 €", freeCandidates= "tous";

        private string id, gratification ,name,publisher;
        private Uri? site;
        private List<string> gallery=new List<string>();
        private string? topic, price, candidates, notes, lenght;
        private DateTime create, deadline;
        private SortedSet<Tag> tags=new SortedSet<Tag>();

        public string Id { get=> id; }
        private Challenge(string id,string gratification, string name, string publisher)
        {
            this.id = id;
            this.gratification = gratification; 
            this.name = name;
            this.publisher = publisher;

        }
        public void SetNote(string note)
        {
            notes = note;
        }
        public void RemoveNote() { notes = null; }
        public void AddTag(Tag tag)=> tags.Add(tag);
        public void RemovePicture()
        {
            //TODO
        }
        public void AddPicture(string picture)
        {
            //TODO
        }

        //TODO Write XML
        static private Challenge Create(string id,string gratification, string lenght, string name, string publisher,
                                    string site,string topic, string price,string candidates,DateTime create,DateTime deadline)
        {
            Challenge? c=ChallengesListManage.ExistingChallenges.FirstOrDefault(cha=>cha.Id==id);
            if (c == null) { c = new Challenge(id, gratification, name, publisher);
                c.create = create;
            }
            else
            {
                c.gratification = gratification;
                c.name = name;
                c.publisher = publisher;
            }
            c.deadline = deadline;
            c.site=new Uri(site);
            c.topic = topic==freeTopic||topic==string.Empty?null:topic;
            c.price = price == freePrice || price == string.Empty ? null : price;
            c.candidates = candidates == freeCandidates || candidates == string.Empty ? null : candidates;
            c.lenght = lenght== string.Empty ? null : lenght;
            ChallengesListManage.AddChallenge(c);
            return c;
        }
        //TODO static Read XML
        static public void ReadHTML(HtmlNode html)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
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
            List<string> tags = html.ChildNodes.Where(h => h.Name == "a").Select(html => html.InnerText).ToList();
            html = html.ParentNode;
            html = html.ChildNodes.Last(h => h.Name == "div");
            foreach (HtmlNode node in html.ChildNodes)
            {
                if (node.Name == "p")
                {
                    string[] text = { "" };
                    foreach (HtmlNode child in node.ChildNodes)
                    {
                        text[0] += child.InnerText;

                    }
                    text = text[0].Trim().Split("&#8239;:&ensp;");
                    data.Add(text[0], text[1]);
                }

            }
            string[] strings = { "Longueur", "Public" };
            foreach (string str in strings)
            {

                if(!data.ContainsKey(str))
                {
                    data.Add(str, string.Empty);
                }
            }
            strings = data["Envois jusqu'au"].Split("/");
            DateTime dateTime = new DateTime(Convert.ToInt32(strings[2]), Convert.ToInt32(strings[1]), Convert.ToInt32(strings[0]));
            

                Create(data["id"], data["Gratification"], data["Longueur"], data["name"], data["publisher"], data["site"], data["Thème"], data["Frais d'inscription"], data["Public"], DateTime.Now, dateTime);



        }



    }
}
