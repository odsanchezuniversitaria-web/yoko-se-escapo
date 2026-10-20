using UnityEngine;
namespace Yoko
{
    public class WorldActor : MonoBehaviour
    {
        public ActorKind Kind;public Mutation Power;public int NoteId;public float Home;
        public bool Used; public SpriteRenderer Visual;
        float cooldown,attackTimer,stagger,flash;int hp;Vector3 originalScale;SpriteRenderer halo,warning;Vector3 visualHome;
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
            originalScale=Visual.transform.localScale;visualHome=Visual.transform.localPosition;
            halo=ArtLibrary.Draw("Presence",MotionFX.RingSprite,(Vector2)transform.position+Vector2.up*.45f,Vector2.one*1.1f,new Color(.9f,.8f,.4f,0),10,transform);
            if(IsEnemy){warning=ArtLibrary.Draw("Attack warning",MotionFX.Disc,(Vector2)transform.position+new Vector2(0,.06f),new Vector2(2.8f,.18f),new Color(1,.15f,.2f,0),18,transform);}
            ArtLibrary.Draw("Shadow",MotionFX.Disc,(Vector2)transform.position+new Vector2(0,.03f),new Vector2(IsEnemy?1.5f:.8f,.13f),new Color(.01f,.02f,.03f,.35f),4,transform);
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
                halo.enabled=false;
                if(Kind==ActorKind.Remains&&cooldown==0){Used=false;Visual.enabled=true;}
                return;
            }
            halo.enabled=!IsEnemy&&Kind!=ActorKind.Exit&&Kind!=ActorKind.Yimmy;
            if(halo.enabled){float near=Vector2.Distance(g.Player.transform.position,transform.position);halo.color=new Color(.95f,.8f,.4f,near<2?.6f:g.Player.Sniffing?.35f:.05f);halo.transform.localScale=Vector3.one*(1+Mathf.Sin(Time.time*3)*.12f);}
            if(IsEnemy)
            {
                stagger=Mathf.Max(0,stagger-Time.deltaTime);flash=Mathf.Max(0,flash-Time.deltaTime);
                float dx=g.Player.transform.position.x-transform.position.x;float range=Kind==ActorKind.Boss?2.8f:1.7f;
                bool near=Mathf.Abs(dx)<8;float speed=Kind==ActorKind.Boss?1.35f:Kind==ActorKind.Hostile?1.7f:2;
                if(stagger==0&&attackTimer<=0&&cooldown<.6f){float direction=near?Mathf.Abs(dx)>1.3f?Mathf.Sign(dx):0:Mathf.Sin(Time.time*.7f+Home);float x=Mathf.Clamp(transform.position.x+direction*speed*Time.deltaTime,Home-6,Home+6);transform.position=new Vector3(x,transform.position.y,0);}
                Visual.flipX=dx>0;
                if(attackTimer>0){
                    attackTimer-=Time.deltaTime;warning.color=new Color(1,.18f,.1f,.3f+Mathf.Sin(Time.time*25)*.15f);
                    if(attackTimer<=0){
                        g.FX.Slash(transform.position+Vector3.up*.8f,dx>0?1:-1,true);g.Sound.Play(SoundKind.Slash,.55f);
                        if(Mathf.Abs(dx)<range+.45f&&Mathf.Abs(g.Player.transform.position.y-transform.position.y)<1.5f)g.Hurt();
                        transform.position+=new Vector3(Mathf.Sign(dx)*.5f,0,0);cooldown=Kind==ActorKind.Boss?1.8f:1.35f;
                    }
                } else {warning.color=Color.clear;if(stagger==0&&Mathf.Abs(dx)<range&&cooldown==0){attackTimer=Kind==ActorKind.Boss?.85f:.65f;g.Sound.Play(SoundKind.Alert,.5f);}}
                Visual.color=flash>0?new Color(1,.8f,.35f):attackTimer>0?new Color(1,.55f,.48f):Color.white;
                float walk=Mathf.Sin(Time.time*7+Home);Visual.transform.localPosition=visualHome+new Vector3(attackTimer>0?-Mathf.Sign(dx)*.18f:0,near?Mathf.Abs(walk)*.045f:0,0);
                Visual.transform.localRotation=Quaternion.Euler(0,0,attackTimer>0?-Mathf.Sign(dx)*12:stagger>0?Mathf.Sign(dx)*15:walk*2.5f);
                Visual.transform.localScale=Vector3.Scale(originalScale,new Vector3(1,1+Mathf.Sin(Time.time*4)*.025f,1));
            }
            else if(Kind==ActorKind.Prey){transform.position=new Vector3(Home+Mathf.Sin(Time.time)*.7f,transform.position.y,0);Visual.transform.localPosition=visualHome+Vector3.up*Mathf.Abs(Mathf.Sin(Time.time*8))*.12f;Visual.flipX=Mathf.Cos(Time.time)<0;}
            else if(Kind==ActorKind.Node)Visual.transform.localScale=originalScale*(1+Mathf.Sin(Time.time*4)*.07f);
            else if(Kind==ActorKind.Serum||Kind==ActorKind.Note||Kind==ActorKind.Clue)Visual.transform.localPosition=visualHome+Vector3.up*(Mathf.Sin(Time.time*2+Home)*.08f);
        }
        public void Interact()
        {
            var g=YokoGame.Instance;if(!CanInteract)return;
            switch(Kind)
            {
                case ActorKind.Serum:g.Session.Data.serum=true;Used=true;Visual.enabled=false;g.Sound.Play(SoundKind.Assimilate);g.FX.Ring(g.Player.transform.position+Vector3.up*.5f,new Color(.5f,1,.9f),3);g.FX.Burst(transform.position,Color.cyan,22,2);g.Story.Say("El cristal cede. El suero toca su piel. Por un instante, la red deja de oírla.",6);g.Hint("Quimera despertó. Explora los restos del ave.",5);break;
                case ActorKind.Remains:
                case ActorKind.Prey:
                    if(!g.Session.Data.serum)return;
                    g.Session.Assimilate(Power,Kind==ActorKind.Prey);Used=true;Visual.enabled=false;cooldown=12;g.Sound.Play(SoundKind.Assimilate);g.FX.Ring(g.Player.transform.position+Vector3.up*.5f,Kind==ActorKind.Prey?Color.red:new Color(.5f,1,.8f),2.7f);g.FX.Burst(g.Player.transform.position,Color.yellow,15,2);g.Hint(Power==Mutation.Wing?"Quimera: ave · mantén Salto para planear":"Quimera: fuerza · Acción rompe el micelio",4);break;
                case ActorKind.Note:g.Session.Data.notes|=1<<NoteId;g.ShowNote(NoteId);break;
                case ActorKind.Clue:Used=true;g.Sound.Play(SoundKind.Objective);g.FX.Burst(transform.position,new Color(1,.8f,.3f),16,2);g.Story.Say("La madera de la patineta está rota. En ella: «Si te pierdes, nos encontramos donde siempre». Yoko levanta las orejas.",7);break;
                case ActorKind.Yimmy:
                    if(g.Session.Data.zone==2){g.Session.Data.foundYimmy=true;g.Session.Stress=70;g.Story.Say("«Yoko… volviste». Yimmy intenta tocar el collar. Las raíces lo arrastran. «Rompe los nudos. Todavía podemos salir».",8);g.Sound.Play(SoundKind.Objective);g.FX.Shake(.12f);g.Hint("Rescátalo: sigue las raíces hasta el corazón.",6);g.Checkpoint();}
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
            hp-=g.Session.Power==Mutation.Claw?2:1;g.Sound.Play(Kind==ActorKind.Node?SoundKind.Node:SoundKind.Hit);g.FX.Burst(transform.position+Vector3.up,Kind==ActorKind.Node?new Color(1,.25f,.55f):new Color(1,.75f,.35f),18,3);g.FX.Shake(Kind==ActorKind.Node?.2f:.08f);stagger=.35f;flash=.18f;attackTimer=0;
            if(hp<=0){Used=true;gameObject.SetActive(false);if(Kind==ActorKind.Node)g.Hint("Una parte de la red se apaga.",2);}
            else if(IsEnemy)transform.position+=new Vector3(g.Player.Facing*.7f,0,0);
        }
    }
}