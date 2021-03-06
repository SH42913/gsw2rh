using System.Collections.Generic;
using Leopotam.Ecs;

namespace GSW3.Bleeding
{
    public class PedBleedingInfoComponent : IEcsAutoResetComponent
    {
        public float BleedingHealRate;
        
        [EcsIgnoreNullCheck]
        public readonly List<EcsEntity> BleedingEntities = new List<EcsEntity>(16);
        
        public void Reset()
        {
            BleedingEntities.Clear();
        }
    }
}