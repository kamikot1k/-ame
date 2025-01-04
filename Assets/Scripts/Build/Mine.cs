using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Mine : NetworkBehaviour
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
