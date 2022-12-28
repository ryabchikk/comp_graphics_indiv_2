namespace comp_graphics_indiv_2
{
    public partial class Form1 : Form
    {
        public List<Figure> scene = new List<Figure>();
        public List<Light> lights = new List<Light>();   // ������ ���������� �����
        public Color[,] pixelsColor;                    // ����� �������� ��� ����������� �� pictureBox
        public Point3D[,] pixels;
        public Point3D cameraPoint;
        public Point3D upLeft, upRight, downLeft, downRight;
        public int h, w;

        public Form1()
        {
            InitializeComponent();
            cameraPoint = new Point3D();
            upLeft = new Point3D();
            upRight = new Point3D();
            downLeft = new Point3D();
            downRight = new Point3D();
            h = pictureBox1.Height;
            w = pictureBox1.Width;
            pictureBox1.Image = new Bitmap(w, h);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BuildScene();
            BackwardRayTracing();
            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    (pictureBox1.Image as Bitmap).SetPixel(i, j, pixelsColor[i, j]);
                }
                pictureBox1.Invalidate();
            }
        }

        public void BuildScene()
        {
            //���� �������
            Figure room = Figure.GetCube(10);
            upLeft = room.sides[0].getPoint(0);
            upRight = room.sides[0].getPoint(1);
            downRight = room.sides[0].getPoint(2);
            downLeft = room.sides[0].getPoint(3);

            //������ ������
            Point3D normal = Side.norm(room.sides[0]);                            // ������� ������� �������
            Point3D center = (upLeft + upRight + downLeft + downRight) / 4;   // ����� ������� �������
            cameraPoint = center + normal * 11;

            //������ �����
            room.SetPen(new Pen(Color.Gray));
            room.sides[0].drawingPen = new Pen(Color.Green);
            room.sides[1].drawingPen = new Pen(Color.Green);
            room.sides[2].drawingPen = new Pen(Color.Green);
            room.sides[3].drawingPen = new Pen(Color.Green);
            room.fMaterial = new Material(0, 0, 0.05f, 0.7f);

            //��������� ��������� �����
            Light l1 = new Light(new Point3D(0f, 2f, 4.9f), new Point3D(1f, 1f, 1f));//�����, ������� �������,��� ������
            Light l2 = new Light(new Point3D(-4.9f, -4.9f, 4.9f), new Point3D(1f, 1f, 1f));//�����, ������� ������� ����� ����
            lights.Add(l1);
            //lights.Add(l2);

            Sphere mirrirSphere = new Sphere(new Point3D(-2.5f, -2, 2.5f), 2.5f);
            
            mirrirSphere.SetPen(new Pen(Color.White));
            mirrirSphere.fMaterial = new Material(0.9f, 0f, 0f, 0.1f, 1f);

            Figure bigCube = Figure.GetCube(2.8f);
            bigCube.Offset(-1.5f, 1.5f, -3.9f);//����� �� ���� 
            bigCube.RotateAround(70, "CZ");
            bigCube.SetPen(new Pen(Color.Magenta));
            bigCube.fMaterial = new Material(0f, 0f, 0.1f, 0.7f, 1.5f);

            Figure transCube = Figure.GetCube(2f);
            transCube.Offset(2f, 2f, -3.95f);
            transCube.RotateAround(-10, "CZ");
            transCube.SetPen(new Pen(Color.Red));
            transCube.fMaterial = new Material(0, 0.7f, 0.1f, 0.5f, 1f);

            scene.Add(room);
            scene.Add(bigCube);
            scene.Add(transCube);
            scene.Add(mirrirSphere);

            
            Figure mirrorCube = Figure.GetCube(1.5f);
            mirrorCube.Offset(-0.5f, -0.9f, -1.7f);
            mirrorCube.RotateAround(120, "CZ");
            mirrorCube.SetPen(new Pen(Color.White));
            mirrorCube.fMaterial = new Material(0.9f, 0f, 0f, 0.1f, 1.2f);
             

            
             
            Sphere smallSphere = new Sphere(new Point3D(-2.5f, 1.7f, -3.7f), 1.4f);
            smallSphere.SetPen(new Pen(Color.White));
            smallSphere.fMaterial = new Material(0.05f, 0.9f, 0f, 0f, 1.05f);
             

            scene.Add(mirrorCube);
            scene.Add(smallSphere);
        }

        public void BackwardRayTracing()
        {
            //���������� � ������ � ������� ��������,������ ������� ������
            GetPixels();
            
            //���������� ��������� ����� ����� �������� � ��� �����
            //���������� �������� �������� ����
            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    Ray ray = new Ray(cameraPoint, pixels[i, j]);
                    
                    ray.start = new Point3D(pixels[i, j]);
                    
                    Point3D color = RayTrace(ray, 10, 1);//���,���-�� ��������,�����
                    
                    if (color.x > 1.0f || color.y > 1.0f || color.z > 1.0f) { 
                        color = Point3D.Normal(color);
                    }
                        
                    pixelsColor[i, j] = Color.FromArgb((int)(255 * color.x), (int)(255 * color.y), (int)(255 * color.z));
                }
            }

        }

        // ��������� ���� �������� �����
        public void GetPixels()
        {
            
            //�������� ������� ����� �������� ������� � �������� ����������� �������� ���������� � ��������
            pixels = new Point3D[w, h];
            pixelsColor = new Color[w, h];
            
            Point3D step_up = (upRight - upLeft) / (w - 1);//��������� ������ ������� � ������ ������
            Point3D step_down = (downRight - downLeft) / (w - 1);//��������� ������ ������� � ������ ������
            Point3D up = new Point3D(upLeft);
            Point3D down = new Point3D(downLeft);
            
            for (int i = 0; i < w; ++i)
            {
                Point3D step_y = (up - down) / (h - 1);
                Point3D d = new Point3D(down);
                
                for (int j = 0; j < h; ++j)
                {
                    pixels[i, j] = d;
                    d += step_y;
                }
                
                up += step_up;
                down += step_down;
            }
        }

        //������ �� ����� ����������� ���� � ������� �� ��������� �����
        public bool IsVisible(Point3D lightPoint, Point3D hitPoint)
        {
            float max_t = (lightPoint - hitPoint).Length(); //������� ��������� ����� �� ����
            
            Ray r = new Ray(hitPoint, lightPoint);
            
            foreach (Figure fig in scene)
            {
                if (fig.FigureIntersection(r, out float t, out Point3D n)) {
                    
                    if (t < max_t && t > Figure.eps) {  
                        return false; 
                    }
                }   
            }
            return true;
        }

        public Point3D RayTrace(Ray ray, int iteration, float enviroment)
        {
            if (iteration <= 0) { 
                return new Point3D(0, 0, 0);
            }

            float rey_fig_intersect = 0;// ������� ����� ����������� ���� � ������� �� ����
            
            //������� ������� ������,� ������� ��������� ���
            Point3D normal = null;
            Material material = new Material();
            Point3D resultColor = new Point3D(0, 0, 0);
            
            //���� ������� ������
            bool refract_out_of_figure = false;

            foreach (Figure fig in scene)
            {
                if (fig.FigureIntersection(ray, out float intersect, out Point3D norm)) { 
                    
                    if (intersect < rey_fig_intersect || rey_fig_intersect == 0) {
                        rey_fig_intersect = intersect;
                        normal = norm;
                        material = new Material(fig.fMaterial);
                    }
                }
            }

            if (rey_fig_intersect == 0) {  
                return new Point3D(0, 0, 0);
            }
               

            //���� ����� ����������� ���� � �������� ������� ������
            //���������� �� ����� ����� � �����
            if (Point3D.Scalar(ray.direction, normal) > 0)
            {
                normal *= -1;
                refract_out_of_figure = true;
            }


            //����� ����������� ���� � �������
            Point3D hit_point = ray.start + ray.direction * rey_fig_intersect;

            foreach (Light light in lights)
            {
                //���� ����������� �������� �������� ���������
                Point3D ambientCoeficient = light.colorLight * material.ambient;
                
                ambientCoeficient.x = (ambientCoeficient.x * material.color.x);
                ambientCoeficient.y = (ambientCoeficient.y * material.color.y);
                ambientCoeficient.z = (ambientCoeficient.z * material.color.z);
                
                resultColor += ambientCoeficient;
                
                // ��������� ���������
                //���� ����� ����������� ���� � �������� ����� �� ��������� �����
                if (IsVisible(light.pointLight, hit_point)) { 
                    resultColor += light.Shade(hit_point, normal, material.color, material.diffuse);
                }
                    
            }

            if (material.reflection > 0) {
                Ray reflected_ray = ray.Reflect(hit_point, normal);
                resultColor += material.reflection * RayTrace(reflected_ray, iteration - 1, enviroment);
            }


            if (material.refraction > 0) {
               
                //� ����������� �� ����,�� ����� ����� � �����,����� �������� ����������� �����������
                float refract_coef;
                
                if (refract_out_of_figure) {  
                    refract_coef = material.environment;
                }
                else { 
                    refract_coef = 1 / material.environment;
                }
                    

                Ray refracted_ray = ray.Refract(hit_point, normal, material.refraction, refract_coef);//������� ������������ ���

                if (refracted_ray != null) { 
                    resultColor += material.refraction * RayTrace(refracted_ray, iteration - 1, material.environment);
                }
            }
            return resultColor;
        }
    }
}