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

namespace DereGunGame
{
    public class Config : IConfig
    {

        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

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

        public Dictionary<int, GunLevel> GunLevels { get; set; } = new()
        {
            {
                0,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunCOM18, ItemType.Painkillers, ItemType.Medkit, ItemType.ArmorLight , ItemType.Ammo9x19 },

                    Effects = new() { new Effect(EffectType.MovementBoost, 9999, 1) },
                    Appearance = RoleTypeId.ClassD,
                    MaxHealth = 100
                }
            },
            {
                1,
                new GunLevel()
                {
                    Loadout = new() { ItemType.GunCOM15, ItemType.Painkillers, ItemType.Medkit },
                    Effects = new() { new Effect(EffectType.MovementBoost, 9999, 1) },
                    Appearance = RoleTypeId.NtfCaptain,
                    MaxHealth = 100
                }
            }
        };

    }
}
