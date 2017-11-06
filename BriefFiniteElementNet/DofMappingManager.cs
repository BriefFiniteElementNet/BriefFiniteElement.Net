using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a class for mapping Dof numbers in real structure, reduced structure (after applying rigid elements) and free and fixed part of reduced structure
    /// </summary>
    /// <remarks>
    /// 
    /// -------------------------------------------------
    /// # of DoF in whole structure: 1
    /// # of DoF in reduced structure: 2
    /// map1[1] = 2     , l = 6*n
    /// rMap1[2] = 1    , l = 6*m
    /// -------------------------------------------------
    /// # of Dof in reduced structure: 3
    /// # of DoF in released DoFs of reduced structure: 4
    /// map2[3] = 4     , l = 6*m
    /// rmap2[4] = 3    , l = mr
    /// -------------------------------------------------
    /// # of Dof in reduced structure: 5
    /// # of DoF in fixed DoFs of reduced structure: 6
    /// map3[5] = 6     , l = 6*m
    /// rmap3[6] = 5    , l = mf
    /// -------------------------------------------------
    /// # of node in whole structure
    /// # of node in reduced structure
    /// map4[7] = 8
    /// rmap4[8] = 7
    /// -------------------------------------------------
    /// # of node in main structure: 9
    /// # of master node: 10
    /// # of master node in all masters: 11
    /// masters[9] = 10
    /// rMasters[9] = 11
    /// </remarks>
    public class DofMappingManager
    {
        private int _n;
        private int _m;

        /// <summary>
        /// Gets or sets the number of total nodes.
        /// </summary>
        /// <value>
        /// The n.
        /// </value>
        public int N
        {
            get { return _n; }
            set { _n = value; }
        }

        /// <summary>
        /// Gets or sets the number of master nodes.
        /// </summary>
        /// <value>
        /// The m.
        /// </value>
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

            map4.FillWith(-1);
            rMap4.FillWith(-1);

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

            map1.FillWith(-1);
            rMap1.FillWith(-1);

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

            rMap2.FillWith(-1);
            rMap3.FillWith(-1);
            map2.FillWith(-1);
            map3.FillWith(-1);


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
            rMaster.FillWith(-1);

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
