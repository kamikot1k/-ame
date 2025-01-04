using UnityEngine;
using UnityEngine.UI;

public class MoneyController : MonoBehaviour
{
    [SerializeField] private float _getMoneyDelay;
    public Text _moneyCountText;
    public float _moneyPerTime;
    public float _moneyCount;
    private float _getMoneyTimer;
    public Buildings[] _buildings;

    private void Update()
    {
        if (_getMoneyTimer < _getMoneyDelay)
        {
            _getMoneyTimer += Time.deltaTime;
        }

        if (_getMoneyTimer >= _getMoneyDelay)
        {
            _moneyCount += _moneyPerTime;
            UpdateText();
            _getMoneyTimer = 0f;
        }
    }

    public void UpdateText()
    {
        _moneyCountText.text = _moneyCount.ToString();
    }
}

[System.Serializable]
public class Buildings
{
    [SerializeField] private BuildingSettings[] _buildingSettings;
    public BuildingSettings[] BuildingSettings { get => _buildingSettings; }
}

[System.Serializable]
public class BuildingSettings
{
    public string _name;
    public float _price;
    public float _priceModifier;
}
