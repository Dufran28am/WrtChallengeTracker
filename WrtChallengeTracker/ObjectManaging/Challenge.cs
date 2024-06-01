using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace WrtChallengeTracker.ObjectManaging
{
    public class Challenge
    {
        const string freeTopic = "libre", freePrice = "0 €", freeCandidates = "tous";

        private string id, gratification, name, publisher;
        private Uri? site;
        private List<string> gallery = new List<string>();
        private string? topic, price, candidates, notes, lenght;
        private DateTime create, deadline;
        private SortedSet<Tag> tags = new SortedSet<Tag>();
        private StatusChallenge status;
        private bool save;
        
        public string Id { get => id; }




        //TODO procédure qui supprime, getter
        private Challenge(string id, string gratification, string name, string publisher,StatusChallenge status,bool save)
        {
            this.id = id;
            this.gratification = gratification;
            this.name = name;
            this.publisher = publisher;
            this.save = save;
            this.status= status;

        }
        public void SetNote(string note)
        {
            notes = note;
        }
        public void RemoveNote() { notes = null; }
        public void AddTag(Tag tag) => tags.Add(tag);

        public bool AnyTagVisible() => tags.Any(t=>t.Visibility);
        public bool ToRemove() => !save && deadline < DateTime.Today;
        public void BeforeRemoving()
        {
            //TODO
        }
        public void AddPicture(string picture)
        {
            //TODO
        }

        public XmlElement ToXml(XmlDocument doc)
        {
            XmlElement xmlElement = doc.CreateElement("challenge");
            XmlElement xml,xml2;
            if(notes!=null)
            {
                xml = doc.CreateElement("note");
                xml.InnerText = notes;
                xmlElement.AppendChild(xml);
            }
            if(gallery.Count>0) {
                xml = doc.CreateElement("gallery");
                foreach(string p in gallery)
                {
                    xml2=doc.CreateElement("picture");
                    xml2.InnerText = p;
                    xml.AppendChild(xml2);
                }
                xmlElement.AppendChild(xml);
            }
            if(tags.Count>0) {
                xml = doc.CreateElement("tags");
                foreach (Tag tag in tags)
                {
                    xml2 = doc.CreateElement("tag");
                    xml2.InnerText = tag.Num.ToString();
                    xml.AppendChild(xml2);
                }
                xmlElement.AppendChild(xml);
            }


            xmlElement.SetAttribute("id", this.id);
            xmlElement.SetAttribute ("name", this.name);
            xmlElement.SetAttribute("gratification", this.gratification);
            xmlElement.SetAttribute("publisher", this.publisher);
            if(this.site!=null)
            {
                xmlElement.SetAttribute("site", this.site.ToString());
            }
            if (this.price != null)
            {
                xmlElement.SetAttribute("price", this.price);
            }
            if (this.topic != null)
            {
                xmlElement.SetAttribute("topic", this.topic);
            }
            if (this.candidates != null)
            {
                xmlElement.SetAttribute("candidates", this.candidates);
            }
            if (this.lenght != null)
            {
                xmlElement.SetAttribute("lenght", this.lenght);
            }
            if(this.status!=StatusChallenge.None)
            {
                xmlElement.SetAttribute("status", this.status == StatusChallenge.Star ? "1" : "0");
            }
            xmlElement.SetAttribute("save", save ? "1" : "0");
            xmlElement.SetAttribute("creatiopn", this.create.ToString());
            xmlElement.SetAttribute("deadline", this.deadline.ToString());

            return xmlElement;
        }
        static public void ReadXML(XElement xElement)
        {
            Dictionary<string,string> attributes = xElement.Attributes().ToDictionary(n=>n.Name.LocalName,n=>n.Value);
            string[] strings = { "site", "price", "topic", "candidates", "lenght", "status" };
            StatusChallenge statusChallenge;
            if(!attributes.ContainsKey("status")) { statusChallenge = StatusChallenge.None; }
            else if (attributes["status"]==true.ToString()) { statusChallenge = StatusChallenge.Star;}
            else { statusChallenge = StatusChallenge.Trash; }
            foreach (string s in strings)
            {
                if(!attributes.ContainsKey(s))
                {
                    attributes.Add(s, "");
                }
            }
            Challenge challenge = Create(attributes["id"], attributes["gratification"], attributes["lenght"], attributes["name"], attributes["publisher"], attributes["site"], attributes["candidates"], attributes["price"], attributes["candida"], DateTime.Parse(attributes["creation"]), DateTime.Parse(attributes["deadline"]), statusChallenge, attributes["save"] == true.ToString());
            XElement? xml1 = (XElement)xElement.Nodes().FirstOrDefault(n => n.GetType() == typeof(XElement) && ((XElement)n).Name.LocalName == "tags");
            IEnumerable<XNode> xml2 = xml1.Nodes();
            foreach(XNode tag in xml2.Nodes())
            {
                if(tag.GetType()==typeof(XElement) )
                {
                    challenge.AddTag(FilterManaging.GetTag(Convert.ToByte(((XElement)tag).)));
                }
            }

        }
        static private Challenge Create(string id, string gratification, string lenght, string name, string publisher,
                                    string site, string topic, string price, string candidates, DateTime create, DateTime deadline,StatusChallenge status,bool save)
        {
            Challenge? c = ChallengesListManaging.ExistingChallenges.FirstOrDefault(cha => cha.Id == id);
            if (c == null)
            {
                c = new Challenge(id, gratification, name, publisher,status,save);
                c.create = create;
            }
            else
            {
                c.gratification = gratification;
                c.name = name;
                c.publisher = publisher;
            }
            c.deadline = deadline;
            c.site = new Uri(site);
            c.topic = topic == freeTopic || topic == string.Empty ? null : topic;
            c.price = price == freePrice || price == string.Empty ? null : price;
            c.candidates = candidates == freeCandidates || candidates == string.Empty ? null : candidates;
            c.lenght = lenght == string.Empty ? null : lenght;
            ChallengesListManaging.AddChallenge(c);
            return c;
        }
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

                if (!data.ContainsKey(str))
                {
                    data.Add(str, string.Empty);
                }
            }
            strings = data["Envois jusqu'au"].Split("/");
            DateTime dateTime = new DateTime(Convert.ToInt32(strings[2]), Convert.ToInt32(strings[1]), Convert.ToInt32(strings[0]));


            Challenge c=Create(data["id"], data["Gratification"], data["Longueur"], data["name"], data["publisher"], data["site"], data["Thème"], data["Frais d'inscription"], data["Public"], DateTime.Now, dateTime,StatusChallenge.None,false);
            foreach(string t in tags)
            {
                c.AddTag(FilterManaging.GetTag(t));
            }


        }
            


    }
}
