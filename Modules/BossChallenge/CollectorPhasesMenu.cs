using Satchel.BetterMenus;

<<<<<<< HEAD
namespace GodhomeQoL.Modules.CollectorPhases;
=======
namespace SafeGodseekerQoL.Modules.CollectorPhases;
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265

public static class CollectorPhasesMenu {
	public static MenuScreen GetMenu(MenuScreen parent) {
		_ = ModuleManager.TryGetModule(typeof(CollectorPhases), out Module? module);

		List<Element> elements = [];

		elements.Add(Blueprints.HorizontalBoolOption(
			"Modules/CollectorPhases".Localize(),
			$"ToggleableLevel/{ToggleableLevel.ChangeScene}".Localize(),
			val => {
				if (module != null) {
					module.Enabled = val;
				}
			},
			() => module?.Enabled ?? false
		));

		elements.AddRange(CollectorPhases.MenuElements());

		return new Menu(
			"CollectorPhases".Localize(),
			[..elements]
		).GetMenuScreen(parent);
	}
}
