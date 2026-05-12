using System;
using System.Collections.Generic;
using System.Text;

namespace Block_Blast.Models
{
    public class Language
    {
        // ── Свойства ──────────────────────────────────────────
        public string Code { get; private set; }   // "et", "ru", "en"
        public string Name { get; private set; }   // "Eesti", "Русский", "English"
        public string Flag { get; private set; }   // эмодзи флага

        // ── Конструктор ───────────────────────────────────────
        public Language(string code, string name, string flag)
        {
            Code = code;
            Name = name;
            Flag = flag;
        }

        // ── Готовые языки ─────────────────────────────────────
        public static Language Estonian => new Language("et", "Eesti", "🇪🇪");
       
        public static Language English => new Language("en", "English", "🇬🇧");

        public static List<Language> All => new List<Language>
        {
            Estonian,
           
            English
        };

        public override string ToString() => $"{Flag} {Name}";
    }

}
