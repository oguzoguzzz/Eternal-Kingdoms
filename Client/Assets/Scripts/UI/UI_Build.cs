using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevelopersHub
{
public class UI_Build : MonoBehaviour
    {
        [SerializeField] public GameObject _elements = null;
        public RectTransform buttonConfirm = null;
        public RectTransform buttonCancel = null;
        private static UI_Build _instance = null; public static UI_Build instance { get { return _instance;}}

        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }
        private void Start()
        {
            buttonConfirm.gameObject.GetComponent<Button>().onClick.AddListener(Confirm);
            buttonCancel.gameObject.GetComponent<Button>().onClick.AddListener(Cancel);
        }
        private void Update()
        {

        }
        public void SetStatus(bool status)
        {
            _elements.SetActive(status);
        }
        private void Confirm()
        {

        }
        public void Cancel()
        {
            
        }
    }
}