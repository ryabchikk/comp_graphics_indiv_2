namespace comp_graphics_indiv_2
{
    public class Material
    {
        public float reflection;    // отражение
        public float refraction;    // преломление
        public float ambient;       // фоновое освещение
        public float diffuse;       // диффузное освещение
        public float environment;   // преломление среды
        public Point3D color;       // цвет материала

        public Material(float reflection, float refraction, float ambient, float diffuse, float environment = 1)
        {
            this.reflection = reflection;
            this.refraction = refraction;
            this.ambient = ambient;
            this.diffuse = diffuse;
            this.environment = environment;
        }

        public Material(Material material)
        {
            reflection = material.reflection;
            refraction = material.refraction;
            environment = material.environment;
            ambient = material.ambient;
            diffuse = material.diffuse;
            color = new Point3D(material.color);
        }

        public Material() { }
    }
}
