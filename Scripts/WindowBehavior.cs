// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using TinyReactive;

namespace TinyServices.Windows {
    public abstract class WindowBehavior : WindowContext, IComparable<WindowBehavior> {
        public virtual int priority => 0;
        public virtual bool ignoreAutoHide => false;
        public virtual bool ignoreInput => false;
        public bool isVisible { get; private set; }
        
        public virtual void Show() => gameObject.SetActive(true);
        
        public virtual void Hide() => gameObject.SetActive(false);
        
        private void OnDestroy() {
            if (this is IUnload unload) {
                unload.Unload();
            }
            
            DisconnectAll();
            WindowsService.DestroyWindow(this);
        }
        
        [Obsolete("Can`t use without parameters", true)]
        protected void Disconnect() { }
        
        internal override void Initialize() => connections = new List<WindowComponentBehaviour>();
        
        internal void ShowInternal() {
            isVisible = true;
            Show();
        }
        
        internal void HideInternal() {
            isVisible = false;
            Hide();
        }
        
        public int CompareTo(WindowBehavior other) => priority - other.priority;
    }
}