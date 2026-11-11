using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FloorPassTrigger))]
public sealed class HelixFloorModule : MonoBehaviour
{
    private sealed class SectorVisual
    {
        public Transform Transform;
        public Renderer Renderer;
        public HazardSector Hazard;
    }

    private readonly List<SectorVisual> sectors = new List<SectorVisual>();
    private GameObject sectorPrefab;
    private SpeedBoostPickup speedBoostPickup;
    private GameProgression progression;
    private Material platformMaterial;
    private Material accentMaterial;
    private Color hazardColor;
    private Action<HelixFloorModule> passedCallback;
    private Action hazardCallback;
    private FloorPassTrigger passTrigger;
    private int sectorsPerFloor;
    private int gapWidth;
    private int firstGapSector;
    private int gapShiftPerFloor;
    private float sectorAngleOffsetPerFloor;
    private float triggerDepth;
    private Vector3 triggerSize;
    private int speedBoostEveryFloors;
    private float speedBoostMultiplier;
    private float speedBoostDuration;
    private float playerOrbitRadius;
    private Color speedBoostColor;
    private float localHeight;
    private int floorIndex;

    public float LocalHeight => localHeight;

    public void Initialize(Transform parent, GameObject prefab, int totalSectors, int widthOfGap,
        int initialGapSector, int gapShift, float angleOffsetPerFloor, float passTriggerDepth, Vector3 passTriggerSize,
        GameProgression gameProgression, Material normalMaterial, Material accentMaterial, Color dangerColor,
        int boostEveryFloors, float boostMultiplier, float boostDuration, float orbitRadius, Color boostColor,
        Action<HelixFloorModule> onPassed, Action onHazard)
    {
        transform.SetParent(parent, false);
        sectorPrefab = prefab;
        sectorsPerFloor = Mathf.Max(3, totalSectors);
        gapWidth = Mathf.Clamp(widthOfGap, 1, sectorsPerFloor - 2);
        firstGapSector = initialGapSector;
        gapShiftPerFloor = gapShift;
        sectorAngleOffsetPerFloor = angleOffsetPerFloor;
        triggerDepth = Mathf.Max(0f, passTriggerDepth);
        triggerSize = new Vector3(Mathf.Max(1f, passTriggerSize.x), Mathf.Max(0.1f, passTriggerSize.y),
            Mathf.Max(1f, passTriggerSize.z));
        progression = gameProgression;
        platformMaterial = normalMaterial;
        this.accentMaterial = accentMaterial;
        hazardColor = dangerColor;
        speedBoostEveryFloors = Mathf.Max(0, boostEveryFloors);
        speedBoostMultiplier = Mathf.Clamp(boostMultiplier, 1f, 3f);
        speedBoostDuration = Mathf.Clamp(boostDuration, 0.1f, 3f);
        playerOrbitRadius = Mathf.Max(0.5f, orbitRadius);
        speedBoostColor = boostColor;
        passedCallback = onPassed;
        hazardCallback = onHazard;

        EnsureSectorInstances();
        EnsurePassTrigger();
        EnsureSpeedBoostPickup();
    }

    public void ConfigureAt(float height, int logicalFloorIndex, int hazardSectorCount)
    {
        localHeight = height;
        floorIndex = Mathf.Max(0, logicalFloorIndex);
        transform.localPosition = new Vector3(0f, localHeight, 0f);

        int normalizedGapStart = Mod(firstGapSector + floorIndex * gapShiftPerFloor, sectorsPerFloor);
        int accentSector = Mod(normalizedGapStart + gapWidth + sectorsPerFloor / 2, sectorsPerFloor);
        int availableSectors = sectorsPerFloor - gapWidth;
        int accentSlot = Mod(accentSector - normalizedGapStart - gapWidth, sectorsPerFloor);
        int safeCapacityForHazards = Mathf.Max(0, availableSectors - 1);
        int hazardsToPlace = Mathf.Clamp(hazardSectorCount, 0, safeCapacityForHazards);

        List<int> hazardSlots = SelectHazardSlots(floorIndex, availableSectors, accentSlot, hazardsToPlace);
        for (int slotIndex = 0; slotIndex < sectors.Count; slotIndex++)
        {
            int sectorIndex = Mod(normalizedGapStart + gapWidth + slotIndex, sectorsPerFloor);
            SectorVisual sector = sectors[slotIndex];
            sector.Transform.name = $"Sector {sectorIndex + 1}";
            sector.Transform.localPosition = Vector3.zero;
            sector.Transform.localRotation = Quaternion.Euler(
                0f,
                sectorIndex * (360f / sectorsPerFloor) + floorIndex * sectorAngleOffsetPerFloor,
                0f) * sectorPrefab.transform.localRotation;
            sector.Transform.localScale = sectorPrefab.transform.localScale;

            bool isHazard = hazardSlots.Contains(slotIndex);
            if (sector.Renderer != null)
                sector.Renderer.sharedMaterial = sectorIndex == accentSector ? accentMaterial : platformMaterial;
            sector.Hazard.Configure(isHazard, hazardColor, hazardCallback);
        }

        if (speedBoostPickup != null)
        {
            bool floorHasBoost = speedBoostEveryFloors > 0
                && (floorIndex + 1) % speedBoostEveryFloors == 0;
            speedBoostPickup.Configure(new Vector3(0f, 0.55f, -playerOrbitRadius), 0.38f,
                speedBoostMultiplier, speedBoostDuration, speedBoostColor, floorHasBoost);
        }

        if (passTrigger != null)
            passTrigger.Configure(progression, transform.position.y, () => passedCallback?.Invoke(this));
    }

    private void EnsureSectorInstances()
    {
        int requiredSectors = sectorsPerFloor - gapWidth;
        if (sectorPrefab == null)
            return;

        if (sectors.Count == requiredSectors)
            return;

        for (int i = sectors.Count - 1; i >= 0; i--)
        {
            if (sectors[i].Transform != null)
                Destroy(sectors[i].Transform.gameObject);
        }
        sectors.Clear();

        for (int i = 0; i < requiredSectors; i++)
        {
            GameObject instance = Instantiate(sectorPrefab, transform, false);
            instance.name = $"Sector {i + 1}";
            MeshCollider meshCollider = instance.GetComponent<MeshCollider>();
            MeshFilter meshFilter = instance.GetComponent<MeshFilter>();
            if (meshCollider == null)
                meshCollider = instance.AddComponent<MeshCollider>();

            meshCollider.sharedMesh = meshFilter != null ? meshFilter.sharedMesh : null;
            meshCollider.convex = true;
            meshCollider.isTrigger = false;
            meshCollider.enabled = meshCollider.sharedMesh != null;

            Renderer sectorRenderer = instance.GetComponentInChildren<Renderer>();
            HazardSector hazard = instance.GetComponent<HazardSector>();
            if (hazard == null)
                hazard = instance.AddComponent<HazardSector>();

            if (sectorRenderer == null || !meshCollider.enabled)
                Debug.LogError($"El prefab de sector '{sectorPrefab.name}' necesita Renderer y MeshCollider con malla.", instance);

            sectors.Add(new SectorVisual
            {
                Transform = instance.transform,
                Renderer = sectorRenderer,
                Hazard = hazard
            });
        }
    }

    private void EnsurePassTrigger()
    {
        Transform triggerTransform = transform.Find("Floor Pass Trigger");
        if (triggerTransform == null)
        {
            GameObject triggerObject = new GameObject("Floor Pass Trigger", typeof(BoxCollider), typeof(FloorPassTrigger));
            triggerTransform = triggerObject.transform;
            triggerTransform.SetParent(transform, false);
        }

        triggerTransform.localPosition = new Vector3(0f, -triggerDepth, 0f);
        triggerTransform.localRotation = Quaternion.identity;
        BoxCollider box = triggerTransform.GetComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = triggerSize;
        passTrigger = triggerTransform.GetComponent<FloorPassTrigger>();
    }

    private void EnsureSpeedBoostPickup()
    {
        if (speedBoostEveryFloors <= 0 || speedBoostPickup != null)
            return;

        GameObject pickupObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pickupObject.name = "Speed Boost Pickup";
        pickupObject.transform.SetParent(transform, false);
        pickupObject.GetComponent<SphereCollider>().isTrigger = true;
        speedBoostPickup = pickupObject.AddComponent<SpeedBoostPickup>();
        speedBoostPickup.SetAvailable(false);
    }

    private static List<int> SelectHazardSlots(int seed, int count, int safeSlot, int hazardsToPlace)
    {
        List<int> candidates = new List<int>(Mathf.Max(0, count - 1));
        for (int slot = 0; slot < count; slot++)
        {
            if (slot != safeSlot)
                candidates.Add(slot);
        }

        System.Random random = new System.Random(seed);
        for (int i = candidates.Count - 1; i > 0; i--)
        {
            int swapIndex = random.Next(i + 1);
            (candidates[i], candidates[swapIndex]) = (candidates[swapIndex], candidates[i]);
        }

        if (candidates.Count > hazardsToPlace)
            candidates.RemoveRange(hazardsToPlace, candidates.Count - hazardsToPlace);
        return candidates;
    }

    private static int Mod(int value, int modulus)
    {
        int result = value % modulus;
        return result < 0 ? result + modulus : result;
    }
}
