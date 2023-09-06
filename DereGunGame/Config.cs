using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Interfaces;
using DereGunGame.Types;
using Exiled.API.Features;
using Exiled.API.Enums;
using PlayerRoles;
using System.ComponentModel;

namespace DereGunGame
{
    public class Config : IConfig
    {

        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;


        [Description("A list of the coordinates of all the possible spawn locations.")]
        public Dictionary<int, Vector3> SpawnLocations { get; set; } = new()
        {
            {
                0,
                new Vector3()
                {
                    x = 49,
                    y = 991,
                    z = -43
                }
            }
        };
        [Description("A list of all the gun levels, and the loadouts. IDs of the gun levels must start from 0 and in order.")]
        public Dictionary<int, GunLevel> GunLevels { get; set; } = new()
        {
            {
                0,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunFSP9, ItemType.Medkit, ItemType.ArmorHeavy , ItemType.Ammo9x19 },

                    Effects = new() { new Effect(EffectType.MovementBoost, 50, 1) },
                    Appearance = RoleTypeId.FacilityGuard,
                    MaxHealth = 100
                }
            },
            {
                1,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunCrossvec, ItemType.Medkit, ItemType.ArmorLight, ItemType.Ammo9x19 },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.NtfPrivate,
                    MaxHealth = 100
                }
            },
            {
                2,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunShotgun, ItemType.Medkit, ItemType.ArmorHeavy, ItemType.Ammo12gauge },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.ChaosRepressor,
                    MaxHealth = 50
                }
            },
            {
                3,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunRevolver, ItemType.Medkit, ItemType.ArmorHeavy, ItemType.Ammo44cal },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.ChaosRepressor,
                    MaxHealth = 100
                }
            },            
            {
                4,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunAK, ItemType.Medkit, ItemType.ArmorCombat, ItemType.Ammo762x39 },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.ChaosRepressor,
                    MaxHealth = 100
                }
            },
            {
                5,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunE11SR, ItemType.Medkit, ItemType.ArmorCombat, ItemType.Ammo556x45 },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.NtfSergeant,
                    MaxHealth = 100
                }
            },
            {
                6,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunFRMG0, ItemType.Medkit, ItemType.ArmorCombat, ItemType.Ammo556x45 },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.NtfCaptain,
                    MaxHealth = 100
                }
            },
            {
                7,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunLogicer, ItemType.Medkit, ItemType.ArmorCombat, ItemType.Ammo762x39 },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.ChaosMarauder,
                    MaxHealth = 50
                }
            },
            {
                8,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunA7, ItemType.Medkit, ItemType.ArmorCombat, ItemType.Ammo762x39 },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.ChaosMarauder,
                    MaxHealth = 100
                }
            },
            {
                9,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GrenadeHE, ItemType.Medkit, ItemType.ArmorCombat, ItemType.GrenadeHE, ItemType.GrenadeHE },
                    Effects = new() { new Effect(EffectType.MovementBoost, 50, 1) },
                    Appearance = RoleTypeId.ClassD,
                    MaxHealth = 200
                }
            },
            {
                10,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunCom45, ItemType.Medkit, ItemType.ArmorCombat, ItemType.Ammo9x19 },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.Scientist,
                    MaxHealth = 100
                }
            },
            {
                11,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunCOM15, ItemType.Medkit, ItemType.ArmorCombat, ItemType.Ammo9x19 },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.Scientist,
                    MaxHealth = 100
                }
            },
            {
                12,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunCOM18, ItemType.Medkit, ItemType.ArmorCombat, ItemType.Ammo9x19 },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.Scientist,
                    MaxHealth = 100
                }
            },
            {
                13,
                new GunLevel()
                {
                    Loadout = new() { ItemType.ParticleDisruptor, ItemType.Medkit, ItemType.ArmorCombat },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.NtfSpecialist,
                    MaxHealth = 100
                }
            },
            {
                14,
                new GunLevel()
                {
                    Loadout = new() { ItemType.MicroHID, ItemType.Medkit, ItemType.ArmorCombat },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.Scientist,
                    MaxHealth = 100
                }
            },
            {
                15,
                new GunLevel()
                {
                    Loadout = new() { ItemType.Jailbird, ItemType.Medkit, ItemType.ArmorCombat },
                    Effects = new() { new Effect(EffectType.MovementBoost, 3, 1) },
                    Appearance = RoleTypeId.Tutorial,
                    MaxHealth = 100
                }
            }
        };

    }
}
