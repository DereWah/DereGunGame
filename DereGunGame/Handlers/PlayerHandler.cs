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

        private DereGunGame plugin;
        public PlayerHandler(DereGunGame plugin)
        {
            this.plugin = plugin;
        }

        public void OnRagdoll(SpawningRagdollEventArgs ev)
        {
            if (!DereGunGame.Singleton.GunGameRound) return;

            ev.IsAllowed = false;
        }

        public void OnDying(DyingEventArgs ev)
        {
            if (!DereGunGame.Singleton.GunGameRound) return;

            //remove all drops, including AMMOS
            foreach (AmmoType at in Enum.GetValues(typeof(AmmoType))) ev.Player.SetAmmo(at, 0);
            ev.Player.ClearInventory();

            //replace the drops with a random drop out of the config.
            ev.Player.AddItem(plugin.Config.DeathDrops.Values.ToList().RandomItem());
        }

        private void showLeaderboard(Player p1)
        {
            //do not display the leaderboard after the round winner is being displayed.
            if (Round.IsEnded) return;

            //get sorted list of all players and their level and display it to all players.
            Dictionary<Player, int> ordered = plugin.Leaderboard.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            foreach(Player p in Player.List) p.Broadcast(30, $"<color=#f6aa1c>{ordered.Keys.First().Nickname}<color=#621708> in the lead <color=#941b0c>[{ordered[ordered.Keys.First()]+1} / {plugin.Config.GunLevels.Count()}].<color=#bc3908> You: <color=#f6aa1c>#{ordered.Keys.ToList().IndexOf(p)+1}", shouldClearPrevious: true);
        }

        public GunLevel getGunLevel(Player player)
        {
            if (player == null) return null;

            int level = plugin.Leaderboard.ContainsKey(player) ? plugin.Leaderboard[player] : 0;

            if (plugin.Config.GunLevels.ContainsKey(level)) return plugin.Config.GunLevels[level];
  
            return null;
        }

        public void MakeEquipFirstWeapon(Player player)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (Item i in player.Items)
                {
                    if (player.CurrentItem != i)
                    {
                        player.CurrentItem = i;
                    }
                    break;
                }

            });
        }

        public void OnDead(DiedEventArgs ev)
        {
            if (!DereGunGame.Singleton.GunGameRound) return;


            ev.Player.ClearInventory();

            // Handle disconnection.
            if (ev.DamageHandler.Type == DamageType.Unknown) return;

            //We don't care about kills when round is ended.
            if (Round.IsEnded) return;

            GunLevel AttackerGunLevel = ev.Attacker != null ? getGunLevel(ev.Attacker) : getGunLevel(ev.Player);
            bool humiliation = (ev.DamageHandler.IsSuicide ||
                (ev.DamageHandler.Type == DamageType.Jailbird && !AttackerGunLevel.Loadout.Contains(ItemType.Jailbird)) || 
                ev.DamageHandler.Type == DamageType.Falldown);
            if (humiliation)
            {
                if (plugin.Leaderboard[ev.Player] + plugin.Config.HumiliationPenalty >= 0) plugin.Leaderboard[ev.Player] += plugin.Config.HumiliationPenalty;
                else plugin.Leaderboard[ev.Player] = 0;
                Player EventAttacker = ev.Attacker != null ? ev.Attacker : ev.Player;
                ev.Player.ShowHint($"<color=#f5a742>Humiliated by </color><color=#660708>{EventAttacker.Nickname}<color=#f5a742>!", 3);
                Cassie.Message($".G7 {EventAttacker.Nickname} Humiliated {ev.Player.Nickname}", false, false, true);
                showLeaderboard(ev.Player);

                return;
            }
            else
            {
                plugin.Leaderboard[ev.Attacker] += 1;
                AttackerGunLevel = getGunLevel(ev.Attacker);
                bool FinalKill = !(plugin.Leaderboard[ev.Attacker] <= plugin.Config.GunLevels.Count() - 1);
                if (!FinalKill)
                {
                    if (AttackerGunLevel == null) return;
                    AttackerGunLevel.giveLoadout(ev.Attacker, plugin);
                    showLeaderboard(ev.Player);
                    ev.Attacker.ShowHint($"<color=#00171F>LV: <color=#003459>{plugin.Leaderboard[ev.Attacker] + 1} </color>/<color=#007ea7> {plugin.Config.GunLevels.Count()}");
                    Cassie.Clear();
                    Cassie.Message($"{ev.Attacker.Nickname} killed {ev.Player.Nickname}", false, false, true);
                    MakeEquipFirstWeapon(ev.Attacker);
                }
                else
                {
                    Round.EndRound();
                    Map.ClearBroadcasts();
                    Map.Broadcast(5, $"<color=#ff9500>{ev.Attacker.Nickname}<color=#ffa200> has won<color=#ffaa00> the <color=#ffb700>Gun<color=#ffc300>Game!", shouldClearPrevious: true);
                    Cassie.Message("Game Over", false, false, true);
                    foreach (Player player in Player.List) player.ClearInventory();
                    Timing.CallDelayed(5f, () => Round.Restart());
                    plugin.roundSpawnpoints.Clear();
                }
            }
        }


        public void OnReload(ReloadingWeaponEventArgs ev)
        {
            if (!DereGunGame.Singleton.GunGameRound) return;

            if (ev.Firearm.Type == ItemType.ParticleDisruptor) return;
            byte mult = getGunLevel(ev.Player).ReloadSpeedMultiplier;
            if (ev.IsAllowed)
            {
                ev.Player.EnableEffect(new Effect(EffectType.Scp1853, 1f, mult));
                ev.Player.SetAmmo(ev.Firearm.AmmoType, ev.Firearm.MaxAmmo);
            }
        }

        public void RandomTeleport(Player player)
        {
            bool FitLocation = false;
            Vector3 ActualLocation = plugin.roundSpawnpoints.Values.ToList().RandomItem();
            int attempt = 0;

            if (plugin.roundSpawnpoints.IsEmpty()) return;

            while(FitLocation == false && attempt < 10)
            {
                ActualLocation = plugin.roundSpawnpoints.Values.ToList().RandomItem();
                FitLocation = true;
                foreach(Player OtherPlayer in Player.List.Where(p => p.IsAlive && p != player))
                {
                    if (Math.Abs((Vector3.Distance(ActualLocation, OtherPlayer.Position))) > plugin.Config.SpawnRadius)
                    {
                        FitLocation = true;
                    }
                    else
                    {
                        FitLocation = false;
                        break;
                    }
                }
                attempt++;
            }
            if (FitLocation == false) ActualLocation = plugin.roundSpawnpoints.Values.ToList().RandomItem();
            if (player.IsAlive) player.Teleport(ActualLocation);
        }

        public void OnPlayerSpawned(SpawnedEventArgs ev)
        {
            if (!DereGunGame.Singleton.GunGameRound) return;

            if (Round.IsEnded) return;
            if (!plugin.Leaderboard.ContainsKey(ev.Player)) plugin.Leaderboard.Add(ev.Player, 0);

            GunLevel gunLevel = getGunLevel(ev.Player);
           
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
                    gunLevel = getGunLevel(ev.Player);
                    gunLevel.spawnPlayer(ev.Player);
                });
                
            }


        }

        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (!DereGunGame.Singleton.GunGameRound) return;

            ev.IsAllowed = false;
        }

        public void OnDroppingAmmo(DroppingAmmoEventArgs ev)
        {
            if (!DereGunGame.Singleton.GunGameRound) return;

            ev.IsAllowed = false;
        }

        public void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (!DereGunGame.Singleton.GunGameRound) return;

            if (!plugin.Config.DeathDrops.Values.Contains(ev.Pickup.Type))
            {
                ev.Pickup.Destroy();
                ev.IsAllowed = false;
            }

        }

        public void OnLeft(LeftEventArgs ev)
        {
            if (!DereGunGame.Singleton.GunGameRound) return;

            if (plugin.Leaderboard.ContainsKey(ev.Player)) plugin.Leaderboard.Remove(ev.Player);
        }
    }
}
