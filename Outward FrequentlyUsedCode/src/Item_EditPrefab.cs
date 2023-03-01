using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OutwardModTemplate
{
	//You should for most projects give your plugin class type a name other than the default, it will help debugging later.
	//If you are using Visual Studio you can select the class name declartion below and press ctrl + r then r again to rename it, you will see a rename box in the top right be default, this will rename it anywhere the name is used in your project
	[BepInPlugin(GUID, NAME, VERSION)]
	public class ModExamplePlugin : BaseUnityPlugin
	{
		public const string GUID = "YOURUNIQUEID.YOURMODNAME";
		public const string NAME = "A MOD NAME";
		public const string VERSION = "1.0.0";

		internal static ManualLogSource Log;
		//There will only ever be one instance of your mod running
		//that means we can use a static variable - static variables are variables that exist on the class type rather than an instance of the class
		//That means typing ModExamplePlugin.Instance anywhere will give us access to this plugin right here, this is the central point for your mod in most cases.
		//but you must remember to set Instance = this in awake otherwise it will be null.
		public static ModExamplePlugin Instance;

		internal void Awake()
		{
			Instance = this;
			Log = this.Logger;


			///Subscribe to SideLoaders.OnPacksLoaded method so we know any custom items and their prefabs have been loaded
            SL.OnPacksLoaded += SL_OnPacksLoaded;


			new Harmony(GUID).PatchAll();
        }


		//This method will be called once SideLoader has finished loading all its item Definitions and finished setting up the prefabs for them.
		//the name can be whatever you want it to be
        private void SL_OnPacksLoaded()
        {
            //Retrieve the Item *Prefab* from the ResourcesPrefabManager for the ItemID in this case an Iron Sword
            Item TheItem = ResourcesPrefabManager.Instance.GetItemPrefab(2000010);

			//If you want to add something that will affect what the player will visually see then you want to be making your changes to the items ItemVisual GameObject and not the Prefab itself
			//we pass in .HasSpecialVisualPrefab as a parameter of GetItemVisual incase the Item uses a special visual for any reason, mostly I have found this to be Chakrams.
			Transform ItemVisualTransform = TheItem.GetItemVisual(TheItem.HasSpecialVisualPrefab);

            if (ItemVisualTransform)
            {
                Log.LogMessage($"Item Visual Found for {TheItem.Name}");

				//create a new GameObject with the name "LightGameObject"
				GameObject NewLightGameObject = new GameObject("LightGameObject");
				//parent it to the Items Visual so it moves with it
				NewLightGameObject.transform.parent = ItemVisualTransform;
				//Add the component to our new GameObject
				Light NewLightComponent = NewLightGameObject.AddComponent<Light>();

				//Now we are getting the transform property from the Light component, every component exists on a GameObject and every GameObject has *at least* a Transform component, where you find one you can always find the other using .gameObject or .transform
				//we are setting the localPosition of the NewGameObject, you would use .position if you are working in World Space (where everything has an absolute position opposed to a position relative to it's parents position such as with localPosition
				//This NewGameObject will be set to (0,0,0) inside its parent
				NewLightComponent.transform.localPosition = Vector3.zero;
				//mess with the Light Component properties
				NewLightComponent.intensity = 10f;
				NewLightComponent.color = Color.red;
				NewLightComponent.range = 25f;
            }
        }
    }
}
