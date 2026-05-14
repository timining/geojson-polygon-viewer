using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace GeoJSONViewer
{
    [RequireComponent(typeof(Camera))]
    public class OrbitCamera : MonoBehaviour
    {
        private Camera _camera;
        private Vector3 _target = Vector3.zero;
        private float _distance = 100f;
        private float _yaw = 0f;
        private float _pitch = 30f;

        private Vector2 _lastMousePos;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {
            bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            if (!overUI)
            {
                HandleOrbit();
                HandlePan();
                HandleZoom();
            }
            HandleFit();
            ApplyTransform();
        }

        public void FitToView(Bounds bounds)
        {
            if (bounds.size == Vector3.zero)
                return;

            _target = bounds.center;

            Vector3 ext = bounds.extents;
            float size = new Vector2(ext.x, ext.z).magnitude;
            if (size < 0.001f) size = ext.magnitude;

            _distance = size * 2.5f;
            _pitch = 45f;
            _yaw = 0f;

            _camera.nearClipPlane = _distance * 0.001f;
            _camera.farClipPlane = _distance * 10f;

            ApplyTransform();
        }

        private void HandleOrbit()
        {
            if (!Mouse.current.leftButton.isPressed)
            {
                return;
            }

            Vector2 delta = Mouse.current.position.ReadValue() - _lastMousePos;
            _yaw += delta.x * 0.3f;
            _pitch -= delta.y * 0.3f;
            _pitch = Mathf.Clamp(_pitch, -89f, 89f);
        }

        private void HandlePan()
        {
            if (!Mouse.current.rightButton.isPressed && !Mouse.current.middleButton.isPressed)
            {
                return;
            }

            Vector2 delta = Mouse.current.position.ReadValue() - _lastMousePos;
            float speed = _distance * 0.001f;
            _target -= transform.right * delta.x * speed;
            _target -= transform.up * delta.y * speed;
        }

        private void HandleZoom()
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (scroll == 0f)
            {
                return;
            }

            _distance *= 1f - scroll * 0.1f;
            _distance = Mathf.Max(0.1f, _distance);
        }

        private void HandleFit()
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                GameObject root = GameObject.Find("FeatureCollection");
                if (root != null)
                {
                    Bounds b = CalculateBounds(root);
                    if (b.size != Vector3.zero)
                        FitToView(b);
                }
            }
        }

        private void ApplyTransform()
        {
            _lastMousePos = Mouse.current.position.ReadValue();

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            transform.position = _target + rotation * new Vector3(0f, 0f, -_distance);
            transform.LookAt(_target);
        }

        public static Bounds CalculateBounds(GameObject root)
        {
            bool initialized = false;
            Bounds bounds = new();

            foreach (LineRenderer lr in root.GetComponentsInChildren<LineRenderer>())
            {
                for (int i = 0; i < lr.positionCount; i++)
                {
                    Vector3 pos = lr.useWorldSpace
                        ? lr.GetPosition(i)
                        : lr.transform.TransformPoint(lr.GetPosition(i));

                    if (!initialized) { bounds = new Bounds(pos, Vector3.zero); initialized = true; }
                    else bounds.Encapsulate(pos);
                }
            }

            foreach (MeshRenderer mr in root.GetComponentsInChildren<MeshRenderer>())
            {
                if (!initialized) { bounds = mr.bounds; initialized = true; }
                else bounds.Encapsulate(mr.bounds);
            }

            if (!initialized)
                return new Bounds(root.transform.position, Vector3.zero);

            return bounds;
        }
    }
}
