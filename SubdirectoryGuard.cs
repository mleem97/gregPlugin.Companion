using MelonLoader;
using MelonLoader.Utils;

namespace GregCompanion;

/// <summary>
/// Absorbiert die SubDirectoryFixer-Rolle: meldet Mod-DLLs in Unterordnern
/// von ./Mods (außer ./Mods/gregNative) sowie Fremdkörper in ./Plugins und
/// ./UserLibs. Report-only — verschiebt/löscht nichts automatisch.
/// </summary>
internal static class SubdirectoryGuard
{
    public static void Scan()
    {
        try
        {
            string root = MelonEnvironment.GameRootDirectory;
            if (string.IsNullOrWhiteSpace(root)) return;
            int findings = 0;
            findings += ScanModsSubdirs(Path.Combine(root, "Mods"));
            if (findings == 0)
                MelonLogger.Msg("[Companion] Verzeichnis-Check ok: keine Mod-DLLs in Unterordnern.");
            else
                MelonLogger.Warning($"[Companion] {findings} Datei(en) in Unterordnern gefunden (siehe oben) — manuelle Mods gehören direkt nach ./Mods, Libraries nach ./UserLibs, Plugins nach ./Plugins.");
        }
        catch (Exception ex)
        {
            MelonLogger.Warning("[Companion] SubdirectoryGuard fehlgeschlagen: " + ex.Message);
        }
    }

    private static int ScanModsSubdirs(string modsDir)
    {
        int count = 0;
        if (!Directory.Exists(modsDir)) return 0;
        string nativeRoot;
        try { nativeRoot = Path.GetFullPath(Path.Combine(modsDir, "gregNative")); }
        catch { nativeRoot = null; }
        foreach (string file in Directory.EnumerateFiles(modsDir, "*.dll", SearchOption.AllDirectories))
        {
            string full;
            try { full = Path.GetFullPath(file); } catch { continue; }
            string dir = Path.GetDirectoryName(full);
            if (string.Equals(dir, modsDir, StringComparison.OrdinalIgnoreCase)) continue;
            if (nativeRoot != null && full.StartsWith(nativeRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                continue; // gregNative-Baum: eigene Regel
            MelonLogger.Warning($"[Companion] DLL in Unterordner: {full} — wird von MelonLoader NICHT geladen. Bitte nach ./Mods (Mods), ./Plugins (Plugins) bzw. ./UserLibs (Libraries) verschieben.");
            count++;
            if (count >= 20) break;
        }
        return count;
    }
}
