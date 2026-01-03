// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyServices.Windows {
    [CreateAssetMenu(fileName = "WindowsDataBase", menuName = "TinyServices/Windows/DataBase")]
    public sealed class WindowsDataBase : ScriptableObject, ISelfValidator {
    #if ODIN_INSPECTOR
        [field: Searchable, LabelText("All Platforms"), Required]
    #endif
        [field: SerializeField]
        public WindowBehavior[] all;
        
    #if UNITY_EDITOR || UNITY_STANDALONE
    #if ODIN_INSPECTOR
        [field: Searchable, LabelText("Standalone (Windows, Linux, MacOS)"), Required]
    #endif
        [field: SerializeField]
        public WindowBehavior[] standalone;
    #endif
        
    #if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
    #if ODIN_INSPECTOR
        [field: Searchable, LabelText("Mobile (Android, IOS)"), Required]
    #endif
        [field: SerializeField]
        public WindowBehavior[] mobile;
    #endif
        
        private const string _PATH = "WindowsDataBase";
        
        public void Fill(Dictionary<Type, WindowBehavior> windows) {
            Fill(all, windows);
        #if UNITY_STANDALONE
            Fill(standalone, windows);
        #elif UNITY_ANDROID || UNITY_IOS
            Fill(mobile, windows);
        #endif
        }
        
        private void Fill(WindowBehavior[] source, Dictionary<Type, WindowBehavior> destination) {
            foreach (WindowBehavior window in source) {
                try {
                    destination.TryAdd(window.GetType(), window);
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        public static WindowsDataBase LoadFromResources() {
            WindowsDataBase dataBase = Resources.Load<WindowsDataBase>(_PATH);
            
            if (dataBase == null) {
                dataBase = Resources.Load<WindowsDataBase>($"{_PATH}_Default");
            }
            
            return dataBase;
        }
        
    #if ODIN_INSPECTOR
        public void Validate(SelfValidationResult result) {
        #if UNITY_EDITOR
            
            Dictionary<Type, WindowBehavior> testList = new Dictionary<Type, WindowBehavior>();
            
            Validate(testList, all, nameof(all), result);
            Validate(testList, standalone, nameof(standalone), result);
            
            testList.Clear();
            
            Validate(testList, all, nameof(all), result);
            Validate(testList, mobile, nameof(mobile), result);
            
        #endif
        }
        
    #if UNITY_EDITOR
        
        private void Validate(Dictionary<Type, WindowBehavior> checkList, WindowBehavior[] windows, string group, SelfValidationResult result) {
            foreach (WindowBehavior window in windows) {
                if (window == null) {
                    continue;
                }
                
                Type windowType = window.GetType();
                
                if (checkList.TryAdd(windowType, window)) {
                    continue;
                }
                
                result.AddError($"Invalid window type {windowType.Name} in group {group}!");
            }
        }
        
    #endif
        
    #endif
    }
}