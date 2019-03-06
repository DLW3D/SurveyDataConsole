using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConsole
{
    /// <summary>
    /// 角度
    /// </summary>
    class Angle
    {
        public double angle;
        public int degree, minute, second;
        // 度构造
        public Angle() { }
        public Angle(string angle)
        {
            this.angle = Convert.ToDouble(angle);
            TransDDMS();
        }
        public Angle(int angle)
        {
            this.angle = angle;
            TransDDMS();
        }
        public Angle(float angle)
        {
            this.angle = angle;
            TransDDMS();
        }
        public Angle(double angle)
        {
            this.angle = angle;
            TransDDMS();
        }
        // 度分秒构造
        public Angle(int degree, int minute, int second)
        {
            this.degree = degree;
            this.minute = minute;
            this.second = second;
            TransDMSD();
        }
        public Angle(double degree, double minute, double second)
        {
            this.degree = Convert.ToInt32(degree);
            this.minute = Convert.ToInt32(minute);
            this.second = Convert.ToInt32(second);
            TransDMSD();
        }
        public Angle(string degree, string minute, string second)
        {
            this.degree = Convert.ToInt32(degree);
            this.minute = Convert.ToInt32(minute);
            this.second = Convert.ToInt32(second);
            TransDMSD();
        }
        /// <summary>
        /// 度转化度分秒
        /// </summary>
        public void TransDDMS()
        {
            double k = angle;
            degree = (int)k;
            double p = Math.Round((k - degree) * 60, 5);
            minute = (int)p;
            double a = Math.Round((p - minute) * 60);
            second = (int)a;
        }
        /// <summary>
        /// 度分秒转化度
        /// </summary>
        public void TransDMSD()
        {
            angle = degree + (double)minute / 60 + (double)second / 3600;
        }
        /// <summary>
        /// 标准化(0~360)
        /// </summary>
        public void Standardization()
        {
            while (angle < 0)
            {
                angle += 360;
            }
            while (angle >= 360)
            {
                angle -= 360;
            }
            TransDDMS();
        }
        /// <summary>
        /// 以DD.MMSS形式输出
        /// </summary>
        /// <returns>DD.MMSS</returns>
        public double GetDMS()
        {
            return degree + (double)minute / 100 + (double)second / 10000;
        }
        /// <summary>
        /// 以double[]{度,分,秒}形式输出
        /// </summary>
        /// <returns>DD.MMSS</returns>
        public double[] GetDMSs()
        {
            return new double[] { degree, minute, second };
        }
        /// <summary>
        /// 以标准化DD.MMSS形式输出
        /// </summary>
        /// <returns>DD.MMSS</returns>
        public double GetDMSSt()
        {
            Angle angle = new Angle(this.angle);
            angle.Standardization();
            return angle.degree + (double)angle.minute / 100 + (double)angle.second / 10000;
        }
        /// <summary>
        /// 以标准化double[]{度,分,秒}形式输出
        /// </summary>
        /// <returns>DD.MMSS</returns>
        public double[] GetDMSSts()
        {
            Angle angle = new Angle(this.angle);
            angle.Standardization();
            return new double[] { angle.degree, angle.minute, angle.second };
        }
        /// <summary>
        /// 以有理数形式输出
        /// </summary>
        /// <returns></returns>
        public double GetD()
        {
            return angle;
        }
        /// <summary>
        /// 以标准化有理数形式输出
        /// </summary>
        /// <returns></returns>
        public double GetDSt()
        {
            Angle angle = new Angle(this.angle);
            angle.Standardization();
            return angle.angle;
        }
        // 运算符重载
        public static Angle operator +(Angle left, Angle right)
        {
            Angle angle;
            try
            {
                angle = new Angle(left.angle + right.angle);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }
            return angle;
        }
        public static Angle operator -(Angle left, Angle right)
        {
            Angle angle;
            try
            {
                angle = new Angle(left.angle - right.angle);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }
            return angle;
        }
        public static Angle operator *(Angle left, int right)
        {
            Angle angle;
            try
            {
                angle = new Angle(left.angle * right);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }
            return angle;
        }
        public static Angle operator /(Angle left, int right)
        {
            Angle angle;
            try
            {
                int d = left.degree;
                int m = left.minute;
                int s = left.second;
                angle = new Angle(d / right, (m + (d % right) * 60) / right, (s + ((m + (d % right) * 60) % right) * 60) / right);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }
            return angle;
        }
        public static int operator %(Angle left, int right)
        {
            int angle;
            try
            {
                //仅适用于最高位为秒的角度
                angle = ((int)(left.GetDMS() * 10000)) % right;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }
            return angle;
        }
        
        override public string ToString()
        {
            return degree + "°" + minute + "'" + second + "''";
        }
    }
}
