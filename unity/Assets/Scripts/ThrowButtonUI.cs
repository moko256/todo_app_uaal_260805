using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Scene 上の ThrowCanvas / ThrowButton への参照を保持する。
/// OnClick 接続は BallLauncher.throwButton 側で行う。
/// </summary>
public class ThrowButtonUI : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button throwButton;
}
