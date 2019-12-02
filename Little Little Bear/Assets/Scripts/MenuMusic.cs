using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    public AudioClip MenuClip;
    public AudioSource SoundSource;
    PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {
        MenuClip = Resources.Load<AudioClip>("Sounds/menuMusic");
        SoundSource.clip = MenuClip;
        SoundSource.volume = 0.1f;
        SoundSource.loop = true;
        SoundSource.Play();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
