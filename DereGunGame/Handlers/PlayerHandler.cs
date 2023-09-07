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

namespace DereGunGame.Handlers
{
    class PlayerHandler
    {

        private DereGunGame plugin;
        public PlayerHandler(DereGunGame plugin)
        {
            this.plugin = plugin;
        }

        public void OnRagdoll(SpawningRagdollEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnDying(DyingEventArgs ev)
        {
            foreach (AmmoType at in Enum.GetValues(typeof(AmmoType))) ev.Player.SetAmmo(at, 0);
            ev.Player.ClearInventory();
            ev.Player.AddItem(plugin.Config.DeathDrops.Values.ToList().RandomItem());
            Log.Info("Dying");
        }

        private void showLeaderboard(Player p1)
        {
            if (Round.IsEnded) return;
            Dictionary<Player, int> ordered = plugin.Leaderboard.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            foreach(Player p in Player.List) p.Broadcast(30, $"{ordered.Keys.First().Nickname} in the lead [{ordered[ordered.Keys.First()]+1} / {plugin.Config.GunLevels.Count()}]. You: #{ordered.Keys.ToList().IndexOf(p)+1}", shouldClearPrevious: true);
        }

        public void OnDead(DiedEventArgs ev)
        {
            if (ev.Attacker == null) return;
            Log.Info("Died");
            ev.Player.ClearInventory();

            // Handle disconnection. When falling down attacker is not set, so we skip
            if (!ev.Player.IsConnected || !plugin.Leaderboard.ContainsKey(ev.Player) || (ev.Attacker == null && !(ev.DamageHandler.Type == DamageType.Falldown))) return;

            // get the gun level of the attacker, if set.
            GunLevel gunLevel = plugin.Config.GunLevels.ContainsKey(plugin.Leaderboard[ev.Attacker]) ? (plugin.Config.GunLevels[plugin.Leaderboard[ev.Attacker]]) : null;

            bool humiliation = ((ev.DamageHandler.Type == DamageType.Jailbird && !gunLevel.Loadout.Contains(ItemType.Jailbird)) || ev.DamageHandler.IsSuicide);
            if (Round.IsEnded) return;

            if (humiliation)
            {
                plugin.Leaderboard[ev.Player] = plugin.Leaderboard[ev.Player] + plugin.Config.HumiliationPenalty >= 0 ? plugin.Leaderboard[ev.Player] + plugin.Config.HumiliationPenalty : plugin.Leaderboard[ev.Player] = 0;
                ev.Player.ShowHint($"Humiliated by {ev.Attacker.Nickname}!", 3);
                Cassie.Message("Humiliation!", false, false, true);
                showLeaderboard(ev.Player);
                return;
            }

            if (ev.DamageHandler.IsSuicide) return;

            plugin.Leaderboard[ev.Attacker] = plugin.Leaderboard[ev.Attacker] + 1;
            gunLevel = plugin.Config.GunLevels.ContainsKey(plugin.Leaderboard[ev.Attacker]) ? (plugin.Config.GunLevels[plugin.Leaderboard[ev.Attacker]]) : null;
            if (plugin.Leaderboard[ev.Attacker] > plugin.Config.GunLevels.Count()-1)
            {
                if(Round.IsEnded) return;
                Round.EndRound();
                Map.ClearBroadcasts();
                Map.Broadcast(5, $"<color=red>{ev.Attacker.Nickname} has won the GunGame!");
                Cassie.Message("Game Over", false, false, true);
                Timing.CallDelayed(5f, () => Round.Restart());
                
            }
            else
            {
                if (gunLevel == null) return;
                gunLevel.giveLoadout(ev.Attacker, plugin);
                showLeaderboard(ev.Player);
                ev.Attacker.ShowHint($"LV: {plugin.Leaderboard[ev.Attacker] + 1} / {plugin.Config.GunLevels.Count()}");
                Timing.CallDelayed(0.1f, () =>
                {
                    foreach (Item i in ev.Attacker.Items)
                    {
                        if (ev.Attacker.CurrentItem != i)
                        {
                            ev.Attacker.CurrentItem = i;
                        }

                        Log.Info(i);
                        break;
                    }

                });
                

            }
        }



        //bugged in exiled 8.1.0
        public void OnChargingJailbird(ChargingJailbirdEventArgs ev)
        {
            Log.Info("Charging jailbird");
            ((Jailbird)ev.Item).FlashDuration = 0f;
            ev.IsAllowed = false;
        }

        public void OnReload(ReloadingWeaponEventArgs ev)
        {
            Log.Info("Reload");
            if (ev.Firearm.Type == ItemType.ParticleDisruptor) return;
            byte mult = plugin.Config.GunLevels[plugin.Leaderboard[ev.Player]].ReloadSpeedMultiplier;
            if (ev.IsAllowed)
            {
                ev.Player.EnableEffect(new Effect(EffectType.Scp1853, 1f, mult));
                ev.Player.SetAmmo(ev.Firearm.AmmoType, ev.Firearm.MaxAmmo);
            }
        }

        public void RandomTeleport(Player player)
        {
            if(player.IsAlive) player.Teleport(plugin.roundSpawnpoints.Values.ToList().RandomItem());
        }

        public void OnPlayerSpawned(SpawnedEventArgs ev)
        {
            if (Round.IsEnded) return;

            Log.Info("Spawned");
            if (!plugin.Leaderboard.ContainsKey(ev.Player)) plugin.Leaderboard.Add(ev.Player, 0);

            GunLevel gunLevel = plugin.Config.GunLevels.ContainsKey(plugin.Leaderboard[ev.Player]) ? plugin.Config.GunLevels[plugin.Leaderboard[ev.Player]] : null;
            // teleport players after they spawned with their role.
            if (ev.Player.Role.Type != RoleTypeId.Spectator && gunLevel != null)
            {
                RandomTeleport(ev.Player);
                if (gunLevel != null) gunLevel.giveLoadout(ev.Player, plugin);
                else Log.Error($"Could not find a GunLevel with numeric ID {plugin.Leaderboard[ev.Player]}.");
            }
            else
            {
                //spawn players after they spawned in spectators.
                Timing.CallDelayed(plugin.Config.RespawnDelay, () =>
                {
                    Log.Info($"Delayed, {gunLevel.Appearance}");
                    gunLevel = plugin.Config.GunLevels.ContainsKey(plugin.Leaderboard[ev.Player]) ? plugin.Config.GunLevels[plugin.Leaderboard[ev.Player]] : null;
                    gunLevel.spawnPlayer(ev.Player);
                });
                
            }


        }

        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnDroppingAmmo(DroppingAmmoEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if(!plugin.Config.DeathDrops.Values.Contains(ev.Pickup.Type))
            {
                ev.Pickup.Destroy();
                ev.IsAllowed = false;
            }

        }

        public void OnLeft(LeftEventArgs ev)
        {
            if (plugin.Leaderboard.ContainsKey(ev.Player)) plugin.Leaderboard.Remove(ev.Player);
        }
    }
}
