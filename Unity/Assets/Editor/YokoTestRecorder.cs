using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
namespace Yoko.Editor {
 [InitializeOnLoad]public class YokoTestRecorder:ICallbacks {
 static TestRunnerApi api;
 static YokoTestRecorder(){api=ScriptableObject.CreateInstance<TestRunnerApi>();api.RegisterCallbacks(new YokoTestRecorder());}
 public void RunStarted(ITestAdaptor testsToRun){Directory.CreateDirectory("../Logs");File.WriteAllText("../Logs/play-tests.txt","Run started\n");}
 public void TestStarted(ITestAdaptor test){}
 public void TestFinished(ITestResultAdaptor result){if(!result.Test.IsSuite)File.AppendAllText("../Logs/play-tests.txt",result.Test.FullName+" : "+result.ResultState+" "+result.Message+"\n");}
 public void RunFinished(ITestResultAdaptor result){TestRunnerApi.SaveResultToFile(result,"../Logs/play-tests.xml");File.AppendAllText("../Logs/play-tests.txt","FINAL: "+result.ResultState+" Passed="+result.PassCount+" Failed="+result.FailCount+"\n");}
 }
}