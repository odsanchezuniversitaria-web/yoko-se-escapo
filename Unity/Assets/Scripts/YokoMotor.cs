using UnityEngine;
namespace Yoko {
public class YokoMotor : MonoBehaviour {
 public Rigidbody2D Body {get;private set;} public SpriteRenderer Visual {get;private set;}
 public bool Sniffing {get;private set;} public bool Grounded {get;private set;} public int Facing {get;private set;}=1;
 public float HurtUntil,TouchMove; public bool TouchSniff,TouchJump,JumpPressed,AttackPressed,InteractPressed,DodgePressed;
 public bool AutoInput; public float AutoMove; public bool AutoJump,AutoSniff,AutoGlide;
 public float DodgeCooldown {get;private set;} public bool Dodging=>dashTime>0;
 public float AttackPose;
 float coyote,jumpBuffer,dashTime,landSquash,stepClock,animationClock,previousY; int dashDirection;
 Transform visualRoot; SpriteRenderer wing;
 public void Setup(){
  Body=gameObject.AddComponent<Rigidbody2D>();Body.gravityScale=2.5f;Body.freezeRotation=true;Body.collisionDetectionMode=CollisionDetectionMode2D.Continuous;Body.interpolation=RigidbodyInterpolation2D.Interpolate;
  var col=gameObject.AddComponent<CapsuleCollider2D>();col.direction=CapsuleDirection2D.Horizontal;col.size=new Vector2(1.15f,.7f);col.offset=new Vector2(0,.43f);
  col.sharedMaterial=new PhysicsMaterial2D("Yoko no friction"){friction=0,bounciness=0};
  visualRoot=new GameObject("Animated Yoko").transform;visualRoot.SetParent(transform,false);
  Visual=ArtLibrary.Draw("Yoko sprite",ArtLibrary.Yoko(0),Vector2.zero,new Vector2(1.85f,1.85f),Color.white,20,visualRoot);Visual.transform.localPosition=new Vector3(0,-.22f,0);
  wing=ArtLibrary.Draw("Quimera wing",MotionFX.Wing,Vector2.zero,new Vector2(1.5f,1.2f),new Color(.55f,.95f,.9f,.8f),19,visualRoot);wing.transform.localPosition=new Vector3(-.1f,.55f,0);wing.enabled=false;
 }
 void Update(){
  var g=YokoGame.Instance;if(g==null||g.State!=ScreenState.Playing){ClearInput();return;}
  float dt=Time.deltaTime;DodgeCooldown=Mathf.Max(0,DodgeCooldown-dt);AttackPose=Mathf.Max(0,AttackPose-dt);
  bool wasSniff=Sniffing;Sniffing=!Dodging&&(AutoInput?AutoSniff:TouchSniff||Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift));
  if(Sniffing&&!wasSniff){g.Sound.Play(SoundKind.Sniff);g.FX.Ring(transform.position+Vector3.up*.5f,new Color(.9f,.8f,.4f),2);}
  if((!AutoInput&&Input.GetKeyDown(KeyCode.Space))||JumpPressed||AutoJump)jumpBuffer=.16f;
  JumpPressed=AutoJump=false;
  if(DodgePressed||(!AutoInput&&(Input.GetKeyDown(KeyCode.K)||Input.GetKeyDown(KeyCode.LeftControl))))TryDodge();
  DodgePressed=false;
  if(InteractPressed||(!AutoInput&&Input.GetKeyDown(KeyCode.E))){InteractPressed=false;g.Interact();}
  if(AttackPressed||(!AutoInput&&Input.GetKeyDown(KeyCode.J))){AttackPressed=false;g.Attack();}
  float vx=Mathf.Abs(Body.linearVelocity.x);animationClock+=dt*(5+vx*1.3f);
  int frame=Time.time<HurtUntil?5:Sniffing?4:!Grounded?3:vx>.3f?1+((int)animationClock%2):0;
  Visual.sprite=ArtLibrary.Yoko(frame);Visual.flipX=Facing<0;
  Visual.color=Dodging?new Color(.65f,1,1):Time.time<HurtUntil&&Mathf.FloorToInt(Time.time*12)%2==0?new Color(1,.5f,.5f):g.Session.Frenzy?new Color(1,.5f,.62f):Color.white;
  landSquash=Mathf.MoveTowards(landSquash,0,dt*4);float bob=Grounded&&vx>.3f?Mathf.Abs(Mathf.Sin(animationClock*Mathf.PI))*.055f:0;
  float stretch=!Grounded?Mathf.Clamp(Body.linearVelocity.y*.014f,-.12f,.14f):0;
  visualRoot.localScale=new Vector3(1+landSquash*.22f-stretch*.3f+(Dodging?.18f:0),1-landSquash*.2f+stretch-(Dodging?.13f:0),1);
  visualRoot.localPosition=new Vector3(AttackPose>0?Facing*Mathf.Sin(AttackPose*18)*.18f:0,bob,0);
  visualRoot.localRotation=Quaternion.Euler(0,0,Dodging?-Facing*8:!Grounded?Mathf.Clamp(Body.linearVelocity.y*Facing,-10,10):Sniffing?-Facing*4:Mathf.Sin(animationClock*Mathf.PI)*vx*.3f);
  wing.enabled=g.Session.Power==Mutation.Wing;wing.flipX=Facing<0;wing.transform.localRotation=Quaternion.Euler(0,0,Facing*(25+Mathf.Sin(Time.time*(Grounded?5:15))*22));
  if(Grounded&&vx>1&&!Sniffing){stepClock-=dt;if(stepClock<=0){stepClock=.24f;g.Sound.Play(SoundKind.Step,.45f);g.FX.Burst(transform.position,new Color(.65f,.67f,.59f,.45f),3,.7f);}}
 }
 public bool TryDodge(){
  var g=YokoGame.Instance;if(g.State!=ScreenState.Playing||DodgeCooldown>0||g.Session.Frenzy)return false;
  dashTime=.18f;dashDirection=Facing;DodgeCooldown=1.1f;Sniffing=false;g.Sound.Play(SoundKind.Dodge);g.FX.Ring(transform.position+Vector3.up*.4f,new Color(.5f,.9f,.9f),1);return true;
 }
 void FixedUpdate(){
  var g=YokoGame.Instance;if(g==null||g.State!=ScreenState.Playing)return;
  bool was=Grounded;Grounded=Physics2D.OverlapBox((Vector2)transform.position+new Vector2(0,.04f),new Vector2(.85f,.14f),0,1<<8)!=null;
  if(Grounded&&!was&&previousY< -2){landSquash=1;g.Sound.Play(SoundKind.Land);g.FX.Burst(transform.position,new Color(.7f,.7f,.6f,.65f),9,1.5f);}
  float dt=Time.fixedDeltaTime;coyote=Grounded?.12f:Mathf.Max(0,coyote-dt);jumpBuffer=Mathf.Max(0,jumpBuffer-dt);
  float move=AutoInput?AutoMove:Mathf.Abs(TouchMove)>.08f?TouchMove:(Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow)?1:0)-(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow)?1:0);
  if(g.Session.Frenzy)move=Mathf.Sin(Time.time*7)>0?1:-1;if(Mathf.Abs(move)>.1f&&!Dodging)Facing=move>0?1:-1;
  float speed=Sniffing?2.1f:6;if(g.Session.Health==1)speed*=.85f;
  Vector2 v=Body.linearVelocity;v.x=Mathf.MoveTowards(v.x,move*speed,(Mathf.Abs(move)>.05f?42:55)*dt);
  if(Dodging){v.x=dashDirection*13;v.y=Mathf.Max(v.y,-1);dashTime=Mathf.Max(0,dashTime-dt);g.FX.Burst(transform.position+Vector3.up*.4f,new Color(.3f,.9f,.86f,.35f),1,.3f);}
  if(jumpBuffer>0&&coyote>0){v.y=10.5f;jumpBuffer=coyote=0;landSquash=-.5f;g.Sound.Play(SoundKind.Jump);g.FX.Burst(transform.position,new Color(.8f,.75f,.55f,.6f),6,1);}
  bool held=AutoInput?AutoGlide:TouchJump||Input.GetKey(KeyCode.Space);
  if(!AutoInput&&!held&&v.y>3)v.y-=28*dt;
  if(g.Session.Power==Mutation.Wing&&v.y<0&&held)v.y=Mathf.Max(v.y,-1.6f);
  if(v.y<0&&!held)v.y-=8*dt;v.y=Mathf.Max(v.y,-17);previousY=v.y;Body.linearVelocity=v;
  if(transform.position.y< -6)g.Die();
  if(transform.position.x<.6f)transform.position=new Vector3(.6f,transform.position.y,0);
  if(transform.position.x>g.World.Length-.5f)transform.position=new Vector3(g.World.Length-.5f,transform.position.y,0);
 }
 public void ClearInput(){TouchMove=0;TouchSniff=TouchJump=JumpPressed=AttackPressed=InteractPressed=DodgePressed=false;Sniffing=false;}
 public void Warp(Vector2 p){transform.position=p;Body.position=p;Body.linearVelocity=Vector2.zero;dashTime=0;DodgeCooldown=0;HurtUntil=0;}
}
}