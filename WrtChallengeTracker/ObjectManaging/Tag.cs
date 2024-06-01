using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrtChallengeTracker.ObjectManaging
{
    public class Tag
    {
        private byte num;
        private string name;
        private bool visibility;
        public Byte Num { get => num; }
        public string Name { get => name; }
        public bool Visibility { get=> visibility; set => visibility = value; }
        public Tag(byte num, string name, bool visibility)
        {
            this.num = num;
            this.name = name;
            this.visibility = visibility;
        }

    }
}
