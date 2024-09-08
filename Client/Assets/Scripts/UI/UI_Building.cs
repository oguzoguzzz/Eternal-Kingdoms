namespace DevelopersHub.RealtimeNetworking
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using DevelopersHub.RealtimeNetworking.Client;

    //  oyunun bina UI yönetimini sağlar.
    public class UI_Building : MonoBehaviour
    {
        [SerializeField] private string _id = ""; // Bina kimliğini tutar.
        [SerializeField] private Button _button = null; // Bina butonunu tutar.
        private void Start() // Oyun başladığında çağrılır. Bina butonunun tıklandığında çağrılacak fonksiyonu ayarlar.
        {

            _button.onClick.AddListener(Clicked);
        }

        // Bu metod, aşağıdaki işlemleri gerçekleştirir:
        // Bina prefabını alır.
        // Dükkan UI'sini gizler ve ana UI'yi gösterir.
        // Bina prefabını oyun alanına yerleştirir.
        // Bina'nın temel alanını aktif eder.
        // Bina örneğini ayarlar.
        // Kamera kontrolörünü yerleştirme moduna geçirir.
        // Bina UI'sini gösterir.
        private void Clicked() // Bina butonunun tıklandığında çağrılır. Bina prefabını oluşturur ve oyun alanına yerleştirir.
        {
            Building prefab = UI_Main.instance.GetBuildingPrefab(_id);
            if(prefab)
            {
                UI_Shop.instance.SetStatus(false); // Dükkan UI'sini gizler.
                UI_Main.instance.SetStatus(true); // Ana UI'yi gösterir.

                Vector3 position = Vector3.zero;

                Building building = Instantiate(prefab, position, Quaternion.identity); 

                building.PlacedOnGrid(20, 20);
                building._baseArea.gameObject.SetActive(true);

                Building.buildInstance = building; // Bina örneğini ayarlar.
                CameraController.instance.isPlacingBuilding = true; //  Kamera kontrolörünü yerleştirme moduna geçirir.

                UI_Build.instance.SetStatus(true); // Bina UI'sini gösterir.
            }
        }
    }
}
