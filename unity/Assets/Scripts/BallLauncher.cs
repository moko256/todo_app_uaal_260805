using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// カメラの向いている方向へ ball を打ち出す。
/// uGUI の「投げる」ボタンから呼び出される想定。
/// </summary>
public class BallLauncher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody ballRigidbody;
    [SerializeField] private Transform throwOrigin;
    [SerializeField] private SwipeAimCamera aimCamera;
    [SerializeField] private Button throwButton;

    [Header("Hold Pose")]
    [SerializeField] private Vector3 holdLocalOffset = new Vector3(0f, -0.15f, 0.45f);
    [SerializeField] private Vector3 holdLocalEuler = Vector3.zero;
    [Tooltip("ball ルートから見た見た目/コライダー中心。ホールド位置の補正に使う")]
    [SerializeField] private Vector3 visualCenterOffset = new Vector3(0.8113202f, 0.17781997f, -0.15533f);

    [Header("Throw")]
    [SerializeField] private float throwForce = 4.5f;
    [SerializeField] private float minThrowForce = 2.5f;
    [SerializeField] private float maxThrowForce = 12f;
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

    private void OnEnable()
    {
        if (throwButton != null)
        {
            throwButton.onClick.AddListener(ThrowBall);
        }
    }

    private void OnDisable()
    {
        if (throwButton != null)
        {
            throwButton.onClick.RemoveListener(ThrowBall);
        }
    }

    private void Start()
    {
        HoldBall();
    }

    private void Update()
    {
        if (_isHeld)
        {
            FollowHoldPose();
        }
    }

    public void SetThrowButton(Button button)
    {
        if (throwButton != null)
        {
            throwButton.onClick.RemoveListener(ThrowBall);
        }

        throwButton = button;

        if (isActiveAndEnabled && throwButton != null)
        {
            throwButton.onClick.AddListener(ThrowBall);
        }

        RefreshThrowButtonState();
    }

    /// <summary>
    /// uGUI ボタンの OnClick から呼ぶ。
    /// </summary>
    public void ThrowBall()
    {
        LaunchInCameraDirection();
    }

    private void FollowHoldPose()
    {
        if (throwOrigin == null || ballRigidbody == null)
        {
            return;
        }

        Transform ballTransform = ballRigidbody.transform;
        ballTransform.rotation = throwOrigin.rotation * Quaternion.Euler(holdLocalEuler);
        Vector3 holdPoint = throwOrigin.TransformPoint(holdLocalOffset);
        ballTransform.position = holdPoint - ballTransform.rotation * visualCenterOffset;
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
            : Mathf.Clamp(throwForce, minThrowForce, maxThrowForce);

        ReleaseAndLaunch(direction, force);
    }

    private void ReleaseAndLaunch(Vector3 direction, float force)
    {
        _isHeld = false;
        _hasThrown = true;
        ThrowStateChanged?.Invoke(true);
        RefreshThrowButtonState();

        ballRigidbody.isKinematic = false;
        ballRigidbody.linearVelocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.AddForce(direction * force, throwForceMode);
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
        RefreshThrowButtonState();

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

    private void RefreshThrowButtonState()
    {
        if (throwButton != null)
        {
            throwButton.interactable = !_hasThrown;
        }
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
        throwForce = Mathf.Clamp(throwForce, minThrowForce, maxThrowForce);
        autoResetSeconds = Mathf.Max(0.1f, autoResetSeconds);
        scoredResetDelay = Mathf.Max(0.1f, scoredResetDelay);
    }
#endif
}
