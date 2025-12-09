using Satchel.BetterMenus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MenuButton = Satchel.BetterMenus.MenuButton;
using SafeGodseekerQoL.Modules;
using SafeGodseekerQoL.Modules.CollectorPhases;
using SafeGodseekerQoL.Modules.QoL;

namespace SafeGodseekerQoL;

public sealed partial class SafeGodseekerQoL : ICustomMenuMod
{
    bool ICustomMenuMod.ToggleButtonInsideMenu => true;

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates) =>
        ModMenu.GetMenuScreen(modListMenu, toggleDelegates);

    private static class ModMenu
    {
        private static bool dirty = true;
        private static Menu? menu = null;
        internal static void MarkDirty() => dirty = true;
        static ModMenu() => On.Language.Language.DoSwitch += (orig, self) =>
        {
            dirty = true;
            orig(self);
        };

        internal static MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            if (menu != null && !dirty)
            {
                return menu.GetMenuScreen(modListMenu);
            }

            menu = new("ModName".Localize(), [
                toggleDelegates!.Value.CreateToggle(
                    "ModName".Localize(),
                    "ToggleButtonDesc".Localize()
                )
            ]);

            ModuleManager
                .Modules
                .Values
                .Filter(module => !module.Hidden && module is not CollectorPhases && module is not FastReload)
                .GroupBy(module => module.Category)
                .OrderBy(group => group.Key == nameof(Modules.Misc))
                .ThenBy(group => group.Key)
                .Map(group => Blueprints.NavigateToMenu(
                    $"Categories/{group.Key}".Localize(),
                    "",
                    () => new Menu(
                        $"Categories/{group.Key}".Localize(),
                        [
                            ..group
                                .Filter(module => module is not FastSuperDash)
                                .Map(module =>
                                Blueprints.HorizontalBoolOption(
                                    $"Modules/{module.Name}".Localize(),
                                    module.Suppressed
                                        ? string.Format(
                                            "Suppression".Localize(),
                                            module.suppressorMap.Values.Distinct().Join(", ")
                                        )
                                        : $"ToggleableLevel/{module.ToggleableLevel}".Localize(),
                                    (val) => module.Enabled = val,
                                    () => module.Enabled
                                )
                            ),
                            ..Setting.Global.GetMenuElements(group.Key),
                            ..Setting.Local.GetMenuElements(group.Key),
                            ..CustomMenuElements(group.Key)
                        ]).GetMenuScreen(menu!.menuScreen)
                ))
                .ForEach(menu.AddElement);

            menu.AddElement(Blueprints.NavigateToMenu(
                "Tools".Localize(),
                "",
                () => new Menu(
                    "Tools".Localize(),
                    [
                        Blueprints.NavigateToMenu("Modules/FastSuperDash".Localize(), "", () => FastSuperDash.GetMenu(menu.menuScreen)),
                        Blueprints.NavigateToMenu("CollectorPhases".Localize(), "", () => CollectorPhasesMenu.GetMenu(menu.menuScreen)),
                        Blueprints.NavigateToMenu("FastReload".Localize(), "", () => new Menu(
                            "FastReload".Localize(),
                            [..CustomMenuElements(nameof(FastReload))]
                        ).GetMenuScreen(menu.menuScreen)),
                        Blueprints.NavigateToMenu("DreamshieldSettings".Localize(), "", () => new Menu(
                            "DreamshieldSettings".Localize(),
                            [..CustomMenuElements("Dreamshield")]
                        ).GetMenuScreen(menu.menuScreen)),
                        Blueprints.NavigateToMenu("TeleportKit".Localize(), "", () => TeleportKitMenu(menu.menuScreen))
                    ]
                ).GetMenuScreen(menu.menuScreen)
            ));

            menu.AddElement(new MenuButton(
                "ResetModules".Localize(),
                string.Empty,
                btn => ModuleManager.Modules.Values.ForEach(
                    module => module.Enabled = module.DefaultEnabled
                ),
                true
            ));

            dirty = false;
            return menu.GetMenuScreen(modListMenu);
        }

    }

    private static IEnumerable<Element> CustomMenuElements(string category)
    {
        List<Element> elements = [];

        if (category == nameof(FastReload))
        {
            elements.Add(FastReload.ReloadBindButton());
        }

        if (category == "CollectorPhases")
        {
            elements.AddRange(CollectorPhases.MenuElements());
        }

        if (category == "Dreamshield")
        {
            elements.AddRange(DreamshieldStartAngle.MenuElements());
        }

        return elements;
    }

    private static MenuScreen TeleportKitMenu(MenuScreen parent)
    {
        _ = ModuleManager.TryGetModule(typeof(TeleportKit), out Module? module);

        Element toggle = Blueprints.HorizontalBoolOption(
            "Modules/TeleportKit".Localize(),
            $"ToggleableLevel/{(module?.ToggleableLevel ?? ToggleableLevel.AnyTime)}".Localize(),
            val =>
            {
                if (module != null)
                {
                    module.Enabled = val;
                }
            },
            () => module?.Enabled ?? false
        );

        return new Menu(
            "TeleportKit".Localize(),
            [toggle]
        ).GetMenuScreen(parent);
    }

}
