using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Door : NetworkBehaviour
{
    public NetworkVariable<bool> IsLocked = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> IsDoorOpen = new NetworkVariable<bool>(false);

    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        IsDoorOpen.OnValueChanged += OnDoorStateChanged;
    }

    [Rpc(SendTo.Server)]
    public void ToggleDoorServerRpc()
    {
        if (IsLocked.Value)
        {
            Debug.Log("No tienes la llave para abrir esta puerta.");
        }
        else
        {
            IsDoorOpen.Value = !IsDoorOpen.Value;
        }

    }

    void OnDoorStateChanged(bool previousValue, bool newValue)
    {
        _animator.SetBool("isOpen", newValue);
    }

}
