using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class RitualController : MonoBehaviour, IInteractible
{
    [Header("Spoty")]
    public RitualSpot bookSpot;
    public RitualSpot gobletSpot;

    [Header("Výsledek")]
    public string questAfterSuccess;
    public UnityEvent onSuccess;

    [Header("Chování")]
    public bool consumeItems = true;
    private bool completed = false;
    public bool lockSpotsAfterSuccess = true;

    private void Awake()
    {
        if (bookSpot)  bookSpot.OnChanged  += OnSpotChanged;
        if (gobletSpot) gobletSpot.OnChanged += OnSpotChanged;
    }

    private void OnDestroy()
    {
        if (bookSpot)  bookSpot.OnChanged  -= OnSpotChanged;
        if (gobletSpot) gobletSpot.OnChanged -= OnSpotChanged;
    }

    private void OnSpotChanged(RitualSpot _)
    {
        TryAutoComplete();
    }

    private void TryAutoComplete()
    {
        if (completed) return;
        if (!bookSpot || !gobletSpot) return;
        if (!bookSpot.IsFilled || !gobletSpot.IsFilled) return;

        completed = true;

        if (consumeItems)
        {
            bookSpot.ClearVisualOnly();
            gobletSpot.ClearVisualOnly();
            bookSpot.currentItem  = null;
            gobletSpot.currentItem = null;
        }

        if (!string.IsNullOrEmpty(questAfterSuccess))
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.questCompleteSound, 0.5f);
            QuestManager.Instance.SetQuest(questAfterSuccess);
        }

        onSuccess?.Invoke();

    if (lockSpotsAfterSuccess)
    {
        if (bookSpot)  bookSpot.enabled = false;
        if (gobletSpot) gobletSpot.enabled = false;
    }
    }

    public void Interact() { /* záměrně prázdné */ }
}
