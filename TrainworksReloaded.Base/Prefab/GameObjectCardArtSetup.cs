using SimpleInjector;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace TrainworksReloaded.Base.Prefab
{
    public class GameObjectCardArtSetup : IDataPipelineSetup<GameObject>
    {
        private readonly IModLogger<GameObjectCardArtSetup> logger;
        private readonly Container container;

        public GameObjectCardArtSetup(
            IModLogger<GameObjectCardArtSetup> logger,
            Container container
        )
        {
            this.logger = logger;
            this.container = container;
        }

        public void Setup(IDefinition<GameObject> definition)
        {
            var type = definition.Configuration.GetSection("type").Value;
            var spriteVal = definition.Configuration.GetSection("sprite").Value;
            if (type != "card_art" || spriteVal == null)
            {
                return;
            }

            var id = spriteVal.ToId(definition.Key, "Sprite");
            var sprite_register = container.GetInstance<IRegister<Sprite>>();
            if (!sprite_register.TryLookupId(id, out var sprite, out _))
            {
                return;
            }

            var gameObject = definition.Data;
            gameObject.AddComponent<AddressableAssetPrefab>();
            gameObject.AddComponent<RectTransform>();

            var cardArt = new GameObject { name = "CardSprite" };
            cardArt.transform.SetParent(gameObject.transform);
            var canvasRenderer = cardArt.AddComponent<CanvasRenderer>();

            var image = cardArt.AddComponent<Image>();
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

            var material = new Material(Shader.Find("Shiny Shoe/CardEffects"))
            {
                mainTexture = sprite.texture,
            };
            material.SetTexture("_Layer1Tex", sprite.texture);
            image.material = material;
            canvasRenderer.materialCount = 1;
            canvasRenderer.SetMaterial(material, 0);

            var cardEffectTransforms = new GameObject { name = "CardEffectTransforms" };
            cardEffectTransforms.transform.SetParent(gameObject.transform);
        }
    }
}
