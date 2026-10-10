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

    [Header("Default Spawn Point")]
    public Transform defaultSpawnPoint;

    private Vector3 currentRespawnPosition;

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
        // Try loading saved data on game launch
        if (SaveSystem.Instance != null && SaveSystem.Instance.LoadGame(out Vector3 savedPos, out int savedLife))
        {
            currentLifeIndex = savedLife;
            currentRespawnPosition = savedPos;
            if (playerController != null)
            {
                playerController.transform.position = savedPos;
            }
        }
        else if (defaultSpawnPoint != null)
        {
            currentRespawnPosition = defaultSpawnPoint.position;
        }
        else if (playerController != null)
        {
            currentRespawnPosition = playerController.transform.position;
        }

        ApplyCurrentLife();
        UpdateLifeUI();
    }

    private void Update()
    {
        #if UNITY_EDITOR
        // Test Kill Key (K) for developer builds
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log($"[DEV] Test Kill Key Pressed! Current Life: {currentLifeIndex + 1}");
            TriggerDeathAndSwitchHost();
        }
        #endif
    }

    public void TriggerDeathAndSwitchHost(Transform overrideSpawnPoint = null)
    {
        if (lifeSequence == null || lifeSequence.Count == 0)
        {
            Debug.LogError("[LifeManager] Cannot switch host: lifeSequence list is empty!");
            return;
        }

        // Increment life index and wrap around from 9 back to 1
        currentLifeIndex = (currentLifeIndex + 1) % lifeSequence.Count;

        // Determine spawn destination (custom override or saved spawn position)
        Vector3 targetSpawnPos = overrideSpawnPoint != null 
            ? overrideSpawnPoint.position 
            : SaveSystem.Instance != null 
                ? SaveSystem.Instance.GetSavedRespawnPosition(currentRespawnPosition) 
                : currentRespawnPosition;

        if (playerController != null)
        {
            playerController.transform.position = targetSpawnPos;
        }

        ApplyCurrentLife();
        UpdateLifeUI();

        // Update active save file with the new life state at the respawn point
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame();
        }

        Debug.Log($"Julian died! Respawning at {targetSpawnPos} as Life {currentLifeIndex + 1}: {lifeSequence[currentLifeIndex].hostName}");
    }

    public void SetNewCheckpoint(Vector3 newCheckpointPos)
    {
        currentRespawnPosition = newCheckpointPos;
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveCustomRespawnPoint(newCheckpointPos);
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