using System;
using System.Collections.Generic;
using System.Text;

namespace Block_Blast.Models
{
    public class Theme
    {
        public string Name { get; private set; }
        public Color BackgroundColor { get; private set; }
        public Color TextColor { get; private set; }
        public Color AccentColor { get; private set; }
        public Color CellEmptyColor { get; private set; }
        public Color CellBorderColor { get; private set; }
        public string FontFamily { get; private set; }

    
        public Theme(string name, Color backgroundColor, Color textColor,
                     Color accentColor, Color cellEmptyColor, Color cellBorderColor,
                     string fontFamily = "OpenSansRegular")
        {
            Name = name;
            BackgroundColor = backgroundColor;
            TextColor = textColor;
            AccentColor = accentColor;
            CellEmptyColor = cellEmptyColor;
            CellBorderColor = cellBorderColor;
            FontFamily = fontFamily;
        }


        public void Apply(ContentPage page)
        {
            page.BackgroundColor = BackgroundColor;

            ApplyToChildren(page.Content);
        }

        private void ApplyToChildren(IView view)
        {
            if (view == null) return;

            if (view is Label label)
            {
                label.TextColor = TextColor;
                label.FontFamily = FontFamily;
            }

            if (view is Button button)
            {
                button.BackgroundColor = AccentColor;
                button.TextColor = TextColor;
                button.FontFamily = FontFamily;
            }

            if (view is Layout layout)
                foreach (var child in layout)
                    ApplyToChildren(child);

            if (view is ContentView contentView)
                ApplyToChildren(contentView.Content);

            if (view is ScrollView scrollView)
                ApplyToChildren(scrollView.Content);
        }
    }

}
