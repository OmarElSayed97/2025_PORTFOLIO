using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    
    	public float smoothSpeed = 0.125f;
    	public Vector3 offset;
        Vector3 velocityRef = Vector3.zero;
    	void LateUpdate ()
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
}
