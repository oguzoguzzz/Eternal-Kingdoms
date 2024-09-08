using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevelopersHub.RealtimeNetworking.Client;

namespace DevelopersHub.RealtimeNetworking
{
    // Bu sınıf, oyunun bina yönetimini sağlar.
    public class Building : MonoBehaviour
    {
        public string id = ""; // Binanın kimliğini tutar.

        // Statik değişkenler, binanın instance'ını tutar.
        private static Building _buildInstance = null; public static Building buildInstance { get { return _buildInstance; } set { _buildInstance = value; } }
        private static Building _selectedInstance = null; public static Building selectedInstance { get { return _selectedInstance; } set { _selectedInstance = value; } }
        
        //  Binanın seviyesini tanımlar. Seviye, ikon, mesh ve level numarasını tutar.
        [System.Serializable] public class Level
        {
            public int level = 1;
            public Sprite icon = null;
            public GameObject mesh = null;
        }
        //  İnşaat alanını tutar.
        private BuildGrid _grid = null;

        // Binanın veritabanı kimliğini tutar.
        [SerializeField] private long _databaseID = 0; public long databaseID { get { return _databaseID; } set {_databaseID = value;}}
        //  Binanın satır ve sütun sayısını tutar.
        [SerializeField] private int _rows = 1; public int rows { get { return _rows; } }
        [SerializeField] private int _columns = 1; public int columns { get { return _columns; } }

        // Binanın temel alanını tutar.
        [SerializeField] public MeshRenderer _baseArea = null;
        // Binanın seviyelerinin listesini tutar.
        [SerializeField] private Level[] _levels = null;

        // Binanın当前 koordinatlarını tutar.
        private int _currentX = 0; public int currentX { get { return _currentX; } }
        private int _currentY = 0; public int currentY { get { return _currentY; } }
        // Binanın önceki koordinatlarını tutar.
        private int _X = 0;
        private int _Y = 0;
        // Binanın orijinal koordinatlarını tutar.
        public int _originalX = 0;
        public int _originalY = 0;
        // Binanın inşaat alanına yerleştirildiğinde çağrılır. Koordinatlarını günceller ve pozisyonunu ayarlar.
        public void PlacedOnGrid(int x, int y)
        {
            _currentX = x;
            _currentY = y;
            _X = x;
            _Y = y;
            _originalX = x;
            _originalY = y;
            Vector3 position = UI_Main.instance._grid.GetCenterPosition(x, y, _rows, _columns);
            transform.position = position;
            SetBaseColor();
        }
        //  Binanın inşaat alanında hareket etmeye başladığında çağrılır. Koordinatlarını günceller.
        public void StartMovingOnGrid()
        {
            _X = _currentX;
            _Y = _currentY;
        }
        // Binanın inşaat alanından kaldırıldığında çağrılır. Instance'ını sıfırlar ve UI'yi günceller.
        public void RemovedFromGrid()
        {
            _buildInstance = null;
            UI_Build.instance.SetStatus(false);
            CameraController.instance.isPlacingBuilding = false;
            Destroy(gameObject);
        }
        // Binanın inşaat alanındaki pozisyonunu günceller. Koordinatlarını günceller ve pozisyonunu ayarlar.
        public void UpdateGridPosition(Vector3 basePosition, Vector3 currentPosition)
        {
            Vector3 dir = UI_Main.instance._grid.transform.TransformPoint(currentPosition) - UI_Main.instance._grid.transform.TransformPoint(basePosition);

            int xDis = Mathf.RoundToInt(dir.z / UI_Main.instance._grid.cellSize);
            int yDis = Mathf.RoundToInt(-dir.x / UI_Main.instance._grid.cellSize);

            _currentX = _X + xDis;
            _currentY = _Y + yDis;

            Vector3 position = UI_Main.instance._grid.GetCenterPosition(_currentX, _currentY, _rows, _columns);
            transform.position = position;

            if(_X != _currentX || _Y != _currentY)
            {
                _baseArea.gameObject.SetActive(true);
            }
            SetBaseColor();
        }
        //  Binanın temel alanının rengini günceller. 
        //İnşaat alanına yerleştirilebiliyorsa yeşil, yerleştirilemiyorsa kırmızı renkte gösterir.
        private void SetBaseColor()
        {
            if(UI_Main.instance._grid.CanPlaceBuilding(this, currentX, currentY))
            {
                UI_Build.instance.clickConfirmButton.interactable = true;
                _baseArea.sharedMaterial.color = Color.green;
            }
            else
            {
                UI_Build.instance.clickConfirmButton.interactable = false;
                _baseArea.sharedMaterial.color = Color.red;
            }
        }
        [HideInInspector]public bool waitingReplaceResponse = false;
        // Binanın seçildiğinde çağrılır. UI'yi günceller ve instance'ını günceller.
        public void Selected()
        {
            if (selectedInstance != null)
            {
                if (selectedInstance == this)
                {
                    return;
                }
                else
                {
                    selectedInstance.Deselected();
                }
            }
            if (waitingReplaceResponse)
            {
                return;
            }

            UI_BuildingOptions.instance.SetStatus(true);

            _originalX = currentX;
            _originalY = currentY;
            selectedInstance = this;
        }
        // Binanın seçimi kaldırıldığında çağrılır. UI'yi günceller ve instance'ını sıfırlar.
        public void Deselected()
        {
            UI_BuildingOptions.instance.SetStatus(false);
            CameraController.instance.isReplacingBuilding = false;
            if (_originalX != currentX || _originalY !=  currentY)
            {
                SaveLocation();
            }
            selectedInstance = null;
        }
        // Binanın pozisyonunu kaydeder. 
        //İnşaat alanına yerleştirilebiliyorsa pozisyonunu kaydeder, yerleştirilemiyorsa önceki pozisyonuna döner.
        public void SaveLocation(bool resetIfNot = true)
        {
            if (UI_Main.instance._grid.CanPlaceBuilding(this, _currentX, _currentY) && (_X != currentX || _Y !=  currentY) && !waitingReplaceResponse)
                {
                    waitingReplaceResponse = true;
                    Packet packet = new Packet();
                    packet.Write((int)Player.RequestID.REPLACE);
                    packet.Write(selectedInstance.databaseID);
                    packet.Write(selectedInstance.currentX);
                    packet.Write(selectedInstance.currentY);
                    Sender.TCP_Send(packet);
                    _baseArea.gameObject.SetActive(false);
                }
                else
                {
                    if (resetIfNot)
                    {
                        if (waitingReplaceResponse == false)
                        {
                            PlacedOnGrid(_originalX, _originalY);
                        }
                        _baseArea.gameObject.SetActive(false);
                    }
                }
        }
    }
}
