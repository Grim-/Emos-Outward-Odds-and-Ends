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
			new Harmony(GUID).PatchAll();
		}
	}