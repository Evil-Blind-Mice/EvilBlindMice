using System.Collections;
using UnityEngine;

public class EnemyDamageHandler : MonoBehaviour, IDamage
{
    [SerializeField] EnemyStats stats;

    [Header("Feedback")]
    [SerializeField] float flashDuration;
    enum FlashType { material, obj};
    [SerializeField] FlashType flashType;

    [Header("Object Flash")]
    [SerializeField] GameObject flashObject;

    [Header("MaterialFlash")]
    [SerializeField] Material flashMaterial;
    [SerializeField] MeshRenderer renderer;
    Material initialMaterial;

    private void Start()
    {
        if (flashType == FlashType.material) initialMaterial = renderer.material;
    }

    public void TakeDamage(int _amount)
    {
        stats.ModifyHealth(-_amount);

        DamageFlash();

        if (stats.GetHealth() <= 0)
        {
            GameManager.instance.UpdateScore(stats.pointValue);
            Destroy(stats.gameObject);
        }
    }

    void DamageFlash()
    {
        if (flashType == FlashType.obj) StartCoroutine(DamageFlashObj());
        else if (flashType == FlashType.material) StartCoroutine(DamageFlashMaterial());
    }

    IEnumerator DamageFlashObj()
    {
        flashObject.SetActive(true);
        yield return new WaitForSeconds(flashDuration);
        flashObject.SetActive(false);
    }

    IEnumerator DamageFlashMaterial()
    {
        renderer.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        renderer.material = initialMaterial;
    }
}
