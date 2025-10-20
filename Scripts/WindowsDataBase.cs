// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

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
        
        private const string _PATH = "WindowsDataBase";
        
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
            
            for (int currentId = 0; currentId < all.Length; currentId++) {
                for (int targetId = 0; targetId < all.Length; targetId++) {
                    if (currentId != targetId && all[currentId].GetType() == all[targetId].GetType()) {
                        result.AddError($"Invalid window type {all[currentId].GetType().Name}!");
                    }
                }
            }
            
        #endif
        }
    #endif
    }
}