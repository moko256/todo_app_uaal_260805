using System.Collections;
using UnityEngine;

/// <summary>
/// ball が bin に入るまでゲームを継続し、入ったら終了画面を表示してから Unity セッションを終了する。
/// Unity as a Library 向けに Application.Unload を使い、ホストアプリのプロセスは維持する。
/// Editor プレビューでは再生を停止する。
/// </summary>
public class EndGameOnBinGoal : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private BinGoalDetector binGoalDetector;
    [SerializeField] private BallLauncher ballLauncher;

    [Header("End Screen")]
    [Tooltip("終了画面 UI（外部 prefab）。未設定でもセッション終了は行う。")]
    [SerializeField] private GameObject gameEndScreenPrefab;
    [Tooltip("終了画面を表示してから Unload / プレビュー終了するまでの秒数")]
    [SerializeField] private float endScreenDisplaySeconds = 3f;

    private bool _isEnding;
    private GameObject _endScreenInstance;

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
        CloseEndScreen();
    }

    private void OnScored()
    {
        if (_isEnding)
        {
            return;
        }

        _isEnding = true;
        StartCoroutine(ShowEndScreenAndEndSession());
    }

    private IEnumerator ShowEndScreenAndEndSession()
    {
        // BinGoalDetector が同フレーム内で NotifyScored（リセット予約）するため、
        // その処理の後にキャンセルする。
        yield return null;

        if (ballLauncher != null)
        {
            ballLauncher.CancelInvoke();
            ballLauncher.enabled = false;
        }

        ShowEndScreen();

        if (endScreenDisplaySeconds > 0f)
        {
            yield return new WaitForSeconds(endScreenDisplaySeconds);
        }

        CloseEndScreen();
        EndUnitySession();
    }

    private void ShowEndScreen()
    {
        if (gameEndScreenPrefab == null)
        {
            Debug.LogWarning("Game end screen prefab is not assigned.");
            return;
        }

        if (_endScreenInstance != null)
        {
            return;
        }

        _endScreenInstance = Instantiate(gameEndScreenPrefab);
        _endScreenInstance.name = gameEndScreenPrefab.name;
    }

    private void CloseEndScreen()
    {
        if (_endScreenInstance == null)
        {
            return;
        }

        Destroy(_endScreenInstance);
        _endScreenInstance = null;
    }

    private void EndUnitySession()
    {
        Debug.Log("Goal reached. Ending Unity session (UaaL unload / editor preview stop).");

#if UNITY_EDITOR
        // プレビュー（Play Mode）を終了する。
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // UaaL: ホストアプリを終了せず Unity Player をアンロードする。
        // Application.Quit だとプロセスごと終了するため使わない。
        Application.Unload();
#endif
    }
}
