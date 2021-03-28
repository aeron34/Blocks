using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Music_Player : MonoBehaviour
{
    public AudioClip[] music = new AudioClip[3] { null, null, null };
    public AudioSource audio;
    System.Random random = new System.Random();
    public AudioClip explode;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    void Awake()
    {


        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (!audio.isPlaying)
        {
            audio.clip = music[random.Next(3)];
            audio.Play();
        }
    }
}
