using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class HazardSector : MonoBehaviour
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private MaterialPropertyBlock propertyBlock;
    private MeshCollider sectorCollider;
    private Renderer sectorRenderer;
    private Action hazardCallback;
    private bool isDangerous;
    private bool reportedThisContact;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        sectorCollider = GetComponent<MeshCollider>();
        sectorRenderer = GetComponentInChildren<Renderer>();
    }

    public void Configure(bool dangerous, Color dangerColor, Action onHazardContact)
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
        if (sectorCollider == null)
            sectorCollider = GetComponent<MeshCollider>();
        if (sectorRenderer == null)
            sectorRenderer = GetComponentInChildren<Renderer>();

        isDangerous = dangerous;
        hazardCallback = onHazardContact;
        reportedThisContact = false;

        if (sectorCollider != null)
            sectorCollider.isTrigger = isDangerous;

        if (sectorRenderer != null)
        {
            if (!isDangerous)
            {
                sectorRenderer.SetPropertyBlock(null);
            }
            else
            {
                sectorRenderer.GetPropertyBlock(propertyBlock);
                int colorProperty = sectorRenderer.sharedMaterial != null
                    && sectorRenderer.sharedMaterial.HasProperty(BaseColorId)
                    ? BaseColorId
                    : ColorId;
                propertyBlock.SetColor(colorProperty, dangerColor);
                sectorRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        enabled = isDangerous;
    }

    private void OnTriggerEnter(Collider other) => TryReportHazard(other);
    private void OnTriggerStay(Collider other) => TryReportHazard(other);

    private void TryReportHazard(Collider other)
    {
        if (!isDangerous || reportedThisContact)
            return;

        if (other.GetComponentInParent<PlayerMovement>() == null)
            return;

        reportedThisContact = true;
        hazardCallback?.Invoke();
    }
}
