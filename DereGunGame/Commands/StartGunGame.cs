using System;
using CommandSystem;

namespace DereGunGame.Commands
{
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class StartGunGame : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "forcestartgungame";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "startgungame" };

        /// <inheritdoc/>
        public string Description { get; } = "Start a new GunGame in the following round.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("deregungame.start"))
            {
                response = "You don't have the permission deregungame.start";
                return false;
            }
            if (Round.IsLobby)
            {
                DereGunGame.Instance.GunGameRound = !DereGunGame.Instance.GunGameRound;
                response = $"GunGame round set to {DereGunGame.Instance.GunGameRound}";
            }
            else
            {
                response = "You can only start a GunGame while in the lobby.";
            }


            // Return true if the command was executed successfully; otherwise, false.
            return true;
        }
    }
}