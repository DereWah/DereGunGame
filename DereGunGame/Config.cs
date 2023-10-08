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


        [Description("If true, every round will be a GunGame. If false, while in Lobby you'll have to type the command forcestartgungame to turn it on for the following round.")]
        public bool AlwaysStart { get; set; } = false;

        [Description("Seconds in between respawning.")]
        public float RespawnDelay { get; set; } = 3f;

        

        [Description("Settings for the humiliation system. The melee item is the Jailbird.")]
        public JailbirdSettings JailbirdSettings { get; set; } = new JailbirdSettings();
        


        [Description("The minimum distance a player will be spawned to another player.")]
        public float SpawnRadius { get; set; } = 10f;

        [Description("One item of the list below will be randomly dropped whenever a player dies.")]
        public Dictionary<int, ItemType> DeathDrops { get; set; } = new()
        {
            { 0, ItemType.Medkit },
            { 1, ItemType.SCP1853 },
            { 2, ItemType.Painkillers },
            { 3, ItemType.Adrenaline }
        };

        [Description("The zones in which the game will take place. A random zone will be chosen at the start of the round.")]
        public Dictionary<int, ZoneType> ZoneTypes { get; set; } = new()
        {
            { 0, ZoneType.Surface },
            { 1, ZoneType.HeavyContainment},
            { 2, ZoneType.LightContainment},
            { 3, ZoneType.Entrance}
        };

        [Description("A list of the coordinates of all the possible spawn locations for the SURFACE. While in other Zones, door positions will be used for spawnpoints.")]
        public Dictionary<int, Vector3> SpawnLocations { get; set; } = new()
        {
            { 0, new Vector3() { x = 49, y = 991, z = -43 } },
            { 1, new Vector3() { x = -8, y = 1001, z = 2 } },
            { 2, new Vector3() { x = 0, y = 1001, z = 6 } },
            { 3, new Vector3() { x = 11, y = 998, z = -15 } },
            { 4, new Vector3() { x = 14, y = 999, z = -34 } },
            { 5, new Vector3() { x = 4, y = 1001, z = -30 } },
            { 6, new Vector3() { x = 19, y = 993, z = -35 } },
            { 7, new Vector3() { x = 29, y = 992, z = -26 } },
            { 8, new Vector3() { x = 8, y = 992, z = -35 } },
            { 9, new Vector3() { x = 50, y = 992, z = -35 } },
            { 10, new Vector3() { x = 61, y = 992, z = -51 } },
            { 11, new Vector3() { x = 62, y = 996, z = -33 } },
            { 12, new Vector3() { x = 108, y = 996, z = -33 } },
            { 13, new Vector3() { x = 131, y = 995, z = -55 } }
        };

        [Description("A list of all the gun levels, and the loadouts. IDs of the gun levels must start from 0 and in order.")]
        public Dictionary<int, GunLevel> GunLevels { get; set; } = new()
        {
            {
                0,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunRevolver, ItemType.Ammo44cal },
                    Effects = new() { new Effect(EffectType.MovementBoost, 9999, 1) },
                    Appearance = RoleTypeId.ChaosRepressor,
                    MaxHealth = 100
                }
            },
            {
                1,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunFSP9, ItemType.ArmorLight , ItemType.Ammo9x19, },

                    Effects = new() { new Effect(EffectType.MovementBoost, 9999, 50) },
                    Appearance = RoleTypeId.FacilityGuard,
                    MaxHealth = 100
                }
            },
            {
                2,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunCrossvec, ItemType.ArmorLight, ItemType.Ammo9x19 },
                    Effects = new() { new Effect(EffectType.MovementBoost, 9999, 20) },
                    Appearance = RoleTypeId.NtfPrivate,
                    MaxHealth = 100
                }
            },
            {
                3,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunShotgun, ItemType.ArmorHeavy, ItemType.Ammo12gauge },
                    Effects = new() { new Effect(EffectType.MovementBoost, 9999, 10) },
                    Appearance = RoleTypeId.ChaosRepressor,
                    MaxHealth = 200
                }
            },       
            {
                4,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunAK, ItemType.ArmorCombat, ItemType.Ammo762x39 },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.ChaosRepressor,
                    MaxHealth = 100
                }
            },
            {
                5,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunE11SR, ItemType.ArmorCombat, ItemType.Ammo556x45 },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.NtfSergeant,
                    MaxHealth = 100
                }
            },
            {
                6,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunLogicer, ItemType.Medkit, ItemType.ArmorCombat, ItemType.Ammo762x39 },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.ChaosMarauder,
                    MaxHealth = 100
                }
            },
            {
                7,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunFRMG0, ItemType.ArmorCombat, ItemType.Ammo556x45 },
                    Effects = new() { new Effect(EffectType.MovementBoost, 1, 1) },
                    Appearance = RoleTypeId.NtfCaptain,
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
                    Loadout = new() { ItemType.GunFSP9, ItemType.ArmorCombat, ItemType.GrenadeHE, ItemType.GrenadeHE },
                    Effects = new() { new Effect(EffectType.MovementBoost, 9999, 1) },
                    Appearance = RoleTypeId.ClassD,
                    MaxHealth = 80
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
                    MaxHealth = 80
                }
            }
        };

    }
}
