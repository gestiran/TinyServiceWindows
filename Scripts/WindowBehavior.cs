// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using UnityEngine;
using TinyReactive;

#if TINY_MVC
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Dependencies.Extensions;
#endif

namespace TinyServices.Windows {
    [DisallowMultipleComponent]
    public abstract class WindowBehavior : MonoBehaviour {
        public bool isVisible { get; private set; }
        
        private List<WindowComponentBehaviour> _components;
        
        public virtual void Show() => gameObject.SetActive(true);
        
        public virtual void Hide() => gameObject.SetActive(false);
        
        [Obsolete("Can`t use without parameters", true)]
        protected void Connect() {
            // Do nothing
        }
        
        protected void Connect(params WindowComponentBehaviour[] components) {
            foreach (WindowComponentBehaviour component in components) {
                Connect(component);
            }
        }
        
    #if TINY_MVC
        
        [Obsolete("Can`t use without parameters", true)]
        protected void Connect<T>(T component) where T : WindowComponentBehaviour, IApplyResolving {
            // Do nothing
        }
        
        protected void Connect<T>(T component, params IDependency[] dependencies) where T : WindowComponentBehaviour, IApplyResolving {
            if (component is IInit init) {
                init.Init();
            }
            
            if (component is IApplyResolving applyResolving) {
                applyResolving.ApplyResolving(dependencies);
            }
            
            if (component is IBeginPlay beginPlay) {
                beginPlay.BeginPlay();
            }
            
            _components.Add(component);
        }
        
    #endif
        
        protected void Connect(WindowComponentBehaviour component) {
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
            
            _components.Add(component);
        }
        
        [Obsolete("Can`t use without parameters", true)]
        protected void Disconnect() {
            // Do nothing
        }
        
        protected void Disconnect(params WindowComponentBehaviour[] components) {
            foreach (WindowComponentBehaviour component in components) {
                Disconnect(component);
            }
        }
        
        protected void DisconnectAll() {
            foreach (WindowComponentBehaviour component in _components) {
                if (component is IUnload unload) {
                    unload.Unload();
                }
            }
            
            _components.Clear();
        }
        
        protected void Disconnect(WindowComponentBehaviour component) {
            int index = _components.IndexOf(component);
            
            if (index < 0) {
                return;
            }
            
            if (_components[index] is IUnload unload) {
                unload.Unload();
            }
            
            _components.RemoveAt(index);
        }
        
        private void Awake() {
            // Do Nothing
        }
        
        private void Start() {
            // Do Nothing
        }
        
        private void OnEnable() {
            // Do Nothing
        }
        
        private void OnDisable() {
            // Do Nothing
        }
        
        private void OnDestroy() {
            if (this is IUnload unload) {
                unload.Unload();
            }
            
            DisconnectAll();
            WindowsService.DestroyWindow(this);
        }
        
        internal void Initialize() {
            _components = new List<WindowComponentBehaviour>();
        }
        
        internal void ShowInternal() {
            isVisible = true;
            Show();
        }
        
        internal void HideInternal() {
            isVisible = false;
            Hide();
        }
    }
}