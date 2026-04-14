using UnityEngine;

public class BookInteractable : MonoBehaviour, Interfaces.IInteractable
{
    public void Interact()
    {
        Debug.Log("Interaction happened");
        Destroy(gameObject);
    }
}
