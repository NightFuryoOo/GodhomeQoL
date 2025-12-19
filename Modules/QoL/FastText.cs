using TMPro;
using Vasi;

<<<<<<< HEAD
namespace GodhomeQoL.Modules.QoL;
=======
namespace SafeGodseekerQoL.Modules.QoL;
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265

public sealed class FastText : Module
{
    public override bool DefaultEnabled => true;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load() =>
        On.DialogueBox.ShowNextChar += OnNextChar;

    private protected override void Unload() =>
        On.DialogueBox.ShowNextChar -= OnNextChar;

    private static void OnNextChar(On.DialogueBox.orig_ShowNextChar orig, DialogueBox self)
    {
        TextMeshPro text = Mirror.GetField<DialogueBox, TextMeshPro>(self, "textMesh");

        int pageIndex = Mathf.Clamp(self.currentPage - 1, 0, text.textInfo.pageCount - 1);
        text.maxVisibleCharacters = text.textInfo.pageInfo[pageIndex].lastCharacterIndex + 1;
    }
}
