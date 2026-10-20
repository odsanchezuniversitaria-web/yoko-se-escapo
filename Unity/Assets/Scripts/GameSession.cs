using System;
using System.IO;
using UnityEngine;

namespace Yoko
{
    public enum Mutation { None, Wing, Claw }
    public enum ScreenState { Title, Prologue, Playing, Pause, Settings, Archive, Dead, Ending }
    [Serializable] public class SaveData
    {
        public int version = 1, zone, violence, notes;
        public bool serum, foundYimmy;
        public SaveData Copy() => JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(this));
        public bool Valid() => version == 1 && zone >= 0 && zone < 5 && violence >= 0 && violence <= 100 && notes >= 0 && notes < 32 && (zone == 0 || serum) && (zone < 3 || foundYimmy);
    }
    public class GameSession
    {
        public const float PowerDuration = 35, FrenzyDuration = 3, FrenzyInterval = 14;
        public SaveData Data = new SaveData();
        public Mutation Power;
        public float PowerLeft, Stress, FrenzyLeft, FrenzyCooldown;
        public int Health = 3;
        public bool Frenzy => FrenzyLeft > 0;
        public bool Tragic => Data.violence >= 75;
        public void Assimilate(Mutation power, bool live)
        {
            if (!Data.serum) return;
            Power = power; PowerLeft = PowerDuration;
            if(live) AddViolence(25);
        }
        public void AddViolence(int value) { Data.violence = Mathf.Clamp(Data.violence + value, 0, 100); }
        public void Tick(float dt)
        {
            PowerLeft = Mathf.Max(0, PowerLeft - dt);
            if(PowerLeft == 0) Power = Mutation.None;
            FrenzyLeft = Mathf.Max(0, FrenzyLeft - dt);
            FrenzyCooldown = Mathf.Max(0, FrenzyCooldown - dt);
            Stress = Mathf.Max(0, Stress - dt * 1.3f);
            if(!Frenzy && FrenzyCooldown == 0 && (Data.violence >= 75 || Stress >= 85))
            { FrenzyLeft = FrenzyDuration; FrenzyCooldown = FrenzyInterval; Stress = 30; }
        }
        public void Restore(SaveData checkpoint)
        { Data = checkpoint.Copy(); Health = 3; Stress = 0; Power = Mutation.None; PowerLeft = FrenzyLeft = FrenzyCooldown = 0; }
    }
    public static class Saves
    {
        public static string PathName => Path.Combine(Application.persistentDataPath, "yoko-campaign.json");
        public static bool TryRead(out SaveData data)
        {
            data = null;
            try { if(!File.Exists(PathName)) return false; data = JsonUtility.FromJson<SaveData>(File.ReadAllText(PathName)); return data != null && data.Valid(); }
            catch(Exception e) when(e is IOException || e is ArgumentException || e is UnauthorizedAccessException) { return false; }
        }
        public static bool Write(SaveData data)
        {
            try
            {
                string temp = PathName + ".tmp";
                File.WriteAllText(temp, JsonUtility.ToJson(data,true));
                if(File.Exists(PathName)) File.Copy(PathName, PathName + ".bak",true);
                File.Copy(temp,PathName,true); File.Delete(temp);
                return true;
            }
            catch(Exception e) when(e is IOException || e is UnauthorizedAccessException) { Debug.LogWarning("No se pudo guardar: " + e.Message); return false; }
        }
    }
}