using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// カメラの向いている方向へ ball を打ち出す。
/// 指を離したときのスワイプ速度に応じて勢いを変える。
/// </summary>
public class BallLauncher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody ballRigidbody;
    [SerializeField] private Transform throwOrigin;
    [SerializeField] private SwipeAimCamera aimCamera;

    [Header("Hold Pose")]
    [SerializeField] private Vector3 holdLocalOffset = new Vector3(0f, -0.15f, 0.45f);
    [SerializeField] private Vector3 holdLocalEuler = Vector3.zero;
    [Tooltip("ball ルートから見た見た目/コライダー中心。ホールド位置の補正に使う")]
    [SerializeField] private Vector3 visualCenterOffset = new Vector3(0.8113202f, 0.17781997f, -0.15533f);

    [Header("Throw")]
    [SerializeField] private float baseThrowForce = 4.5f;
    [SerializeField] private float swipeForceMultiplier = 0.035f;
    [SerializeField] private float minThrowForce = 2.5f;
    [SerializeField] private float maxThrowForce = 12f;
    [Tooltip("このスワイプ速度（px/秒）未満なら打ち出さない")]
    [SerializeField] private float minSwipeSpeed = 400f;
    [Tooltip("上方向スワイプのみ打ち出す場合は true")]
    [SerializeField] private bool requireUpwardSwipe = true;
    [SerializeField] private float upwardThrowBias = 0.15f;
    [SerializeField] private ForceMode throwForceMode = ForceMode.VelocityChange;

    [Header("Reset")]
    [SerializeField] private float autoResetSeconds = 4f;
    [SerializeField] private float scoredResetDelay = 1.25f;

    public event Action Thrown;
    public event Action ResetReady;
    public event Action<bool> ThrowStateChanged;

    private bool _isHeld = true;
    private bool _hasThrown;
    private bool _isResetScheduled;
    private Vector2 _lastPosition;
    private float _lastMoveTime;
    private Vector2 _recentVelocity;

    public bool HasThrown => _hasThrown;
    public bool IsHeld => _isHeld;
    public Rigidbody BallRigidbody => ballRigidbody;

    private void Awake()
    {
        if (aimCamera == null)
        {
            aimCamera = FindFirstObjectByType<SwipeAimCamera>();
        }

        if (throwOrigin == null && aimCamera != null)
        {
            throwOrigin = aimCamera.CameraTransform;
        }

        if (ballRigidbody == null)
        {
            GameObject ballObject = GameObject.Find("ball");
            if (ballObject != null)
            {
                ballRigidbody = ballObject.GetComponent<Rigidbody>();
            }
        }
    }

    private void Start()
    {
        HoldBall();
    }

    private void Update()
    {
        HandlePointerInput();

        if (_isHeld)
        {
            FollowHoldPose();
        }
    }

    private void HandlePointerInput()
    {
        if (_hasThrown || ballRigidbody == null)
        {
            return;
        }

        Pointer pointer = Pointer.current;
        if (pointer == null)
        {
            return;
        }

        Vector2 position = pointer.position.ReadValue();
        float now = Time.unscaledTime;

        if (pointer.press.wasPressedThisFrame)
        {
            _lastPosition = position;
            _lastMoveTime = now;
            _recentVelocity = Vector2.zero;
        }

        if (pointer.press.isPressed)
        {
            float dt = Mathf.Max(now - _lastMoveTime, 0.0001f);
            Vector2 frameDelta = position - _lastPosition;
            _recentVelocity = frameDelta / dt;
            _lastPosition = position;
            _lastMoveTime = now;
        }

        if (pointer.press.wasReleasedThisFrame)
        {
            TryThrow(_recentVelocity);
        }
    }

    private void FollowHoldPose()
    {
        if (throwOrigin == null)
        {
            return;
        }

        Transform ballTransform = ballRigidbody.transform;
        ballTransform.rotation = throwOrigin.rotation * Quaternion.Euler(holdLocalEuler);
        Vector3 holdPoint = throwOrigin.TransformPoint(holdLocalOffset);
        ballTransform.position = holdPoint - ballTransform.rotation * visualCenterOffset;
    }

    public void TryThrow(Vector2 swipeVelocityPixelsPerSecond)
    {
        if (_hasThrown || ballRigidbody == null)
        {
            return;
        }

        float upwardSpeed = swipeVelocityPixelsPerSecond.y;
        float swipeSpeed = swipeVelocityPixelsPerSecond.magnitude;

        if (requireUpwardSwipe && upwardSpeed < minSwipeSpeed * 0.25f)
        {
            return;
        }

        if (swipeSpeed < minSwipeSpeed)
        {
            return;
        }

        Transform aimTransform = throwOrigin != null ? throwOrigin : transform;
        Vector3 direction = (aimTransform.forward + Vector3.up * upwardThrowBias).normalized;
        float force = Mathf.Clamp(
            baseThrowForce + swipeSpeed * swipeForceMultiplier,
            minThrowForce,
            maxThrowForce);

        ReleaseAndLaunch(direction, force);
    }

    public void LaunchInCameraDirection(float forceOverride = -1f)
    {
        if (_hasThrown || ballRigidbody == null)
        {
            return;
        }

        Transform aimTransform = throwOrigin != null ? throwOrigin : transform;
        Vector3 direction = (aimTransform.forward + Vector3.up * upwardThrowBias).normalized;
        float force = forceOverride > 0f
            ? Mathf.Clamp(forceOverride, minThrowForce, maxThrowForce)
            : baseThrowForce;

        ReleaseAndLaunch(direction, force);
    }

    private void ReleaseAndLaunch(Vector3 direction, float force)
    {
        _isHeld = false;
        _hasThrown = true;
        ThrowStateChanged?.Invoke(true);

        ballRigidbody.isKinematic = false;
        ballRigidbody.linearVelocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.AddForce(direction * force, throwForceMode);

        // 少し回転をつけて紙っぽくする
        ballRigidbody.AddTorque(UnityEngine.Random.insideUnitSphere * force * 0.35f, ForceMode.VelocityChange);

        Thrown?.Invoke();
        ScheduleReset(autoResetSeconds);
    }

    public void NotifyScored()
    {
        ScheduleReset(scoredResetDelay);
    }

    public void HoldBall()
    {
        CancelInvoke(nameof(HoldBall));
        _isResetScheduled = false;
        _hasThrown = false;
        _isHeld = true;
        ThrowStateChanged?.Invoke(false);

        if (ballRigidbody == null)
        {
            return;
        }

        ballRigidbody.linearVelocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.isKinematic = true;
        FollowHoldPose();
        ResetReady?.Invoke();
    }

    private void ScheduleReset(float delay)
    {
        if (_isResetScheduled)
        {
            CancelInvoke(nameof(HoldBall));
        }

        _isResetScheduled = true;
        Invoke(nameof(HoldBall), Mathf.Max(0.05f, delay));
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        minThrowForce = Mathf.Max(0.1f, minThrowForce);
        maxThrowForce = Mathf.Max(minThrowForce, maxThrowForce);
        minSwipeSpeed = Mathf.Max(0f, minSwipeSpeed);
        autoResetSeconds = Mathf.Max(0.1f, autoResetSeconds);
        scoredResetDelay = Mathf.Max(0.1f, scoredResetDelay);
    }
#endif
}
