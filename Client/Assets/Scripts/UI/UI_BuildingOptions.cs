using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevelopersHub
{
    public class UI_BuildingOptions : MonoBehaviour
    {
        [SerializeField] public GameObject _elements = null; // Bina seçenekleri UI öğelerini tutar.
        // Sınıfın tek bir örneğini tutar.
        private static UI_BuildingOptions _instance = null; public static UI_BuildingOptions instance { get { return _instance;}}
        // Oyun başladığında çağrılır. Sınıfın tek bir örneğini oluşturur ve bina seçenekleri UI öğelerini gizler.
        // Sınıfın tek bir örneğini oluşturur ve bina seçenekleri UI öğelerini gizler. 
        // Bu, bina seçenekleri UI'sini gizlemek için kullanılır.
        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }
        // Bina seçenekleri UI öğelerinin durumunu ayarlar. Bina seçenekleri UI öğelerini gösterir veya gizler.
        public void SetStatus(bool status)
        {
            _elements.SetActive(status);
        }
    }
}
