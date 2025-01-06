using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    [Header("Fade Durations")]
    public float fadeInDuration = 1f; // Duration for fading in
    public float fadeOutDuration = 1f; // Duration for fading out
    public float delayBeforeFadeOut = 1f; // Delay before starting the fade-out

    private Image imageComponent;
    private TextMeshProUGUI textComponent;

    private void Start()
    {
        // Get references to the Image and TextMeshPro components
        imageComponent = transform.GetChild(0).GetComponent<Image>();
        textComponent = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        if (imageComponent == null || textComponent == null)
        {
            Debug.LogError("Missing Image or TextMeshPro component in the first two children of this GameObject.");
            return;
        }

        // Start the fade sequence
        PlayFadeSequence();
    }

    private void PlayFadeSequence()
    {
        // Ensure initial states
        imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 0);
        textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 0);

        // Create a sequence for the fade-in and fade-out animations
        Sequence fadeSequence = DOTween.Sequence();
        fadeSequence.AppendInterval(2);
        // Fade in the Image and TextMeshPro
        fadeSequence.Append(imageComponent.DOFade(1, fadeInDuration + 2));
        fadeSequence.Append(textComponent.DOFade(1, fadeInDuration));

        // Add a delay before fading out
        fadeSequence.AppendInterval(delayBeforeFadeOut);

        // Fade out the Image and TextMeshPro
        fadeSequence.Append(imageComponent.DOFade(0, fadeOutDuration));
        fadeSequence.Join(textComponent.DOFade(0, fadeOutDuration));

        // Destroy the GameObject after the sequence completes
        fadeSequence.OnComplete(() => Destroy(gameObject));
    }
}
