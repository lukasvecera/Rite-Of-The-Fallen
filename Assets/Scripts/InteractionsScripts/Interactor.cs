using UnityEngine;
using UnityEngine.UI;

public interface IInteractible { void Interact(); }

public class Interactor : MonoBehaviour
{
    [Header("Ray source (např. Main Camera)")]
    public Transform InteractorSource;

    [Header("Raycast")]
    public float interactRange = 3f;
    public LayerMask interactableMask = ~0;
    public bool includeTriggers = true;

    [Header("Crosshair")]
    public Image crosshair;
    public Color crosshairIdle  = new Color(1,1,1,0.6f);
    public Color crosshairReady = new Color(0f,1f,1f,0.9f);
    public float readyScale = 1.25f;
    public float uiLerpSpeed = 16f;

    Vector2 baseSize;
    IInteractible target;

    void Awake()
    {
        if (crosshair) baseSize = crosshair.rectTransform.sizeDelta;
    }

    void Update()
    {
        if (InteractorSource == null && Camera.main) InteractorSource = Camera.main.transform;

        target = null;

        if (InteractorSource)
        {
            var ray = new Ray(InteractorSource.position, InteractorSource.forward);

            var triggerMode = includeTriggers ? QueryTriggerInteraction.Collide
                                              : QueryTriggerInteraction.Ignore;

            if (Physics.Raycast(ray, out var hit, interactRange, interactableMask, triggerMode))
            {
                target = hit.collider.GetComponentInParent<IInteractible>();
            }
        }

        if (crosshair)
        {
            bool hasTarget = target != null;
            var rt = crosshair.rectTransform;
            var wantedSize = hasTarget ? baseSize * readyScale : baseSize;
            rt.sizeDelta = Vector2.Lerp(rt.sizeDelta, wantedSize, uiLerpSpeed * Time.deltaTime);
            crosshair.color = Color.Lerp(crosshair.color, hasTarget ? crosshairReady : crosshairIdle, uiLerpSpeed * Time.deltaTime);
        }

        if (target != null && Input.GetKeyDown(KeyCode.E))
            target.Interact();
    }
}
