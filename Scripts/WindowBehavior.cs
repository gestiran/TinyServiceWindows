// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;

namespace TinyServices.Windows {
    [DisallowMultipleComponent]
    public abstract class WindowBehavior : MonoBehaviour {
        public bool isVisible { get; private set; }
        
        public virtual void Init() { }
        
        public virtual void Unload() { }
        
        public virtual void Show() => gameObject.SetActive(true);
        
        public virtual void Hide() => gameObject.SetActive(false);
        
        private void OnDestroy() {
            Unload();
            WindowsService.DestroyWindow(this);
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