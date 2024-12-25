using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public float _moneyPerTime;
    public MoneyController _moneyControl;

    private void Start()
    {
        _moneyControl = Camera.main.gameObject.GetComponent<MoneyController>();
    }

    public void AddMoneyPerTime()
    {
        _moneyControl._moneyPerTime += _moneyPerTime;
    }
}
