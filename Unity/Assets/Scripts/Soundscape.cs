using UnityEngine;
namespace Yoko
{
    public class Soundscape : MonoBehaviour
    {
        AudioSource ambience,effects;
        public float Threat;
        public void Setup()
        {
            ambience=gameObject.AddComponent<AudioSource>();effects=gameObject.AddComponent<AudioSource>();
            ambience.loop=true;ambience.volume=.13f;
            int rate=22050,n=rate*12;float[] samples=new float[n];var rng=new System.Random(91);float noise=0;
            for(int i=0;i<n;i++){float t=i/(float)rate;noise=noise*.97f+((float)rng.NextDouble()*2-1)*.03f;samples[i]=(Mathf.Sin(t*2*Mathf.PI*55)*.23f+Mathf.Sin(t*2*Mathf.PI*82.5f)*.13f+noise*.6f)*(.6f+.4f*Mathf.Sin(Mathf.PI*t/12));}
            var c=AudioClip.Create("Bogota - viento y red",n,1,rate,false);c.SetData(samples,0);ambience.clip=c;ambience.Play();
            AudioListener.volume=PlayerPrefs.GetFloat("yoko-volume",.7f);
        }
        void Update(){ambience.pitch=Mathf.Lerp(.85f,1.15f,Threat);ambience.volume=.13f+Threat*.07f;}
        public void Cue(float hz,float duration)
        {
            int rate=22050,n=Mathf.Max(1,(int)(duration*rate));float[] data=new float[n];
            for(int i=0;i<n;i++){float t=i/(float)rate;data[i]=Mathf.Sin(t*hz*2*Mathf.PI)*Mathf.Sin(Mathf.PI*i/n)*.18f;}
            var c=AudioClip.Create("Cue",n,1,rate,false);c.SetData(data,0);effects.PlayOneShot(c);Destroy(c,duration+1);
        }
    }
}