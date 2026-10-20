using System.Collections.Generic;
using UnityEngine;
namespace Yoko
{
    public enum ActorKind { Serum, Remains, Prey, Note, Exit, Yimmy, Node, Enemy, Hostile, Boss, Clue }
    public class WorldFactory
    {
        public Transform Root;
        public readonly List<WorldActor> Actors=new List<WorldActor>();
        readonly List<SpriteRenderer> scent=new List<SpriteRenderer>();
        readonly List<SpriteRenderer> spores=new List<SpriteRenderer>();
        Transform backdrop;
        public float Length=58;
        public int Zone;
        readonly Color ink=new Color(.08f,.11f,.13f),rose=new Color(.83f,.24f,.40f),gold=new Color(.92f,.76f,.34f);
        public void Build(int zone)
        {
            if(Root!=null){Root.gameObject.SetActive(false);Object.Destroy(Root.gameObject);}
            Actors.Clear();scent.Clear();spores.Clear();Zone=zone;
            Root=new GameObject("Zone "+zone).transform;
            Length=zone==4?64:58;
            backdrop=ArtLibrary.Draw("Bogota panorama",ArtLibrary.Background,new Vector2(14,-3),new Vector2(42,21),zone==0?new Color(.38f,.48f,.52f):zone>=3?new Color(.64f,.45f,.53f):Color.white,-100,Root).transform;
            Platform(Length/2,-.75f,Length+4,1.5f);
            // Traverseable platforms with a lower exploration route and upper short route.
            Platform(11,1.1f,5,.45f);Platform(19,2.3f,4,.45f);
            Platform(28,1.3f,5,.45f);Platform(39,2.2f,5,.45f);Platform(49,1.1f,4,.45f);
            if(zone==0)
            {
                for(int i=0;i<6;i++)
                {
                    ArtLibrary.Box("Bunker pillar",new Vector2(i*12,4),new Vector2(.65f,8),new Color(.12f,.18f,.20f),-25,Root);
                    ArtLibrary.Box("Laboratory light",new Vector2(i*12+5,6.4f),new Vector2(2,.1f),new Color(.55f,.83f,.75f),-20,Root);
                }
                Sign("OPHIOCORDYCEPS-X",new Vector2(8,4.5f));
                Add(ActorKind.Serum,5,.2f);
                Add(ActorKind.Remains,9,.2f,Mutation.Wing);
                Add(ActorKind.Note,16,.25f,Mutation.None,0);
                // A tall block requires the first bird mutation: jump and glide from the upper platform.
                Platform(24,1.6f,1.2f,3.2f);
            }
            else
            {
                Sign(zone==1?"SUR / BOGOTA":zone==2?"ZONA DE IMPACTO":zone==3?"EVACUACION":"OPHIO-X / NUCLEO",new Vector2(8,4));
                Add(ActorKind.Remains,7,.2f,Mutation.Wing);
                Add(ActorKind.Prey,13,1.35f,Mutation.Wing);
                Add(ActorKind.Remains,24,.2f,Mutation.Claw);
                Add(ActorKind.Prey,34,1.55f,Mutation.Claw);
                Add(ActorKind.Note,20,2.55f,Mutation.None,zone);
                Add(ActorKind.Enemy,18,.05f);
                Add(ActorKind.Hostile,31,.05f);
                Add(zone>=3?ActorKind.Boss:ActorKind.Enemy,44,.05f);
                if(zone==2)Add(ActorKind.Yimmy,51,.05f);
                else Add(ActorKind.Clue,51,.15f);
                if(zone>=3)
                {
                    Add(ActorKind.Node,36,.1f);
                    if(zone==4){Add(ActorKind.Node,48,.1f);Add(ActorKind.Node,57,.1f);Add(ActorKind.Remains,53,.15f,Mutation.Claw);Add(ActorKind.Yimmy,61,.05f);}
                }
            }
            if(zone<4)Add(ActorKind.Exit,55,.1f);
            for(int i=0;i<70;i++)
            {
                float x=i*(Length/70f);
                scent.Add(ArtLibrary.Box("Olfactory trail",new Vector2(x,.25f+Mathf.Sin(x*.5f)*.1f),new Vector2(.15f,.06f),gold,5,Root));
            }
            Random.InitState(zone+524);
            for(int i=0;i<75;i++)
            {
                float x=Random.Range(0,Length),y=Random.Range(.2f,8);
                var r=ArtLibrary.Box("Spore",new Vector2(x,y),Vector2.one*Random.Range(.025f,.07f),new Color(.93f,.67f,.64f,.45f),-2,Root);spores.Add(r);
            }
            for(int i=0;i<24;i++)
            {
                float x=i*2.6f;
                var r=ArtLibrary.Box("Rubble",new Vector2(x,-.08f),new Vector2(Random.Range(.4f,1.4f),Random.Range(.1f,.3f)),new Color(.23f,.26f,.27f),2,Root);r.transform.Rotate(0,0,Random.Range(-12f,12f));
                if(zone>=2)
                {
                    var vein=ArtLibrary.Box("Mycelium",new Vector2(x,.1f),new Vector2(.09f,Random.Range(.5f,1.6f)),rose,-1,Root);vein.transform.Rotate(0,0,-40);
                }
            }
        }
        public void Presentation(){foreach(Transform t in Root)t.gameObject.SetActive(t==backdrop);}
        public void Platform(float x,float y,float w,float h)
        {
            var r=ArtLibrary.Box("Solid concrete",new Vector2(x,y),new Vector2(w,h),ink,0,Root);
            r.gameObject.layer=8;r.gameObject.AddComponent<BoxCollider2D>();
            ArtLibrary.Box("Concrete edge",new Vector2(x,y+h/2),new Vector2(w,.07f),new Color(.42f,.48f,.47f),1,Root);
            if(w<10)ArtLibrary.Draw("Broken concrete",ArtLibrary.Prop(7),new Vector2(x,y-h/2-.55f),new Vector2(w+.4f,h+1f),Color.white,3,Root);
            for(float a=x-w/2+.2f;a<x+w/2;a+=1.4f)ArtLibrary.Box("Concrete seam",new Vector2(a,y),new Vector2(.015f,h*.8f),new Color(.18f,.22f,.24f),1,Root);
        }
        void Sign(string text,Vector2 p)
        {
            ArtLibrary.Box("Sign",p,new Vector2(5,.8f),new Color(.12f,.19f,.19f),-5,Root);
            var go=new GameObject("Diegetic sign");go.transform.SetParent(Root);go.transform.position=new Vector3(p.x,p.y,-.1f);
            var t=go.AddComponent<TextMesh>();t.text=text;t.fontSize=42;t.characterSize=.08f;t.anchor=TextAnchor.MiddleCenter;t.color=new Color(.76f,.79f,.69f);go.GetComponent<MeshRenderer>().sortingOrder=-4;
        }
        public WorldActor Add(ActorKind kind,float x,float y,Mutation power=Mutation.None,int note=0)
        {
            var go=new GameObject(kind.ToString());go.transform.SetParent(Root);go.transform.position=new Vector3(x,y,0);
            var a=go.AddComponent<WorldActor>();a.Kind=kind;a.Power=power;a.NoteId=note;a.Home=x;a.Setup();Actors.Add(a);return a;
        }
        public void Tick(float dt)
        {
            var g=YokoGame.Instance;
            backdrop.position=new Vector3(9+g.Cam.transform.position.x*.65f,-3,0);
            for(int i=0;i<scent.Count;i++)scent[i].enabled=g.Player.Sniffing;
            for(int i=0;i<spores.Count;i++){var t=spores[i].transform;t.position+=new Vector3(Mathf.Sin(Time.time*.25f+i)*dt*.12f,dt*.1f,0);if(t.position.y>9)t.position=new Vector3(t.position.x,0,0);}
        }
        public int RemainingNodes {get {int n=0;foreach(var a in Actors)if(a.Kind==ActorKind.Node&&!a.Used)n++;return n;}}
    }
}