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
        [field: Searchable, Required]
    #endif
        [field: SerializeField]
        public WindowBehavior[] all;
        
    #if UNITY_EDITOR || UNITY_STANDALONE
    #if ODIN_INSPECTOR
        [field: Searchable, LabelText("PC (Windows, Linux, MacOS)"), Required]
    #endif
        [field: SerializeField]
        public WindowBehavior[] pc;
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
            foreach (WindowBehavior window in all) {
                windows.TryAdd(window.GetType(), window);
            }
            
        #if UNITY_STANDALONE
            foreach (WindowBehavior window in pc) {
                windows.TryAdd(window.GetType(), window);
            }
        #elif UNITY_ANDROID || UNITY_IOS
            foreach (WindowBehavior window in mobile) {
                windows.TryAdd(window.GetType(), window);
            }
        #endif
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
            
            Dictionary<Type, WindowBehavior> pcList = new Dictionary<Type, WindowBehavior>(all.Length + pc.Length);
            Dictionary<Type, WindowBehavior> mobileList = new Dictionary<Type, WindowBehavior>(all.Length + mobile.Length);
            
            Validate(pcList, all, nameof(all), result);
            Validate(mobileList, all, nameof(all), result);
            
            Validate(pcList, pc, nameof(pc), result);
            Validate(mobileList, mobile, nameof(mobile), result);
            
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