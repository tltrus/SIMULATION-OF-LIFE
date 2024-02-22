using System.Windows.Media;


namespace WpfApp.Classes
{
    class Particle
    {
        public Vector2D pos;
        public Vector2D velocity;

        public double speed;
        public bool isLive;
        public double radius;         

        public int lifetime;
        public double range;
        public double width, height;

        public Brush color;

        public void CollX()
        {
            // справа и слева нет границ. Сквозной пролет
            if (pos.x < 0)
                pos.x = width;
            if (pos.x > width)
                pos.x = 0;
        }
        public void CollY()
        {
            // сверху и снизу нет границ. Сквозной пролет
            if (pos.y < 0)
                pos.y = height;
            if (pos.y > height)
                pos.y = 0;
        }

        // Поворот объекта на случайный угол вправо или влево
        public void Turn(double speed, double angle)
        {
            velocity.Normalize();
            velocity.Rotate(angle);
            velocity.Mult(speed);
        }

        public void SetColor(Brush color) => this.color = color;

        public void SetRandomColor()
        {
            byte R = (byte)MainWindow.rnd.Next(255);
            byte G = (byte)MainWindow.rnd.Next(255);
            byte B = (byte)MainWindow.rnd.Next(255);

            this.color = new SolidColorBrush(Color.FromRgb(R, G, B)); 
        }

    }
}
