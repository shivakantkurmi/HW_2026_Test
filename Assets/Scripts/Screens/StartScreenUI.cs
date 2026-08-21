using UnityEngine;
using UnityEngine.UI;
using Doofus.Core;

namespace Doofus.Screens
{
    public class StartScreenUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button startButton;
        [SerializeField] private GameManager gameManager;

        private void Awake()
        {
            Debug.Log("[StartScreenUI] Awake.");
            if (startButton != null) startButton.onClick.AddListener(HandleStartClicked);

            // Subscribed here rather than OnEnable for consistency with GameplayHUD/
            // GameOverUI, which must subscribe in Awake since they deactivate their own
            // GameObject there (which would otherwise skip OnEnable on the first pass).
            GameEvents.OnGameStart += HandleGameStart;
            GameEvents.OnGameReset += HandleGameReset;

            if (panel != null) panel.SetActive(true);
        }

        private void HandleGameReset()
        {
            Debug.Log("[StartScreenUI] HandleGameReset: Showing panel.");
            if (panel != null) panel.SetActive(true);
        }

        private void HandleStartClicked()
        {
            Debug.Log("[StartScreenUI] HandleStartClicked: Hiding panel and starting game.");
            if (panel != null) panel.SetActive(false);
            if (gameManager != null) gameManager.StartGame();
        }

        private void HandleGameStart()
        {
            Debug.Log("[StartScreenUI] HandleGameStart: Hiding panel.");
            if (panel != null) panel.SetActive(false);
        }
    }
}
