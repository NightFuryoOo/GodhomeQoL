using System;
using UnityEngine;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private static Module? GetCachedModule(ref Module? cache, Type moduleType)
        {
            if (cache == null)
            {
                ModuleManager.TryGetModule(moduleType, out cache);
            }

            return cache;
        }

        private static void SetModuleEnabledFlag(Module? module, bool value)
        {
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private void ApplyPanelVisible(GameObject? root, bool value, Action refreshUi)
        {
            if (root != null)
            {
                root.SetActive(value);
            }

            if (value)
            {
                refreshUi();
            }

            UpdateUiState();
        }
    }
}
