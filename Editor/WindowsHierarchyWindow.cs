using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TinyServices.Windows.Editor {
    internal sealed class WindowsHierarchyWindow : EditorWindow {
        private VisualElement _group;
        
        [MenuItem("Window/Tiny MVC/Windows Hierarchy", priority = 0)]
        private static void OpenWindow() => GetWindow<WindowsHierarchyWindow>("Windows");
        
        private void OnBecameVisible() {
            WindowsService.onShow.AddListener(UpdateWindowsList);
            WindowsService.onHide.AddListener(UpdateWindowsList);
            EditorApplication.playModeStateChanged += PlayModeChange;
        }
        
        private void OnBecameInvisible() {
            WindowsService.onShow.RemoveListener(UpdateWindowsList);
            WindowsService.onHide.RemoveListener(UpdateWindowsList);
            EditorApplication.playModeStateChanged -= PlayModeChange;
        }
        
        private void CreateGUI() {
            VisualElement root = rootVisualElement;
            ScrollView scroll = new ScrollView(ScrollViewMode.Vertical);
            
            scroll.style.flexGrow = 1;
            scroll.style.flexShrink = 1;
            scroll.style.flexBasis = 0;
            
            _group = new VisualElement();
            
            _group.style.flexDirection = FlexDirection.Column;
            _group.style.justifyContent = Justify.SpaceBetween;
            
            _group.style.marginTop = 4f;
            _group.style.marginBottom = 4f;
            _group.style.marginLeft = 4f;
            _group.style.marginRight = 4f;
            
            scroll.Add(_group);
            root.Add(scroll);
            
            UpdateWindowsList();
        }
        
        private void PlayModeChange(PlayModeStateChange mode) {
            if (mode == PlayModeStateChange.EnteredPlayMode) {
                UpdateWindowsList();
            } else if (mode == PlayModeStateChange.EnteredEditMode) {
                UpdateWindowsList();
            }
        }
        
        private void UpdateWindowsList() {
            _group.Clear();
            
            if (Application.isPlaying) {
                if (WindowsService.visibleCount > 0) {
                    Type objectType = typeof(WindowBehavior);
                    
                    foreach (WindowBehavior window in WindowsService.ForeachVisible()) {
                        ObjectField objectField = new ObjectField();
                        objectField.objectType = objectType;
                        objectField.value = window;
                        objectField.SetEnabled(false);
                        
                        _group.Add(objectField);
                    }
                } else {
                    TextElement label = new TextElement();
                    label.text = "Not have opened windows!";
                    _group.Add(label);
                }
            } else {
                TextElement label = new TextElement();
                label.text = "Working in PlayMode only!";
                _group.Add(label);
            }
            
            _group.MarkDirtyRepaint();
        }
    }
}