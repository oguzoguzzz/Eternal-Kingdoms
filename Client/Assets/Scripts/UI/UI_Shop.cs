using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevelopersHub.RealtimeNetworking
{
    public class UI_Shop : MonoBehaviour
    {
        [SerializeField] public GameObject _elements = null; // Dükkan UI öğelerini tutar.
        [SerializeField] private Button _closeButton = null; // Dükkan kapatma butonunu tutar.
        // Sınıfın tek bir örneğini tutar.
        private static UI_Shop _instance = null; public static UI_Shop instance { get { return _instance;}}

        private void Awake()
        {
            // Oyun başladığında çağrılır. Sınıfın tek bir örneğini oluşturur ve dükkan UI öğelerini gizler.
            _instance = this;
            _elements.SetActive(false);
        }
        // Oyun başladığında çağrılır. Dükkan kapatma butonunun tıklandığında çağrılacak fonksiyonu ayarlar.
        private void Start()
        {
            _closeButton.onClick.AddListener(CloseShop);
        }
        // Dükkan UI öğelerinin durumunu ayarlar. Dükkan UI öğelerini gösterir veya gizler.
        public void SetStatus(bool status)
        {
            _elements.SetActive(status);
        }
        // Dükkan kapatma butonunun tıklandığında çağrılır. Dükkan UI öğelerini gizler ve ana UI'yi gösterir.
        // dükkan kapatma butonunun tıklandığında çağrılır. 
        // Dükkan UI öğelerini gizler ve ana UI'yi gösterir. Bu, dükkan UI'sini kapatır ve ana UI'yi açar.
        private void CloseShop()
        {
            SetStatus(false);
            UI_Main.instance.SetStatus(true);
        }
    }
}
