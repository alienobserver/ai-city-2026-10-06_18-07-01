using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopTriggerMenu : MonoBehaviour
{
    [System.Serializable]
    public struct DressItem
    {
        public string dressName;
        public int price;
        public Sprite dressSprite; // Optional: visual icon for the card
    }

    [Header("UI References")]
    [Tooltip("The main Panel/GameObject for the shop menu.")]
    public GameObject shopMenuPanel;
    [Tooltip("Text element showing the dress name.")]
    public TextMeshProUGUI cardDisplayText;
    [Tooltip("Text element showing the price.")]
    public TextMeshProUGUI priceDisplayText;
    [Tooltip("Optional image component on the card to display dress art.")]
    public Image dressCardImage;
    
    [Header("Buttons")]
    public Button nextButton;
    public Button prevButton;
    public Button purchaseButton;
    public Button closeButton;

    [Header("Shop Inventory")]
    public List<DressItem> inventory = new List<DressItem>
    {
        new DressItem { dressName = "Summer Floral Dress", price = 100 },
        new DressItem { dressName = "Cyberpunk Neon Jacket", price = 250 },
        new DressItem { dressName = "Classic Black Gown", price = 500 }
    };

    [Header("Visual Settings")]
    public float rotationSpeed = 100f;     // Speed at which the marker spins

    private int currentIndex = 0;
    private bool playerInTrigger = false;

    void Start()
    {
        // Ensure menu is closed at start
        if (shopMenuPanel != null)
            shopMenuPanel.SetActive(false);

        // Hook up button listeners
        if (nextButton != null) nextButton.onClick.AddListener(NextDress);
        if (prevButton != null) prevButton.onClick.AddListener(PrevDress);
        if (closeButton != null) closeButton.onClick.AddListener(CloseShop);
        if (purchaseButton != null) purchaseButton.onClick.AddListener(DummyPurchase);
    }


    // New Update method to make the trigger spin continuously
    void Update()
    {
        // Rotate around the Y axis. 
        // Using unscaledDeltaTime ensures it keeps spinning even when Time.timeScale = 0.
        transform.Rotate(Vector3.up * rotationSpeed * Time.unscaledDeltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        // Change "Player" to match your character's tag if needed
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            OpenShop();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            CloseShop();
        }
    }

    void OpenShop()
    {
        if (shopMenuPanel != null)
        {
            shopMenuPanel.SetActive(true);
            currentIndex = 0;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            UpdateCardUI();
        }
    }

    public void CloseShop()
    {
        if (shopMenuPanel != null)
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            shopMenuPanel.SetActive(false);
        }
    }

    public void NextDress()
    {
        if (inventory.Count == 0) return;
        currentIndex = (currentIndex + 1) % inventory.Count;
        UpdateCardUI();
    }

    public void PrevDress()
    {
        if (inventory.Count == 0) return;
        currentIndex = (currentIndex - 1 + inventory.Count) % inventory.Count;
        UpdateCardUI();
    }

    void UpdateCardUI()
    {
        if (inventory.Count == 0) return;

        DressItem currentItem = inventory[currentIndex];
        
        if (cardDisplayText != null)
        {
            cardDisplayText.text = $"{currentItem.dressName}";
            priceDisplayText.text = $"${currentItem.price}";
        }

        if (dressCardImage != null && currentItem.dressSprite != null)
        {
            dressCardImage.sprite = currentItem.dressSprite;
            dressCardImage.gameObject.SetActive(true);
        }
        else if (dressCardImage != null)
        {
            dressCardImage.gameObject.SetActive(false);
        }
    }

    void DummyPurchase()
    {
        Debug.Log($"[DUMMY PURCHASE] Successfully 'purchased' {inventory[currentIndex].dressName} for {inventory[currentIndex].price} coins!");
        // Add your sound effects or UI feedback here if desired
    }
}