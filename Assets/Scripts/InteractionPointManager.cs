using UnityEngine;

public class InteractionPointManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject textUI; // The text UI to activate/deactivate
    
    [Header("Interaction Point Story")]
    public string storyText; // The text UI to activate/deactivate
    
    private bool playerInside = false; // Tracks if the player is inside the collider

    private void Start()
    {
        // Ensure the text UI is deactivated at the start
        if (textUI != null)
        {
            textUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            // Activate the text UI
            if (textUI != null)
            {
                textUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            // Deactivate the text UI
            if (textUI != null)
            {
                textUI.SetActive(false);
            }

            // Perform an action when the player exits the collider
            OnPlayerExit();
        }
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            // Check the cinematic state
            if (GameManager.Instance is not null && !GameManager.Instance.isCurrentlyCinematic)
            {
                // Perform an action if not cinematic
                OnPlayerInteract();
            }
            else
            {
                // Return if currently cinematic
                return;
            }
        }
    }

    private void OnPlayerInteract()
    {
        GameManager.Instance.InteractWithStory(storyText);
    }

    private void OnPlayerExit()
    {
        GameManager.Instance.EndInteractionWithStory();
    }
}
