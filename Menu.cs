using Satchel.BetterMenus;
using GodhomeQoL.Modules;
using GodhomeQoL.Modules.CollectorPhases;
using GodhomeQoL.Modules.QoL;
using GodhomeQoL.Modules.Tools;

namespace GodhomeQoL;

public sealed partial class GodhomeQoL
{
    bool ICustomMenuMod.ToggleButtonInsideMenu => true;

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates) =>
        ModMenu.GetMenuScreen(modListMenu, toggleDelegates);

    private static class ModMenu
    {
        private static bool dirty = true;
        private static Menu? menu;
        private static bool hooksInstalled;
        private static On.Language.Language.hook_DoSwitch? doSwitchHook;

        internal static void MarkDirty() => dirty = true;

        internal static void InstallHooks()
        {
            if (hooksInstalled)
            {
                return;
            }

            doSwitchHook ??= (orig, self) =>
            {
                dirty = true;
                orig(self);
            };
            On.Language.Language.DoSwitch += doSwitchHook;
            hooksInstalled = true;
        }

        internal static void UninstallHooks()
        {
            if (!hooksInstalled)
            {
                return;
            }

            if (doSwitchHook != null)
            {
                On.Language.Language.DoSwitch -= doSwitchHook;
            }
            hooksInstalled = false;
            dirty = true;
            menu = null;
        }

        internal static MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            if (menu != null && !dirty)
            {
                return menu.GetMenuScreen(modListMenu);
            }

            menu = new Menu("ModName".Localize(), [
                toggleDelegates!.Value.CreateToggle(
                    "ModName".Localize(),
                    "ToggleButtonDesc".Localize()
                )
            ]);

            menu.AddElement(QuickMenu.QuickMenuHotkeyButton());
            ModuleManager
                .Modules
                .Values
                .Filter(module =>
                    !module.Hidden
                    && module.Category != "Bugfix"
                    && module.Category != "Tools"
                    && !ShouldHideCategoryFromMainMenu(module.Category)
                )
                .GroupBy(module => module.Category)
                .OrderBy(group => group.Key)
                .Map(group =>
                    Blueprints.NavigateToMenu(
                        $"Categories/{group.Key}".Localize(),
                        "",
                        () => new Menu(
                            $"Categories/{group.Key}".Localize(),
                            [
                                ..group
                                    .Filter(ShouldShowInMainList)
                                    .Map(module =>
                                        Blueprints.HorizontalBoolOption(
                                            $"Modules/{module.Name}".Localize(),
                                            module.Suppressed
                                                ? string.Format(
                                                    "Suppression".Localize(),
                                                    module.suppressorMap.Values.Distinct().Join(", ")
                                                )
                                                : $"ToggleableLevel/{module.ToggleableLevel}".Localize(),
                                            val => module.Enabled = val,
                                            () => module.Enabled
                                        )
                                    ),
                                ..Setting.Global.GetMenuElements(group.Key),
                                ..Setting.Local.GetMenuElements(group.Key)
                            ]
                        ).GetMenuScreen(menu!.menuScreen)
                    )
                )
                .ForEach(menu.AddElement);

            menu.AddElement(new MenuButton(
                "QuickMenu/ResetFreeMenu".Localize(),
                "",
                _ => Modules.Tools.QuickMenu.ResetFreeMenuPositions(),
                true
            ));

            dirty = false;
            return menu.GetMenuScreen(modListMenu);
        }
    }

    private static bool ShouldHideCategoryFromMainMenu(string category)
    {
        return category == nameof(Modules.BossChallenge)
            || category == nameof(Modules.QoL)
            || category == nameof(FastReload)
            || category == "BossManipulate"
            || category == "CollectorPhases";
    }

    private static bool ShouldShowInMainList(Module module)
    {
        if (module is FastSuperDash
            || module is TeleportKit
            || module is Modules.QoL.FastDreamWarp
            || module is Modules.QoL.DoorDefaultBegin
            || module is Modules.QoL.FasterLoads
            || module is Modules.QoL.FastMenus
            || module is Modules.QoL.FastText
            || module is Modules.QoL.ShortDeathAnimation)
        {
            return false;
        }

        if (module.Category == nameof(Modules.BossChallenge))
        {
            return false;
        }

        if (module is Modules.BossChallenge.ForceGreyPrinceEnterType
            || module is Modules.BossChallenge.AddLifeblood
            || module is Modules.BossChallenge.AddSoul)
        {
            return false;
        }

        return true;
    }
}
