using UnityEngine;
namespace Yoko
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public class DebugManager : MonoBehaviour
    {
        public string mcpCommand="";
        void Update(){if(string.IsNullOrEmpty(mcpCommand))return;string c=mcpCommand;mcpCommand="";Execute(c);}
        public void Execute(string command)
        {
            var g=YokoGame.Instance;
            switch(command)
            {
                case "start":g.TestMode=true;g.BeginCampaign();break;
                case "god_on":g.GodMode=true;break;
                case "god_off":g.GodMode=false;break;
                case "die":g.GodMode=false;g.Die();break;
                case "retry":g.Retry();break;
                case "title":g.Title();break;
                case "pause":g.ChangeState(ScreenState.Pause);break;
                case "settings":g.ChangeState(ScreenState.Settings);break;
                case "resume":g.ChangeState(ScreenState.Playing);break;
            }
        }
    }
#endif
}