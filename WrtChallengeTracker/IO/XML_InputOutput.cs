using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using WrtChallengeTracker.ObjectManaging;

namespace WrtChallengeTracker.IO
{
    static public class Xml_InputOutput
    {
        private const string pathSaveChallenges = "saveChallenges.xml", pathSaveSettings = "saveSettings.xml", pathXSDChallenges= "..\\..\\..\\IO\\fileChallengSave.xsd";

  
        static public void SaveChallenges()
        {
           XmlDocument xmlDocument = new XmlDocument();
           XmlElement root= xmlDocument.CreateElement("challengeDataRoot");
            xmlDocument.AppendChild(root);
            XmlElement challenges = xmlDocument.CreateElement("challenges");
            XmlElement tags = xmlDocument.CreateElement("tags");
            root.AppendChild(challenges);
            root.AppendChild(tags);

            XmlElement xml;

            List<Tag> tags1 = FilterManaging.Tags.ToList();
            foreach (Tag tag in tags1)
            {
                xml= xmlDocument.CreateElement("tag");
                xml.SetAttribute("name", tag.Name);
                xml.SetAttribute("num", tag.Num.ToString());
                xml.SetAttribute("visibility",tag.Visibility.ToString());
                tags.AppendChild(xml);
            }

            List<Challenge> challenges1=ChallengesListManaging.ExistingChallenges.ToList();
            foreach (Challenge challenge in challenges1)
            {
                challenges.AppendChild(challenge.ToXml(xmlDocument));
            }

            xmlDocument.Save(pathSaveChallenges);
            

        }

        static private XDocument? Check(string path,string pathXSD)
        { 
            bool error=false;
            if(File.Exists(path) && File.Exists(pathXSD)) {
                XmlSchemaSet schemas = new XmlSchemaSet();
                XDocument document = XDocument.Load(path);
                StreamReader stream =new StreamReader(pathXSD);
                schemas.Add("", XmlReader.Create(stream));
                document.Validate(schemas, (o, e) => { error = true; });
                return document;
            }
            return null;


        }
        static public void LoadChallenges() {
            XDocument? document = Check(pathSaveChallenges, pathXSDChallenges);
            XElement root = document.Root;
            XElement tags=(XElement)root.Nodes().First(n=>n.GetType()==typeof(XElement) && ((XElement)n).Name.LocalName=="tags" );
            XElement challenges = (XElement)root.Nodes().First(n => n.GetType() == typeof(XElement) && ((XElement)n).Name.LocalName == "challenges");
            IEnumerable<XNode> xNodes = tags.Nodes();
            foreach(XNode xNode in xNodes)
            {
                if(xNode.GetType() == typeof(XElement))
                {
                    Dictionary<string,string> d= ((XElement)xNode).Attributes().ToDictionary(a=>a.Name.LocalName,a=>a.Value);
                    FilterManaging.Add(d["name"], Convert.ToByte(d["num"]), d["visibility"]==true.ToString());
                }
            }
            xNodes = challenges.Nodes();
            foreach( XNode xNode in xNodes)
            {
                if (xNode.GetType() == typeof(XElement))
                {
                    Challenge.ReadXML((XElement)xNode);
                }
            }

        }
    }
}
