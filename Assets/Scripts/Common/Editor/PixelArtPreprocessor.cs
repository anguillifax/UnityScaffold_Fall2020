using UnityEditor;
using UnityEngine;

namespace Scaffold
{
	// An asset processor automatically sets properties of an imported asset.
	//
	// This preprocessor sets many defaults that are helpful when working with
	// pixel art.
	//
	// For more information on how to automate importing, check out the Unity
	// docs.

	public class PixelArtPreprocessor : AssetPostprocessor
	{
		private void OnPreprocessTexture()
		{
			if (assetPath.Contains("!! Import Pixel Art"))
			{
				TextureImporter importer = (TextureImporter)assetImporter;

				// CHANGE THIS NUMBER ACCORDING TO PROJECT
				importer.spritePixelsPerUnit = 32;
				
				importer.textureCompression = TextureImporterCompression.Uncompressed;
				importer.filterMode = FilterMode.Point;
				importer.wrapMode = TextureWrapMode.Repeat;

				TextureImporterSettings settings = new TextureImporterSettings();
				importer.ReadTextureSettings(settings);
				settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
				settings.spriteMeshType = SpriteMeshType.FullRect;
				settings.spriteExtrude = 0;
				importer.SetTextureSettings(settings);
			}
		}
	}
}
