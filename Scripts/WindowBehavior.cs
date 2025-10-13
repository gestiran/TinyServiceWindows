// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;
using TinyReactive;

namespace TinyServices.Windows {
    [DisallowMultipleComponent]
    public abstract class WindowBehavior : MonoBehaviour {
        public bool isVisible { get; private set; }
        
        public virtual void Show() => gameObject.SetActive(true);
        
        public virtual void Hide() => gameObject.SetActive(false);
        
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