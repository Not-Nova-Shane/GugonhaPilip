using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;


namespace GugoPilip;

[BepInPlugin(GUID, NAME, VERSION)]
public class Main : BasePlugin
{
    public const string GUID = $"{AUTHOR}.{NAME}";
    public const string NAME = "GugoPilip";
    public const string VERSION = "1.0.0";
    public const string AUTHOR = "Gugonha";
    public static ManualLogSource log; // logging object to access later so that we have logs that save to LogOutput.log

    public override void Load()
    {
        Harmony harmony = new Harmony(NAME); // all of our passives and effects will be patched through this object
        log = new ManualLogSource(NAME);
        BepInEx.Logging.Logger.Sources.Add(log);

        harmony.PatchAll(typeof(Passive_CooledHeart)); // patch specifically a class related to our shi
        harmony.PatchAll(typeof(Buff_Overheat));
        harmony.PatchAll(typeof(Buff_RekindledPassion));
        harmony.PatchAll(typeof(Passive_Sunset));
        harmony.PatchAll(typeof(Passive_CelloEnsemble));
        harmony.PatchAll(typeof(Buff_VanishingMind));
        harmony.PatchAll(typeof(Buff_PastEmbers));


    }
}
