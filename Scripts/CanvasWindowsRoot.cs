// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;

#if TINY_MVC
using TinyMVC.Loop;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
using TinyReactive;
#else
using System.Collections;
#endif

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyServices.Windows {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
#if TINY_MVC
    public sealed class CanvasWindowsRoot : View, IInit, IBeginPlay, IUnload, IGeneratedContext {
    #else
        public sealed class CanvasWindowsRoot : MonoBehaviour {
    #endif
        public Canvas canvas => _thisCanvas;
        
        [field: SerializeField]
        public bool withWindows { get; private set; }
        
    #if ODIN_INSPECTOR
        [ValueDropdown("GetAllWindows"), Required]
    #endif
        [SerializeField]
        private WindowBehavior[] _windows;
        
    #if ODIN_INSPECTOR
        [BoxGroup("Generated"), Required, ReadOnly]
    #endif
        [SerializeField]
        private Canvas _thisCanvas;
        
        public void Init() => WindowsService.AddRoot(_thisCanvas, withWindows);
        
        public void BeginPlay() {
            foreach (WindowBehavior window in _windows) {
                WindowsService.Show(window.GetType(), transform, WindowsService.Instantiate);
            }
        }
        
        public void Unload() => WindowsService.RemoveRoot(_thisCanvas);
        
    #if !TINY_MVC
        private void Awake() => Init();
        
        private IEnumerator Start() {
            yield return new WaitForEndOfFrame();
            BeginPlay();
        }
        
        private void OnDestroy() => Unload();
    #endif
        
    #if UNITY_EDITOR
        
    #if ODIN_INSPECTOR
        
        private ValueDropdownList<WindowBehavior> GetAllWindows() {
            WindowsDataBase dataBase = WindowsDataBase.LoadFromResources();
            
            ValueDropdownList<WindowBehavior> windows = new ValueDropdownList<WindowBehavior>();
            
            foreach (WindowBehavior window in dataBase.all) {
                windows.Add(window.GetType().Name, window);
            }
            
            return windows;
        }
        
    #endif
        
        [ContextMenu("Soft Reset")]
    #if TINY_MVC
        public override void Reset() {
            _thisCanvas = GetComponent<Canvas>();
            base.Reset();
        }
    #else
        private void Reset() {
            _thisCanvas = GetComponent<Canvas>();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    #endif
        
        public override string ToString() => gameObject.name;
        
    #endif
    }
}