<<<<<<< HEAD
namespace GodhomeQoL;

internal static class L11nUtil {
	internal static readonly Dict dict = new(typeof(GodhomeQoL).Assembly, "Resources.Lang");
=======
namespace SafeGodseekerQoL;

internal static class L11nUtil {
	internal static readonly Dict dict = new(typeof(SafeGodseekerQoL).Assembly, "Resources.Lang");
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265

	internal static string Localize(this string key) =>
		dict.Localize(key);
}
