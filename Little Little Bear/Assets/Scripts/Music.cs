using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioClip MusicClip;
    public AudioSource MusicSource;
    public int depth;

    // Start is called before the first frame update
    void Start()
    {
        MusicSource.clip = MusicClip;
        depth = GetComponent<LLB>().DungeonDepth;
    }

    // Update is called once per frame
    void Update()
    {
        if(depth < 4)
        {
            //MusicSource = Resources.Load<AudioSource>("/Sounds/forest1");
            //MusicSource.Play();
        }
    }
}
