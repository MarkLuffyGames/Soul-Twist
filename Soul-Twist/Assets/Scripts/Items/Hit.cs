using UnityEngine;

public class Hit : MonoBehaviour
{
    public Healt _healt;
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Healt healt))
        {
            if(healt != _healt)
            {
                healt.ReceiveDamage(GetComponentInParent<Combat>().gameObject.transform.position);
            }
        }
    }
}
