/*using MathNet.Numerics;
using MathNet.Symbolics;
using Microsoft.FSharp.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeGenerate
{
    public class UnitStepIntegrationOld
    {

        public static void Run()
        {
            var w = Infix.ParseOrThrow("F0(x)*(u1-u2)");

            var v = integrate(w);

            for (int i = 0; i < 4; i++)
            {
                var fi = Expression.NewIdentifier(Symbol.NewSymbol("F" + i));
                var ui = Expression.NewIdentifier(Symbol.NewSymbol("u" + i));
                var xi = Expression.NewIdentifier(Symbol.NewSymbol("x" + i));

                v += fi * xi * ui;
            }

            var m = integrate(v);

            for (int i = 0; i < 4; i++)
            {
                var mi = Expression.NewIdentifier(Symbol.NewSymbol("M" + i));
                var xi = Expression.NewIdentifier(Symbol.NewSymbol("x" + i));
                var ui = Expression.NewIdentifier(Symbol.NewSymbol("u" + i));

                m += mi * xi * ui;
            }


            var mei = ApplyQ(m);

            var teta = integrate(mei);

            var delta = integrate(teta);

            Console.WriteLine(Infix.Format(mei));
        }

        private static Expression integrate(Expression exp)
        {
            var t = MathNet.Symbolics.Algebraic.Expand(exp);

            var buf = new List<Expression>();

            if (t is Expression.Sum tum)
            {
                foreach (var item in tum.Item)
                {
                    if (item is Expression.Product prd)
                        buf.Add(integrateAtom(prd));
                }
            }
            else
                throw new Exception();

            var lst = ListModule.OfArray(buf.ToArray());

            return Expression.NewSum(lst);
        }

        private static Expression integrateAtom(Expression.Product atom)
        {
            var us = Enumerable.Range(0, 4).Select(i => MathNet.Symbolics.Symbol.NewSymbol("u" + i)).ToArray();
            var xs = Enumerable.Range(0, 4).Select(i => MathNet.Symbolics.Symbol.NewSymbol("x" + i)).ToArray();

            var ab = new Symbol[] { Symbol.NewSymbol("alpha"), Symbol.NewSymbol("beta") };

            var x = Symbol.NewSymbol("x");

            var t = Infix.Format(atom);

            var flag = false;



            foreach (var item in atom.Item)
            {
                if (item is Expression.Identifier)
                    flag = true;

                if (item is Expression.Power p)
                {
                    var bs = p.Item1 as Expression.Identifier;

                    var parName = bs.Item.Item;

                    if(!Regex.IsMatch(parName,@"^x(\d)*$"))//only x and xi can have power of int
                    //if (bs.Item.Item != "x")
                    {
                        throw new NotImplementedException();
                    }

                    if (p.Item2.IsConstant)
                        flag = true;
                }
            }

            if (!flag)
                throw new NotImplementedException();

            //var idents = atom.Item.Cast<Expression.Identifier>().ToArray();



            var n = GetPowerOf(atom, x);

            var cus = us.Select(i => atom.Item.Contains(Expression.NewIdentifier(i))).ToArray();
            var cxs = xs.Select(i => atom.Item.Contains(Expression.NewIdentifier(i))).ToArray();
            var cabs = ab.Select(i => atom.Item.Contains(Expression.NewIdentifier(i))).ToArray();


            if (cus.Count(i => i) != 1)
                throw new Exception("only one U? in each function");

            var e1 = Copy(atom);

            e1 = e1 / Expression.Pow(Expression.NewIdentifier(x), n);//remove x
            var e2 = Copy(e1);

            e1 = e1 * Expression.Pow(Expression.NewIdentifier(x), n + 1) / Expression.NewNumber(BigRational.FromInt(n + 1));
            

            var uIndex = cus.FirstIndexOf(true);

            var xi = Symbol.NewSymbol("x" + uIndex.ToString());

            e2 = e2 * Expression.Pow(Expression.NewIdentifier(xi), n + 1) / Expression.NewNumber(BigRational.FromInt(n + 1));

            //var cus = us.Select(i => idents.Contains(Expression.newf(i))).ToArray();

            var buf = ListModule.OfArray(new Expression[] { e1, -e2 });

            var ret = Expression.NewSum(buf);

            var tpp = Infix.Format(atom) + "  ->  " + Infix.Format(ret);

            return ret;
        }

        private static Expression ApplyQ(Expression exp)
        {
            var t = MathNet.Symbolics.Algebraic.Expand(exp);


            var buf = new List<Expression>();

            if (t is Expression.Sum tum)
            {
                foreach (var item in tum.Item)
                {
                    if (item is Expression.Product prd)
                        buf.Add(ApplyQ(prd));
                }
            }
            else
                throw new Exception();

            var lst = ListModule.OfArray(buf.ToArray());

            return Expression.NewSum(lst);
        }

        private static Expression.Product ApplyQ(Expression.Product atom)
        {
            var x = Symbol.NewSymbol("x");
            var n = GetPowerOf(atom, x);

            var e1 = Copy(atom);
            e1 = e1 / Expression.Pow(Expression.NewIdentifier(x), n);//remove x

            var qi = Symbol.NewSymbol("Q" + n);

            e1 = e1 * Expression.NewIdentifier(qi);

            return (Expression.Product)e1;
        }

        private static int GetPowerOf(Expression.Product exp, Symbol symbol)
        {
            foreach (var item in exp.Item)
            {
                if (item is Expression.Identifier id)
                {
                    if(id.Item.Item == symbol.Item)
                    {
                        return 1;
                    }
                }

                if (item is Expression.Power p)
                {
                    var bs = p.Item1 as Expression.Identifier;

                    if (bs.Item.Item == symbol.Item)//only x can have power of int
                    {
                        if (!p.Item2.IsNumber)
                            throw new NotImplementedException();

                        var n2 = p.Item2 as Expression.Number;

                        var v2 = int.Parse(n2.Item.ToString());
                        return v2;
                    }
                }
            }


            return 0;
        }

        private static int RemoveTerm(Expression.Product exp, Symbol symbol)
        {
            foreach (var item in exp.Item)
            {
                if (item is Expression.Identifier id)
                {
                    if (id.Item == symbol)
                    {
                        return 1;
                    }
                }

                if (item is Expression.Power p)
                {
                    var bs = p.Item1 as Expression.Identifier;

                    if (bs.Item.Item == symbol.Item)//only x can have power of int
                    {
                        if (!p.Item2.IsNumber)
                            throw new NotImplementedException();

                        var n2 = p.Item2 as Expression.Number;

                        var v2 = int.Parse(n2.Item.ToString());
                        return v2;
                    }
                }
            }


            return 0;
        }


        private static Expression Copy(Expression exp)
        {
            return Infix.ParseOrThrow(Infix.Format(exp));
        }


        private static Function Function(string nm)
        {
            var f = typeof(Function);

            var cns = typeof(Function).GetConstructors();//.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);


            throw new NotImplementedException();
        }
    }
}
*/