using System.Collections.Generic;
using System.Collections.Immutable;

namespace EngineSynth.Common.Util
{
    public class Point
    {
        public float x, y;

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Automation
    {
        public float ScaleX = 1, ScaleY = 1;
        public string Name = "";
        private List<Point> points = new List<Point>();
        public List<Point> Points
        {
            get => points;
            set => points = value;
        }

        public Point AddPoint(float x, float y)
        {
            Point point = new(x, y);
            points.Add(point);
            Sort();

            return point;
        }

        public void RemovePoint(Point point)
        {
            points.Remove(point);
            Sort();
        }

        public ImmutableList<Point> GetPoints()
        {
            return points.ToImmutableList();
        }

        public void Sort()
        {
            points.Sort((p1, p2) => (int)(p1.x - p2.x));
        }

        public float GetValueAt(float x)
        {
            int i = 0;
            while (i < points.Count && x > points[i].x)
            {
                i++;
            }

            if (i == 0)
            {
                return points[0].y;
            }
            else if (i == points.Count)
            {
                return points[points.Count - 1].y;
            }
            else
            {
                float x0 = points[i - 1].x;
                float x1 = points[i].x;
                float y0 = points[i - 1].y;
                float y1 = points[i].y;

                return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
            }
        }
    }
}
