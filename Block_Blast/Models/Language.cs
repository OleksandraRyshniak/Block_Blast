using System;
using System.Collections.Generic;
using System.Text;


namespace Block_Blast.Models
{
    public class Language
    {
       
        public string Code { get; private set; } 
        public string Name { get; private set; }  
        public string Flag { get; private set; }   

        public Language(string code, string name, string flag)
        {
            Code = code;
            Name = name;
            Flag = flag;
        }

   
        public static Language Estonian => new Language("et", "Eesti", "🇪🇪");
       
        public static Language English => new Language("en", "English", "🇬🇧");
        public static Language Russian => new Language("ru", "Русский", "RU");

        public static List<Language> All => new List<Language>
        {
            Estonian,
           
            English
        };

        public override string ToString() => $"{Flag} {Name}";
    }

}
