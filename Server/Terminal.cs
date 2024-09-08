using System;
using System.Numerics;

namespace DevelopersHub.RealtimeNetworking.Server
{
    class Terminal // sunucunun temel işlemlerini sağlar.
    {

        #region Update // Sunucunun güncelleme metodudur. Her güncelleme periyodunda çağrılır.
        public const int updatesPerSecond = 30; // Sunucunun güncelleme hızını belirler.
        public static void Update()
        {
            
        }
        #endregion

        #region Connection
        public const int maxPlayers = 100000; // Sunucunun desteklediği maksimum oyuncu sayısını belirler.
        public const int port = 5555; // Sunucunun dinlediği port numarasını belirler.
        public static void OnClientConnected(int id, string ip) // Bir istemci bağlandığında çağrılır.
        {
            
        }

        public static void OnClientDisconnected(int id, string ip) //  Bir istemci bağlantısını kestiğinde çağrılır.
        {
            
        }
        #endregion

        #region Data
        public enum RequestID //  İstemciden gelen paketlerin türlerini belirler.
        {
            AUTH = 1, SYNC = 2, BUILD = 3, REPLACE = 4 // AUTH: İstemcinin kimlik doğrulama talebi.
            // SYNC: İstemcinin veri senkronizasyonu talebi.
            // BUILD: İstemcinin bina yerleştirme talebi.
            // REPLACE: İstemcinin bina değiştirme talebi.
        }
        public static void ReceivedPacket(int clientID, Packet packet) //  Bir istemciden paket aldığında çağrılır. 
        // Paketi işler ve gerekli işlemleri gerçekleştirir.
        {
            int id = packet.ReadInt();
            string device = "";
            switch ((RequestID)id)
            {
                case RequestID.AUTH:
                    device = packet.ReadString();
                    Database.AuthenticatePlayer(clientID, device);
                    break;
                case RequestID.SYNC:
                    device = packet.ReadString();
                    Database.SyncPlayerData(clientID, device);
                    break;
                case RequestID.BUILD:
                    device = packet.ReadString();
                    string building = packet.ReadString();
                    int x = packet.ReadInt();
                    int y = packet.ReadInt();
                    Database.PlaceBuilding(clientID, device, building, x, y);
                    break;
                case RequestID.REPLACE:
                    long databaseID = packet.ReadLong();
                    int replaceX = packet.ReadInt();
                    int replaceY = packet.ReadInt();
                    Database.ReplaceBuilding(clientID,databaseID,replaceX, replaceY);
                    break;
            }
        }

        public static void ReceivedBytes(int clientID, int packetID, byte[] data)
        {
            
        }

        public static void ReceivedString(int clientID, int packetID, string data)
        {
        }

        public static void ReceivedInteger(int clientID, int packetID, int data)
        {
        }

        public static void ReceivedFloat(int clientID, int packetID, float data)
        {

        }

        public static void ReceivedBoolean(int clientID, int packetID, bool data)
        {

        }

        public static void ReceivedVector3(int clientID, int packetID, Vector3 data)
        {

        }

        public static void ReceivedQuaternion(int clientID, int packetID, Quaternion data)
        {

        }

        public static void ReceivedLong(int clientID, int packetID, long data)
        {

        }

        public static void ReceivedShort(int clientID, int packetID, short data)
        {

        }

        public static void ReceivedByte(int clientID, int packetID, byte data)
        {

        }

        public static void ReceivedEvent(int clientID, int packetID)
        {

        }
        #endregion

    }
}