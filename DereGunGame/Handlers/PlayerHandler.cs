using DereGunGame.Types;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
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

        public void OnDied(DiedEventArgs ev)
        {
            ev.Player.ClearInventory();
            Log.Info("ph1");
            plugin.Leaderboard[ev.Attacker] = plugin.Leaderboard[ev.Attacker] + 1;
            Log.Info("ph2");
            ev.Attacker.Broadcast(3, $"You killed {ev.Player.Nickname}.");
            Log.Info("ph2");
            if (plugin.Leaderboard[ev.Attacker] > plugin.Config.GunLevels.Count()-1)
            {
                Map.ClearBroadcasts();
                Map.Broadcast(5, $"{ev.Attacker.Nickname} has won the GunGame!");
                Round.EndRound();
            }
        }

        public void OnReload(ReloadingWeaponEventArgs ev)
        {
            if(ev.IsAllowed) ev.Player.SetAmmo(ev.Firearm.AmmoType, 100);
        }

        public void OnPlayerSpawned(SpawnedEventArgs ev)
        {
            if (!plugin.Leaderboard.ContainsKey(ev.Player)) plugin.Leaderboard.Add(ev.Player, 0);

            GunLevel gunLevel = plugin.Config.GunLevels.ContainsKey(plugin.Leaderboard[ev.Player]) ? plugin.Config.GunLevels[plugin.Leaderboard[ev.Player]] : null;

            if (ev.Player.Role.Type == RoleTypeId.Spectator && gunLevel != null) gunLevel.spawnPlayer(ev.Player);
            else
            {
                ev.Player.Teleport(plugin.Config.SpawnLocations.Values.ToList().RandomItem());

                if (gunLevel != null) gunLevel.giveLoadout(ev.Player);
                else Log.Error($"Could not find a GunLevel with numeric ID {plugin.Leaderboard[ev.Player]}.");
            }


        }

        public void OnLeft(LeftEventArgs ev)
        {
            if (plugin.Leaderboard.ContainsKey(ev.Player)) plugin.Leaderboard.Remove(ev.Player);
        }
    }
}
