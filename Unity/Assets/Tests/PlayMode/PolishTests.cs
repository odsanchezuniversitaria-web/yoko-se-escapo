using System.Collections;using NUnit.Framework;using UnityEngine;using UnityEngine.SceneManagement;using UnityEngine.TestTools;using UnityEngine.EventSystems;
namespace Yoko.Tests {
public class PolishTests {
 YokoGame g;
 [UnitySetUp]public IEnumerator Setup(){SceneManager.LoadScene("Yoko");yield return null;yield return null;g=YokoGame.Instance;g.TestMode=true;g.BeginCampaign();g.Player.AutoInput=true;yield return new WaitForSeconds(.2f);}
 [UnityTearDown]public IEnumerator End(){PlayerPrefs.SetInt("yoko-touch",0);Time.timeScale=1;yield return null;}
 [UnityTest]public IEnumerator PT01_DodgeHasCooldownAndAvoidsDamage(){
  Assert.IsTrue(g.Player.TryDodge());g.Hurt();Assert.AreEqual(3,g.Session.Health);Assert.IsFalse(g.Player.TryDodge());yield return new WaitForSeconds(.25f);g.Hurt();Assert.AreEqual(2,g.Session.Health);yield return new WaitForSeconds(1);Assert.IsTrue(g.Player.TryDodge());
 }
 [UnityTest]public IEnumerator PT02_TouchOwnershipAndPauseRelease(){
  PlayerPrefs.SetInt("yoko-touch",1);g.UI.Build();yield return null;Canvas.ForceUpdateCanvases();
  var stick=Object.FindFirstObjectByType<AnalogStick>();Assert.IsNotNull(stick);
  var center=RectTransformUtility.WorldToScreenPoint(g.Cam,stick.transform.position);
  var one=new PointerEventData(EventSystem.current){pointerId=11,position=center};
  var two=new PointerEventData(EventSystem.current){pointerId=22,position=center};
  one.pointerPressRaycast=new RaycastResult{module=Object.FindFirstObjectByType<UnityEngine.UI.GraphicRaycaster>()};
  stick.OnPointerDown(one);Assert.That(stick.Value,Is.EqualTo(0).Within(.03f));one.position=RectTransformUtility.WorldToScreenPoint(g.Cam,stick.transform.TransformPoint(Vector3.right*130));stick.OnDrag(one);Assert.Greater(stick.Value,.95f);Assert.Greater(g.Player.TouchMove,.95f);
  one.position=center;stick.OnDrag(one);Assert.That(stick.Value,Is.EqualTo(0).Within(.03f));stick.OnPointerDown(two);Assert.AreEqual(11,stick.Owner);stick.OnPointerUp(two);Assert.AreEqual(11,stick.Owner);
  var pads=Object.FindObjectsByType<ActionPad>(FindObjectsSortMode.None);ActionPad jump=null;foreach(var p in pads)if(p.name=="SALTO")jump=p;Assert.IsNotNull(jump);
  jump.OnPointerDown(two);Assert.IsTrue(g.Player.TouchJump);jump.OnPointerUp(one);Assert.IsTrue(g.Player.TouchJump);
  g.ChangeState(ScreenState.Pause);Assert.AreEqual(0,g.Player.TouchMove);Assert.IsFalse(g.Player.TouchJump);Assert.AreEqual(int.MinValue,stick.Owner);Assert.AreEqual(int.MinValue,jump.Owner);
 }
 [UnityTest]public IEnumerator PT03_ObjectivesFollowProgressAndPausedAttackDoesNothing(){
  StringAssert.Contains("SUERO",g.Story.Objective);g.World.Actors.Find(a=>a.Kind==ActorKind.Serum).Interact();StringAssert.Contains("AVE",g.Story.Objective);
  g.Session.Assimilate(Mutation.Wing,false);StringAssert.Contains("RUINAS",g.Story.Objective);
  g.Session.Data.serum=true;g.GoZone(1);var enemy=g.World.Actors.Find(a=>a.IsEnemy);g.Player.Warp(enemy.transform.position-Vector3.right);
  g.ChangeState(ScreenState.Pause);g.Attack();Assert.AreEqual(0,g.Session.Data.violence);g.ChangeState(ScreenState.Playing);g.Attack();Assert.Greater(g.Session.Data.violence,0);yield return null;
 }
}
}