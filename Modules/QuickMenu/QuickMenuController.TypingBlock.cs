using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private bool gameInputBlockedForTyping;

        private bool IsModTextFieldFocused()
        {
            EventSystem? eventSystem = EventSystem.current;
            GameObject? selected = eventSystem != null ? eventSystem.currentSelectedGameObject : null;
            if (selected == null || !selected.transform.IsChildOf(transform))
            {
                return false;
            }

            InputField? field = selected.GetComponent<InputField>();
            return field != null && field.isFocused;
        }

        private void UpdateTypingGameInputBlock()
        {
            bool typing = IsModTextFieldFocused();
            if (typing == gameInputBlockedForTyping)
            {
                return;
            }

            gameInputBlockedForTyping = typing;
            SetGameActionsEnabled(!typing);
        }

        private void ReleaseTypingGameInputBlock()
        {
            if (!gameInputBlockedForTyping)
            {
                return;
            }

            gameInputBlockedForTyping = false;
            SetGameActionsEnabled(true);
        }

        private static void SetGameActionsEnabled(bool value)
        {
            try
            {
                HeroActions? actions = InputHandler.Instance != null ? InputHandler.Instance.inputActions : null;
                if (actions != null)
                {
                    actions.Enabled = value;
                }
            }
            catch (Exception ex)
            {
                LogSuppressed(ex, "QuickMenuController.TypingBlock.cs");
            }
        }
    }
}
