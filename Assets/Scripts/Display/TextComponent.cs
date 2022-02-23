using FactoryZero.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FactoryZero.Display
{
    public class TextComponent
    {
        static Dictionary<string, ITextEffect> effectRegistry = new Dictionary<string, ITextEffect>();
        public static void SetEffectExecutor(string name, ITextEffect executor)
        {
            if(!effectRegistry.ContainsKey(name))
            {
                effectRegistry.Add(name, executor);
            }
            else
            {
                throw new AccessViolationException($"An executor with the name \"{name}\" has already been registered.");
            }
        }

        public class EffectExecutionEventArgs : EventArgs, ICancellable
        {
            public bool Cancel { get; set; }

            TextComponent component;
            public EffectExecutionEventArgs(TextComponent component)
            {
                this.component = component;
            }

            public TextComponent Component { get => component; }

            string text, font;
            bool bold, underline, italic;
            Color color;

            public string Text { get => text; set => text = value; }
            public string Font { get => font; set => font = value; }
            public Color Color { get => color; set => color = value; }
            public bool Underline { get => underline; set => underline = value; }
            public bool Bold { get => bold; set => bold = value; }
            public bool Italic { get => italic; set => italic = value; }
        }

        public class EffectParams
        {
            public string effect;
            public object parameters;
        }

        public class JSON
        {
            public string group;
            public string text;
            public string font;
            public string color;
            public EffectParams[] effects;
            public bool bold;
            public bool italic;
            public bool underline;
        }

        string text, originalText, font, group;
        EffectParams[] effects;
        bool underline, italic, bold;
        Color color;

        public TextComponent(string text, Color? color = null, string font = "PF_MainFont", bool bold = false, 
            bool italic = false, bool underline = false, EffectParams[] effects = null, string group = "default")
        {
            if(!color.HasValue)
            {
                color = Color.white;
            }

            this.color = color.Value;

            originalText = text;
            this.text = text;
            this.font = font;
            this.group = group;

            if(effects == null)
            {
                this.effects = new EffectParams[0];
            }
            else
            {     
                this.effects = new EffectParams[effects.Length];
                effects.CopyTo(this.effects, 0);
            }

            this.underline = underline;
            this.italic = italic;
            this.bold = bold;
        }

        public void ExecuteEffects()
        {
            EffectExecutionEventArgs args = new EffectExecutionEventArgs(this);
            foreach (EffectParams effect in effects)
            {
                ITextEffect executor = effectRegistry[effect.effect];
                if(executor != null)
                {
                    executor.Execute(args);
                }
            }

            if (args.Cancel) return;

            text = args.Text ?? "";
            font = args.Font ?? "default";
            color = args.Color;
            underline = args.Underline;
            bold = args.Bold;
            italic = args.Italic;
        }

        public Color Color { get => color; }
        public string Text { get => text; }
        public string OriginalText { get => originalText; }
        public string Font { get => font; }
        public string Group { get => group; }
        public EffectParams[] Effects { get => effects; }
        public bool IsUnderlined { get => underline; }
        public bool IsBold { get => bold; }
        public bool IsItalic { get => italic; }
    }
}
