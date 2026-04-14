using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class InteractionSphere : MonoBehaviour
{
    private CharacterController _characterController;
    private Interfaces.IInteractable _currentInteractable;
    public TextMeshProUGUI interactionText;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }
    
    // Update is called once per frame
    private void Update()
    {
        CheckForInteractable();
        // TODO: Change this to input system
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.LogError("Pressed");
            AttemptInteraction();
        }
    }

    // Uses a box overlap to check for objects with a script containing the IInteractable interface
    private void CheckForInteractable()
    {
        var center = transform.position + _characterController.center;
        center += transform.forward * 1f;

        // Box must be halved !??!
        var halfExtents = new Vector3(1.5f, 1.0f, 0.2f);

        var hits = Physics.OverlapBox(center, halfExtents, transform.rotation);

        var closestDistance = float.MaxValue;
        Interfaces.IInteractable currentInteractable = null;
        Collider closestCollider = null;

        foreach (var col in hits)
        {
            var interactable = col.GetComponentInParent<Interfaces.IInteractable>();
            if (interactable == null) continue;

            var distance = Vector3.Distance(transform.position, col.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentInteractable = interactable;
                closestCollider = col;
            }
        }

        if (currentInteractable != null)
        {
            _currentInteractable = currentInteractable;
            interactionText.text = $"Press 'E' to interact with {closestCollider.name}";
        }
        else
        {
            ClearInteractionText();
        }
    }

    private void ClearInteractionText()
    {
        _currentInteractable = null;
        interactionText.text = "";
    }

    private void AttemptInteraction()
    {
        if (_currentInteractable != null)
        {
            _currentInteractable.Interact();
        }
    }
    
    #region Gizmos
    
    private void OnDrawGizmos()
    {
        var controller = GetComponent<CharacterController>();
        if (controller == null) return;

        var center = transform.position + controller.center;
        center += transform.forward * 1f;

        var halfExtents = new Vector3(1.5f, 1.0f, 0.2f);

        Gizmos.color = Color.green;

        var oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2);
        Gizmos.matrix = oldMatrix;
    }
    #endregion
}
