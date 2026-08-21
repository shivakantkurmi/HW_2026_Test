using UnityEngine;
using UnityEngine.UI;
using Doofus.Core;

namespace Doofus.Screens
{
    public class GameplayHUD : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Text scoreText;

        private void Awake()
        {
            Debug.Log("[GameplayHUD] Awake.");
            // Subscribing here (not OnEnable) is deliberate: 'panel' is this same
            // GameObject, and deactivating it below would otherwise skip OnEnable
            // entirely on this first pass, silently dropping these subscriptions.
            GameEvents.OnScoreChanged += HandleScoreChanged;
            GameEvents.OnGameStart += HandleGameStart;
            GameEvents.OnGameOver += HandleGameOver;
            GameEvents.OnGameReset += HandleGameReset;

            if (panel != null) panel.SetActive(false);
        }

        private void HandleGameReset()
        {
            Debug.Log("[GameplayHUD] HandleGameReset: Hiding panel.");
            if (panel != null) panel.SetActive(false);
        }

        private void HandleScoreChanged(int score)
        {
            Debug.Log($"[GameplayHUD] HandleScoreChanged: Score = {score}");
            if (scoreText != null) scoreText.text = $"{score}";
        }

        private void HandleGameStart()
        {
            Debug.Log("[GameplayHUD] HandleGameStart: Showing panel.");
            if (panel != null) panel.SetActive(true);
            HandleScoreChanged(0);
        }

        private void HandleGameOver()
        {
            Debug.Log("[GameplayHUD] HandleGameOver: Hiding panel.");
            if (panel != null) panel.SetActive(false);
        }
    }
}
