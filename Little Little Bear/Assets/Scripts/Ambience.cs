using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambience : MonoBehaviour
{
    public AudioClip SoundClip;
    public AudioSource SoundSource;
    public int depth;
    PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {
        playerData = GameObject.Find("GlobalManager").GetComponent<GlobalMan>().data;
        SoundSource.clip = SoundClip;
        depth = playerData.depth;
    }

    // Update is called once per frame
    void Update()
    {
        //depth = GetComponent<LLB>().DungeonDepth;
        if (depth < 4)
        {
            SoundClip = Resources.Load<AudioClip>("Sounds/forest");
            SoundSource.volume = 0.1f;
            SoundSource.Play();
        }
        else if(depth < 7)
        {
            SoundClip = Resources.Load<AudioClip>("Sounds/pond");
            SoundSource.volume = 0.1f;
            SoundSource.Play();
        }
        else
        {
            SoundClip = Resources.Load<AudioClip>("Sounds/cave");
            SoundSource.volume = 0.05f;
            SoundSource.Play();
        }
    }
}
