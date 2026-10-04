#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public sealed class DewyBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => -1000;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.WebGL)
            ApplyWebGLSettings();
    }

    [MenuItem("Dewy/Apply WebGL Settings")]
    public static void ApplyWebGLSettings()
    {
        PlayerSettings.defaultWebScreenWidth = 450;
        PlayerSettings.defaultWebScreenHeight = 900;
        PlayerSettings.WebGL.template = "PROJECT:Dewy";
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.decompressionFallback = true;
        EditorUserBuildSettings.SetBuildLocation(BuildTarget.WebGL, "Builds/WebGL");
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/Main.unity", true)
        };
    }

    [MenuItem("Dewy/Build WebGL for itch.io")]
    public static void BuildWebGL()
    {
        ApplyWebGLSettings();
        const string output = "Builds/WebGL";
        if (Directory.Exists(output)) Directory.Delete(output, true);
        Directory.CreateDirectory(output);

        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Main.unity" },
            locationPathName = output,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        });

        if (report.summary.result != BuildResult.Succeeded)
            throw new BuildFailedException($"Dewy WebGL build failed: {report.summary.result}");

        Debug.Log($"Dewy WebGL build complete: {Path.GetFullPath(output)}");
    }
}
#endif
