using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthbar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);

    private Transform target;

    public void Init(Transform followTarget)
    {
        target = followTarget;
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }

    void LateUpdate()
    {
        if (!target) return;

        transform.position = target.position + offset;
        transform.rotation = Quaternion.identity; // bleibt IMMER gerade
    }
}
