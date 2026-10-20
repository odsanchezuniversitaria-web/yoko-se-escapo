using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace Yoko.Editor {
public class YokoShaderBuild : IPreprocessBuildWithReport {
public int callbackOrder => -100;
[InitializeOnLoadMethod] static void OnLoad(){ EditorApplication.delayCall += Ensure; }
public void OnPreprocessBuild(BuildReport report){Ensure();}
static void Ensure(){
var assets=AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset");
if(assets.Length==0)return;
var so=new SerializedObject(assets[0]);var list=so.FindProperty("m_AlwaysIncludedShaders");
foreach(var name in new[]{"UI/Default","Sprites/Default","UI/Default Font"}){
var shader=Shader.Find(name);if(shader==null)continue;bool found=false;
for(int i=0;i<list.arraySize;i++)if(list.GetArrayElementAtIndex(i).objectReferenceValue==shader)found=true;
if(!found){int index=list.arraySize;list.InsertArrayElementAtIndex(index);list.GetArrayElementAtIndex(index).objectReferenceValue=shader;}
}
so.ApplyModifiedPropertiesWithoutUndo();AssetDatabase.SaveAssets();
}
}
}