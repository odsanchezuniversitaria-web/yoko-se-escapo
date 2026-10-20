using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
namespace Yoko.Editor {
public class YokoWebBuild : IPreprocessBuildWithReport {
public int callbackOrder => -90;
public void OnPreprocessBuild(BuildReport report){
if(report.summary.platform != BuildTarget.WebGL)return;
PlayerSettings.WebGL.compressionFormat=WebGLCompressionFormat.Disabled;
PlayerSettings.WebGL.decompressionFallback=false;
PlayerSettings.WebGL.template="APPLICATION:Default";
}
}
}