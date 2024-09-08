namespace DevelopersHub.RealtimeNetworking
{
    // Bu bölüm, gerekli kütüphaneleri ve namespace'ları tanımlar.
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    
    // Bu sınıf, kameranın kontrolünü sağlar.
    public class CameraController : MonoBehaviour
    {
        // Bu bölüm, kameranın static bir örneğini oluşturur ve erişimi sağlar.
        private static CameraController _instance = null; public static CameraController instance { get { return _instance; } }

        // Bu bölüm, kameranın hareket ve zoom hızlarını ayarlamak için kullanılan değişkenleri tanımlar.
        [SerializeField] private Camera _camera = null;
        [SerializeField] private float _moveSpeed = 50;
        [SerializeField] private float _moveSmooth = 5;

        [SerializeField] private float _zoomSpeed = 10f;
        [SerializeField] private float _zoomSmooth = 5;

        // Bu bölüm, kameranın çeşitli ayarlarını ve durumlarını saklamak için kullanılan değişkenleri tanımlar.
        private Controls _inputs = null;

        private bool _zooming = false;
        private bool _moving = false;
        private Vector3 _center = Vector3.zero;
        private float _right = 10;
        private float _left = 10;
        private float _up = 10;
        private float _down = 10;
        private float _angle = 45;
        private float _zoom = 5;
        private float _zoomMax = 100;
        private float _zoomMin = 1;
        private Vector2 _zoomPositionOnScreen = Vector2.zero;
        private Vector3 _zoomPositionInWorld = Vector3.zero;
        private float _zoomBaseValue = 0;
        private float _zoomBaseDistance = 0;

        private Transform _root = null;
        private Transform _pivot = null;
        private Transform _target = null;

        private bool _building = false; public bool isPlacingBuilding { get { return _building; } set { _building = value; } }
        private Vector3 _buildBasePosition = Vector3.zero;
        private bool _movingBuilding = false;

        private bool _replacing = false; public bool isReplacingBuilding { get { return _replacing; } set { _replacing = value; } }
        private Vector3 _replaceBasePosition = Vector3.zero;
        private bool _replacingBuilding = false;

        // Bu yöntem, kameranın başlatılmasını sağlar ve gerekli game objelerini oluşturur.
        private void Awake()
        {
            _instance = this;
            _inputs = new Controls();
            _root = new GameObject("CameraHelper").transform;
            _pivot = new GameObject("CameraPivot").transform;
            _target = new GameObject("CameraTarget").transform;
            _camera.orthographic = true;
            _camera.nearClipPlane = 0;
        }
        // Bu yöntem, kameranın başlatılmasını sağlar ve varsayılan ayarlarını uygular.
        private void Start()
        {
            Initialize(Vector3.zero, 40, 40, 40, 40, 45, 10, 5, 20);
        }

        // Bu yöntem, kameranın ayarlarını uygular ve sınırlandırmalarını belirler.
        public void Initialize(Vector3 center, float right, float left, float up, float down, float angle, float zoom, float zoomMin, float zoomMax)
        {
            _center = center;
            _right = right;
            _left = left;
            _up = up;
            _down = down;
            _angle = angle;
            _zoom = zoom;
            _zoomMin = zoomMin;
            _zoomMax = zoomMax;

            _camera.orthographicSize = _zoom;

            _zooming = false;
            _moving = false;
            _pivot.SetParent(_root);
            _target.SetParent(_pivot);

            _root.position = _center;
            _root.localEulerAngles = Vector3.zero;

            _pivot.localPosition = Vector3.zero;
            _pivot.localEulerAngles = new Vector3(_angle, 0, 0);

            _target.localPosition = new Vector3(0, 0, -100);
            _target.localEulerAngles = Vector3.zero;
        }

        // Bu yöntemler, kameranın etkinleştirilmesi ve devre dışı bırakılması durumunda çalışır.
        private void OnEnable()
        {
            _inputs.Enable();
            _inputs.Main.Move.started += _ => MoveStarted();
            _inputs.Main.Move.canceled += _ => MoveCanceled();
            _inputs.Main.TouchZoom.started += _ => ZoomStarted();
            _inputs.Main.PointerClick.performed += _ => ScreenClicked();
        }
        // Bu yöntemler, kameranın etkinleştirilmesi ve devre dışı bırakılması durumunda çalışır.
        private void OnDisable()
        {
            _inputs.Main.Move.started -= _ => MoveStarted();
            _inputs.Main.Move.canceled -= _ => MoveCanceled();
            _inputs.Main.TouchZoom.started -= _ => ZoomStarted();
            _inputs.Main.TouchZoom.canceled -= _ => ZoomCanceled();
            _inputs.Main.PointerClick.performed -= _ => ScreenClicked();

            _inputs.Disable();
        }
        // Bu yöntem, ekranın tıklandığında çalışır ve kameranın durumunu günceller.
        private void ScreenClicked()
        {
            Vector2 position = _inputs.Main.PointerPosition.ReadValue<Vector2>();
            if(IsScreenPointOverUI(position) == false)
            {
                bool found = false;
                Vector3 planePosition= CameraScreenPositionToPlanePosition(position);
                for (int i = 0; i < UI_Main.instance._grid.buildings.Count; i++)
                {
                    if (UI_Main.instance._grid.IsWorldPositionIsOnPlane(planePosition, UI_Main.instance._grid.buildings[i].currentX, UI_Main.instance._grid.buildings[i].currentY, UI_Main.instance._grid.buildings[i].rows, UI_Main.instance._grid.buildings[i].columns))
                    {
                        found = true;
                        UI_Main.instance._grid.buildings[i].Selected();
                        break;
                    }
                }
                if (!found)
                {
                    if (Building.selectedInstance != null)
                    {
                        Building.selectedInstance.Deselected();
                    }
                }
            }
            else
            {
                if (Building.selectedInstance != null)
                {
                    Building.selectedInstance.Deselected();
                }
            }
        }
        // Bu yöntem, ekran noktasının UI elemanları üzerinde olup olmadığını kontrol eder.
        public bool IsScreenPointOverUI(Vector2 position)
        {
            PointerEventData data = new PointerEventData(EventSystem.current);
            data.position = position;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data,results);
            return results.Count > 0;
        }
        // Bu yöntemler, kameranın hareketinin başlatılması ve iptal edilmesi durumunda çalışır.
        private void MoveStarted()
        {
            if (UI_Main.instance.isActive)
            {
                if (_building)
                {
                    _buildBasePosition = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
                    if (UI_Main.instance._grid.IsWorldPositionIsOnPlane(_buildBasePosition, Building.buildInstance.currentX, Building.buildInstance.currentY, Building.buildInstance.rows, Building.buildInstance.columns))
                    {
                        Building.buildInstance.StartMovingOnGrid();
                        _movingBuilding = true;
                    }
                }
                
                if (Building.selectedInstance != null)
                {
                    _replaceBasePosition = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
                    if (UI_Main.instance._grid.IsWorldPositionIsOnPlane(_replaceBasePosition, Building.selectedInstance.currentX, Building.selectedInstance.currentY, Building.selectedInstance.rows, Building.selectedInstance.columns))
                    {
                        if (!_replacing)
                        {
                            _replacing = true;
                        }
                        Building.selectedInstance.StartMovingOnGrid();
                        _replacingBuilding = true;
                    }
                }

                if(_movingBuilding == false && _replacingBuilding == false)
                {
                    _moving = true;
                }
            }
        }
        // Bu yöntemler, kameranın hareketinin başlatılması ve iptal edilmesi durumunda çalışır.
        private void MoveCanceled()
        {
            _moving = false;
            _movingBuilding = false;
            if (_replacingBuilding)
            {
                _replacingBuilding = false;
                if (Building.selectedInstance)
                {
                    Building.selectedInstance.SaveLocation(false);
                }
            }
        }
        // Bu yöntemler, kameranın zoomunun başlatılması ve iptal edilmesi durumunda çalışır.
        private void ZoomStarted()
        {
            if (UI_Main.instance.isActive)
            {
                Vector2 touch0 = _inputs.Main.TouchPosition0.ReadValue<Vector2>();
                Vector2 touch1 = _inputs.Main.TouchPosition1.ReadValue<Vector2>();
                _zoomPositionOnScreen = Vector2.Lerp(touch0, touch1, 0.5f);
                _zoomPositionInWorld = CameraScreenPositionToPlanePosition(_zoomPositionOnScreen);
                _zoomBaseValue = _zoom;

                touch0.x /= Screen.width;
                touch1.x /= Screen.width;
                touch0.y /= Screen.height;
                touch1.y /= Screen.height;

                _zoomBaseDistance = Vector2.Distance(touch0, touch1);
                _zooming = true;
            }
        }
        // Bu yöntemler, kameranın zoomunun başlatılması ve iptal edilmesi durumunda çalışır.
        private void ZoomCanceled()
        {
            _zooming = false;
        }
        // Bu yöntem, kameranın durumunu güncellemek için kullanılır.
        private void Update()
        {

            if (Input.touchSupported == false)
            {
                float mouseScroll = _inputs.Main.MouseScroll.ReadValue<float>();
                if(mouseScroll > 0)
                {
                    _zoom -= 3f * Time.deltaTime;
                }
                else if (mouseScroll < 0)
                {
                    _zoom += 3f * Time.deltaTime;
                }
            }

            if (_zooming)
            {
                Vector2 touch0 = _inputs.Main.TouchPosition0.ReadValue<Vector2>();
                Vector2 touch1 = _inputs.Main.TouchPosition1.ReadValue<Vector2>();

                touch0.x /= Screen.width;
                touch1.x /= Screen.width;
                touch0.y /= Screen.height;
                touch1.y /= Screen.height;

                float currentDistance = Vector2.Distance(touch0, touch1);
                float deltaDistance = currentDistance - _zoomBaseDistance;
                _zoom = _zoomBaseValue - (deltaDistance * _zoomSpeed);

                Vector3 zoomCenter = CameraScreenPositionToPlanePosition(_zoomPositionOnScreen);
                _root.position += (_zoomPositionInWorld - zoomCenter);
            }
            else if (_moving)
            {
                Vector2 move = _inputs.Main.MoveDelta.ReadValue<Vector2>();
                if(move != Vector2.zero)
                {
                    move.x /= Screen.width;
                    move.y /= Screen.height;
                    _root.position -= _root.right.normalized * move.x * _moveSpeed * _zoom / _zoomMax;
                    _root.position -= _root.forward.normalized * move.y * _moveSpeed * _zoom / _zoomMax;
                }
            }
            
            AdjustBounds();

            if (_camera.orthographicSize != _zoom)
            {
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _zoom, _zoomSmooth * Time.deltaTime);
            }
            if (_camera.transform.position != _target.position)
            {
                Vector3 velocity = Vector3.zero;
                _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, _target.position, ref velocity, _moveSmooth * Time.deltaTime);
            }
            if (_camera.transform.rotation != _target.rotation)
            {
                _camera.transform.rotation = _target.rotation;
            }

            if (_building && _movingBuilding)
            {
                Vector3 pos = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
                Building.buildInstance.UpdateGridPosition(_buildBasePosition, pos);
            }
            if (_replacing && _replacingBuilding)
            {
                Vector3 pos = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
                Building.selectedInstance.UpdateGridPosition(_replaceBasePosition, pos);
            }
        }
        // Bu yöntem, kameranın sınırlandırmalarını güncellemek için kullanılır.
        private void AdjustBounds()
        {
            if(_zoom < _zoomMin)
            {
                _zoom = _zoomMin;
            }
            if(_zoom > _zoomMax)
            {
                _zoom = _zoomMax;
            }

            float h = PlaneOrtographicSize();
            float w = h * _camera.aspect;

            if(h > (_up + _down) / 2f)
            {
                float n = (_up + _down) / 2f;
                _zoom = n * Mathf.Sin(_angle * Mathf.Deg2Rad);
            }

            if (w > (_right + _left) / 2f)
            {
                float n = (_right + _left) / 2f;
                _zoom = n * Mathf.Sin(_angle * Mathf.Deg2Rad) / _camera.aspect;
            }

            h = PlaneOrtographicSize();
            w = h * _camera.aspect;

            Vector3 tr = _root.position + _root.right.normalized * w + _root.forward.normalized * h;
            Vector3 tl = _root.position - _root.right.normalized * w + _root.forward.normalized * h;
            Vector3 dr = _root.position + _root.right.normalized * w - _root.forward.normalized * h;
            Vector3 dl = _root.position - _root.right.normalized * w - _root.forward.normalized * h;

            if(tr.x > _center.x + _right)
            {
                _root.position += Vector3.left * Mathf.Abs(tr.x - (_center.x + _right));
            }
            if (tl.x < _center.x - _left)
            {
                _root.position += Vector3.right * Mathf.Abs((_center.x - _left) - tl.x);
            }

            if (tr.z > _center.z + _up)
            {
                _root.position += Vector3.back * Mathf.Abs(tr.z - (_center.z + _up));
            }
            if (dl.z < _center.z - _down)
            {
                _root.position += Vector3.forward * Mathf.Abs((_center.z - _down) - dl.z);
            }
        }
        // Bu metod, kameranın ortografik boyutunu hesaplar.
        private float PlaneOrtographicSize()
        {
            float h = _zoom * 2f;
            return h / Mathf.Sin(_angle * Mathf.Deg2Rad) / 2f;
        }
        // Bu metod, ekran koordinatlarını dünya koordinatlarına dönüştürür.
        private Vector3 CameraScreenPositionToWorldPosition(Vector2 position)
        {
            float h = _camera.orthographicSize * 2f;
            float w = _camera.aspect * h;
            Vector3 ancher = _camera.transform.position - (_camera.transform.right.normalized * w / 2f) - (_camera.transform.up.normalized * h / 2f);
            return ancher + (_camera.transform.right.normalized * position.x / Screen.width * w) + (_camera.transform.up.normalized * position.y / Screen.height * h);
        }
        // Bu metod, ekran koordinatlarını düzlem koordinatlarına dönüştürür.
        public Vector3 CameraScreenPositionToPlanePosition(Vector2 position)
        {
            Vector3 point = CameraScreenPositionToWorldPosition(position);
            float h = point.y - _root.position.y;
            float x = h / Mathf.Sin(_angle * Mathf.Deg2Rad);
            return point + _camera.transform.forward.normalized * x;
        }

    }
}