// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;

namespace TinyServices.Windows {
    [DisallowMultipleComponent]
    public abstract class WindowButtonShow<T> : MonoBehaviour where T : WindowBehavior {
        public virtual void Show() => WindowsService.Show<T>();
    }
}