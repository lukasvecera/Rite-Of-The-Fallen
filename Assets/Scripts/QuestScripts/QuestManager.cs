using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public TMP_Text questTextUI;
    public string startingQuestName = "Find paper guiding to quest book (1st floor of cathedral)";

    private string currentQuest;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;

        SetQuest(startingQuestName);
    }

    public void SetQuest(string NewQuestDescription)
    {
        currentQuest = NewQuestDescription;
        UpdateQuestUI();
    }

    public string GetCurrentQuest()
    {
        return currentQuest;
    }

    private void UpdateQuestUI()
    {
        if (questTextUI == null)
        {

            TMP_Text found = GameObject.FindWithTag("questTextUI")?.GetComponent<TMP_Text>();
            if (found != null)
                questTextUI = found;
        }

        if (questTextUI != null)
            questTextUI.text = currentQuest;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        UpdateQuestUI();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
