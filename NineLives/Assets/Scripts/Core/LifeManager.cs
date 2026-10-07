using UnityEngine;
using System.Collections.Generic;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance { get; private set; }

    [Header("Host Register (1 through 9)")]
    public List<HostLifeData> lifeSequence;
    public int currentLifeIndex = 0; // 0 = Life 1, 8 = Life 9

    [Header("Player Reference")]
    public PlayerController2D playerController;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ApplyCurrentLife();
    }

    public void TriggerDeathAndSwitchHost(Transform newHostSpawnPoint)
    {
        if (currentLifeIndex < lifeSequence.Count - 1)
        {
            currentLifeIndex++;
            playerController.transform.position = newHostSpawnPoint.position;
            ApplyCurrentLife();
            Debug.Log($"Julian died! Now possessing Life {currentLifeIndex + 1}: {lifeSequence[currentLifeIndex].hostName}");
        }
        else
        {
            Debug.Log("Life 9 Reached - Final Death / Game Over Cycle!");
        }
    }

    private void ApplyCurrentLife()
    {
        if (lifeSequence.Count > currentLifeIndex)
        {
            playerController.LoadHostData(lifeSequence[currentLifeIndex]);
        }
    }
}