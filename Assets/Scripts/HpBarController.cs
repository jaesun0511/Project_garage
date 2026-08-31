using UnityEngine;
using UnityEngine.UI;

public class HpBarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Character targetCharacter;
    [SerializeField] private Image fillImage;

    [Header("Settings")]
    [SerializeField] private bool useSmoothFill = false;
    [SerializeField] private float smoothSpeed = 5f;

    private float currentFill;

    private void Start()
    {
        if (targetCharacter == null)
        {
            targetCharacter = GetComponentInParent<Character>();
        }

        if (fillImage == null)
        {
            fillImage = GetComponent<Image>();
        }

        UpdateHpBar();
    }

    private void Update()
    {
        if (targetCharacter == null || fillImage == null)
        {
            return;
        }

        float targetFill = Mathf.Clamp01(targetCharacter.currentHp / Mathf.Max(targetCharacter.maxHp, 0.0001f));

        if (useSmoothFill)
        {
            currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * smoothSpeed);
        }
        else
        {
            currentFill = targetFill;
        }

        fillImage.fillAmount = currentFill;
    }

    public void SetTarget(Character character)
    {
        targetCharacter = character;
        UpdateHpBar();
    }

    public void UpdateHpBar()
    {
        if (targetCharacter == null || fillImage == null)
        {
            return;
        }

        currentFill = Mathf.Clamp01(targetCharacter.currentHp / Mathf.Max(targetCharacter.maxHp, 0.0001f));
        fillImage.fillAmount = currentFill;
    }
}
