using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrtChallengeTracker.ObjectManaging
{
    static public class FilterManaging
    {
        static private byte inc = 0;
        static private List<Tag> tags=new List<Tag>();
        static public List<Tag> Tags { get => tags; }

        static public Tag? Add(string name, byte num,bool visibility=true)
        {
            Tag? tag = null;
            if(!tags.Any(t=>t.Num == num))
            {
                tag=new Tag(num,name,visibility);
                tags.Add(tag);
                if (num >= inc)
                {
                    inc = Convert.ToByte(num + 1);
                }
            }
            return tag;

        }

        static public Tag GetTag(string name)
        {
            Tag? tag=tags.FirstOrDefault(t=>t.Name == name);
            if(tag == null)
            {
                tag=new Tag(inc,name,true);
                inc++;
            }    
            return tag;
        }
        //TODO rendre non nullable
        static public Tag? GetTag(byte num) =>tags.FirstOrDefault(t=>t.Num == num);

    }
}
