using System;
using DG.Tweening;
using UnityEngine;

public class GameoverHandler : MonoBehaviour
{
    public PlayerController playerController;
    public Rigidbody rb;
    public Transform playerTransform,respawnTransform,platformTransform;
    public Vector3 platformEndPos;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController.enabled = false;
            rb.isKinematic = true;
            Sequence sequence = DOTween.Sequence();

            sequence.Append(playerTransform.DOMove(respawnTransform.position, 2).SetEase(Ease.OutQuad));
            sequence.AppendInterval(6);
            sequence.Append(platformTransform.DOLocalMove(platformEndPos, 3)).SetEase(Ease.OutQuad);
            sequence.OnComplete(OnGameover);
        }
    }

    void OnGameover()
    {
        playerController.enabled = true;
        rb.isKinematic = false;
    }
}
