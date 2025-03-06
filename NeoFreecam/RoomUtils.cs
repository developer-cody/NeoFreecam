using GorillaNetworking;
using Photon.Pun;
using System.Threading.Tasks;

namespace NeoFreecam
{
    public static class RoomUtils
    {
        public static void JoinRoomWithCode(string code) => TryJoinRoom(code);
        public static void Disconnect() => NetworkSystem.Instance.ReturnToSinglePlayer();

        public static async Task Generate(string code)
        {
            if (PhotonNetwork.InRoom)
            {
                await DisconnectAndWait();
            }

            TryJoinRoom(code);
        }

        public static async Task Rejoin(string code)
        {
            await Task.Delay(3000);
            TryJoinRoom(code);
        }

        private static void TryJoinRoom(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(code, JoinType.Solo);
            }
        }

        private static async Task DisconnectAndWait()
        {
            await NetworkSystem.Instance.ReturnToSinglePlayer();
            await Task.Delay(1000);
        }
    }
}