using UnityEngine;
namespace Yoko
{
    public class YokoGame : MonoBehaviour
    {
        public static YokoGame Instance {get;private set;}
        public GameSession Session=new GameSession();
        public ScreenState State {get;private set;}
        public YokoMotor Player;public Camera Cam;public GameUI UI;public Soundscape Sound;
        public MotionFX FX;public StoryDirector Story;
        public WorldFactory World=new WorldFactory();
        public SaveData CheckpointData=new SaveData();
        public string HintText="";public float HintUntil;
        public string ArchiveText="";public bool ArchiveReturnToTitle;
        public bool TestMode,GodMode;
        float attackCooldown,prologueTimer;int prologueBeat;
        public static readonly string[] ZoneNames={"LAS RUINAS","LA CIUDAD CAIDA","UN OLOR CONOCIDO","EL EPICENTRO","EL CORAZON"};
        public static readonly string[] Notes={
            "ARCHIVO 01 / OPHIOCORDYCEPS-X\n\nEl hongo reescribe el ADN del huésped a partir de biomasa consumida. El suero limita el control de la red, pero no vuelve invulnerable al sujeto.\n\nEn las instalaciones, un ave yace entre los escombros.",
            "ARCHIVO 02 / LA CAIDA\n\nLo confundimos con un meteorito. Entre los restos había paneles de la estación espacial. Las esporas llegaron antes que los equipos de rescate.",
            "ARCHIVO 03 / DESINFORMACION\n\nCulpan a los animales. Los persiguen. Pero la infección cayó del cielo, no llegó con ellos. El miedo está destruyendo lo que todavía podíamos cuidar.",
            "ARCHIVO 04 / LA RED\n\nLos infectados no cazan solos. El micelio conecta cada cuerpo. Los nodos sostienen las barreras; romperlos interrumpe la red local.",
            "ARCHIVO 05 / IDENTIDAD\n\nEl suero protege durante la adaptación. El estrés y la violencia debilitan esa defensa. Asimilar no significa dejar de ser uno mismo."
        };
        void Awake()
        {
            Instance=this;Application.targetFrameRate=60;QualitySettings.vSyncCount=1;
            Time.timeScale=1;
            var camObj=new GameObject("Main Camera");Cam=camObj.AddComponent<Camera>();camObj.tag="MainCamera";
            Cam.orthographic=true;Cam.orthographicSize=5.1f;Cam.backgroundColor=new Color(.07f,.1f,.12f);Cam.clearFlags=CameraClearFlags.SolidColor;Cam.nearClipPlane=.1f;Cam.farClipPlane=100;
            camObj.AddComponent<AudioListener>();Cam.transform.position=new Vector3(10,3.2f,-20);
            var light=new GameObject("Directional Light").AddComponent<Light>();light.type=LightType.Directional;light.intensity=.6f;
            FX=gameObject.AddComponent<MotionFX>();FX.Setup();
            Player=new GameObject("Yoko").AddComponent<YokoMotor>();Player.Setup();
            Sound=gameObject.AddComponent<Soundscape>();Sound.Setup();
            UI=gameObject.AddComponent<GameUI>();UI.Setup(Cam);Story=gameObject.AddComponent<StoryDirector>();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            gameObject.AddComponent<DebugManager>();
#endif
            Title();
        }
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(State==ScreenState.Playing)ChangeState(ScreenState.Pause);
                else if(State==ScreenState.Archive&&ArchiveReturnToTitle)Title();
                else if(State==ScreenState.Pause||State==ScreenState.Settings||State==ScreenState.Archive)ChangeState(ScreenState.Playing);
            }
            if(State==ScreenState.Playing)
            {
                Session.Tick(Time.deltaTime);attackCooldown=Mathf.Max(0,attackCooldown-Time.deltaTime);
                World.Tick(Time.deltaTime);Story.Tick();
                if(Session.Frenzy)Sound.Threat=1;
                else Sound.Threat=Session.Data.zone*.16f;
            }
            else if(State==ScreenState.Prologue)
            {
                prologueTimer+=Time.deltaTime;
                int beat=Mathf.Min(2,(int)(prologueTimer/3));
                if(beat!=prologueBeat){prologueBeat=beat;UI.Build();}
                if(prologueTimer>9)BeginCampaign();
            }
        }
        void LateUpdate()
        {
            Cam.orthographicSize=Mathf.Max(5.1f,8.8f/Mathf.Max(.5f,Cam.aspect));
            if(State==ScreenState.Playing)
            {
                float half=Cam.orthographicSize*Cam.aspect;
                Vector3 target=new Vector3(Mathf.Clamp(Player.transform.position.x+Player.Facing*2.3f,half,World.Length-half),Mathf.Max(2.45f,Player.transform.position.y+1),-20);
                Cam.transform.position=Vector3.Lerp(Cam.transform.position,target,1-Mathf.Exp(-Time.deltaTime*6));
                Cam.transform.position+=FX.CameraOffset();
            }
        }
        public int PrologueBeat=>prologueBeat;
        public void Title()
        {
            Time.timeScale=1;FX.Clear();World.Build(1);World.Presentation();Player.Warp(new Vector2(8,.2f));
            Cam.transform.position=new Vector3(12,3.2f,-20);Player.Visual.sprite=ArtLibrary.Yoko(0);
            ChangeState(ScreenState.Title);
        }
        public void NewGame(){FX.Clear();GodMode=false;TestMode=false;Session=new GameSession();prologueTimer=0;prologueBeat=0;World.Build(1);World.Add(ActorKind.Yimmy,10,0);ChangeState(ScreenState.Prologue);}
        public void BeginCampaign(){Session=new GameSession();GoZone(0);}
        public void ContinueGame(){GodMode=false;TestMode=false;if(Saves.TryRead(out SaveData data)){Session.Restore(data);LoadZone(data.zone,false);}else Hint("No hay una partida válida guardada.",3);}
        public void GoZone(int zone){Session.Data.zone=Mathf.Clamp(zone,0,4);LoadZone(zone,true);}
        void LoadZone(int zone,bool save)
        {
            FX.Clear();World.Build(zone);Story.ResetZone(zone);Session.Health=3;Session.Power=Mutation.None;Session.PowerLeft=0;
            Player.Warp(new Vector2(2,.2f));Player.ClearInput();Cam.transform.position=new Vector3(9,3.2f,-20);
            CheckpointData=Session.Data.Copy();if(save&&!TestMode&&!Saves.Write(CheckpointData))Hint("No se pudo guardar. Revisa el espacio disponible.",6);
            ChangeState(ScreenState.Playing);
            if(zone==0)Hint("Busca la luz del suero. Olfato revela lo que los ojos no ven.",6);
            else if(zone==1)Hint("Mantén Olfato para reconocer el rastro.",4);
        }
        public void Checkpoint(){CheckpointData=Session.Data.Copy();if(!TestMode&&!Saves.Write(CheckpointData))Hint("No se pudo guardar.",4);}
        public void Retry(){Session.Restore(CheckpointData);LoadZone(Session.Data.zone,false);}
        public void ChangeState(ScreenState state)
        {
            State=state;Time.timeScale=state==ScreenState.Playing||state==ScreenState.Prologue?1:0;
            Player.ClearInput();UI.Build();
        }
        public void Hint(string message,float seconds=3){HintText=message;HintUntil=Time.unscaledTime+seconds;}
        public WorldActor Nearest()
        {
            WorldActor best=null;float distance=2;
            foreach(var a in World.Actors)
            {
                if(!a.gameObject.activeSelf||!a.CanInteract)continue;
                float d=Vector2.Distance(Player.transform.position,a.transform.position);
                if(d<distance){distance=d;best=a;}
            }
            return best;
        }
        public void Interact(){Nearest()?.Interact();}
        public void Attack()
        {
            if(State!=ScreenState.Playing||attackCooldown>0||Player.Sniffing)return;
            attackCooldown=.34f;Player.AttackPose=.22f;Sound.Play(SoundKind.Slash);FX.Slash(Player.transform.position+Vector3.up*.8f,Player.Facing,Session.Power==Mutation.Claw);
            foreach(var a in World.Actors)
            {
                if(!a.gameObject.activeSelf)continue;
                Vector2 delta=a.transform.position-Player.transform.position;
                if(Mathf.Abs(delta.x)<2.3f&&Mathf.Abs(delta.y)<1.8f&&delta.x*Player.Facing>-.3f)a.Hit();
            }
        }
        public void Hurt()
        {
            if(GodMode||State!=ScreenState.Playing||Player.Dodging||Time.time<Player.HurtUntil)return;
            Session.Health--;Session.Stress=Mathf.Min(100,Session.Stress+32);Player.HurtUntil=Time.time+1.4f;Sound.Play(SoundKind.Hurt);FX.Shake(.18f);FX.Burst(Player.transform.position+Vector3.up*.6f,new Color(1,.35f,.4f),12,2);Player.Body.linearVelocity=new Vector2(-Player.Facing*4,4);
            if(Session.Health<=0)Die();
        }
        public void Die(){if(GodMode){Player.Warp(new Vector2(2,.2f));return;}ChangeState(ScreenState.Dead);}
        public void ShowNote(int id){ArchiveReturnToTitle=false;ArchiveText=Notes[Mathf.Clamp(id,0,4)];ChangeState(ScreenState.Archive);}
        public void Finish()
        {
            if(Session.Data.zone!=4||World.RemainingNodes!=0)return;
            Player.Warp(new Vector2(59,.1f));Cam.transform.position=new Vector3(56,3.2f,-20);
            World.Tick(0);ChangeState(ScreenState.Ending);Sound.Cue(Session.Tragic?85:660,1);
        }
        void OnApplicationFocus(bool focus){if(!focus&&State==ScreenState.Playing)ChangeState(ScreenState.Pause);}
        void OnApplicationPause(bool pause){if(pause&&State==ScreenState.Playing)ChangeState(ScreenState.Pause);}
        void OnDestroy(){Time.timeScale=1;if(Instance==this)Instance=null;}
    }
}