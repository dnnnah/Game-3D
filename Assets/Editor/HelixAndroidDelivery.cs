using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class HelixAndroidDelivery
{
    private const string LegacyIconPath = "Assets/Icons/Helix3D-AppIcon.png";
    private const string ForegroundIconPath = "Assets/Icons/Helix3D-AppIcon-Foreground.png";
    private const string BackgroundIconPath = "Assets/Icons/Helix3D-AppIcon-Background.png";
    private const string ApkPath = "Builds/Android/Helix3D.apk";

    [MenuItem("Tools/Helix Jump/Configure Android Player Settings")]
    public static void ConfigureAndroidPlayerSettings()
    {
        NamedBuildTarget android = NamedBuildTarget.Android;
        PlayerSettings.productName = "Helix 3D";
        PlayerSettings.SetApplicationIdentifier(android, "dnnnah.udg");
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        PlayerSettings.SetScriptingBackend(android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
        PlayerSettings.Android.buildApkPerCpuArchitecture = false;

        Texture2D legacyIcon = LoadIcon(LegacyIconPath, false);
        Texture2D foregroundIcon = LoadIcon(ForegroundIconPath, true);
        Texture2D backgroundIcon = LoadIcon(BackgroundIconPath, false);

        PlatformIconKind legacyKind = GetAndroidIconKind("Legacy");
        PlatformIconKind roundKind = GetAndroidIconKind("Round");
        PlatformIconKind adaptiveKind = GetAndroidIconKind("Adaptive");
        SetIconKind(legacyKind, legacyIcon);
        SetIconKind(roundKind, legacyIcon);
        SetAdaptiveIcons(adaptiveKind, foregroundIcon, backgroundIcon);

        AssetDatabase.SaveAssets();
        Debug.Log("Android Player Settings listos: dnnnah.udg, API 26+, IL2CPP, ARMv7 y ARM64.");
    }

    [MenuItem("Tools/Helix Jump/Build Android APK")]
    public static void BuildAndroidApk()
    {
        ConfigureAndroidPlayerSettings();

        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
            throw new InvalidOperationException("Unity no pudo activar la plataforma Android.");

        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
        string[] requiredScenes =
        {
            "Assets/Scenes/Main.unity",
            "Assets/Scenes/Level1.unity",
            "Assets/Scenes/Level2.unity",
            "Assets/Scenes/Level3.unity"
        };
        foreach (string requiredScene in requiredScenes)
        {
            if (!scenes.Contains(requiredScene))
                throw new InvalidOperationException($"Falta habilitar {requiredScene} en Build Settings.");
        }

        Directory.CreateDirectory(Path.GetDirectoryName(ApkPath));
        EditorUserBuildSettings.buildAppBundle = false;
        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = ApkPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        });

        if (report.summary.result != BuildResult.Succeeded)
            throw new InvalidOperationException($"La compilación Android terminó con estado {report.summary.result}.");

        Debug.Log($"APK creado: {Path.GetFullPath(ApkPath)} ({report.summary.totalSize} bytes).");
    }

    private static Texture2D LoadIcon(string path, bool needsAlpha)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            throw new FileNotFoundException($"No se encontró una textura importada para {path}.");

        bool changed = importer.textureType != TextureImporterType.Default
            || importer.alphaIsTransparency != needsAlpha
            || importer.maxTextureSize < 1024
            || importer.textureCompression != TextureImporterCompression.Uncompressed;
        importer.textureType = TextureImporterType.Default;
        importer.alphaIsTransparency = needsAlpha;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        if (changed)
            importer.SaveAndReimport();

        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (icon == null || icon.width < 1024 || icon.height < 1024)
            throw new InvalidOperationException($"El ícono {path} debe medir al menos 1024×1024 px.");
        return icon;
    }

    private static void SetIconKind(PlatformIconKind kind, Texture2D iconTexture)
    {
        PlatformIcon[] icons = PlayerSettings.GetPlatformIcons(NamedBuildTarget.Android, kind);
        if (icons == null || icons.Length == 0)
            throw new InvalidOperationException($"Unity no expone espacios de ícono para Android ({kind}).");

        for (int i = 0; i < icons.Length; i++)
            icons[i].SetTextures(iconTexture);

        PlayerSettings.SetPlatformIcons(NamedBuildTarget.Android, kind, icons);
    }

    private static void SetAdaptiveIcons(PlatformIconKind adaptiveKind, Texture2D foreground, Texture2D background)
    {
        PlatformIcon[] icons = PlayerSettings.GetPlatformIcons(NamedBuildTarget.Android, adaptiveKind);
        if (icons == null || icons.Length == 0)
            throw new InvalidOperationException("Unity no expone espacios adaptativos para Android.");

        for (int i = 0; i < icons.Length; i++)
        {
            if (icons[i].maxLayerCount != 2)
                throw new InvalidOperationException("El ícono adaptativo de Android requiere dos capas.");
            icons[i].SetTextures(foreground, background);
        }

        PlayerSettings.SetPlatformIcons(NamedBuildTarget.Android, adaptiveKind, icons);
    }

    private static PlatformIconKind GetAndroidIconKind(string kindName)
    {
        Type kindProvider = Type.GetType(
            "UnityEditor.Android.AndroidPlatformIconKind, UnityEditor.Android.Extensions", false);
        if (kindProvider == null)
            throw new InvalidOperationException("Instala Android Build Support desde Unity Hub antes de asignar los íconos.");

        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
        FieldInfo field = kindProvider.GetField(kindName, flags);
        if (field != null)
            return (PlatformIconKind)field.GetValue(null);

        PropertyInfo property = kindProvider.GetProperty(kindName, flags);
        if (property != null)
            return (PlatformIconKind)property.GetValue(null);

        throw new MissingMemberException(kindProvider.FullName, kindName);
    }
}
