using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public float floatSpeed = 1f;
    public float duration = 1f;

    private void Start()
    {
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    public void SetDamage(int damage)
    {
        damageText.text = damage.ToString();
    }
}
