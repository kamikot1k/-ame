using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerTip : MonoBehaviour
{
    [SerializeField] private Barracks _barrack;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Image _timerTip;
    [SerializeField] private Text _textTip;
    public int _count;

    private void Update()
    {
        if (_count > 0 && _barrack._trainQueue.Count > 0)
        {
            _timerTip.gameObject.SetActive(true);
            _textTip.gameObject.SetActive(true);

            if (_prefab == _barrack._trainQueue[0])
            {
                _timerTip.fillAmount = _barrack._trainTimer / _barrack._trainQueue[0].gameObject.GetComponent<Pawn>()._trainTime;
            }
            if (_barrack._trainTimer >= _barrack._trainQueue[0].gameObject.GetComponent<Pawn>()._trainTime)
            {
                _timerTip.fillAmount = 0;
                _count -= 1;
                _textTip.text = _count.ToString() + 'x';
            }
        }
    }

    public void AddCount()
    {
        _count++;
        _textTip.text = _count.ToString() + 'x';
    }
}
