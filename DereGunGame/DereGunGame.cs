﻿using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;

namespace DereGunGame
{
    public class DereGunGame : Plugin<Config>
    {
        public override string Name => "DereGunGame";
        public override string Prefix => "GunGame";
        public override string Author => "@derewah";
        public override PluginPriority Priority { get; } = PluginPriority.Medium;


        private static readonly DereGunGame Singleton = new DereGunGame();

        private DereGunGame()
        {

        }

        public static DereGunGame Instance => Singleton;

        private Handlers.ServerHandler serverHandler;
        private Handlers.PlayerHandler playerHandler;

        public Dictionary<Exiled.API.Features.Player, int> Leaderboard = new();

        public override void OnEnabled()
        {
            base.OnEnabled();
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
            Player.Died += playerHandler.OnDied;
            Player.ReloadingWeapon += playerHandler.OnReload;
        }

        public void UnregisterEvents()
        {
            Server.RoundStarted -= serverHandler.OnRoundStart;
            Player.Spawned -= playerHandler.OnPlayerSpawned;
            Player.Left -= playerHandler.OnLeft;
            Player.Died -= playerHandler.OnDied;
            Player.ReloadingWeapon -= playerHandler.OnReload;

            playerHandler = null;
            serverHandler = null;
        }





    }
}