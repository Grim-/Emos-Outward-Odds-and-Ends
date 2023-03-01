	[BepInPlugin(GUID, NAME, VERSION)]
	public class EmosModClass : BaseUnityPlugin
	{
		public const string GUID = "YOURUNIQUEID.YOURMODNAME";
		public const string NAME = "A MOD NAME";
		public const string VERSION = "1.0.0";

		internal static ManualLogSource Log;
		//There will only ever be one instance of your mod running
		//that means we can use a static variable - static variables are variables that exist on the class type rather than an instance of the class
		//That means typing EmosModClass.Instance anywhere will give us access to this plugin right here, this is the central point for your mod in most cases.
		//but you must remember to set Instance = this in awake otherwise it will be null.
		public static EmosModClass Instance;

		internal void Awake()
		{
			Instance = this;
			Log = this.Logger;


			///Subscribe to SideLoaders.OnPacksLoaded method so we know any custom items and their prefabs have been loaded
            SL.OnPacksLoaded += SL_OnPacksLoaded;


			new Harmony(GUID).PatchAll();
        }

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

				//Now we are getting the transform property from the Light component, every component exists on a GameObject and every GameObject has *atleast* a Transform component
				//we are setting the localPosition of the NewGameObject, you would use .position if you are working in World Space (where everything has an absolute position opposed to a position relative to it's parents position such as with localPosition
				//This NewGameObject will be set to (0,0,0) inside its parent
				NewLightComponent.transform.localPosition = Vector3.zero;
				//mess with the Light Component properties
				NewLightComponent.intensity = 10f;
				NewLightComponent.color = Color.red;
            }
        }
    }