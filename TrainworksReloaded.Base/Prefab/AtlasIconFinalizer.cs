using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TrainworksReloaded.Base.Prefab
{
    public class AtlasIconFinalizer(
        IRegister<Texture2D> iconRegister,
        ICache<IDefinition<Texture2D>> cache
        ) : IDataFinalizer
    {
        private readonly ICache<IDefinition<Texture2D>> cache = cache;
        private readonly IRegister<Texture2D> iconRegister = iconRegister;
        private const int MAX_TEXTURE_SIZE = 480;

        public void FinalizeData()
        {
            List<Texture2D> textures = [];
            foreach (var definition in cache.GetCacheItems())
            {
                textures.Add(definition.Data);
            }
            cache.Clear();

            var SpriteAtlas = new Texture2D(MAX_TEXTURE_SIZE, MAX_TEXTURE_SIZE);

            Rect[] rects = SpriteAtlas.PackTextures([.. textures], 0);

            var spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
            spriteAsset.name = "Trainworks Sprite Atlas";
            spriteAsset.spriteSheet = SpriteAtlas;
            spriteAsset.material = new Material(Shader.Find("TextMeshPro/Sprite"))
            {
                mainTexture = SpriteAtlas
            };
            spriteAsset.spriteInfoList = [];

            for (int j = 0; j < rects.Length; j++)
            {
                var texture = textures[j];
                var rect = rects[j];

                TMP_Sprite sprite = new()
                {
                    x = rect.x * SpriteAtlas.width,
                    y = rect.y * SpriteAtlas.height
                };
                sprite.sprite = Sprite.Create(SpriteAtlas, new Rect(sprite.x, sprite.y, texture.width, texture.height), new Vector2(0.5f, 0.5f), 128f);
                sprite.name = texture.name;
                sprite.unicode = 0;
                sprite.width = texture.width;
                sprite.height = texture.height;
                sprite.xOffset = 0;
                sprite.yOffset = 20; // Hardcoded constant to line up with the rest of the text.
                sprite.xAdvance = texture.width;
                sprite.scale = 1;

                spriteAsset.spriteInfoList.Add(sprite);
            }

            spriteAsset.UpdateLookupTables();

            TMP_Settings.defaultSpriteAsset.fallbackSpriteAssets.Add(spriteAsset);
        }
    }
}
