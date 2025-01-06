using TMPro;
using UnityEngine;

public class InteractionPointManager : MonoBehaviour
{
    [Header("Interaction Point Info")] 
    public int interactionPointID;
    public string storyText; // The text UI to activate/deactivate
    public bool isEvolved;

    [HideInInspector] public bool isJustEvolved;
    
    GameObject textUI; // The text UI to activate/deactivate
    private bool playerInside = false; // Tracks if the player is inside the collider
    private Renderer childMaterial;
    public Material evolvedMaterial;

    private string string1 = "Press E To Interact";
    private string string2 = "Press Shift To Channel";

    private void Start()
    {
        childMaterial = transform.GetChild(0).GetComponent<Renderer>();
        if (isEvolved)
            childMaterial.material = evolvedMaterial;
        textUI = GameManager.Instance.interactionLabel;
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
            if (isEvolved)
                textUI.GetComponent<TextMeshProUGUI>().text = string1;
            else
            {
                textUI.GetComponent<TextMeshProUGUI>().text = string2;
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
                textUI.GetComponent<TextMeshProUGUI>().text = "";
                textUI.SetActive(false);
            }
            GameManager.Instance.StopAllCoroutines();
            // Perform an action when the player exits the collider
            OnPlayerExit();
        }
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E) && isEvolved)
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

        if (playerInside && isJustEvolved)
        {
            isJustEvolved = false;
            childMaterial.material = evolvedMaterial;
            OnPlayerInteract();
            isEvolved = true;
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
