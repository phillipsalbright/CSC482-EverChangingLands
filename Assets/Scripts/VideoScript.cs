using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoScript : MonoBehaviour
{
    [SerializeField] private string filename;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<VideoPlayer>().url = System.IO.Path.Combine(Application.streamingAssetsPath,filename);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
