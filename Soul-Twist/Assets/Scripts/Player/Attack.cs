using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.VisualScripting;

public class Attack : NetworkBehaviour
{
    private PlayerController _playerController;
    private Inventory _inventory;
    private Animator _animator;

    private InputAction attackAction;


    private int attackNumber;
    private bool inCombo;
    private void Start()
    {
        if (!IsOwner) return;
        _playerController = GetComponent<PlayerController>();
        _inventory = GetComponent<Inventory>();
        _animator = GetComponent<Animator>();

        attackAction = InputSystem.actions.FindAction("Attack");
        attackAction.started += AttackAction_started;
    }

    private void AttackAction_started(InputAction.CallbackContext obj)
    {
        if (_playerController.canAttack)
        {

            if (inCombo)
            {
                attackNumber++;
                if(attackNumber > 3)
                {
                    attackNumber = 0;
                }
            }
            else
            {
                attackNumber = 0;
            }

            _animator.SetFloat("AttackType", _inventory.GetAttackType().GetHashCode());
            _animator.SetFloat("AttackNumber", attackNumber);
            _animator.SetTrigger("Attack");
        }
    }

    private void OnStartAttack(AnimationEvent animationEvent)
    {
        Debug.Log("StartAttack");
        _playerController.canAttack = false;
        _playerController.canMove = false;
    }
    private void OnFinishAttack(AnimationEvent animationEvent)
    {
        Debug.Log("FinishAttack");
        _playerController.canMove = true;
        _playerController.canAttack = true;

        inCombo = true;
        Invoke("CancelCombo", 0.2f);
    }

    private void CancelCombo()
    {
        inCombo = false;
    }
}
