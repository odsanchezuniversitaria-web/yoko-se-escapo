using System.Collections;using NUnit.Framework;using UnityEngine;using UnityEngine.SceneManagement;using UnityEngine.TestTools;
namespace Yoko.Tests {
 public class TraversalTests {
 YokoGame g;
 IEnumerator Move(float x,float limit=10){float end=Time.time+limit;g.Player.AutoMove=1;while(g.Player.transform.position.x<x&&Time.time<end)yield return null;g.Player.AutoMove=0;Assert.GreaterOrEqual(g.Player.transform.position.x,x-.2f,"Traversal blocked before "+x);}
 IEnumerator Land(){float end=Time.time+4;yield return new WaitForSeconds(.15f);while(!g.Player.Grounded&&Time.time<end)yield return null;Assert.IsTrue(g.Player.Grounded);}
 [UnityTest]public IEnumerator IT05_PhysicalTraversalOfTutorial(){
 SceneManager.LoadScene("Yoko");yield return null;yield return null;g=YokoGame.Instance;g.TestMode=true;g.BeginCampaign();g.Player.AutoInput=true;yield return new WaitForSeconds(.2f);
 yield return Move(4.5f);g.Interact();Assert.IsTrue(g.Session.Data.serum);
 yield return Move(7.2f);g.Interact();Assert.AreEqual(Mutation.Wing,g.Session.Power);
 g.Player.AutoGlide=true;g.Player.AutoJump=true;yield return Move(11);yield return Land();
 Assert.Greater(g.Player.transform.position.y,1);
 yield return Move(13);g.Player.AutoJump=true;yield return Move(18);yield return Land();
 Assert.Greater(g.Player.transform.position.y,2);
 yield return Move(20.5f);g.Player.AutoJump=true;yield return Move(25.5f);yield return Land();
 yield return Move(54);g.Interact();Assert.AreEqual(1,g.Session.Data.zone);
 g.Player.AutoInput=false;
 }
 }
}