using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// bin 内のトリガーに ball が入ったら成功とみなす。
/// </summary>
[RequireComponent(typeof(Collider))]
public class BinGoalDetector : MonoBehaviour
{
    [Header("Filter")]
    [SerializeField] private string ballTag = "Ball";
    [SerializeField] private Rigidbody ballRigidbody;
    [SerializeField] private bool requireDownwardEntry = true;
    [SerializeField] private float maxUpwardVelocityToScore = 0.5f;

    [Header("Events")]
    [SerializeField] private UnityEvent onScored;
    [SerializeField] private bool scoreOnlyOncePerThrow = true;

    public event Action Scored;

    private bool _hasScoredThisThrow = true;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
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
        BallLauncher launcher = FindFirstObjectByType<BallLauncher>();
        if (launcher != null)
        {
            launcher.ThrowStateChanged += OnThrowStateChanged;
            launcher.ResetReady += OnBallReset;
        }
    }

    private void OnDisable()
    {
        BallLauncher launcher = FindFirstObjectByType<BallLauncher>();
        if (launcher != null)
        {
            launcher.ThrowStateChanged -= OnThrowStateChanged;
            launcher.ResetReady -= OnBallReset;
        }
    }

    private void OnThrowStateChanged(bool thrown)
    {
        if (thrown)
        {
            _hasScoredThisThrow = false;
        }
    }

    private void OnBallReset()
    {
        _hasScoredThisThrow = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (scoreOnlyOncePerThrow && _hasScoredThisThrow)
        {
            return;
        }

        if (!IsBall(other))
        {
            return;
        }

        Rigidbody rb = other.attachedRigidbody != null ? other.attachedRigidbody : ballRigidbody;
        if (requireDownwardEntry && rb != null && rb.linearVelocity.y > maxUpwardVelocityToScore)
        {
            return;
        }

        _hasScoredThisThrow = true;
        onScored?.Invoke();
        Scored?.Invoke();

        BallLauncher launcher = FindFirstObjectByType<BallLauncher>();
        if (launcher != null)
        {
            launcher.NotifyScored();
        }

        Debug.Log("Goal! Ball entered the bin.");
    }

    private bool IsBall(Collider other)
    {
        if (!string.IsNullOrEmpty(ballTag) && other.CompareTag(ballTag))
        {
            return true;
        }

        if (ballRigidbody != null)
        {
            if (other.attachedRigidbody == ballRigidbody)
            {
                return true;
            }

            if (other.transform == ballRigidbody.transform || other.transform.IsChildOf(ballRigidbody.transform))
            {
                return true;
            }
        }

        return other.name.IndexOf("ball", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public void ResetScoreGate()
    {
        _hasScoredThisThrow = false;
    }
}
