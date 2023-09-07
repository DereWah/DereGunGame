using System;
using CommandSystem;

namespace DereGunGame.Commands
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;


    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SetLocation : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "getlocation";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "getloc" };

        /// <inheritdoc/>
        public string Description { get; } = "Set a spawn location for the GunGame plugin.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            
            response = $"Your location is ({player.Position}).";

            // Return true if the command was executed successfully; otherwise, false.
            return true;
        }
    }
}