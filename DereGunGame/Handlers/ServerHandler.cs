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

namespace DereGunGame.Handlers
{
    class ServerHandler
    {
        private DereGunGame plugin;
        public ServerHandler(DereGunGame plugin)
        {
            this.plugin = plugin;
        }

        public void OnRoundStart()
        {
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

                    door.KeycardPermissions = KeycardPermissions.None;
                    plugin.roundSpawnpoints.Add(plugin.roundSpawnpoints.Count(), door.Position+new Vector3(0, 1, 0));
                    door.IsOpen = true;
                }
                else
                {
                    door.IsOpen = false;
                    door.KeycardPermissions = KeycardPermissions.AlphaWarhead;
                }
                if (door.IsElevator || door.IsCheckpoint) door.ChangeLock(DoorLockType.AdminCommand);
                if (door.IsGate) door.IsOpen = true;
            }
            Log.Info("Finished loading doors.");
            foreach (Player player in Player.List)
            {
                plugin.Leaderboard.Add(player, 0);
                GunLevel gunLevel = plugin.Config.GunLevels[0];
                gunLevel.spawnPlayer(player);
            }


        }
    }
}
