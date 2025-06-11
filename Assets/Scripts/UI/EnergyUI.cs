using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using EmpireRush.Economy;
using EmpireRush.Core;

namespace EmpireRush.UI
{
    public class EnergyUI : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Slider energySlider;
        [SerializeField] private Text energyText;
        [SerializeField] private Text regenTimerText;
        [SerializeField] private Button buyEnergyButton;
        [SerializeField] private GameObject energyFullIndicator;
        
        [Header("Visual Effects")]
        [SerializeField] private ParticleSystem energyGainEffect;
        [SerializeField] private AnimationCurve energyBarAnimCurve = AnimationCurve.EaseOutBack(0, 0, 1, 1);
        [SerializeField] private float animationDuration = 0.5f;
        
        [Header("Colors")]
        [SerializeField] private Color normalColor = Color.green;
        [SerializeField] private Color lowColor = Color.yellow;
        [SerializeField] private Color emptyColor = Color.red;
        [SerializeField] private Color fullColor = Color.cyan;
        
        private int currentEnergy;
        private int maxEnergy = 10;
        private float energyRegenTime = 300f; // 5 minutes in seconds
        private DateTime lastEnergyRegenTime;
        private Coroutine animationCoroutine;
        private Coroutine timerCoroutine;
        
        private void Start()
        {
            InitializeEnergyUI();
            SetupEvents();
            StartRegenTimer();
        }
        
        private void InitializeEnergyUI()
        {
            if (EconomyManager.Instance != null)
            {
                currentEnergy = EconomyManager.Instance.Energy;
                maxEnergy = 10; // This should match EconomyManager's maxEnergy
            }
            
            if (energySlider != null)
            {
                energySlider.maxValue = maxEnergy;
                energySlider.value = currentEnergy;
            }
            
            UpdateEnergyDisplay();
            
            if (buyEnergyButton != null)
                buyEnergyButton.onClick.AddListener(OnBuyEnergyClicked);
        }
        
        private void SetupEvents()
        {
            EconomyManager.OnEnergyChanged += OnEnergyChanged;
        }
        
        private void OnDestroy()
        {
            EconomyManager.OnEnergyChanged -= OnEnergyChanged;
            
            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);
                
            if (timerCoroutine != null)
                StopCoroutine(timerCoroutine);
        }
        
        private void OnEnergyChanged(int newEnergy)
        {
            int previousEnergy = currentEnergy;
            currentEnergy = newEnergy;
            
            // Play energy gain effect if energy increased
            if (newEnergy > previousEnergy && energyGainEffect != null)
                energyGainEffect.Play();
            
            // Animate energy bar
            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);
                
            animationCoroutine = StartCoroutine(AnimateEnergyBar(previousEnergy, newEnergy));
            
            UpdateEnergyDisplay();
        }
        
        private IEnumerator AnimateEnergyBar(int fromEnergy, int toEnergy)
        {
            float elapsedTime = 0f;
            
            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / animationDuration;
                float curveValue = energyBarAnimCurve.Evaluate(progress);
                
                float currentValue = Mathf.Lerp(fromEnergy, toEnergy, curveValue);
                
                if (energySlider != null)
                    energySlider.value = currentValue;
                
                yield return null;
            }
            
            // Ensure final value
            if (energySlider != null)
                energySlider.value = toEnergy;
        }
        
        private void UpdateEnergyDisplay()
        {
            // Update text
            if (energyText != null)
                energyText.text = $"{currentEnergy}/{maxEnergy}";
            
            // Update slider color based on energy level
            UpdateEnergyBarColor();
            
            // Update full indicator
            if (energyFullIndicator != null)
                energyFullIndicator.SetActive(currentEnergy >= maxEnergy);
            
            // Update buy button state
            if (buyEnergyButton != null)
                buyEnergyButton.interactable = currentEnergy < maxEnergy;
        }
        
        private void UpdateEnergyBarColor()
        {
            if (energySlider == null) return;
            
            Image fillImage = energySlider.fillRect.GetComponent<Image>();
            if (fillImage == null) return;
            
            float energyPercentage = (float)currentEnergy / maxEnergy;
            
            Color targetColor;
            if (currentEnergy >= maxEnergy)
                targetColor = fullColor;
            else if (energyPercentage <= 0.2f)
                targetColor = emptyColor;
            else if (energyPercentage <= 0.5f)
                targetColor = lowColor;
            else
                targetColor = normalColor;
            
            fillImage.color = targetColor;
        }
        
        private void StartRegenTimer()
        {
            if (timerCoroutine != null)
                StopCoroutine(timerCoroutine);
                
            timerCoroutine = StartCoroutine(RegenTimerCoroutine());
        }
        
        private IEnumerator RegenTimerCoroutine()
        {
            while (true)
            {
                UpdateRegenTimer();
                yield return new WaitForSeconds(1f);
            }
        }
        
        private void UpdateRegenTimer()
        {
            if (regenTimerText == null || currentEnergy >= maxEnergy)
            {
                if (regenTimerText != null)
                    regenTimerText.text = "FULL";
                return;
            }
            
            // Calculate time until next energy
            float timeSinceLastRegen = (float)(DateTime.Now - lastEnergyRegenTime).TotalSeconds;
            float timeUntilNextEnergy = energyRegenTime - (timeSinceLastRegen % energyRegenTime);
            
            if (timeUntilNextEnergy <= 0)
                timeUntilNextEnergy = energyRegenTime;
            
            // Format time as MM:SS
            int minutes = Mathf.FloorToInt(timeUntilNextEnergy / 60f);
            int seconds = Mathf.FloorToInt(timeUntilNextEnergy % 60f);
            
            regenTimerText.text = $"{minutes:00}:{seconds:00}";
        }
        
        private void OnBuyEnergyClicked()
        {
            // Open energy store UI
            UIManager.Instance?.ShowEnergyStore();
        }
        
        public void UpdateRegenTime(DateTime newRegenTime)
        {
            lastEnergyRegenTime = newRegenTime;
        }
        
        // Method to pulse the energy bar when energy is needed
        public void PulseEnergyBar()
        {
            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);
                
            animationCoroutine = StartCoroutine(PulseAnimation());
        }
        
        private IEnumerator PulseAnimation()
        {
            Vector3 originalScale = transform.localScale;
            Vector3 targetScale = originalScale * 1.1f;
            
            // Scale up
            float elapsedTime = 0f;
            while (elapsedTime < 0.2f)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / 0.2f;
                transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
                yield return null;
            }
            
            // Scale down
            elapsedTime = 0f;
            while (elapsedTime < 0.2f)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / 0.2f;
                transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
                yield return null;
            }
            
            transform.localScale = originalScale;
        }
    }
}