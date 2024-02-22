using System;
using System.Windows;

namespace WpfApp.Classes
{
    public class Vector2D
    {
        public double x { get; set; }

        public double y { get; set; }

        public Vector2D(double x = 0.0, double y = 0.0)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2D CopyToVector()
        {
            return new Vector2D(x, y);
        }

        public double[] CopyToArray()
        {
            return new double[2] { x, y };
        }

        public void Set(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"Vector2D Object: [ {x}, {y} ]";
        }

        public Point ToPoint()
        {
            return new Point(x, y);
        }

        public Vector2D Add(double x, double y)
        {
            Vector2D vector2D = new Vector2D();
            this.x += x;
            this.y += y;
            return CopyToVector();
        }

        public Vector2D Add(Vector2D v)
        {
            Vector2D vector2D = new Vector2D();
            x += v.x;
            y += v.y;
            return CopyToVector();
        }

        public static Vector2D Add(Vector2D v1, Vector2D v2)
        {
            Vector2D vector2D = new Vector2D();
            vector2D = v1.CopyToVector();
            vector2D.Add(v2);
            return vector2D;
        }

        public Vector2D Sub(double x, double y)
        {
            Vector2D vector2D = new Vector2D();
            this.x -= x;
            this.y -= y;
            return CopyToVector();
        }

        public Vector2D Sub(Vector2D v)
        {
            Vector2D vector2D = new Vector2D();
            x -= v.x;
            y -= v.y;
            return CopyToVector();
        }

        public static Vector2D Sub(Vector2D v1, Vector2D v2)
        {
            Vector2D vector2D = new Vector2D();
            vector2D = v1.CopyToVector();
            vector2D.Sub(v2);
            return vector2D;
        }

        public Vector2D Div(double n)
        {
            Vector2D vector2D = new Vector2D();
            x /= n;
            y /= n;
            return CopyToVector();
        }

        public Vector2D Div(Vector2D v)
        {
            Vector2D vector2D = new Vector2D();
            x /= v.x;
            y /= v.y;
            return CopyToVector();
        }

        public static Vector2D Div(Vector2D v1, Vector2D v2)
        {
            Vector2D vector2D = new Vector2D();
            vector2D = v1.CopyToVector();
            vector2D.Div(v2);
            return vector2D;
        }

        public static Vector2D Div(Vector2D v1, double n)
        {
            Vector2D vector2D = new Vector2D();
            vector2D = v1.CopyToVector();
            vector2D.Div(n);
            return vector2D;
        }

        public Vector2D Mult(double n)
        {
            Vector2D vector2D = new Vector2D();
            x *= n;
            y *= n;
            return CopyToVector();
        }

        public static Vector2D Mult(Vector2D v, double val)
        {
            Vector2D vector2D = new Vector2D();
            vector2D = v.CopyToVector();
            vector2D.Mult(val);
            return vector2D;
        }

        public double Dot(Vector2D v)
        {
            return x * v.x + y * v.y;
        }

        public double Cross(Vector2D v)
        {
            return x = x * v.y - y * v.x;
        }

        public Vector2D Lerp(double x, double y, double amt)
        {
            Vector2D vector2D = new Vector2D();
            this.x += (x - this.x) * amt;
            this.y += (y - this.y) * amt;
            return CopyToVector();
        }

        public Vector2D Lerp(Vector2D v, double amt)
        {
            Vector2D vector2D = new Vector2D();
            x += (v.x - x) * amt;
            y += (v.y - y) * amt;
            return CopyToVector();
        }

        public static Vector2D Lerp(Vector2D v1, Vector2D v2, double amt)
        {
            Vector2D vector2D = new Vector2D();
            vector2D = v1.CopyToVector();
            vector2D.Lerp(v2, amt);
            return vector2D;
        }

        public double HeadingRad()
        {
            return Math.Atan2(y, x);
        }

        public double HeadingDeg()
        {
            double num = HeadingRad();
            return (num >= 0.0) ? (num * 180.0 / Math.PI) : ((Math.PI * 2.0 + num) * 360.0 / (Math.PI * 2.0));
        }

        public double angleBetween(Vector2D v)
        {
            double val = Dot(v) / (Mag() * v.Mag());
            return Math.Acos(Math.Min(1.0, Math.Max(-1.0, val)));
        }

        public Vector2D Rotate(double a)
        {
            Vector2D vector2D = new Vector2D();
            double num = HeadingRad() + a;
            double num2 = Mag();
            x = Math.Cos(num) * num2;
            y = Math.Sin(num) * num2;
            return CopyToVector();
        }

        public double MagSq()
        {
            double num = x;
            double num2 = y;
            return num * num + num2 * num2;
        }

        public Vector2D Limit(double max)
        {
            Vector2D vector2D = new Vector2D();
            double num = MagSq();
            if (num > max * max)
            {
                Div(Math.Sqrt(num)).Mult(max);
            }

            return CopyToVector();
        }

        public double Mag()
        {
            return Math.Sqrt(MagSq());
        }

        public Vector2D Normalize()
        {
            double num = Mag();
            if (num != 0.0)
            {
                Mult(1.0 / num);
            }

            return this;
        }

        public void SetMag(int n)
        {
            Normalize().Mult(n);
        }

        public void SetMag(double n)
        {
            Normalize().Mult(n);
        }

        public static Vector2D FromAngle(double angle, double length = 1.0)
        {
            return new Vector2D(length * Math.Cos(angle), length * Math.Sin(angle));
        }

        public Vector2D Random2D(Random rnd)
        {
            return FromAngle(rnd.NextDouble() * Math.PI * 2.0);
        }

        public double Dist(Vector2D v)
        {
            return Sub(this, v).Mag();
        }

        public static double Dist(Vector2D v1, Vector2D v2)
        {
            return v1.Dist(v2);
        }

        public static Vector2D NormalVector(Vector2D a)
        {
            double num = 0.0;
            double num2 = 0.0;
            if (a.x != 0.0)
            {
                num2 = 1.0;
                num = (0.0 - a.y) * num2 / a.x;
            }
            else if (a.y != 0.0)
            {
                num = 1.0;
                num2 = (0.0 - a.x) * num / a.y;
            }

            return new Vector2D(num, num2);
        }

        public static Vector2D operator +(Vector2D left, Vector2D right)
        {
            return Add(left, right);
        }

        public static Vector2D operator -(Vector2D value)
        {
            return value.Mult(-1.0);
        }

        public static Vector2D operator -(Vector2D left, Vector2D right)
        {
            return Sub(left, right);
        }

        public static Vector2D operator *(float left, Vector2D right)
        {
            return Mult(right, left);
        }

        public static Vector2D operator *(Vector2D left, Vector2D right)
        {
            return new Vector2D
            {
                x = left.x * right.x,
                y = left.y * right.y
            };
        }

        public static Vector2D operator *(Vector2D left, float right)
        {
            return Mult(left, right);
        }

        public static Vector2D operator /(Vector2D left, Vector2D right)
        {
            return Div(left, right);
        }

        public static Vector2D operator /(Vector2D value1, float value2)
        {
            return Div(value1, value2);
        }

        public static bool operator ==(Vector2D left, Vector2D right)
        {
            if (left.x == right.x && left.y == right.y)
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(Vector2D left, Vector2D right)
        {
            if (left.x != right.x || left.y != right.y)
            {
                return true;
            }

            return false;
        }
    }

}
