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
            foreach (Player player in Player.List)
            {
                plugin.Leaderboard.Add(player, 0);
                GunLevel gunLevel = plugin.Config.GunLevels[0];
                gunLevel.spawnPlayer(player);
            }
        }
    }
}
