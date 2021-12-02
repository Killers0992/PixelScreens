using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using LedDisplay;
using LedDisplay.Enums;
using System;
using System.Linq;
using UnityEngine;

namespace PixelScreens.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SetIntervalCommand : ICommand
    {
        public string Command => "setinterval";

        public string[] Aliases => new string[] { "seti" };

        public string Description => "Set interval of update text in led display.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);

            if (!player.CheckPermission("pixelscreens.setinterval"))
            {
                response = "Missing permission: pixelscreens.setinterval!";
                return false;
            }

            if (arguments.Count < 9)
            {
                response = "Syntax: setinterval <id> <interval(default 0.1)>";
                return false;
            }

            if (!int.TryParse(arguments.At(0), out int id))
            {
                response = "Failed parsing ID";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float interval))
            {
                response = "Failed parsing interval";
                return false;
            }

            if (LedDisplayController.LedDisplays.TryGetValue(id, out LedDisplayController display))
            {
                display.StartMove(interval);
                response = $"Set text interval to {interval} in display with id {id}.";
                return true;
            }

            response = $"Display with id {id} not exists!";
            return false;
        }
    }
}
