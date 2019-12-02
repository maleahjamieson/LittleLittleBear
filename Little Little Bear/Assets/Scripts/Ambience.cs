using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambience : MonoBehaviour
{
    public AudioClip SoundClip;
    public AudioSource SoundSource;
    public int depth;

    // Start is called before the first frame update
    void Start()
    {
        SoundSource.clip = SoundClip;
        SoundSource.volume = 0.1f;
        depth = GetComponent<LLB>().DungeonDepth;
    }

    // Update is called once per frame
    void Update()
    {
        if(depth < 4)
        {
            SoundClip = Resources.Load<AudioClip>("Sounds/forest");
            SoundSource.Play();
        }
        else if(depth < 7)
        {
            SoundClip = Resources.Load<AudioClip>("Sounds/pond");
            SoundSource.Play();
        }
        else
        {
            SoundClip = Resources.Load<AudioClip>("Sounds/cave");
            SoundSource.Play();
        }
    }
}
