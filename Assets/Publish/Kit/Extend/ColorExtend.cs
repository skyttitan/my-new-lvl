using UnityEngine;

namespace Kit.Extend
{
    public static class ColorExtend
    {
		#region basic
		/// <summary>Clone & modify alpha value, This method alloc double memory.</summary>
		/// <param name="self"></param>
		/// <param name="value"></param>
		/// <returns>return a new color with new alpha value.</returns>
        public static Color CloneAlpha(this Color self, float value)
        {
            self.a = value;
            return self;
        }

        public static bool Approximately(this Color self, Color target)
        {
			return
				Mathf.Approximately(self.r, target.r) &&
				Mathf.Approximately(self.g, target.g) &&
				Mathf.Approximately(self.b, target.b) &&
				Mathf.Approximately(self.a, target.a);
        }

        public static bool EqualRoughly(this Color self, Color target, float threshold = float.Epsilon)
		{
            return
                self.r.EqualRoughly(target.r, threshold) &&
                self.g.EqualRoughly(target.g, threshold) &&
                self.b.EqualRoughly(target.b, threshold) &&
                self.a.EqualRoughly(target.a, threshold);
        }

        public static Color TryParse(string RGBANumbers)
        {
            // clear up
            string[] param = RGBANumbers.Trim().Split(',');
            if (param == null || param.Length == 0)
                return Color.black;

            int pt = 0;
            int count = 0;
            bool Is255 = false;
            float[] rgba = new float[4]{ 0f,0f,0f,1f };
            
            while(param.Length > pt && count <= 4)
            {
                float tmp;
                if(float.TryParse(param[pt], out tmp))
                {
                    rgba[count] = tmp;
                    count++;
                    if (tmp > 1f) Is255 = true;
                }
                pt++;
            }

            // hotfix for 255
            if (Is255)
            {
                for (int i = 0; i < 3; i++) { rgba[i] /= 255f; }
                rgba[3] = Mathf.Clamp(rgba[3], 0f, 1f);
            }
            return new Color(rgba[0], rgba[1], rgba[2], rgba[3]);
        }

        public static Color Random(this Color color)
        {
            return color.RandomRange(Color.clear, Color.white);
        }

        public static Color RandomRange(this Color color, Color min, Color max)
        {
            color.r = UnityEngine.Random.Range(min.r, max.r);
            color.g = UnityEngine.Random.Range(min.g, max.g);
            color.b = UnityEngine.Random.Range(min.b, max.b);
            color.a = UnityEngine.Random.Range(min.a, max.a);
            return color;
        }
        #endregion
    }
}