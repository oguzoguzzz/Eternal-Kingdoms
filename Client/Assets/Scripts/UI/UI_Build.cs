using System.Collections;
using System.Collections.Generic;
using DevelopersHub.RealtimeNetworking;
using UnityEngine;
using UnityEngine.UI;
using DevelopersHub.RealtimeNetworking.Client;

namespace DevelopersHub.RealtimeNetworking
{
public class UI_Build : MonoBehaviour
    {
        [SerializeField] public GameObject _elements = null; // Bina UI öğelerini tutar.
        public RectTransform buttonConfirm = null; // Onay butonunu tutar.
        public RectTransform buttonCancel = null; // İptal butonunu tutar.
        [HideInInspector] public Button clickConfirmButton = null;  // Onay butonunun tıklandığında çağrılacak fonksiyonu tutar.
        private static UI_Build _instance = null; public static UI_Build instance { get { return _instance;}} // Sınıfın tek bir örneğini tutar.

        private void Awake() // Oyun başladığında çağrılır. Sınıfın tek bir örneğini oluşturur ve bina UI öğelerini gizler.
        {
            _instance = this;
            _elements.SetActive(false);
            clickConfirmButton = buttonConfirm.gameObject.GetComponent<Button>();
        }
        private void Start() // Oyun başladığında çağrılır. Onay ve iptal butonlarının tıklandığında çağrılacak fonksiyonları ayarlar.
        {
            buttonConfirm.gameObject.GetComponent<Button>().onClick.AddListener(Confirm);
            buttonCancel.gameObject.GetComponent<Button>().onClick.AddListener(Cancel);
            buttonConfirm.anchorMin = Vector3.zero;
            buttonConfirm.anchorMax = Vector3.zero;
            buttonCancel.anchorMin = Vector3.zero;
            buttonCancel.anchorMax = Vector3.zero;
        }
        private void Update() // Her frame'de çağrılır. Bina UI öğelerini günceller.
        {
            if(Building.buildInstance != null && CameraController.instance.isPlacingBuilding)
            {
                Vector3 end = UI_Main.instance._grid.GetEndPosition(Building.buildInstance);

                Vector3 planDownLeft = CameraController.instance.CameraScreenPositionToPlanePosition(Vector2.zero);
                Vector3 planTopRight = CameraController.instance.CameraScreenPositionToPlanePosition(new Vector2(Screen.width, Screen.height));

                float w = planTopRight.x - planDownLeft.x;
                float h = planTopRight.z - planDownLeft.z;

                float endW = end.x - planDownLeft.x;
                float endH = end.z - planDownLeft.z;

                Vector2 screenPoint = new Vector2(endW / w * Screen.width, endH / h * Screen.height);

                Vector2 confirmPoint = screenPoint;
                confirmPoint.x += (buttonConfirm.rect.width + 10f);
                buttonConfirm.anchoredPosition = confirmPoint;

                Vector2 cancelPoint = screenPoint;
                cancelPoint.x -= (buttonCancel.rect.width + 10f);
                buttonCancel.anchoredPosition = cancelPoint;
            }
        }
        public void SetStatus(bool status) // Bina UI öğelerinin durumunu ayarlar.
        {
            _elements.SetActive(status);
        }
        private void Confirm() // Onay butonunun tıklandığında çağrılır. Bina prefabını yerleştirir ve sunucuya bilgi gönderir.
        {
            if (Building.buildInstance != null && UI_Main.instance._grid.CanPlaceBuilding(Building.buildInstance, Building.buildInstance.currentX, Building.buildInstance.currentY))
            //  Bina prefabının yerleştirilmesi için gerekli kontrolleri yapar.
            {
                Packet packet = new Packet(); // Sunucuya bilgi göndermek için bir paket oluşturur.
                packet.Write((int)Player.RequestID.BUILD);
                packet.Write(SystemInfo.deviceUniqueIdentifier);
                packet.Write(Building.buildInstance.id);
                packet.Write(Building.buildInstance.currentX);
                packet.Write(Building.buildInstance.currentY);
                Sender.TCP_Send(packet); // Sunucuya paketi gönderir.
                Cancel();
            }
        }
        public void Cancel() // İptal butonunun tıklandığında çağrılır. Bina prefabını kaldırır.
        {
            if(Building.buildInstance != null)
            {
                CameraController.instance.isPlacingBuilding = false; // Kamera kontrolörünün yerleştirme modunu tutar.
                Building.buildInstance.RemovedFromGrid();
            }
        }
    }
}
