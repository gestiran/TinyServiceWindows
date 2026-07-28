// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyServices.Windows {
    public abstract class WindowComponentBehaviour : WindowContext {
        public WindowContext root { get; internal set; }
        
        private void OnDestroy() { }
        
        protected bool Disconnect() {
            if (connectState == ConnectState.Disconnected) {
                return true;
            }
            
            return root.DisconnectSelf(this);
        }
    }
}