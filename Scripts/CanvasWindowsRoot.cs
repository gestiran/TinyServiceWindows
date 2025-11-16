// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyServices.Windows {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public sealed class CanvasWindowsRoot : MonoBehaviour {
        public Canvas canvas => _thisCanvas;
        
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
        
        private void Start() {
            WindowsService.AddRoot(_thisCanvas);
            StartCoroutine(CreateWindows());
        }
        
        private void OnDestroy() => WindowsService.RemoveRoot(_thisCanvas);
        
        private IEnumerator CreateWindows() {
            yield return new WaitForEndOfFrame();
            
            foreach (WindowBehavior window in _windows) {
                WindowsService.Show(window.GetType(), transform, WindowsService.Instantiate);
            }
        }
        
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
        private void Reset() {
            _thisCanvas = GetComponent<Canvas>();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
    #endif
    }
}