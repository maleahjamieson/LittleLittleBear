﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioClip ForestClip;
    public AudioClip SwampClip;
    public AudioClip CaveClip;
    public AudioClip BearClip;
    public AudioSource SoundSource;
    public int depth;
    PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {
        ForestClip = Resources.Load<AudioClip>("Sounds/forestMusic");
        SwampClip = Resources.Load<AudioClip>("Sounds/swampMusic");
        CaveClip = Resources.Load<AudioClip>("Sounds/caveMusic");
        BearClip = Resources.Load<AudioClip>("Sounds/bearMusic");
        depth = GameObject.Find("Player").GetComponent<LLB>().DungeonDepth;

        if (depth < 2)
        {
            SoundSource.clip = ForestClip;
            SoundSource.volume = 0.1f;
            SoundSource.loop = true;
            SoundSource.Play();
        }
        else if (depth < 3)
        {
            SoundSource.clip = SwampClip;
            SoundSource.volume = 0.1f;
            SoundSource.loop = true;
            SoundSource.Play();
        }
        else if (depth < 4)
        {
            SoundSource.clip = CaveClip;
            SoundSource.volume = 0.25f;
            SoundSource.loop = true;
            SoundSource.Play();
        }
        else
        {
            SoundSource.clip = BearClip;
            SoundSource.volume = 0.1f;
            SoundSource.loop = true;
            SoundSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        depth = GameObject.Find("Player").GetComponent<LLB>().DungeonDepth;

        if (depth < 2)
        {
            if (SoundSource.clip != ForestClip)
            {
                SoundSource.clip = ForestClip;
                SoundSource.volume = 0.1f;
                SoundSource.loop = true;
                SoundSource.Play();
            }
        }
        else if (depth < 3)
        {
            if (SoundSource.clip != SwampClip)
            {
                SoundSource.clip = SwampClip;
                SoundSource.volume = 0.1f;
                SoundSource.loop = true;
                SoundSource.Play();
            }
        }
        else if (depth < 4)
        {
            if (SoundSource.clip != CaveClip)
            {
                SoundSource.clip = CaveClip;
                SoundSource.volume = 0.25f;
                SoundSource.loop = true;
                SoundSource.Play();
            }
        }
        else
        {
            if (SoundSource.clip != BearClip)
            {
                SoundSource.clip = BearClip;
                SoundSource.volume = 0.1f;
                SoundSource.loop = true;
                SoundSource.Play();
            }
        }
    }
}
