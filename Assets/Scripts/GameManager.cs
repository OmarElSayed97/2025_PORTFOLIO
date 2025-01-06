using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TMPro text
using DG.Tweening; // For tweening

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [Header("UI Elements")]
    public RectTransform cinematicBarTop; // Top bar UI
    public RectTransform cinematicBarBottom; // Bottom bar UI
    public TextMeshProUGUI titleText; // Main title
    public TextMeshProUGUI subtitleText; // Subtitle
    public TextMeshProUGUI storyText; // TextMeshPro component for the dialogue
    public GameObject interactionLabel;
    public Image logoImage;
    public float letterDelay = 0.05f; // Delay between each letter in the dialogue
    
    // Store initial positions
    private Vector2 cinematicBarTopInitialPos;
    private Vector2 cinematicBarBottomInitialPos;

    [Header("Materials")]
    public Material skyboxMaterial;
    public Material edgeMaterial;

    [Header("Colors")]
    public Color[] skyboxColors;
    public Color[] edgeMaterialBaseColors;
    public Color[] edgeMaterialEmissionColors;

    [HideInInspector] public bool isCurrentlyCinematic;

    private int currentCheckpointID = 0; // Tracks the current checkpoint ID
    private Sequence dialogueSequence,cinematicSequence;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        skyboxMaterial = RenderSettings.skybox;
        // Save the initial positions of the cinematic bars
        cinematicBarTopInitialPos = cinematicBarTop.anchoredPosition;
        cinematicBarBottomInitialPos = cinematicBarBottom.anchoredPosition;
    }

    public void CheckpointReached(ChapterCheckpoint checkpoint)
    {
        if (checkpoint.checkpointID == currentCheckpointID) return; // Avoid re-processing the same checkpoint

        currentCheckpointID = checkpoint.checkpointID;
        isCurrentlyCinematic = true;
        StopAllCoroutines();
        
        // Activate UI elements for cinematic effect
        cinematicBarTop.gameObject.SetActive(true);
        cinematicBarBottom.gameObject.SetActive(true);
        titleText.gameObject.SetActive(true);
        subtitleText.gameObject.SetActive(true);
        cinematicBarTop.anchoredPosition = cinematicBarTopInitialPos;
        cinematicBarBottom.anchoredPosition = cinematicBarBottomInitialPos;
        storyText.gameObject.SetActive(false);

        // Prepare initial states
        titleText.alpha = 0;
        subtitleText.alpha = 0;
        switch (checkpoint.checkpointID)
        {
            case 6:
                titleText.text = "Chapter " + (checkpoint.checkpointID - 1) + ": " + checkpoint.chapterTitle; // Update title text
                break;
            case 7:
                titleText.text = "Try Again"; // Update title text
                break;
            default:
                titleText.text = "Chapter " + (checkpoint.checkpointID) + ": " + checkpoint.chapterTitle; // Update title text
                break;
        }
        subtitleText.text = checkpoint.chapterSubtitle; // Update subtitle text
       
        // Start the tween sequence
        cinematicSequence = DOTween.Sequence();

        // Move bars in and fade in title
        cinematicSequence.Append(cinematicBarTop.DOAnchorPosY(cinematicBarTopInitialPos.y - cinematicBarTop.rect.height, 2f).SetEase(Ease.OutQuad)); // Move top bar down
        cinematicSequence.Join(cinematicBarBottom.DOAnchorPosY(cinematicBarBottomInitialPos.y + cinematicBarBottom.rect.height, 2f).SetEase(Ease.OutQuad)); // Move bottom bar up
        cinematicSequence.Join(titleText.DOFade(1, 3f)); // Fade in the title
        // Fade in subtitle and lerp colors
        cinematicSequence.Join(subtitleText.DOFade(1, 5f)); // Fade in the subtitle

        if (checkpoint.checkpointID != 7)
        {
            cinematicSequence.Join(DOTween.To(() => skyboxMaterial.GetColor("_Tint"), x => skyboxMaterial.SetColor("_Tint", x), skyboxColors[checkpoint.checkpointID], 3f));
            cinematicSequence.Join(DOTween.To(() => edgeMaterial.color, x => edgeMaterial.color = x, edgeMaterialBaseColors[checkpoint.checkpointID], 3f));
            cinematicSequence.Join(DOTween.To(
                () => edgeMaterial.GetColor("_EmissionColor"), 
                x => edgeMaterial.SetColor("_EmissionColor", x * 2.5f), 
                edgeMaterialEmissionColors[checkpoint.checkpointID] * 2.5f, 
                3f
            ));
        }
       

        // Move bars out and fade out text
        cinematicSequence.Append(cinematicBarTop.DOAnchorPosY(cinematicBarTopInitialPos.y, 1.5f).SetEase(Ease.InQuad)); // Move top bar back up
        cinematicSequence.Join(cinematicBarBottom.DOAnchorPosY(cinematicBarBottomInitialPos.y, 1.5f).SetEase(Ease.InQuad)); // Move bottom bar back down
        cinematicSequence.Join(titleText.DOFade(0, 2f)); // Fade out title
        cinematicSequence.Join(subtitleText.DOFade(0, 2f)); // Fade out subtitle

        // Deactivate UI elements at the end
        cinematicSequence.AppendCallback(() =>
        {
            cinematicBarTop.gameObject.SetActive(false);
            cinematicBarBottom.gameObject.SetActive(false);
            titleText.gameObject.SetActive(false);
            subtitleText.gameObject.SetActive(false);
            storyText.text = "";
            cinematicSequence.Kill();
            StopAllCoroutines();
            isCurrentlyCinematic = false;
        });
    }


    public void InteractWithStory(string story)
    {
        cinematicSequence.Kill();
        StopAllCoroutines();
        if(isCurrentlyCinematic) return;
        storyText.gameObject.SetActive(true);
        // Activate UI elements for cinematic effect
        isCurrentlyCinematic = true;
        cinematicBarTop.gameObject.SetActive(true);
        cinematicBarBottom.gameObject.SetActive(true);
        storyText.text = "";
        cinematicSequence = DOTween.Sequence();

        // Move bars in and fade in title
        cinematicSequence.Append(cinematicBarTop.DOAnchorPosY(cinematicBarTopInitialPos.y - cinematicBarTop.rect.height, 2f).SetEase(Ease.OutQuad)); // Move top bar down
        cinematicSequence.Join(cinematicBarBottom.DOAnchorPosY(cinematicBarBottomInitialPos.y + cinematicBarBottom.rect.height, 2f).SetEase(Ease.OutQuad)); // Move bottom bar up
        // Animate the dialogue letter by letter
        cinematicSequence.AppendCallback(() => AnimateStory(story));

        // Optional: Add a delay after the dialogue finishes
        cinematicSequence.AppendInterval(1f);
    }

    public void EndInteractionWithStory()
    {
        cinematicSequence = DOTween.Sequence();
        // Move bars out and fade out text
        cinematicSequence.Append(cinematicBarTop.DOAnchorPosY(cinematicBarTopInitialPos.y, 1.5f).SetEase(Ease.InQuad)); // Move top bar back up
        cinematicSequence.Join(cinematicBarBottom.DOAnchorPosY(cinematicBarBottomInitialPos.y, 1.5f).SetEase(Ease.InQuad)); // Move bottom bar back down
        // Deactivate UI elements at the end
        cinematicSequence.AppendCallback(() =>
        {
            cinematicBarTop.gameObject.SetActive(false);
            cinematicBarBottom.gameObject.SetActive(false);
            
            storyText.text = "";
            storyText.gameObject.SetActive(false);
            cinematicSequence.Kill();
            dialogueSequence.Kill();
        });
        isCurrentlyCinematic = false;
        StopAllCoroutines();
    }
    
    private void AnimateStory(string text)
    {
        dialogueSequence.Kill();
        dialogueSequence = DOTween.Sequence();
        storyText.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            if (!isCurrentlyCinematic)
            {
                StopAllCoroutines();
                return;
            }
            char currentChar = text[i];
            dialogueSequence.AppendCallback(() =>
            {
                storyText.text += currentChar; // Add the next letter
            });

            dialogueSequence.AppendInterval(letterDelay); // Wait before adding the next letter
        }
    }

    public void EndGame()
    {
        cinematicSequence.Kill();
        // Activate UI elements for cinematic effect
        cinematicBarTop.gameObject.SetActive(true);
        cinematicBarBottom.gameObject.SetActive(true);
        titleText.gameObject.SetActive(true);
        subtitleText.gameObject.SetActive(true);
        StopAllCoroutines();
        cinematicBarTop.anchoredPosition = cinematicBarTopInitialPos;
        cinematicBarBottom.anchoredPosition = cinematicBarBottomInitialPos;
        storyText.text = "";
        
        titleText.alpha = 0;
        subtitleText.alpha = 0;
        
        titleText.text = "WHAT IS NEXT?"; // Update title text
        titleText.color = new Color(0,0,0,0);

        // Start the tween sequence
        cinematicSequence = DOTween.Sequence();
        // Get the current exposure value
        float initialExposure = skyboxMaterial.GetFloat("_Exposure");

        // Tween the exposure property
        cinematicSequence.Append(DOTween.To(() => initialExposure, x => skyboxMaterial.SetFloat("_Exposure", x), 8, 5)
            .SetEase(Ease.InOutQuad));
        cinematicSequence.Join(DOTween.To(() => skyboxMaterial.GetColor("_Tint"), x => skyboxMaterial.SetColor("_Tint", x), Color.white, 5f));
        // Move bars in and fade in title
        cinematicSequence.Join(cinematicBarTop.DOAnchorPosY(cinematicBarTopInitialPos.y - cinematicBarTop.rect.height, 3f).SetEase(Ease.OutQuad)); // Move top bar down
        cinematicSequence.Join(cinematicBarBottom.DOAnchorPosY(cinematicBarBottomInitialPos.y + cinematicBarBottom.rect.height, 3f).SetEase(Ease.OutQuad)); // Move bottom bar up
        cinematicSequence.Append(titleText.DOFade(1, 2f)); // Fade in the title
        cinematicSequence.Append(logoImage.DOFade(1, 3));

    }
}
