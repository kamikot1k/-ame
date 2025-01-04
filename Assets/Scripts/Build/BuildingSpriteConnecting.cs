using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class BuildingSpriteConnecting : MonoBehaviour
{
    private BuildingsGrid _bulindingsGrid;
    private SpriteRenderer _sr;

    [SerializeField] private Sprite _baseTexture;
    [SerializeField] private Sprite _horizontalTexture;
    [SerializeField] private Sprite _verticalTexture;

    public void UpdateTexture()
    {
        _bulindingsGrid = GameObject.Find("Ground").GetComponent<BuildingsGrid>();
        _sr = GetComponent<SpriteRenderer>();

        int x = Mathf.RoundToInt(transform.position.x);
        int y = Mathf.RoundToInt(transform.position.y);

        if (_bulindingsGrid.grid[x - 1, y] != null && _bulindingsGrid.grid[x + 1, y] != null && gameObject.name == _bulindingsGrid.grid[x - 1, y].name && gameObject.name == _bulindingsGrid.grid[x + 1, y].name)
        {
            _sr.sprite = _horizontalTexture;
            if (_bulindingsGrid.grid[x - 1, y].GetComponent<SpriteRenderer>().sprite == _verticalTexture)
            {
                _bulindingsGrid.grid[x - 1, y].GetComponent<SpriteRenderer>().sprite = _baseTexture;
            }
            if (_bulindingsGrid.grid[x + 1, y].GetComponent<SpriteRenderer>().sprite == _verticalTexture)
            {
                _bulindingsGrid.grid[x + 1, y].GetComponent<SpriteRenderer>().sprite = _baseTexture;
            }
        }
        else if (_bulindingsGrid.grid[x, y - 1] != null && _bulindingsGrid.grid[x, y + 1] && gameObject.name == _bulindingsGrid.grid[x, y - 1].name && gameObject.name == _bulindingsGrid.grid[x, y + 1].name)
        {
            _sr.sprite = _verticalTexture;
            if (_bulindingsGrid.grid[x, y - 1].GetComponent<SpriteRenderer>().sprite == _horizontalTexture)
            {
                _bulindingsGrid.grid[x, y - 1].GetComponent<SpriteRenderer>().sprite = _baseTexture;
            }
            if (_bulindingsGrid.grid[x, y + 1].GetComponent<SpriteRenderer>().sprite == _horizontalTexture)
            {
                _bulindingsGrid.grid[x, y + 1].GetComponent<SpriteRenderer>().sprite = _baseTexture;
            }
        }
    }
}
