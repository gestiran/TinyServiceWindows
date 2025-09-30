// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using TinyReactive.Fields;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace TinyServices.Windows {
    public static class WindowsService {
        public static int visibleCount => _visible.Count;
        public static Canvas root { get; private set; }
        
        private static Transform _rootTransform;
        
        public static readonly InputListener<WindowBehavior> onShow;
        public static readonly InputListener<WindowBehavior> onHide;
        
        private static readonly Dictionary<Type, WindowBehavior> _instances;
        private static readonly Dictionary<Type, WindowBehavior> _all;
        private static readonly Stack<WindowBehavior> _visible;
        
        static WindowsService() {
            onShow = new InputListener<WindowBehavior>();
            onHide = new InputListener<WindowBehavior>();
            
            _instances = new Dictionary<Type, WindowBehavior>();
            _all = new Dictionary<Type, WindowBehavior>();
            _visible = new Stack<WindowBehavior>();
            
            WindowsDataBase dataBase = WindowsDataBase.LoadFromResources();
            
            foreach (WindowBehavior window in dataBase.all) {
                _all.TryAdd(window.GetType(), window);
            }
        }
        
        public static void ChangeRoot(Canvas canvas) {
            root = canvas;
            _rootTransform = canvas.transform;
        }
        
        public static IEnumerable<WindowBehavior> ForeachVisible() {
            foreach (WindowBehavior window in _visible) {
                yield return window;
            }
        }
        
        public static bool TryGetTop<T>(out T window) where T : WindowBehavior {
            if (TryGetTop(out WindowBehavior other) && other is T target) {
                window = target;
                return true;
            }
            
            window = null;
            return false;
        }
        
        public static bool TryGetTop(out WindowBehavior window) => _visible.TryPeek(out window);
        
        public static T Show<T>() where T : WindowBehavior => Show<T>(_rootTransform);
        
        public static T Show<T>(Canvas canvas) where T : WindowBehavior => Show<T>(canvas.transform);
        
        public static T Show<T>(Transform parent) where T : WindowBehavior {
            Type type = typeof(T);
            
            if (_instances.TryGetValue(type, out WindowBehavior instance)) {
                if (instance != null) {
                    instance.transform.SetParent(parent);
                } else if (_all.TryGetValue(type, out WindowBehavior window) && window != null) {
                    instance = UnityObject.Instantiate(window, parent);
                    _instances[type] = instance;
                }
            } else if (_all.TryGetValue(type, out WindowBehavior window) && window != null) {
                instance = UnityObject.Instantiate(window, parent);
                _instances.Add(type, instance);
            } else {
                return null;
            }
            
            instance.ShowInternal();
            onShow.Send(instance);
            _visible.Push(instance);
            return instance as T;
        }
        
        public static bool Hide() => Hide(out _);
        
        public static bool Hide(out WindowBehavior window) {
            if (_visible.TryPop(out window) && window != null) {
                window.HideInternal();
                onHide.Send(window);
                return true;
            }
            
            return false;
        }
        
        public static void HideAll() {
            foreach (WindowBehavior window in _visible) {
                if (window != null) {
                    window.Hide();
                    onHide.Send(window);
                }
            }
            
            _visible.Clear();
        }
    }
}