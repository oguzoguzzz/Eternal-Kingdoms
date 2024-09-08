namespace DevelopersHub.RealtimeNetworking
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using UnityEngine.UI;

    // oyunun ana UI yönetimini sağlar.
    public class UI_Main : MonoBehaviour
    {
        [SerializeField] public GameObject _elements = null; // UI öğelerini tutar.
        [SerializeField] public TextMeshProUGUI _goldText = null; // Oyunun kaynaklarını gösteren metin öğelerini tutar.
        [SerializeField] public TextMeshProUGUI _woodText = null; // Oyunun kaynaklarını gösteren metin öğelerini tutar.
        [SerializeField] public TextMeshProUGUI _foodText = null; // Oyunun kaynaklarını gösteren metin öğelerini tutar.
        [SerializeField] public TextMeshProUGUI _stoneText = null; // Oyunun kaynaklarını gösteren metin öğelerini tutar.
        [SerializeField] public TextMeshProUGUI _gemsText = null; // Oyunun kaynaklarını gösteren metin öğelerini tutar.
        [SerializeField] private Button _shopButton = null; // Dükkan butonunu tutar.

        [SerializeField] public BuildGrid _grid = null; // İnşaat alanını tutar.
        [SerializeField] public Building[] _buildingPrefabs = null; // Bina prefab'larını tutar.

        // Sınıfın tek bir örneğini tutar.
        private static UI_Main _instance = null; public static UI_Main instance { get { return _instance;}}
        //  UI'nin aktif olup olmadığını tutar
        private bool _active = true;public bool isActive { get{ return _active;}}

        // Oyun başladığında çağrılır. Sınıfın tek bir örneğini oluşturur ve UI öğelerini gösterir.
        private void Awake()
        {
            _instance = this;
            _elements.SetActive(true);
        }

        //  Oyun başladığında çağrılır. Dükkan butonunun tıklandığında çağrılacak fonksiyonu ayarlar.
        private void Start()
        {
            _shopButton.onClick.AddListener(ShopButtonClicked);
        }
        //  Dükkan butonunun tıklandığında çağrılır. UI_Build sınıfının Cancel metodunu çağırır, 
        //UI_Shop sınıfının SetStatus metodunu çağırır ve UI_Main sınıfının SetStatus metodunu çağırır.
        private void ShopButtonClicked()
        {
            UI_Build.instance.Cancel();
            UI_Shop.instance.SetStatus(true);
            SetStatus(false);
        }
        // UI'nin durumunu ayarlar. UI öğelerini gösterir veya gizler.
        public void SetStatus(bool status)
        {
            _active = status;
            _elements.SetActive(status);
        }
        // İstenilen ID'ye göre bina prefab'ını döndürür.
        // Bu, bina prefab'larını ID'lerine göre aramak için kullanılır.
        public Building GetBuildingPrefab(string id)
        {
            for (int i = 0; i < _buildingPrefabs.Length; i++)
            {
                if(_buildingPrefabs[i].id == id)
                {
                    return _buildingPrefabs[i];
                }
            }
            return null;
        }
    }
}