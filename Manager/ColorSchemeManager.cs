using System;
using System.Collections.Generic;
using System.Text;

namespace Telinha.Manager
{
    public struct ColorScheme
    {
        public Color Primary;
        public Color Secondary;
        public Color Tertiary;

        public Color OnPrimary;
        public Color OnSecondary;
        public Color OnTertiary;
    }

    public static class ColorSchemeManager
    {
        private static readonly Random _random = new();
        private static int _index = 0;

        private static readonly string SavePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "app_colorscheme.txt");

        #region PUBLIC API

        public static ColorScheme GenerateFromBase(Color baseColor, bool darkMode = false)
        {
            // Geração estilo Material You (simplificada)

            var primary = FromHsl(ToHsl(baseColor).H, 0.7, darkMode ? 0.4 : 0.6);
            var secondary = FromHsl((ToHsl(baseColor).H + 30) % 360, 0.4, darkMode ? 0.3 : 0.7);
            var tertiary = FromHsl((ToHsl(baseColor).H + 60) % 360, 0.5, darkMode ? 0.35 : 0.65);

            return BuildScheme(primary, secondary, tertiary);
        }

        public static ColorScheme GetRandomDynamic(bool darkMode = false)
        {
            var baseColor = Color.FromArgb(
                _random.Next(50, 230),
                _random.Next(50, 230),
                _random.Next(50, 230));

            return GenerateFromBase(baseColor, darkMode);
        }

        public static ColorScheme GetNextFixed()
        {
            var scheme = FixedSchemes[_index];
            _index = (_index + 1) % FixedSchemes.Count;
            return BuildScheme(scheme.Primary, scheme.Secondary, scheme.Tertiary);
        }

        public static void Apply(Control c1, Control c2, Control c3, ColorScheme scheme)
        {
            ApplyControl(c1, scheme.Primary, scheme.OnPrimary);
            ApplyControl(c2, scheme.Secondary, scheme.OnSecondary);
            ApplyControl(c3, scheme.Tertiary, scheme.OnTertiary);
        }

        public static void Save(ColorScheme scheme)
        {
            File.WriteAllText(SavePath,
                $"{scheme.Primary.ToArgb()}|{scheme.Secondary.ToArgb()}|{scheme.Tertiary.ToArgb()}");
        }

        public static ColorScheme LoadOrCreate(bool darkMode = false)
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    var parts = File.ReadAllText(SavePath).Split('|');
                    var p = Color.FromArgb(int.Parse(parts[0]));
                    var s = Color.FromArgb(int.Parse(parts[1]));
                    var t = Color.FromArgb(int.Parse(parts[2]));
                    return BuildScheme(p, s, t);
                }
            }
            catch { }

            var scheme = GetRandomDynamic(darkMode);
            Save(scheme);
            return scheme;
        }

        #endregion

        #region INTERNALS

        private static void ApplyControl(Control c, Color back, Color fore)
        {
            if (c == null) return;

            c.BackColor = back;
            c.ForeColor = fore;

            foreach (Control child in c.Controls)
                ApplyControl(child, back, fore);
        }

        private static ColorScheme BuildScheme(Color p, Color s, Color t)
        {
            return new ColorScheme
            {
                Primary = p,
                Secondary = s,
                Tertiary = t,

                OnPrimary = GetContrastColor(p),
                OnSecondary = GetContrastColor(s),
                OnTertiary = GetContrastColor(t)
            };
        }

        private static Color GetContrastColor(Color color)
        {
            // luminância (WCAG)
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return luminance > 0.5 ? Color.Black : Color.White;
        }

        #endregion

        #region HSL

        private static (double H, double S, double L) ToHsl(Color c)
        {
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double h = 0, s, l = (max + min) / 2;

            if (max == min)
            {
                s = 0;
            }
            else
            {
                double d = max - min;
                s = l > 0.5 ? d / (2 - max - min) : d / (max + min);

                if (max == r)
                    h = (g - b) / d + (g < b ? 6 : 0);
                else if (max == g)
                    h = (b - r) / d + 2;
                else
                    h = (r - g) / d + 4;

                h /= 6;
            }

            return (h * 360, s, l);
        }

        private static Color FromHsl(double h, double s, double l)
        {
            h /= 360;

            double r, g, b;

            if (s == 0)
            {
                r = g = b = l;
            }
            else
            {
                double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                double p = 2 * l - q;

                r = HueToRgb(p, q, h + 1.0 / 3);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1.0 / 3);
            }

            return Color.FromArgb(
                (int)(r * 255),
                (int)(g * 255),
                (int)(b * 255));
        }

        private static double HueToRgb(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0 / 6) return p + (q - p) * 6 * t;
            if (t < 1.0 / 2) return q;
            if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
            return p;
        }

        #endregion

        #region FIXED FALLBACK

        private static readonly List<(Color Primary, Color Secondary, Color Tertiary)> FixedSchemes =
            new()
            {
            (Color.FromArgb(103,80,164), Color.FromArgb(98,91,113), Color.FromArgb(125,82,96)),
            (Color.FromArgb(56,106,32), Color.FromArgb(85,98,76), Color.FromArgb(56,102,99)),
            (Color.FromArgb(0,99,155), Color.FromArgb(79,97,110), Color.FromArgb(0,104,116)),
            (Color.FromArgb(179,38,30), Color.FromArgb(119,86,83), Color.FromArgb(112,92,46)),
            (Color.FromArgb(154,64,88), Color.FromArgb(117,86,91), Color.FromArgb(122,87,51)),
            (Color.FromArgb(0,106,106), Color.FromArgb(74,99,99), Color.FromArgb(79,95,122)),
            (Color.FromArgb(93,95,239), Color.FromArgb(92,95,115), Color.FromArgb(121,83,106)),
            (Color.FromArgb(46,125,50), Color.FromArgb(84,110,122), Color.FromArgb(106,27,154)),
            (Color.FromArgb(211,47,47), Color.FromArgb(141,110,99), Color.FromArgb(69,90,100)),
            (Color.FromArgb(25,118,210), Color.FromArgb(69,90,100), Color.FromArgb(0,121,107)),
            (Color.FromArgb(81,45,168), Color.FromArgb(97,97,97), Color.FromArgb(173,20,87)),
            (Color.FromArgb(0,121,107), Color.FromArgb(93,64,55), Color.FromArgb(40,53,147)),
            (Color.FromArgb(245,124,0), Color.FromArgb(109,76,65), Color.FromArgb(0,131,143)),
            (Color.FromArgb(194,24,91), Color.FromArgb(123,31,162), Color.FromArgb(48,63,159)),
            (Color.FromArgb(2,136,209), Color.FromArgb(69,90,100), Color.FromArgb(104,159,56)),
            (Color.FromArgb(56,142,60), Color.FromArgb(97,97,97), Color.FromArgb(251,192,45)),
            (Color.FromArgb(230,74,25), Color.FromArgb(93,64,55), Color.FromArgb(25,118,210)),
            (Color.FromArgb(123,31,162), Color.FromArgb(69,90,100), Color.FromArgb(0,121,107)),
            (Color.FromArgb(0,151,167), Color.FromArgb(109,76,65), Color.FromArgb(81,45,168)),
            (Color.FromArgb(175,180,43), Color.FromArgb(69,90,100), Color.FromArgb(211,47,47))
            };

        #endregion
    }
}