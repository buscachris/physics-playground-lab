using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using PhysicsPlaygroundLab;

public static class ProjectBuilder
{
    public static void BuildStandalone()
    {
        string outputDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Builds", "Linux"));
        Directory.CreateDirectory(outputDirectory);
        string outputPath = Path.Combine(outputDirectory, "PhysicsPlaygroundLab.x86_64");
        BuildReport report = BuildPipeline.BuildPlayer(
            new[] { "Assets/Scenes/Main.unity" },
            outputPath,
            BuildTarget.StandaloneLinux64,
            BuildOptions.None);
        Debug.Log($"PHYSICS_PLAYGROUND_BUILD_RESULT:{report.summary.result}:{report.summary.totalSize}");
    }

    public static void OpenMainScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
        Debug.Log("PHYSICS_PLAYGROUND_MAIN_SCENE_OPENED");
    }

    public static void OpenMainSceneAndPlay()
    {
        OpenMainScene();
        EditorApplication.isPlaying = true;
    }

    public static void BuildProject()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Scenes")) AssetDatabase.CreateFolder("Assets", "Scenes");
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject app = new GameObject("PhysicsPlaygroundApp");
        app.AddComponent<PhysicsPlaygroundApp>();
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Main.unity");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("PHYSICS_PLAYGROUND_SCENE_CREATED");
    }
}
