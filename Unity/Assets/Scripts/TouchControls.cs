using System;using UnityEngine;using UnityEngine.UI;using UnityEngine.EventSystems;
namespace Yoko {
public class AnalogStick : MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler {
 public RectTransform Knob;public Action<float> Changed;public int Owner {get;private set;}=int.MinValue;public float Value {get;private set;}
 RectTransform rect;const float Radius=65;
 void Awake(){rect=(RectTransform)transform;}
 public void OnPointerDown(PointerEventData e){if(Owner!=int.MinValue)return;Owner=e.pointerId;OnDrag(e);}
 public void OnDrag(PointerEventData e){if(e.pointerId!=Owner)return;if(RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,e.position,e.pressEventCamera,out Vector2 p)){p=Vector2.ClampMagnitude(p,Radius);Knob.anchoredPosition=p;float x=p.x/Radius;Value=Mathf.Abs(x)<.12f?0:Mathf.Sign(x)*(Mathf.Abs(x)-.12f)/.88f;Changed?.Invoke(Value);}}
 public void OnPointerUp(PointerEventData e){if(e.pointerId==Owner)ResetStick();}
 public void ResetStick(){Owner=int.MinValue;Value=0;if(Knob)Knob.anchoredPosition=Vector2.zero;Changed?.Invoke(0);}
 void OnDisable(){ResetStick();}void OnApplicationFocus(bool focus){if(!focus)ResetStick();}
}
public class ActionPad : MonoBehaviour,IPointerDownHandler,IPointerUpHandler {
 public Action<bool> Changed;public Func<float> Cooldown;public Image Fill;public int Owner {get;private set;}=int.MinValue;
 public bool Pressed=>Owner!=int.MinValue;Vector3 scale=Vector3.one;Image background;Color baseColor;bool active;
 void Awake(){background=GetComponent<Image>();baseColor=background.color;}
 public void OnPointerDown(PointerEventData e){if(Pressed)return;Owner=e.pointerId;Changed?.Invoke(true);}
 public void OnPointerUp(PointerEventData e){if(e.pointerId!=Owner)return;Release();}
 public void Release(){Owner=int.MinValue;Changed?.Invoke(false);}
 void Update(){float t=Pressed?.91f:1;transform.localScale=Vector3.Lerp(transform.localScale,Vector3.one*t,Time.unscaledDeltaTime*22);background.color=Pressed?Color.Lerp(baseColor,Color.white,.35f):baseColor;if(Fill&&Cooldown!=null)Fill.fillAmount=Cooldown();}
 void OnDisable(){Release();}void OnApplicationFocus(bool focus){if(!focus)Release();}
}
public class ButtonMotion : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerEnterHandler,IPointerExitHandler {
 bool down,over;
 public void OnPointerDown(PointerEventData e){down=true;}public void OnPointerUp(PointerEventData e){down=false;}public void OnPointerEnter(PointerEventData e){over=true;}public void OnPointerExit(PointerEventData e){over=down=false;}
 void Update(){transform.localScale=Vector3.Lerp(transform.localScale,Vector3.one*(down?.97f:over?1.018f:1),Time.unscaledDeltaTime*18);}
}
}