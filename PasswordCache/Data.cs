using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace PasswordCache
{
    [Serializable]
    class Data
    {
        public string Name { get; set; }
        public string Log { get; set; }
        public string Pas { get; set; }
        public string Com { get; set; }
        public string HLink { get; set; }

        public Data(string n,string l,string p,string c,string h)
        {
            Name = n;
            Log = l;
            Pas = p;
            Com = c;
            HLink = h;
        }

        public override string ToString()
        {
            return base.ToString();
        }

    }
}
