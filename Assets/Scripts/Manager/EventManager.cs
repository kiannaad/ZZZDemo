using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : Singleton<EventManager>
{
    private TypeEventSystem mTypeEventSystem = new TypeEventSystem();

    public void SendEvent<TEvent>() where TEvent : new() => mTypeEventSystem.Send<TEvent>();

    public void SendEvent<TEvent>(TEvent e) => mTypeEventSystem.Send<TEvent>(e);

    public IUnRegister RegisterEvent<TEvent>(Action<TEvent> onEvent) => mTypeEventSystem.Register<TEvent>(onEvent);

    public void UnRegisterEvent<TEvent>(Action<TEvent> onEvent) => mTypeEventSystem.UnRegister<TEvent>(onEvent);
    
    #region UnRegister

    public interface IUnRegister
    {
        void UnRegister();
    }

    public interface IUnRegisterList
    {
        List<IUnRegister> UnregisterList { get; }
    }

    public struct CustomUnRegister : IUnRegister
    {
        private Action mOnUnRegister { get; set; }
        public CustomUnRegister(Action onUnRegister) => mOnUnRegister = onUnRegister;

        public void UnRegister()
        {
            mOnUnRegister.Invoke();
            mOnUnRegister = null;
        }
    }

#if UNITY_5_6_OR_NEWER
    public abstract class UnRegisterTrigger : UnityEngine.MonoBehaviour
    {
        private readonly HashSet<IUnRegister> mUnRegisters = new HashSet<IUnRegister>();

        public IUnRegister AddUnRegister(IUnRegister unRegister)
        {
            mUnRegisters.Add(unRegister);
            return unRegister;
        }

        public void RemoveUnRegister(IUnRegister unRegister) => mUnRegisters.Remove(unRegister);

        public void UnRegister()
        {
            foreach (var unRegister in mUnRegisters)
            {
                unRegister.UnRegister();
            }

            mUnRegisters.Clear();
        }
    }

    public class UnRegisterOnDestroyTrigger : UnRegisterTrigger
    {
        private void OnDestroy()
        {
            UnRegister();
        }
    }

    public class UnRegisterOnDisableTrigger : UnRegisterTrigger
    {
        private void OnDisable()
        {
            UnRegister();
        }
    }
    
    public class UnRegisterOnDisableAndDestroyTrigger : UnRegisterTrigger
    {
        private void OnDisable()
        {
            UnRegister();
        }
        private void OnDestroy()
        {
            UnRegister();
        }
    }
    

    #endif
    public class UnRegisterCurrentSceneUnloadedTrigger : UnRegisterTrigger
    {
        private static UnRegisterCurrentSceneUnloadedTrigger mDefault;

        public static UnRegisterCurrentSceneUnloadedTrigger Get
        {
            get
            {
                if (!mDefault)
                {
                    mDefault = new GameObject("UnRegisterCurrentSceneUnloadedTrigger")
                        .AddComponent<UnRegisterCurrentSceneUnloadedTrigger>();
                }

                return mDefault;
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            hideFlags = HideFlags.HideInHierarchy;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDestroy() => SceneManager.sceneUnloaded -= OnSceneUnloaded;
        void OnSceneUnloaded(Scene scene) => UnRegister();
    }
    #endregion
}

 public interface IEasyEvent
    {
        EventManager.IUnRegister Register(Action onEvent); 
    }

    public class EasyEvent : IEasyEvent
    {
        private Action mOnEvent = () => { };

        public EventManager.IUnRegister Register(Action onEvent)
        {
            mOnEvent += onEvent;
            return new EventManager.CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public EventManager.IUnRegister RegisterWithACall(Action onEvent)
        {
            onEvent.Invoke();
            return Register(onEvent);
        }

        public void UnRegister(Action onEvent) => mOnEvent -= onEvent;

        public void Trigger() => mOnEvent?.Invoke();
    }

    public class EasyEvent<T> : IEasyEvent
    {
        private Action<T> mOnEvent = e => { };

        public EventManager.IUnRegister Register(Action<T> onEvent)
        {
            mOnEvent += onEvent;
            return new EventManager.CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action<T> onEvent) => mOnEvent -= onEvent;
        

        public void Trigger(T t) => mOnEvent?.Invoke(t);

        EventManager.IUnRegister IEasyEvent.Register(Action onEvent)
        {
            return Register(Action);
            void Action(T _) => onEvent();
        }
    }

    public class EasyEvent<T, K> : IEasyEvent
    {
        private Action<T, K> mOnEvent = (t, k) => { };

        public EventManager.IUnRegister Register(Action<T, K> onEvent)
        {
            mOnEvent += onEvent;
            return new EventManager.CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action<T, K> onEvent) => mOnEvent -= onEvent;

        public void Trigger(T t, K k) => mOnEvent?.Invoke(t, k);

        EventManager.IUnRegister IEasyEvent.Register(Action onEvent)
        {
            return Register(Action);
            void Action(T _, K __) => onEvent();
        }
    }

    public class EasyEvent<T, K, S> : IEasyEvent
    {
        private Action<T, K, S> mOnEvent = (t, k, s) => { };

        public EventManager.IUnRegister Register(Action<T, K, S> onEvent)
        {
            mOnEvent += onEvent;
            return new EventManager.CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action<T, K, S> onEvent) => mOnEvent -= onEvent;

        public void Trigger(T t, K k, S s) => mOnEvent?.Invoke(t, k, s);

        EventManager.IUnRegister IEasyEvent.Register(Action onEvent)
        {
            return Register(Action);
            void Action(T _, K __, S ___) => onEvent();
        }
    }

    public class EasyEvents
    {
        private static readonly EasyEvents mGlobalEvents = new EasyEvents();

        public static T Get<T>() where T : IEasyEvent => mGlobalEvents.GetEvent<T>();

        public static void Register<T>() where T : IEasyEvent, new() => mGlobalEvents.AddEvent<T>();

        private readonly Dictionary<Type, IEasyEvent> mTypeEvents = new Dictionary<Type, IEasyEvent>();

        public void AddEvent<T>() where T : IEasyEvent, new() => mTypeEvents.Add(typeof(T), new T());

        public T GetEvent<T>() where T : IEasyEvent
        {
            return mTypeEvents.TryGetValue(typeof(T), out var e) ? (T)e : default;
        }

        public T GetOrAddEvent<T>() where T : IEasyEvent, new()
        {
            var eType = typeof(T);
            if (mTypeEvents.TryGetValue(eType, out var e))
            {
                return (T)e;
            }

            var t = new T();
            mTypeEvents.Add(eType, t);
            return t;
        }
        
    }
    public class TypeEventSystem
    {
        private readonly EasyEvents mEvents = new EasyEvents();

        public static readonly TypeEventSystem Global = new TypeEventSystem();

        public void Send<T>() where T : new() => mEvents.GetEvent<EasyEvent<T>>()?.Trigger(new T());

        public void Send<T>(T e) => mEvents.GetEvent<EasyEvent<T>>()?.Trigger(e);

        public EventManager.IUnRegister Register<T>(Action<T> onEvent) => mEvents.GetOrAddEvent<EasyEvent<T>>().Register(onEvent);

        public void UnRegister<T>(Action<T> onEvent)
        {
            var e = mEvents.GetEvent<EasyEvent<T>>();
            e?.UnRegister(onEvent);
        }
    }