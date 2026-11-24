using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CyberEscape.Core
{
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver
    }

    /// <summary>
    /// GameManager: Singleton managing game state, score, speed scaling, and power-up durations.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("State")]
        [SerializeField] private GameState currentState = GameState.Playing;
        public GameState CurrentState => currentState;

        [Header("Score Settings")]
        [SerializeField] private float scoreMultiplier = 10f;
        [SerializeField] private int coinValue = 100;
        private float distanceTravelled = 0f;
        private int coinsCollected = 0;
        private int currentScore = 0;
        private int highScore = 0;

        [Header("Speed & Difficulty Progression")]
        [SerializeField] private float initialSpeed = 12f;
        [SerializeField] private float maxSpeed = 30f;
        [SerializeField] private float speedIncreaseRate = 0.25f; // speed added per second
        private float currentSpeed;

        [Header("Power-up States")]
        private bool hasShield = false;
        private float magnetTimer = 0f;
        private float slowMoTimer = 0f;

        // Events
        public event Action<int> OnScoreUpdated;
        public event Action<int> OnCoinsUpdated;
        public event Action<GameState> OnStateChanged;
        public event Action<bool> OnShieldChanged;
        public event Action<float> OnMagnetTimeRemaining;

        public int CurrentScore => currentScore;
        public int HighScore => highScore;
        public int CoinsCollected => coinsCollected;
        public float CurrentSpeed => currentSpeed;
        public bool HasShield => hasShield;
        public bool IsMagnetActive => magnetTimer > 0f;
        public bool IsSlowMoActive => slowMoTimer > 0f;

        private const string HIGH_SCORE_KEY = "CyberEscape_HighScore";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
            currentSpeed = initialSpeed;
        }

        private void Start()
        {
            SetState(GameState.Playing);
        }

        private void Update()
        {
            if (currentState != GameState.Playing) return;

            // Handle pause toggle
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                TogglePause();
            }

            // Power-up timers
            if (magnetTimer > 0f)
            {
                magnetTimer -= Time.deltaTime;
                OnMagnetTimeRemaining?.Invoke(Mathf.Max(0f, magnetTimer));
            }

            if (slowMoTimer > 0f)
            {
                slowMoTimer -= Time.deltaTime;
            }

            // Speed progression
            float targetSpeedRate = (slowMoTimer > 0f) ? 0.6f : 1.0f;
            currentSpeed = Mathf.Min(maxSpeed, currentSpeed + speedIncreaseRate * Time.deltaTime * targetSpeedRate);

            // Distance score progression
            distanceTravelled += currentSpeed * Time.deltaTime;
            currentScore = Mathf.FloorToInt(distanceTravelled * scoreMultiplier / 10f) + (coinsCollected * coinValue);
            OnScoreUpdated?.Invoke(currentScore);
        }

        public void AddCoins(int amount = 1)
        {
            coinsCollected += amount;
            currentScore += coinValue * amount;
            OnCoinsUpdated?.Invoke(coinsCollected);
            OnScoreUpdated?.Invoke(currentScore);
        }

        public void ActivateShield()
        {
            hasShield = true;
            OnShieldChanged?.Invoke(true);
        }

        public bool ConsumeShield()
        {
            if (hasShield)
            {
                hasShield = false;
                OnShieldChanged?.Invoke(false);
                return true;
            }
            return false;
        }

        public void ActivateMagnet(float duration = 8f)
        {
            magnetTimer = Mathf.Max(magnetTimer, duration);
        }

        public void ActivateSlowMotion(float duration = 5f)
        {
            slowMoTimer = Mathf.Max(slowMoTimer, duration);
        }

        public void GameOver()
        {
            if (currentState == GameState.GameOver) return;

            SetState(GameState.GameOver);

            if (currentScore > highScore)
            {
                highScore = currentScore;
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
                PlayerPrefs.Save();
            }
        }

        public void TogglePause()
        {
            if (currentState == GameState.Playing)
            {
                SetState(GameState.Paused);
                Time.timeScale = 0f;
            }
            else if (currentState == GameState.Paused)
            {
                SetState(GameState.Playing);
                Time.timeScale = 1f;
            }
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void SetState(GameState newState)
        {
            currentState = newState;
            OnStateChanged?.Invoke(newState);
        }
    }
}
