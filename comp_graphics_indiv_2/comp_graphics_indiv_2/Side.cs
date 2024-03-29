﻿namespace comp_graphics_indiv_2
{
    public class Side
    {
        public Figure host;
        public List<int> points = new List<int>();
        public Pen drawingPen = new Pen(Color.Black);
        public Point3D Normal;

        public Side(Figure h = null)
        {
            host = h;
        }

        public Point3D getPoint(int index)
        {
            if (host != null) { 
                return host.points[points[index]];
            }
                
            return null;
        }

        public static Point3D norm(Side S)
        {
            if (S.points.Count() < 3)
                return new Point3D(0, 0, 0);
            Point3D U = S.getPoint(1) - S.getPoint(0);
            Point3D V = S.getPoint(S.points.Count - 1) - S.getPoint(0);
            Point3D normal = U * V;
            return Point3D.Normal(normal);
        }
    }
}
