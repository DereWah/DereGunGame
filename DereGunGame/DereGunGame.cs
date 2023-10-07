using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;
using Map = Exiled.Events.Handlers.Map;
using UnityEngine;
using DereGunGame.Utils;

namespace DereGunGame
{
    public class DereGunGame : Plugin<Config>
    {
        public override string Name => "DereGunGame";
        public override string Prefix => "GunGame";
        public override string Author => "@derewah";
        public override PluginPriority Priority { get; } = PluginPriority.Medium;


        public static DereGunGame Instance;

        private Handlers.ServerHandler serverHandler;
        private Handlers.PlayerHandler playerHandler;

        public Dictionary<Exiled.API.Features.Player, int> Leaderboard = new();
        public ZoneType roundZone;
        public List<Vector3> roundSpawnpoints = new();
        public bool GunGameRound = false;
        public Utilities Utilities;

        
        public override void OnEnabled()
        {
            base.OnEnabled();
            Instance = this;
            Utilities = new Utilities(this);
            RegisterEvents();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            base.OnDisabled();
        }

        public void RegisterEvents()
        {
            playerHandler = new Handlers.PlayerHandler(this);
            serverHandler = new Handlers.ServerHandler(this);

            Server.RoundStarted += serverHandler.OnRoundStart;
            Player.Spawned += playerHandler.OnPlayerSpawned;
            Player.Left += playerHandler.OnLeft;
            Player.Died += playerHandler.OnDead;
            Player.ReloadingWeapon += playerHandler.OnReload;
            Player.DroppingItem += playerHandler.OnDroppingItem;
            Player.PickingUpItem += playerHandler.OnPickingUpItem;
            Player.Dying += playerHandler.OnDying;
            Player.DroppingAmmo += playerHandler.OnDroppingAmmo;
            Player.SpawningRagdoll += playerHandler.OnRagdoll;
            Map.Decontaminating += serverHandler.OnDecontamination;
            
        }

        public void UnregisterEvents()
        {
            Server.RoundStarted -= serverHandler.OnRoundStart;
            Player.Spawned -= playerHandler.OnPlayerSpawned;
            Player.Left -= playerHandler.OnLeft;
            Player.Died -= playerHandler.OnDead;
            Player.ReloadingWeapon -= playerHandler.OnReload;
            Player.DroppingItem -= playerHandler.OnDroppingItem;
            Player.PickingUpItem -= playerHandler.OnPickingUpItem;
            Player.Dying -= playerHandler.OnDying;
            Player.DroppingAmmo -= playerHandler.OnDroppingAmmo;
            Map.Decontaminating -= serverHandler.OnDecontamination;

            playerHandler = null;
            serverHandler = null;
        }





    }
}
