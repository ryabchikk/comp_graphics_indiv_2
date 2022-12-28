namespace comp_graphics_indiv_2
{

    public class Figure
    {
        public static float eps = 0.0001f;
        public List<Point3D> points = new List<Point3D>(); 
        public List<Side> sides = new List<Side>();        
        public Material fMaterial;
        public Figure() { }


        public bool RayIntersectsTriangle(Ray r, Point3D p0, Point3D p1, Point3D p2, out float intersect)
        {
            intersect = -1;
            
            Point3D edge1 = p1 - p0;
            Point3D edge2 = p2 - p0;
            Point3D h = r.direction * edge2;
            
            float a = Point3D.Scalar(edge1, h);
            
            if (a > -eps && a < eps) {
                return false;       // Этот луч параллелен этому треугольнику.
            }
                
            float f = 1.0f / a;
            
            Point3D s = r.start - p0;
            
            float u = f * Point3D.Scalar(s, h);
            
            if (u < 0 || u > 1) {  
                return false;
            }
               
            Point3D q = s * edge1;
            
            float v = f * Point3D.Scalar(r.direction, q);
            
            if (v < 0 || u + v > 1) { 
                return false;
            }
                
            // На этом этапе мы можем вычислить t, чтобы узнать, где находится точка пересечения на линии.
            float t = f * Point3D.Scalar(edge2, q);

            if (t > eps) {
                intersect = t;
                return true;
            }
            else { //Это означает, что есть пересечение линий, но не пересечение лучей.
                 return false;
            }      
               
        }

        // пересечение луча с фигурой
        public virtual bool FigureIntersection(Ray r, out float intersect, out Point3D normal)
        {
            intersect = 0;
            normal = null;
            
            Side side = null;
            
            foreach (Side figure_side in sides)
            {
                //треугольная сторона
                if (figure_side.points.Count == 3) {
                    if (RayIntersectsTriangle(r, figure_side.getPoint(0), figure_side.getPoint(1), figure_side.getPoint(2), out float t) && (intersect == 0 || t < intersect)) {
                        intersect = t;
                        side = figure_side;
                    }
                }
                //четырехугольная сторона
                else if (figure_side.points.Count == 4) {
                    if (RayIntersectsTriangle(r, figure_side.getPoint(0), figure_side.getPoint(1), figure_side.getPoint(3), out float t) && (intersect == 0 || t < intersect)) {
                        intersect = t;
                        side = figure_side;
                    }
                    else if (RayIntersectsTriangle(r, figure_side.getPoint(1), figure_side.getPoint(2), figure_side.getPoint(3), out t) && (intersect == 0 || t < intersect)) {
                        intersect = t;
                        side = figure_side;
                    }
                }
            }
            if (intersect != 0) {
                normal = Side.norm(side);
                fMaterial.color = new Point3D(side.drawingPen.Color.R / 255f, side.drawingPen.Color.G / 255f, side.drawingPen.Color.B / 255f);
                return true;
            }
            
            return false;
        }

        public float[,] GetMatrix()
        {
            var res = new float[points.Count, 4];
            for (int i = 0; i < points.Count; i++)
            {
                res[i, 0] = points[i].x;
                res[i, 1] = points[i].y;
                res[i, 2] = points[i].z;
                res[i, 3] = 1;
            }
            return res;
        }

        public void ApplyMatrix(float[,] matrix)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].x = matrix[i, 0] / matrix[i, 3];
                points[i].y = matrix[i, 1] / matrix[i, 3];
                points[i].z = matrix[i, 2] / matrix[i, 3];
            }
        }

        private Point3D GetCenter()
        {
            Point3D res = new Point3D(0, 0, 0);
            foreach (Point3D p in points)
            {
                res.x += p.x;
                res.y += p.y;
                res.z += p.z;

            }
            
            res.x /= points.Count();
            res.y /= points.Count();
            res.z /= points.Count();
            
            return res;
        }

        public void RotateArondRad(float rangle, string type)
        {
            float[,] mt = GetMatrix();
            Point3D center = GetCenter();
            switch (type)
            {
                case "CX":
                    mt = ApplyOffset(mt, -center.x, -center.y, -center.z);
                    mt = ApplyRotationX(mt, rangle);
                    mt = ApplyOffset(mt, center.x, center.y, center.z);
                    break;
                case "CY":
                    mt = ApplyOffset(mt, -center.x, -center.y, -center.z);
                    mt = ApplyRotationY(mt, rangle);
                    mt = ApplyOffset(mt, center.x, center.y, center.z);
                    break;
                case "CZ":
                    mt = ApplyOffset(mt, -center.x, -center.y, -center.z);
                    mt = ApplyRotationZ(mt, rangle);
                    mt = ApplyOffset(mt, center.x, center.y, center.z);
                    break;
                case "X":
                    mt = ApplyRotationX(mt, rangle);
                    break;
                case "Y":
                    mt = ApplyRotationY(mt, rangle);
                    break;
                case "Z":
                    mt = ApplyRotationZ(mt, rangle);
                    break;
                default:
                    break;
            }
            ApplyMatrix(mt);
        }

        public void RotateAround(float angle, string type)
        {
            RotateArondRad(angle * (float)Math.PI / 180, type);
        }

        public void Offset(float xs, float ys, float zs)
        {
            ApplyMatrix(ApplyOffset(GetMatrix(), xs, ys, zs));
        }

        public void SetPen(Pen dw)
        {
            foreach (Side s in sides)
                s.drawingPen = dw;
        }

        private static float[,] MultiplyMatrix(float[,] m1, float[,] m2)
        {
            float[,] res = new float[m1.GetLength(0), m2.GetLength(1)];
            for (int i = 0; i < m1.GetLength(0); i++)
            {
                for (int j = 0; j < m2.GetLength(1); j++)
                {
                    for (int k = 0; k < m2.GetLength(0); k++)
                    {
                        res[i, j] += m1[i, k] * m2[k, j];
                    }
                }
            }
            return res;
        }

        private static float[,] ApplyOffset(float[,] transform_matrix, float offset_x, float offset_y, float offset_z)
        {
            float[,] translationMatrix = new float[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { offset_x, offset_y, offset_z, 1 } };
            return MultiplyMatrix(transform_matrix, translationMatrix);
        }

        private static float[,] ApplyRotationX(float[,] transform_matrix, float angle)
        {
            float[,] rotationMatrix = new float[,] { { 1, 0, 0, 0 }, { 0, (float)Math.Cos(angle), (float)Math.Sin(angle), 0 },
                { 0, -(float)Math.Sin(angle), (float)Math.Cos(angle), 0}, { 0, 0, 0, 1} };
            return MultiplyMatrix(transform_matrix, rotationMatrix);
        }

        private static float[,] ApplyRotationY(float[,] transform_matrix, float angle)
        {
            float[,] rotationMatrix = new float[,] { { (float)Math.Cos(angle), 0, -(float)Math.Sin(angle), 0 }, { 0, 1, 0, 0 },
                { (float)Math.Sin(angle), 0, (float)Math.Cos(angle), 0}, { 0, 0, 0, 1} };
            return MultiplyMatrix(transform_matrix, rotationMatrix);
        }

        private static float[,] ApplyRotationZ(float[,] transform_matrix, float angle)
        {
            float[,] rotationMatrix = new float[,] { { (float)Math.Cos(angle), (float)Math.Sin(angle), 0, 0 }, { -(float)Math.Sin(angle), (float)Math.Cos(angle), 0, 0 },
                { 0, 0, 1, 0 }, { 0, 0, 0, 1} };
            return MultiplyMatrix(transform_matrix, rotationMatrix);
        }

        static public Figure GetCube(float sz)
        {
            Figure res = new Figure();
            res.points.Add(new Point3D(sz / 2, sz / 2, sz / 2)); // 0 
            res.points.Add(new Point3D(-sz / 2, sz / 2, sz / 2)); // 1
            res.points.Add(new Point3D(-sz / 2, sz / 2, -sz / 2)); // 2
            res.points.Add(new Point3D(sz / 2, sz / 2, -sz / 2)); //3

            res.points.Add(new Point3D(sz / 2, -sz / 2, sz / 2)); // 4
            res.points.Add(new Point3D(-sz / 2, -sz / 2, sz / 2)); //5
            res.points.Add(new Point3D(-sz / 2, -sz / 2, -sz / 2)); // 6
            res.points.Add(new Point3D(sz / 2, -sz / 2, -sz / 2)); // 7

            Side s = new Side(res);
            s.points.AddRange(new int[] { 3, 2, 1, 0 });
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 4, 5, 6, 7 });
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 2, 6, 5, 1 });
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 0, 4, 7, 3 });
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 1, 5, 4, 0 });
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 2, 3, 7, 6 });
            res.sides.Add(s);
            return res;
        }
    }
}
