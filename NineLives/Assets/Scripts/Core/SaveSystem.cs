using UnityEngine;
using TMPro;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    private const string SAVE_POS_X = "Player_PosX";
    private const string SAVE_POS_Y = "Player_PosY";
    private const string SAVE_POS_Z = "Player_PosZ";
    private const string SAVE_LIFE_INDEX = "Player_LifeIndex";
    private const string HAS_SAVE_KEY = "HasSaveData";

    [Header("Optional UI Feedback")]
    [SerializeField] private TextMeshProUGUI saveStatusText;

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

    /// <summary>
    /// Attach this method to your UI Button's OnClick event!
    /// </summary>
    public void OnSaveButtonClicked()
    {
        SaveGame();
    }

    public void SaveGame()
    {
        if (LifeManager.Instance == null || LifeManager.Instance.playerController == null)
        {
            Debug.LogWarning("[SaveSystem] Cannot save: LifeManager or PlayerController is missing!");
            return;
        }

        Vector3 pos = LifeManager.Instance.playerController.transform.position;
        int currentLife = LifeManager.Instance.currentLifeIndex;

        // Persist coordinates and current life state
        PlayerPrefs.SetFloat(SAVE_POS_X, pos.x);
        PlayerPrefs.SetFloat(SAVE_POS_Y, pos.y);
        PlayerPrefs.SetFloat(SAVE_POS_Z, pos.z);
        PlayerPrefs.SetInt(SAVE_LIFE_INDEX, currentLife);
        PlayerPrefs.SetInt(HAS_SAVE_KEY, 1);
        PlayerPrefs.Save();

        Debug.Log($"[Save System] Game Saved! Position: {pos} | Current Life: {currentLife + 1}");

        if (saveStatusText != null)
        {
            saveStatusText.text = "Game Saved!";
            CancelInvoke(nameof(ClearSaveStatusText));
            Invoke(nameof(ClearSaveStatusText), 2f);
        }
    }

    public bool LoadGame(out Vector3 position, out int lifeIndex)
    {
        if (PlayerPrefs.HasKey(HAS_SAVE_KEY))
        {
            float x = PlayerPrefs.GetFloat(SAVE_POS_X);
            float y = PlayerPrefs.GetFloat(SAVE_POS_Y);
            float z = PlayerPrefs.GetFloat(SAVE_POS_Z);
            position = new Vector3(x, y, z);
            lifeIndex = PlayerPrefs.GetInt(SAVE_LIFE_INDEX, 0);
            return true;
        }

        position = Vector3.zero;
        lifeIndex = 0;
        return false;
    }

    public void SaveCustomRespawnPoint(Vector3 newRespawnPosition)
    {
        PlayerPrefs.SetFloat(SAVE_POS_X, newRespawnPosition.x);
        PlayerPrefs.SetFloat(SAVE_POS_Y, newRespawnPosition.y);
        PlayerPrefs.SetFloat(SAVE_POS_Z, newRespawnPosition.z);
        PlayerPrefs.Save();
    }

    public Vector3 GetSavedRespawnPosition(Vector3 defaultPosition)
    {
        if (PlayerPrefs.HasKey(HAS_SAVE_KEY))
        {
            return new Vector3(
                PlayerPrefs.GetFloat(SAVE_POS_X),
                PlayerPrefs.GetFloat(SAVE_POS_Y),
                PlayerPrefs.GetFloat(SAVE_POS_Z)
            );
        }
        return defaultPosition;
    }

    private void ClearSaveStatusText()
    {
        if (saveStatusText != null) saveStatusText.text = "";
    }
}