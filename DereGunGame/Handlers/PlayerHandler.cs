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
            Log.Info("OnDied");

            if (Round.IsEnded) return;

            ev.Player.ClearInventory();
            plugin.Leaderboard[ev.Attacker] = plugin.Leaderboard[ev.Attacker] + 1;
            if (plugin.Leaderboard[ev.Attacker] > plugin.Config.GunLevels.Count()-1)
            {
                if(Round.IsEnded) return;
                Round.EndRound();
                Map.ClearBroadcasts();
                Map.Broadcast(5, $"{ev.Attacker.Nickname} has won the GunGame!");
                
            }
            else
            {
                GunLevel gunLevel = plugin.Config.GunLevels.ContainsKey(plugin.Leaderboard[ev.Attacker]) ? plugin.Config.GunLevels[plugin.Leaderboard[ev.Attacker]] : null;
                if (gunLevel == null) return;
                gunLevel.giveLoadout(ev.Attacker);
                ev.Attacker.Broadcast(5, $"LV: {plugin.Leaderboard[ev.Attacker]+1} / {plugin.Config.GunLevels.Count()}", shouldClearPrevious: true);
            }
        }

        public void OnReload(ReloadingWeaponEventArgs ev)
        {
            Log.Info("Reload");
            if(ev.IsAllowed) ev.Player.SetAmmo(ev.Firearm.AmmoType, 100);
        }

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (Round.IsEnded) return;


            if (!plugin.Leaderboard.ContainsKey(ev.Player)) plugin.Leaderboard.Add(ev.Player, 0);

            if (ev.NewRole == RoleTypeId.Spectator)
            {
                GunLevel gunLevel = plugin.Config.GunLevels.ContainsKey(plugin.Leaderboard[ev.Player]) ? plugin.Config.GunLevels[plugin.Leaderboard[ev.Player]] : null;
                if (gunLevel != null) ev.NewRole = gunLevel.Appearance;
            }
        }

        public void OnPlayerSpawned(SpawnedEventArgs ev)
        {
            if (Round.IsEnded) return;

            Log.Info("Spawned");


            GunLevel gunLevel = plugin.Config.GunLevels.ContainsKey(plugin.Leaderboard[ev.Player]) ? plugin.Config.GunLevels[plugin.Leaderboard[ev.Player]] : null;

            if (ev.Player.Role.Type != RoleTypeId.Spectator && gunLevel != null)
            {
                ev.Player.Teleport(plugin.Config.SpawnLocations.Values.ToList().RandomItem());

                if (gunLevel != null) gunLevel.giveLoadout(ev.Player);
                else Log.Error($"Could not find a GunLevel with numeric ID {plugin.Leaderboard[ev.Player]}.");
            }


        }

        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            ev.Pickup.Destroy();
            ev.IsAllowed = false;
        }

        public void OnLeft(LeftEventArgs ev)
        {
            if (plugin.Leaderboard.ContainsKey(ev.Player)) plugin.Leaderboard.Remove(ev.Player);
        }
    }
}
