using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
namespace Yoko.Tests
{
    public class CampaignTests
    {
        YokoGame g;
        [UnitySetUp]public IEnumerator Setup()
        {
            SceneManager.LoadScene("Yoko");yield return null;yield return null;
            g=YokoGame.Instance;g.TestMode=true;g.BeginCampaign();g.Player.AutoInput=true;
            yield return new WaitForSeconds(.2f);
        }
        [UnityTearDown]public IEnumerator Teardown(){Time.timeScale=1;yield return null;}
        WorldActor Actor(ActorKind kind){return g.World.Actors.Find(a=>a.Kind==kind&&!a.Used);}
        [UnityTest]public IEnumerator IT01_MovementPauseDamageAndRetry()
        {
            float x=g.Player.transform.position.x;g.Player.AutoMove=1;yield return new WaitForSeconds(.5f);Assert.Greater(g.Player.transform.position.x,x+1);
            g.Player.AutoMove=0;g.ChangeState(ScreenState.Pause);Assert.AreEqual(0,Time.timeScale);g.ChangeState(ScreenState.Playing);
            g.Hurt();Assert.AreEqual(2,g.Session.Health);g.Player.HurtUntil=0;g.Hurt();g.Player.HurtUntil=0;g.Hurt();Assert.AreEqual(ScreenState.Dead,g.State);
            g.Retry();Assert.AreEqual(3,g.Session.Health);Assert.AreEqual(ScreenState.Playing,g.State);yield return null;
        }
        [UnityTest]public IEnumerator IT02_PeacefulCampaignAndRescue()
        {
            g.GodMode=true;
            Actor(ActorKind.Serum).Interact();Actor(ActorKind.Remains).Interact();Assert.AreEqual(Mutation.Wing,g.Session.Power);
            Actor(ActorKind.Exit).Interact();Assert.AreEqual(1,g.Session.Data.zone);
            Actor(ActorKind.Exit).Interact();Assert.AreEqual(2,g.Session.Data.zone);
            Actor(ActorKind.Exit).Interact();Assert.AreEqual(2,g.Session.Data.zone,"Encuentro obligatorio");
            Actor(ActorKind.Yimmy).Interact();Actor(ActorKind.Exit).Interact();Assert.AreEqual(3,g.Session.Data.zone);
            g.Session.Assimilate(Mutation.Claw,false);
            foreach(var a in g.World.Actors)if(a.Kind==ActorKind.Node)a.Hit();
            Actor(ActorKind.Exit).Interact();Assert.AreEqual(4,g.Session.Data.zone);
            Actor(ActorKind.Yimmy).Interact();Assert.AreEqual(ScreenState.Playing,g.State,"No rescue before nodes");
            g.Session.Assimilate(Mutation.Claw,false);
            foreach(var a in g.World.Actors)if(a.Kind==ActorKind.Node)a.Hit();
            Actor(ActorKind.Yimmy).Interact();Assert.AreEqual(ScreenState.Ending,g.State);Assert.IsFalse(g.Session.Tragic);Assert.AreEqual(0,g.Session.Data.violence);
            yield return null;
        }
        [UnityTest]public IEnumerator IT03_ViolentChoicesPersistAndLeadToTragicEnding()
        {
            g.GodMode=true;g.Session.Data.serum=true;g.GoZone(1);
            Actor(ActorKind.Prey).Interact();g.GoZone(2);Actor(ActorKind.Prey).Interact();Actor(ActorKind.Yimmy).Interact();g.GoZone(3);Actor(ActorKind.Prey).Interact();
            Assert.AreEqual(75,g.Session.Data.violence);g.Session.Tick(.1f);Assert.IsTrue(g.Session.Frenzy);
            g.GoZone(4);g.Session.Assimilate(Mutation.Claw,false);foreach(var a in g.World.Actors)if(a.Kind==ActorKind.Node)a.Hit();
            Actor(ActorKind.Yimmy).Interact();Assert.AreEqual(ScreenState.Ending,g.State);Assert.IsTrue(g.Session.Tragic);yield return null;
        }
        [UnityTest]public IEnumerator IT04_SniffingAndExpiredPower()
        {
            g.Player.AutoSniff=true;yield return new WaitForSeconds(.1f);Assert.IsTrue(g.Player.Sniffing);
            g.Session.Data.serum=true;g.Session.Assimilate(Mutation.Wing,false);g.Session.Tick(36);Assert.AreEqual(Mutation.None,g.Session.Power);
            Assert.IsNotNull(g.Player.Visual.sprite);Assert.IsTrue(g.Cam.orthographic);
            Assert.Greater(g.World.Actors.Count,0);Assert.AreEqual(RenderMode.ScreenSpaceCamera,Object.FindFirstObjectByType<Canvas>().renderMode);
        }
    }
}