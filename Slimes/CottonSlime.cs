﻿using SRML.SR;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CottonSlime
{
    class CottonSlime
    {
        public static Texture2D LoadImage(string filename)
        {
            var a = Assembly.GetExecutingAssembly();
            var spriteData = a.GetManifestResourceStream(a.GetName().Name + "." + filename);
            var rawData = new byte[spriteData.Length];
            spriteData.Read(rawData, 0, rawData.Length);
            var tex = new Texture2D(1, 1);
            tex.LoadImage(rawData);
            tex.filterMode = FilterMode.Bilinear;
            return tex;
        }
        public static Sprite CreateSprite(Texture2D texture) => Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);
        public static (SlimeDefinition, GameObject) CreateSlime(Identifiable.Id SlimeId, String SlimeName)
        {
            // DEFINE
            SlimeDefinition hunterSlimeDefinition = SRSingleton<GameContext>.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.HUNTER_SLIME);
            SlimeDefinition slimeDefinition = (SlimeDefinition)PrefabUtils.DeepCopyObject(hunterSlimeDefinition);
            slimeDefinition.AppearancesDefault = new SlimeAppearance[1];
            slimeDefinition.Diet.Produces = new Identifiable.Id[1]
            {
                ModdedIds.cottonIds.COTTON_PLORT
            };
            slimeDefinition.Diet.MajorFoodGroups = new SlimeEat.FoodGroup[1]
            {
                SlimeEat.FoodGroup.VEGGIES
            };
            slimeDefinition.Diet.AdditionalFoods = new Identifiable.Id[0];
            slimeDefinition.Diet.Favorites = new Identifiable.Id[0];
            slimeDefinition.Diet.EatMap?.Clear();
            slimeDefinition.CanLargofy = false;
            slimeDefinition.FavoriteToys = new Identifiable.Id[1];
            slimeDefinition.Name = "Cotton Slime";
            slimeDefinition.IdentifiableId = ModdedIds.cottonIds.COTTON_SLIME;
            // OBJECT
            GameObject slimeObject = PrefabUtils.CopyPrefab(SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(Identifiable.Id.PINK_SLIME));
            slimeObject.name = "slimeCotton";
            slimeObject.GetComponent<PlayWithToys>().slimeDefinition = slimeDefinition;
            slimeObject.GetComponent<SlimeAppearanceApplicator>().SlimeDefinition = slimeDefinition;
            slimeObject.GetComponent<SlimeEat>().slimeDefinition = slimeDefinition;
            slimeObject.GetComponent<Identifiable>().id = ModdedIds.cottonIds.COTTON_SLIME;
            slimeObject.AddComponent<BetterSlimeHover>();
            // UnityEngine.Object.Destroy(slimeObject.GetComponent<SlimeEatWater>());
            UnityEngine.Object.Destroy(slimeObject.GetComponent<PinkSlimeFoodTypeTracker>());
            // APPEARANCE
            SlimeAppearance slimeAppearance = (SlimeAppearance)PrefabUtils.DeepCopyObject(hunterSlimeDefinition.AppearancesDefault[0]);
            slimeDefinition.AppearancesDefault[0] = slimeAppearance;
            SlimeAppearanceStructure[] structures = slimeAppearance.Structures;
            foreach (SlimeAppearanceStructure slimeAppearanceStructure in structures)
            {
                Material[] defaultMaterials = slimeAppearanceStructure.DefaultMaterials;
                if (defaultMaterials != null && defaultMaterials.Length != 0)
                {
                    Material material = UnityEngine.Object.Instantiate(SRSingleton<GameContext>.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.HUNTER_SLIME).AppearancesDefault[0].Structures[0].DefaultMaterials[0]);
                    material.SetColor("_TopColor", new Color32(255, 251, 246, 255));
                    material.SetColor("_MiddleColor", new Color32(255, 251, 246, 255));
                    material.SetColor("_BottomColor", new Color32(230, 226, 222, 255));
                    material.SetColor("_SpecColor", new Color32(255, 251, 246, 255));
                    material.SetFloat("_Shininess", 1f);
                    material.SetFloat("_Gloss", 1f);
                    slimeAppearanceStructure.DefaultMaterials[0] = material;
                }
            }
            SlimeExpressionFace[] expressionFaces = slimeAppearance.Face.ExpressionFaces;
            for (int k = 0; k < expressionFaces.Length; k++)
            {
                SlimeExpressionFace slimeExpressionFace = expressionFaces[k];
                if ((bool)slimeExpressionFace.Mouth)
                {
                    slimeExpressionFace.Mouth.SetColor("_MouthBot", Color.white);
                    slimeExpressionFace.Mouth.SetColor("_MouthMid", Color.white);
                    slimeExpressionFace.Mouth.SetColor("_MouthTop", Color.white);
                }
                if ((bool)slimeExpressionFace.Eyes)
                {   /*
                    slimeExpressionFace.Eyes.SetColor("_EyeRed", new Color32(205, 190, 255, 255));
                    slimeExpressionFace.Eyes.SetColor("_EyeGreen", new Color32(182, 170, 226, 255));
                    slimeExpressionFace.Eyes.SetColor("_EyeBlue", new Color32(182, 170, 205, 255));
                    */
                    slimeExpressionFace.Eyes.SetColor("_EyeRed", new Color32(255, 251, 246, 255));
                    slimeExpressionFace.Eyes.SetColor("_EyeGreen", new Color32(255, 251, 246, 255));
                    slimeExpressionFace.Eyes.SetColor("_EyeBlue", new Color32(255, 251, 246, 255));
                }
            }
            slimeAppearance.Icon = CreateSprite(LoadImage("Assets.cotton_slime.png"));
            slimeAppearance.Face.OnEnable();
            slimeAppearance.ColorPalette = new SlimeAppearance.Palette
            {
                Top = new Color32(230, 226, 222, 255),
                Middle = new Color32(230, 226, 222, 255),
                Bottom = new Color32(230, 226, 222, 255)
            };
            PediaRegistry.RegisterIdEntry(ModdedIds.cottonIds.COTTON_ENTRY, CreateSprite(LoadImage("Assets.cotton_slime.png")));
            slimeObject.GetComponent<SlimeAppearanceApplicator>().Appearance = slimeAppearance;
            return (slimeDefinition, slimeObject);
        }
    }
}
