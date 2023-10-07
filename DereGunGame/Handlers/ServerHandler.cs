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
        private readonly DereGunGame plugin;
        private readonly Config config;
        public ServerHandler(DereGunGame plugin)
        {
            this.plugin = plugin;
            config = plugin.Config;
        }

        public void OnDecontamination(DecontaminatingEventArgs ev)
        {
            if (!plugin.GunGameRound) return;

            ev.IsAllowed = false;
        }
        public void OnRoundStart()
        {
            if (config.AlwaysStart)
            {
                plugin.GunGameRound = true;
            }
            if (!plugin.GunGameRound) return;

            Round.IsLocked = true;
            plugin.Leaderboard.Clear();
            plugin.roundZone = plugin.Config.ZoneTypes.GetRandomValue().Value;
            
            Cassie.Message("Gun Game . First player to get a kill with every weapon wins", false, true, true);
            plugin.Utilities.PrepareMap(plugin.roundZone);

            GunLevel gunLevel = plugin.Config.GunLevels[0];
            //spawn all the players in spectator.
            foreach (Player player in Player.List)
            {
                plugin.Leaderboard.Add(player, 0);
                gunLevel.spawnPlayer(player);
            }


        }
    }
}
