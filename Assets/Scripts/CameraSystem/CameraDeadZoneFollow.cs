using UnityEngine;
using Doofus.Core;

namespace Doofus.CameraSystem
{
    // Name: Shivakant Kurmi
    
    public class CameraDeadZoneFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offsetFromAnchor = new Vector3(0f, 51f, -60f);
        [SerializeField] private float pitchAngle = 50f;
        // Fraction of the way from screen center to the screen edge (0 = dead center,
        // 1 = edge of the viewport) the target may drift before the anchor starts
        // catching up.
        [SerializeField] private float deadZoneFraction = 0.4f;
        [SerializeField] private float smoothTime = 0.1f;

        private static readonly Vector3 InitialAnchor = Vector3.zero;

        private Camera _camera;
        private Vector3 _anchor;
        private Vector3 _velocity;

        public void SetTarget(Transform newTarget) => target = newTarget;

        private void Awake()
        {
            _camera = GetComponent<Camera>();

            _anchor = InitialAnchor;
            ApplyPose();
        }

        private void OnEnable()
        {
            GameEvents.OnGameReset += HandleReset;
        }

        private void OnDisable()
        {
            GameEvents.OnGameReset -= HandleReset;
        }

        private void HandleReset()
        {
            _anchor = InitialAnchor;
            _velocity = Vector3.zero;
            ApplyPose();
        }

        private void ApplyPose()
        {
            transform.position = _anchor + offsetFromAnchor;
            transform.rotation = Quaternion.Euler(pitchAngle, 0f, 0f);
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Camera cam = _camera ??= GetComponent<Camera>();
            if (cam == null) return;

            Vector3 viewportPos = cam.WorldToViewportPoint(target.position);

            // Behind the camera entirely (shouldn't normally happen given the dead zone
            // keeps pulling the anchor in, but treat it as maximally out of bounds rather
            // than silently doing nothing if it ever does).
            bool behindCamera = viewportPos.z <= 0f;
            Vector2 offsetFromCenter = new Vector2((viewportPos.x - 0.5f) * 2f, (viewportPos.y - 0.5f) * 2f);
            float screenDistance = offsetFromCenter.magnitude;

            if (behindCamera || screenDistance > deadZoneFraction)
            {
                Vector3 targetFlat = new Vector3(target.position.x, 0f, target.position.z);
                _anchor = Vector3.SmoothDamp(_anchor, targetFlat, ref _velocity, smoothTime);
                transform.position = _anchor + offsetFromAnchor;
            }
        }
    }
}
