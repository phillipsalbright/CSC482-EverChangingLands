using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] private AudioSource sunnyBGM;
    [SerializeField] private AudioSource rainyBGM;
    [SerializeField] private AudioSource droughtBGM;

    public enum BGMType { SUNNY, RAINY, DROUGHT }

    private BGMType backgroundMusicType;

    public void pauseBGM()
    {
        switch(backgroundMusicType)
        {
            case BGMType.SUNNY:
                sunnyBGM.Pause();
                break;
            case BGMType.RAINY:
                rainyBGM.Pause();
                break;
            case BGMType.DROUGHT:
                if (droughtBGM != null) droughtBGM.Pause();
                break;
        }
    }

    public void playBGM()
    {
        switch(backgroundMusicType)
        {
            case BGMType.SUNNY:
                sunnyBGM.Play();
                break;
            case BGMType.RAINY:
                rainyBGM.Play();
                break;
            case BGMType.DROUGHT:
                if (droughtBGM != null) droughtBGM.Play();
                break;
        }
    }

    public void setBGMType(BGMType type)
    {
        switch (backgroundMusicType)
        {
            case BGMType.SUNNY:
                sunnyBGM.Stop();
                break;
            case BGMType.RAINY:
                rainyBGM.Stop();
                break;
            case BGMType.DROUGHT:
                if(droughtBGM != null) droughtBGM.Stop();
                break;
        }

        backgroundMusicType = type;

        switch (backgroundMusicType)
        {
            case BGMType.SUNNY:
                sunnyBGM.Play();
                break;
            case BGMType.RAINY:
                rainyBGM.Play();
                break;
            case BGMType.DROUGHT:
                if (droughtBGM != null) droughtBGM.Play();
                break;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
