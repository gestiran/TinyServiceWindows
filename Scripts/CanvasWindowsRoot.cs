// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
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
    public class CanvasWindowsRoot : View, IInit, IBeginPlay, IUnload, IGeneratedContext {
    #else
        public class CanvasWindowsRoot : MonoBehaviour {
    #endif
        public Canvas canvas => _thisCanvas;
        
        [field: SerializeField]
        public bool withWindows { get; private set; }
        
    #if ODIN_INSPECTOR
        [Searchable, ValueDropdown("GetAllWindows"), LabelText("All Platforms"), Required]
    #endif
        [SerializeField]
        private WindowBehavior[] _windows;
        
    #if UNITY_EDITOR || UNITY_STANDALONE
    #if ODIN_INSPECTOR
        [Searchable, ValueDropdown("GetStandaloneWindows"), LabelText("Standalone (Windows, Linux, MacOS)"), Required]
    #endif
        [SerializeField]
        private WindowBehavior[] _windowsStandalone;
    #endif
        
    #if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
    #if ODIN_INSPECTOR
        [Searchable, ValueDropdown("GetMobileWindows"), LabelText("Mobile (Android, IOS)"), Required]
    #endif
        [SerializeField]
        private WindowBehavior[] _windowsMobile;
    #endif
        
    #if ODIN_INSPECTOR
        [BoxGroup("Generated"), Required, ReadOnly]
    #endif
        [SerializeField]
        private Canvas _thisCanvas;
        
        public virtual void Init() => WindowsService.AddRoot(_thisCanvas, withWindows);
        
        public virtual void BeginPlay() {
            CreateWindows(_windows, transform);
        #if UNITY_STANDALONE
            CreateWindows(_windowsStandalone, transform);
        #elif UNITY_ANDROID || UNITY_IOS
            CreateWindows(_windowsMobile, transform);
        #endif
        }
        
        public virtual void Unload() => WindowsService.RemoveRoot(_thisCanvas);
        
        private void CreateWindows(WindowBehavior[] windows, Transform parent) {
            foreach (WindowBehavior window in windows) {
                try {
                    WindowsService.Show(window.GetType(), parent, WindowsService.Instantiate);
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
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
        
        private ValueDropdownList<WindowBehavior> GetAllWindows() => GetWindows(WindowsDataBase.LoadFromResources().all);
        
        private ValueDropdownList<WindowBehavior> GetStandaloneWindows() => GetWindows(WindowsDataBase.LoadFromResources().standalone);
        
        private ValueDropdownList<WindowBehavior> GetMobileWindows() => GetWindows(WindowsDataBase.LoadFromResources().mobile);
        
        private ValueDropdownList<WindowBehavior> GetWindows(WindowBehavior[] source) {
            ValueDropdownList<WindowBehavior> result = new ValueDropdownList<WindowBehavior>();
            
            foreach (WindowBehavior window in source) {
                if (window != null) {
                    result.Add(window.GetType().Name, window);   
                }
            }
            
            return result;
        }
        
    #endif
        
        [ContextMenu("Soft Reset")]
    #if TINY_MVC
        public override void Reset() {
            _thisCanvas = GetComponent<Canvas>();
            base.Reset();
        }
    #else
        public virtual void Reset() {
            _thisCanvas = GetComponent<Canvas>();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    #endif
        
        public override string ToString() => gameObject.name;
        
    #endif
    }
}