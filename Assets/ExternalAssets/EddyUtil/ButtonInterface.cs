using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using NaughtyAttributes;

public abstract class ButtonInterface<T> : AbstractGameInterface where T : ButtonInterface<T>.ButtonEntry
{   
    [SerializeField] protected List<T> buttons;
    public List<T> Buttons => buttons;
    protected override void OnAwake() 
    {
        foreach(T buttonEntry in buttons)
        {
            AddEvent(buttonEntry.Button, EventTriggerType.PointerEnter, delegate { OnEnterButton(buttonEntry); });
            AddEvent(buttonEntry.Button, EventTriggerType.PointerExit, delegate { OnExitButton(buttonEntry); });
            AddEvent(buttonEntry.Button, EventTriggerType.PointerClick, (data) => { OnClick((PointerEventData)data, buttonEntry); });
        }
    }

    protected override void Initialize() {}
    
    protected override void UpdateInterface() {}

    private void OnEnterButton(ButtonEntry buttonEntry)
    {
        buttonEntry.SetIsHovered(true);
        OnEnter(buttonEntry);
    }

    private void OnExitButton(ButtonEntry buttonEntry)
    {
        buttonEntry.SetIsHovered(false);
        OnExit(buttonEntry);
    }
    protected virtual void OnEnter(ButtonEntry buttonEntry) {}

    protected virtual void OnExit(ButtonEntry buttonEntry) {} 

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
        [SerializeField] protected GameObject button;
        public GameObject Button => button;
        [SerializeField] ButtonEntryEvent function = new ButtonEntryEvent();
        public ButtonEntryEvent Function => function;
        [SerializeField, ReadOnly] bool isHovered;
        public bool IsHovered => isHovered;
        [SerializeField, ReadOnly] bool isSelected;
        public bool IsSelected => isSelected;

        public void SetIsHovered(bool b)
        {
            isHovered = b;
        }

        public virtual void SetIsSelected(bool b)
        {
            isSelected = b;
        }
        
        [System.Serializable]
        public class ButtonEntryEvent : UnityEvent {}
    }
}


