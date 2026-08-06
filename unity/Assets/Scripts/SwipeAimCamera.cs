using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// 上下スワイプでカメラのピッチだけを操作する。UI 上の操作は無視する。
/// </summary>
public class SwipeAimCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float minPitch = -25f;
    [SerializeField] private float maxPitch = 45f;
    [SerializeField] private float pitchSensitivity = 0.15f;
    [SerializeField] private bool invertPitch = true;
    [SerializeField] private float aimSmoothTime = 0.05f;

    private float _pitch;
    private float _baseYaw;
    private float _pitchVelocity;
    private float _targetPitch;
    private bool _isDragging;
    private bool _ignoreCurrentPointer;
    private float _lastPointerY;

    public float CurrentPitch => _pitch;
    public Transform CameraTransform => cameraTransform != null ? cameraTransform : transform;

    private void Awake()
    {
        if (cameraTransform == null)
        {
            cameraTransform = transform;
        }

        Vector3 euler = cameraTransform.localEulerAngles;
        _pitch = NormalizeAngle(euler.x);
        _baseYaw = NormalizeAngle(euler.y);
        _targetPitch = _pitch;
    }

    private void Update()
    {
        HandlePointerInput();
        ApplyPitch();
    }

    private void HandlePointerInput()
    {
        Pointer pointer = Pointer.current;
        if (pointer == null)
        {
            return;
        }

        float pointerY = pointer.position.ReadValue().y;

        if (pointer.press.wasPressedThisFrame)
        {
            _ignoreCurrentPointer = IsPointerOverUi();
            _isDragging = !_ignoreCurrentPointer;
            _lastPointerY = pointerY;
        }

        if (_isDragging && pointer.press.isPressed)
        {
            float deltaY = pointerY - _lastPointerY;
            _lastPointerY = pointerY;

            float pitchDelta = deltaY * pitchSensitivity * (invertPitch ? -1f : 1f);
            _targetPitch = Mathf.Clamp(_targetPitch + pitchDelta, minPitch, maxPitch);
        }

        if (pointer.press.wasReleasedThisFrame)
        {
            _isDragging = false;
            _ignoreCurrentPointer = false;
        }
    }

    private static bool IsPointerOverUi()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    private void ApplyPitch()
    {
        _pitch = Mathf.SmoothDampAngle(_pitch, _targetPitch, ref _pitchVelocity, aimSmoothTime);
        cameraTransform.localRotation = Quaternion.Euler(_pitch, _baseYaw, 0f);
    }

    public void ResetAim(float pitch = 0f)
    {
        _targetPitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        _pitch = _targetPitch;
        _pitchVelocity = 0f;
        cameraTransform.localRotation = Quaternion.Euler(_pitch, _baseYaw, 0f);
    }

    private static float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f)
        {
            angle -= 360f;
        }

        return angle;
    }
}
