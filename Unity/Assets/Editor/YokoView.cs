using System;using System.Reflection;using UnityEditor;using UnityEngine;
namespace Yoko.Editor {
 public static class YokoView {
 [MenuItem("Yoko/View 1600x900")]public static void Size() {
 var assembly=typeof(UnityEditor.Editor).Assembly;
 var sizesType=assembly.GetType("UnityEditor.GameViewSizes");
 var singleton=typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
 var sizes=singleton.GetProperty("instance",BindingFlags.Public|BindingFlags.Static|BindingFlags.FlattenHierarchy).GetValue(null);
 var group=sizesType.GetMethod("GetGroup").Invoke(sizes,new object[]{0});
 var gt=group.GetType();int index=(int)gt.GetMethod("GetBuiltinCount").Invoke(group,null)+(int)gt.GetMethod("GetCustomCount").Invoke(group,null);
 var sizeType=assembly.GetType("UnityEditor.GameViewSize");
 var enumType=assembly.GetType("UnityEditor.GameViewSizeType");
 var size=Activator.CreateInstance(sizeType,new object[]{Enum.ToObject(enumType,1),1600,900,"Yoko QA"});
 gt.GetMethod("AddCustomSize").Invoke(group,new[]{size});
 var view=EditorWindow.GetWindow(assembly.GetType("UnityEditor.GameView"));view.Show();view.Focus();view.maximized=true;
 view.GetType().GetProperty("selectedSizeIndex",BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance).SetValue(view,index);
 Debug.Log("YOKO_VIEW_READY");
 }
 }
}