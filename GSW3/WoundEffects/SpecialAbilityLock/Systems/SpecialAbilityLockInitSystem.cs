using System.Xml.Linq;
using GSW3.Utils;
using Leopotam.Ecs;

namespace GSW3.WoundEffects.SpecialAbilityLock.Systems
{
    [EcsInject]
    public class SpecialAbilityLockInitSystem : BaseEffectInitSystem
    {
        public SpecialAbilityLockInitSystem() : base(new GswLogger(typeof(SpecialAbilityLockInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, EcsEntity partEntity)
        {
            XElement lockAbility = partRoot.Element("LockSpecialAbility");
            if (lockAbility != null)
            {
                EcsWorld.AddComponent<LockSpecialAbilityComponent>(partEntity);
            }

            XElement unlock = partRoot.Element("UnlockSpecialAbility");
            if (unlock != null)
            {
                EcsWorld.AddComponent<UnlockSpecialAbilityComponent>(partEntity);
            }
        }
    }
}