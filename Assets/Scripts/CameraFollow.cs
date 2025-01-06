using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    [Header("References")]
    public Transform ball; // The ball to animate
    public Transform groundTransform;
    public Vector3 finalCamPos; // The target position for the ball to fall
    public Volume postProcessVolume; // Post-processing volume
    public PlayerController playerController;
    public Rigidbody rigidbody;
    private MotionBlur motionBlur;
    private bool hasReached;

    [Header("Animation Settings")]
    public float scaleDuration = 1f; // Duration for scaling the ball
    public float fallDuration = 2f;
    private float cameraTweenDuration = 2f; // Example duration// Duration for the ball to fall
    public Vector3 cameraOffset = new Vector3(0, 5, -10); // Offset of the camera relative to the target

    private bool isCinematicPlaying = true; // Tracks if the cinematic is active
   
    
	public float smoothSpeed = 0.125f;
	public Vector3 offset;
	Vector3 velocityRef = Vector3.zero;
	
	private void Start()
	{
		// Ensure the ball starts at scale zero
		ball.localScale = Vector3.zero;
		playerController.enabled = false;

		// Enable motion blur if available
		if (postProcessVolume != null && postProcessVolume.profile.TryGet(out motionBlur))
		{
			motionBlur.active = true;
		}

		// Start the cinematic sequence
		PlayCinematic();
	}
	
	
	private void PlayCinematic()
	{
		// Create a sequence for the cinematic
		Sequence cinematicSequence = DOTween.Sequence();

		cinematicSequence.AppendInterval(10);
		// Scale the ball from 0 to 1
		cinematicSequence.Append(ball.DOScale(Vector3.one, scaleDuration).SetEase(Ease.InSine));

		// Move the ball to the ground position
		cinematicSequence.Append(ball.DOMove(groundTransform.position, fallDuration).SetEase(Ease.InQuad));
		cinematicSequence.AppendCallback(Reached);
		cinematicSequence.Append(transform.DOMove(finalCamPos, cameraTweenDuration).SetEase(Ease.OutQuad));
		cinematicSequence.Join(transform.DORotate(Vector3.zero, cameraTweenDuration)).SetEase(Ease.OutQuad);
		cinematicSequence.AppendInterval(0.4f);
		// After the cinematic ends, start gameplay
		cinematicSequence.OnComplete(() => EndCinematic());
	}

	private void EndCinematic()
	{
		isCinematicPlaying = false;
		playerController.enabled = true;

		// Disable motion blur
		if (motionBlur != null)
		{
			motionBlur.active = false;
		}
	}
	
	
    	void LateUpdate ()
    	{
	        if (isCinematicPlaying && !hasReached)
	        {
		        // Make the camera follow the ball
		        FollowTarget(ball);
	        }
	        else if(!isCinematicPlaying)
	        {
		        FollowPlayer();
	        }
	        
        }
        private void FollowTarget(Transform target)
        {
	        if (target is not null)
	        {
		        transform.position = target.position + cameraOffset;
		        transform.LookAt(target);
	        }
        }
    	void FollowPlayer()
    	{
	        // Calculate desired position, maintaining current camera Y position
	        Vector3 desiredPosition = new Vector3(
		        target.position.x + offset.x,
		        transform.position.y, // Keep the camera's current Y position
		        target.position.z + offset.z
	        );

	        // Smooth the camera's position
	        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocityRef, smoothSpeed);
	        transform.position = smoothedPosition;

	        // Make the camera look at the target
	        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
    	}

        void Reached()
        {
	        hasReached = true;
	        rigidbody.isKinematic = false;
        }

       
}
