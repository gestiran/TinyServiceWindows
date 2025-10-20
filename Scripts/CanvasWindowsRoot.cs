// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyServices.Windows {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public class CanvasWindowsRoot : MonoBehaviour {
    #if ODIN_INSPECTOR
        [BoxGroup("Generated"), Required, ReadOnly]
    #endif
        [SerializeField]
        private Canvas _thisCanvas;
        
        protected virtual void Start() => WindowsService.ChangeRoot(_thisCanvas);
        
    #if UNITY_EDITOR
        
        [ContextMenu("Soft Reset")]
        protected virtual void Reset() {
            _thisCanvas = GetComponent<Canvas>();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
    #endif
    }
}