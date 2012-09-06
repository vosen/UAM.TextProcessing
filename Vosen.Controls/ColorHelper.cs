using System;
using System.Windows.Media;

namespace Vosen.Controls
{
	// Ripped from mono 2.10 - mcs\class\Managed.Windows.Forms\System.Windows.Forms\ControlPaint.cs
	public static class ColorHelper
	{
		private const int RGBMax = 255;
		private const int HLSMax = 255;

		internal static void Color2HBS(Color color, out int h, out int l, out int s)
		{
			int r;
			int g;
			int b;
			int cMax;
			int cMin;
			int rDelta;
			int gDelta;
			int bDelta;

			r = color.R;
			g = color.G;
			b = color.B;

			cMax = Math.Max(Math.Max(r, g), b);
			cMin = Math.Min(Math.Min(r, g), b);

			l = (((cMax + cMin) * HLSMax) + RGBMax) / (2 * RGBMax);

			if (cMax == cMin)
			{		// Achromatic
				h = 0;					// h undefined
				s = 0;
				return;
			}

			/* saturation */
			if (l <= (HLSMax / 2))
			{
				s = (((cMax - cMin) * HLSMax) + ((cMax + cMin) / 2)) / (cMax + cMin);
			}
			else
			{
				s = (((cMax - cMin) * HLSMax) + ((2 * RGBMax - cMax - cMin) / 2)) / (2 * RGBMax - cMax - cMin);
			}

			/* hue */
			rDelta = (((cMax - r) * (HLSMax / 6)) + ((cMax - cMin) / 2)) / (cMax - cMin);
			gDelta = (((cMax - g) * (HLSMax / 6)) + ((cMax - cMin) / 2)) / (cMax - cMin);
			bDelta = (((cMax - b) * (HLSMax / 6)) + ((cMax - cMin) / 2)) / (cMax - cMin);

			if (r == cMax)
			{
				h = bDelta - gDelta;
			}
			else if (g == cMax)
			{
				h = (HLSMax / 3) + rDelta - bDelta;
			}
			else
			{ /* B == cMax */
				h = ((2 * HLSMax) / 3) + gDelta - rDelta;
			}

			if (h < 0)
			{
				h += HLSMax;
			}

			if (h > HLSMax)
			{
				h -= HLSMax;
			}
		}

		private static int HueToRGB(int n1, int n2, int hue)
		{
			if (hue < 0)
			{
				hue += HLSMax;
			}

			if (hue > HLSMax)
			{
				hue -= HLSMax;
			}

			/* return r,g, or b value from this tridrant */
			if (hue < (HLSMax / 6))
			{
				return (n1 + (((n2 - n1) * hue + (HLSMax / 12)) / (HLSMax / 6)));
			}

			if (hue < (HLSMax / 2))
			{
				return (n2);
			}

			if (hue < ((HLSMax * 2) / 3))
			{
				return (n1 + (((n2 - n1) * (((HLSMax * 2) / 3) - hue) + (HLSMax / 12)) / (HLSMax / 6)));
			}
			return (n1);
		}

		internal static Color HBS2Color(int hue, int lum, int sat, byte alpha)
		{
			int r;
			int g;
			int b;
			int Magic1;
			int Magic2;

			if (sat == 0)
			{            /* Achromatic */
				r = g = b = (lum * RGBMax) / HLSMax;
				// FIXME : Should throw exception if hue!=0
			}
			else
			{
				if (lum <= (HLSMax / 2))
				{
					Magic2 = (lum * (HLSMax + sat) + (HLSMax / 2)) / HLSMax;
				}
				else
				{
					Magic2 = sat + lum - ((sat * lum) + (HLSMax / 2)) / HLSMax;
				}
				Magic1 = 2 * lum - Magic2;

				r = Math.Min(255, (HueToRGB(Magic1, Magic2, hue + (HLSMax / 3)) * RGBMax + (HLSMax / 2)) / HLSMax);
				g = Math.Min(255, (HueToRGB(Magic1, Magic2, hue) * RGBMax + (HLSMax / 2)) / HLSMax);
				b = Math.Min(255, (HueToRGB(Magic1, Magic2, hue - (HLSMax / 3)) * RGBMax + (HLSMax / 2)) / HLSMax);
			}
			return (Color.FromArgb(alpha, (byte)r, (byte)g, (byte)b));
		}

		public static Color Light(Color baseColor)
		{
			return Light(baseColor, 0.5f);
		}

		public static Color Light(Color baseColor, float percOfLightLight)
		{
			int h, i, s;

			Color2HBS(baseColor, out h, out i, out s);
			int newIntensity = Math.Min(255, i + (int)((255 - i) * 0.5f * percOfLightLight));
			return HBS2Color(h, newIntensity, s,baseColor.A);
		}

		public static Color Dark(Color baseColor, float percOfDarkDark)
		{
			int h, i, s;
			Color2HBS(baseColor, out h, out i, out s);
			int preIntensity = Math.Max(0, i - (int)(i * 0.333f));
			int newIntensity = Math.Max(0, preIntensity - (int)(preIntensity * percOfDarkDark));
			return HBS2Color(h, newIntensity, s, baseColor.A);
		}

		public static Color Darken(Color baseColor, float perc)
		{
			int h, i, s;
			Color2HBS(baseColor, out h, out i, out s);
			int newIntensity = Math.Max(0, (int)(i - (i*perc)));
			return HBS2Color(h, newIntensity, s, baseColor.A);
		}

		public static Color Lighten(Color baseColor, float perc)
		{
			int h, i, s;
			Color2HBS(baseColor, out h, out i, out s);
			int newIntensity = Math.Min(HLSMax, (int)(i + ((HLSMax - i)*perc)));
			return HBS2Color(h, newIntensity, s, baseColor.A);
		}

		public static Color BlendScreen(Color first, Color second)
		{
			return Color.FromArgb(
					ScreenChannels(first.A, second.A),
					ScreenChannels(first.R, second.R),
					ScreenChannels(first.G, second.G),
					ScreenChannels(first.B, second.B));
		}

		private static byte ScreenChannels(byte first, byte second)
		{
			return (byte)(255 - (((255 - first)*(255-second))/255));
		}
	}
}
