using GorillaNetworking;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SkibidiFreecam.Patches
{
    public static class RoomPatches
    {
        public static void JoinRoomWithCode(string code)
            => PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(code, JoinType.Solo);

        public static void Disconnect()
            => NetworkSystem.Instance.ReturnToSinglePlayer();

        public static async Task Generate(string code)
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(code, GorillaNetworking.JoinType.Solo);
            }

            if (NetworkSystem.Instance.InRoom)
            {
                await NetworkSystem.Instance.ReturnToSinglePlayer();
            }

            await Task.Delay(1000);
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(code, GorillaNetworking.JoinType.Solo);
        }

        public static async Task Rejoin(string code)
        {
            await Task.Delay(3000);
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(code, GorillaNetworking.JoinType.Solo);
        }
    }
}
