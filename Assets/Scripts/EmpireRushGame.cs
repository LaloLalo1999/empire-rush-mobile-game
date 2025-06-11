using UnityEngine;
using UnityEngine.UI;

public class EmpireRushGame : MonoBehaviour
{
    [Header("Empire Rush - Mobile Idle Game")]
    public Text statusText;
    public Button actionButton;
    
    private int coins = 100;
    private int energy = 5;
    private int businesses = 0;
    
    void Start()
    {
        Debug.Log("🎮 Empire Rush Mobile Game - iOS Build Ready!");
        
        // Create UI if it doesn't exist
        SetupGameUI();
        UpdateDisplay();
    }
    
    void SetupGameUI()
    {
        // Find or create Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Create status text
        if (statusText == null)
        {
            GameObject textGO = new GameObject("StatusText");
            textGO.transform.SetParent(canvas.transform, false);
            statusText = textGO.AddComponent<Text>();
            statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            statusText.fontSize = 24;
            statusText.color = Color.white;
            statusText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.1f, 0.7f);
            textRect.anchorMax = new Vector2(0.9f, 0.9f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }
        
        // Create action button
        if (actionButton == null)
        {
            GameObject buttonGO = new GameObject("ActionButton");
            buttonGO.transform.SetParent(canvas.transform, false);
            
            Image buttonImage = buttonGO.AddComponent<Image>();
            buttonImage.color = Color.blue;
            
            actionButton = buttonGO.AddComponent<Button>();
            actionButton.onClick.AddListener(OnActionClicked);
            
            // Button text
            GameObject buttonTextGO = new GameObject("ButtonText");
            buttonTextGO.transform.SetParent(buttonGO.transform, false);
            Text buttonText = buttonTextGO.AddComponent<Text>();
            buttonText.text = "SPIN WHEEL";
            buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            buttonText.fontSize = 18;
            buttonText.color = Color.white;
            buttonText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform buttonTextRect = buttonTextGO.GetComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;
            
            RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.3f, 0.4f);
            buttonRect.anchorMax = new Vector2(0.7f, 0.6f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
        }
        
        // Ensure EventSystem exists
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }
    
    void OnActionClicked()
    {
        if (energy > 0)
        {
            // Spin wheel simulation
            energy--;
            int reward = Random.Range(10, 100);
            coins += reward;
            
            // Chance to unlock business
            if (Random.Range(0f, 1f) < 0.3f && businesses < 5)
            {
                businesses++;
                Debug.Log($"🏢 New business unlocked! Total: {businesses}");
            }
            
            Debug.Log($"🎰 Spin completed! +{reward} coins");
            UpdateDisplay();
        }
        else
        {
            Debug.Log("❌ No energy! Wait for regeneration...");
        }
    }
    
    void UpdateDisplay()
    {
        if (statusText != null)
        {
            string status = $"EMPIRE RUSH\n\n";
            status += $"💰 Coins: {coins:N0}\n";
            status += $"⚡ Energy: {energy}/5\n";
            status += $"🏢 Businesses: {businesses}/5\n\n";
            
            if (energy > 0)
                status += "Tap SPIN WHEEL to play!";
            else
                status += "Energy regenerating...";
                
            statusText.text = status;
        }
        
        if (actionButton != null)
            actionButton.interactable = energy > 0;
    }
    
    void Update()
    {
        // Simple energy regeneration every 10 seconds
        if (energy < 5 && Time.time % 10f < Time.deltaTime)
        {
            energy++;
            UpdateDisplay();
            Debug.Log($"⚡ Energy regenerated: {energy}/5");
        }
        
        // Passive income from businesses
        if (businesses > 0 && Time.time % 2f < Time.deltaTime)
        {
            int passiveIncome = businesses * 5;
            coins += passiveIncome;
            Debug.Log($"💰 Passive income: +{passiveIncome} coins from {businesses} businesses");
            UpdateDisplay();
        }
    }
}