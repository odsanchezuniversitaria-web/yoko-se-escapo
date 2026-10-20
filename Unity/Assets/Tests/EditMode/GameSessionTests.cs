using NUnit.Framework;
using UnityEngine;
namespace Yoko.Tests
{
    public class GameSessionTests
    {
        [Test]public void UT01_RemainsAndHuntingHaveDistinctConsequences()
        {var s=new GameSession();s.Data.serum=true;s.Assimilate(Mutation.Wing,false);Assert.AreEqual(0,s.Data.violence);s.Assimilate(Mutation.Claw,true);Assert.AreEqual(25,s.Data.violence);s.AddViolence(200);Assert.AreEqual(100,s.Data.violence);}
        [Test]public void UT02_NoAssimilationBeforeSerum()
        {var s=new GameSession();s.Assimilate(Mutation.Wing,true);Assert.AreEqual(Mutation.None,s.Power);Assert.AreEqual(0,s.Data.violence);}
        [Test]public void UT03_PowerExpiresAndHealthDoesNotIncrease()
        {var s=new GameSession();s.Data.serum=true;s.Health=1;s.Assimilate(Mutation.Wing,false);s.Tick(36);Assert.AreEqual(Mutation.None,s.Power);Assert.AreEqual(1,s.Health);}
        [Test]public void UT04_FrenzyHasBoundedDurationAndCooldown()
        {var s=new GameSession();s.Data.violence=75;s.Tick(.01f);Assert.IsTrue(s.Frenzy);s.Tick(3.1f);Assert.IsFalse(s.Frenzy);s.Tick(1);Assert.IsFalse(s.Frenzy);s.Tick(11);Assert.IsTrue(s.Frenzy);}
        [Test]public void UT05_StressCanTriggerWithoutViolence()
        {var s=new GameSession();s.Stress=90;s.Tick(.01f);Assert.IsTrue(s.Frenzy);Assert.IsFalse(s.Tragic);}
        [Test]public void UT06_EndingThreshold()
        {var s=new GameSession();s.Data.violence=74;Assert.IsFalse(s.Tragic);s.AddViolence(1);Assert.IsTrue(s.Tragic);}
        [Test]public void UT07_SaveValidationAndRoundTrip()
        {var d=new SaveData{zone=3,violence=25,serum=true,foundYimmy=true,notes=7};var copy=JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(d));Assert.IsTrue(copy.Valid());Assert.AreEqual(7,copy.notes);copy.zone=6;Assert.IsFalse(copy.Valid());copy.zone=3;copy.foundYimmy=false;Assert.IsFalse(copy.Valid());}
        [Test]public void UT08_RestoreCopiesAndClearsTemporaryStates()
        {var d=new SaveData{zone=1,serum=true,violence=25};var s=new GameSession();s.Restore(d);s.Data.violence=50;Assert.AreEqual(25,d.violence);Assert.AreEqual(3,s.Health);Assert.AreEqual(Mutation.None,s.Power);}
    }
}