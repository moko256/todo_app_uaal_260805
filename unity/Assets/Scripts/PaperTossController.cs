using UnityEngine;

/// <summary>
/// MainScene 上のカメラ / ball / bin をまとめて接続するエントリポイント。
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

    private void Start()
    {
        if (swipeAimCamera != null)
        {
            swipeAimCamera.ResetAim(initialPitch);
        }
    }
}
