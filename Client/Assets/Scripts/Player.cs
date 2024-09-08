namespace DevelopersHub.RealtimeNetworking
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using DevelopersHub.RealtimeNetworking.Client;

    // Bu sınıf, oyuncunun bilgilerini yönetmek için kullanılır.
    public class Player : MonoBehaviour
    {
        // RequestID enum'u, sunucuya gönderilecek isteklerin türünü belirlemek için kullanılır. Şu anda dört tür istek vardır:
        public enum RequestID
        {
            AUTH = 1, SYNC = 2, BUILD = 3, REPLACE = 4
        }
        private void Start()
        {
            // Start metodu, oyun başladıktan sonra çalışır. 
            // RealtimeNetworking.OnPacketReceived olayına abone olur ve ConnectToServer metodunu çağırır.
            RealtimeNetworking.OnPacketReceived += ReceivedPacket;
            ConnectToServer();
        }
        // sunucudan gelen paketleri işler. Paketin içindeki istek türünü kontrol eder ve buna göre farklı işlemler gerçekleştirir.
        private void ReceivedPacket(Packet packet)
        {
            int id = packet.ReadInt();

            switch ((RequestID)id)
            {
                // hesap kimliğini alır ve SendSyncRequest metodunu çağırır.
                case RequestID.AUTH:
                    long accountID = packet.ReadLong();
                    SendSyncRequest();
                    break;
                // oyuncu verilerini alır ve SyncData metodunu çağırır.
                case RequestID.SYNC:
                    string playerData = packet.ReadString();
                    Data.Player playerSyncData = Data.Deserialize<Data.Player>(playerData);
                    SyncData(playerSyncData);
                    break;
                // bina inşa sonucu alır ve buna göre farklı işlemler gerçekleştirir.
                case RequestID.BUILD:
                    int response = packet.ReadInt();
                    switch (response)
                    {
                        case 0:
                            Debug.Log("No resources");
                            break;
                        case 1:
                            Debug.Log("Place Successfully");
                            SendSyncRequest();
                            break;
                        case 2:
                            Debug.Log("Place taken");
                            break;
                    }
                    break;
                    // bina yerine koyma sonucu alır ve buna göre farklı işlemler gerçekleştirir.
                    case RequestID.REPLACE:
                    int replaceResponse = packet.ReadInt();
                    int replaceX = packet.ReadInt();
                    int replaceY = packet.ReadInt();
                    long replaceID = packet.ReadLong();

                    for (int i = 0; i < UI_Main.instance._grid.buildings.Count; i++)
                    {
                        if (UI_Main.instance._grid.buildings[i].databaseID == replaceID)
                        {
                            switch (replaceResponse)
                            {
                                case 0:
                                    Debug.Log("No building");
                                    break;
                                case 1:
                                    Debug.Log("Replace Successfully");
                                    UI_Main.instance._grid.buildings[i].PlacedOnGrid(replaceX, replaceY);
                                    if (UI_Main.instance._grid.buildings[i] != Building.selectedInstance)
                                    {
                                        
                                    }
                                    break;
                                case 2:
                                    Debug.Log("Replace taken");
                                    break;
                            }
                            UI_Main.instance._grid.buildings[i].waitingReplaceResponse = false;
                            break;
                        }
                    }
                    break;
            }
        }
        // sunucuya veri senkronizasyonu isteği gönderir.
        public void SendSyncRequest()
        {
            Packet p = new Packet();
            p.Write((int)RequestID.SYNC);
            p.Write(SystemInfo.deviceUniqueIdentifier);
            Sender.TCP_Send(p);
        }
        // oyuncu verilerini güncellemek için kullanılır.
        // Oyuncunun altın, gıda, odun, taş ve mücevher miktarlarını güncellemek için kullanılır.
        // Ayrıca, oyuncunun sahip olduğu binaların listesini güncellemek için kullanılır.
        private void SyncData(Data.Player player)
        {
            UI_Main.instance._goldText.text = player.gold.ToString();
            UI_Main.instance._foodText.text = player.food.ToString();
            UI_Main.instance._woodText.text = player.wood.ToString();
            UI_Main.instance._stoneText.text = player.stone.ToString();
            UI_Main.instance._gemsText.text = player.gems.ToString();
            if (player.buildings != null && player.buildings.Count > 0)
            {
                for (int i = 0; i< player.buildings.Count; i ++)
                {
                    Building building = UI_Main.instance._grid.GetBuilding(player.buildings[i].databaseID);
                    if(building != null)
                    {

                    }
                    else
                    {
                        Building prefab = UI_Main.instance.GetBuildingPrefab(player.buildings[i].id);
                        if(prefab)
                        {
                            Building b = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                            b.databaseID = player.buildings[i].databaseID;
                            b.PlacedOnGrid(player.buildings[i].x, player.buildings[i].y);
                            b._baseArea.gameObject.SetActive(false);

                            UI_Main.instance._grid.buildings.Add(b);
                        }
                    }
                }
            }
        }
        // sunucuya bağlanma sonucu alır.
        // Bağlanma başarılı ise, DisconnectedFromServer olayına abone olur ve oturum açma isteği gönderir.
        // Bağlanma başarısız ise, bağlantı başarısız mesaj kutusu gösterilir.
        private void ConnectionResponse(bool successful)
        {
            if (successful)
            {
                RealtimeNetworking.OnDisconnectedFromServer += DisconnectedFromServer;
                string device = SystemInfo.deviceUniqueIdentifier;
                Packet packet = new Packet();
                packet.Write((int)RequestID.AUTH);
                packet.Write(device);
                Sender.TCP_Send(packet);
            }
            else
            {
                // TODO: Connection failed message box with retry button.
            }
            RealtimeNetworking.OnConnectingToServerResult -= ConnectionResponse;
        }
        // sunucuya bağlanmak için kullanılır.
        // Bağlanma sonucu ConnectionResponse metodunu çağırır.
        public void ConnectToServer()
        {
            RealtimeNetworking.OnConnectingToServerResult += ConnectionResponse;
            RealtimeNetworking.Connect();
        }
        // sunucudan ayrılma sonucu alır.
        // Bağlantı başarısız mesaj kutusu gösterilir.
        private void DisconnectedFromServer()
        {
            RealtimeNetworking.OnDisconnectedFromServer -= DisconnectedFromServer;
            // TODO: Connection failed message box with retry button.
        }
    }
}