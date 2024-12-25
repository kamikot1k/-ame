using Unity.VisualScripting;
using UnityEngine;

public class BuildingHealth : MonoBehaviour
{
    [SerializeField] private float _health;

    private void takeDamage(float damage)
    {
        if (_health - damage > 0)
        {
            _health -= damage;
        } else
        {
            if (gameObject.GetComponent<Mine>() != null)
            {
                gameObject.GetComponent<Mine>()._moneyControl._moneyPerTime -= gameObject.GetComponent<Mine>()._moneyPerTime;
            }    
            Destroy(gameObject);
        }
    }
}
