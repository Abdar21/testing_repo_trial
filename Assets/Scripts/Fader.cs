using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening; // Import the DOTween namespace
using UnityEngine.Video;

/// <summary>
/// Controls the fading of a CanvasGroup using DOTween.
/// This component ensures that fade animations are smooth and properly handled,
/// even when triggered in rapid succession.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class Fader : MonoBehaviour
{
    [Header("Components")]
    [Tooltip("The CanvasGroup to be faded. If not assigned, it will be fetched from this GameObject.")]
    public CanvasGroup canvasGroup;

    [Header("Fade Timings")]
    [Tooltip("The duration of the fade-in animation in seconds.")]
    public float fadeInDuration = 0.4f;

    [Tooltip("The duration of the fade-out animation in seconds.")]
    public float fadeOutDuration = 0.4f;

    [Header("Automatic Behavior")]
    [Tooltip("If checked, the canvas will automatically fade in when the GameObject is enabled.")]
    [SerializeField] private bool autoFadeInOnEnable = false;

    [Tooltip("The delay in seconds before the automatic fade-in begins.")]
    [SerializeField] private float autoFadeInDelay = 0f;

    [Tooltip("If checked, the canvas alpha will be set to 0 in Awake, making it invisible on start.")]
    [SerializeField] private bool setAlphaToZeroOnAwake = false;

    [Header("Events")]
    [Tooltip("Action invoked when the GameObject is enabled.")]
    [SerializeField] private UnityEvent onEnableAction;

    [Tooltip("Action invoked when the GameObject is Disabled.")]
    [SerializeField] private UnityEvent onDisableAction;

    [Tooltip("Action invoked when the fade-in animation is complete.")]
    [SerializeField] private UnityEvent onFadeInComplete;

    [Tooltip("Action invoked when the fade-out animation is complete.")]
    [SerializeField] private UnityEvent onFadeOutComplete;

    [Header("Video Player Management")]
    [Tooltip("If true, releases the RenderTextures of all child VideoPlayers when this component is enabled. Useful for memory management.")]
    [SerializeField] private bool releaseRenderTexturesOnEnable = false;

    // Stores the currently active fade animation.
    private Tween _currentFade;
    private VideoPlayer[] _videoPlayers;

    private void Awake()
    {
        // Ensure the CanvasGroup component is assigned.
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        // Set the initial state of the canvas if required.
        if (setAlphaToZeroOnAwake)
        {
            canvasGroup.alpha = 0f;
        }
    }

    private void OnEnable()
    {
        if (setAlphaToZeroOnAwake)
        {
            canvasGroup.alpha = 0f;
        }
        // Invoke any specified OnEnable actions.
        onEnableAction?.Invoke();

        // Trigger the automatic fade-in if configured.
        if (autoFadeInOnEnable)
        {
            FadeIn(autoFadeInDelay);
        }

        // Release video player render textures if enabled
        if (releaseRenderTexturesOnEnable)
        {
            ReleaseAllVideoPlayersInChildren();
        }
    }

    private void OnDisable()
    {
        onDisableAction?.Invoke();
        _currentFade?.Kill();
    }

    public bool IsFading()
    {
        // A tween is active if it's not null and hasn't been killed.
        return _currentFade != null && _currentFade.IsActive();
    }


    public void FadeIn(float delay = 0f)
    {
        StartFade(1f, fadeInDuration, delay, onFadeInComplete);
    }


    public void FadeOut(float delay = 0f)
    {
        StartFade(0f, fadeOutDuration, delay, onFadeOutComplete);
    }

    /// <summary>
    /// Fades the canvas out and then deactivates the GameObject.
    /// </summary>
    public void FadeOutAndDisable()
    {
        // Start the fade and add a custom action to the OnComplete callback.
        StartFade(0f, fadeOutDuration, 0f, onFadeOutComplete, () => gameObject.SetActive(false));
    }

    private void StartFade(float targetAlpha, float duration, float delay, UnityEvent onCompleteEvent, System.Action onCompleteAction = null)
    {
        _currentFade?.Kill();

        // Create the new fade tween.
        _currentFade = canvasGroup.DOFade(targetAlpha, duration)
            .SetDelay(delay)
            .SetEase(Ease.InOutSine) // A smooth ease for UI transitions
            .OnComplete(() =>
            {
                onCompleteEvent?.Invoke();
                onCompleteAction?.Invoke();
                _currentFade = null; // Clear the tween reference.
            });
    }


    private void ReleaseAllVideoPlayersInChildren()
    {
        if (_videoPlayers == null)
        {
            _videoPlayers = GetComponentsInChildren<VideoPlayer>(true); // Include inactive
        }

        foreach (VideoPlayer videoPlayer in _videoPlayers)
        {
            if (videoPlayer.targetTexture != null)
            {
                // Release the RenderTexture to free up memory.
                videoPlayer.targetTexture.Release();
            }
        }
    }
}