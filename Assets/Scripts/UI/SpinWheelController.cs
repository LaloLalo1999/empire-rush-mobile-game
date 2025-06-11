using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EmpireRush.Core;
using EmpireRush.Economy;
using EmpireRush.Analytics;

namespace EmpireRush.UI
{
    public class SpinWheelController : MonoBehaviour
    {
        [Header("Wheel Components")]
        [SerializeField] private Transform wheelTransform;
        [SerializeField] private Button spinButton;
        [SerializeField] private ParticleSystem rewardParticles;
        [SerializeField] private AudioSource spinAudioSource;
        
        [Header("Wheel Settings")]
        [SerializeField] private float spinDuration = 3f;
        [SerializeField] private float minSpinSpeed = 720f;
        [SerializeField] private float maxSpinSpeed = 1440f;
        [SerializeField] private AnimationCurve spinCurve = AnimationCurve.EaseOut(0, 1, 1, 0);
        
        [Header("Rewards")]
        [SerializeField] private List<WheelReward> rewards = new List<WheelReward>();
        
        [Header("Audio")]
        [SerializeField] private AudioClip spinStartClip;
        [SerializeField] private AudioClip spinLoopClip;
        [SerializeField] private AudioClip spinEndClip;
        [SerializeField] private AudioClip rewardClip;
        
        private bool isSpinning = false;
        private float currentRotation = 0f;
        private Coroutine spinCoroutine;
        
        public static event Action<WheelReward> OnRewardReceived;
        
        private void Start()
        {
            InitializeWheel();
            SetupEvents();
        }
        
        private void InitializeWheel()
        {
            if (rewards.Count == 0)
            {
                CreateDefaultRewards();
            }
            
            spinButton.onClick.AddListener(OnSpinButtonClicked);
            UpdateSpinButtonState();
        }
        
        private void CreateDefaultRewards()
        {
            float sectorAngle = 360f / 8f; // 8 sectors
            
            rewards = new List<WheelReward>
            {
                new WheelReward(RewardType.Coins, 50, 70f, sectorAngle),
                new WheelReward(RewardType.Coins, 100, 90f, sectorAngle),
                new WheelReward(RewardType.Coins, 25, 30f, sectorAngle),
                new WheelReward(RewardType.Energy, 1, 10f, sectorAngle),
                new WheelReward(RewardType.Coins, 75, 50f, sectorAngle),
                new WheelReward(RewardType.Gems, 1, 5f, sectorAngle),
                new WheelReward(RewardType.Coins, 200, 95f, sectorAngle),
                new WheelReward(RewardType.Energy, 2, 2f, sectorAngle)
            };
        }
        
        private void SetupEvents()
        {
            EconomyManager.OnEnergyChanged += OnEnergyChanged;
            GameManager.OnGameStateChanged += OnGameStateChanged;
        }
        
        private void OnDestroy()
        {
            EconomyManager.OnEnergyChanged -= OnEnergyChanged;
            GameManager.OnGameStateChanged -= OnGameStateChanged;
        }
        
        private void OnSpinButtonClicked()
        {
            if (CanSpin())
            {
                StartSpin();
            }
        }
        
        private bool CanSpin()
        {
            return !isSpinning && 
                   EconomyManager.Instance != null && 
                   EconomyManager.Instance.Energy > 0 &&
                   GameManager.Instance.currentState == GameManager.GameState.Playing;
        }
        
        private void StartSpin()
        {
            if (!EconomyManager.Instance.SpendEnergy(1))
                return;
                
            isSpinning = true;
            UpdateSpinButtonState();
            
            // Play spin start audio
            if (spinStartClip != null)
                spinAudioSource.PlayOneShot(spinStartClip);
            
            // Start spin coroutine
            spinCoroutine = StartCoroutine(SpinWheelCoroutine());
        }
        
        private IEnumerator SpinWheelCoroutine()
        {
            // Calculate spin parameters
            float targetRotation = UnityEngine.Random.Range(minSpinSpeed, maxSpinSpeed);
            float startRotation = currentRotation;
            float endRotation = startRotation + targetRotation;
            
            // Play loop audio
            if (spinLoopClip != null)
            {
                spinAudioSource.clip = spinLoopClip;
                spinAudioSource.loop = true;
                spinAudioSource.Play();
            }
            
            // Spin animation
            float elapsedTime = 0f;
            while (elapsedTime < spinDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / spinDuration;
                float curveValue = spinCurve.Evaluate(progress);
                
                currentRotation = Mathf.Lerp(startRotation, endRotation, curveValue);
                wheelTransform.rotation = Quaternion.Euler(0, 0, -currentRotation);
                
                yield return null;
            }
            
            // Ensure final rotation
            currentRotation = endRotation % 360f;
            wheelTransform.rotation = Quaternion.Euler(0, 0, -currentRotation);
            
            // Stop loop audio
            spinAudioSource.loop = false;
            spinAudioSource.Stop();
            
            // Play end audio
            if (spinEndClip != null)
                spinAudioSource.PlayOneShot(spinEndClip);
            
            // Determine reward and give it to player
            WheelReward reward = DetermineReward(currentRotation);
            yield return StartCoroutine(GiveReward(reward));
            
            isSpinning = false;
            UpdateSpinButtonState();
        }
        
        private WheelReward DetermineReward(float rotation)
        {
            // Normalize rotation to 0-360
            float normalizedRotation = rotation % 360f;
            if (normalizedRotation < 0) normalizedRotation += 360f;
            
            // Calculate which sector the wheel landed on
            float sectorAngle = 360f / rewards.Count;
            int sectorIndex = Mathf.FloorToInt(normalizedRotation / sectorAngle);
            sectorIndex = Mathf.Clamp(sectorIndex, 0, rewards.Count - 1);
            
            return rewards[sectorIndex];
        }
        
        private IEnumerator GiveReward(WheelReward reward)
        {
            // Show reward animation
            if (rewardParticles != null)
                rewardParticles.Play();
                
            // Play reward audio
            if (rewardClip != null)
                spinAudioSource.PlayOneShot(rewardClip);
            
            // Give reward to player
            switch (reward.type)
            {
                case RewardType.Coins:
                    EconomyManager.Instance.AddCoins(reward.amount);
                    break;
                case RewardType.Energy:
                    EconomyManager.Instance.AddEnergy((int)reward.amount);
                    break;
                case RewardType.Gems:
                    EconomyManager.Instance.AddGems((int)reward.amount);
                    break;
            }
            
            // Track analytics
            AnalyticsManager.Instance?.TrackSpinWheel(reward.type.ToString(), reward.amount);
            
            // Notify UI
            OnRewardReceived?.Invoke(reward);
            
            yield return new WaitForSeconds(1f); // Show reward for 1 second
        }
        
        private void UpdateSpinButtonState()
        {
            if (spinButton != null)
            {
                spinButton.interactable = CanSpin();
            }
        }
        
        private void OnEnergyChanged(int newEnergy)
        {
            UpdateSpinButtonState();
        }
        
        private void OnGameStateChanged(GameManager.GameState newState)
        {
            UpdateSpinButtonState();
        }
        
        public void ForceStopSpin()
        {
            if (spinCoroutine != null)
            {
                StopCoroutine(spinCoroutine);
                spinCoroutine = null;
            }
            
            isSpinning = false;
            spinAudioSource.Stop();
            UpdateSpinButtonState();
        }
    }
    
    [System.Serializable]
    public class WheelReward
    {
        public RewardType type;
        public long amount;
        public float weight; // For weighted random selection (future feature)
        public float sectorAngle;
        
        public WheelReward(RewardType type, long amount, float weight, float sectorAngle)
        {
            this.type = type;
            this.amount = amount;
            this.weight = weight;
            this.sectorAngle = sectorAngle;
        }
    }
    
    public enum RewardType
    {
        Coins,
        Energy,
        Gems
    }
}