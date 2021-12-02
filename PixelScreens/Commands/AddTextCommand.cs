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
    public class AddTextCommand : ICommand
    {
        public string Command => "addtext";

        public string[] Aliases => new string[] { "addt" };

        public string Description => "Add text to led display.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);

            if (!player.CheckPermission("pixelscreens.addtext"))
            {
                response = "Missing permission: pixelscreens.addtext!";
                return false;
            }

            if (arguments.Count < 6)
            {
                response = "Syntax: addtext <id> <posX> <posY> <color> <direction(left/up/down/right)> <speed(fast/idle/slow)> <text>";
                return false;
            }

            if (!int.TryParse(arguments.At(0), out int id))
            {
                response = "Failed parsing ID";
                return false;
            }

            if (!int.TryParse(arguments.At(1), out int posX))
            {
                response = "Failed parsing position X";
                return false;
            }

            if (!int.TryParse(arguments.At(2), out int posY))
            {
                response = "Failed parsing position Y";
                return false;
            }


            if (!ColorUtility.TryParseHtmlString(arguments.At(3), out Color color))
            {
                response = "Failed parsing color";
                return false;
            }

            if (!Enum.TryParse<ItemDirection>(arguments.At(4), true, out ItemDirection direction))
            {
                response = "Failed parsing direction";
                return false;
            }

            if (!Enum.TryParse<ItemSpeed>(arguments.At(5), true, out ItemSpeed speed))
            {
                response = "Failed parsing speed";
                return false;
            }

            string text = string.Join(" ", arguments.Skip(6));

            if (LedDisplayController.LedDisplays.TryGetValue(id, out LedDisplayController display))
            {
                var id2 = display.AddTextItem(text, new Vector2Int(posX, posY), color, direction, speed);
                response = $"Add text ({id2}) to display with id {id}.";
                return true;
            }

            response = $"Display with id {id} not exists!";
            return false;
        }
    }
}
