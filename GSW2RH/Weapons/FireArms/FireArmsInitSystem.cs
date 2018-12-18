﻿using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Weighted_Randomizer;

namespace GunshotWound2.Weapons.FireArms
{
    [EcsInject]
    public class FireArmsInitSystem : IEcsInitSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<WeaponTypeComponent, WeaponInitComponent, WeaponHashesComponent> _initWeapons;

        private const string FIRE_ARMS_CHANCES_KEY = "FireArmsWeightChances";

        private readonly GswLogger _logger;

        public FireArmsInitSystem()
        {
            _logger = new GswLogger(typeof(FireArmsInitSystem));
        }

        public void Initialize()
        {
            foreach (int i in _initWeapons)
            {
                WeaponTypes type = _initWeapons.Components1[i].Type;
                if (type != WeaponTypes.FIRE_ARMS) continue;

                XElement root = _initWeapons.Components2[i].WeaponRoot;
                XElement chancesElement = root.GetElement(FIRE_ARMS_CHANCES_KEY);
                int weaponEntity = _initWeapons.Entities[i];
                string weaponName = _initWeapons.Components3[i].Name;

                int graze = chancesElement.GetInt("Graze");
                int flesh = chancesElement.GetInt("Flesh");
                int penetrating = chancesElement.GetInt("Penetrating");
                int perforating = chancesElement.GetInt("Perforating");
                int avulsive = chancesElement.GetInt("Avulsive");

                var chances = _ecsWorld.AddComponent<FireArmsWoundRandomizerComponent>(weaponEntity);
                chances.WoundRandomizer = new StaticWeightedRandomizer<FireArmsWounds>
                {
                    {FireArmsWounds.GRAZE_WOUND, graze},
                    {FireArmsWounds.FLESH_WOUND, flesh},
                    {FireArmsWounds.PENETRATING_WOUND, penetrating},
                    {FireArmsWounds.PERFORATING_WOUND, perforating},
                    {FireArmsWounds.AVULSIVE_WOUND, avulsive}
                };

#if DEBUG
                _logger.MakeLog($"{weaponName} is {type}, chances: {FireArmsWounds.GRAZE_WOUND} {graze}; " +
                                $"{FireArmsWounds.FLESH_WOUND} {flesh}; " +
                                $"{FireArmsWounds.PENETRATING_WOUND} {penetrating}; " +
                                $"{FireArmsWounds.PERFORATING_WOUND} {perforating}; " +
                                $"{FireArmsWounds.AVULSIVE_WOUND} {avulsive}");
#endif
            }
        }

        public void Destroy()
        {
        }
    }
}