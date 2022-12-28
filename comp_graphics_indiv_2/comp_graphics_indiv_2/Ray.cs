namespace comp_graphics_indiv_2
{
    public class Ray
    {
        public Point3D start, direction;

        public Ray(Point3D st, Point3D end)
        {
            start = new Point3D(st);
            direction = Point3D.Normal(end - st);
        }

        public Ray() { }

        //отражение
        public Ray Reflect(Point3D hit_point, Point3D normal)
        {
            //высчитываем направление отраженного луча
            Point3D reflect_dir = direction - 2 * normal * Point3D.Scalar(direction, normal);
            
            return new Ray(hit_point, hit_point + reflect_dir);
        }

        //преломление
        //все вычисления взяты из презентации
        public Ray Refract(Point3D hit_point, Point3D normal,float refraction ,float refract_coef)
        {
            
            Ray res_ray = new Ray();
            
            float sclr = Point3D.Scalar(normal, direction);
             
            //Если луч падает,то он проходит прямо,не преломляясь
            float n1n2div = refraction / refract_coef;
            float theta_formula = 1 - n1n2div*n1n2div * (1 - sclr * sclr);
            
            if (theta_formula >= 0) {
                float cos_theta = (float)Math.Sqrt(theta_formula);
                res_ray.start = new Point3D(hit_point);
                res_ray.direction = Point3D.Normal(direction * n1n2div - (cos_theta + n1n2div * sclr) * normal);
                return res_ray;
            }
            else { 
                return null;
            }
                
        }
    }

}
