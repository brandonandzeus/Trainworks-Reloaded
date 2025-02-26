using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace TrainworksReloaded.Base.Prefab
{
    public class TextureImportCardArtSetup : IDataPipelineSetup<TextureImport>
    {
        public void Setup(IDefinition<TextureImport> definition)
        {
            var type = definition.Configuration.GetSection("type").Value;
            if (type != "card_art")
            {
                return;
            }

            var gameObject = definition.Data.GameObject;
            var texture2d = definition.Data.Texture2D;

            var prefab = gameObject.AddComponent<AddressableAssetPrefab>();

            gameObject.AddComponent<RectTransform>();
            var cardArt = new GameObject { name = "CardSprite" };
            cardArt.transform.SetParent(gameObject.transform);
            var canvasRenderer = cardArt.AddComponent<CanvasRenderer>();
            //var spriteRenderer = cardArt.AddComponent<SpriteRenderer>();

            var image = cardArt.AddComponent<Image>();
            var sprite = Sprite.Create(
                texture2d,
                new Rect(0, 0, texture2d.width, texture2d.height),
                new Vector2(0.5f, 0.5f),
                128f
            );
            image.sprite = sprite;
            image.preserveAspect = true;
            image.SetNativeSize();

            var rectTransform = cardArt.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = Vector2.zero; // Bottom-left corner
                rectTransform.anchorMax = Vector2.one; // Top-right corner
                rectTransform.offsetMin = Vector2.zero; // Zero out offsets
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.pivot = new Vector2(0.5f, 0.5f); // Center pivot
            }

            var material = new Material(Shader.Find("Shiny Shoe/CardEffects"));
            //var material = new Material(Shader.Find("Sprites/Default"));
            material.mainTexture = sprite.texture;
            material.SetTexture("_Layer1Tex", sprite.texture);
            image.material = material;
            //spriteRenderer.material = material;
            //spriteRenderer.sprite = sprite;
            canvasRenderer.materialCount = 1;
            canvasRenderer.SetMaterial(material, 0);

            var cardEffectTransforms = new GameObject { name = "CardEffectTransforms" };
            cardEffectTransforms.transform.SetParent(gameObject.transform);
        }
    }
}
