using DG.Tweening;
using UnityEngine;

public class CompanyManager : MonoBehaviour
{
    [Header("Spin Settings")]
    public Transform spinningChild; // The child transform to spin
    public int baseSpins = 3; // Base number of spins
    public float baseSpinDuration = 1f; // Base duration for the spins
    public float returnToOriginalDuration = 2f; // Duration for returning to the original rotation
    public int maxTriggerCount = 5; // Maximum times the player can trigger the collider

    private Quaternion originalRotation; // Stores the original rotation of the transform
    private int currentSpinCount = 0; // Keeps track of how many spins have been added
    private Tween spinTween; // Stores the current spin tween
    private int triggerCount = 0; // Tracks how many times the player has triggered
    
    [Header("Camera Shake Settings")]
    public Transform cameraTransform; // Reference to the camera's transform
    public float shakeDuration = 0.5f; // Duration of the shake
    public float shakeStrength = 0.5f; // Strength of the shake
    public int shakeVibrato = 10; // Number of shakes per second
    public float shakeRandomness = 90f; // Randomness of the shake direction
    
    [Header("Audio Settings")]
    public AudioSource audioSource; // Reference to the AudioSource
    public AudioClip shakeSound; // Sound to play when the shake starts
    
    [Header("Materials")]
    public Material baseMaterial;

    [Header("Transforms")]
    public Transform ballsParentTransform;

    [Header("Company Colors")]
    public Color companyBaseColor;
    public Color companyEmissionColor;

    [Header("Floating Settings")]
    public float floatRange = 0.5f;
    public float floatDuration = 2f;

    private Transform[] balls;
    private Material[] ballMaterials;
    private Material spinningChildMaterial;
    private int remainingHits;
    
    
    private void Start()
    {
        // Save the original rotation of the child transform
        if (spinningChild != null)
        {
            originalRotation = spinningChild.rotation;
        }
        InitializeBalls();
        InitializeSpinningChild();
    }
    
    private void InitializeBalls()
    {
        // Get all child transforms under the ballsParentTransform
        balls = new Transform[ballsParentTransform.childCount];
        ballMaterials = new Material[balls.Length];

        for (int i = 0; i < ballsParentTransform.childCount; i++)
        {
            balls[i] = ballsParentTransform.GetChild(i);

            // Create a new material instance for each ball based on the exposed baseMaterial
            ballMaterials[i] = new Material(baseMaterial);
            balls[i].GetComponent<Renderer>().material = ballMaterials[i];
        }

        // Set the remaining hits to the number of balls
        remainingHits = balls.Length;
    }

    private void InitializeSpinningChild()
    {
        // Create a new material for the spinning child based on the exposed baseMaterial
        spinningChildMaterial = new Material(baseMaterial);
        spinningChild.GetComponent<Renderer>().material = spinningChildMaterial;
    }
    
    public void OnBallHit()
    {
        if (remainingHits <= 0) return; // Ignore hits if already finished

        int currentBallIndex = balls.Length - remainingHits;
        //Transform currentBall = balls[currentBallIndex];
        Material currentBallMaterial = ballMaterials[currentBallIndex];

        // Set ball material colors and emission intensity
        currentBallMaterial.DOColor(companyBaseColor, "_BaseColor", 1f).SetEase(Ease.OutQuad);
        currentBallMaterial.DOColor(companyEmissionColor * 2.5f, "_EmissionColor", 1f).SetEase(Ease.Linear);
        remainingHits--;

        if (remainingHits == 0)
        {
            CompleteBossSequence();
        }
    }

    private void CompleteBossSequence()
    {
        Sequence sequence = DOTween.Sequence();

        // Animate each ball moving to the spinningChild
        foreach (Transform ball in balls)
        {
            sequence.Append(ball.DOMove(spinningChild.position, 1f).SetEase(Ease.InOutQuad));
            sequence.AppendCallback(() => Destroy(ball.gameObject)); // Destroy ball after reaching
        }

        // Tween the spinningChild material colors
        sequence.Append(spinningChildMaterial.DOColor(companyBaseColor, "_BaseColor", 1f).SetEase(Ease.Linear));
        sequence.Join(spinningChildMaterial.DOColor(companyEmissionColor * 2.5f, "_EmissionColor", 1f).SetEase(Ease.Linear));

        // Start floating animation for the spinningChild
        sequence.OnComplete(() => StartFloating());
    }

    private void StartFloating()
    {
        spinningChild.DOMoveY(spinningChild.position.y + floatRange, floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && remainingHits > 0)
        {
            // Shake the camera
            cameraTransform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness);
            // Play the sound
            if (audioSource != null && shakeSound != null)
            {
                audioSource.PlayOneShot(shakeSound);
            }
            if (triggerCount < maxTriggerCount)
            {
                triggerCount++;
                AddSpins(triggerCount <= maxTriggerCount ? 1 : 0); // Add 1 spin unless max count is reached
            }
            OnBallHit();
        }
    }
    
    private void AddSpins(int additionalSpins)
    {
        // Add spins to the total count
        currentSpinCount += additionalSpins;

        // Calculate the new spin duration
        float spinDuration = baseSpinDuration / (1 + (triggerCount - 1) * 0.2f); // Spin faster on consecutive triggers

        // If there’s an active spin tween, append to it
        if (spinTween != null && spinTween.IsActive() && !spinTween.IsComplete())
        {
            // Extend the existing spin
            spinTween.Kill(); // Stop the current tween
        }

        // Create a new spin tween
        spinTween = spinningChild.DORotate(
                new Vector3(0, 360 * currentSpinCount, 0),
                spinDuration * currentSpinCount,
                RotateMode.LocalAxisAdd
            )
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // Smoothly return to the original rotation at the end
                spinningChild.DORotateQuaternion(originalRotation, returnToOriginalDuration).SetEase(Ease.OutElastic);
                triggerCount = 0; // Reset the trigger count after the sequence
                currentSpinCount = 0; // Reset spin count
            });
    }
    
    
}
