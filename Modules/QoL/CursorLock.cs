namespace GodhomeQoL.Modules.QoL;

public sealed class CursorLock : Module
{
    private protected override void Load()
    {
        On.InputHandler.OnGUI += OnInputHandlerGui;
    }

    private protected override void Unload()
    {
        On.InputHandler.OnGUI -= OnInputHandlerGui;
        if (Cursor.lockState == CursorLockMode.Confined)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private static void OnInputHandlerGui(On.InputHandler.orig_OnGUI orig, InputHandler self)
    {
        orig(self);
        if (ShouldConfine())
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private static bool ShouldConfine()
    {
        if (!Application.isFocused)
        {
            return false;
        }

        GameManager? gm = GameManager.instance;
        if (gm == null || gm.isPaused || gm.gameState == GameState.PAUSED || !gm.IsGameplayScene())
        {
            return false;
        }

        return !Tools.QuickMenu.IsAnyUiVisible();
    }
}
