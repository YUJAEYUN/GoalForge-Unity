using UnityEngine;
using UnityEditor;

public class GrassBackgroundPostprocessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (assetPath.Contains("GrassBackground.png"))
        {
            TextureImporter importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point; // For pixel art look
            importer.textureCompression = TextureImporterCompression.Uncompressed;
        }
    }
}
