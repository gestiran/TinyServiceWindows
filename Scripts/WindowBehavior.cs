// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;

#if TINY_MVC
using TinyMVC.Loop;
using TinyMVC.Views;
using TinyReactive;
#endif

namespace TinyServices.Windows {
    [DisallowMultipleComponent]
#if TINY_MVC
    public abstract class WindowBehavior : View, IInit, IUnload {
    #else
    public abstract class WindowBehavior : MonoBehaviour {
    #endif
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