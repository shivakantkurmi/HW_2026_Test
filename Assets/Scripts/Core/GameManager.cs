// Author: Shivakant kurmi
// Summary: The central state machine that controls the game's flow (Start, Playing, Game Over).
using UnityEngine;
using Doofus.Player;
using Doofus.Pulpits;

namespace Doofus.Core
{
    public enum GameState
    {
        StartScreen,
        Playing,
        GameOver
    }

    // Top-level state machine: StartScreen -> Playing -> GameOver -> (retry) -> Playing.
    // Orchestrates resetting/positioning Doofus and (re)starting the pulpit spawner;
    // everything else reacts to GameEvents rather than being driven directly.
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private DoofusController doofus;
        [SerializeField] private PulpitSpawner spawner;
        [SerializeField] private float doofusSpawnHeightOffset = 1f;

        public GameState State { get; private set; } = GameState.StartScreen;

        private void OnEnable()
        {
            GameEvents.OnPlayerFell += HandlePlayerFell;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerFell -= HandlePlayerFell;
        }

        public void StartGame()
        {
            if (State == GameState.Playing) return;
            if (doofus == null || spawner == null)
            {
                return;
            }

            State = GameState.Playing;

            Vector3 spawnPos = spawner.GetOriginWorldPosition() + Vector3.up * doofusSpawnHeightOffset;
            doofus.ResetState(spawnPos);

            GameEvents.RaiseGameStart();
        }

        public void RestartGame()
        {
            State = GameState.StartScreen;
            GameEvents.RaiseGameReset();
        }

        private void HandlePlayerFell()
        {
            if (State != GameState.Playing) return;
            State = GameState.GameOver;
        }
    }
}
