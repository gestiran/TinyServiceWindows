// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using TinyReactive;
using UnityEngine;

#if TINY_MVC
using TinyMVC.Dependencies;
using TinyMVC.Dependencies.Extensions;
using TinyMVC.Loop;
using TinyMVC.Views;
#endif

namespace TinyServices.Windows {
    [DisallowMultipleComponent]
    public abstract class WindowContext : MonoBehaviour {
        internal List<WindowComponentBehaviour> connections;
        
        private void Awake() { }
        
        private void Start() { }
        
        private void OnEnable() { }
        
        private void OnDisable() { }
        
        protected IEnumerable<WindowComponentBehaviour> ForEachConnections() => connections;
        
        [Obsolete("Can`t use without parameters", true)]
        protected void Connect() { }
        
        protected void Connect(params WindowComponentBehaviour[] components) {
            foreach (WindowComponentBehaviour component in components) {
                Connect<WindowComponentBehaviour>(component);
            }
        }
        
    #if TINY_MVC
        
        protected void Connect(WindowComponentBehaviour[] components, params IDependency[] dependencies) {
            foreach (WindowComponentBehaviour component in components) {
                Connect(component, dependencies);
            }
        }
        
        protected T Connect<T>(T component, params IDependency[] dependencies) where T : WindowComponentBehaviour {
            if (component.connectState == WindowComponentBehaviour.ConnectState.Connected) {
                return component;
            }
            
            component.Initialize();
            component.root = this;
            component.connectState = WindowComponentBehaviour.ConnectState.Connected;
            connections.Add(component);
            
            if (component is IInit init) {
                init.Init();
            }
            
            if (component is IApplyResolving applyResolving) {
                applyResolving.ApplyResolving(dependencies);
            }
            
            if (component is IBeginPlay beginPlay) {
                beginPlay.BeginPlay();
            }
            
            return component;
        }
        
    #endif
        
        protected T Connect<T>(T component) where T : WindowComponentBehaviour {
            if (component.connectState == WindowComponentBehaviour.ConnectState.Connected) {
                return component;
            }
            
            component.Initialize();
            component.root = this;
            component.connectState = WindowComponentBehaviour.ConnectState.Connected;
            connections.Add(component);
            
        #if TINY_MVC
            
            if (component is IInit init) {
                init.Init();
            }
            
            if (component is IApplyResolving applyResolving) {
                applyResolving.ApplyResolving();
            }
            
            if (component is IBeginPlay beginPlay) {
                beginPlay.BeginPlay();
            }
            
        #endif
            
            return component;
        }
        
        internal bool DisconnectSelf(WindowComponentBehaviour component) => component.root.Disconnect(component);
        
        protected void Disconnect(params WindowComponentBehaviour[] components) {
            foreach (WindowComponentBehaviour component in components) {
                Disconnect(component);
            }
        }
        
        protected void DisconnectAll() {
            foreach (WindowComponentBehaviour connect in connections) {
                if (connect.connectState != WindowComponentBehaviour.ConnectState.Connected) {
                    continue;
                }
                
                if (connect is IUnload unload) {
                    unload.Unload();
                }
                
                connect.root = null;
                connect.connectState = WindowComponentBehaviour.ConnectState.Disconnected;
                connect.DisconnectAll();
            }
            
            connections.Clear();
        }
        
        protected bool Disconnect(WindowComponentBehaviour component) {
            if (component.connectState == WindowComponentBehaviour.ConnectState.Disconnected) {
                return false;
            }
            
            int index = connections.IndexOf(component);
            
            if (index < 0) {
                return false;
            }
            
            if (connections[index] is IUnload unload) {
                unload.Unload();
            }
            
            connections[index].root = null;
            connections[index].connectState = WindowComponentBehaviour.ConnectState.Disconnected;
            connections[index].DisconnectAll();
            connections.RemoveAt(index);
            return true;
        }
        
    #if TINY_MVC
        
        public void UpdateConnections<T>() where T : WindowComponentBehaviour {
            foreach (WindowComponentBehaviour connection in connections) {
                if (connection is not T) {
                    continue;
                }
                
                if (connection is IUpdateConnection update) {
                    update.UpdateConnection();
                }
            }
        }
        
        public void UpdateConnections() {
            foreach (WindowComponentBehaviour connection in connections) {
                if (connection is IUpdateConnection update) {
                    update.UpdateConnection();
                }
            }
        }
        
    #endif
        
        [Obsolete("Can`t use without parameters", true)]
        public void Reconnect() { }
        
        public void Reconnect(params WindowComponentBehaviour[] components) {
            foreach (WindowComponentBehaviour component in components) {
                if (component.connectState == WindowComponentBehaviour.ConnectState.Connected) {
                    Disconnect(component);
                }
                
                Connect(component);
            }
        }
        
    #if TINY_MVC
        
        [Obsolete("Can`t use without parameters", true)]
        public T Reconnect<T>(T component) where T : WindowComponentBehaviour, IApplyResolving => component;
        
        public T Reconnect<T>(T component, params IDependency[] dependencies) where T : WindowComponentBehaviour, IApplyResolving {
            if (component.connectState == WindowComponentBehaviour.ConnectState.Connected) {
                Disconnect(component);
            }
            
            return Connect(component, dependencies);
        }
        
    #endif
        
        internal abstract void Initialize();
        
    #if UNITY_EDITOR
        
        public virtual void Reset() => UnityEditor.EditorUtility.SetDirty(this);
        
    #endif
    }
}