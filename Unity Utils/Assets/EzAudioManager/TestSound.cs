using System.Collections;
using System.Collections.Generic;
using EzAudio;
using UnityEngine;

public class TestSound : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {
        EzAudioSystem.instance.PlayClip (EzAudioFiles.SFX_BREAKSHOT_8);
    }

    // Update is called once per frame
    void Update () {

    }
}