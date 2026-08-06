using UnityEngine;

/// <summary>
/// MainScene 上のカメラ / ball / bin をまとめて接続するエントリポイント。
/// Inspector から参照と各パラメータを調整できる。
/// </summary>
public class PaperTossController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private SwipeAimCamera swipeAimCamera;
    [SerializeField] private BallLauncher ballLauncher;
    [SerializeField] private BinGoalDetector binGoalDetector;
    [SerializeField] private ThrowButtonUI throwButtonUI;
    [SerializeField] private Transform ball;
    [SerializeField] private Transform bin;

    [Header("Startup Aim")]
    [SerializeField] private float initialPitch = 8f;
    [SerializeField] private float initialYaw = 0f;

    private void Awake()
    {
        AutoWireIfNeeded();
    }

    private void Start()
    {
        if (swipeAimCamera != null)
        {
            swipeAimCamera.ResetAim(initialPitch, initialYaw);
        }

        if (binGoalDetector != null)
        {
            binGoalDetector.Scored += HandleScored;
        }
    }

    private void OnDestroy()
    {
        if (binGoalDetector != null)
        {
            binGoalDetector.Scored -= HandleScored;
        }
    }

    private void HandleScored()
    {
        // 必要なら UI や効果音をここから拡張する
    }

    [ContextMenu("Auto Wire References")]
    private void AutoWireIfNeeded()
    {
        if (swipeAimCamera == null)
        {
            swipeAimCamera = FindFirstObjectByType<SwipeAimCamera>();
        }

        if (ballLauncher == null)
        {
            ballLauncher = FindFirstObjectByType<BallLauncher>();
        }

        if (binGoalDetector == null)
        {
            binGoalDetector = FindFirstObjectByType<BinGoalDetector>();
        }

        if (throwButtonUI == null)
        {
            throwButtonUI = FindFirstObjectByType<ThrowButtonUI>();
        }

        if (ball == null)
        {
            GameObject ballObject = GameObject.Find("ball");
            if (ballObject != null)
            {
                ball = ballObject.transform;
            }
        }

        if (bin == null)
        {
            GameObject binObject = GameObject.Find("bin");
            if (binObject != null)
            {
                bin = binObject.transform;
            }
        }
    }
}
