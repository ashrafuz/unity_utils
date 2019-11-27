using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EzAudio {

    [System.Serializable]
    public class AudioPack {
        public int index;
        public EzAudioFiles rawEnum;
        public string fullPath;
        public string fileNameWithoutExtension;
        public AudioClip audioClip;
        public AudioPack (int _val, string _fp, AudioClip _ac) {
            index = _val;
            audioClip = _ac;
            fullPath = _fp;

            rawEnum = (EzAudioFiles) _val;
            fileNameWithoutExtension = Path.GetFileNameWithoutExtension (_fp);
        }
    }

    public class EzAudioBook : ScriptableObject {
        public List<AudioPack> audioBook = new List<AudioPack> ();

        public void AddBook (List<string> _allFiles, ref List<AudioClip> _aClips) {
            if (_allFiles.Count != _aClips.Count) {
                Debug.LogError ("All audio file did not load. Can't Create asset");
                return;
            }

            audioBook.Clear ();
            for (int i = 0; i < _allFiles.Count; i++) {
                audioBook.Add (new AudioPack (i, _allFiles[i], _aClips[0]));
            }
        }

        public AudioClip GetClip (EzAudioFiles _file) {
            int index = (int) _file;
            if (index < audioBook.Count) {
                return audioBook[index].audioClip;
            }

            Debug.LogError ("can't find audio file " + _file);
            return null;
        }
    }
}