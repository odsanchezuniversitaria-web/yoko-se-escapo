using UnityEngine;
namespace Yoko
{
    public class YokoMotor : MonoBehaviour
    {
        public Rigidbody2D Body {get;private set;}
        public SpriteRenderer Visual {get;private set;}
        public bool Sniffing {get;private set;}
        public bool Grounded {get;private set;}
        public int Facing {get;private set;} = 1;
        public float HurtUntil;
        public float TouchMove;
        public bool TouchSniff,TouchJump,JumpPressed,AttackPressed,InteractPressed;
        public bool AutoInput; public float AutoMove; public bool AutoJump,AutoSniff,AutoGlide;
        float coyote,jumpBuffer;
        Transform visualRoot;
        public void Setup()
        {
            Body=gameObject.AddComponent<Rigidbody2D>();Body.gravityScale=2.5f;Body.freezeRotation=true;
            Body.collisionDetectionMode=CollisionDetectionMode2D.Continuous;Body.interpolation=RigidbodyInterpolation2D.Interpolate;
            var col=gameObject.AddComponent<CapsuleCollider2D>();col.direction=CapsuleDirection2D.Horizontal;col.size=new Vector2(1.15f,.7f);col.offset=new Vector2(0,.43f);
            var mat=new PhysicsMaterial2D("Yoko no friction"){friction=0,bounciness=0};col.sharedMaterial=mat;
            visualRoot=new GameObject("Animated Yoko").transform;visualRoot.SetParent(transform,false);
            Visual=ArtLibrary.Draw("Yoko sprite",ArtLibrary.Yoko(0),Vector2.zero,new Vector2(1.85f,1.85f),Color.white,20,visualRoot);
            Visual.transform.localPosition=new Vector3(0,-.22f,0);
        }
        void Update()
        {
            var g=YokoGame.Instance;if(g == null || g.State != ScreenState.Playing) {ClearInput();return;}
            Sniffing=AutoInput?AutoSniff:TouchSniff||Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift);
            if((!AutoInput && Input.GetKeyDown(KeyCode.Space))||JumpPressed||AutoJump) jumpBuffer=.14f;
            JumpPressed=false;AutoJump=false;
            if(InteractPressed||(!AutoInput&&Input.GetKeyDown(KeyCode.E))) {InteractPressed=false;g.Interact();}
            if(AttackPressed||(!AutoInput&&Input.GetKeyDown(KeyCode.J))) {AttackPressed=false;g.Attack();}
            float vx=Mathf.Abs(Body.linearVelocity.x);
            int frame=Time.time<HurtUntil?5:Sniffing?4:!Grounded?3:vx>.3f?1+(Mathf.FloorToInt(Time.time*9)%2):0;
            Visual.sprite=ArtLibrary.Yoko(frame);
            Visual.flipX=Facing<0;
            Visual.color=Time.time<HurtUntil&&Mathf.FloorToInt(Time.time*12)%2==0?new Color(1,.45f,.45f):g.Session.Frenzy?new Color(1,.5f,.62f):Color.white;
            float breathe=1+Mathf.Sin(Time.time*(g.Session.Health==1?9:3))*.018f;
            visualRoot.localScale=new Vector3(1,breathe,1);
            visualRoot.localRotation=Quaternion.Euler(0,0,g.Session.Health==1&&vx>.3f?Mathf.Sin(Time.time*9)*4:0);
        }
        void FixedUpdate()
        {
            var g=YokoGame.Instance;if(g == null || g.State != ScreenState.Playing) return;
            Grounded=Physics2D.OverlapBox((Vector2)transform.position+new Vector2(0,.04f),new Vector2(.85f,.14f),0,1<<8)!=null;
            coyote=Grounded?.12f:Mathf.Max(0,coyote-Time.fixedDeltaTime);jumpBuffer=Mathf.Max(0,jumpBuffer-Time.fixedDeltaTime);
            float move=AutoInput?AutoMove:Mathf.Abs(TouchMove)>.1f?TouchMove:(Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow)?1:0)-(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow)?1:0);
            if(g.Session.Frenzy) move=Mathf.Sin(Time.time*7)>0?1:-1;
            if(Mathf.Abs(move)>.1f) Facing=move>0?1:-1;
            float speed=Sniffing?2:5;
            if(g.Session.Health==1)speed*=.8f;
            Vector2 v=Body.linearVelocity;v.x=move*speed;
            if(jumpBuffer>0&&coyote>0) {v.y=10.5f;jumpBuffer=coyote=0;g.Sound.Cue(540,.08f);}
            bool held=AutoInput?AutoGlide:TouchJump||Input.GetKey(KeyCode.Space);
            if(g.Session.Power==Mutation.Wing && v.y<0 && held)v.y=Mathf.Max(v.y,-1.6f);
            Body.linearVelocity=v;
            if(transform.position.y < -6)g.Die();
            if(transform.position.x<.6f)transform.position=new Vector3(.6f,transform.position.y,0);
            if(transform.position.x>g.World.Length-.5f)transform.position=new Vector3(g.World.Length-.5f,transform.position.y,0);
        }
        public void ClearInput(){TouchMove=0;TouchSniff=TouchJump=JumpPressed=AttackPressed=InteractPressed=false;Sniffing=false;}
        public void Warp(Vector2 p){transform.position=p;Body.position=p;Body.linearVelocity=Vector2.zero;}
    }
}