using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace PreemptiveStrike.Interceptor
{
    class GroupListStorage : IExposable
    {
        private List<Pawn> pawns;
        private List<int> group;
        private List<IntVec3> vec;

        public GroupListStorage()
        {
            pawns = new List<Pawn>();
            group = new List<int>();
            vec = new List<IntVec3>();
        }

        public GroupListStorage(List<Pair<List<Pawn>, IntVec3>> raw)
        {
            pawns = new List<Pawn>();
            group = new List<int>();
            vec = new List<IntVec3>();
            foreach(var p in raw)
            {
                group.Add(p.First.Count);
                vec.Add(p.Second);
                foreach (var pp in p.First)
                    pawns.Add(pp);
            }
        }

        public List<Pair<List<Pawn>, IntVec3>> RebuildList()
        {
            List<Pair<List<Pawn>, IntVec3>> res = new List<Pair<List<Pawn>, IntVec3>>();
            int cur = 0;
            for(int i = 0;i<vec.Count;++i)
            {
                List<Pawn> tmp = new List<Pawn>();
                int remain = group[i];
                while(remain > 0)
                {
                    tmp.Add(pawns[cur]);
                    ++cur;
                    --remain;
                }
                res.Add(new Pair<List<Pawn>, IntVec3>(tmp, vec[i]));
            }
            return res;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref pawns, "pawns", LookMode.Reference);
            Scribe_Collections.Look(ref group, "group", LookMode.Value);
            Scribe_Collections.Look(ref vec, "vec", LookMode.Value);
        }
    }
}
