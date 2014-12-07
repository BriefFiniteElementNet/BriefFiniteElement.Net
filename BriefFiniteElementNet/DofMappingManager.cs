using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a class for mapping Dof numbers in real structure, reduced structure (after applying rigid elements) and free and fixed part of reduced structure
    /// </summary>
    public class DofMappingManager
    {
        private int _n;
        private int _m;

        public int N
        {
            get { return _n; }
            set { _n = value; }
        }

        public int M
        {
            get { return _m; }
            set { _m = value; }
        }

        public int[] MasterMap;
        public int[] RMasterMap;

        public int[] Map1;
        public int[] RMap1;

        public int[] Map2;
        public int[] RMap2;

        public int[] Map3;
        public int[] RMap3;

        public int[] Map4;
        public int[] RMap4;

        /// <summary>
        /// The fixity in all nodes of structure
        /// </summary>
        /// <remarks>
        /// length = n
        /// Fixity[i] = fixity of i'th DoF in structure
        /// </remarks>
        public DofConstraint[] Fixity;


        public static DofMappingManager Create(Model model, LoadCase cse)
        {
            var n = model.Nodes.Count;
            var m = 0;

            for (var i = 0; i < n; i++)
                model.Nodes[i].Index = i;
            
            var masters = CalcUtil.GetMasterMapping(model, cse);

            for (var i = 0; i < n; i++)
            {
                if (masters[i] == i)
                    m++;
            }

            var cnt = 0;

            var fixity = new DofConstraint[6*n];


            #region Map4 and rMap4

            var map4 = new int[n];
            var rMap4 = new int[m];

            map4.FillNegative();
            rMap4.FillNegative();

            for (var i = 0; i < n; i++)
            {
                if (masters[i] == i)
                {
                    map4[i] = cnt;
                    rMap4[cnt] = i;
                    cnt++;
                }
            }

            #endregion

            #region Map1 and rMap1

            var map1 = new int[6*n];
            var rMap1 = new int[6*m];

            map1.FillNegative();
            rMap1.FillNegative();

            cnt = 0;

            for (var i = 0; i < m; i++)
            {
                var ir = i;
                var it = rMap4[i];

                for (var j = 0; j < 6; j++)
                {
                    map1[it*6 + j] = ir*6 + j;
                    rMap1[ir*6 + j] = it*6 + j;
                }
            }

            #endregion

            #region map2,map3,rmap2,rmap3

            var mf = 0;//fixes
            var mr = 0;

            for (var i = 0; i < n; i++)
            {
                if (masters[i] != i)
                    continue;

                var ctx = model.Nodes[i].Constraints;
                var s = ctx.Sum();
                mf += s;
            }

            mr = 6*m - mf;

            var rMap2 = new int[mr];
            var rMap3 = new int[mf];
            var map2 = new int[6*m];
            var map3 = new int[6*m];

            rMap2.FillNegative();
            rMap3.FillNegative();
            map2.FillNegative();
            map3.FillNegative();


            var rcnt = 0;
            var fcnt = 0;

            for (var i = 0; i < m; i++)
            {
                var arr = model.Nodes[rMap4[i]].Constraints.ToArray();

                for (var j = 0; j < 6; j++)
                {
                    if (arr[j] == DofConstraint.Released)
                    {
                        map2[6*i + j] = rcnt;
                        rMap2[rcnt] = 6*i + j;
                        rcnt++;
                    }
                    else
                    {
                        map3[6*i + j] = fcnt;
                        rMap3[fcnt] = 6*i + j;
                        fcnt++;
                    }
                }
            }


            for (var i = 0; i < n; i++)
            {
                var ctx = model.Nodes[i].Constraints.ToArray();

                for (var j = 0; j < 6; j++)
                {
                    fixity[6*i + j] = ctx[j];
                }
            }

            #endregion

            #region RMaster

            var rMaster = new int[n];
            rMaster.FillNegative();

            cnt = 0;

            for (var i = 0; i < masters.Length; i++)
            {
                if (masters[i] == i)
                    rMaster[i] = cnt++;
            }

            #endregion


            var buf = new DofMappingManager();

            buf.Map1 = map1;
            buf.RMap1 = rMap1;

            buf.Map2 = map2;
            buf.RMap2 = rMap2;

            buf.Map3 = map3;
            buf.RMap3 = rMap3;

            buf.Map4 = map4;
            buf.RMap4 = rMap4;

            buf.Fixity = fixity;

            buf.M = m;
            buf.N = n;

            buf.MasterMap = masters;
            buf.RMasterMap = rMaster;

            return buf;
        }
    }
}
