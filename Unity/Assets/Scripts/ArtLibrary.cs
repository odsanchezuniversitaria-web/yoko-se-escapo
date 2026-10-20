using System;
using System.Collections.Generic;
using UnityEngine;
namespace Yoko
{
    public static class ArtLibrary
    {
        static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();
        public static Sprite Cell(string name,int index,int cols,int rows)
        {
            string key = name+index;
            if(cache.TryGetValue(key,out Sprite s)) return s;
            Texture2D t = Resources.Load<Texture2D>("Art/"+name);
            if(t == null) throw new InvalidOperationException("Falta Art/"+name);
            int w = t.width/cols, h = t.height/rows;
            Rect rect=new Rect((index%cols)*w,t.height-(index/cols+1)*h,w,h);
            if(name=="Cast") {
                // Artist's atlas is irregular; source-space rectangles avoid neighboring figures.
                Rect[] cells={new Rect(0,0,278,608),new Rect(279,240,382,369),new Rect(663,0,281,608),new Rect(945,439,309,167),new Rect(0,610,277,644),new Rect(279,850,377,404),new Rect(588,612,390,625),new Rect(975,708,279,527)};
                Rect c=cells[index];float sx=t.width/1254f,sy=t.height/1254f;
                rect=new Rect(c.x*sx,t.height-(c.y+c.height)*sy,c.width*sx,c.height*sy);
            }
            s = Sprite.Create(t,rect,new Vector2(.5f,0),100);
            cache[key]=s; return s;
        }
        public static Sprite Yoko(int index) => Cell("Yoko",index,3,2);
        public static Sprite Cast(int index) => Cell("Cast",index,4,2);
        public static Sprite Prop(int index) => Cell("Props",index,4,2);
        public static Sprite Background => Cell("Bogota",0,1,1);
        public static Sprite Square
        {
            get {
                if(cache.TryGetValue("square",out Sprite s)) return s;
                Texture2D t=new Texture2D(2,2); t.SetPixels(new[]{Color.white,Color.white,Color.white,Color.white});t.Apply();
                s=Sprite.Create(t,new Rect(0,0,2,2),new Vector2(.5f,.5f),2); cache["square"]=s;return s;
            }
        }
        public static SpriteRenderer Draw(string name, Sprite sprite, Vector2 pos, Vector2 size, Color color, int order, Transform parent)
        {
            var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.position=pos;
            var r=go.AddComponent<SpriteRenderer>();r.sprite=sprite;r.color=color;r.sortingOrder=order;
            go.transform.localScale=new Vector3(size.x/sprite.bounds.size.x,size.y/sprite.bounds.size.y,1); return r;
        }
        public static SpriteRenderer Box(string name,Vector2 pos,Vector2 size,Color color,int order,Transform parent) => Draw(name,Square,pos,size,color,order,parent);
    }
}