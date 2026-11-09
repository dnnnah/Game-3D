using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class ProceduralLevelGenerator : MonoBehaviour
{
    private readonly List<HelixFloorModule> modules = new List<HelixFloorModule>();
    private GameObject sectorPrefab;
    private Transform towerRoot;
    private Transform centralColumn;
    private GameProgression progression;
    private Material platformMaterial;
    private Material accentMaterial;
    private Color hazardColor;
    private System.Action hazardCallback;
    private int activeModuleCount;
    private int sectorsPerFloor;
    private int gapWidth;
    private int firstGapSector;
    private int gapShiftPerFloor;
    private float floorSpacing;
    private float sectorAngleOffsetPerFloor;
    private float passTriggerDepth;
    private Vector3 passTriggerSize;
    private float columnEndPadding;
    private int nextFloorIndex;
    private bool initialized;

    public int ActiveModuleCount => modules.Count;

    public void Configure(GameObject prefab, Transform tower, Transform column, GameProgression gameProgression,
        int moduleCount, int sectorCount, int widthOfGap, int initialGapSector, int gapShift, float spacing,
        float angleOffsetPerFloor, float triggerDepth, Vector3 triggerSize, float columnPadding,
        Material normalMaterial, Material yellowMaterial, Color dangerColor, System.Action onHazardContact)
    {
        sectorPrefab = prefab;
        towerRoot = tower;
        centralColumn = column;
        progression = gameProgression;
        activeModuleCount = Mathf.Max(3, moduleCount);
        sectorsPerFloor = Mathf.Max(3, sectorCount);
        gapWidth = Mathf.Clamp(widthOfGap, 1, sectorsPerFloor - 2);
        firstGapSector = initialGapSector;
        gapShiftPerFloor = gapShift;
        floorSpacing = Mathf.Max(0.5f, spacing);
        sectorAngleOffsetPerFloor = angleOffsetPerFloor;
        passTriggerDepth = Mathf.Max(0f, triggerDepth);
        passTriggerSize = triggerSize;
        columnEndPadding = Mathf.Max(0f, columnPadding);
        platformMaterial = normalMaterial;
        accentMaterial = yellowMaterial;
        hazardColor = dangerColor;
        hazardCallback = onHazardContact;
    }

    public bool Initialize()
    {
        if (initialized)
            return true;

        if (sectorPrefab == null || towerRoot == null)
        {
            Debug.LogError("El generador procedural necesita un prefab de sector y la raíz de la torre.", this);
            return false;
        }

        RemoveEditorPreviewFloors();
        modules.Clear();
        nextFloorIndex = 0;

        for (int i = 0; i < activeModuleCount; i++)
        {
            GameObject moduleObject = new GameObject($"Floor Module {i + 1}");
            moduleObject.transform.SetParent(towerRoot, false);
            HelixFloorModule module = moduleObject.AddComponent<HelixFloorModule>();
            module.Initialize(towerRoot, sectorPrefab, sectorsPerFloor, gapWidth, firstGapSector, gapShiftPerFloor,
                sectorAngleOffsetPerFloor, passTriggerDepth, passTriggerSize, progression,
                platformMaterial, accentMaterial, hazardColor, OnFloorPassed, hazardCallback);
            modules.Add(module);
        }

        initialized = true;
        ResetForRun();
        return true;
    }

    public void ResetForRun()
    {
        if (!initialized && !Initialize())
            return;

        nextFloorIndex = activeModuleCount;
        for (int i = 0; i < modules.Count; i++)
        {
            float height = (activeModuleCount - 1 - i) * floorSpacing;
            modules[i].ConfigureAt(height, i, progression != null ? progression.HazardSectorsPerFloor : 0);
        }

        UpdateColumnBounds();
        Physics.SyncTransforms();
    }

    private void OnFloorPassed(HelixFloorModule passedModule)
    {
        if (!initialized || passedModule == null)
            return;

        float lowestHeight = float.PositiveInfinity;
        for (int i = 0; i < modules.Count; i++)
            lowestHeight = Mathf.Min(lowestHeight, modules[i].LocalHeight);

        float recycledHeight = lowestHeight - floorSpacing;
        int recycledFloorIndex = nextFloorIndex++;
        int hazardCount = progression != null ? progression.HazardSectorsPerFloor : 0;
        passedModule.ConfigureAt(recycledHeight, recycledFloorIndex, hazardCount);
        UpdateColumnBounds();
        Physics.SyncTransforms();
    }

    private void UpdateColumnBounds()
    {
        if (centralColumn == null || modules.Count == 0)
            return;

        float highestHeight = float.NegativeInfinity;
        float lowestHeight = float.PositiveInfinity;
        for (int i = 0; i < modules.Count; i++)
        {
            highestHeight = Mathf.Max(highestHeight, modules[i].LocalHeight);
            lowestHeight = Mathf.Min(lowestHeight, modules[i].LocalHeight);
        }

        float halfHeight = (highestHeight - lowestHeight) * 0.5f + columnEndPadding;
        centralColumn.localPosition = new Vector3(0f, (highestHeight + lowestHeight) * 0.5f, 0f);
        centralColumn.localScale = new Vector3(1.35f, Mathf.Max(1f, halfHeight), 1.35f);
    }

    private void RemoveEditorPreviewFloors()
    {
        if (towerRoot == null)
            return;

        List<GameObject> previewObjects = new List<GameObject>();
        foreach (Transform child in towerRoot)
        {
            bool isSector = child.name.StartsWith("Floor ", System.StringComparison.Ordinal)
                && child.name.Contains(" · Sector ");
            bool isTrigger = child.name.StartsWith("Floor Pass Trigger ", System.StringComparison.Ordinal);
            if (!isSector && !isTrigger)
                continue;

            foreach (Collider collider in child.GetComponentsInChildren<Collider>(true))
                collider.enabled = false;
            previewObjects.Add(child.gameObject);
        }

        for (int i = 0; i < previewObjects.Count; i++)
        {
            if (Application.isPlaying)
                Destroy(previewObjects[i]);
            else
                DestroyImmediate(previewObjects[i]);
        }
    }
}
