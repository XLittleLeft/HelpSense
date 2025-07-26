using MapGeneration;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HelpSense.Handler
{
    public static class LobbyLocationHandler
    {
        public static Vector3 LobbyPosition;
        public static Quaternion LobbyRotation;

        public static void TowerLocation()
        {
            LobbyPosition = Random.Range(1, 6) switch
            {
                1 => new Vector3(162.893f, 319.470f, -13.430f),
                2 => new Vector3(107.698f, 314.048f, -12.555f),
                3 => new Vector3(39.262f, 314.112f, -31.844f),
                4 => new Vector3(-15.854f, 314.461f, -31.543f),
                5 => new Vector3(130.483f, 293.366f, 20.601f),
                _ => new Vector3(39.262f, 314.112f, -31.844f),
            };
        }

        public static void IntercomLocation()
        {
            var intercomRoom = RoomIdentifier.AllRoomIdentifiers.FirstOrDefault(x => x.Name is RoomName.EzIntercom);

            if (intercomRoom == null)
                return;

            LobbyPosition = intercomRoom.transform.TransformPoint(new Vector3(-4.16f, -3.860f, -2.113f));
            LobbyRotation = Quaternion.Euler(intercomRoom.transform.rotation.eulerAngles.x, intercomRoom.transform.rotation.eulerAngles.y + 180, intercomRoom.transform.rotation.eulerAngles.z);
        }

        public static void MountainLocation()
        {
            LobbyPosition = new Vector3(103.492f, 298.946f, 24.672f);
        }

        public static void ChaosLocation()
        {
            LobbyPosition = new Vector3(-49.074f, 289.055f, -42.844f);
            //LobbyPosition = new Vector3(-7.500f, 295.402f, -7.910f);
        }
    }
}
