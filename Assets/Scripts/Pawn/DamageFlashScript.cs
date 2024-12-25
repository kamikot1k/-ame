using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlashScript : MonoBehaviour
{
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashTime = 0.25f;

    private SpriteRenderer _sr;
    private Material _material;

    private Coroutine _damageFlashCoroutine;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();

        Init();
    }

    private void Init()
    {
        _material = _sr.material;
    }

    public void callDamageFlash()
    {
        _damageFlashCoroutine = StartCoroutine(DamageFlasher());
    } 

    private IEnumerator DamageFlasher()
    {
        _material.SetColor("_FlashColor", flashColor);

        float currFlashAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            currFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / flashTime));

            _material.SetFloat("_FlashAmount", currFlashAmount);

            yield return null;
        }
    }
}
