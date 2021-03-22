using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolynomialInterpolation
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<double> x = new List<double>() { -4, -3, -2, 0, 2, 3, 4 };
            //List<double> f = new List<double>() { 1024, 243, 32, 0, -32, -243, -1024 };
            List<double> x = new List<double>() { -2, 0, 2, -3, 3, -4, 4 };
            List<double> f = new List<double>() { 32, 0, -32, 243, -243, 1024, -1024 };
            Interpolation p2 = new Interpolation(x, f);
            Console.WriteLine(p2);
            Polynomial pol = p2.GetPolynomial();
            Console.WriteLine(pol);
        }
    }

    class Polynomial
    {
        public List<double> coefficients;

        public int Deg { get { return coefficients.Count - 1; } }

        public double Compute(double x)
        {
            double ans = 0;
            for(int i=coefficients.Count-1;i>=0;i--)
            {
                ans *= x;
                ans += coefficients[i];
            }
            return ans;
        }

        public Polynomial(List<double> coef) { coefficients = coef; }
        public Polynomial() { coefficients = new List<double> { 0 }; }

        public static Polynomial operator+ (Polynomial a, Polynomial b)
        {
            List<double> list = new List<double>();
            for(int i=0;i<=Math.Max(a.Deg, b.Deg);i++)
            {
                list.Add(0);
                if (a.Deg >= i) list[i] += a.coefficients[i];
                if (b.Deg >= i) list[i] += b.coefficients[i];
            }
            return new Polynomial(list);
        }

        public static Polynomial operator* (Polynomial a, double c)
        {
            List<double> list = new List<double>(a.coefficients);
            for(int i=0;i<list.Count;i++)
            {
                list[i] *= c;
            }
            return new Polynomial(list);
        }

        public static Polynomial operator* (Polynomial a, Polynomial b)
        {
            List<double> list = new List<double>(a.Deg+b.Deg+1);
            for(int i=0;i<a.Deg+b.Deg+1;i++)
            {
                list.Add(0);
            }
            for(int i=0;i<=a.Deg;i++)
            {
                for(int j=0;j<=b.Deg;j++)
                {
                    
                    list[i + j] += a.coefficients[i] * b.coefficients[j];
                }
            }
            return new Polynomial(list);
        }

        public override string ToString()
        {
            string s = "";
            for(int i=Deg;i>=0;i--)
            {
                if(coefficients[i]!=0)
                {
                    if (coefficients[i] > 0 && i!=Deg) s += "+";
                    s += $"{coefficients[i]}*x^{i}";
                }
            }
            return s;
        }
    }

    class Interpolation
    {
        List<double> x;
        List<List<double>> f;

        bool computed = false;

        public Interpolation(List<double> _x, List<double> y)
        {
            x = _x;
            f = new List<List<double>>();
            for(int i=0;i<y.Count;i++)
            {
                f.Add(new List<double>());
                f[i].Add(y[i]);
            }
        }

        public void Compute()
        {
            computed = true;
            for(int i=1;i<f.Count;i++)
            {
                for(int j=i;j<f.Count;j++)
                {
                    double value = (f[j][i - 1] - f[j - 1][i - 1]) / (x[j] - x[j - i]);
                    f[j].Add(value);
                }
            }
        }

        public Polynomial GetPolynomial()
        {
            if (!computed) Compute();
            Polynomial ans = new Polynomial();
            for(int i=0;i<f.Count;i++)
            {
                Polynomial temp = new Polynomial(new List<double>() { 1 });
                for(int j=0;j<i;j++)
                {
                    temp = temp * new Polynomial(new List<double>() { -x[j], 1 });
                }
                temp = temp * f[i][i];
                ans = ans + temp;
            }
            return ans;
        }

        public override string ToString()
        {
            string s = "";
            if (!computed) Compute();
            for(int i=0;i<x.Count;i++)
            {
                s += $"{x[i]}   ";
                foreach(var ft in f[i])
                {
                    s += $"{ft}   ";
                }
                s += "\n";
            }
            return s;
        }
    }

}
