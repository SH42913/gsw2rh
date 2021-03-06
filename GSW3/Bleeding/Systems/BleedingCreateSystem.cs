using System;
using GSW3.BodyParts;
using GSW3.Utils;
using GSW3.Weapons;
using GSW3.Wounds;
using Leopotam.Ecs;

namespace GSW3.Bleeding.Systems
{
    [EcsInject]
    public class BleedingCreateSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly Random _random = null;

        private readonly EcsFilter<BleedingStatsComponent> _bleedingStats = null;
        private readonly EcsFilter<PedBleedingInfoComponent, WoundedComponent, DamagedBodyPartComponent, DamagedByWeaponComponent> _wounded = null;

#if DEBUG
        private readonly GswLogger _logger;

        public BleedingCreateSystem()
        {
            _logger = new GswLogger(typeof(BleedingCreateSystem));
        }
#endif

        public void Run()
        {
            BleedingStatsComponent stats = _bleedingStats.Components1[0];
            foreach (int i in _wounded)
            {
                PedBleedingInfoComponent info = _wounded.Components1[i];
                WoundedComponent wounded = _wounded.Components2[i];
                DamagedBodyPartComponent damagedBodyPart = _wounded.Components3[i];
                DamagedByWeaponComponent damagedByWeapon = _wounded.Components4[i];
                
                EcsEntity pedEntity = _wounded.Entities[i];
                foreach (EcsEntity woundEntity in wounded.WoundEntities)
                {
                    var baseBleeding = _ecsWorld.GetComponent<BaseBleedingComponent>(woundEntity);
                    if (baseBleeding == null) continue;

                    float baseSeverity = baseBleeding.BaseBleeding;
                    if (baseSeverity <= 0) continue;

                    EcsEntity bodyPartEntity = damagedBodyPart.DamagedBodyPartEntity;
                    float bodyPartMult = _ecsWorld.GetComponent<BleedingMultiplierComponent>(bodyPartEntity).Multiplier;
                    float sevWithMult = stats.BleedingMultiplier * bodyPartMult * baseSeverity;

                    float sevDeviation = sevWithMult * stats.BleedingDeviation;
                    sevDeviation = _random.NextFloat(-sevDeviation, sevDeviation);

                    float finalSeverity = sevWithMult + sevDeviation;

                    EcsEntity bleedingEntity = _ecsWorld.CreateEntityWith(out BleedingComponent bleeding);
                    bleeding.DamagedBoneId = damagedBodyPart.DamagedBoneId;
                    bleeding.BodyPartEntity = damagedBodyPart.DamagedBodyPartEntity;
                    bleeding.Severity = finalSeverity;
                    bleeding.MotherWoundEntity = woundEntity;
                    bleeding.WeaponEntity = damagedByWeapon.WeaponEntity;
                    
#if DEBUG
                    _logger.MakeLog(
                        $"Created bleeding {woundEntity.GetEntityName()} on bone {bleeding.DamagedBoneId} for {pedEntity.GetEntityName()}. " +
                        $"Base severity {baseSeverity:0.00}; final severity {finalSeverity:0.00}");
#endif
                    info.BleedingEntities.Add(bleedingEntity);
                }

#if DEBUG
                _ecsWorld.ProcessDelayedUpdates();
                string bleedingList = "";
                foreach (EcsEntity bleedEntity in info.BleedingEntities)
                {
                    bleedingList += $"{_ecsWorld.GetComponent<BleedingComponent>(bleedEntity).Severity:0.00} ";
                }

                _logger.MakeLog($"BleedingList for {pedEntity.GetEntityName()}: {bleedingList}");
#endif
            }
        }
    }
}