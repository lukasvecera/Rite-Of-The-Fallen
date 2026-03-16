using UnityEngine;
using TMPro;

public class CandlesInteraction : MonoBehaviour, IInteractible
{
    public string requiredQuestCandles;
    public string questAfterCompletionCandles;
    [Header("Odkazy (tahle instance = konkrétní svíčka)")]
    [SerializeField] private GameObject candleFlame;
    [SerializeField] private Light candleLight;

    [Header("Počáteční stav")]
    [SerializeField] private bool startLit = false;

    [Header("Volitelné: UI ovládání (zapni jen u jedné svíčky!)")]
    [SerializeField] private bool isUIController = false;
    [SerializeField] private TMP_Text counterText;

    private bool isLit;

    private static int s_totalCandles = 0;
    private static int s_litCandles = 0;
    private static TMP_Text s_counterText;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        s_totalCandles = 0;
        s_litCandles = 0;
        s_counterText = null;
    }

    void Update()
    {
        if (!isUIController || counterText == null) return;

        bool shouldShow = requiredQuestCandles == QuestManager.Instance.GetCurrentQuest();
        counterText.gameObject.SetActive(shouldShow);
    }

    private void Awake()
    {
        s_totalCandles++;

        isLit = startLit;
        ApplyVisual(isLit);

        if (isLit) s_litCandles++;

        if (isUIController)
        {
            s_counterText = counterText;
        }
    }

    private void Start()
    {
        UpdateUI();
        CheckAllLit();
    }

    private void OnDestroy()
    {
        s_totalCandles = Mathf.Max(0, s_totalCandles - 1);
        if (isLit) s_litCandles = Mathf.Max(0, s_litCandles - 1);

        if (isUIController && s_counterText == counterText)
            s_counterText = null;

        UpdateUI();
    }

    public void Interact()
    {
        if (requiredQuestCandles == QuestManager.Instance.GetCurrentQuest()) {

        isLit = !isLit;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.matchCandleSound);
        if (isLit) s_litCandles++;
        else       s_litCandles = Mathf.Max(0, s_litCandles - 1);

        ApplyVisual(isLit);
        UpdateUI();
        CheckAllLit();
        } else {
            return;
        }
    }

    private void ApplyVisual(bool lit)
    {
        if (candleFlame) candleFlame.SetActive(lit);
        if (candleLight) candleLight.enabled = lit;
    }

    private static void UpdateUI()
    {
        if (!s_counterText) return;
        int remaining = Mathf.Max(0, s_totalCandles - s_litCandles);
        s_counterText.text = $"Remaining: {remaining}/{s_totalCandles}";
    }

    private void CheckAllLit()
    {
        if (s_totalCandles > 0 && s_litCandles == s_totalCandles)
        {
            QuestManager.Instance.SetQuest(questAfterCompletionCandles);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.questCompleteSound, 0.5f);
        }
    }
}
