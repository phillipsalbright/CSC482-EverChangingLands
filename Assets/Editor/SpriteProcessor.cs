using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using static TreeEditor.TextureAtlas;
using Unity.VisualScripting;

public class SpriteProcessor : AssetPostprocessor
{

    private static readonly string _TILE_SPRITE_FOLDER = "Resources/TileSprites";
    private static readonly string _TILE_FOLDER = "Resources/Tiles";

    private void OnPreprocessTexture()
    {
        TextureImporter texture = (TextureImporter)assetImporter;
        if (texture.assetPath.Contains(_TILE_SPRITE_FOLDER))
        {
            texture.textureType = TextureImporterType.Sprite;
            texture.spriteImportMode = SpriteImportMode.Single;
            texture.spritePixelsPerUnit = 64;
            texture.filterMode = FilterMode.Point;
            texture.SaveAndReimport();
            string textureName = Path.GetFileNameWithoutExtension(texture.assetPath);
            IsometricRuleTile isometricRuleTile = new IsometricRuleTile();
            string loc = Path.Combine("Assets", _TILE_FOLDER, textureName) + ".asset";
            if (!File.Exists(loc))
            {
                AssetDatabase.CreateAsset(isometricRuleTile, loc);

            }
        }
    }
}
