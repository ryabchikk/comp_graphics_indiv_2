namespace comp_graphics_indiv_2
{
    public class Light : Figure
    {
        public Point3D pointLight;
        public Point3D colorLight;

        public Light(Point3D p, Point3D c)
        {
            pointLight = new Point3D(p);
            colorLight = new Point3D(c);
        }

        //вычисление локальной модели освещения
        public Point3D Shade(Point3D hitPoint, Point3D normal, Point3D materialColor, float diffuseCoeficient)
        {
            Point3D dir = pointLight - hitPoint;
            
            dir = Point3D.Normal(dir);

            //если угол между нормалью и направлением луча больше 90 градусов,то диффузное  освещение равно 0
            Point3D diff = diffuseCoeficient * colorLight * Math.Max(Point3D.Scalar(normal, dir), 0);
            
            return new Point3D(diff.x * materialColor.x, diff.y * materialColor.y, diff.z * materialColor.z);
        }
    }
}
