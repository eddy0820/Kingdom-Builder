using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System.Linq;

public abstract class StateMachine<T> : MonoBehaviour where T : StateMachine<T>.State
{
    [Header("State Machine")]
    [SerializeField] bool statesAreChildGameObjects = true;
    [SerializeField, HideIf("statesAreChildGameObjects")] List<T> states;
    [SerializeField] T startingState;

    [Space(5)]

    [ReadOnly, SerializeField] protected T currentState;
    public T CurrentState => currentState;

    bool initialized = false;
    protected bool Initialized => initialized;

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

        states.ForEach(x => x.Initialize(this));

        initialized = true;
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
    }

    protected abstract void OnAwake();
    protected abstract void OnStart();
    protected abstract void OnUpdate();

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
        protected StateMachine<T> stateMachine;

        public virtual void Initialize(StateMachine<T> stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public abstract void OnAwake();
        public abstract void OnStart();
        public abstract void OnEnterState(T fromState);
        public abstract void OnExitState(T toState);
        public abstract void OnUpdate();
    }
}