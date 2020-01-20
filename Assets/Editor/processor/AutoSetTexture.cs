using UnityEditor;

public class AutoSetTexture : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        //自动设置类型;
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.mipmapEnabled = false;
        textureImporter.isReadable = false;
        textureImporter.npotScale = TextureImporterNPOTScale.None;
    }

    // void OnPostprocessTexture()
    // {
    
    // }
}