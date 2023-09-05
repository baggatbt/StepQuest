using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class DamagePopup : MonoBehaviour
{
    public float floatSpeed = 0.5f; 
    public float fadeSpeed = 1f;
    private TextMeshProUGUI textMesh;
    private Color textColor;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textColor = textMesh.color;
    }

    public void Setup(int damageAmount)
    {
        textMesh.text = damageAmount.ToString();
    }

    private void Update()
    {
        // Move text up and fade it out
        transform.position += new Vector3(0, floatSpeed * Time.deltaTime, 0);
        textColor.a -= fadeSpeed * Time.deltaTime;
        textMesh.color = textColor;

        if (textColor.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
