using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevelopersHub
{
    public class UI_BuildingOptions : MonoBehaviour
    {
        [SerializeField] public GameObject _elements = null;
        private static UI_BuildingOptions _instance = null; public static UI_BuildingOptions instance { get { return _instance;}}

        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }
        public void SetStatus(bool status)
        {
            _elements.SetActive(status);
        }
    }
}
