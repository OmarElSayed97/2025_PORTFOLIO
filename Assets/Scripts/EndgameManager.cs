using System;
using UnityEngine;

public class EndgameManager : MonoBehaviour
{
    public PlayerController playerController;

    public Rigidbody rb;
    private bool gameEnded;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameEnded) return;
        
            gameEnded = true;
            playerController.enabled = false;
            rb.isKinematic = true;
            GameManager.Instance.EndGame();
        }
       
        
        
    }
}
