using UnityEngine;

public class DoorsGroup : MonoBehaviour, IInteractible
{
    public DoorInteraction left;
    public DoorInteraction right;

    public void Interact()
    {
        left?.Interact();
        right?.Interact();
    }
}
