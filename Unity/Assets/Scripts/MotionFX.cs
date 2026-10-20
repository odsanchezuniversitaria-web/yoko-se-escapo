using UnityEngine;
namespace Yoko {
public class MotionFX : MonoBehaviour {
 class Dot {public SpriteRenderer r;public Vector2 velocity;public float life,max,size;public Color color;public bool ring;}
 readonly Dot[] dots=new Dot[96];int cursor;float shake;static Sprite disc,ring,wing;
 public static Sprite Disc=>disc?disc:disc=Shape(false);
 public static Sprite RingSprite=>ring?ring:ring=Shape(true);
 public static Sprite Wing {
  get {if(wing)return wing;var t=new Texture2D(64,64);var c=new Color[4096];
   for(int y=0;y<64;y++)for(int x=0;x<64;x++){float u=x/63f,v=y/63f;bool inside=v>.12f&&v<1-u*.7f&&u>.05f&&u<.95f;float feather=Mathf.Repeat(u*6,1);c[y*64+x]=inside&&v>(.12f+feather*.18f)?Color.white:Color.clear;}t.SetPixels(c);t.Apply();wing=Sprite.Create(t,new Rect(0,0,64,64),new Vector2(.2f,.1f),64);return wing;}
 }
 static Sprite Shape(bool outline){var t=new Texture2D(64,64);var c=new Color[4096];for(int y=0;y<64;y++)for(int x=0;x<64;x++){float d=Vector2.Distance(new Vector2(x,y),new Vector2(31.5f,31.5f))/31;float a=outline?Mathf.Clamp01((1-Mathf.Abs(d-.87f)/.08f)*3):Mathf.Clamp01((1-d)*12);c[y*64+x]=new Color(1,1,1,a);}t.SetPixels(c);t.Apply();return Sprite.Create(t,new Rect(0,0,64,64),new Vector2(.5f,.5f),64);}
 public void Setup(){for(int i=0;i<dots.Length;i++){var r=ArtLibrary.Draw("FX",Disc,Vector2.zero,Vector2.one,Color.clear,30,transform);r.enabled=false;dots[i]=new Dot{r=r};}}
 Dot Next(Vector2 p,Color color,float life,float size,bool isRing){var d=dots[cursor++%dots.Length];d.r.gameObject.SetActive(true);d.r.enabled=true;d.r.sprite=isRing?RingSprite:Disc;d.r.transform.position=p;d.life=d.max=life;d.size=size;d.color=color;d.ring=isRing;d.velocity=Vector2.zero;return d;}
 public void Burst(Vector2 p,Color color,int count,float speed){for(int i=0;i<count;i++){var d=Next(p,color,Random.Range(.25f,.6f),Random.Range(.055f,.14f),false);d.velocity=new Vector2(Random.Range(-1f,1f),Random.Range(.25f,1.2f))*speed;}}
 public void Ring(Vector2 p,Color color,float size){Next(p,color,.5f,size,true);}
 public void Slash(Vector2 p,int direction,bool claw){var c=claw?new Color(1,.65f,.25f):new Color(.8f,.95f,.95f);for(int i=0;i<12;i++){float angle=Mathf.Lerp(-65,65,i/11f)*Mathf.Deg2Rad;var d=Next(p+new Vector2(Mathf.Cos(angle)*direction,Mathf.Sin(angle))*.9f,c,.16f,.10f,false);d.velocity=new Vector2(direction*2,0);}Shake(.05f);}
 public void Shake(float strength){shake=Mathf.Max(shake,strength);}
 public Vector3 CameraOffset(){shake=Mathf.MoveTowards(shake,0,Time.deltaTime*.8f);return new Vector3(Mathf.Sin(Time.time*83),Mathf.Cos(Time.time*67),0)*shake;}
 void Update(){if(YokoGame.Instance==null||YokoGame.Instance.State!=ScreenState.Playing)return;foreach(var d in dots){if(d.life<=0)continue;d.life-=Time.deltaTime;if(d.life<=0){d.r.enabled=false;continue;}float t=1-d.life/d.max;d.r.transform.position+=(Vector3)(d.velocity*Time.deltaTime);if(!d.ring)d.velocity+=Vector2.down*Time.deltaTime*2;float s=d.ring?Mathf.Lerp(.15f,d.size,t):d.size*(1-t*.4f);d.r.transform.localScale=Vector3.one*s;var c=d.color;c.a*=1-t;d.r.color=c;}}
 public void Clear(){foreach(var d in dots){d.life=0;d.r.enabled=false;}shake=0;}
}
}