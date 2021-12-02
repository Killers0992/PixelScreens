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
    public class RemoveTextCommand : ICommand
    {
        public string Command => "removetext";

        public string[] Aliases => new string[] { "removet" };

        public string Description => "Remove text from led display.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);

            if (!player.CheckPermission("pixelscreens.removetext"))
            {
                response = "Missing permission: pixelscreens.removetext!";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Syntax: removetext <id> <textId>";
                return false;
            }

            if (!int.TryParse(arguments.At(0), out int id))
            {
                response = "Failed parsing ID";
                return false;
            }

            if (!int.TryParse(arguments.At(1), out int textId))
            {
                response = "Failed parsing text ID";
                return false;
            }

            if (LedDisplayController.LedDisplays.TryGetValue(id, out LedDisplayController display))
            {
                var text = display.Items.FirstOrDefault(p => p.Id == textId);
                if (text == null)
                {
                    response = $"Text with id {textId} not exists in display {id}.";
                    return false;
                }
                display.Items.Remove(text);
                response = $"Removed text ({textId}) from display with id {id}.";
                return true;
            }

            response = $"Display with id {id} not exists!";
            return false;
        }
    }
}
