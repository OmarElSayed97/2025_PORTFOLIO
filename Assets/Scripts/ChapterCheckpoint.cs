using System;
using UnityEngine;

public class ChapterCheckpoint : MonoBehaviour
{
    public int checkpointID;
    public Transform checkpointPosition;
    public string chapterSubtitle;
    private bool checkpointReached;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !checkpointReached)
        {
            GameManager.Instance.CheckpointReached(this);
            checkpointReached = true;
        }
    }
}
