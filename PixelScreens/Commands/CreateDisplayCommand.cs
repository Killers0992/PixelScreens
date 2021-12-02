using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using LedDisplay;
using LedDisplay.Enums;
using System;
using UnityEngine;

namespace PixelScreens.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CreateDisplayCommand : ICommand
    {
        public string Command => "createdisplay";

        public string[] Aliases => new string[] { "created" };

        public string Description => "Create led display.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);

            if (!player.CheckPermission("pixelscreens.createdisplay"))
            {
                response = "Missing permission: pixelscreens.createdisplay!";
                return false;
            }

            if (arguments.Count < 9)
            {
                response = "Syntax: createdisplay <id> <posX> <posY> <posZ> <sizeX> <sizeY> <displayType(plane/sphere)> <pixelSize> <spacing> <fontPath>";
                return false;
            }

            if (!int.TryParse(arguments.At(0), out int id))               
            {
                response = "Failed parsing ID";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float posX))
            {
                response = "Failed parsing position X";
                return false;
            }

            if (!float.TryParse(arguments.At(2), out float posY))
            {
                response = "Failed parsing position Y";
                return false;
            }

            if (!float.TryParse(arguments.At(3), out float posZ))
            {
                response = "Failed parsing position Z";
                return false;
            }

            if (!int.TryParse(arguments.At(4), out int sizeX))
            {
                response = "Failed parsing size X";
                return false;
            }

            if (!int.TryParse(arguments.At(5), out int sizeY))
            {
                response = "Failed parsing size Y";
                return false;
            }

            if (!Enum.TryParse<DisplayObjectType>(arguments.At(6), true, out DisplayObjectType displayType))
            {
                response = "Failed parsing display type";
                return false;
            }

            if (!float.TryParse(arguments.At(7), out float pixelSize))
            {
                response = "Failed parsing pizel size";
                return false;
            }

            if (!float.TryParse(arguments.At(8), out float spacing))
            {
                response = "Failed parsing spacing";
                return false;
            }

            var newDisplay = LedDisplayController.CreateDisplay(
                id, 
                new Vector3(posX, posY, posZ),
                new Vector3(90f, 0f, 0f),
                sizeX, 
                sizeY, 
                displayType, 
                pixelSize, 
                spacing, 
                null);
            newDisplay.StartMove(0.1f);
            response = $"Created display with id {id}.";
            return true;
        }
    }
}
