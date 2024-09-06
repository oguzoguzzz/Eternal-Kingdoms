using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

namespace DevelopersHub.RealtimeNetworking
{
    public class UI_Shop : MonoBehaviour
    {
        [SerializeField] public GameObject _elements = null;
        [SerializeField] private Button _closeButton = null;
        private static UI_Shop _instance = null; public static UI_Shop instance { get { return _instance;}}

        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }

        private Canvas _canvas;
        private EventTrigger _eventTrigger;

        private void Start()
        {
            _canvas = GetComponentInParent<Canvas>();
            _eventTrigger = _canvas.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });
            _eventTrigger.triggers.Add(entry);
        }
        
        private void OnPointerClick(PointerEventData eventData)
        {
            if (!IsPointerOverUIElement(eventData.position))
            {
                SetStatus(false);
            }
        }
        private bool IsPointerOverUIElement(Vector2 position)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = position;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject == _elements || result.gameObject.transform.IsChildOf(_elements.transform))
                {
                    return true;
                }
            }
            return false;
        }
        public void SetStatus(bool status)
        {
            _elements.SetActive(status);
        }

        private void CloseShop()
        {
            SetStatus(false);
        }

        private void OnDestroy()
        {
            if (_eventTrigger != null)
            {
                Destroy(_eventTrigger);
            }
        }
    }
}
