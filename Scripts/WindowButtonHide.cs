using UnityEngine;

namespace TinyServices.Windows {
    [DisallowMultipleComponent]
    public sealed class WindowButtonHide : MonoBehaviour {
        public void Hide() => WindowsService.Hide();
    }
}