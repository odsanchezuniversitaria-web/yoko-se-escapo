using UnityEngine;
namespace Yoko {
public class SporeVent:MonoBehaviour {
 public float Offset;SpriteRenderer ring,column;float lastDamage,lastBurst;bool wasWarning;
 public void Setup(){ring=ArtLibrary.Draw("Vent warning",MotionFX.RingSprite,transform.position,new Vector2(2.6f,.35f),Color.clear,8,transform);column=ArtLibrary.Draw("Spore plume",MotionFX.Disc,(Vector2)transform.position+Vector2.up*.7f,new Vector2(1.7f,1.7f),Color.clear,9,transform);}
 void Update(){
  var g=YokoGame.Instance;if(g==null||g.State!=ScreenState.Playing)return;float phase=Mathf.Repeat(Time.time+Offset,5.5f);bool warning=phase>2.8f&&phase<4,active=phase>=4;
  ring.color=warning?new Color(1,.65f,.2f,.4f+.3f*Mathf.Sin(Time.time*18)):active?new Color(1,.25f,.4f,.8f):new Color(.6f,.2f,.4f,.18f);
  column.color=active?new Color(.9f,.2f,.5f,.2f):Color.clear;
  if(warning&&!wasWarning&&Vector2.Distance(g.Player.transform.position,transform.position)<8)g.Sound.Play(SoundKind.Alert,.25f);wasWarning=warning;
  if(active){
   if(Time.time-lastBurst>.12f){lastBurst=Time.time;g.FX.Burst(transform.position,new Color(.95f,.3f,.55f,.5f),3,2);}
   if(Time.time-lastDamage>1&&Mathf.Abs(g.Player.transform.position.x-transform.position.x)<.9f&&g.Player.transform.position.y<1.5f){g.Hurt();lastDamage=Time.time;}
  }
 }
}
}