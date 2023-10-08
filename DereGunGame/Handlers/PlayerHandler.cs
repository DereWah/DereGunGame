using DereGunGame.Types;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DereGunGame.Handlers
{
    class PlayerHandler
    {

        private readonly DereGunGame plugin;
        private readonly Config config;
        public PlayerHandler(DereGunGame plugin)
        {
            this.plugin = plugin;
            config = plugin.Config;
        }

        public void OnRagdoll(SpawningRagdollEventArgs ev)
        {
            if (!plugin.GunGameRound) return;

            ev.IsAllowed = false;
        }

        public void OnDying(DyingEventArgs ev)
        {
            if (!plugin.GunGameRound) return;

            plugin.Utilities.ClearInventory(ev.Player);
            //replace the drops with a random drop out of the config.
            ev.Player.AddItem(plugin.Config.DeathDrops.Values.ToList().RandomItem());
        }

        public void OnDead(DiedEventArgs ev)
        {
            if (!plugin.GunGameRound) return;

            ev.Player.ClearInventory();

            // Handle disconnection.
            if (ev.DamageHandler.Type == DamageType.Unknown
                || Round.IsEnded)
            {
                return;
            }

            Player EventAttacker = ev.Attacker is not null ? ev.Attacker : ev.Player;
            GunLevel AttackerGunLevel = plugin.Utilities.GetGunLevel(EventAttacker);

            bool killedByJailbird = (ev.DamageHandler.Type == DamageType.Jailbird
                && !AttackerGunLevel.Loadout.Contains(ItemType.Jailbird));
            bool humiliation = (ev.DamageHandler.IsSuicide
                || killedByJailbird
                || ev.DamageHandler.Type == DamageType.Falldown);


            switch (humiliation)
            {
                case true when plugin.Leaderboard[ev.Player] + config.JailbirdSettings.HumiliationPenalty >= 0:
                    plugin.Leaderboard[ev.Player] += config.JailbirdSettings.HumiliationPenalty;
                    goto default;
                case true:
                    plugin.Leaderboard[ev.Player] = 0;
                    goto default;
                case false when (plugin.Leaderboard[ev.Attacker] + 1 >= config.GunLevels.Count() - 1):
                    plugin.Utilities.EndRound(EventAttacker);
                    break;
                case false:
                    plugin.Utilities.AwardKill(EventAttacker, ev.Player);
                    break;
                default:
                    ev.Player.ShowHint($"<color=#f5a742>Humiliated by </color><color=#660708>{EventAttacker.Nickname}<color=#f5a742>!", 3);
                    Cassie.Message($".G7 {EventAttacker.Nickname} Humiliated {ev.Player.Nickname}", false, false, true);
                    plugin.Utilities.ShowLeaderboard(ev.Player);
                    break;
            }
        }

        public void OnReload(ReloadingWeaponEventArgs ev)
        {
            if (!plugin.GunGameRound) return;

            if (ev.Firearm.Type != ItemType.ParticleDisruptor)
            {
                ev.Player.SetAmmo(ev.Firearm.AmmoType, ev.Firearm.MaxAmmo);
                
                if(ev.Firearm.Ammo < ev.Firearm.MaxAmmo)
                {
                    byte mult = plugin.Utilities.GetGunLevel(ev.Player).ReloadSpeedMultiplier;
                    ev.Player.EnableEffect(new Effect(EffectType.Scp1853, 1f, mult));
                }

            }
        }

        public void OnPlayerSpawned(SpawnedEventArgs ev)
        {
            if (!plugin.GunGameRound
                || Round.IsEnded) return;

            if (!plugin.Leaderboard.ContainsKey(ev.Player))
            {
                plugin.Leaderboard.Add(ev.Player, 0);
            }

            GunLevel gunLevel = plugin.Utilities.GetGunLevel(ev.Player);

            switch (ev.Player.Role.Type == RoleTypeId.Spectator) {
                case false when gunLevel is not null:
                    // teleport players after they spawned with their role.
                    plugin.Utilities.RandomTeleport(ev.Player);
                    gunLevel.giveLoadout(ev.Player, plugin);
                    break;
                case false:
                    Log.Error($"Could not find a GunLevel with numeric ID {plugin.Leaderboard[ev.Player]}.");
                    break;
                default:
                    //spawn players after they spawned in spectators.
                    Timing.CallDelayed(plugin.Config.RespawnDelay, () =>
                    {
                        gunLevel.spawnPlayer(ev.Player);
                    });
                    break;
            }
        }

        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (plugin.GunGameRound)
            {
                ev.IsAllowed = false;
            }
        }

        public void OnDroppingAmmo(DroppingAmmoEventArgs ev)
        {
            if (plugin.GunGameRound)
            {
                ev.IsAllowed = false;
            }
        }

        public void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (!plugin.GunGameRound) return;

            if (!config.DeathDrops.Values.Contains(ev.Pickup.Type))
            {
                ev.Pickup.Destroy();
                ev.IsAllowed = false;
            }

        }

        public void OnLeft(LeftEventArgs ev)
        {
            if (!plugin.GunGameRound) return;

            if (plugin.Leaderboard.ContainsKey(ev.Player))
            {
                plugin.Leaderboard.Remove(ev.Player);
            }
        }
    }
}
