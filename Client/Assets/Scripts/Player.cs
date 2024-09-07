namespace DevelopersHub.RealtimeNetworking
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using DevelopersHub.RealtimeNetworking.Client;

    public class Player : MonoBehaviour
    {
        public enum RequestID
        {
            AUTH = 1, SYNC = 2, BUILD = 3
        }
        private void Start()
        {
            RealtimeNetworking.OnLongReceived += ReceivedLong;
            RealtimeNetworking.OnPacketReceived += ReceivedPacket;

            ConnectToServer();
        }
        private void ReceivedLong(int id, long value)
        {
            switch (id)
            {
            
                case 1:
                    Debug.Log(value);
                    Sender.TCP_Send((int)RequestID.SYNC, SystemInfo.deviceUniqueIdentifier);
                    break;
            }
        }

        private void ReceivedPacket(Packet packet)
        {
            int id = packet.ReadInt();
            switch (id)
            {
            
                case 2:
                    string playerClass = packet.ReadString();
                    Data.Player player = Data.Deserialize<Data.Player>(playerClass);
                    UI_Main.instance._goldText.text = player.gold.ToString();
                    UI_Main.instance._foodText.text = player.food.ToString();
                    UI_Main.instance._woodText.text = player.wood.ToString();
                    UI_Main.instance._stoneText.text = player.stone.ToString();
                    UI_Main.instance._gemsText.text = player.gems.ToString();
                    break;
                case 3:
                    int response = packet.ReadInt();
                    switch (response)
                    {
                        case 0:
                            Debug.Log("No resources");
                            break;
                        case 1:
                            Debug.Log("Place Successfully");
                            break;
                        case 2:
                            Debug.Log("Place taken");
                            break;
                    }
                    break;
            }
        }

        private void ConnecctionResponse(bool successful)
        {
            if (successful)
            {
                RealtimeNetworking.OnDisconnectedFromServer += DisconnectedFromServer;
                string device = SystemInfo.deviceUniqueIdentifier;
                Sender.TCP_Send((int)RequestID.AUTH, device);
            }
            else
            {
                // TODO: Connection failed message box with retry button.
            }
            RealtimeNetworking.OnConnectingToServerResult -= ConnecctionResponse;
        }

        public void ConnectToServer()
        {
            RealtimeNetworking.OnConnectingToServerResult += ConnecctionResponse;
            RealtimeNetworking.Connect();
        }
        private void DisconnectedFromServer()
        {
            RealtimeNetworking.OnDisconnectedFromServer -= DisconnectedFromServer;
            // TODO: Connection failed message box with retry button.
        }
    }
}