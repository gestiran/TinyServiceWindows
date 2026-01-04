// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using TinyReactive;
using UnityEngine;

namespace TinyServices.Windows {
    public abstract class WindowBehavior : WindowContext, IComparable<WindowBehavior> {
        public virtual int priority => 0;
        public virtual bool ignoreAutoHide => false;
        public virtual bool ignoreInput => false;
        public virtual bool staticCanvas => false;
        public bool isVisible { get; internal set; }
        
        public virtual void Show() => gameObject.SetActive(true);
        
        public virtual void Hide() => gameObject.SetActive(false);
        
        private void OnDestroy() {
            if (this is IUnload unload) {
                try {
                    unload.Unload();
                } catch (Exception exception) {
                    Debug.LogException(new Exception($"Invalid Unload operation - {GetType().Name}", exception));
                }
            }
            
            DisconnectAll();
            WindowsService.DestroyWindow(this);
        }
        
        [Obsolete("Can`t use without parameters", true)]
        protected void Disconnect() { }
        
        internal override void Initialize() => connections = new List<WindowComponentBehaviour>();
        
        internal virtual void ShowInternal() {
            isVisible = true;
            
            try {
                Show();
            } catch (Exception exception) {
                Debug.LogException(new Exception($"Invalid Show operation - {GetType().Name}", exception));
            }
        }
        
        internal virtual void HideInternal() {
            isVisible = false;
            
            try {
                Hide();
            } catch (Exception exception) {
                Debug.LogException(new Exception($"Invalid Hide operation - {GetType().Name}", exception));
            }
        }
        
        public int CompareTo(WindowBehavior other) => priority - other.priority;
    }
}