using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EzAudioManagerWindow : EditorWindow {
    static List<string> allAudioAssetName = new List<string> ();

    [MenuItem ("Window/EzAudioManager")]
    static void OpenWindow () {
        EzAudioManagerWindow window = (EzAudioManagerWindow) GetWindow (typeof (EzAudioManagerWindow));
        window.minSize = new Vector2 (300, 300);

        window.Show ();

    }

    private void OnGUI () {
        if (GUILayout.Button ("Print All Audio Asset", GUILayout.Height (40))) {
            GetAllAudioAssets ();
        }
    }

    static void GetAllAudioAssets () {
        string[] guids = AssetDatabase.FindAssets ("t:audioClip");
        foreach (string guid in guids) {
            string fname = AssetDatabase.GUIDToAssetPath (guid);
            string rawFileName = Path.GetFileNameWithoutExtension (fname);
            allAudioAssetName.Add (rawFileName.ToUpper ());
        }

        AudioClassCreator acc = new AudioClassCreator ("EzAudioManagerPack", allAudioAssetName);
    }

}