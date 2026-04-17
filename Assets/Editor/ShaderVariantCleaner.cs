using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Rendering;
using TMPro;

public class ShaderVariantCleaner : EditorWindow
{
    [MenuItem("Tools/Shader Variant Cleaner")]
    public static void ShowWindow()
    {
        GetWindow<ShaderVariantCleaner>("Shader Cleaner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Shader Variant Explosion Cleaner", EditorStyles.boldLabel);

        if (GUILayout.Button("Scan Project"))
        {
            ScanProject();
        }

        if (GUILayout.Button("Fix All (Safe Mode)"))
        {
            FixAll();
        }
    }

    static List<Material> allMaterials = new List<Material>();

    static void ScanProject()
    {
        allMaterials.Clear();

        string[] guids = AssetDatabase.FindAssets("t:Material");

        int tmpLitCount = 0;
        int urpLitCount = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat == null) continue;

            allMaterials.Add(mat);

            if (mat.shader == null) continue;

            string shaderName = mat.shader.name;

            if (shaderName.Contains("TMP") && shaderName.Contains("Lit"))
            {
                tmpLitCount++;
                Debug.LogWarning("TMP Lit found: " + path);
            }

            if (shaderName.Contains("Universal Render Pipeline/Lit"))
            {
                urpLitCount++;
                Debug.LogWarning("URP Lit found: " + path);
            }
        }

        Debug.Log($"Scan Complete -> TMP Lit: {tmpLitCount}, URP Lit: {urpLitCount}");
    }

    static void FixAll()
    {
        int fixedCount = 0;

        foreach (Material mat in allMaterials)
        {
            if (mat == null) continue;

            string shaderName = mat.shader.name;

            // 🔥 Fix TMP Lit (核心问题)
            if (shaderName.Contains("TMP") && shaderName.Contains("Lit"))
            {
                Shader safeShader = Shader.Find("TextMeshPro/Distance Field");
                if (safeShader != null)
                {
                    mat.shader = safeShader;
                    EditorUtility.SetDirty(mat);
                    fixedCount++;
                    Debug.Log("Fixed TMP Lit -> Unlit: " + mat.name);
                }
            }

            // 🔥 Fix URP Lit (降低 variant)
            if (shaderName.Contains("Universal Render Pipeline/Lit"))
            {
                Shader safeShader = Shader.Find("Universal Render Pipeline/Unlit");
                if (safeShader != null)
                {
                    mat.shader = safeShader;
                    EditorUtility.SetDirty(mat);
                    fixedCount++;
                    Debug.Log("Fixed URP Lit -> Unlit: " + mat.name);
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Fix Complete! Total fixed: " + fixedCount);
    }
}