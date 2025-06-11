using UnityEngine;
using UnityEditor;
using System.IO;

public class iOSBuildScript
{
    [MenuItem("Build/Build for iOS")]
    public static void BuildiOS()
    {
        Debug.Log("🎮 Building Empire Rush for iOS...");
        
        // Configure project settings
        PlayerSettings.companyName = "EmpireRush Studios";
        PlayerSettings.productName = "Empire Rush";
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.iOS.buildNumber = "1";
        PlayerSettings.iOS.targetOSVersionString = "11.0";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.empirerushstudios.empirerush");
        
        // Switch to iOS platform
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
        
        // Create scene if none exists
        string scenePath = CreateGameScene();
        
        // Setup build
        string buildPath = "Builds/iOS";
        Directory.CreateDirectory(buildPath);
        
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { scenePath },
            locationPathName = buildPath,
            target = BuildTarget.iOS,
            options = BuildOptions.None
        };
        
        // Execute build
        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        
        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("✅ iOS Build Successful!");
            Debug.Log($"📱 Xcode project created at: {Path.GetFullPath(buildPath)}");
            Debug.Log("🚀 Next step: Open Unity-iPhone.xcodeproj in Xcode");
        }
        else
        {
            Debug.LogError($"❌ Build failed: {report.summary.result}");
        }
    }
    
    private static string CreateGameScene()
    {
        // Create new scene
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
            UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
            UnityEditor.SceneManagement.NewSceneMode.Single
        );
        
        // Add camera
        GameObject camera = new GameObject("Main Camera");
        camera.AddComponent<Camera>();
        camera.transform.position = new Vector3(0, 1, -10);
        camera.tag = "MainCamera";
        
        // Add light
        GameObject light = new GameObject("Directional Light");
        Light lightComponent = light.AddComponent<Light>();
        lightComponent.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(50, -30, 0);
        
        // Add game controller
        GameObject gameController = new GameObject("EmpireRushGameController");
        gameController.AddComponent<EmpireRushGame>();
        
        // Save scene
        string scenePath = "Assets/Scenes/EmpireRushMain.unity";
        Directory.CreateDirectory("Assets/Scenes");
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, scenePath);
        
        // Add to build settings
        EditorBuildSettings.scenes = new[] {
            new EditorBuildSettingsScene(scenePath, true)
        };
        
        return scenePath;
    }
    
    // For command line builds
    public static void BuildFromCommandLine()
    {
        BuildiOS();
    }
}