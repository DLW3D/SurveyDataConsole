using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConsole
{
    /// <summary>
    /// 二维矢量
    /// </summary>
    class Vector2
    {
        public double x;
        public double y;
        public double distance;
        public double angle;
        // 构造函数
        public Vector2() { }
        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
            distance = Math.Sqrt(Math.Pow(this.x, 2) + Math.Pow(this.y, 2));
            angle = Math.Atan2(this.y, this.x);
        }
        public Vector2(string x, string y)
        {
            this.x = Convert.ToDouble(x);
            this.y = Convert.ToDouble(y);
            distance = Math.Sqrt(Math.Pow(this.x, 2) + Math.Pow(this.y, 2));
            angle = Math.Atan2(this.y, this.x);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="angle"></param>
        /// <param name="accuracy">x,y精度(小数后位数)(-1不进行处理)</param>
        public Vector2(double distance, double angle, int accuracy)
        {
            this.distance = distance;
            this.angle = angle;
            x = distance * Math.Cos(angle * (Math.PI / 180));
            y = distance * Math.Sin(angle * (Math.PI / 180));
            if (accuracy != -1)
            {
                Accuracy(accuracy);
            }
        }
        /// <summary>
        /// 限制精度
        /// </summary>
        /// <param name="accuracy">x,y精度(小数后位数)</param>
        public void Accuracy(int accuracy)
        {
            x = Math.Round(x, accuracy);
            y = Math.Round(y, accuracy);
            distance = Math.Sqrt(Math.Pow(this.x, 2) + Math.Pow(this.y, 2));
            angle = Math.Atan2(this.y, this.x);
        }
        // 运算符重载
        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            try
            {
                return new Vector2(left.x + right.x, left.y + right.y);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }
        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            try
            {
                return new Vector2(left.x - right.x, left.y - right.y);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }
        public static Vector2 operator *(Vector2 left, double right)
        {
            try
            {
                return new Vector2(left.x * right, left.y * right);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }
        public static Vector2 operator /(Vector2 left, int right)
        {
            try
            {
                return new Vector2(left.x / right, left.y / right);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }

        override public string ToString()
        {
            return String.Format("({0},{1})", Math.Round(x, 3), Math.Round(y, 3));
        }
    }
}
