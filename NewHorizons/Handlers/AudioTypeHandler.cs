using NewHorizons.Utility;
using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = NewHorizons.Utility.Logger;

namespace NewHorizons.Handlers
{
    public static class AudioTypeHandler
    {
        private static Dictionary<string, AudioType> _customAudioTypes;
        private static List<AudioLibrary.AudioEntry> _audioEntries;
        private static int _startingInt = 4000;

        public static void Init()
        {
            _customAudioTypes = new Dictionary<string, AudioType>();
            _audioEntries = new List<AudioLibrary.AudioEntry>();

            Delay.RunWhen(
                () => Locator.GetAudioManager()?._libraryAsset != null,
                PostInit
            );
        }

        private static void PostInit()
        {
            Logger.LogVerbose($"Adding all custom AudioTypes to the library");

            var library = Locator.GetAudioManager()._libraryAsset;
            var audioEntries = library.audioEntries; // store previous array
            library.audioEntries = library.audioEntries.Concat(_audioEntries).ToArray(); // concat custom entries
            Locator.GetAudioManager()._audioLibraryDict = library.BuildAudioEntryDictionary();
            library.audioEntries = audioEntries; // reset it back for next time we build
        }

        // Will return an existing audio type or create a new one for the given audio string
        public static AudioType GetAudioType(string audio, IModBehaviour mod)
        {
            try
            {
                if (audio.Contains(".wav") || audio.Contains(".mp3") || audio.Contains(".ogg"))
                {
                    return AddCustomAudioType(audio, mod);
                }
                else
                {
                    return (AudioType)Enum.Parse(typeof(AudioType), audio);
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"Couldn't load AudioType:\n{e}");
                return AudioType.None;
            }
        }

        // Create a custom audio type from relative file path and the mod
        public static AudioType AddCustomAudioType(string audioPath, IModBehaviour mod)
        {
            AudioType audioType;

            var id = mod.ModHelper.Manifest.UniqueName + "_" + audioPath;
            if (_customAudioTypes.TryGetValue(id, out audioType)) return audioType;

            var audioClip = AudioUtilities.LoadAudio(mod.ModHelper.Manifest.ModFolderPath + "/" + audioPath);

            if (audioClip == null)
            {
                Logger.LogError($"Couldn't create audioType for {audioPath}");
                return AudioType.None;
            }

            return AddCustomAudioType(id, new AudioClip[] { audioClip });
        }

        // Create a custom audio type from a set of audio clips. Needs a unique ID
        public static AudioType AddCustomAudioType(string id, AudioClip[] audioClips)
        {
            var audioType = (AudioType)_startingInt + _customAudioTypes.Count();

            Logger.LogVerbose($"Registering custom audio type {id} as {audioType}");

            _audioEntries.Add(new AudioLibrary.AudioEntry(audioType, audioClips));
            _customAudioTypes.Add(id, audioType);

            return audioType;
        }
    }
}
