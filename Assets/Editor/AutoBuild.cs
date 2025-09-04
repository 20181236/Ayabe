using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AutoBuild : MonoBehaviour 
{
    static string[] SCENES = FindEnableEditorScenes();
    static string TARGET_DIR = "Build";
    static string APP_ANME = "name";

    static string[] FindEnableEditorScenes()
    {
        List<string> editorscenes = new List<string>();

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if(!scene.enabled) continue;

            editorscenes.Add(scene.path);
        }

        return editorscenes.ToArray();
    }


    //static void CodeUp()
    //{

    //    Debug.Log(" ============= CODE =============" + code);
    //}
    [MenuItem("Custom/Build/Android", false, 1)]
    static void AndroidBuild()
    {
        string buildpath = TARGET_DIR + "/Android/";

        Directory.CreateDirectory(buildpath);

        PlayerSettings.companyName = "";
        PlayerSettings.productName = "";

        PlayerSettings.Android.keystoreName = Application.dataPath + "/user.keystore";
        PlayerSettings.Android.keystoreName = "";
        PlayerSettings.Android.keyaliasPass = "";
        PlayerSettings.Android.keyaliasName = "";

        PlayerSettings.bundleVersion = Application.version;

        string filre = APP_ANME + ".apk";

        GenericBuile(SCENES, buildpath + filre, BuildTarget.Android, BuildOptions.None);

    }


    static void GenericBuile(string[] scenes, string filename, BuildTarget buildTarget, BuildOptions buildOptions)
    {
        BuildPipeline.BuildPlayer(scenes, filename, buildTarget, buildOptions);
    }
}
