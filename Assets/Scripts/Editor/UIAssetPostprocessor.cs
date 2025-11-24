using UnityEngine;
using UnityEditor;

public class UIAssetPostprocessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (assetPath.Contains("Assets/Art/UI/"))
        {
            TextureImporter importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false; // UI sprites usually don't need mipmaps
            
            // Optional: Set compression to High Quality for better visuals
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
        }
    }
}
