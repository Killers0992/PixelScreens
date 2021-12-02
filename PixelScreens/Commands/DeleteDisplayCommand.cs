using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using LedDisplay;
using System;

namespace PixelScreens.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class DeleteDisplayCommand : ICommand
    {
        public string Command => "deletedisplay";

        public string[] Aliases => new string[] { "deleted" };

        public string Description => "Delete led display.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);

            if (!player.CheckPermission("pixelscreens.deletedisplay"))
            {
                response = "Missing permission: pixelscreens.deletedisplay!";
                return false;
            }

            if (arguments.Count < 0)
            {
                response = "Syntax: deletedisplay <id>";
                return false;
            }

            if (!int.TryParse(arguments.At(0), out int id))
            {
                response = "Failed parsing ID";
                return false;
            }

            if (LedDisplayController.LedDisplays.TryGetValue(id, out LedDisplayController display))
            {
                UnityEngine.Object.Destroy(display.gameObject);
                response = $"Deleted display with id {id}.";
                return true;
            }

            response = $"Display with id {id} not exists!";
            return false;
        }
    }
}
