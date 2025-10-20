// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using TinyReactive.Fields;
using UnityEngine;
using UnityObject = UnityEngine.Object;

#if TINY_MVC
using TinyMVC.Loop;
using TinyMVC.Dependencies;
using TinyMVC.Dependencies.Extensions;
#endif

namespace TinyServices.Windows {
    public static class WindowsService {
        public static int visibleCount => _visible.Count;
        public static Canvas root { get; private set; }
        
        private static Transform _rootTransform;
        
        public static readonly InputListener<WindowBehavior> onShow;
        public static readonly InputListener<WindowBehavior> onHide;
        public static readonly InputListener onUpdateVisible;
        
        private static readonly Dictionary<Type, WindowBehavior> _instances;
        private static readonly Dictionary<Type, WindowBehavior> _all;
        private static readonly List<WindowBehavior> _visible;
        
        static WindowsService() {
            onShow = new InputListener<WindowBehavior>();
            onHide = new InputListener<WindowBehavior>();
            onUpdateVisible = new InputListener();
            
            _instances = new Dictionary<Type, WindowBehavior>();
            _all = new Dictionary<Type, WindowBehavior>();
            _visible = new List<WindowBehavior>();
            
            WindowsDataBase dataBase = WindowsDataBase.LoadFromResources();
            
            foreach (WindowBehavior window in dataBase.all) {
                _all.TryAdd(window.GetType(), window);
            }
        }
        
        public static void ChangeRoot(Canvas canvas) {
            root = canvas;
            _rootTransform = canvas.transform;
            
            foreach (WindowBehavior window in _visible) {
                window.transform.SetParent(_rootTransform);
            }
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
        
        public static bool TryGetTop(out WindowBehavior window) {
            if (_visible.Count > 0) {
                window = _visible[^1];
                return true;
            }
            
            window = null;
            return false;
        }
        
        public static bool TryGetVisible<T>(out T window) where T : WindowBehavior {
            foreach (WindowBehavior other in _visible) {
                if (other is T target) {
                    window = target;
                    return true;
                }
            }
            
            window = null;
            return false;
        }
        
    #if TINY_MVC
        
        public static T Show<T>(params IDependency[] dependencies) where T : WindowBehavior {
            return Show<T>(_rootTransform, (window, transform) => Instantiate(window, transform, dependencies));
        }
        
        public static T Show<T>(Canvas canvas, params IDependency[] dependencies) where T : WindowBehavior {
            return Show<T>(canvas.transform, (window, transform) => Instantiate(window, transform, dependencies));
        }
        
        public static T Show<T>(Transform parent, params IDependency[] dependencies) where T : WindowBehavior {
            return Show<T>(parent, (window, transform) => Instantiate(window, transform, dependencies));
        }
        
    #endif
        
        public static T Show<T>() where T : WindowBehavior => Show<T>(_rootTransform, Instantiate);
        
        public static T Show<T>(Canvas canvas) where T : WindowBehavior => Show<T>(canvas.transform, Instantiate);
        
        public static T Show<T>(Transform parent) where T : WindowBehavior => Show<T>(parent, Instantiate);
        
        private static T Show<T>(Transform parent, Func<WindowBehavior, Transform, WindowBehavior> instantiate) where T : WindowBehavior {
            Type type = typeof(T);
            
            if (_instances.TryGetValue(type, out WindowBehavior instance) == false) {
                if (_all.TryGetValue(type, out WindowBehavior window)) {
                    instance = instantiate(window, parent);
                } else {
                    return null;
                }
                
                _instances.Add(type, instance);
            } else {
                instance.transform.SetParent(parent);
            }
            
            instance.ShowInternal();
            onShow.Send(instance);
            _visible.Add(instance);
            
            _visible.Sort();
            
            for (int orderId = 0; orderId < _visible.Count; orderId++) {
                _visible[orderId].transform.SetSiblingIndex(orderId);
            }
            
            onUpdateVisible.Send();
            return instance as T;
        }
        
        public static bool Hide() => Hide(out _);
        
        public static bool Hide(out WindowBehavior window) {
            if (_visible.Count > 0) {
                int windowId = _visible.Count - 1;
                
                window = _visible[windowId];
                _visible.RemoveAt(windowId);
                onUpdateVisible.Send();
                window.HideInternal();
                onHide.Send(window);
                return true;
            }
            
            window = null;
            return false;
        }
        
        public static void HideAll() {
            foreach (WindowBehavior window in _visible) {
                window.Hide();
                onHide.Send(window);
            }
            
            _visible.Clear();
            onUpdateVisible.Send();
        }
        
        internal static void DestroyWindow(WindowBehavior window) {
            _instances.Remove(window.GetType());
            _visible.Remove(window);
            onUpdateVisible.Send();
        }
        
    #if TINY_MVC
        private static WindowBehavior Instantiate(WindowBehavior prefab, Transform parent, IDependency[] dependencies) {
            WindowBehavior instance = UnityObject.Instantiate(prefab, parent);
            instance.Initialize();
            
            if (instance is IInit init) {
                init.Init();
            }
            
            if (instance is IApplyResolving applyResolving) {
                applyResolving.ApplyResolving(dependencies);
            }
            
            if (instance is IBeginPlay beginPlay) {
                beginPlay.BeginPlay();
            }
            
            return instance;
        }
    #endif
        
        private static WindowBehavior Instantiate(WindowBehavior prefab, Transform parent) {
            WindowBehavior instance = UnityObject.Instantiate(prefab, parent);
            instance.Initialize();
            
        #if TINY_MVC
            
            if (instance is IInit init) {
                init.Init();
            }
            
            if (instance is IApplyResolving applyResolving) {
                applyResolving.ApplyResolving();
            }
            
            if (instance is IBeginPlay beginPlay) {
                beginPlay.BeginPlay();
            }
            
        #endif
            
            return instance;
        }
    }
}