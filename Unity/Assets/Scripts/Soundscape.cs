using System;using System.Collections.Generic;using UnityEngine;
namespace Yoko {
public enum SoundKind { Step,Jump,Land,Dodge,Slash,Hit,Hurt,Assimilate,Sniff,Alert,Objective,Node,Memory }
public class Soundscape : MonoBehaviour {
 AudioSource ambience,music,effects;readonly Dictionary<SoundKind,AudioClip> clips=new Dictionary<SoundKind,AudioClip>();public float Threat;
 const int Rate=22050;static readonly float[] Chord={440,554.37f,659.25f,880};
 public void Setup(){
  ambience=gameObject.AddComponent<AudioSource>();music=gameObject.AddComponent<AudioSource>();effects=gameObject.AddComponent<AudioSource>();
  ambience.loop=music.loop=true;AudioListener.volume=PlayerPrefs.GetFloat("yoko-volume",.7f);
  foreach(SoundKind kind in Enum.GetValues(typeof(SoundKind)))clips[kind]=Effect(kind);
  ambience.clip=Create("Lluvia y viento",16,t=>{float breath=.65f+.35f*Mathf.Sin(t*.4f);return (Mathf.Sin(t*345)*.08f+Noise(t*Rate)*.12f)*breath;});
  float[] notes={220,261.63f,329.63f,293.66f,220,196,164.81f,196};
  music.clip=Create("El rastro - motivo",16,t=>{int n=(int)(t*2)%32;float age=Mathf.Repeat(t,.5f),hz=notes[(n/2)%8]*(n%2==0?1:2);float bell=(Mathf.Sin(t*hz*2*Mathf.PI)+Mathf.Sin(t*hz*4*Mathf.PI)*.22f)*Mathf.Exp(-age*7)*.10f;float pad=(Mathf.Sin(t*2*Mathf.PI*110)+Mathf.Sin(t*2*Mathf.PI*164.81f))*.035f;return bell+pad;});
  ambience.Play();music.Play();
 }
 static float Noise(float seed){return Mathf.Sin(seed*12.9898f+78.233f)*43758.5453f%1f;}
 AudioClip Create(string name,float seconds,Func<float,float> sample){int n=(int)(Rate*seconds);var data=new float[n];for(int i=0;i<n;i++)data[i]=Mathf.Clamp(sample(i/(float)Rate),-.95f,.95f);var c=AudioClip.Create(name,n,1,Rate,false);c.SetData(data,0);return c;}
 AudioClip Effect(SoundKind kind){
  float length=kind==SoundKind.Objective||kind==SoundKind.Memory?1.1f:kind==SoundKind.Assimilate?.7f:kind==SoundKind.Alert?.55f:.28f;
  return Create(kind.ToString(),length,t=>{float u=t/length,env=Mathf.Sin(Mathf.PI*u)*Mathf.Exp(-u*2),noise=Noise(t*Rate);
   switch(kind){
   case SoundKind.Step:return noise*Mathf.Exp(-t*38)*.12f;
   case SoundKind.Land:return (noise*.4f+Mathf.Sin(t*390)*.35f)*env;
   case SoundKind.Jump:return (Mathf.Sin(2*Mathf.PI*(300*t+650*t*t))*.25f+noise*.08f)*env;
   case SoundKind.Dodge:return noise*env*.5f;
   case SoundKind.Slash:return (noise*.6f+Mathf.Sin(t*1300)*.12f)*env;
   case SoundKind.Hit:case SoundKind.Node:return (noise*.5f+Mathf.Sin(2*Mathf.PI*(100*t-70*t*t))*.6f)*env;
   case SoundKind.Hurt:return (Mathf.Sin(t*1100)+Mathf.Sin(t*1250))*.2f*env;
   case SoundKind.Sniff:return noise*.22f*env*(.6f+.4f*Mathf.Sin(t*70));
   case SoundKind.Alert:return Mathf.Sin(t*2*Mathf.PI*(180+Mathf.Sin(t*24)*30))*.3f*env;
   default:int n=Mathf.Min(3,(int)(u*4));return (Mathf.Sin(t*2*Mathf.PI*Chord[n])*.25f+Mathf.Sin(t*4*Mathf.PI*Chord[n])*.06f)*env;
  }});
 }
 void Update(){if(!ambience)return;float target=YokoGame.Instance!=null&&YokoGame.Instance.State==ScreenState.Playing?1:.5f;music.volume=Mathf.Lerp(music.volume,(.25f+Threat*.12f)*target,Time.unscaledDeltaTime*2);ambience.volume=.24f+Threat*.1f;music.pitch=1+Threat*.06f;}
 public void Play(SoundKind kind,float volume=1){effects.PlayOneShot(clips[kind],volume);}
 public void Cue(float hz,float duration){Play(hz<250?SoundKind.Hit:hz>700?SoundKind.Objective:SoundKind.Memory,.6f);}
 void OnDestroy(){foreach(var c in clips.Values)Destroy(c);if(ambience&&ambience.clip)Destroy(ambience.clip);if(music&&music.clip)Destroy(music.clip);}
}
}
