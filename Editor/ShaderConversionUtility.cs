using UnityEngine;
using UnityEditor;

public class ShaderConversionUtility : MonoBehaviour
{
    [MenuItem("Tools/Convert URP Materials to HDRP")]
    public static void ConvertMaterialsToHDRP()
    {
        ConvertMaterials("Universal Render Pipeline/Lit", "HDRP/Lit", true);
    }

    [MenuItem("Tools/Convert HDRP Materials to URP")]
    public static void ConvertMaterialsToURP()
    {
        ConvertMaterials("HDRP/Lit", "Universal Render Pipeline/Lit", false);
    }

    private static void ConvertMaterials(string sourceShaderName, string targetShaderName, bool toHDRP)
    {
        var guids = AssetDatabase.FindAssets("t:Material");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (material.shader.name == sourceShaderName)
            {
                // Сохраняем текущие текстуры
                Texture baseColorMap = toHDRP ? material.GetTexture("_BaseMap") : material.GetTexture("_BaseColorMap");
                Texture normalMap = toHDRP ? material.GetTexture("_BumpMap") : material.GetTexture("_NormalMap");
                Texture emissionMap = toHDRP ? material.GetTexture("_EmissionMap") : material.GetTexture("_EmissiveColorMap");

                // Изменяем шейдер
                material.shader = Shader.Find(targetShaderName);

                // Переносим текстуры
                if (toHDRP)
                {
                    // URP -> HDRP
                    material.SetTexture("_BaseColorMap", baseColorMap);
                    material.SetTexture("_NormalMap", normalMap);
                    material.SetTexture("_EmissiveColorMap", emissionMap);
                }
                else
                {
                    // HDRP -> URP
                    material.SetTexture("_BaseMap", baseColorMap);
                    material.SetTexture("_BumpMap", normalMap);
                    material.SetTexture("_EmissionMap", emissionMap);
                }

                Debug.Log("Конвертирован материал: " + material.name);
            }
        }
        Debug.Log("Конвертация завершена.");
    }
}
