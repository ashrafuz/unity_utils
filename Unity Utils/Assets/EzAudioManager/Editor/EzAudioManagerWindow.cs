using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EzAudio {

    public class EzAudioManagerWindow : EditorWindow {
        static List<string> allAudioAssetName = new List<string> ();
        static List<AudioClip> allAudioClip = new List<AudioClip> ();
        private static WWW audioLoader = null;
        private static bool shouldStackClips = false;
        public const string EZ_AUDIO_MANAGER_DIR = "Assets/EzAudioManager/";

        [MenuItem ("Window/EzAudioManager")]
        static void OpenWindow () {
            EzAudioManagerWindow window = (EzAudioManagerWindow) GetWindow (typeof (EzAudioManagerWindow));
            window.minSize = new Vector2 (300, 300);
            window.Show ();
        }

        private void OnGUI () {
            if (GUILayout.Button ("Generate Audio Book", GUILayout.Height (40))) {
                shouldStackClips = true;
                currentlyStacked = 0;
                GetAllAudioFileNames ();
            }
        }

        private int currentlyStacked = 0;
        private void Update () {
            if (shouldStackClips && currentlyStacked < allAudioAssetName.Count) {
                if (audioLoader == null) {
                    string currentFileName = allAudioAssetName[currentlyStacked];
                    audioLoader = new WWW (currentFileName);
                    Debug.Log ("loading >> " + allAudioAssetName[currentlyStacked]);
                } else if (audioLoader.isDone) {
                    allAudioClip.Add (audioLoader.GetAudioClip ());
                    currentlyStacked++;
                    audioLoader = null;
                }

                if (currentlyStacked == allAudioAssetName.Count) { // last iteratiooon
                    Debug.Log ("loaded all audio assets...total:: " + allAudioAssetName.Count);
                    AudioPackCreator<EzAudioFiles> audioPackCreator = new AudioPackCreator<EzAudioFiles> (allAudioClip, allAudioAssetName);
                    shouldStackClips = false;
                    CreateAudioBook ();
                }
            }
        }

        static void GetAllAudioFileNames () {
            allAudioAssetName.Clear ();
            allAudioClip.Clear ();
            string[] guids = AssetDatabase.FindAssets ("t:audioClip");
            foreach (string guid in guids) {
                string fname = AssetDatabase.GUIDToAssetPath (guid);
                allAudioAssetName.Add (fname);
            }
        }

        static void CreateAudioBook () {
            EzAudioBook ezbook = ScriptableObject.CreateInstance<EzAudioBook> ();
            ezbook.AddBook (allAudioAssetName, ref allAudioClip);
            string fullName = EZ_AUDIO_MANAGER_DIR + "EzAudioBook.asset";
            AssetDatabase.CreateAsset (ezbook, fullName);
            AssetDatabase.SaveAssets ();

            AssetDatabase.Refresh ();

            EditorUtility.FocusProjectWindow ();
            Selection.activeObject = ezbook;
        }

    }

    /*==================*/

    public class AudioPackCreator<T> {
        public Dictionary<string, AudioClip> audioBundle = new Dictionary<string, AudioClip> ();
        public AudioPackCreator (List<AudioClip> allClips, List<string> allNames) {
            string classPath = EzAudioManagerWindow.EZ_AUDIO_MANAGER_DIR + typeof (T) + ".cs";
            int invalidClipNameCounter = 0;
            if (File.Exists (classPath)) { File.Delete (classPath); } // delete previous file
            using (StreamWriter outfile = new StreamWriter (classPath)) {
                outfile.WriteLine ("public enum " + typeof (T) + " { ");

                for (int i = 0; i < allClips.Count; i++) {
                    outfile.Write ("\t\t");

                    string clipname = Path.GetFileNameWithoutExtension (allNames[i]).ToUpper ();
                    if (!IsIdentifier (clipname)) {
                        clipname = "INVALID_NAME_" + invalidClipNameCounter + " /* Invalidated name => " + allNames[i] + " =>*/";
                        invalidClipNameCounter++;
                    }

                    if (!audioBundle.ContainsKey (clipname)) {
                        outfile.Write (clipname + " = " + i);
                        audioBundle.Add (clipname, allClips[i]);
                    }

                    if (i != allClips.Count - 1) { outfile.Write (", \n"); }
                }

                outfile.WriteLine ("\n}");
            } //file writtten

            AssetDatabase.Refresh ();
        }

        public Dictionary<string, AudioClip> GetAudioPool () { return audioBundle; }

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
}