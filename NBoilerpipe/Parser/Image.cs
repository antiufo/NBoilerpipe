using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBoilerpipe.Parser
{

    public class Image : IComparable<Image>
    {
        private readonly String src;
        private readonly String width;
        private readonly String height;
        private readonly String alt;
        private readonly int area;

        public Image(String src, String width, String height, String alt)
        {
            this.src = src;
            if (src == null)
            {
                throw new ArgumentNullException("src attribute must not be null");
            }
            this.width = nullTrim(width);
            this.height = nullTrim(height);
            this.alt = nullTrim(alt);

            if (width != null && height != null)
            {
                int a;
                try
                {
                    a = int.Parse(width) * int.Parse(height);
                }
                catch
                {
                    a = -1;
                }
                this.area = a;
            }
            else
            {
                this.area = -1;
            }
        }

        public String getSrc()
        {
            return src;
        }

        public String getWidth()
        {
            return width;
        }

        public String getHeight()
        {
            return height;
        }

        public String getAlt()
        {
            return alt;
        }

        private static String nullTrim(String s)
        {
            if (s == null)
            {
                return null;
            }
            s = s.Trim();
            if (s.Length == 0)
            {
                return null;
            }
            return s;
        }

        /**
         * Returns the image's area (specified by width * height), or -1 if width/height weren't both specified or could not be parsed.
         * 
         * @return
         */
        public int getArea()
        {
            return area;
        }


        public override string ToString()
        {
            return src + "\twidth=" + width + "\theight=" + height + "\talt=" + alt + "\tarea=" + area;
        }





        public int CompareTo(Image o)
        {
            if (o == this)
            {
                return 0;
            }
            if (area > o.area)
            {
                return -1;
            }
            else if (area == o.area)
            {
                return src.CompareTo(o.src);
            }
            else
            {
                return 1;
            }
        }
    }

}
