using System;
using CommandSystem;

namespace DereGunGame.Commands
{
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SetLocation : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "getlocation";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "getloc" };

        /// <inheritdoc/>
        public string Description { get; } = "Get your world location coordinates.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("deregungame.getlocation"))
            {
                response = "You don't have the permission deregungame.getlocation";
                return false;
            }
            Player player = Player.Get(sender);
            response = $"Your location is {player.Position}. Add it in the config to add a new location for the surface zone GunGame..";

            // Return true if the command was executed successfully; otherwise, false.
            return true;
        }
    }
}