using System.Xml.Linq;
using GSW3.Utils;
using Leopotam.Ecs;

namespace GSW3.WoundEffects.VehicleControl.Systems
{
    [EcsInject]
    public class VehicleControlInitSystem : BaseEffectInitSystem
    {
        public VehicleControlInitSystem() : base(new GswLogger(typeof(VehicleControlInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, EcsEntity partEntity)
        {
            XElement disable = partRoot.Element("DisableCarControl");
            if (disable != null)
            {
                var component = EcsWorld.AddComponent<DisableVehicleControlComponent>(partEntity);
                component.EnableOnlyOnHeal = disable.GetBool("EnableOnlyOnHeal");
            }

            XElement enable = partRoot.Element("EnableCarControl");
            if (enable != null)
            {
                EcsWorld.AddComponent<EnableVehicleControlComponent>(partEntity);
            }
        }
    }
}