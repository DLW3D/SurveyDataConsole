using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConsole
{
    static class Survey
    {
        /// <summary>
        /// 将非零的角直接加起来
        /// </summary>
        /// <param name="angles"></param>
        /// <returns></returns>
        static public Angle SumAngle(List<Angle> angles)
        {
            Angle angle = new Angle();
            foreach (Angle ang in angles)
            {
                if (ang != null)
                {
                    angle += ang;
                }
            }
            return angle;
        }
        /// <summary>
        /// 导线方位角计算
        /// </summary>
        /// <param name="start">起始方位角</param>
        /// <param name="add">观测角(改正后)</param>
        /// <param name="sign">角方向(1/-1)</param>
        /// <returns>标准化的坐标方位角集合</returns>
        static public List<Angle> SumTraverse(Angle start, List<Angle> add, int sign)
        {
            List<Angle> angle = new List<Angle>();
            angle.Add(start);
            foreach (Angle ang in add)
            {
                if (ang != null)
                {
                    Angle newangle = angle.Last() + ang * sign + new Angle(180);
                    newangle.Standardization();
                    angle.Add(newangle);
                }
            }
            return angle;
        }
        /// <summary>
        /// 导线坐标计算
        /// </summary>
        /// <param name="start">起始坐标</param>
        /// <param name="add">坐标增量(改正后)</param>
        /// <returns>坐标集合</returns>
        static public List<Vector2> SumTraverse(Vector2 start, List<Vector2> add)
        {
            List<Vector2> vector = new List<Vector2>();
            vector.Add(start);
            foreach (Vector2 s in add)
            {
                if (s != null)
                {
                    Vector2 newvector = vector.Last() + s;
                    vector.Add(newvector);
                }
            }
            return vector;
        }
        /// <summary>
        /// 平分角度改正
        /// </summary>
        /// <param name="n">角度数量</param>
        /// <param name="dAngle">改正值(闭合差)</param>
        /// <param name="sign">角方向(1/-1)</param>
        /// <returns></returns>
        static public List<Angle> AngleCorrection(int n, Angle dAngle, int sign)
        {
            Angle dbAngle;
            List<Angle> db = new List<Angle>(new Angle[n]);
            dbAngle = dAngle / (n) * -sign;
            for (int i = 0; i < n; i++)
            {
                db[i] = dbAngle;
            }
            // 处理余数
            for (int i = 0; i < Math.Abs(dAngle % (n)); i++)
            {
                db[i] += new Angle(0, 0, Math.Sign(dAngle.GetD() * -sign));
            }
            return db;
        }
        /// <summary>
        /// 根据导线长角度改正
        /// </summary>
        /// <param name="dAngle">改正值(闭合差)</param>
        /// <param name="s">导线长度集合</param>
        /// <returns></returns>
        static public List<Angle> AngleCorrection(Angle dAngle, List<double> s)
        {
            int n = s.Count();
            int start = -1;
            // 检查导线类型(1:闭合 2:附合)
            if (s.Last() == 0)
            {
                n -= 1;
                start = 0;
            }
            else
            {
                start = 1;
            }
            // 取平分值并赋值
            Angle dbAngle = dAngle / -(n - start);
            List<Angle> db = new List<Angle>(new Angle[n]);
            for (int i = start; i < n; i++)
            {
                db[i] = dbAngle;
            }
            // 以下根据导线长度添加余数
            List<int> sort = Sort(s);// 对序号根据值排序
            List<int> added = new List<int>();// 已计算序号
            for (int i = 0; i < Math.Abs(dAngle % (n - start)); i++)// 余数数量循环
            {
                for (int ii = 0; ii < sort.Count; ii++)// 查找最小边长
                {
                    // "左"角
                    int index = sort[ii] - 1;
                    if (index < start) index = n - 1;
                    if (!added.Exists(delegate (int a) { return a == index; }))
                    {
                        db[index] += new Angle(0, 0, Math.Sign(-dAngle.GetD()));
                        added.Add(index);
                        break;
                    }
                    // "右"角
                    index++;
                    if (index >= n) index = start;
                    if (!added.Exists(delegate (int a) { return a == index; }))
                    {
                        db[index] += new Angle(0, 0, Math.Sign(dbAngle.GetD()));
                        added.Add(index);
                        break;
                    }
                }
            }
            return db;
        }
        /// <summary>
        /// 坐标增量改正
        /// </summary>
        /// <param name="dVector">改正量(闭合差)</param>
        /// <param name="s">导线长集合</param>
        /// <returns>根据导线长格式化的改正量集合</returns>
        static public List<Vector2> VectorCorrection(Vector2 dVector, List<double> s, int fix)
        {
            List<Vector2> dbvectors = new List<Vector2>(new Vector2[s.Count]);
            double es = 0;
            foreach (double i in s)
            {
                es += i;
            }
            Vector2 ddv = new Vector2();
            for (int i = 0; i < s.Count; i++)
            {
                if (s[i] != 0) 
                {
                    dbvectors[i] = dVector * -(s[i] / es);
                    if (fix != -1)
                    {
                        dbvectors[i].Accuracy(fix);
                    }
                    ddv += dbvectors[i];
                }
            }
            // 检查改正
            double ddx = Math.Round(ddv.x + dVector.x, fix);// 计算四舍五入导致的改正误差
            int ddxc = (int)Math.Abs(Math.Round(ddx * Math.Pow(10, fix)));// 将该误差按精度化为整数
            double ddy = Math.Round(ddv.y + dVector.y, fix);
            int ddyc = (int)Math.Abs(Math.Round(ddy * Math.Pow(10, fix)));
            // 修正四舍五入导致的改正误差
            List<int> sort = Sort(s);// 对序号根据值排序
            List<int> added = new List<int>();// 已计算序号
            for (int i = 0; i < ddxc; i++)// x余数循环
            {
                for (int ii = sort.Count - 1; ii >= 0; ii--)// 距离从大到小循环
                {
                    int index = sort[ii];
                    if (s[index] != 0 && !added.Exists(delegate (int a) { return a == index; }))// 不为0且不重复
                    {
                        dbvectors[index] += new Vector2(Math.Sign(-ddx) * Math.Pow(0.1, fix), 0);
                        added.Add(index);
                        break;
                    }
                }
            }
            added.Clear();
            for (int i = 0; i < ddyc; i++)// y余数循环
            {
                for (int ii = sort.Count - 1; ii >= 0; ii--)// 距离从大到小循环
                {
                    int index = sort[ii];
                    if (s[index] != 0 && !added.Exists(delegate (int a) { return a == index; }))// 不为0且不重复
                    {
                        dbvectors[index] += new Vector2(0, Math.Sign(-ddy) * Math.Pow(0.1, fix));
                        added.Add(index);
                        break;
                    }
                }
            }
            return dbvectors;
        }

        /// <summary>
        ///  对序号根据值排序(值为0的放最后)
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        static public List<int> Sort(List<double> ts)
        {
            List<int> list = new List<int>();
            while (list.Count < ts.Count)
            {
                int index = 0;
                double min = 99999999;
                for (int i = 0; i < ts.Count; i++)
                {
                    if (ts[i] == 0)
                    {
                        continue;
                    }
                    // 奇怪的操作,来自
                    // https://stackoverflow.com/questions/1710301/what-is-a-predicate-in-c
                    Predicate<int> finder = delegate (int a) { return a == i; };
                    //已经找过的跳过
                    if (list.Exists(finder)) continue;
                    if (ts[i] <= min)
                    {
                        index = i;
                        min = ts[i];
                    }
                }
                if (ts[index] == 0)
                {
                    break;
                }
                list.Add(index);
            }
            return list;
        }

        /// <summary>
        /// 打印表格
        /// </summary>
        /// <param name="n">列数</param>
        /// <param name="keys">表头</param>
        /// <param name="values">表格内容List<List<Object>></param>
        /// <param name="exkeys">额外参数名</param>
        /// <param name="exvalues">额外参数</param>
        static public void Prints(int n, List<string> keys, List<Object> values, List<string> exkeys, List<Object> exvalues, int accuracy)
        {
            // 表头
            Console.Write("|");
            for (int i = 0; i < keys.Count; i++)
            {
                if (values[i] is List<Angle>)
                {
                    Console.Write("{0,5}|", keys[i]);
                }
                else if (values[i] is List<Vector2>)
                {
                    Console.Write("({0,5},{1,5})|", keys[i] + "x", keys[i] + "y");
                }
                else if (values[i] is List<double>)
                {
                    Console.Write("{0,5}|", keys[i]);
                }
            }
            Console.Write("\r\n");
            // 主体
            for (int ii = 0; ii < n; ii++)// 行数循环
            {
                // 精度标识
                string acc = "";
                if (accuracy != -1)
                {
                    acc += ":.";
                    for (int iii = 0; iii < accuracy; iii++)
                    {
                        acc += "0";
                    }
                }
                // 开始输出
                Console.Write("|");
                for (int i = 0; i < keys.Count; i++)// 列数循环
                {
                    if (values[i] is List<Angle>)// 角
                    {
                        Angle v = ((List<Angle>)values[i])[ii];
                        Console.Write("{0,9:.0000}|", v == null ? 0 : v.GetDMS());
                    }
                    else if (values[i] is List<Vector2>)// 矢量
                    {
                        Vector2 v = ((List<Vector2>)values[i])[ii];
                        Console.Write("({0,9"+acc+"},{1,10"+acc+"})|", (v == null ? 0 : Math.Round(v.x, 3)), (v == null ? 0 : Math.Round(v.y, 3)));
                    }
                    else if (values[i] is List<double>)// 长度
                    {
                        Console.Write("{0,9"+acc+"}|", ((List<double>)values[i])[ii]);
                    }
                }
                Console.Write("\r\n");
            }
            // 额外数据
            for (int i = 0; i < exkeys.Count; i++)
            {
                Console.Write("{0}={1:.######}, ", exkeys[i], exvalues[i]);
            }
            Console.Write("\r\n");
        }
    }
}
