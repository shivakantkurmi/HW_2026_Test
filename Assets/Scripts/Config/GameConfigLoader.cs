// Author: Shivakant kurmi
// Summary: Downloads and parses the game's JSON configuration file.
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Doofus.Config
{
    // Reads Doofus's Diary (StreamingAssets/doofus_diary.json) and exposes the parsed values.
    // Falls back to sane defaults if the file is missing, unreachable, or malformed so the
    // game is always playable even when the config can't be read.
    public class GameConfigLoader : MonoBehaviour
    {
        private const string FileName = "doofus_diary.json";

        public static GameConfigLoader Instance { get; private set; }
        public GameConfig Config { get; private set; }
        public bool IsLoaded { get; private set; }

        public static event Action<GameConfig> OnConfigReady;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            StartCoroutine(LoadConfig());
        }

        private IEnumerator LoadConfig()
        {
            string path = Path.Combine(Application.streamingAssetsPath, FileName);
            GameConfig loaded = null;

            if (path.Contains("://"))
            {
                using UnityWebRequest request = UnityWebRequest.Get(path);
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    loaded = TryParse(request.downloadHandler.text);
                }
                
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Config = new GameConfig();
                    IsLoaded = true;
                }
            }
            else
            {
                try
                {
                    loaded = TryParse(File.ReadAllText(path));
                }
                catch (Exception)
                {
                    Config = new GameConfig();
                    IsLoaded = true;
                }
            }

            Config = loaded ?? new GameConfig();
            IsLoaded = true;
            OnConfigReady?.Invoke(Config);
        }

        private GameConfig TryParse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                Config = new GameConfig();
                IsLoaded = true;
                return null;
            }

            try
            {
                GameConfig parsed = JsonUtility.FromJson<GameConfig>(json);
                if (Config == null)
                {
                    Config = new GameConfig();
                }

                parsed.player_data ??= new PlayerData();
                parsed.pulpit_data ??= new PulpitData();
                return parsed;
            }
            catch (Exception)
            {
                Config = new GameConfig();
                return null;
            }
        }
    }
}
