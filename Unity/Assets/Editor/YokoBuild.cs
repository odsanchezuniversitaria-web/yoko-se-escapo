using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yoko;
namespace Yoko.Editor
{
    public static class YokoBuild
    {
        [MenuItem("Yoko/Prepare Project")]
        public static void Prepare()
        {
            PlayerSettings.companyName="Yoko";
            PlayerSettings.productName="Yoko se escapo";
            PlayerSettings.bundleVersion="0.1.0";
            PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android,"com.yoko.seescapo");
            PlayerSettings.defaultScreenWidth=1600;PlayerSettings.defaultScreenHeight=900;
            PlayerSettings.fullScreenMode=FullScreenMode.Windowed;
            PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToPortrait=false;PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;
            PlayerSettings.allowedAutorotateToLandscapeLeft=true;PlayerSettings.allowedAutorotateToLandscapeRight=true;
            PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);
            PlayerSettings.runInBackground=true;
            var settings=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            settings.FindProperty("activeInputHandler").intValue=0;settings.ApplyModifiedPropertiesWithoutUndo();
            var tags=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            tags.FindProperty("layers").GetArrayElementAtIndex(8).stringValue="Ground";tags.ApplyModifiedPropertiesWithoutUndo();
            foreach(string name in new[]{"Yoko","Cast","Bogota","Props"})
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath("Assets/Resources/Art/"+name+".png");
                importer.textureType=TextureImporterType.Default;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;
                importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=2048;importer.textureCompression=TextureImporterCompression.Uncompressed;
                importer.filterMode=FilterMode.Bilinear;importer.SaveAndReimport();
            }
            Directory.CreateDirectory("Assets/Scenes");Directory.CreateDirectory("Assets/Prefabs");
            Scene scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var root=new GameObject("Yoko Game");root.AddComponent<YokoGame>();
            PrefabUtility.SaveAsPrefabAsset(root,"Assets/Prefabs/YokoGame.prefab");
            EditorSceneManager.SaveScene(scene,"Assets/Scenes/Yoko.unity");
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/Yoko.unity",true)};
            AssetDatabase.SaveAssets();
            Debug.Log("YOKO_PREPARED "+Application.unityVersion);
        }
        [MenuItem("Yoko/MCP/Start")]public static void Start()=>YokoGame.Instance.GetComponent<DebugManager>().Execute("start");
        [MenuItem("Yoko/MCP/Pause")]public static void Pause()=>YokoGame.Instance.GetComponent<DebugManager>().Execute("pause");
        [MenuItem("Yoko/MCP/Settings")]public static void Settings()=>YokoGame.Instance.GetComponent<DebugManager>().Execute("settings");
        [MenuItem("Yoko/MCP/Resume")]public static void Resume()=>YokoGame.Instance.GetComponent<DebugManager>().Execute("resume");
        [MenuItem("Yoko/MCP/Die")]public static void Die()=>YokoGame.Instance.GetComponent<DebugManager>().Execute("die");
        [MenuItem("Yoko/MCP/Retry")]public static void Retry()=>YokoGame.Instance.GetComponent<DebugManager>().Execute("retry");
        [MenuItem("Yoko/MCP/Title")]public static void Title()=>YokoGame.Instance.GetComponent<DebugManager>().Execute("title");
        [MenuItem("Yoko/MCP/City")]public static void City(){var g=YokoGame.Instance;g.TestMode=true;g.Session.Data.serum=true;g.GoZone(1);g.GodMode=true;}
        [MenuItem("Yoko/MCP/Good Ending")]public static void Good()=>Ending(false);
        [MenuItem("Yoko/MCP/Tragic Ending")]public static void Bad()=>Ending(true);
        static void Ending(bool tragic)
        {
            var g=YokoGame.Instance;g.TestMode=true;g.Session.Data.serum=true;g.Session.Data.foundYimmy=true;g.Session.Data.violence=tragic?100:0;g.GoZone(4);
            g.Session.Assimilate(Mutation.Claw,false);
            foreach(var a in g.World.Actors)if(a.Kind==ActorKind.Node)a.Hit();
            g.Finish();
        }
        [MenuItem("Yoko/Build Windows")]public static void Windows()=>Build(BuildTarget.StandaloneWindows64,"../Builds/Windows/Yoko.exe");
        [MenuItem("Yoko/Build Android")]public static void Android()=>Build(BuildTarget.Android,"../Builds/Android/Yoko.apk");
        static void Build(BuildTarget target,string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var r=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/Yoko.unity"},locationPathName=path,target=target,options=BuildOptions.None});
            File.WriteAllText("../Logs/build-"+target+".txt",r.summary.result+"\nErrors: "+r.summary.totalErrors+"\nSize: "+r.summary.totalSize);
            if(r.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new Exception("Build failed "+r.summary.result);
        }
    }
}