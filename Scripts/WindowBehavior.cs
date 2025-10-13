// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using TinyReactive;

namespace TinyServices.Windows {
    public abstract class WindowBehavior : WindowContext {
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
        
        internal override void Initialize() => connections = new List<WindowComponentBehaviour>();
        
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