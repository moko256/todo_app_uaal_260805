using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// スマートフォンの上下スワイプでカメラのピッチ（必要ならヨー）を操作する。
/// </summary>
public class SwipeAimCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Aim Limits")]
    [SerializeField] private float minPitch = -25f;
    [SerializeField] private float maxPitch = 45f;
    [SerializeField] private float minYaw = -40f;
    [SerializeField] private float maxYaw = 40f;
    [SerializeField] private bool allowHorizontalAim = true;

    [Header("Sensitivity")]
    [Tooltip("画面縦方向のピクセル移動に対するピッチ変化量")]
    [SerializeField] private float pitchSensitivity = 0.15f;
    [Tooltip("画面横方向のピクセル移動に対するヨー変化量")]
    [SerializeField] private float yawSensitivity = 0.12f;
    [SerializeField] private bool invertPitch = true;

    [Header("Smoothing")]
    [SerializeField] private float aimSmoothTime = 0.05f;

    private float _pitch;
    private float _yaw;
    private float _pitchVelocity;
    private float _yawVelocity;
    private float _targetPitch;
    private float _targetYaw;
    private bool _isDragging;
    private Vector2 _lastPointerPosition;

    public float CurrentPitch => _pitch;
    public float CurrentYaw => _yaw;
    public bool IsDragging => _isDragging;
    public Transform CameraTransform => cameraTransform != null ? cameraTransform : transform;

    private void Awake()
    {
        if (cameraTransform == null)
        {
            cameraTransform = transform;
        }

        Vector3 euler = cameraTransform.localEulerAngles;
        _pitch = NormalizeAngle(euler.x);
        _yaw = NormalizeAngle(euler.y);
        _targetPitch = _pitch;
        _targetYaw = _yaw;
    }

    private void Update()
    {
        HandlePointerInput();
        ApplyAimRotation();
    }

    private void HandlePointerInput()
    {
        Pointer pointer = Pointer.current;
        if (pointer == null)
        {
            return;
        }

        Vector2 position = pointer.position.ReadValue();

        if (pointer.press.wasPressedThisFrame)
        {
            _isDragging = true;
            _lastPointerPosition = position;
        }

        if (_isDragging && pointer.press.isPressed)
        {
            Vector2 delta = position - _lastPointerPosition;
            _lastPointerPosition = position;

            float pitchDelta = delta.y * pitchSensitivity * (invertPitch ? -1f : 1f);
            _targetPitch = Mathf.Clamp(_targetPitch + pitchDelta, minPitch, maxPitch);

            if (allowHorizontalAim)
            {
                _targetYaw = Mathf.Clamp(_targetYaw + delta.x * yawSensitivity, minYaw, maxYaw);
            }
        }

        if (pointer.press.wasReleasedThisFrame)
        {
            _isDragging = false;
        }
    }

    private void ApplyAimRotation()
    {
        _pitch = Mathf.SmoothDampAngle(_pitch, _targetPitch, ref _pitchVelocity, aimSmoothTime);
        _yaw = Mathf.SmoothDampAngle(_yaw, _targetYaw, ref _yawVelocity, aimSmoothTime);
        cameraTransform.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }

    public void ResetAim(float pitch = 0f, float yaw = 0f)
    {
        _targetPitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        _targetYaw = Mathf.Clamp(yaw, minYaw, maxYaw);
        _pitch = _targetPitch;
        _yaw = _targetYaw;
        _pitchVelocity = 0f;
        _yawVelocity = 0f;
        cameraTransform.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);
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
