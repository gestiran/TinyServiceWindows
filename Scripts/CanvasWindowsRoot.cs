// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;

namespace TinyServices.Windows {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public sealed class CanvasWindowsRoot : MonoBehaviour {
        [SerializeField]
        private Canvas _thisCanvas;
        
        private void Start() => WindowsService.ChangeRoot(_thisCanvas);
        
    #if UNITY_EDITOR
        
        [ContextMenu("Soft Reset")]
        private void Reset() {
            _thisCanvas = GetComponent<Canvas>();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
    #endif
    }
}