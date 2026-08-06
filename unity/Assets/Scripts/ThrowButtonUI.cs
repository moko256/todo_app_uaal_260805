using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 投げるボタン UI Prefab を生成し、BallLauncher に接続する。
/// </summary>
public class ThrowButtonUI : MonoBehaviour
{
    [Header("Prefab")]
    [Tooltip("投げるボタン UI（外部 prefab）。")]
    [SerializeField] private GameObject throwCanvasPrefab;

    [Header("References")]
    [SerializeField] private BallLauncher ballLauncher;

    private GameObject _canvasInstance;

    private void Awake()
    {
        if (ballLauncher == null)
        {
            ballLauncher = FindFirstObjectByType<BallLauncher>();
        }

        Button throwButton = ShowThrowCanvas();
        if (ballLauncher != null && throwButton != null)
        {
            ballLauncher.SetThrowButton(throwButton);
        }
    }

    private void OnDestroy()
    {
        CloseThrowCanvas();
    }

    private Button ShowThrowCanvas()
    {
        if (throwCanvasPrefab == null)
        {
            Debug.LogWarning("Throw canvas prefab is not assigned.");
            return null;
        }

        if (_canvasInstance != null)
        {
            return _canvasInstance.GetComponentInChildren<Button>(true);
        }

        _canvasInstance = Instantiate(throwCanvasPrefab);
        _canvasInstance.name = throwCanvasPrefab.name;
        return _canvasInstance.GetComponentInChildren<Button>(true);
    }

    private void CloseThrowCanvas()
    {
        if (_canvasInstance == null)
        {
            return;
        }

        Destroy(_canvasInstance);
        _canvasInstance = null;
    }
}
