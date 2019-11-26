using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AudioClassCreator {
    public Dictionary<string, AudioClip> audioBundle = new Dictionary<string, AudioClip> ();
    public AudioClassCreator (string _className, List<AudioClip> allClipNames) {
        if (!IsIdentifier (_className)) {
            Debug.LogError ("not a valid identifier");
            return;
        }

        string classPath = "Assets/AudioManager/" + _className + ".cs";
        int invalidClipNameCounter = 0;
        if (File.Exists (classPath)) { File.Delete (classPath); } // delete previous file
        using (StreamWriter outfile = new StreamWriter (classPath)) {
            outfile.WriteLine ("public enum " + _className + " { ");

            for (int i = 0; i < allClipNames.Count; i++) {
                outfile.Write ("\t\t");

                string clipname = allClipNames[i].name;

                if (!IsIdentifier (clipname)) {
                    clipname = invalidClipNameCounter + "";
                    invalidClipNameCounter++;
                }

                if (!audioBundle.ContainsKey (clipname)) {
                    outfile.WriteLine (clipname);
                    //audioBundle.Add (clipname, );
                }

                if (i != allClipNames.Count - 1) { outfile.Write (", \n"); }
            }

            outfile.WriteLine ("}");
        } //file writtten

        AssetDatabase.Refresh ();
    }

    public void AddToPool (string fileName) { }

    public static bool IsIdentifier (string text) {
        if (string.IsNullOrEmpty (text))
            return false;
        if (!char.IsLetter (text[0]) && text[0] != '_')
            return false;
        for (int ix = 1; ix < text.Length; ++ix)
            if (!char.IsLetterOrDigit (text[ix]) && text[ix] != '_')
                return false;
        return true;
    }

}