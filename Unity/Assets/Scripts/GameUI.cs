using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace Yoko
{
    public class GameUI : MonoBehaviour
    {
        Canvas canvas;RectTransform safe,content;Font font;
        Text prompt,hint,power,objective,caption;Image veil,powerMeter;bool confirmNew;
        readonly Color cream=new Color(.94f,.93f,.86f),gold=new Color(.94f,.79f,.36f),dark=new Color(.045f,.075f,.087f,.96f);
        YokoGame G=>YokoGame.Instance;
        public void Setup(Camera cam)
        {
            var go=new GameObject("Interface",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
            canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=cam;canvas.planeDistance=1;canvas.sortingOrder=100;
            var scale=go.GetComponent<CanvasScaler>();scale.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scale.referenceResolution=new Vector2(1600,900);scale.matchWidthOrHeight=.5f;
            safe=Rect("Safe area",go.transform);safe.anchorMin=Vector2.zero;safe.anchorMax=Vector2.one;safe.offsetMin=safe.offsetMax=Vector2.zero;
            font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if(FindFirstObjectByType<EventSystem>()==null)new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));
        }
        RectTransform Rect(string name,Transform parent)
        {var go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(parent,false);return go.GetComponent<RectTransform>();}
        RectTransform Place(string name,Transform parent,float x,float y,float w,float h)
        {var rt=Rect(name,parent);rt.anchorMin=rt.anchorMax=new Vector2(0,1);rt.pivot=new Vector2(0,1);rt.anchoredPosition=new Vector2(x,-y);rt.sizeDelta=new Vector2(w,h);return rt;}
        Image Panel(Transform p,float x,float y,float w,float h,Color c)
        {var r=Place("Panel",p,x,y,w,h);var i=r.gameObject.AddComponent<Image>();i.color=c;return i;}
        Text Label(Transform p,string text,float x,float y,float w,float h,int size,Color? color=null)
        {var r=Place("Text",p,x,y,w,h);var t=r.gameObject.AddComponent<Text>();t.font=font;t.fontSize=size;t.color=color??cream;t.text=text;t.supportRichText=true;t.raycastTarget=false;return t;}
        Button Button(Transform p,string title,float x,float y,float w,Action action,bool primary=false)
        {
            var i=Panel(p,x,y,w,56,primary?gold:new Color(.14f,.20f,.22f,.95f));
            i.gameObject.AddComponent<ButtonMotion>();
            var b=i.gameObject.AddComponent<Button>();b.targetGraphic=i;var colors=b.colors;colors.highlightedColor=new Color(1,1,.8f);b.colors=colors;
            var t=Label(i.transform,title,22,15,w-40,35,21,primary?dark:cream);
            b.onClick.AddListener(()=>{G.Sound.Cue(620,.06f);action();});return b;
        }
        void Sprite(Transform p,Sprite s,float x,float y,float w,float h,Color? tint=null)
        {var r=Place("Portrait",p,x,y,w,h);var i=r.gameObject.AddComponent<Image>();i.sprite=s;i.preserveAspect=true;i.color=tint??Color.white;i.raycastTarget=false;}
        RectTransform Center(float w,float h)
        {
            var rt=Rect("Dialog",content);rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f);rt.sizeDelta=new Vector2(w,h);
            var bg=rt.gameObject.AddComponent<Image>();bg.color=dark;return rt;
        }
        public void Build()
        {
            if(content!=null){content.gameObject.SetActive(false);Destroy(content.gameObject);}
            content=Rect("Screen "+G.State,safe);content.anchorMin=Vector2.zero;content.anchorMax=Vector2.one;content.offsetMin=content.offsetMax=Vector2.zero;
            prompt=hint=power=objective=caption=null;veil=powerMeter=null;
            switch(G.State)
            {
                case ScreenState.Title:Title();break;
                case ScreenState.Prologue:Prologue();break;
                case ScreenState.Playing:HUD();break;
                case ScreenState.Pause:Pause();break;
                case ScreenState.Settings:Settings();break;
                case ScreenState.Archive:Archive();break;
                case ScreenState.Dead:Dead();break;
                case ScreenState.Ending:Ending();break;
            }
        }
        void Title()
        {
            Panel(content,0,0,690,1100,dark);
            Label(content,"UNA HISTORIA EN EL SUR DE BOGOTA",64,65,560,30,17,gold);
            Label(content,"YOKO",58,136,610,150,128);
            Label(content,"SE ESCAPO",65,284,600,80,58,gold);
            Panel(content,66,390,65,4,gold);
            Label(content,"Todo lo que ama está al otro lado\nde una ciudad que ya no reconoce.",66,422,540,90,26);
            Label(content,"RASTREA  /  SOBREVIVE  /  CONSERVA TU IDENTIDAD",66,545,560,35,15,new Color(.61f,.72f,.70f));
            bool saved=Saves.TryRead(out SaveData _);
            Button(content,saved?"CONTINUAR  >":"COMENZAR  >",66,610,470,()=>{if(saved)G.ContinueGame();else G.NewGame();},true);
            if(saved)Button(content,confirmNew?"CONFIRMAR NUEVA PARTIDA":"NUEVA PARTIDA",66,680,470,()=>{if(confirmNew){confirmNew=false;G.NewGame();}else{confirmNew=true;Build();}});
            Button(content,"CONTROLES Y CREDITOS",66,saved?750:680,470,()=>{G.ArchiveReturnToTitle=true;G.ArchiveText="CONTROLES\n\nA / D o flechas: mover\nEspacio: saltar; mantener para planear con Quimera de ave\nShift: olfato · E: interactuar · J: acción · K: esquivar\nEscape: pausa\n\nBIBLIA NARRATIVA\nLaura Daniela Rodríguez Bernal\nAngie Jasbleidy Fajardo Valbuena\nLeidy Yised Estupiñán Salazar\nFundación Universitaria Compensar, 2026\n\nAdaptación jugable con arte generado a partir de las referencias.\nYoko nunca habla. Yimmy no es jugable.";G.ChangeState(ScreenState.Archive);});
            Sprite(content,ArtLibrary.Yoko(0),800,260,630,630);
            Label(content,"01 / EL RASTRO",1050,95,400,50,18,gold);
            Label(content,"Una perrita. Un vínculo.\nUna ciudad en ruinas.",1050,135,450,90,25);
        }
        void Prologue()
        {
            string[] titles={"ANTES DEL RUIDO","LA CAIDA","BAJO LOS ESCOMBROS"};
            string[] lines={"Yimmy. La patineta. El olor de casa.","Algo atraviesa el cielo.\nLa calle desaparece entre el polvo.","Una luz entre las ruinas.\nYoko sigue respirando."};
            Panel(content,0,0,1800,1100,new Color(.025f,.04f,.05f,.65f));
            var p=Center(1020,600);Label(p,titles[G.PrologueBeat],55,45,900,70,48,gold);Label(p,lines[G.PrologueBeat],55,145,530,160,29);
            Sprite(p,G.PrologueBeat==0?ArtLibrary.Cast(0):ArtLibrary.Yoko(G.PrologueBeat==2?5:3),630,120,300,340);
            Label(p,"No hace falta una voz para reconocer a quien amas.",55,380,700,50,21);
            Button(p,"ENTRAR EN LA HISTORIA  >",55,490,460,G.BeginCampaign,true);
        }
        void HUD()
        {
            Panel(content,28,22,550,68,new Color(.035f,.07f,.085f,.88f));
            Label(content,"0"+(G.Session.Data.zone+1),48,37,65,40,30,gold);
            Label(content,YokoGame.ZoneNames[G.Session.Data.zone],118,40,440,40,23);
            var pause=Button(content,"II",0,0,78,()=>G.ChangeState(ScreenState.Pause));var pr=pause.GetComponent<RectTransform>();pr.anchorMin=pr.anchorMax=new Vector2(1,1);pr.anchoredPosition=new Vector2(-108,-28);
            objective=Label(content,"",42,103,1050,34,21,cream);
            power=Label(content,"",42,148,800,30,18,gold);
            var track=Panel(content,42,183,320,5,new Color(.2f,.28f,.3f,.5f));track.raycastTarget=false;
            powerMeter=Panel(content,42,183,320,5,gold);powerMeter.raycastTarget=false;
            var story=Rect("Story caption",content);story.anchorMin=story.anchorMax=new Vector2(.5f,1);story.anchoredPosition=new Vector2(0,-210);story.sizeDelta=new Vector2(1050,76);
            caption=Label(story,"",0,0,1050,76,23,cream);caption.alignment=TextAnchor.MiddleCenter;caption.gameObject.AddComponent<Shadow>().effectColor=new Color(0,0,0,.9f);
            var bottom=Rect("Hints",content);bottom.anchorMin=bottom.anchorMax=new Vector2(.5f,0);bottom.pivot=new Vector2(.5f,0);bottom.sizeDelta=new Vector2(880,100);bottom.anchoredPosition=new Vector2(0,270);
            prompt=Label(bottom,"",0,0,880,36,25,gold);prompt.alignment=TextAnchor.MiddleCenter;
            hint=Label(bottom,"",0,40,880,66,22);hint.alignment=TextAnchor.MiddleCenter;
            if(Application.isMobilePlatform||PlayerPrefs.GetInt("yoko-touch",0)==1)TouchControls();
            else {var t=Label(content,"A / D  MOVER     ESPACIO  SALTO / PLANEO     SHIFT  OLFATO     E  EXPLORAR     J  ATAQUE     K  ESQUIVA",30,0,1450,28,16,new Color(.7f,.8f,.78f));var r=t.rectTransform;r.anchorMin=r.anchorMax=new Vector2(0,0);r.anchoredPosition=new Vector2(35,35);bottom.anchoredPosition=new Vector2(0,95);}
            veil=Panel(content,0,0,2400,1500,Color.clear);veil.raycastTarget=false;
        }
        Image Circle(Transform parent,string name,Vector2 pos,float diameter,Color color,Sprite sprite=null)
        {
            var rt=Rect(name,parent);rt.anchorMin=rt.anchorMax=new Vector2(0,0);rt.pivot=new Vector2(.5f,.5f);rt.anchoredPosition=pos;rt.sizeDelta=Vector2.one*diameter;
            var i=rt.gameObject.AddComponent<Image>();i.sprite=sprite??MotionFX.Disc;i.color=color;return i;
        }
        void TouchControls()
        {
            var joystick=Circle(content,"Joystick",new Vector2(150,150),220,new Color(.06f,.14f,.17f,.72f));
            var border=Circle(joystick.transform,"Outer ring",new Vector2(110,110),208,new Color(.45f,.76f,.76f,.6f),MotionFX.RingSprite);border.raycastTarget=false;
            var knob=Circle(joystick.transform,"Handle",new Vector2(110,110),94,new Color(.79f,.91f,.87f,.92f));
            knob.rectTransform.anchorMin=knob.rectTransform.anchorMax=new Vector2(.5f,.5f);knob.rectTransform.anchoredPosition=Vector2.zero;knob.raycastTarget=false;
            var stick=joystick.gameObject.AddComponent<AnalogStick>();stick.Knob=knob.rectTransform;stick.Changed=v=>G.Player.TouchMove=v;
            var right=Rect("Action cluster",content);right.anchorMin=right.anchorMax=new Vector2(1,0);right.pivot=Vector2.zero;right.anchoredPosition=new Vector2(-490,20);right.sizeDelta=new Vector2(470,300);
            Pad(right,"SALTO",new Vector2(390,110),142,gold,v=>{G.Player.TouchJump=v;if(v)G.Player.JumpPressed=true;});
            Pad(right,"ATAQUE",new Vector2(245,96),112,new Color(.83f,.39f,.32f,.95f),v=>{if(v)G.Player.AttackPressed=true;});
            Pad(right,"ESQUIVA",new Vector2(355,255),98,new Color(.34f,.72f,.77f,.9f),v=>{if(v)G.Player.DodgePressed=true;},()=>G.Player.DodgeCooldown/1.1f);
            Pad(right,"OLFATO",new Vector2(90,96),96,new Color(.29f,.48f,.46f,.9f),v=>G.Player.TouchSniff=v);
            Pad(right,"EXPLORAR",new Vector2(205,235),104,new Color(.4f,.56f,.5f,.95f),v=>{if(v)G.Player.InteractPressed=true;});
        }
        void Pad(Transform parent,string title,Vector2 p,float size,Color color,Action<bool> action,Func<float> cooldown=null)
        {
            var i=Circle(parent,title,p,size,color);var ring=Circle(i.transform,"Border",Vector2.one*(size/2),size-4,new Color(1,1,1,.4f),MotionFX.RingSprite);ring.raycastTarget=false;
            var label=Label(i.transform,title,2,size/2-14,size-4,30,title=="EXPLORAR"?15:18,new Color(.04f,.1f,.12f));label.alignment=TextAnchor.MiddleCenter;label.fontStyle=FontStyle.Bold;
            var pad=i.gameObject.AddComponent<ActionPad>();pad.Changed=action;pad.Cooldown=cooldown;
            if(cooldown!=null){var fill=Circle(i.transform,"Recovery",Vector2.one*(size/2),size-5,new Color(.04f,.09f,.12f,.65f));fill.raycastTarget=false;fill.type=Image.Type.Filled;fill.fillMethod=Image.FillMethod.Radial360;pad.Fill=fill;label.transform.SetAsLastSibling();}
        }
        void Pause()
        {
            var p=Center(620,560);Label(p,"UN RESPIRO",45,40,530,80,48,gold);
            Label(p,"El mundo puede esperar un momento.",45,135,530,55,23);
            Button(p,"VOLVER AL RASTRO",45,230,530,()=>G.ChangeState(ScreenState.Playing),true);
            Button(p,"CONFIGURACION",45,300,530,()=>G.ChangeState(ScreenState.Settings));
            Button(p,"REINTENTAR DESDE REFUGIO",45,370,530,G.Retry);
            Button(p,"VOLVER AL INICIO",45,440,530,G.Title);
        }
        void Settings()
        {
            var p=Center(720,600);Label(p,"CONFIGURACION",45,40,640,65,40,gold);
            Label(p,"Violencia acumulada: "+G.Session.Data.violence+" / 100\nEste valor permanece oculto durante la partida.",45,125,640,95,24);
            Button(p,"SONIDO: "+(AudioListener.volume>0?"ACTIVADO":"SILENCIO"),45,260,630,()=>{AudioListener.volume=AudioListener.volume>0?0:.7f;PlayerPrefs.SetFloat("yoko-volume",AudioListener.volume);Build();});
            Button(p,"CONTROLES TACTILES: "+(PlayerPrefs.GetInt("yoko-touch",0)==1?"SI":"NO"),45,330,630,()=>{PlayerPrefs.SetInt("yoko-touch",1-PlayerPrefs.GetInt("yoko-touch",0));Build();});
            Button(p,"VOLVER",45,470,630,()=>G.ChangeState(ScreenState.Pause),true);
        }
        void Archive()
        {
            var p=Center(1040,760);Label(p,G.ArchiveText,55,45,930,600,24);
            Button(p,"CERRAR",55,660,930,()=>{if(G.ArchiveReturnToTitle)G.Title();else G.ChangeState(ScreenState.Playing);},true);
        }
        void Dead()
        {
            var p=Center(780,510);Label(p,"EL RASTRO SIGUE AHI",45,40,690,80,42,gold);
            Label(p,"Yoko necesita otra oportunidad.\nVuelve al último refugio y busca otro camino.",45,155,690,100,27);
            Button(p,"VOLVER A INTENTAR",45,320,690,G.Retry,true);Button(p,"INICIO",45,390,690,G.Title);
        }
        void Ending()
        {
            var p=Center(1100,650);bool tragic=G.Session.Tragic;
            Label(p,tragic?"LO QUE SE PERDIO":"TODAVIA ES YOKO",55,40,990,80,52,gold);
            Label(p,tragic?"La red se rompe, pero Yoko ya no reconoce\nel olor que la trajo hasta aquí.\n\nSalvar un cuerpo no siempre salva un vínculo.":"Entre el polvo, Yimmy reconoce el collar amarillo.\nYoko se acerca, pequeña y cansada.\n\nNo tuvo que dejar de ser ella para encontrarlo.",55,160,650,260,29);
            Sprite(p,ArtLibrary.Cast(tragic?1:2),735,160,210,330);
            Sprite(p,tragic?ArtLibrary.Yoko(5):ArtLibrary.Yoko(0),855,315,200,200,tragic?new Color(1,.45f,.55f):Color.white);
            Label(p,"FIN  /  "+(tragic?"FRENESI":"EMPATIA"),55,450,700,55,20,gold);
            Button(p,"VOLVER AL INICIO",55,545,990,G.Title,true);
        }
        void Update()
        {
            Rect a=Screen.safeArea;safe.anchorMin=new Vector2(a.x/Screen.width,a.y/Screen.height);safe.anchorMax=new Vector2(a.xMax/Screen.width,a.yMax/Screen.height);
            if(G.State!=ScreenState.Playing)return;
            var actor=G.Nearest();prompt.text=actor==null?"":actor.Prompt;
            objective.text=G.Story.Objective;caption.text=Time.time<G.Story.CaptionUntil?G.Story.Caption:"";
            hint.text=Time.unscaledTime<G.HintUntil?G.HintText:"";
            power.text=G.Session.Power==Mutation.None?"":(G.Session.Power==Mutation.Wing?"QUIMERA / AVE":"QUIMERA / FUERZA")+"     "+Mathf.CeilToInt(G.Session.PowerLeft)+" s";
            powerMeter.rectTransform.sizeDelta=new Vector2(320*Mathf.Clamp01(G.Session.PowerLeft/GameSession.PowerDuration),5);
            veil.color=new Color(.65f,.04f,.13f,G.Session.Frenzy?.12f:Time.time<G.Player.HurtUntil?.10f:0);
        }
    }
}