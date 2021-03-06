using GSW3.Player;
using GSW3.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GSW3.WoundEffects.Movement.Systems
{
    [EcsInject]
    public class MovementSystem : BaseEffectSystem
    {
        public MovementSystem() : base(new GswLogger(typeof(MovementSystem)))
        {
        }

        protected override void ResetEffect(Ped ped, EcsEntity pedEntity)
        {
            var permanentRate = EcsWorld.GetComponent<PermanentMovementRateComponent>(pedEntity);
            if (permanentRate != null)
            {
                NativeFunction.Natives.SET_PED_MOVE_RATE_OVERRIDE(ped, 1.0f);
                EcsWorld.RemoveComponent<PermanentMovementRateComponent>(pedEntity);
#if DEBUG
                Logger.MakeLog($"Permanent move rate for {pedEntity.GetEntityName()} was reset");
#endif
            }

            var player = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity);
            if (player == null) return;

            NativeFunction.Natives.SET_PLAYER_SPRINT(Game.LocalPlayer, true);
            EcsWorld.RemoveComponent<PermanentDisabledSprintComponent>(pedEntity, true);
        }

        protected override void ProcessWound(Ped ped, EcsEntity pedEntity, EcsEntity woundEntity)
        {
            var permanentDisabled = EcsWorld.GetComponent<PermanentDisabledSprintComponent>(pedEntity);
            var player = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity);
            if (permanentDisabled == null && player != null)
            {
                var disable = EcsWorld.GetComponent<DisableSprintComponent>(woundEntity);
                if (disable != null)
                {
                    NativeFunction.Natives.SET_PLAYER_SPRINT(Game.LocalPlayer, false);
                    if (disable.RestoreOnlyOnHeal)
                    {
                        EcsWorld.AddComponent<PermanentDisabledSprintComponent>(pedEntity);
                    }
#if DEBUG
                    Logger.MakeLog($"Sprint disabled for player, permanent = {disable.RestoreOnlyOnHeal}");
#endif
                }

                var enable = EcsWorld.GetComponent<EnableSprintComponent>(woundEntity);
                if (enable != null)
                {
                    NativeFunction.Natives.SET_PLAYER_SPRINT(Game.LocalPlayer, true);
#if DEBUG
                    Logger.MakeLog("Sprint enabled for player");
#endif
                }
            }

            var permanentRate = EcsWorld.GetComponent<PermanentDisabledSprintComponent>(pedEntity);
            if (permanentRate == null)
            {
                var newRate = EcsWorld.GetComponent<NewMovementRateComponent>(woundEntity);
                if (newRate != null)
                {
                    NativeFunction.Natives.SET_PED_MOVE_RATE_OVERRIDE(ped, newRate.Rate);
                    if (newRate.RestoreOnlyOnHeal)
                    {
                        EcsWorld.AddComponent<PermanentDisabledSprintComponent>(pedEntity);
                    }
#if DEBUG
                    Logger.MakeLog($"Move rate for {pedEntity.GetEntityName()} " +
                                   $"was changed to {newRate.Rate}, permanent = {newRate.RestoreOnlyOnHeal}");
#endif
                }

                var restore = EcsWorld.GetComponent<RestoreMovementComponent>(woundEntity);
                if (restore != null)
                {
                    NativeFunction.Natives.SET_PED_MOVE_RATE_OVERRIDE(ped, 1.0f);
#if DEBUG
                    Logger.MakeLog($"Move rate for {pedEntity.GetEntityName()} was restored");
#endif
                }
            }
        }
    }
}