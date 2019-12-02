using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambience : MonoBehaviour
{
    public AudioClip ForestClip;
    public AudioClip SwampClip;
    public AudioClip CaveClip;
    public AudioSource SoundSource;
    public int depth;
    PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {
        // playerData = GameObject.Find("GlobalManager").GetComponent<GlobalMan>().data;
        ForestClip = Resources.Load<AudioClip>("Sounds/forest");
        SwampClip = Resources.Load<AudioClip>("Sounds/pond");
        CaveClip = Resources.Load<AudioClip>("Sounds/cave");
        depth = GameObject.Find("Player").GetComponent<LLB>().DungeonDepth;

        if (depth < 2)
        {
            SoundSource.clip = ForestClip;
            SoundSource.volume = 0.04f;
            SoundSource.loop = true;
            SoundSource.Play();
        }
        else if (depth < 3)
        {
            SoundSource.clip = SwampClip;
            SoundSource.volume = 0.02f;
            SoundSource.loop = true;
            SoundSource.Play();
        }
        else
        {
            SoundSource.clip = CaveClip;
            SoundSource.volume = 0.05f;
            SoundSource.loop = true;
            SoundSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // asdf
    }
}
