using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            RETRY:
            Console.WriteLine("选择计算内容,有两个已知方位角的附和导线计算:1,有一个已知方位角的闭合导线计算:2");
            switch (Console.ReadLine())
            {
                case "1":
                    ConnectingTraverse();
                    break;
                case "2":
                    CloseTraverse();
                    break;
                default:
                    goto RETRY;
            }

            Console.ReadKey();
        }

        static void ConnectingTraverse()
        {
            int n = LoadInt("请输入坐标数量int32");
            int sign = LoadSign("请输入角方向,左角1,右角-1");
            //输入已知坐标方位角
            List<Angle> angles = LoadVariables<Angle>(1, "请输入已知坐标方位角DD,MM,SS DD,MM,SS", n, new int[] { 0, n - 1 });
            //输入已知坐标
            List<Vector2> vectors = LoadVariables<Vector2>(1, "请输入已知坐标x,y x,y", n, new int[] { 0, n - 2 });
            //输入观测角
            List<Angle> anglesView = LoadVariables<Angle>(0, "请输入观测角DD,MM,SS", n, new int[] { 0, n - 2 });
            //输入已知坐标
            List<double> S = LoadVariables<double>(0, "请输入观测边长S", n, new int[] { 1, n - 2 });

            ConnectingTraverse table1 = new ConnectingTraverse();
            table1.Count(n, sign, angles, vectors, anglesView, S, 3);
            table1.Prints();
        }
        static void CloseTraverse()
        {
            int n = LoadInt("请输入坐标数量int32");
            int sign = LoadSign("请输入角方向,左角1,右角-1");
            //输入已知角
            Angle start = LoadVariables<Angle>(1, "请输入已知角", 1, new int[] { 0 })[0];
            //输入已知坐标方位角
            List<Angle> angles = LoadVariables<Angle>(1, "请输入已知坐标方位角DD,MM,SS", n, new int[] { 0 });
            //输入已知坐标
            List<Vector2> vectors = LoadVariables<Vector2>(1, "请输入已知坐标x,y", n, new int[] { 0 });
            //输入观测角
            List<Angle> anglesView = LoadVariables<Angle>(0, "请输入观测角DD,MM,SS", n, new int[] { 1, n - 1 });
            //输入已知坐标
            List<double> S = LoadVariables<double>(0, "请输入观测边长S", n, new int[] { 1, n - 1 });

            ClosedTraverses table1 = new ClosedTraverses();
            table1.Count(n, sign, start, angles, vectors, anglesView, S, 3);
            table1.Prints();
        }


        static int LoadInt(string word)
        {
            int n;
            RETRY:
            Console.WriteLine(word);
            try
            {
                n = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("[输入错误]只允许输入数字(Int32)");
                goto RETRY;
            }
            return n;
        }
        static int LoadSign(string word)
        {
            while (true)
            {
                Console.WriteLine(word);
                string read = Console.ReadLine();
                if (read == "1")
                {
                   return 1;
                }
                else if (read == "-1")
                {
                    return -1;
                }
                else
                {
                    Console.WriteLine("[输入错误]只允许输入1或-1");
                }
            }
        }

        /// <summary>
        /// 用户输入已知量《角/二维矢量/长度》(0:连续,1:离散)
        /// </summary>
        /// <param name="mod">模式:连续:0,离散:1</param>
        /// <param name="word">提示文字</param>
        /// <param name="n">总长度</param>
        /// <param name="layout">(从0计数)格式连续:{ 起始位置 , 结束位置 },格式离散:长度=输入数,值=插入位置</param>
        /// <returns></returns>
        static List<T> LoadVariables<T>(int mod, string word, int n, int[] layout)
        {
            RETRY:
            //读取输入
            Console.WriteLine(word);
            string[] read = Console.ReadLine().Split(' ');
            //规范化格式
            for (int i = 0; i < layout.Length; i++)
            {
                if (layout[i] < 0)
                {
                    layout[i] += n;
                }
            }
            //检查输入
            int correctLength;
            List<int> order = new List<int>();
            if (mod == 0)//连续
            {
                correctLength = layout[1] - layout[0] + 1;
                if (read.Length != correctLength)
                {
                    Console.WriteLine("[输入错误]输入数量错误,应输入{0}个数,输入了{1}个数", correctLength, read.Length);
                    goto RETRY;
                }
                for (int i = 0; i < read.Length; i++)
                {
                    order.Add(i + layout[0]);
                }
            }
            else if (mod == 1)//离散
            {
                correctLength = layout.Length;
                if (read.Length != correctLength)
                {
                    Console.WriteLine("[输入错误]输入数量错误,应输入{0}个数,输入了{1}个数", correctLength, read.Length);
                    goto RETRY;
                }
                for (int i = 0; i < read.Length; i++)
                {
                    order.Add(layout[i]);
                }
            }
            else
            {
                Console.WriteLine("Error:模式错误!");
                return null;
            }
            //载入内容
            List<T> v = new List<T>(new T[n]);
            for (int i = 0; i < read.Length; i++)
            {
                List<double> pics = new List<double>();
                // 检查输入是数字
                try
                {
                    string[] spl = read[i].Split(',');
                    foreach (string s in spl)
                    {
                        pics.Add(Convert.ToDouble(s));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    goto RETRY;
                }
                if (typeof(T) == typeof(Angle))// 角
                {
                    if (pics.Count != 3)
                    {
                        Console.WriteLine("[输入错误]输入格式错误,应输入 DD,MM,SS 为一个单位");
                        goto RETRY;
                    }
                    v[order[i]] = (T)(Object)new Angle(pics[0], pics[1], pics[2]);
                }
                else if (typeof(T) == typeof(Vector2))// 二维矢量
                {
                    if (pics.Count != 2)
                    {
                        Console.WriteLine("[输入错误]输入格式错误,应输入 x,y 为一个单位");
                        goto RETRY;
                    }
                    v[order[i]] = (T)(Object)new Vector2(pics[0], pics[1]);
                }
                else if (typeof(T) == typeof(double))// 长度
                {
                    if (pics.Count != 1)
                    {
                        Console.WriteLine("[输入错误]输入格式错误,不应有\",\"");
                        goto RETRY;
                    }
                    v[order[i]] = (T)(Object)pics[0];
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;// 设置前景色
                    Console.WriteLine("[类型错误]无法识别的泛型");
                    Console.ResetColor();//将控制台的前景色和背景色设为默认值
                    goto RETRY;
                }
            }
            return v;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    

    class Test
    {

    }
}
