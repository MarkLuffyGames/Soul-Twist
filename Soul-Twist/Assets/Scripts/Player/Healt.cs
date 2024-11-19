using System.Collections;
using UnityEditor;
using UnityEngine;

public class Healt : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();   
    }
    public void ReceiveDamage(Vector3 positionHit)
    {

        _animator.SetTrigger("Hit");

        Vector3 direction = transform.position - positionHit;
        Vector3 directionHit = new Vector3 (direction.x, 0, direction.z).normalized;

        StartCoroutine(RecoilWhenHit(directionHit));
    }

    IEnumerator RecoilWhenHit(Vector3 directionHit)
    {
        if (!Physics.Raycast(transform.position + Vector3.up, directionHit, out RaycastHit hit, 1f))
        {
            float time = 0f;
            
            while (time < 0.2f)
            {
                transform.position += directionHit * 4 * Time.deltaTime;
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up, directionHit, Color.red, 1f, true);
        }
    }
}
