using System.Collections;
using UnityEngine;

/// <summary>
/// ball が bin に入るまでゲームを継続し、入ったら Unity セッションを終了する。
/// Unity as a Library 向けに Application.Unload を使い、ホストアプリのプロセスは維持する。
/// </summary>
public class EndGameOnBinGoal : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BinGoalDetector binGoalDetector;
    [SerializeField] private BallLauncher ballLauncher;

    [Header("End Timing")]
    [Tooltip("ゴール演出を見せてから Unload するまでの秒数")]
    [SerializeField] private float unloadDelaySeconds = 0.75f;

    private bool _isEnding;

    private void Awake()
    {
        if (binGoalDetector == null)
        {
            binGoalDetector = FindFirstObjectByType<BinGoalDetector>();
        }

        if (ballLauncher == null)
        {
            ballLauncher = FindFirstObjectByType<BallLauncher>();
        }
    }

    private void OnEnable()
    {
        if (binGoalDetector != null)
        {
            binGoalDetector.Scored += OnScored;
        }
    }

    private void OnDisable()
    {
        if (binGoalDetector != null)
        {
            binGoalDetector.Scored -= OnScored;
        }

        StopAllCoroutines();
    }

    private void OnScored()
    {
        if (_isEnding)
        {
            return;
        }

        _isEnding = true;
        StartCoroutine(StopPlayAndEndSession());
    }

    private IEnumerator StopPlayAndEndSession()
    {
        // BinGoalDetector が同フレーム内で NotifyScored（リセット予約）するため、
        // その処理の後にキャンセルする。
        yield return null;

        if (ballLauncher != null)
        {
            ballLauncher.CancelInvoke();
            ballLauncher.enabled = false;
        }

        if (unloadDelaySeconds > 0f)
        {
            yield return new WaitForSeconds(unloadDelaySeconds);
        }

        EndUnitySession();
    }

    private void EndUnitySession()
    {
        Debug.Log("Goal reached. Ending Unity session (UaaL unload).");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // UaaL: ホストアプリを終了せず Unity Player をアンロードする。
        // Application.Quit だとプロセスごと終了するため使わない。
        Application.Unload();
#endif
    }
}
