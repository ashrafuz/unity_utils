using System.Collections;
using System.Collections.Generic;
using EzAudio;
using UnityEngine;

public class TestEzSound : MonoBehaviour {

    [SerializeField] AudioSource audioSource;
    [SerializeField] EzAudioBook ezbook;
    void Start () {
        AudioClip ac = ezbook.GetClip (EzAudioFiles.SFX_BREAKSHOT_8);
        if (ac != null) {
            audioSource.PlayOneShot (ezbook.GetClip (EzAudioFiles.SFX_BREAKSHOT_8));
        } else {
            Debug.LogError ("no clip found");
        }
    }

    // Update is called once per frame
    void Update () {

    }
}