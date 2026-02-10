namespace GodhomeQoL.Modules.Tools;

internal sealed class FreezeHitboxesViewer
{
    private FreezeHitboxesRender? hitboxRender;

    public void Load()
    {
        Unload();
        USceneManager.activeSceneChanged += OnSceneChanged;
        ModHooks.ColliderCreateHook += UpdateHitboxRender;
        CreateHitboxRender();
    }

    public void Unload()
    {
        USceneManager.activeSceneChanged -= OnSceneChanged;
        ModHooks.ColliderCreateHook -= UpdateHitboxRender;
        DestroyHitboxRender();
    }

    private void OnSceneChanged(Scene current, Scene next) => CreateHitboxRender();

    private void CreateHitboxRender()
    {
        DestroyHitboxRender();
        if (GameManager.instance != null && GameManager.instance.IsGameplayScene())
        {
            hitboxRender = new GameObject("FreezeHitboxesRender").AddComponent<FreezeHitboxesRender>();
        }
    }

    private void DestroyHitboxRender()
    {
        if (hitboxRender != null)
        {
            UObject.Destroy(hitboxRender);
            hitboxRender = null;
        }
    }

    private void UpdateHitboxRender(GameObject go)
    {
        hitboxRender?.UpdateHitbox(go);
    }
}
