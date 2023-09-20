using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using PlayerRoles;
using DereGunGame.Types;
using Exiled.API.Features.Doors;
using Exiled.API.Extensions;
using UnityEngine;
using Exiled.Events.EventArgs.Map;

namespace DereGunGame.Handlers
{
    class ServerHandler
    {
        private DereGunGame plugin;
        public ServerHandler(DereGunGame plugin)
        {
            this.plugin = plugin;
        }

        public void OnDecontamination(DecontaminatingEventArgs ev)
        {
            if (!DereGunGame.Singleton.GunGameRound) return;
            ev.IsAllowed = false;
        }
        public void OnRoundStart()
        {
            DereGunGame.Singleton.FirstKill = false;
            if (DereGunGame.Singleton.Config.AlwaysStart) DereGunGame.Singleton.GunGameRound = true;
            if (!DereGunGame.Singleton.GunGameRound) return;

            Round.IsLocked = true;
            plugin.Leaderboard.Clear();
            plugin.roundZone = plugin.Config.ZoneTypes.GetRandomValue().Value;
            
            Cassie.Message("Gun Game . First player to get a kill with every weapon wins", false, true, true);
            if (plugin.roundZone == ZoneType.Surface)
            {
                plugin.roundSpawnpoints = plugin.Config.SpawnLocations;
            }
            foreach (Door door in Door.List)
            {
                if(door.Zone == plugin.roundZone)
                {
                    //prevent spawning in unreachable rooms such as nuke or hcz049
                    if (door.Room.Type == RoomType.Hcz049 || door.Room.Type == RoomType.HczNuke || door.Room.Type == RoomType.Hcz106 || door.Room.Type == RoomType.Hcz049) continue;
                    //allow all doors in the play zone to be opened
                    door.KeycardPermissions = KeycardPermissions.None;
                    if(!door.Type.IsElevator() && door.Room.Type != RoomType.LczClassDSpawn) plugin.roundSpawnpoints.Add(plugin.roundSpawnpoints.Count(), door.Position+new Vector3(0, 1, 0));
                    door.IsOpen = true;
                }
                else
                {
                    //lock all the doors outside of the play zone
                    door.IsOpen = false;
                    door.KeycardPermissions = KeycardPermissions.AlphaWarhead;
                }
                //block elevators and checkpoint from being opened
                if (door.IsElevator || door.IsPartOfCheckpoint)
                {
                    door.IsOpen = false;
                    door.ChangeLock(DoorLockType.AdminCommand);
                }
            }

            //spawn all the players in spectator.
            foreach (Player player in Player.List)
            {
                plugin.Leaderboard.Add(player, 0);
                GunLevel gunLevel = plugin.Config.GunLevels[0];
                gunLevel.spawnPlayer(player);
            }


        }
    }
}
