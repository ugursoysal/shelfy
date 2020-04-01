using IBM.WatsonDeveloperCloud.PersonalityInsights.v3.Model;
using System.Collections.Generic;

namespace Shelfy.Models
{
    public class WatsonPI
    {
        public class Gelen
        {
            public string Body { get; set; }
        }
        public class Sonuclar
        {
            public List<Trait> Personality { get; set; }
            public List<Trait> Values { get; set; }
            public List<Trait> Needs { get; set; }
        }
    }
}
