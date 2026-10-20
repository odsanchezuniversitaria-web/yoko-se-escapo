using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace Yoko
{
    public class TouchAction : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerExitHandler
    {
        public Action<bool> Set;
        public void OnPointerDown(PointerEventData e){Set?.Invoke(true);}
        public void OnPointerUp(PointerEventData e){Set?.Invoke(false);}
        public void OnPointerExit(PointerEventData e){Set?.Invoke(false);}
        void OnDisable(){Set?.Invoke(false);}
    }
    public class GameUI : MonoBehaviour
    {
        Canvas canvas;RectTransform safe,content;Font font;
        Text prompt,hint,power;Image veil;bool confirmNew;
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
            prompt=hint=power=null;veil=null;
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
            Button(content,"CONTROLES Y CREDITOS",66,saved?750:680,470,()=>{G.ArchiveReturnToTitle=true;G.ArchiveText="CONTROLES\n\nA / D o flechas: mover\nEspacio: saltar; mantener para planear con Quimera de ave\nShift: olfato · E: interactuar · J: acción\nEscape: pausa\n\nBIBLIA NARRATIVA\nLaura Daniela Rodríguez Bernal\nAngie Jasbleidy Fajardo Valbuena\nLeidy Yised Estupiñán Salazar\nFundación Universitaria Compensar, 2026\n\nAdaptación jugable con arte generado a partir de las referencias.\nYoko nunca habla. Yimmy no es jugable.";G.ChangeState(ScreenState.Archive);});
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
            Panel(content,32,24,480,70,new Color(.04f,.075f,.09f,.75f));
            Label(content,"0"+(G.Session.Data.zone+1),52,41,65,40,30,gold);
            Label(content,YokoGame.ZoneNames[G.Session.Data.zone],125,45,365,40,23);
            var pause=Button(content,"II",0,0,75,()=>G.ChangeState(ScreenState.Pause));var pr=pause.GetComponent<RectTransform>();pr.anchorMin=pr.anchorMax=new Vector2(1,1);pr.anchoredPosition=new Vector2(-105,-30);
            power=Label(content,"",40,111,700,35,19,gold);
            var bottom=Rect("Bottom hints",content);bottom.anchorMin=bottom.anchorMax=new Vector2(.5f,0);bottom.pivot=new Vector2(.5f,0);bottom.sizeDelta=new Vector2(1000,180);bottom.anchoredPosition=new Vector2(0,20);
            prompt=Label(bottom,"",180,0,700,42,24,gold);prompt.alignment=TextAnchor.MiddleCenter;
            hint=Label(bottom,"",0,48,1000,90,21);hint.alignment=TextAnchor.MiddleCenter;
            bool touch=Application.isMobilePlatform||PlayerPrefs.GetInt("yoko-touch",0)==1;
            if(touch)TouchControls();
            else {var t=Label(content,"A / D   MOVER       ESPACIO   SALTAR       SHIFT   OLFATO       E   EXPLORAR       J   ACCION",30,0,1300,28,15,new Color(.65f,.74f,.72f));var r=t.rectTransform;r.anchorMin=r.anchorMax=new Vector2(0,0);r.anchoredPosition=new Vector2(35,30);}
            veil=Panel(content,0,0,2000,1300,new Color(.7f,.04f,.14f,0));veil.raycastTarget=false;
        }
        void TouchControls()
        {
            var left=Rect("Move touch",content);left.anchorMin=left.anchorMax=new Vector2(0,0);left.pivot=Vector2.zero;left.anchoredPosition=new Vector2(30,22);left.sizeDelta=new Vector2(240,150);
            Touch(left,"<",0,0,v=>G.Player.TouchMove=v?-1:0);
            Touch(left,">",125,0,v=>G.Player.TouchMove=v?1:0);
            var right=Rect("Action touch",content);right.anchorMin=right.anchorMax=new Vector2(1,0);right.pivot=new Vector2(1,0);right.anchoredPosition=new Vector2(-25,22);right.sizeDelta=new Vector2(450,240);
            Touch(right,"OLFATO",0,0,v=>G.Player.TouchSniff=v);
            Touch(right,"EXPLORAR",115,0,v=>{if(v)G.Player.InteractPressed=true;});
            Touch(right,"ACCION",230,0,v=>{if(v)G.Player.AttackPressed=true;});
            Touch(right,"SALTO",340,0,v=>{G.Player.TouchJump=v;if(v)G.Player.JumpPressed=true;});
        }
        void Touch(Transform parent,string name,float x,float y,Action<bool> action)
        {
            var i=Panel(parent,x,y,105,100,new Color(.09f,.16f,.18f,.86f));i.gameObject.AddComponent<TouchAction>().Set=action;
            var t=Label(i.transform,name,0,32,105,40,17,gold);t.alignment=TextAnchor.MiddleCenter;
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
            hint.text=Time.unscaledTime<G.HintUntil?G.HintText:"";
            power.text=G.Session.Power==Mutation.None?"":(G.Session.Power==Mutation.Wing?"QUIMERA / AVE":"QUIMERA / FUERZA")+"     "+Mathf.CeilToInt(G.Session.PowerLeft)+" s";
            veil.color=new Color(.65f,.04f,.13f,G.Session.Frenzy?.12f:Time.time<G.Player.HurtUntil?.10f:0);
        }
    }
}