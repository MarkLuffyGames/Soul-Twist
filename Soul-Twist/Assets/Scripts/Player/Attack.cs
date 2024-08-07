using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class Attack : NetworkBehaviour
{
    private PlayerController _playerController;
    private Animator _animator;

    private InputAction attackAction;
    private void Start()
    {
        if (!IsOwner) return;
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();

        attackAction = InputSystem.actions.FindAction("Attack");
        attackAction.started += AttackAction_started;
    }

    private void AttackAction_started(InputAction.CallbackContext obj)
    {
        if (_playerController.canAttack)
        {
            _playerController.canAttack = false;
            _playerController.canMove = false;
            _animator.SetTrigger("Punch Attack");
        }
    }
}
