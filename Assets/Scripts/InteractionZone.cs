using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionZone : MonoBehaviour
{
    private Interfaces.IInteractable _currentInteractable;
    public TextMeshProUGUI interactionText;

    private InputSystem_Actions _playerInputActions;
    
    private void Awake()
    {
        _playerInputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
        _playerInputActions.Player.Interact.performed += AttemptInteraction;
    }

    [SerializeField] private List<GameObject> interactableObjects;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Interfaces.IInteractable interactable))
        {
            interactableObjects.Add(other.gameObject);
        }
        
        HandleText();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Interfaces.IInteractable interactable))
        {
            interactableObjects.Remove(other.gameObject);
        }

        HandleText();
    }

    private void HandleText()
    {
        if (interactableObjects.Count > 0)
        {
            SetupInteractionText();
        }
        else
        {
            ClearInteractionText();
        }
    }

    private void SetupInteractionText()
    {
        interactableObjects.RemoveAll(item => item == null);
        
        if (interactableObjects.Count > 0)
        {
            var keyName = _playerInputActions.Player.Interact.GetBindingDisplayString(InputBinding.MaskByGroup("Keyboard&Mouse"));
            
            interactionText.text = $"Press '{keyName}' to interact with {interactableObjects[0].gameObject.name}";
        }
    }
    
    private void ClearInteractionText()
    {
        _currentInteractable = null;
        interactionText.text = "";
    }

    private void AttemptInteraction(InputAction.CallbackContext context)
    {
        Debug.LogError("E was pressed");

        if (interactableObjects.Count > 0)
        {
            _currentInteractable = interactableObjects[0].GetComponent<Interfaces.IInteractable>();
        }
        
        if (_currentInteractable != null)
        {
            _currentInteractable.Interact();
        }

        StartCoroutine(UpdateTextAfterInteraction());
    }

    private IEnumerator UpdateTextAfterInteraction()
    {
        yield return new WaitForEndOfFrame();
        if (interactableObjects[0].gameObject == null && interactableObjects.Count > 1)
        {
            SetupInteractionText();
        }
        else if (interactableObjects[0].gameObject == null)
        {
            ClearInteractionText();
        }
    }
}
