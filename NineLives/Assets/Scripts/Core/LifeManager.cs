using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance { get; private set; }

    [Header("Host Register (1 through 9)")]
    public List<HostLifeData> lifeSequence = new List<HostLifeData>();
    [Tooltip("0 = Life 1, 8 = Life 9")]
    public int currentLifeIndex = 0; 

    [Header("UI & Player References")]
    public PlayerController2D playerController;
    [SerializeField] private TextMeshProUGUI lifeTextUI;

    [Header("Dev Testing Settings")]
    [SerializeField] private Transform testSpawnPoint; // Assign a test spawn position in Inspector

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
    }

    private void Start()
    {
        ApplyCurrentLife();
        UpdateLifeUI();
    }

    private void Update()
    {
        // Development Build / Editor feature for instant death testing
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log($"[DEV] Test Kill Key Pressed! Current Life before kill: {currentLifeIndex + 1}");
            TriggerDeathAndSwitchHost(testSpawnPoint);
        }
        #endif
    }

    public void TriggerDeathAndSwitchHost(Transform newHostSpawnPoint = null)
    {
        // Check if we can still advance to a new life (Max index is lifeSequence.Count - 1)
        if (currentLifeIndex < lifeSequence.Count - 1)
        {
            currentLifeIndex++; // Move to next life (e.g., from 0 to 1)

            // Teleport player to new spawn point if assigned
            if (newHostSpawnPoint != null)
            {
                playerController.transform.position = newHostSpawnPoint.position;
            }

            ApplyCurrentLife();
            UpdateLifeUI();

            Debug.Log($"Julian died! Now possessing Life {currentLifeIndex + 1}: {lifeSequence[currentLifeIndex].hostName}");
        }
        else
        {
            // Already on Life 9 (index 8)
            Debug.LogWarning($"Life {currentLifeIndex + 1} Reached - Final Life! No more host bodies available.");
        }
    }

    private void ApplyCurrentLife()
    {
        if (lifeSequence != null && currentLifeIndex < lifeSequence.Count && lifeSequence[currentLifeIndex] != null)
        {
            playerController.LoadHostData(lifeSequence[currentLifeIndex]);
        }
        else
        {
            Debug.LogWarning($"[LifeManager] HostLifeData missing or not assigned at index {currentLifeIndex}!");
        }
    }

    public void UpdateLifeUI()
    {
        if (lifeTextUI != null)
        {
            lifeTextUI.text = $"Current Life: {currentLifeIndex + 1}";
        }
    }
}