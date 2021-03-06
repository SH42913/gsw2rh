﻿using System.Collections.Generic;
using Leopotam.Ecs;

namespace GSW3.Hashes
{
    public class HashesComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck] 
        public readonly List<uint> Hashes = new List<uint>(16);

        public void Reset()
        {
            Hashes.Clear();
        }

        public override string ToString()
        {
            string hashes = "Hashes: ";
            if (Hashes == null || Hashes.Count <= 0)
            {
                hashes += "nothing to see here";
                return hashes;
            }

            foreach (uint hash in Hashes)
            {
                hashes += $"{hash.ToString()}; ";
            }

            return hashes;
        }
    }
}