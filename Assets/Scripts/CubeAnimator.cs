using DG.Tweening;
using UnityEngine;

public class CubeAnimator : MonoBehaviour
{
    [Header("Rotation Speed")]
    public float rotationSpeed = 1f;

    private void Start()
    {
        StartRotation();
    }

    private void StartRotation()
    {
        // Generate random rotation angles for each axis
        Vector3 randomRotation = new Vector3(
            Random.Range(0f, 360f),
            Random.Range(0f, 360f),
            Random.Range(0f, 360f)
        );

        // Start rotation using DOTween
        transform.DORotate(randomRotation, rotationSpeed, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear) // Linear easing for continuous rotation
            .SetLoops(-1, LoopType.Incremental); // Infinite incremental loops
    }
}
