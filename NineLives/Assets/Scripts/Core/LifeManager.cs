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
    [SerializeField] private Transform testSpawnPoint;

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
        if (lifeSequence == null || lifeSequence.Count == 0)
        {
            Debug.LogError("[LifeManager] Cannot switch host: lifeSequence list is empty!");
            return;
        }

        // Increment index and wrap around to 0 when passing Life 9 (index 8)
        currentLifeIndex = (currentLifeIndex + 1) % lifeSequence.Count;

        // Teleport player to spawn point if assigned
        if (newHostSpawnPoint != null && playerController != null)
        {
            playerController.transform.position = newHostSpawnPoint.position;
        }

        ApplyCurrentLife();
        UpdateLifeUI();

        if (currentLifeIndex == 0)
        {
            Debug.Log($"[Loop Reset] Julian died on Life {lifeSequence.Count}! Returning to Life 1: {lifeSequence[0].hostName}");
        }
        else
        {
            Debug.Log($"Julian died! Now possessing Life {currentLifeIndex + 1}: {lifeSequence[currentLifeIndex].hostName}");
        }
    }

    private void ApplyCurrentLife()
    {
        if (lifeSequence != null && currentLifeIndex < lifeSequence.Count && lifeSequence[currentLifeIndex] != null)
        {
            if (playerController != null)
            {
                playerController.LoadHostData(lifeSequence[currentLifeIndex]);
            }
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