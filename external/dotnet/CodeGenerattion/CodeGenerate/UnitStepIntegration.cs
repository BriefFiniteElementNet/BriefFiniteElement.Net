using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static CodeGenerate.UnitStepIntegration;

namespace CodeGenerate
{
    public class UnitStepIntegration
    {

        public static void Run()
        {
            var q = new Sum();

            {
                var q1 = new Product();
                q1.Atoms.Add(new Function("f0", "x"));
                q1.Atoms.Add(new Function("u1", "x"));
                q.Products.Add(q1);
            }

            {
                var q1 = new Product();
                q1.Atoms.Add(new MinusOne());
                q1.Atoms.Add(new Function("f0", "x"));
                q1.Atoms.Add(new Function("u2", "x"));
                q.Products.Add(q1);
            }


            var v = Integrate(q);


            var idxs = new int[] { 0, 1, 2, 3 };
            

            foreach (var idx in idxs)
            {
                var vi = new Product();
                vi.Atoms.Add(new Identifier("v" + idx, 1));
                vi.Atoms.Add(new Function("u" + idx, "x"));
                v.Products.Add(vi);
            }

            var m = Integrate(v);


            foreach (var idx in idxs)
            {
                var mi = new Product();
                mi.Atoms.Add(new Identifier("m" + idx, 1));
                mi.Atoms.Add(new Function("u" + idx, "x"));
                m.Products.Add(mi);
            }

            var mei = ApplyQEI(m);
            var meis = Sum.Simplify(mei);


            var t = Integrate(mei);
            t = Sum.Simplify(t);

            var d = Integrate(t);
            d = Sum.Simplify(d);


            var vs = Sum.Simplify(EvaluateAtPositiveZero(Sum.Simplify(v)));
            var ve = Sum.Simplify(EvaluateAtNegativeEnd(Sum.Simplify(v)));

            var ms = Sum.Simplify(EvaluateAtPositiveZero(Sum.Simplify(m)));
            var me = Sum.Simplify(EvaluateAtNegativeEnd(Sum.Simplify(m)));

            var ds = Sum.Simplify(EvaluateAtPositiveZero(d));
            var de = Sum.Simplify(EvaluateAtNegativeEnd(d));

            var ts = Sum.Simplify(EvaluateAtPositiveZero(t));
            var te = Sum.Simplify(EvaluateAtNegativeEnd(t));

            var sb = new StringBuilder();

            sb.AppendLine("Vs = " + vs.ToString());
            sb.AppendLine("Ve = " + ve.ToString());
            sb.AppendLine("Ms = " + ms.ToString());
            sb.AppendLine("Me = " + me.ToString());
            sb.AppendLine("Ts = " + ts.ToString());
            sb.AppendLine("Te = " + te.ToString());
            sb.AppendLine("Ds = " + ds.ToString());
            sb.AppendLine("De = " + de.ToString());

            var all = sb.ToString();
            //Console.WriteLine(Infix.Format(mei));
        }

        #region classes
        public abstract class Atom
        {
            public abstract Atom Clone();

        }

        public class Function : Atom
        {
            public string Name, Arg;

            public Function(string name, string arg)
            {
                Name = name;
                Arg = arg;
            }

            public override Atom Clone()
            {
                return new Function(this.Name, this.Arg);
            }

            public override string ToString()
            {
                return string.Format("{0}({1})", Name, Arg);
            }
        }

        public class Identifier : Atom
        {
            public string Base;
            public int Power;

            public Identifier(string @base, int power)
            {
                Base = @base;
                Power = power;
            }

            public override Atom Clone()
            {
                return new Identifier(this.Base, this.Power);
            }

            public override string ToString()
            {
                if (Power == 0)
                    return "1";

                if (Power == 1)
                    return Base;

                return string.Format("{0}^{1}", Base, Power);
            }
        }
        public class MinusOne : Number
        {
            public MinusOne():base("-1")
            {
            }

            public override Atom Clone()
            {
                return new MinusOne();
            }

            public override string ToString()
            {
                return "-1";
            }
        }

        public class One : Number
        {
            public One() : base("1")
            {

            }

            public new Atom Clone()
            {
                return new One();
            }

            public override string ToString()
            {
                return "1";
            }
        }

        public class Zero : Number
        {
            public Zero() : base("0")
            {

            }


            public new Atom Clone()
            {
                return new Zero();
            }

            public override string ToString()
            {
                return "0";
            }
        }

        public class Number : Atom
        {
            public string Value;//value is like "1/3.244"

            public Number(string value)
            {
                Value = value;
            }

            public override Atom Clone()
            {
                return new Number(this.Value);
            }


            public double Evaluate()
            {
                var res = new DataTable().Compute(Value, "");

                    var d = Convert.ToDouble(res);

                return d;
            }

            
            public override string ToString()
            {
                return Evaluate().ToString();
            }
        }

       

        public class Product
        {
            public List<Atom> Atoms = new List<Atom>();

            public bool Negated = false;

            public Product(params Atom[] atoms)
            {
                Atoms = atoms.ToList();
            }

            public Product Clone()
            {
                var lst = Atoms.Select(i => i == null ? null : i.Clone()).ToList();

                return new Product() { Atoms = lst, Negated = this.Negated };
            }

            public override string ToString()
            {
                var isNegate = this.Negated;

                var buf = Atoms.Where(i => i != null && i is not MinusOne).Select(i => i.ToString()).Aggregate((i, j) => i + "*" + j);

                if (isNegate)
                    buf = "-" + buf;

                return buf;
            }

            public static bool IsZero(Product prd)
            {
                return prd.Atoms.Any(i => i is Zero);
            }

            public static Product Symplify(Product prd)
            {
                var buf = new Product();

                var coefs = new List<Number>();

                if (prd.Negated)
                    coefs.Add(new MinusOne());

                foreach (var atom in prd.Atoms)
                {
                    if (atom is Number nm)
                    {
                        coefs.Add(nm);
                        continue;
                    }

                    if (atom == null)
                        continue;

                    if (atom is Function fnc)
                    {
                        buf.Atoms.Add(fnc.Clone());
                        continue;
                    }

                    if (atom is Identifier idf)
                    {
                        buf.Atoms.Add(idf.Clone());
                        continue;
                    }
                }

                var haveAnyZero = coefs.Any(i => i is Zero);

                if (haveAnyZero)
                {
                    return new Product(new Zero());
                }

                var numT = 1.0;

                foreach (var item in coefs)
                {
                    numT *= item.Evaluate();
                }

                if (numT == 1.0)
                {
                    //nothing
                }

                else if (numT == -1.0)
                {
                    buf.Negated = !buf.Negated;
                }
                else
                {
                    buf.Atoms.Insert(0, new Number(numT.ToString()));
                }

                var tt = buf.ToString();

                return buf;
            }
        }

        public class Sum
        {

            public static Sum Simplify(Sum input)
            {
                var buf = new Sum();

                foreach (var item in input.Products)
                {
                    var simped = Product.Symplify(item);

                    if (!Product.IsZero(simped))
                    {
                        buf.Products.Add(simped.Clone());
                    }
                }

                return buf;
            }

            public List<Product> Products;

            public Sum(params Product[] products)
            {
                Products = products.ToList();
            }

            public override string ToString()
            {
                var prds = new List<Tuple<string, string>>();//sign vale
                
                var sb = new StringBuilder();

                foreach (var product in Products)
                {
                    var item = product.ToString();

                    if (item.StartsWith("-"))
                        sb.Append(item);
                    else
                        sb.Append("+" + item);
                }

                return sb.ToString();
            }
        }
        #endregion

        public static Sum EvaluateAtPositiveZero(Sum sum)
        {
            //evaluates the sum at 0+ (0 + epsilon)

            var buf = new Sum();

            foreach (var item in sum.Products)
            {
                buf.Products.Add(EvaluateAtPositiveZero(item));
            }

            return buf;
        }

        public static Sum EvaluateAtNegativeEnd(Sum sum)
        {
            //evaluates the sum at L- (L - epsilon)

            var buf = new Sum();

            foreach (var item in sum.Products)
            {
                buf.Products.Add(EvaluateAtNegativeEnd(item));
            }

            return buf;
        }

        public static Product EvaluateAtNegativeEnd(Product pr)
        {
            var buf = pr.Clone();


            for (int i = 0; i < buf.Atoms.Count; i++)
            {
                var atom = buf.Atoms[i];

                Atom newAtom = null;

                if (atom == null)
                    continue;

                if (atom is Function fnc)
                {
                    if (fnc.Arg != "x")
                        continue;

                    var mtch = Regex.Match(fnc.Name, "^(\\w)(\\d)$");

                    if (mtch.Success)//that is u
                    {
                        var fncName = mtch.Groups[1].Value;
                        var id = int.Parse(mtch.Groups[2].Value);

                        if (fncName == "u")
                        {
                            newAtom = id < 3 ? new One() : new Zero();
                        }
                        else
                        {
                            var newF = (Function)atom.Clone();
                            newF.Arg = "L";
                            newAtom = newF;
                        }
                    }
                }

                if (atom is Identifier idf)
                {
                    if (idf.Base != "x")
                        continue;

                    var newIdf = (Identifier)idf.Clone();

                    newIdf.Base = "L";

                    newAtom = newIdf;
                }



                if (atom is Number)
                {
                    newAtom = atom.Clone();
                }


                var at = atom.GetType();
                if (newAtom == null)
                    throw new Exception();

                buf.Atoms[i] = newAtom;
            }

            return buf;
        }

        public static Product EvaluateAtPositiveZero(Product pr)
        {
            var buf = pr.Clone();


            for (int i = 0; i < buf.Atoms.Count; i++)
            {
                var atom = buf.Atoms[i];

                Atom newAtom = null;

                if (atom == null)
                    continue;

                if (atom is Function fnc)
                {
                    if (fnc.Arg != "x")
                        continue;

                    var mtch = Regex.Match(fnc.Name, "^(\\w)(\\d)$");

                    if (mtch.Success)//that is u
                    {
                        var fncName = mtch.Groups[1].Value;
                        var id = int.Parse(mtch.Groups[2].Value);

                        if (fncName == "u")
                        {
                            newAtom = id == 0 ? new One() : new Zero();
                        }
                        else
                        {
                            var newF = (Function)atom.Clone();
                            newF.Arg = "0";
                            newAtom = newF;
                        }
                    }
                }

                if (atom is Identifier idf)
                {
                    if (idf.Base != "x")
                        continue;

                    var newIdf = (Identifier)idf.Clone();

                    newIdf.Base = "0";

                    newAtom = newIdf;
                }

                

                if (atom is Number)
                {
                    newAtom = atom.Clone();
                }


                var at = atom.GetType();
                if (newAtom == null)
                    throw new Exception();

                buf.Atoms[i] = newAtom;
            }

            return buf;
        }

        public static Sum Integrate(Sum sum)
        {
            var buf = new Sum();

            buf.Products = new List<Product>();

            foreach (var item in sum.Products)
            {
                var inted = IntegrateProduct(item);

                buf.Products.AddRange(inted.Products);
            }

            return buf;
        }

        public static Sum ApplyQEI(Sum sum)//divide by 1/EI
        {
            var buf = new Sum();

            foreach (var item in sum.Products)
            {
                var qed=ApplyQEI(item);
                buf.Products.Add(qed);
            }

            return buf;
        }

        public static Product ApplyQEI(Product prd)//divide by 1/EI
        {
            var product = prd.Clone();

            var fids = new List<int>();

            var qids = new List<int>();
            var rids = new List<int>();
            var sids = new List<int>();
            var tids = new List<int>();
            var uids = new List<int>();

            var n = 0;

            for (var i = 0; i < product.Atoms.Count; i++)
            {
                var atm = product.Atoms[i];

                if (atm is Function fnc)
                {
                    if (fnc.Arg != "x")
                        continue;

                    var mtch = Regex.Match(fnc.Name, "^([fqrstu])(\\d)$");

                    if (!mtch.Success)
                        throw new NotImplementedException();

                    var nm = mtch.Groups[1].Value;
                    var num = int.Parse(mtch.Groups[2].Value);

                    switch (nm)
                    {
                        case "f":
                            fids.Add(num); break;
                        case "q":
                            qids.Add(num); break;
                        case "r":
                            rids.Add(num); break;
                        case "s":
                            sids.Add(num); break;
                        case "t":
                            tids.Add(num); break;
                        case "u":
                            uids.Add(num); break;
                        default:
                            throw new NotImplementedException();
                    }

                    //product.Atoms[i] = null;
                }

                if (atm is Identifier idf)
                {
                    if (idf.Base != "x")
                        continue;

                    n = idf.Power;

                    //product.Atoms[i] = null;
                }

            }

            var haveX = n > 0;
            var haveF = fids.Count > 0;
            var haveQ = qids.Count > 0;
            var haveR = rids.Count > 0;
            var haveS = sids.Count > 0;
            var haveT = tids.Count > 0;
            var haveU = uids.Count > 0;



            var counts = new int[] { fids.Count, qids.Count, rids.Count, sids.Count, tids.Count };

            if (counts.Sum() > 1)//just one of items: f,q,r,s,t
            {
                throw new Exception();
            }

            if (counts.Sum() > 1 && n > 0)//just one of items: x,f,q,r,s,t
            {
                throw new Exception();
            }


            if (uids.Count > 1)//just one of items: x,f,q,r,s,t
            {
                throw new Exception();
            }

            if (!haveU)
            {
                //TODO
                //have not Ui
                //handle normally
                throw new NotImplementedException();
            }

            var fid = haveF ? fids.First() : -1;
            var qid = haveQ ? qids.First() : -1;
            var rid = haveR ? rids.First() : -1;
            var sid = haveS ? sids.First() : -1;
            var tid = haveT ? tids.First() : -1;
            var uid = haveU ? uids.First() : -1;


            if (!haveF)
            {
                //replace x^i by Q,R,S,T

                var xAtomIdx = product.Atoms.FindIndex(i => i is Identifier && ((Identifier)i).Base == "x");

                if (xAtomIdx != -1)
                    product.Atoms[xAtomIdx] = null;

                var nm = "?";

                switch (n)
                {
                    case 0: nm = "q"; break;
                    case 1: nm = "r"; break;
                    case 2: nm = "s"; break;
                    default:
                        throw new Exception();
                }

                product.Atoms.Add(new Function(nm+0, "x"));

                return product;
            }

            //if (haveF)
            {
                if (fid != 2)
                    throw new Exception();

                var xAtomIdx = -1;

                for (int i = 0; i < product.Atoms.Count; i++)
                {
                    if (product.Atoms[i] is Function fnc)
                    {
                        if (fnc.Name == "f" + fid && fnc.Arg == "x")
                        {
                            xAtomIdx= i;
                            break;
                        }
                    }
                }


                if (xAtomIdx == -1)
                    throw new Exception();

                var fatom = product.Atoms[xAtomIdx] as Function;

                if (fatom.Name != "f2")
                    throw new Exception();

                fatom.Name = "t0";

                return product;
            }

           
        }

        public static Sum IntegrateProduct(Product product)
        {
            product = product.Clone();
            var fids = new List<int>();
            
            var qids= new List<int>();
            var rids= new List<int>();
            var sids= new List<int>();
            var tids= new List<int>();
            var uids = new List<int>();

            var n = 0;

            for (var i = 0; i < product.Atoms.Count; i++)
            {
                var atm = product.Atoms[i];

                if (atm is Function fnc)
                {
                    if (fnc.Arg != "x")
                        continue;

                    var mtch = Regex.Match(fnc.Name, "^([fqrstu])(\\d)$");

                    if (!mtch.Success)
                        throw new NotImplementedException();

                    var nm = mtch.Groups[1].Value;
                    var num = int.Parse(mtch.Groups[2].Value);

                    switch (nm)
                    {
                        case "f":
                            fids.Add(num); break;
                        case "q":
                            qids.Add(num); break;
                        case "r":
                            rids.Add(num); break;
                        case "s":
                            sids.Add(num); break;
                        case "t":
                            tids.Add(num); break;
                        case "u":
                            uids.Add(num); break;
                        default:
                            throw new NotImplementedException();
                    }

                    product.Atoms[i] = null;
                }

                if (atm is Identifier idf)
                {
                    if (idf.Base != "x")
                        continue;

                    n = idf.Power;

                    product.Atoms[i] = null;
                }

            }

            var haveX = n > 0;
            var haveF = fids.Count > 0;
            var haveQ = qids.Count > 0;
            var haveR = rids.Count > 0;
            var haveS = sids.Count > 0;
            var haveT = tids.Count > 0;
            var haveU = uids.Count > 0;



            var counts = new int[] { fids.Count, qids.Count, rids.Count, sids.Count, tids.Count };

            if (counts.Sum() > 1)//just one of items: f,q,r,s,t
            {
                throw new Exception();
            }

            if (counts.Sum() > 1 && n > 0)//just one of items: x,f,q,r,s,t
            {
                throw new Exception();
            }


            if (uids.Count > 1)//just one of items: x,f,q,r,s,t
            {
                throw new Exception();
            }

            if (!haveU)
            {
                //TODO
                //have not Ui
                //handle normally
                throw new NotImplementedException();
            }

            var qid = haveQ ? qids.First() : -1;
            var rid = haveR ? rids.First() : -1;
            var sid = haveS ? sids.First() : -1;
            var tid = haveT ? tids.First() : -1;
            var uid = haveU ? uids.First() : -1;

            var c1 = product.Clone();
            var c2 = product.Clone();

            if (haveF || haveQ || haveR || haveS || haveT)
            {
                var nms = new string[] { "f", "q", "r", "s", "t" };
                var flags = new bool[] { haveF, haveQ, haveR, haveS, haveT };
                var ids = new List<int>[] { fids, qids, rids, sids, tids };

                var idx = flags.FirstIndexOf(true);

                var nm = nms[idx];
                var id = ids[idx].First();

                c1.Atoms.Add(new Function(nm + (id + 1), "x"));
                c2.Atoms.Add(new Function(nm + (id + 1), "x" + uid));
                c2.Atoms.Add(new MinusOne());

                c1.Atoms.Add(new Function("u" + uid, "x"));
                c2.Atoms.Add(new Function("u" + uid, "x"));

                return new Sum(c1, c2);
            }

            c1.Atoms.Add(new Identifier("x", n + 1));
            c2.Atoms.Add(new Identifier("x" + uid, n + 1));
            c1.Atoms.Add(new Number("1.0/" + (n + 1)));
            c2.Atoms.Add(new Number("1.0/" + (n + 1)));
            c2.Atoms.Add(new MinusOne());

            c1.Atoms.Add(new Function("u" + uid, "x"));
            c2.Atoms.Add(new Function("u" + uid, "x"));

            return new Sum(c1, c2);
        }
    }
}
