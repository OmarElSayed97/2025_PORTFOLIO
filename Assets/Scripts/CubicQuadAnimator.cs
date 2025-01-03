using UnityEngine;
using DG.Tweening; // Import DOTween namespace
public class CubicQuadAnimator : MonoBehaviour
{
    [Header("Movement Range (Local X Axis)")]
    public Vector2 movementRange = new Vector2(-5f, 5f);

    [Header("Speed Range (Duration)")]
    public Vector2 speedRange = new Vector2(1f, 3f);

    private float unifiedDuration;

    private void Start()
    {
        // Randomize a single duration for all children
        unifiedDuration = Random.Range(speedRange.x, speedRange.y);

        // Loop through all child transforms
        foreach (Transform child in transform)
        {
            StartTween(child);
        }
    }

    private void StartTween(Transform child)
    {
        // Generate a random movement distance
        float randomMovement = Random.Range(movementRange.x, movementRange.y);
        
        float randomDuration = Random.Range(speedRange.x, speedRange.y);
        
        // Define the target positions
        Vector3 startPosition = child.localPosition;
        Vector3 targetPosition = startPosition + new Vector3(randomMovement, 0f, 0f);

        // Start tweening with DOTween
        child.DOLocalMove(targetPosition, randomDuration)
            .SetEase(Ease.InOutSine) // Smooth easing
            .SetLoops(-1, LoopType.Yoyo); // Infinite loop with yoyo effect
    }
}
