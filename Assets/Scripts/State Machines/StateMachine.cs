using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System.Linq;
using System;

public abstract class StateMachine<T> : MonoBehaviour where T : StateMachine<T>.State
{
    [Header("State Machine")]
    [SerializeField] bool statesAreChildGameObjects = true;
    [SerializeField, HideIf("statesAreChildGameObjects")] protected List<T> states;
    [SerializeField] protected T startingState;

    [Space(5)]

    [ReadOnly, SerializeField] protected T currentState;
    public T CurrentState => currentState;

    bool initialized = false;
    protected bool Initialized => initialized;

    public Action OnStateMachineInitialized;

    private void Awake()
    {
        Initialize();

        OnAwake();
        states.ForEach(x => x.OnAwake());
    }

    private void Initialize()
    {
        if(initialized) return;

        if(statesAreChildGameObjects)
        {
            states = new List<T>(GetComponentsInChildren<T>()); 
        }

        initialized = true;
        OnStateMachineInitialized?.Invoke();
    }

    private void Start()
    {
        if(!initialized) return;

        OnStart();
        states.ForEach(x => x.OnStart());
    }

    private void Update()
    {
        if(!initialized) return;

        OnUpdate();
        states.ForEach(x => x.OnUpdate());
        currentState?.OnUpdateState();
    }

    private void FixedUpdate()
    {
        if(!initialized) return;

        OnFixedUpdate();
        states.ForEach(x => x.OnFixedUpdate());
        currentState?.OnFixedUpdateState();
    }

    protected virtual void OnAwake() {}
    protected virtual void OnStart() {}
    protected virtual void OnUpdate() {}
    protected virtual void OnFixedUpdate() {}

    public bool GetState<F>(out F t) where F : T
    {
        if(!initialized) 
        {
            Debug.LogError(name + " is not initialized! Can't retrieve state!");
            t = null;
            return false;
        }

        t = states.FirstOrDefault(x => x.GetType() == typeof(F)) as F;
        return t != null;
    }

    public abstract class State : MonoBehaviour
    {
        public virtual void OnAwake() {}
        public virtual void OnStart() {}
        public virtual void OnEnterState(T fromState) {}
        public virtual void OnExitState(T toState) {}
        public virtual void OnUpdate() {}
        public virtual void OnUpdateState() {}
        public virtual void OnFixedUpdate() {}
        public virtual void OnFixedUpdateState() {}

        public Action<T> OnEnterStateEvent;
        public Action<T> OnExitStateEvent;
    }
}
