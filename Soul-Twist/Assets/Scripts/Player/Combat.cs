using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class Combat : NetworkBehaviour
{
    private PlayerController _playerController;
    private Inventory _inventory;
    private Animator _animator;

    public BoxCollider weaponCollider;

    private InputAction attackAction;
    private InputAction blockAction;
    private InputAction aimAction;

    public LayerMask targetLayerMask;

    private int attackNumber;
    private bool inCombo;
    private bool blocking;
    private void Start()
    {
        if (!IsOwner) return;
        _playerController = GetComponent<PlayerController>();
        _inventory = GetComponent<Inventory>();
        _animator = GetComponent<Animator>();

        attackAction = InputSystem.actions.FindAction("Attack");
        blockAction = InputSystem.actions.FindAction("Block");
        aimAction = InputSystem.actions.FindAction("Aim");

        attackAction.started += AttackAction_started;
        blockAction.started += BlockAction_started;
        blockAction.canceled += BlockAction_canceled;
        aimAction.performed += AimAction_started;
        aimAction.canceled += AimAction_canceled;
    }

    private void AimAction_canceled(InputAction.CallbackContext obj)
    {
        _playerController.target = null;
    }

    private void AimAction_started(InputAction.CallbackContext obj)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5.0f, targetLayerMask);
        List<Combat> healtList = new List<Combat>();

        foreach (Collider collider in colliders)
        {
            if(collider.gameObject.TryGetComponent(out Combat component))
            {
                if (component == this) continue;
                healtList.Add(component);
            }
        }

        healtList.Sort((a, b) =>
        {
            float distanceA = Vector3.Distance(transform.position, a.transform.position);
            float distanceB = Vector3.Distance(transform.position, b.transform.position);
            return distanceA.CompareTo(distanceB);
        });

        if (healtList.Count >= 1)
        {
            _playerController.target = healtList[0].gameObject;
            healtList.Clear();
        }
    }

    private void BlockAction_started(InputAction.CallbackContext obj)
    {
        if (_playerController.canBlock)
        {
            blocking = true;
            _animator.SetBool("Block", blocking);
            _playerController.canMove = false;
        }
    }

    private void BlockAction_canceled(InputAction.CallbackContext obj)
    {
        blocking = false;
        _animator.SetBool("Block", blocking);
        _playerController.canMove = true;
    }

    private void AttackAction_started(InputAction.CallbackContext obj)
    {
        if (_playerController.canAttack && !EventSystem.current.IsPointerOverGameObject())
        {

            if (inCombo)
            {
                attackNumber++;
                if(attackNumber == _inventory.GetAttackNumber())
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
        StartCoroutine(MoveForward());
        _playerController.canAttack = false;
        _playerController.canMove = false;
    }
    public IEnumerator MoveForward()
    {
        float move = 0;

        while (move < 0.5f)
        {
            transform.position += transform.forward * 2 * Time.deltaTime;
            move += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnActiveCollider(AnimationEvent animationEvent)
    {
        weaponCollider.enabled = true;
    }
    private void OnDesactiveCollider(AnimationEvent animationEvent)
    {
        weaponCollider.enabled = false;
    }
    private void OnFinishAttack(AnimationEvent animationEvent)
    {
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
