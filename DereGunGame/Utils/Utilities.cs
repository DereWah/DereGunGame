using DereGunGame.Types;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DereGunGame.Utils
{
    public class Utilities
    {
        private readonly DereGunGame plugin;
        private readonly Config config;
        public Utilities(DereGunGame plugin)
        {
            this.plugin = plugin;
            config = plugin.Config;
        }

        public void ClearInventory(Player player)
        {
            foreach (AmmoType at in Enum.GetValues(typeof(AmmoType)))
            {
                if(player.GetAmmo(at) > 0)
                {
                    player.SetAmmo(at, 0);
                }
            }
            player.ClearInventory();
        }

        public void ShowLeaderboard(Player player)
        {
            //do not display the leaderboard after the round winner is being displayed.
            if (!Round.IsEnded)
            {
                //get sorted list of all players and their level and display it to all players.
                Dictionary<Player, int> ordered = plugin.Leaderboard.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                foreach (Player p in Player.List)
                {
                    p.Broadcast(30, $"<color=#f6aa1c>{ordered.Keys.First().Nickname}<color=#621708>"
                        + $"in the lead <color=#941b0c>[{ordered[ordered.Keys.First()] + 1} / {plugin.Config.GunLevels.Count()}]."
                        + $"<color=#bc3908> You: <color=#f6aa1c>#{ordered.Keys.ToList().IndexOf(p) + 1}", shouldClearPrevious: true);
                }

            }
        }

        /// <summary>
        /// Gets a player's gun level.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns></returns>
        public GunLevel GetGunLevel(Player player)
        {
            GunLevel gunLevel = null;
            if (player != null)
            {
                if (!plugin.Leaderboard.TryGetValue(player, out int level))
                {
                    level = 0;
                }
                config.GunLevels.TryGetValue(level, out gunLevel);
            }

            return gunLevel;
        }

        public void MakeEquipFirstWeapon(Player player)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                Item i = player.Items.FirstOrDefault(i => player.CurrentItem != i);
                player.CurrentItem = i;
            });
        }

        public void EndRound(Player winner)
        {
            Round.EndRound();
            Map.ClearBroadcasts();
            Map.Broadcast(5, $"<color=#ff9500>{winner.Nickname}<color=#ffa200> has won<color=#ffaa00> the <color=#ffb700>Gun<color=#ffc300>Game!", shouldClearPrevious: true);
            Cassie.Message("Game Over", false, false, true);
            foreach (Player player in Player.List)
            {
                player.ClearInventory();
            }
            Timing.CallDelayed(5f, () => Round.Restart());
            plugin.roundSpawnpoints.Clear();
        }

        public void AwardKill(Player attacker, Player victim)
        {
            GunLevel AttackerGunLevel = GetGunLevel(attacker);
            if (AttackerGunLevel != null)
            {
                AttackerGunLevel.giveLoadout(attacker, plugin);
                plugin.Utilities.ShowLeaderboard(victim);
                attacker.ShowHint($"<color=#00171F>LV: <color=#003459>{plugin.Leaderboard[attacker] + 1} </color>/<color=#007ea7> {plugin.Config.GunLevels.Count()}");
                Cassie.Clear();
                Cassie.Message($"{attacker.Nickname} killed {victim.Nickname}", false, false, true);
                plugin.Utilities.MakeEquipFirstWeapon(attacker);
            }
        }

        public void RandomTeleport(Player player)
        {
            if (!plugin.roundSpawnpoints.IsEmpty())
            {
                bool fitLocation = false;
                Vector3 actualLocation = plugin.roundSpawnpoints.RandomItem();
                int attempt = 0;

                while (fitLocation == false
                    && attempt < 10)
                {
                    actualLocation = plugin.roundSpawnpoints.RandomItem();
                    fitLocation = true;
                    foreach (Player OtherPlayer in Player.List.Where(p => p.IsAlive && p != player))
                    {
                        if (Math.Abs((Vector3.Distance(actualLocation, OtherPlayer.Position))) > plugin.Config.SpawnRadius)
                        {
                            fitLocation = true;
                        }
                        else
                        {
                            fitLocation = false;
                            break;
                        }
                    }
                    attempt++;
                }
                if (fitLocation == false)
                {
                    actualLocation = plugin.roundSpawnpoints.RandomItem();
                }
                if (player.IsAlive)
                {
                    player.Teleport(actualLocation);
                }
            }
        }

        public void PrepareMap(ZoneType roundZone)
        {
            if (roundZone == ZoneType.Surface)
            {
                plugin.roundSpawnpoints = plugin.Config.SpawnLocations.Values.ToList();
            }
            foreach (Door door in Door.List)
            {
                if (door.Zone == roundZone)
                {
                    //prevent spawning in unreachable rooms such as nuke or hcz049
                    if (!(door.Room.Type == RoomType.Hcz049
                        || door.Room.Type == RoomType.HczNuke
                        || door.Room.Type == RoomType.Hcz106
                        || door.Room.Type == RoomType.Hcz049))
                    {
                        //allow all doors in the play zone to be opened
                        door.IsOpen = true;
                        door.KeycardPermissions = KeycardPermissions.None;
                        //register all doors to the spawnpoints
                        if (!door.Type.IsElevator()
                            && door.Room.Type != RoomType.LczClassDSpawn)
                        {
                            Vector3 newSpawnpoint = door.Position + new Vector3(0, 1, 0);
                            plugin.roundSpawnpoints.Add(newSpawnpoint);
                        }
                    }
                }
                else
                {
                    //lock all the doors outside of the play zone
                    door.IsOpen = false;
                    door.KeycardPermissions = KeycardPermissions.AlphaWarhead;
                }
                //block elevators and checkpoint from being opened
                if (door.IsElevator
                    || door.IsPartOfCheckpoint)
                {
                    door.IsOpen = false;
                    door.ChangeLock(DoorLockType.AdminCommand);
                }
            }
        }
    }
}
