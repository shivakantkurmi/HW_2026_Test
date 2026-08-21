using UnityEngine;
using Doofus.Core;

namespace Doofus.Screens
{
    // Place this script on any initial ground or menu platforms you want to disappear
    // the moment the player clicks "Start".
    public class HideOnGameStart : MonoBehaviour
    {
        private void OnEnable()
        {
            GameEvents.OnGameStart += HandleGameStart;
            GameEvents.OnGameReset += HandleGameReset;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStart -= HandleGameStart;
            GameEvents.OnGameReset -= HandleGameReset;
        }

        private void HandleGameStart()
        {
            gameObject.SetActive(false);
        }

        private void HandleGameReset()
        {
            gameObject.SetActive(true);
        }
    }
}
