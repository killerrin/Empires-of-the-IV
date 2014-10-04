#pragma once
namespace Anarian {
	struct Color
	{
		float R, G, B, A;

		// Constructors
		Color() { };
		Color(float r, float g, float b, float a = 1.0f)
		{
			if (r > 1.0f) r /= 255.0f;
			if (g > 1.0f) g /= 255.0f;
			if (b > 1.0f) b /= 255.0f;
			if (a > 1.0f) a /= 255.0f;

			R = r;
			G = g;
			B = b;
			A = a;
		};
		Color(const Color& c)
		{
			R = c.R;
			G = c.G;
			B = c.B;
			A = c.A;
		};

		// Helper Functions
		Color Normalize()
		{
			Color color = Color(*this);
			color.NormalizeThis();
			return color;
		};
		void NormalizeThis()
		{
			if (R > 1.0f) R /= 255.0f;
			if (G > 1.0f) G /= 255.0f;
			if (B > 1.0f) B /= 255.0f;
			if (A > 1.0f) A /= 255.0f;
		};

		// Operator Overloads
		Color& operator = (const Color& c)
		{
			R = c.R;
			G = c.G;
			B = c.B;
			A = c.A;

			return *this;
		};

		const float* operator [] (int index) const
		{
			return (&R + index);
		};


		// Static Colors
		static Color Black()			{ return Color(0.0f, 0.0f, 0.0f, 1.0f); };
		static Color White()			{ return Color(1.0f, 1.0f, 1.0f, 1.0f); };
		static Color Red()				{ return Color(1.0f, 0.0f, 0.0f, 1.0f); };
		static Color Green()			{ return Color(0.0f, 1.0f, 0.0f, 1.0f); };
		static Color Blue()				{ return Color(0.0f, 0.0f, 1.0f, 1.0f); };

		static Color CornFlowerBlue()	{ return Color(0.0f, 0.2f, 0.4f, 1.0f); };
		static Color LemonChiffon()		{ return Color(1.0f, 0.984f, 0.815f, 1.0f); };
	};
}