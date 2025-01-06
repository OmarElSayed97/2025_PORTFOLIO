using System.Linq;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayersManager : MonoBehaviour
{
    public VideoPlayer[] videoPlayers;

    private string videoName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (VideoPlayer videoPlayer in videoPlayers)
        {
            videoPlayer.url = System.IO.Path.Combine (Application.streamingAssetsPath,GetVideoName(videoPlayer.targetTexture.name) + ".mp4");
            videoPlayer.Play();
        }
    }

    string GetVideoName(string texName)
    {
        videoName = "";
        videoName += texName.Substring(0,2) + "-Vid-" + texName[texName.Length-1];
        return videoName;
    }
}
