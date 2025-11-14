// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;

namespace TinyServices.Windows {
    public abstract class WindowComponentBehaviour : WindowContext {
        public WindowContext root { get; internal set; }
        
        internal ConnectState connectState;
        
        internal enum ConnectState {
            None,
            Disconnected,
            Connected
        }
        
        private void OnDestroy() { }
        
        internal override void Initialize() {
            if (connectState != ConnectState.None) {
                return;
            }
            
            connections = new List<WindowComponentBehaviour>();
        }
        
        protected bool Disconnect() => root.DisconnectSelf(this);
    }
}