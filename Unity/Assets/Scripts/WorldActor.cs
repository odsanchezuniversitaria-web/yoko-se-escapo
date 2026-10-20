using UnityEngine;
namespace Yoko
{
    public class WorldActor : MonoBehaviour
    {
        public ActorKind Kind;public Mutation Power;public int NoteId;public float Home;
        public bool Used; public SpriteRenderer Visual;
        float cooldown,attackTimer;int hp;Vector3 originalScale;
        public bool IsEnemy=>Kind==ActorKind.Enemy||Kind==ActorKind.Hostile||Kind==ActorKind.Boss;
        public bool CanInteract=>!IsEnemy&&!Used;
        public string Prompt
        {
            get {
                switch(Kind){
                    case ActorKind.Serum:return "E  /  EXAMINAR SUERO";
                    case ActorKind.Remains:return "E  /  ASIMILAR RESTOS";
                    case ActorKind.Prey:return "E  /  CAZAR";
                    case ActorKind.Note:return "E  /  LEER ARCHIVO";
                    case ActorKind.Exit:return "E  /  SEGUIR EL RASTRO";
                    case ActorKind.Yimmy:return "E  /  ACERCARSE";
                    case ActorKind.Clue:return "E  /  RECONOCER OLOR";
                    case ActorKind.Node:return "J  /  ROMPER CON QUIMERA";
                    default:return "";
                }
            }
        }
        public void Setup()
        {
            Color col=Color.white;Sprite sprite=ArtLibrary.Square;Vector2 size=Vector2.one;
            if(IsEnemy){sprite=ArtLibrary.Cast(Kind==ActorKind.Boss?6:Kind==ActorKind.Hostile?7:4);size=Kind==ActorKind.Boss?new Vector2(3.6f,4):new Vector2(1.8f,2.9f);hp=Kind==ActorKind.Boss?5:2;}
            else if(Kind==ActorKind.Yimmy){sprite=ArtLibrary.Cast(1);size=new Vector2(2.3f,2.9f);}
            else if(Kind==ActorKind.Clue){sprite=ArtLibrary.Cast(3);size=new Vector2(1.4f,1.4f);}
            else if(Kind==ActorKind.Remains){sprite=ArtLibrary.Prop(Power==Mutation.Wing?0:2);size=new Vector2(1.1f,1.1f);}
            else if(Kind==ActorKind.Prey){sprite=ArtLibrary.Prop(Power==Mutation.Wing?1:3);size=new Vector2(.9f,1.1f);}
            else if(Kind==ActorKind.Serum){sprite=ArtLibrary.Prop(4);size=new Vector2(.9f,1.1f);}
            else if(Kind==ActorKind.Note){sprite=ArtLibrary.Prop(5);size=new Vector2(.8f,1);}
            else if(Kind==ActorKind.Exit){col=new Color(.55f,.73f,.65f,.35f);size=new Vector2(.2f,3);}
            else if(Kind==ActorKind.Node){sprite=ArtLibrary.Prop(6);size=new Vector2(1.6f,2.8f);hp=2;}
            Visual=ArtLibrary.Draw("Visual",sprite,transform.position,size,col,IsEnemy?15:12,transform);
            if(sprite==ArtLibrary.Square)Visual.transform.localPosition=new Vector3(0,size.y/2,0);
            originalScale=Visual.transform.localScale;
            if(Kind==ActorKind.Node)
            {
                for(int i=0;i<5;i++){var r=ArtLibrary.Box("Micelio",new Vector2(transform.position.x+(i-2)*.2f,.9f),new Vector2(.08f,2.6f),new Color(.65f,.17f,.31f),11,transform);r.transform.Rotate(0,0,(i-2)*16);}
            }
        }
        void Update()
        {
            var g=YokoGame.Instance;if(g==null||g.State!=ScreenState.Playing)return;
            cooldown=Mathf.Max(0,cooldown-Time.deltaTime);
            if(Used)
            {
                if(Kind==ActorKind.Remains&&cooldown==0){Used=false;Visual.enabled=true;}
                return;
            }
            if(IsEnemy)
            {
                float dx=g.Player.transform.position.x-transform.position.x;
                bool near=Mathf.Abs(dx)<7;
                if(near && Mathf.Abs(dx)>1.4f)
                {
                    float speed=Kind==ActorKind.Boss?.75f:Kind==ActorKind.Hostile?1.2f:1.5f;
                    float x=Mathf.Clamp(transform.position.x+Mathf.Sign(dx)*speed*Time.deltaTime,Home-5,Home+5);
                    transform.position=new Vector3(x,transform.position.y,0);
                }
                Visual.flipX=dx>0;
                float range=Kind==ActorKind.Boss?2.8f:1.6f;
                if(attackTimer>0)
                {
                    attackTimer-=Time.deltaTime;Visual.color=new Color(1,.45f,.45f);
                    if(attackTimer<=0){if(Mathf.Abs(dx)<range&&Mathf.Abs(g.Player.transform.position.y-transform.position.y)<1.5f)g.Hurt();cooldown=Kind==ActorKind.Boss?2.1f:1.5f;}
                }
                else {Visual.color=Color.white;if(Mathf.Abs(dx)<range&&cooldown==0)attackTimer=.65f;}
                Visual.transform.localScale=Vector3.Scale(originalScale,new Vector3(1,1+Mathf.Sin(Time.time*3)*.025f,1));
            }
            else if(Kind==ActorKind.Prey)transform.position=new Vector3(Home+Mathf.Sin(Time.time)*.5f,transform.position.y,0);
            else if(Kind==ActorKind.Node)Visual.transform.localScale=originalScale*(1+Mathf.Sin(Time.time*4)*.035f);
        }
        public void Interact()
        {
            var g=YokoGame.Instance;if(!CanInteract)return;
            switch(Kind)
            {
                case ActorKind.Serum:g.Session.Data.serum=true;Used=true;Visual.enabled=false;g.Sound.Cue(880,.4f);g.Hint("El suero se derrama sobre Yoko. Algo cambia.",4);break;
                case ActorKind.Remains:
                case ActorKind.Prey:
                    if(!g.Session.Data.serum)return;
                    g.Session.Assimilate(Power,Kind==ActorKind.Prey);Used=true;Visual.enabled=false;cooldown=12;g.Sound.Cue(Kind==ActorKind.Prey?140:620,.25f);g.Hint(Power==Mutation.Wing?"Quimera: ave · mantén Salto para planear":"Quimera: fuerza · Acción rompe el micelio",4);break;
                case ActorKind.Note:g.Session.Data.notes|=1<<NoteId;g.ShowNote(NoteId);break;
                case ActorKind.Clue:g.Sound.Cue(740,.2f);g.Hint("El olor de Yimmy aún está aquí.",3);break;
                case ActorKind.Yimmy:
                    if(g.Session.Data.zone==2){g.Session.Data.foundYimmy=true;g.Session.Stress=70;g.Hint("Yimmy está atrapado. El micelio se extiende hacia el epicentro.",5);g.Checkpoint();}
                    else if(g.Session.Data.zone==4){if(g.World.RemainingNodes==0)g.Finish();else g.Hint("El micelio todavía lo retiene.",3);}
                    break;
                case ActorKind.Exit:
                    if(g.Session.Data.zone==0&&!g.Session.Data.serum){g.Hint("Algo brilla entre los escombros.",2);return;}
                    if(g.Session.Data.zone==2&&!g.Session.Data.foundYimmy){g.Hint("El rastro se detiene cerca de alguien.",2);return;}
                    if(g.World.RemainingNodes>0){g.Hint("La red micelial bloquea el paso.",2);return;}
                    g.GoZone(g.Session.Data.zone+1);break;
            }
        }
        public void Hit()
        {
            var g=YokoGame.Instance;
            if(Used||(!IsEnemy&&Kind!=ActorKind.Node))return;
            if(Kind==ActorKind.Node&&g.Session.Power!=Mutation.Claw){g.Hint("Las garras de Quimera podrían romperlo.",2);return;}
            if(IsEnemy)g.Session.AddViolence(8);
            hp-=g.Session.Power==Mutation.Claw?2:1;g.Sound.Cue(110,.12f);
            if(hp<=0){Used=true;gameObject.SetActive(false);if(Kind==ActorKind.Node)g.Hint("Una parte de la red se apaga.",2);}
            else if(IsEnemy)transform.position+=new Vector3(g.Player.Facing*.7f,0,0);
        }
    }
}