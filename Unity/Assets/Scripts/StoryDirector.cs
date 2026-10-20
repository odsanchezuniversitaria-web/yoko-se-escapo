using UnityEngine;
namespace Yoko {
public class StoryDirector : MonoBehaviour {
 static readonly float[] Triggers={2.3f,23,46};int beat,zone;float lastBeat;public string Caption="";public float CaptionUntil;
 YokoGame G=>YokoGame.Instance;
 static readonly string[][] Lines={
 new[]{"El aire huele a metal. Entre el polvo, el collar aún brilla.","Una rueda gira en algún lugar. Ese sonido pertenecía a casa.","El ave no volverá a volar. Su memoria puede ayudar a Yoko a salir."},
 new[]{"La ciudad culpa a los animales. En las paredes: «No los dejen entrar».","Un Slay escucha tus pasos. Salta por arriba o esquiva cuando se lance.","La patineta de Yimmy. Una rueda sigue tibia. Estuvo aquí."},
 new[]{"Entre las esporas hay otro olor. Miedo… y la ropa de Yimmy.","«¿Yoko? Si estás ahí… no te acerques a las raíces».","Su voz se rompe. Los filamentos se tensan hacia el epicentro."},
 new[]{"Las raíces laten al mismo tiempo. El suelo responde a cada golpe.","La red se protege. Espera el ataque del guardián y esquiva.","Cada nudo roto devuelve un instante de silencio a la ciudad."},
 new[]{"Yimmy apenas respira. Tres nudos mantienen cerrada la red.","«Reconocería ese collar en cualquier parte. Ven, pequeña».","El último paso no exige ser más fuerte. Exige seguir siendo Yoko."}
 };
 public void ResetZone(int z){zone=z;beat=0;lastBeat=-10;Caption="";CaptionUntil=0;}
 public void Say(string text,float seconds=5){Caption=text;CaptionUntil=Time.time+seconds;lastBeat=Time.time;G.Sound.Play(SoundKind.Memory,.5f);}
 public void Tick(){float x=G.Player.transform.position.x;if(beat<3&&x>=Triggers[beat]&&Time.time-lastBeat>6&&Time.time>CaptionUntil){Say(Lines[zone][beat]);beat++;lastBeat=Time.time;}}
 public string Objective {
 get {
  var d=G.Session.Data;
  if(d.zone==0)return !d.serum?"EXAMINA EL SUERO  ·  busca el destello":G.Session.Power==Mutation.None?"ASIMILA EL AVE  ·  E / Explorar":"SAL DE LAS RUINAS  ·  salta y mantén para planear";
  if(d.zone==1)return "SIGUE EL OLOR DE YIMMY  ·  usa Olfato";
  if(d.zone==2)return !d.foundYimmy?"ENCUENTRA A YIMMY  ·  sigue la voz":"BUSCA EL ORIGEN DE LAS RAÍCES  →";
  if(G.World.RemainingNodes>0)return "ROMPE LA RED  ·  "+G.World.RemainingNodes+" nudo(s)  ·  necesita Fuerza";
  return d.zone==4?"VUELVE JUNTO A YIMMY  ·  E / Explorar":"EL CAMINO ESTÁ ABIERTO  →";
 }}
}
}