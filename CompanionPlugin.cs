using MelonLoader;
using MelonLoader.Utils;

namespace GregCompanion;

/// <summary>
/// gregPlugin.Companion — fusioniert SteamModfix (Workshop-Staging + externe
/// Quellen, läuft vor dem Mod-Scan) mit der SubDirectoryFixer-Rolle
/// (Verzeichnis-Policy-Report). Arbeitet gregCore zu: stellt sicher, dass
/// Mods aus ./Mods, Plugins aus ./Plugins und Libraries aus ./UserLibs
/// kommen, bevor irgendwas lädt.
/// </summary>
public sealed class CompanionPlugin : MelonPlugin
{
    public override void OnEarlyInitializeMelon()
    {
        try
        {
            EnsureLayout();
        }
        catch (Exception ex)
        {
            MelonLogger.Warning("[Companion] Layout-Setup fehlgeschlagen: " + ex.Message);
        }
        SubdirectoryGuard.Scan();
        WorkshopModLoader.EarlyInit();
    }

    public override void OnPreModsLoaded()
    {
        WorkshopModLoader.PreModsLoaded();
    }

    private static void EnsureLayout()
    {
        string root = MelonEnvironment.GameRootDirectory;
        if (string.IsNullOrWhiteSpace(root)) return;
        foreach (string sub in new[] { "Mods", "Plugins", "UserLibs", "UserData", Path.Combine("Mods", "gregNative") })
        {
            try
            {
                string dir = Path.Combine(root, sub);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            }
            catch { }
        }
        MelonLogger.Msg("[Companion] Verzeichnis-Layout ok (Mods/Plugins/UserLibs/UserData, Mods/gregNative).");
    }
}
