using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class ButtonInterface<T> : AbstractGameInterface where T : ButtonInterface<T>.ButtonEntry
{   
    [SerializeField] List<T> buttons;
    public List<T> Buttons => buttons;
    protected override void OnAwake() 
    {
        foreach(T buttonEntry in buttons)
        {
            AddEvent(buttonEntry.Button, EventTriggerType.PointerEnter, delegate { OnEnter(buttonEntry.Button); });
            AddEvent(buttonEntry.Button, EventTriggerType.PointerExit, delegate { OnExit(buttonEntry.Button); });
            AddEvent(buttonEntry.Button, EventTriggerType.PointerClick, (data) => { OnClick((PointerEventData)data, buttonEntry); });
        }
    }

    protected override void Initialize() {}
    
    protected override void UpdateInterface() {}

    protected virtual void OnEnter(GameObject obj) {}

    protected virtual void OnExit(GameObject obj) {} 

    protected virtual void OnClick(PointerEventData data, ButtonEntry buttonEntry)
    {
        if(data.button == PointerEventData.InputButton.Left)
        {  
            OnSelectButton(buttonEntry);
        }
    }

    public virtual void OnSelectButton(ButtonEntry buttonEntry)
    {
        buttonEntry.Function.Invoke();
    }
    
    [System.Serializable]
    public abstract class ButtonEntry
    {
        [SerializeField] GameObject button;
        public GameObject Button => button;
        [SerializeField] ButtonEntryEvent function = new ButtonEntryEvent();
        public ButtonEntryEvent Function => function;
        
        [System.Serializable]
        public class ButtonEntryEvent : UnityEvent{}
    }
}


