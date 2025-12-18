using System;
using UnityEngine;
using Random = UnityEngine.Random;  // Resolver ambigüedad de Random
using Vector3 = UnityEngine.Vector3;  // Resolver ambigüedad de Vector3

public class VegetationPlacer : MonoBehaviour
{
    [Header("Referencias")]
    public Terrain terrain;
    public Texture2D vegetationMask; // Tu máscara desde QGIS

    [Header("Configuración de Árboles")]
    public int numberOfTrees = 1000;
    [Range(0f, 1f)]
    public float minMaskValue = 0.5f; // Umbral mínimo de vegetación

    [Header("Filtros Adicionales")]
    [Range(0f, 1f)]
    public float minHeight = 0.05f; // Altura mínima normalizada
    [Range(0f, 1f)]
    public float maxHeight = 0.85f; // Altura máxima normalizada
    public float maxSteepness = 30f; // Pendiente máxima en grados

    [Header("Variación")]
    public float minTreeScale = 0.8f;
    public float maxTreeScale = 1.2f;

    [ContextMenu("Colocar Árboles")]
    public void PlaceTrees()
    {
        if (terrain == null || vegetationMask == null)
        {
            UnityEngine.Debug.LogError("Falta asignar Terrain o Máscara de Vegetación");
            return;
        }

        TerrainData terrainData = terrain.terrainData;

        // Verificar que hay prototipos de árboles configurados
        if (terrainData.treePrototypes.Length == 0)
        {
            UnityEngine.Debug.LogError("No hay Tree Prototypes configurados en el Terrain. " +
                "Ve a Paint Trees > Edit Trees > Add Tree para agregar árboles.");
            return;
        }

        // Limpiar árboles existentes
        terrain.terrainData.treeInstances = new TreeInstance[0];

        int treesPlaced = 0;
        int attempts = 0;
        int maxAttempts = numberOfTrees * 10; // Evitar bucle infinito

        UnityEngine.Debug.Log("Iniciando colocación de árboles...");

        while (treesPlaced < numberOfTrees && attempts < maxAttempts)
        {
            attempts++;

            // Generar posición aleatoria normalizada (0-1)
            float x = Random.Range(0f, 1f);
            float z = Random.Range(0f, 1f);

            // 1. Verificar máscara de vegetación
            Color maskColor = vegetationMask.GetPixelBilinear(x, z);
            if (maskColor.r < minMaskValue)
                continue; // No hay vegetación suficiente aquí

            // 2. Verificar altura del terreno
            float height = terrainData.GetInterpolatedHeight(x, z) / terrainData.size.y;
            if (height < minHeight || height > maxHeight)
                continue; // Fuera del rango de altura permitido

            // 3. Verificar pendiente
            float steepness = terrainData.GetSteepness(x, z);
            if (steepness > maxSteepness)
                continue; // Demasiado empinado

            // 4. Crear instancia de árbol
            TreeInstance tree = new TreeInstance();
            tree.position = new Vector3(x, 0, z);
            tree.prototypeIndex = Random.Range(0, terrainData.treePrototypes.Length);
            tree.widthScale = Random.Range(minTreeScale, maxTreeScale);
            tree.heightScale = Random.Range(minTreeScale, maxTreeScale);
            tree.rotation = Random.Range(0f, Mathf.PI * 2f);
            tree.color = Color.white;
            tree.lightmapColor = Color.white;

            terrain.AddTreeInstance(tree);
            treesPlaced++;
        }

        terrain.Flush();

        if (treesPlaced < numberOfTrees)
        {
            UnityEngine.Debug.LogWarning($"Solo se colocaron {treesPlaced} de {numberOfTrees} árboles solicitados. " +
                "Considera ajustar los parámetros (minMaskValue, altura, pendiente) o aumentar el área de vegetación.");
        }
        else
        {
            UnityEngine.Debug.Log($"¡Colocación completada! Se colocaron {treesPlaced} árboles exitosamente.");
        }
    }

    [ContextMenu("Limpiar Árboles")]
    public void ClearTrees()
    {
        if (terrain == null)
        {
            UnityEngine.Debug.LogError("Falta asignar Terrain");
            return;
        }

        terrain.terrainData.treeInstances = new TreeInstance[0];
        terrain.Flush();
        UnityEngine.Debug.Log("Todos los árboles han sido eliminados");
    }

    [ContextMenu("Mostrar Info de Máscara")]
    public void ShowMaskInfo()
    {
        if (vegetationMask == null)
        {
            UnityEngine.Debug.LogError("No hay máscara asignada");
            return;
        }

        UnityEngine.Debug.Log($"Máscara de Vegetación Info:\n" +
            $"Dimensiones: {vegetationMask.width}x{vegetationMask.height}\n" +
            $"Formato: {vegetationMask.format}\n" +
            $"Read/Write Enabled: {vegetationMask.isReadable}");

        if (!vegetationMask.isReadable)
        {
            UnityEngine.Debug.LogWarning("¡ADVERTENCIA! La textura no tiene Read/Write habilitado. " +
                "Selecciona la textura y marca 'Read/Write Enabled' en el Inspector.");
        }
    }

    [ContextMenu("Diagnosticar Máscara")]
    public void DiagnoseMask()
    {
        if (vegetationMask == null)
        {
            UnityEngine.Debug.LogError("No hay máscara asignada");
            return;
        }

        if (!vegetationMask.isReadable)
        {
            UnityEngine.Debug.LogError("La textura no tiene Read/Write habilitado!");
            return;
        }

        // Muestrear varios puntos de la máscara
        int samples = 20;
        int greenPixels = 0;

        UnityEngine.Debug.Log("=== DIAGNÓSTICO DE MÁSCARA ===");

        for (int i = 0; i < samples; i++)
        {
            float x = Random.Range(0f, 1f);
            float z = Random.Range(0f, 1f);
            Color pixel = vegetationMask.GetPixelBilinear(x, z);

            UnityEngine.Debug.Log($"Muestra {i}: Posición ({x:F2}, {z:F2}) - " +
                $"Color RGB({pixel.r:F3}, {pixel.g:F3}, {pixel.b:F3})");

            if (pixel.r > 0.1f || pixel.g > 0.1f || pixel.b > 0.1f)
                greenPixels++;
        }

        UnityEngine.Debug.Log($"Píxeles con vegetación detectados: {greenPixels}/{samples}");
        UnityEngine.Debug.Log($"Umbral actual: {minMaskValue}");
    }

}
