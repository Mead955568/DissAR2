using UnityEngine;

public class FlashingBall : MonoBehaviour
{
    private float flashInterval = 1f;
    private float colorChangeInterval = 1f;
    private float elapsedTime = 0f;
    private Renderer renderer;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        InvokeRepeating("Flash", flashInterval, flashInterval);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= colorChangeInterval)
        {
            ChangeColor();
            elapsedTime = 0f;
        }
    }

    private void Flash()
    {
        renderer.enabled = !renderer.enabled;
    }

    private void ChangeColor()
    {
        Color newColor = new Color(Random.value, Random.value, Random.value);
        renderer.material.color = newColor;
    }
}
