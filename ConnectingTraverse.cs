using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConsole
{
    // 具有两个连接角的附和图根导线计算
    class ConnectingTraverse
    {
        // 点数=5
        int n = 5;
        // 左右角
        int sign = -1;
        // 观测角β
        List<Angle> b;
        // 观测角β改正值dβ            ;
        List<Angle> db;
        // 观测角β改正后值β'          ;
        List<Angle> bc;
        // 坐标方位角α                 ;
        List<Angle> a;
        // 边长S                        ;
        List<double> s;
        // 坐标增量Δx,y                ;
        List<Vector2> deltaVector;
        // 坐标改正dx,y                 ;
        List<Vector2> ddeltaVector;
        // 坐标增量改正后值Δx',y'      ;
        List<Vector2> cdeltaVector;
        // 坐标(x,y)                    ;
        List<Vector2> vector;

        // 观测角之和Eβ
        Angle eb;
        // 计算得终点坐标方位角α'
        Angle tAngle;
        // 角度闭合差Δα
        Angle dAngle;
        // 改正后终点坐标方位角α''
        Angle ttAngle;
        // 坐标增量和
        Vector2 edeltaVector;
        // 坐标闭合差fx,y
        Vector2 dVector;
        // 改正后终点坐标x'',y''
        Vector2 ddVector;
        // 导线总长ES
        double es;
        // 坐标精度
        int accuracy;

        public ConnectingTraverse()
        {

        }

        /// <summary>
        /// 具有两个连接角的附和图根导线计算
        /// </summary>
        /// <param name="n">点数</param>
        /// <param name="sign">左右角,左角1,右角-1</param>
        /// <param name="angles">已知坐标方位角</param>
        /// <param name="vectors">已知坐标</param>
        /// <param name="anglesView">观测角</param>
        /// <param name="s">观测边长</param>
        /// <param name="accuracy">精度(坐标小数点后位数)(-1不作限制)</param>
        public void Count(int n, int sign, List<Angle> angles, List<Vector2> vectors, List<Angle> anglesView, List<double> s, int accuracy)
        {
            this.n = n;
            this.sign = sign;
            this.accuracy = accuracy;
            this.b = new List<Angle>(anglesView);
            this.db = new List<Angle>(new Angle[n]);
            this.bc = new List<Angle>(new Angle[n]);
            this.a = new List<Angle>(angles);
            this.s = new List<double>(s);
            this.deltaVector = new List<Vector2>(new Vector2[n]);
            this.ddeltaVector = new List<Vector2>(new Vector2[n]);
            this.cdeltaVector = new List<Vector2>(new Vector2[n]);
            this.vector = new List<Vector2>(vectors);

            // 计算观测角之和Eβ
            eb = Survey.SumAngle(b);
            // 计算终点坐标方位角α'
            tAngle = Survey.SumTraverse(a[0], b, sign).Last();
            // 计算角度闭合差Δα
            dAngle = tAngle - a[n - 1];
            // 计算改正值dβ
            db = new List<Angle>();
            //db.AddRange(Survey.AngleCorrection(n - 1, dAngle, sign));
            db.AddRange(Survey.AngleCorrection(dAngle, s));
            db.Add(null);
            // 计算改正后值β'
            for (int i = 0; i < n - 1; i++)
            {
                bc[i] = b[i] + db[i];
            }
            // 计算改正后终点坐标方位角α''
            ttAngle = Survey.SumTraverse(a[0], bc, sign).Last();
            // 检查改正
            if (ttAngle.GetDMS() != a[n - 1].GetDMS())
            {
                Console.ForegroundColor = ConsoleColor.Red;// 设置前景色
                Console.WriteLine("Error:角度改正错误,{0},{1}", a[n - 1].GetDMS(), ttAngle.GetDMS());
                Console.ResetColor();//将控制台的前景色和背景色设为默认值
            }
            // 计算途中坐标方位角
            a = Survey.SumTraverse(a[0], bc, sign);
            // 计算坐标增量Δx,y
            for (int i = 1; i < n - 1; i++)
            {
                deltaVector[i] = new Vector2(s[i], a[i].angle, accuracy);
            }
            // 计算坐标增量和
            edeltaVector = Survey.SumTraverse(vector[0], deltaVector).Last() - vector[0];
            // 计算坐标闭合差fx,y
            dVector = Survey.SumTraverse(vector[0], deltaVector).Last() - vector[n - 2];
            // 计算导线总长
            es = 0;
            for (int i = 1; i < n - 1; i++)
            {
                es += s[i];
            }
            // 计算改正dx,y
            ddeltaVector = Survey.VectorCorrection(dVector, s, accuracy);
            for (int i = 0; i < ddeltaVector.Count; i++)
            {
                if (ddeltaVector[i] != null)
                {
                    ddeltaVector[i].Accuracy(accuracy);
                }
            }
            // 计算改正后坐标增量Δx',y'
            for (int i = 1; i < n - 1; i++)
            {
                cdeltaVector[i] = deltaVector[i] + ddeltaVector[i];
            }
            // 计算改正后终点坐标x'',y''
            ddVector = vector[0];
            for (int i = 1; i < n - 1; i++)
            {
                ddVector += cdeltaVector[i];
            }
            ddVector = Survey.SumTraverse(vector[0], cdeltaVector).Last();
            // 检查改正
            int acc = accuracy == -1 ? 6 : accuracy;
            double ddx = Math.Round(ddVector.x - vector[n - 2].x, acc);
            double ddy = Math.Round(ddVector.y - vector[n - 2].y, acc);
            if (ddx > Math.Pow(0.1, acc * 2) || ddy > Math.Pow(0.1, acc * 2)) 
            {
                Console.ForegroundColor = ConsoleColor.Red;// 设置前景色
                Console.WriteLine("Error:坐标改正错误,({0},{1}),({2},{3}),Δ({4},{5})", ddVector.x, ddVector.y, vector[n - 2].x, vector[n - 2].y, ddx, ddy);
                Console.ResetColor();//将控制台的前景色和背景色设为默认值
            }
            // 计算途中坐标
            vector = Survey.SumTraverse(vector[0], cdeltaVector);
            vector.Add(null);
        }

        /// <summary>
        /// 打印表格(已弃用)
        /// </summary>
        public void Print()
        {
            Console.WriteLine("|{0,5}|{1,5}|{2,5}|{3,5}|{4,5}|({5,5},{6,5})|({7,5},{8,5})|({9,5},{10,5})|({11,5},{12,5})|",
                "观测角β", "角度改正值", "改正后角度", "坐标方位角", "边长S", "坐标增量x", "坐标增量y", "坐标改正x", "坐标改正y", "改正坐标增量x", "改正坐标增量y", "测站坐标x", "测站坐标y");
            for (int i = 0; i < n; i++)
            {
                List<double> str = new List<double>
                {
/*00*/              b[i] == null ? 0 : b[i].GetDMSSt(),
/*01*/              db[i] == null ? 0 : db[i].GetDMSSt(),
/*02*/              bc[i] == null ? 0 : bc[i].GetDMSSt(),
/*03*/              a[i] == null ? 0 : a[i].GetDMSSt(),
/*04*/              s[i],
/*05*/              deltaVector[i] == null ? 0 : Math.Round(deltaVector[i].x, 3),
/*06*/              deltaVector[i] == null ? 0 : Math.Round(deltaVector[i].y, 3),
/*07*/              ddeltaVector[i] == null ? 0 : Math.Round(ddeltaVector[i].x, 3),
/*08*/              ddeltaVector[i] == null ? 0 : Math.Round(ddeltaVector[i].y, 3),
/*09*/              cdeltaVector[i] == null ? 0 : Math.Round(cdeltaVector[i].x, 3),
/*10*/              cdeltaVector[i] == null ? 0 : Math.Round(cdeltaVector[i].y, 3),
/*11*/              vector[i] == null ? 0 : Math.Round(vector[i].x, 3),
/*12*/              vector[i] == null ? 0 : Math.Round(vector[i].y, 3),
                };
                //Console.WriteLine(b[i].returnDMSSt() + a[i].returnDMSSt() + s[i] + deltaVector[i].x + deltaVector[i].y + vector[i].x + vector[i].y);
                Console.WriteLine("|{0,10:.0000}|{1,10:.0000}|{2,10:.0000}|{3,10:.0000}|{4,10}|({5,10},{6,10})|({7,10},{8,10})|({9,10},{10,10})|({11,10},{12,10})|",
                    str[0], str[1], str[2], str[3], str[4], str[5], str[6], str[7], str[8], str[9], str[10], str[11], str[12]);
            }
            Console.WriteLine("Eβ={0},α'={1},角度闭合差Δα={2},α''={3},fx={4},fy={5}",
                eb.GetDMS(), tAngle.GetDMS(), dAngle.GetDMS(), ttAngle.GetDMS(), dVector.x, dVector.y);
        }

        public void Prints()
        {
            List<string> keys = new List<string>
            {
                "观测角β"      ,
                "角度改正值"    ,
                "改正后角度"    ,
                "坐标方位角"    ,
                "边长S"         ,
                "坐标增量"      ,
                "坐标改正"      ,
                "改正坐标增量"  ,
                "测站坐标"      ,
            };
            List<Object> values = new List<object>
            {
                b,
                db,
                bc,
                a,
                s,
                deltaVector,
                ddeltaVector,
                cdeltaVector,
                vector,
            };
            List<string> exkeys = new List<string>
            {
                "观测角之和Eβ",
                "αn-α1",
                "▲角度闭合差Δα",
                //"计算得终点坐标方位角α'",
                //"修正后终点坐标方位角α''",
                "导线总长Es",
                "Xn-X1,Yn-Y1",
                "坐标增量和Ex,Ey",
                "坐标闭合差fx,fy",
                "全长闭合差fs",
                "▲相对闭合差K=fs/Es",
            };
            List<Object> exvalues = new List<object>
            {
                eb.GetDMS(),
                (a.Last() - a.First()).GetDMS(),
                dAngle.GetDMS(),
                //tAngle.GetDMS(),
                //ttAngle.GetDMS(),
                es,
                vector[n - 2] - vector.First(),
                edeltaVector,
                dVector,
                Math.Round(Math.Sqrt(Math.Pow(dVector.x,2)+Math.Pow(dVector.y,2)),accuracy),
                "1/" + es/Math.Round(Math.Sqrt(Math.Pow(dVector.x,2)+Math.Pow(dVector.y,2)),accuracy),
            };
            Survey.Prints(n, keys, values, exkeys, exvalues, accuracy);
        }
    }
}
