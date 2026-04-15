using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int MarmuHelperCanvasSortOrder = 10031;
        private const int XeroHelperCanvasSortOrder = 10032;
        private const int MarkothHelperCanvasSortOrder = 10033;
        private const int GalienHelperCanvasSortOrder = 10034;
        private const int GorbHelperCanvasSortOrder = 10035;
        private const int ElderHuHelperCanvasSortOrder = 10036;
        private const int NoEyesHelperCanvasSortOrder = 10037;
        private const int DungDefenderHelperCanvasSortOrder = 10038;
        private const int WhiteDefenderHelperCanvasSortOrder = 10039;
        private const int HiveKnightHelperCanvasSortOrder = 10040;
        private const int BrokenVesselHelperCanvasSortOrder = 10041;
        private const int LostKinHelperCanvasSortOrder = 10042;
        private const int WingedNoskHelperCanvasSortOrder = 10043;
        private const int UumuuHelperCanvasSortOrder = 10044;
        private const int TraitorLordHelperCanvasSortOrder = 10045;
        private const int TroupeMasterGrimmHelperCanvasSortOrder = 10046;
        private const int NightmareKingGrimmHelperCanvasSortOrder = 10047;
        private const int PureVesselHelperCanvasSortOrder = 10048;
        private const int AbsoluteRadianceHelperCanvasSortOrder = 10049;
        private const int PaintmasterSheoHelperCanvasSortOrder = 10050;
        private const int SoulWarriorHelperCanvasSortOrder = 10051;
        private const int NailsageSlyHelperCanvasSortOrder = 10052;
        private const int FlukemarmHelperCanvasSortOrder = 10053;
        private const int VengeflyKingCanvasSortOrder = 10054;
        private const int SoulMasterHelperCanvasSortOrder = 10055;
        private const int SoulTyrantHelperCanvasSortOrder = 10056;
        private const int WatcherKnightHelperCanvasSortOrder = 10057;
        private const int OroMatoHelperCanvasSortOrder = 10058;
        private const int GodTamerHelperCanvasSortOrder = 10059;
        private const int OblobblesHelperCanvasSortOrder = 10060;
        private const int FalseKnightHelperCanvasSortOrder = 10061;
        private const int FailedChampionHelperCanvasSortOrder = 10062;
        private const int SisterOfBattleHelperCanvasSortOrder = 10063;
        private const int MantisLordHelperCanvasSortOrder = 10064;
        private const int NoskHelperCanvasSortOrder = 10065;
        private const float MarmuHelperPanelHeight = PanelHeight;
        private const float XeroHelperPanelHeight = PanelHeight;
        private const float MarkothHelperPanelHeight = PanelHeight;
        private const float GalienHelperPanelHeight = PanelHeight;
        private const float GorbHelperPanelHeight = PanelHeight;
        private const float ElderHuHelperPanelHeight = PanelHeight;
        private const float NoEyesHelperPanelHeight = PanelHeight;
        private const float DungDefenderHelperPanelHeight = PanelHeight;
        private const float WhiteDefenderHelperPanelHeight = PanelHeight;
        private const float HiveKnightHelperPanelHeight = PanelHeight;
        private const float BrokenVesselHelperPanelHeight = PanelHeight;
        private const float LostKinHelperPanelHeight = PanelHeight;
        private const float WingedNoskHelperPanelHeight = PanelHeight;
        private const float UumuuHelperPanelHeight = PanelHeight;
        private const float TraitorLordHelperPanelHeight = PanelHeight;
        private const float TroupeMasterGrimmHelperPanelHeight = PanelHeight;
        private const float NightmareKingGrimmHelperPanelHeight = PanelHeight;
        private const float PureVesselHelperPanelHeight = PanelHeight;
        private const float AbsoluteRadianceHelperPanelHeight = PanelHeight;
        private const float PaintmasterSheoHelperPanelHeight = PanelHeight;
        private const float SoulWarriorHelperPanelHeight = PanelHeight;
        private const float NailsageSlyHelperPanelHeight = PanelHeight;
        private const float FlukemarmHelperPanelHeight = PanelHeight;
        private const float VengeflyKingPanelHeight = PanelHeight;
        private const float SoulMasterHelperPanelHeight = PanelHeight;
        private const float SoulTyrantHelperPanelHeight = PanelHeight;
        private const float WatcherKnightHelperPanelHeight = PanelHeight;
        private const float OroMatoHelperPanelHeight = PanelHeight;
        private const float GodTamerHelperPanelHeight = PanelHeight;
        private const float OblobblesHelperPanelHeight = PanelHeight;
        private const float FalseKnightHelperPanelHeight = PanelHeight;
        private const float FailedChampionHelperPanelHeight = PanelHeight;
        private const float SisterOfBattleHelperPanelHeight = PanelHeight;
        private const float MantisLordHelperPanelHeight = PanelHeight;
        private const float NoskHelperPanelHeight = PanelHeight;
        private const float MarmuHelperRowSpacing = 44f;
        private const float XeroHelperRowSpacing = 44f;
        private const float MarkothHelperRowSpacing = 44f;
        private const float GalienHelperRowSpacing = 44f;
        private const float GorbHelperRowSpacing = 44f;
        private const float ElderHuHelperRowSpacing = 44f;
        private const float NoEyesHelperRowSpacing = 44f;
        private const float DungDefenderHelperRowSpacing = 44f;
        private const float WhiteDefenderHelperRowSpacing = 44f;
        private const float HiveKnightHelperRowSpacing = 44f;
        private const float BrokenVesselHelperRowSpacing = 44f;
        private const float LostKinHelperRowSpacing = 44f;
        private const float WingedNoskHelperRowSpacing = 44f;
        private const float UumuuHelperRowSpacing = 44f;
        private const float TraitorLordHelperRowSpacing = 44f;
        private const float TroupeMasterGrimmHelperRowSpacing = 44f;
        private const float NightmareKingGrimmHelperRowSpacing = 44f;
        private const float PureVesselHelperRowSpacing = 44f;
        private const float AbsoluteRadianceHelperRowSpacing = 44f;
        private const float PaintmasterSheoHelperRowSpacing = 44f;
        private const float SoulWarriorHelperRowSpacing = 44f;
        private const float NailsageSlyHelperRowSpacing = 44f;
        private const float FlukemarmHelperRowSpacing = 44f;
        private const float VengeflyKingRowSpacing = 44f;
        private const float SoulMasterHelperRowSpacing = 44f;
        private const float SoulTyrantHelperRowSpacing = 44f;
        private const float WatcherKnightHelperRowSpacing = 44f;
        private const float OroMatoHelperRowSpacing = 44f;
        private const float GodTamerHelperRowSpacing = 44f;
        private const float OblobblesHelperRowSpacing = 44f;
        private const float FalseKnightHelperRowSpacing = 44f;
        private const float FailedChampionHelperRowSpacing = 44f;
        private const float SisterOfBattleHelperRowSpacing = 44f;
        private const float MantisLordHelperRowSpacing = 44f;
        private const float NoskHelperRowSpacing = 44f;

        private RectTransform? marmuHelperContent;
        private GameObject? marmuHelperRoot;
        private bool marmuHelperVisible;
        private Text? marmuHelperToggleValue;
        private Image? marmuHelperToggleIcon;
        private Text? marmuP5HpValue;
        private Text? marmuUseMaxHpValue;
        private InputField? marmuMaxHpField;
        private Module? marmuHelperModule;

        private RectTransform? xeroHelperContent;
        private GameObject? xeroHelperRoot;
        private bool xeroHelperVisible;
        private Text? xeroHelperToggleValue;
        private Image? xeroHelperToggleIcon;
        private Text? xeroP5HpValue;
        private Text? xeroUseMaxHpValue;
        private Text? xeroUseCustomPhaseValue;
        private InputField? xeroMaxHpField;
        private InputField? xeroPhase2HpField;
        private Module? xeroHelperModule;

        private RectTransform? markothHelperContent;
        private GameObject? markothHelperRoot;
        private bool markothHelperVisible;
        private Text? markothHelperToggleValue;
        private Image? markothHelperToggleIcon;
        private Text? markothP5HpValue;
        private Text? markothUseMaxHpValue;
        private Text? markothUseCustomPhaseValue;
        private InputField? markothMaxHpField;
        private InputField? markothPhase2HpField;
        private Module? markothHelperModule;

        private RectTransform? galienHelperContent;
        private GameObject? galienHelperRoot;
        private bool galienHelperVisible;
        private Text? galienHelperToggleValue;
        private Image? galienHelperToggleIcon;
        private Text? galienP5HpValue;
        private Text? galienUseMaxHpValue;
        private Text? galienUseCustomPhaseValue;
        private InputField? galienMaxHpField;
        private InputField? galienPhase2HpField;
        private InputField? galienPhase3HpField;
        private Module? galienHelperModule;

        private RectTransform? gorbHelperContent;
        private GameObject? gorbHelperRoot;
        private bool gorbHelperVisible;
        private Text? gorbHelperToggleValue;
        private Image? gorbHelperToggleIcon;
        private Text? gorbP5HpValue;
        private Text? gorbUseMaxHpValue;
        private Text? gorbUseCustomPhaseValue;
        private InputField? gorbMaxHpField;
        private InputField? gorbPhase2HpField;
        private InputField? gorbPhase3HpField;
        private Module? gorbHelperModule;

        private RectTransform? elderHuHelperContent;
        private GameObject? elderHuHelperRoot;
        private bool elderHuHelperVisible;
        private Text? elderHuHelperToggleValue;
        private Image? elderHuHelperToggleIcon;
        private Text? elderHuP5HpValue;
        private Text? elderHuUseMaxHpValue;
        private InputField? elderHuMaxHpField;
        private Module? elderHuHelperModule;

        private RectTransform? noEyesHelperContent;
        private GameObject? noEyesHelperRoot;
        private bool noEyesHelperVisible;
        private Text? noEyesHelperToggleValue;
        private Image? noEyesHelperToggleIcon;
        private Text? noEyesP5HpValue;
        private Text? noEyesUseMaxHpValue;
        private Text? noEyesUseCustomPhaseValue;
        private InputField? noEyesMaxHpField;
        private InputField? noEyesPhase2HpField;
        private InputField? noEyesPhase3HpField;
        private Module? noEyesHelperModule;

        private RectTransform? dungDefenderHelperContent;
        private GameObject? dungDefenderHelperRoot;
        private bool dungDefenderHelperVisible;
        private Text? dungDefenderHelperToggleValue;
        private Image? dungDefenderHelperToggleIcon;
        private Text? dungDefenderP5HpValue;
        private Text? dungDefenderUseMaxHpValue;
        private Text? dungDefenderUseCustomPhaseValue;
        private InputField? dungDefenderMaxHpField;
        private InputField? dungDefenderPhase2HpField;
        private Module? dungDefenderHelperModule;

        private RectTransform? whiteDefenderHelperContent;
        private GameObject? whiteDefenderHelperRoot;
        private bool whiteDefenderHelperVisible;
        private Text? whiteDefenderHelperToggleValue;
        private Image? whiteDefenderHelperToggleIcon;
        private Text? whiteDefenderP5HpValue;
        private Text? whiteDefenderUseMaxHpValue;
        private Text? whiteDefenderUseCustomPhaseValue;
        private InputField? whiteDefenderMaxHpField;
        private InputField? whiteDefenderPhase2HpField;
        private Module? whiteDefenderHelperModule;

        private RectTransform? hiveKnightHelperContent;
        private GameObject? hiveKnightHelperRoot;
        private bool hiveKnightHelperVisible;
        private Text? hiveKnightHelperToggleValue;
        private Image? hiveKnightHelperToggleIcon;
        private Text? hiveKnightP5HpValue;
        private Text? hiveKnightUseMaxHpValue;
        private Text? hiveKnightUseCustomPhaseValue;
        private InputField? hiveKnightMaxHpField;
        private InputField? hiveKnightPhase2HpField;
        private InputField? hiveKnightPhase3HpField;
        private Module? hiveKnightHelperModule;

        private RectTransform? brokenVesselHelperContent;
        private GameObject? brokenVesselHelperRoot;
        private bool brokenVesselHelperVisible;
        private Text? brokenVesselHelperToggleValue;
        private Image? brokenVesselHelperToggleIcon;
        private Text? brokenVesselP5HpValue;
        private Text? brokenVesselUseMaxHpValue;
        private Text? brokenVesselUseCustomPhaseValue;
        private Text? brokenVesselUseCustomSummonHpValue;
        private Text? brokenVesselUseCustomSummonLimitValue;
        private InputField? brokenVesselMaxHpField;
        private InputField? brokenVesselPhase2HpField;
        private InputField? brokenVesselPhase3HpField;
        private InputField? brokenVesselPhase4HpField;
        private InputField? brokenVesselPhase5HpField;
        private InputField? brokenVesselSummonHpField;
        private InputField? brokenVesselSummonLimitField;
        private Module? brokenVesselHelperModule;

        private RectTransform? lostKinHelperContent;
        private GameObject? lostKinHelperRoot;
        private bool lostKinHelperVisible;
        private Text? lostKinHelperToggleValue;
        private Image? lostKinHelperToggleIcon;
        private Text? lostKinP5HpValue;
        private Text? lostKinUseMaxHpValue;
        private Text? lostKinUseCustomPhaseValue;
        private Text? lostKinUseCustomSummonHpValue;
        private Text? lostKinUseCustomSummonLimitValue;
        private InputField? lostKinMaxHpField;
        private InputField? lostKinPhase2HpField;
        private InputField? lostKinPhase3HpField;
        private InputField? lostKinPhase4HpField;
        private InputField? lostKinPhase5HpField;
        private InputField? lostKinSummonHpField;
        private InputField? lostKinSummonLimitField;
        private Module? lostKinHelperModule;

        private RectTransform? noskHelperContent;
        private GameObject? noskHelperRoot;
        private bool noskHelperVisible;
        private Text? noskHelperToggleValue;
        private Image? noskHelperToggleIcon;
        private Text? noskUseMaxHpValue;
        private Text? noskUseCustomPhaseValue;
        private InputField? noskMaxHpField;
        private InputField? noskPhase2HpField;
        private Module? noskHelperModule;

        private RectTransform? wingedNoskHelperContent;
        private GameObject? wingedNoskHelperRoot;
        private bool wingedNoskHelperVisible;
        private Text? wingedNoskHelperToggleValue;
        private Image? wingedNoskHelperToggleIcon;
        private Text? wingedNoskP5HpValue;
        private Text? wingedNoskUseMaxHpValue;
        private Text? wingedNoskUseCustomPhaseValue;
        private Text? wingedNoskUseCustomSummonHpValue;
        private Text? wingedNoskUseCustomSummonLimitValue;
        private InputField? wingedNoskMaxHpField;
        private InputField? wingedNoskPhase2HpField;
        private InputField? wingedNoskSummonHpField;
        private InputField? wingedNoskSummonLimitField;
        private Module? wingedNoskHelperModule;

        private RectTransform? uumuuHelperContent;
        private GameObject? uumuuHelperRoot;
        private bool uumuuHelperVisible;
        private Text? uumuuHelperToggleValue;
        private Image? uumuuHelperToggleIcon;
        private Text? uumuuP5HpValue;
        private Text? uumuuUseMaxHpValue;
        private Text? uumuuUseCustomSummonHpValue;
        private InputField? uumuuMaxHpField;
        private InputField? uumuuSummonHpField;
        private Module? uumuuHelperModule;

        private RectTransform? traitorLordHelperContent;
        private GameObject? traitorLordHelperRoot;
        private bool traitorLordHelperVisible;
        private Text? traitorLordHelperToggleValue;
        private Image? traitorLordHelperToggleIcon;
        private Text? traitorLordP5HpValue;
        private Text? traitorLordUseMaxHpValue;
        private Text? traitorLordUseCustomPhaseValue;
        private InputField? traitorLordMaxHpField;
        private InputField? traitorLordPhase2HpField;
        private Module? traitorLordHelperModule;

        private RectTransform? troupeMasterGrimmHelperContent;
        private GameObject? troupeMasterGrimmHelperRoot;
        private bool troupeMasterGrimmHelperVisible;
        private Text? troupeMasterGrimmHelperToggleValue;
        private Image? troupeMasterGrimmHelperToggleIcon;
        private Text? troupeMasterGrimmP5HpValue;
        private Text? troupeMasterGrimmUseMaxHpValue;
        private Text? troupeMasterGrimmUseCustomPhaseValue;
        private InputField? troupeMasterGrimmMaxHpField;
        private InputField? troupeMasterGrimmPhase2HpField;
        private InputField? troupeMasterGrimmPhase3HpField;
        private InputField? troupeMasterGrimmPhase4HpField;
        private Module? troupeMasterGrimmHelperModule;

        private RectTransform? nightmareKingGrimmHelperContent;
        private GameObject? nightmareKingGrimmHelperRoot;
        private bool nightmareKingGrimmHelperVisible;
        private Text? nightmareKingGrimmHelperToggleValue;
        private Image? nightmareKingGrimmHelperToggleIcon;
        private Text? nightmareKingGrimmP5HpValue;
        private Text? nightmareKingGrimmUseMaxHpValue;
        private Text? nightmareKingGrimmUseCustomPhaseValue;
        private InputField? nightmareKingGrimmMaxHpField;
        private InputField? nightmareKingGrimmRagePhase1HpField;
        private InputField? nightmareKingGrimmRagePhase2HpField;
        private InputField? nightmareKingGrimmRagePhase3HpField;
        private Module? nightmareKingGrimmHelperModule;

        private RectTransform? pureVesselHelperContent;
        private GameObject? pureVesselHelperRoot;
        private bool pureVesselHelperVisible;
        private Text? pureVesselHelperToggleValue;
        private Image? pureVesselHelperToggleIcon;
        private Text? pureVesselP5HpValue;
        private Text? pureVesselUseMaxHpValue;
        private Text? pureVesselUseCustomPhaseValue;
        private InputField? pureVesselMaxHpField;
        private InputField? pureVesselPhase2HpField;
        private InputField? pureVesselPhase3HpField;
        private Module? pureVesselHelperModule;

        private RectTransform? absoluteRadianceHelperContent;
        private GameObject? absoluteRadianceHelperRoot;
        private bool absoluteRadianceHelperVisible;
        private Text? absoluteRadianceHelperToggleValue;
        private Image? absoluteRadianceHelperToggleIcon;
        private Text? absoluteRadianceP5HpValue;
        private Text? absoluteRadianceUseMaxHpValue;
        private Text? absoluteRadianceUseCustomPhaseValue;
        private InputField? absoluteRadianceMaxHpField;
        private InputField? absoluteRadiancePhase2HpField;
        private InputField? absoluteRadiancePhase3HpField;
        private InputField? absoluteRadiancePhase4HpField;
        private InputField? absoluteRadiancePhase5HpField;
        private InputField? absoluteRadianceFinalPhaseHpField;
        private Module? absoluteRadianceHelperModule;

        private RectTransform? paintmasterSheoHelperContent;
        private GameObject? paintmasterSheoHelperRoot;
        private bool paintmasterSheoHelperVisible;
        private Text? paintmasterSheoHelperToggleValue;
        private Image? paintmasterSheoHelperToggleIcon;
        private Text? paintmasterSheoP5HpValue;
        private Text? paintmasterSheoUseMaxHpValue;
        private InputField? paintmasterSheoMaxHpField;
        private Module? paintmasterSheoHelperModule;

        private RectTransform? soulWarriorHelperContent;
        private GameObject? soulWarriorHelperRoot;
        private bool soulWarriorHelperVisible;
        private Text? soulWarriorHelperToggleValue;
        private Image? soulWarriorHelperToggleIcon;
        private Text? soulWarriorP5HpValue;
        private Text? soulWarriorUseMaxHpValue;
        private Text? soulWarriorUseCustomSummonHpValue;
        private Text? soulWarriorUseCustomSummonLimitValue;
        private InputField? soulWarriorMaxHpField;
        private InputField? soulWarriorSummonHpField;
        private InputField? soulWarriorSummonLimitField;
        private Module? soulWarriorHelperModule;

        private RectTransform? nailsageSlyHelperContent;
        private GameObject? nailsageSlyHelperRoot;
        private bool nailsageSlyHelperVisible;
        private Text? nailsageSlyHelperToggleValue;
        private Image? nailsageSlyHelperToggleIcon;
        private Text? nailsageSlyP5HpValue;
        private InputField? nailsageSlyPhase1HpField;
        private InputField? nailsageSlyPhase2HpField;
        private Module? nailsageSlyHelperModule;

        private RectTransform? soulMasterHelperContent;
        private GameObject? soulMasterHelperRoot;
        private bool soulMasterHelperVisible;
        private Text? soulMasterHelperToggleValue;
        private Image? soulMasterHelperToggleIcon;
        private Text? soulMasterP5HpValue;
        private InputField? soulMasterPhase1HpField;
        private InputField? soulMasterPhase2HpField;
        private Module? soulMasterHelperModule;

        private RectTransform? soulTyrantHelperContent;
        private GameObject? soulTyrantHelperRoot;
        private bool soulTyrantHelperVisible;
        private Text? soulTyrantHelperToggleValue;
        private Image? soulTyrantHelperToggleIcon;
        private Text? soulTyrantP5HpValue;
        private InputField? soulTyrantPhase1HpField;
        private InputField? soulTyrantPhase2HpField;
        private Module? soulTyrantHelperModule;

        private RectTransform? watcherKnightHelperContent;
        private GameObject? watcherKnightHelperRoot;
        private bool watcherKnightHelperVisible;
        private Text? watcherKnightHelperToggleValue;
        private Image? watcherKnightHelperToggleIcon;
        private Text? watcherKnightP5HpValue;
        private InputField? watcherKnight1HpField;
        private InputField? watcherKnight2HpField;
        private InputField? watcherKnight3HpField;
        private InputField? watcherKnight4HpField;
        private InputField? watcherKnight5HpField;
        private InputField? watcherKnight6HpField;
        private Module? watcherKnightHelperModule;

        private RectTransform? oroMatoHelperContent;
        private GameObject? oroMatoHelperRoot;
        private bool oroMatoHelperVisible;
        private Text? oroMatoHelperToggleValue;
        private Image? oroMatoHelperToggleIcon;
        private Text? oroMatoP5HpValue;
        private InputField? oroMatoOroPhase1HpField;
        private InputField? oroMatoOroPhase2HpField;
        private InputField? oroMatoMatoHpField;
        private Module? oroMatoHelperModule;

        private RectTransform? godTamerHelperContent;
        private GameObject? godTamerHelperRoot;
        private bool godTamerHelperVisible;
        private Text? godTamerHelperToggleValue;
        private Image? godTamerHelperToggleIcon;
        private Text? godTamerP5HpValue;
        private InputField? godTamerLobsterHpField;
        private InputField? godTamerLancerHpField;
        private Module? godTamerHelperModule;

        private RectTransform? oblobblesHelperContent;
        private GameObject? oblobblesHelperRoot;
        private bool oblobblesHelperVisible;
        private Text? oblobblesHelperToggleValue;
        private Image? oblobblesHelperToggleIcon;
        private Text? oblobblesP5HpValue;
        private Text? oblobblesUsePhase2HpValue;
        private InputField? oblobblesLeftPhase1HpField;
        private InputField? oblobblesRightPhase1HpField;
        private InputField? oblobblesPhase2HpField;
        private Module? oblobblesHelperModule;

        private RectTransform? falseKnightHelperContent;
        private GameObject? falseKnightHelperRoot;
        private bool falseKnightHelperVisible;
        private Text? falseKnightHelperToggleValue;
        private Image? falseKnightHelperToggleIcon;
        private Text? falseKnightP5HpValue;
        private InputField? falseKnightArmorPhase1HpField;
        private InputField? falseKnightArmorPhase2HpField;
        private InputField? falseKnightArmorPhase3HpField;
        private Module? falseKnightHelperModule;

        private RectTransform? failedChampionHelperContent;
        private GameObject? failedChampionHelperRoot;
        private bool failedChampionHelperVisible;
        private Text? failedChampionHelperToggleValue;
        private Image? failedChampionHelperToggleIcon;
        private Text? failedChampionP5HpValue;
        private InputField? failedChampionArmorPhase1HpField;
        private InputField? failedChampionArmorPhase2HpField;
        private InputField? failedChampionArmorPhase3HpField;
        private Module? failedChampionHelperModule;

        private RectTransform? sisterOfBattleHelperContent;
        private GameObject? sisterOfBattleHelperRoot;
        private bool sisterOfBattleHelperVisible;
        private Text? sisterOfBattleHelperToggleValue;
        private Image? sisterOfBattleHelperToggleIcon;
        private Text? sisterOfBattleP5HpValue;
        private InputField? sisterOfBattlePhase1HpField;
        private InputField? sisterOfBattlePhase2S1HpField;
        private InputField? sisterOfBattlePhase2S2HpField;
        private InputField? sisterOfBattlePhase2S3HpField;
        private Module? sisterOfBattleHelperModule;

        private RectTransform? mantisLordHelperContent;
        private GameObject? mantisLordHelperRoot;
        private bool mantisLordHelperVisible;
        private Text? mantisLordHelperToggleValue;
        private Image? mantisLordHelperToggleIcon;
        private InputField? mantisLordPhase1HpField;
        private InputField? mantisLordPhase2S1HpField;
        private InputField? mantisLordPhase2S2HpField;
        private Module? mantisLordHelperModule;

        private RectTransform? flukemarmHelperContent;
        private GameObject? flukemarmHelperRoot;
        private bool flukemarmHelperVisible;
        private Text? flukemarmHelperToggleValue;
        private Image? flukemarmHelperToggleIcon;
        private Text? flukemarmP5HpValue;
        private Text? flukemarmUseMaxHpValue;
        private Text? flukemarmUseCustomSummonLimitValue;
        private InputField? flukemarmMaxHpField;
        private InputField? flukemarmFlyHpField;
        private InputField? flukemarmSummonLimitField;
        private Module? flukemarmHelperModule;

        private RectTransform? vengeflyKingContent;
        private GameObject? vengeflyKingRoot;
        private bool vengeflyKingVisible;
        private Text? vengeflyKingToggleValue;
        private Image? vengeflyKingToggleIcon;
        private Text? vengeflyKingP5HpValue;
        private Text? vengeflyKingUseMaxHpValue;
        private Text? vengeflyKingUseCustomSummonLimitValue;
        private InputField? vengeflyKingLeftMaxHpField;
        private InputField? vengeflyKingRightMaxHpField;
        private InputField? vengeflyKingSummonMaxHpField;
        private InputField? vengeflyKingLeftSummonLimitField;
        private InputField? vengeflyKingRightSummonLimitField;
        private InputField? vengeflyKingLeftSummonAttackLimitField;
        private InputField? vengeflyKingRightSummonAttackLimitField;
        private Module? vengeflyKingModule;

        private static void ResetMarmuHelperDefaults()
        {
            Modules.BossChallenge.MarmuHelper.marmuUseMaxHp = false;
            Modules.BossChallenge.MarmuHelper.marmuP5Hp = false;
            Modules.BossChallenge.MarmuHelper.marmuMaxHp = 600;
            Modules.BossChallenge.MarmuHelper.marmuMaxHpBeforeP5 = 600;
            Modules.BossChallenge.MarmuHelper.marmuUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.MarmuHelper.marmuHasStoredStateBeforeP5 = false;
        }

        private static void ResetXeroHelperDefaults()
        {
            Modules.BossChallenge.XeroHelper.xeroUseMaxHp = false;
            Modules.BossChallenge.XeroHelper.xeroP5Hp = false;
            Modules.BossChallenge.XeroHelper.xeroUseCustomPhase = false;
            Modules.BossChallenge.XeroHelper.xeroMaxHp = 900;
            Modules.BossChallenge.XeroHelper.xeroPhase2Hp = 450;
            Modules.BossChallenge.XeroHelper.xeroMaxHpBeforeP5 = 900;
            Modules.BossChallenge.XeroHelper.xeroUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.XeroHelper.xeroUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.XeroHelper.xeroPhase2HpBeforeP5 = 450;
            Modules.BossChallenge.XeroHelper.xeroHasStoredStateBeforeP5 = false;
        }

        private static void ResetMarkothHelperDefaults()
        {
            Modules.BossChallenge.MarkothHelper.markothUseMaxHp = false;
            Modules.BossChallenge.MarkothHelper.markothP5Hp = false;
            Modules.BossChallenge.MarkothHelper.markothUseCustomPhase = false;
            Modules.BossChallenge.MarkothHelper.markothMaxHp = 950;
            Modules.BossChallenge.MarkothHelper.markothPhase2Hp = 475;
            Modules.BossChallenge.MarkothHelper.markothMaxHpBeforeP5 = 950;
            Modules.BossChallenge.MarkothHelper.markothUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.MarkothHelper.markothUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.MarkothHelper.markothPhase2HpBeforeP5 = 475;
            Modules.BossChallenge.MarkothHelper.markothHasStoredStateBeforeP5 = false;
        }

        private static void ResetGalienHelperDefaults()
        {
            Modules.BossChallenge.GalienHelper.galienUseMaxHp = false;
            Modules.BossChallenge.GalienHelper.galienP5Hp = false;
            Modules.BossChallenge.GalienHelper.galienUseCustomPhase = false;
            Modules.BossChallenge.GalienHelper.galienMaxHp = 1000;
            Modules.BossChallenge.GalienHelper.galienPhase2Hp = 700;
            Modules.BossChallenge.GalienHelper.galienPhase3Hp = 400;
            Modules.BossChallenge.GalienHelper.galienMaxHpBeforeP5 = 1000;
            Modules.BossChallenge.GalienHelper.galienUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.GalienHelper.galienHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.GalienHelper.galienUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.GalienHelper.galienPhase2HpBeforeP5 = 700;
            Modules.BossChallenge.GalienHelper.galienPhase3HpBeforeP5 = 400;
        }

        private static void ResetGorbHelperDefaults()
        {
            Modules.BossChallenge.GorbHelper.gorbUseMaxHp = false;
            Modules.BossChallenge.GorbHelper.gorbP5Hp = false;
            Modules.BossChallenge.GorbHelper.gorbUseCustomPhase = false;
            Modules.BossChallenge.GorbHelper.gorbMaxHp = 1000;
            Modules.BossChallenge.GorbHelper.gorbPhase2Hp = 700;
            Modules.BossChallenge.GorbHelper.gorbPhase3Hp = 400;
            Modules.BossChallenge.GorbHelper.gorbMaxHpBeforeP5 = 1000;
            Modules.BossChallenge.GorbHelper.gorbUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.GorbHelper.gorbHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.GorbHelper.gorbUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.GorbHelper.gorbPhase2HpBeforeP5 = 700;
            Modules.BossChallenge.GorbHelper.gorbPhase3HpBeforeP5 = 400;
        }

        private static void ResetElderHuHelperDefaults()
        {
            Modules.BossChallenge.ElderHuHelper.elderHuUseMaxHp = false;
            Modules.BossChallenge.ElderHuHelper.elderHuP5Hp = false;
            Modules.BossChallenge.ElderHuHelper.elderHuMaxHp = 800;
            Modules.BossChallenge.ElderHuHelper.elderHuMaxHpBeforeP5 = 800;
            Modules.BossChallenge.ElderHuHelper.elderHuUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.ElderHuHelper.elderHuHasStoredStateBeforeP5 = false;
        }

        private static void ResetNoEyesHelperDefaults()
        {
            Modules.BossChallenge.NoEyesHelper.noEyesUseMaxHp = false;
            Modules.BossChallenge.NoEyesHelper.noEyesP5Hp = false;
            Modules.BossChallenge.NoEyesHelper.noEyesMaxHp = 800;
            Modules.BossChallenge.NoEyesHelper.noEyesMaxHpBeforeP5 = 800;
            Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase = false;
            Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp = 150;
            Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp = 90;
            Modules.BossChallenge.NoEyesHelper.noEyesUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.NoEyesHelper.noEyesHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.NoEyesHelper.noEyesPhase2HpBeforeP5 = 150;
            Modules.BossChallenge.NoEyesHelper.noEyesPhase3HpBeforeP5 = 90;
        }

        private static void ResetDungDefenderHelperDefaults()
        {
            Modules.BossChallenge.DungDefenderHelper.dungDefenderUseMaxHp = false;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderP5Hp = false;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderMaxHp = 1100;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderMaxHpBeforeP5 = 1100;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhase = false;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2Hp = 350;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2HpBeforeP5 = 350;
        }

        private static void ResetWhiteDefenderHelperDefaults()
        {
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseMaxHp = false;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderP5Hp = false;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderMaxHp = 1600;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderMaxHpBeforeP5 = 1600;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseCustomPhase = false;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderPhase2Hp = 600;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderPhase2HpBeforeP5 = 600;
        }

        private static void ResetHiveKnightHelperDefaults()
        {
            Modules.BossChallenge.HiveKnightHelper.hiveKnightUseMaxHp = false;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp = false;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightMaxHp = 1300;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase = false;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase2Hp = 580;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase3Hp = 350;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightMaxHpBeforeP5 = 1300;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase2HpBeforeP5 = 580;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase3HpBeforeP5 = 350;
        }

        private static void ResetBrokenVesselHelperDefaults()
        {
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseMaxHp = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselMaxHp = 1000;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonHp = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonHp = 1;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonLimit = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonLimit = 3;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp = 420;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp = 370;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp = 220;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp = 110;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselMaxHpBeforeP5 = 1000;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonHpBeforeP5 = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonHpBeforeP5 = 1;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonLimitBeforeP5 = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonLimitBeforeP5 = 3;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2HpBeforeP5 = 420;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3HpBeforeP5 = 370;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4HpBeforeP5 = 220;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5HpBeforeP5 = 110;
        }

        private static void ResetLostKinHelperDefaults()
        {
            Modules.BossChallenge.LostKinHelper.lostKinUseMaxHp = false;
            Modules.BossChallenge.LostKinHelper.lostKinP5Hp = false;
            Modules.BossChallenge.LostKinHelper.lostKinMaxHp = 1650;
            Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonHp = false;
            Modules.BossChallenge.LostKinHelper.lostKinSummonHp = 1;
            Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonLimit = false;
            Modules.BossChallenge.LostKinHelper.lostKinSummonLimit = 5;
            Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase = false;
            Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp = 1150;
            Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp = 550;
            Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp = 350;
            Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp = 175;
            Modules.BossChallenge.LostKinHelper.lostKinMaxHpBeforeP5 = 1650;
            Modules.BossChallenge.LostKinHelper.lostKinUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.LostKinHelper.lostKinHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonHpBeforeP5 = false;
            Modules.BossChallenge.LostKinHelper.lostKinSummonHpBeforeP5 = 1;
            Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonLimitBeforeP5 = false;
            Modules.BossChallenge.LostKinHelper.lostKinSummonLimitBeforeP5 = 5;
            Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.LostKinHelper.lostKinPhase2HpBeforeP5 = 1150;
            Modules.BossChallenge.LostKinHelper.lostKinPhase3HpBeforeP5 = 550;
            Modules.BossChallenge.LostKinHelper.lostKinPhase4HpBeforeP5 = 350;
            Modules.BossChallenge.LostKinHelper.lostKinPhase5HpBeforeP5 = 175;
        }

        private static void ResetNoskHelperDefaults()
        {
            Modules.BossChallenge.NoskHelper.noskUseMaxHp = false;
            Modules.BossChallenge.NoskHelper.noskUseCustomPhase = false;
            Modules.BossChallenge.NoskHelper.noskMaxHp = 980;
            Modules.BossChallenge.NoskHelper.noskPhase2Hp = 560;
        }

        private static void ResetWingedNoskHelperDefaults()
        {
            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseMaxHp = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomPhase = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonHp = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonLimit = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskMaxHp = 1050;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2Hp = 525;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonHp = 1;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonLimit = 5;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskMaxHpBeforeP5 = 1050;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2HpBeforeP5 = 525;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonHpBeforeP5 = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonHpBeforeP5 = 1;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonLimitBeforeP5 = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonLimitBeforeP5 = 5;
        }

        private static void ResetUumuuHelperDefaults()
        {
            Modules.BossChallenge.UumuuHelper.uumuuUseMaxHp = false;
            Modules.BossChallenge.UumuuHelper.uumuuP5Hp = false;
            Modules.BossChallenge.UumuuHelper.uumuuUseCustomSummonHp = false;
            Modules.BossChallenge.UumuuHelper.uumuuMaxHp = 700;
            Modules.BossChallenge.UumuuHelper.uumuuSummonHp = 1;
            Modules.BossChallenge.UumuuHelper.uumuuMaxHpBeforeP5 = 700;
            Modules.BossChallenge.UumuuHelper.uumuuUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.UumuuHelper.uumuuUseCustomSummonHpBeforeP5 = false;
            Modules.BossChallenge.UumuuHelper.uumuuSummonHpBeforeP5 = 1;
            Modules.BossChallenge.UumuuHelper.uumuuHasStoredStateBeforeP5 = false;
        }

        private static void ResetTraitorLordHelperDefaults()
        {
            Modules.BossChallenge.TraitorLordHelper.traitorLordUseMaxHp = false;
            Modules.BossChallenge.TraitorLordHelper.traitorLordP5Hp = false;
            Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhase = false;
            Modules.BossChallenge.TraitorLordHelper.traitorLordMaxHp = 1300;
            Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp = 500;
            Modules.BossChallenge.TraitorLordHelper.traitorLordMaxHpBeforeP5 = 1300;
            Modules.BossChallenge.TraitorLordHelper.traitorLordUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2HpBeforeP5 = 500;
            Modules.BossChallenge.TraitorLordHelper.traitorLordHasStoredStateBeforeP5 = false;
        }

        private static void ResetTroupeMasterGrimmHelperDefaults()
        {
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseMaxHp = false;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp = false;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase = false;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmMaxHp = 1000;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp = 750;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp = 500;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp = 250;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmMaxHpBeforeP5 = 1000;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2HpBeforeP5 = 750;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3HpBeforeP5 = 500;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4HpBeforeP5 = 250;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmHasStoredStateBeforeP5 = false;
        }

        private static void ResetNightmareKingGrimmHelperDefaults()
        {
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseMaxHp = false;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp = false;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase = false;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmMaxHp = 1650;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1Hp = 1238;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2Hp = 826;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3Hp = 414;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmMaxHpBeforeP5 = 1650;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1HpBeforeP5 = 1238;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2HpBeforeP5 = 826;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3HpBeforeP5 = 414;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmHasStoredStateBeforeP5 = false;
        }

        private static void ResetPureVesselHelperDefaults()
        {
            Modules.BossChallenge.PureVesselHelper.pureVesselUseMaxHp = false;
            Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp = false;
            Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase = false;
            Modules.BossChallenge.PureVesselHelper.pureVesselMaxHp = 1850;
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase2Hp = 1232;
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase3Hp = 616;
            Modules.BossChallenge.PureVesselHelper.pureVesselMaxHpBeforeP5 = 1850;
            Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase2HpBeforeP5 = 1232;
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase3HpBeforeP5 = 616;
            Modules.BossChallenge.PureVesselHelper.pureVesselUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.PureVesselHelper.pureVesselHasStoredStateBeforeP5 = false;
        }

        private static void ResetAbsoluteRadianceHelperDefaults()
        {
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseMaxHp = false;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp = false;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase = false;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp = 3000;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp = 2600;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp = 2150;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp = 1850;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp = 1100;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHp = 1000;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHpBeforeP5 = 3000;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2HpBeforeP5 = 2600;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3HpBeforeP5 = 2150;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4HpBeforeP5 = 1850;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5HpBeforeP5 = 1100;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHpBeforeP5 = 1000;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceHasStoredStateBeforeP5 = false;
        }

        private static void ResetPaintmasterSheoHelperDefaults()
        {
            Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoUseMaxHp = false;
            Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoP5Hp = false;
            Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoMaxHp = 1450;
            Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoMaxHpBeforeP5 = 1450;
            Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoHasStoredStateBeforeP5 = false;
        }

        private static void ResetSoulWarriorHelperDefaults()
        {
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseMaxHp = false;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp = false;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonHp = false;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonLimit = false;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorMaxHp = 1000;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonHp = 13;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonLimit = 36;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorMaxHpBeforeP5 = 1000;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonHpBeforeP5 = false;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonHpBeforeP5 = 13;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonLimitBeforeP5 = false;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonLimitBeforeP5 = 36;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorHasStoredStateBeforeP5 = false;
        }

        private static void ResetNailsageSlyHelperDefaults()
        {
            Modules.BossChallenge.NailsageSlyHelper.nailsageSlyP5Hp = false;
            Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase1Hp = 1200;
            Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase2Hp = 600;
            Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase1HpBeforeP5 = 1200;
            Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase2HpBeforeP5 = 600;
            Modules.BossChallenge.NailsageSlyHelper.nailsageSlyHasStoredStateBeforeP5 = false;
        }

        private static void ResetSoulMasterHelperDefaults()
        {
            Modules.BossChallenge.SoulMasterHelper.soulMasterP5Hp = false;
            Modules.BossChallenge.SoulMasterHelper.soulMasterPhase1Hp = 900;
            Modules.BossChallenge.SoulMasterHelper.soulMasterPhase2Hp = 600;
            Modules.BossChallenge.SoulMasterHelper.soulMasterPhase1HpBeforeP5 = 900;
            Modules.BossChallenge.SoulMasterHelper.soulMasterPhase2HpBeforeP5 = 600;
            Modules.BossChallenge.SoulMasterHelper.soulMasterHasStoredStateBeforeP5 = false;
        }

        private static void ResetSoulTyrantHelperDefaults()
        {
            Modules.BossChallenge.SoulTyrantHelper.soulTyrantP5Hp = false;
            Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase1Hp = 1200;
            Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase2Hp = 650;
            Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase1HpBeforeP5 = 1200;
            Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase2HpBeforeP5 = 650;
            Modules.BossChallenge.SoulTyrantHelper.soulTyrantHasStoredStateBeforeP5 = false;
        }

        private static void ResetWatcherKnightHelperDefaults()
        {
            Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp = false;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight1Hp = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight2Hp = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight3Hp = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight4Hp = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight5Hp = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight6Hp = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight1HpBeforeP5 = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight2HpBeforeP5 = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight3HpBeforeP5 = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight4HpBeforeP5 = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight5HpBeforeP5 = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight6HpBeforeP5 = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnightHasStoredStateBeforeP5 = false;
        }

        private static void ResetOroMatoHelperDefaults()
        {
            Modules.BossChallenge.OroMatoHelper.oroMatoP5Hp = false;
            Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase1Hp = 800;
            Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase2Hp = 1000;
            Modules.BossChallenge.OroMatoHelper.oroMatoMatoHp = 1000;
            Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase1HpBeforeP5 = 800;
            Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase2HpBeforeP5 = 1000;
            Modules.BossChallenge.OroMatoHelper.oroMatoMatoHpBeforeP5 = 1000;
            Modules.BossChallenge.OroMatoHelper.oroMatoHasStoredStateBeforeP5 = false;
        }

        private static void ResetGodTamerHelperDefaults()
        {
            Modules.BossChallenge.GodTamerHelper.godTamerP5Hp = false;
            Modules.BossChallenge.GodTamerHelper.godTamerLobsterHp = 1000;
            Modules.BossChallenge.GodTamerHelper.godTamerLancerHp = 1000;
            Modules.BossChallenge.GodTamerHelper.godTamerLobsterHpBeforeP5 = 1000;
            Modules.BossChallenge.GodTamerHelper.godTamerLancerHpBeforeP5 = 1000;
            Modules.BossChallenge.GodTamerHelper.godTamerHasStoredStateBeforeP5 = false;
        }

        private static void ResetOblobblesHelperDefaults()
        {
            Modules.BossChallenge.OblobblesHelper.oblobblesP5Hp = false;
            Modules.BossChallenge.OblobblesHelper.oblobblesLeftPhase1Hp = 750;
            Modules.BossChallenge.OblobblesHelper.oblobblesRightPhase1Hp = 750;
            Modules.BossChallenge.OblobblesHelper.oblobblesUsePhase2Hp = false;
            Modules.BossChallenge.OblobblesHelper.oblobblesPhase2Hp = 750;
            Modules.BossChallenge.OblobblesHelper.oblobblesLeftPhase1HpBeforeP5 = 750;
            Modules.BossChallenge.OblobblesHelper.oblobblesRightPhase1HpBeforeP5 = 750;
            Modules.BossChallenge.OblobblesHelper.oblobblesUsePhase2HpBeforeP5 = false;
            Modules.BossChallenge.OblobblesHelper.oblobblesPhase2HpBeforeP5 = 750;
            Modules.BossChallenge.OblobblesHelper.oblobblesHasStoredStateBeforeP5 = false;
        }

        private static void ResetFalseKnightHelperDefaults()
        {
            Modules.BossChallenge.FalseKnightHelper.falseKnightP5Hp = false;
            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase1Hp = 560;
            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase2Hp = 560;
            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase3Hp = 560;
            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase1HpBeforeP5 = 560;
            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase2HpBeforeP5 = 560;
            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase3HpBeforeP5 = 560;
            Modules.BossChallenge.FalseKnightHelper.falseKnightHasStoredStateBeforeP5 = false;
        }

        private static void ResetFailedChampionHelperDefaults()
        {
            Modules.BossChallenge.FailedChampionHelper.failedChampionP5Hp = false;
            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase1Hp = 600;
            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase2Hp = 600;
            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase3Hp = 600;
            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase1HpBeforeP5 = 600;
            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase2HpBeforeP5 = 600;
            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase3HpBeforeP5 = 600;
            Modules.BossChallenge.FailedChampionHelper.failedChampionHasStoredStateBeforeP5 = false;
        }

        private static void ResetSisterOfBattleHelperDefaults()
        {
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleP5Hp = false;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase1Hp = 600;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S1Hp = 950;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S2Hp = 950;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S3Hp = 950;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase1HpBeforeP5 = 600;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S1HpBeforeP5 = 950;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S2HpBeforeP5 = 950;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S3HpBeforeP5 = 950;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleHasStoredStateBeforeP5 = false;
        }

        private static void ResetMantisLordHelperDefaults()
        {
            Modules.BossChallenge.MantisLordHelper.mantisLordPhase1Hp = 500;
            Modules.BossChallenge.MantisLordHelper.mantisLordPhase2S1Hp = 600;
            Modules.BossChallenge.MantisLordHelper.mantisLordPhase2S2Hp = 600;
        }

        private static void ResetFlukemarmHelperDefaults()
        {
            Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHp = false;
            Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp = false;
            Modules.BossChallenge.FlukemarmHelper.flukemarmUseCustomSummonLimit = false;
            Modules.BossChallenge.FlukemarmHelper.flukemarmMaxHp = 900;
            Modules.BossChallenge.FlukemarmHelper.flukemarmFlyHp = 35;
            Modules.BossChallenge.FlukemarmHelper.flukemarmSummonLimit = 6;
            Modules.BossChallenge.FlukemarmHelper.flukemarmMaxHpBeforeP5 = 900;
            Modules.BossChallenge.FlukemarmHelper.flukemarmFlyHpBeforeP5 = 35;
            Modules.BossChallenge.FlukemarmHelper.flukemarmUseCustomSummonLimitBeforeP5 = false;
            Modules.BossChallenge.FlukemarmHelper.flukemarmSummonLimitBeforeP5 = 6;
            Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.FlukemarmHelper.flukemarmHasStoredStateBeforeP5 = false;
        }

        private static void ResetVengeflyKingDefaults()
        {
            Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp = false;
            Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp = false;
            Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit = false;
            Modules.BossChallenge.VengeflyKing.vengeflyKingLeftMaxHp = 750;
            Modules.BossChallenge.VengeflyKing.vengeflyKingRightMaxHp = 430;
            Modules.BossChallenge.VengeflyKing.vengeflyKingSummonMaxHp = 8;
            Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonLimit = 4;
            Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonLimit = 4;
            Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonAttackLimit = 15;
            Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonAttackLimit = 15;
            Modules.BossChallenge.VengeflyKing.vengeflyKingLeftMaxHpBeforeP5 = 750;
            Modules.BossChallenge.VengeflyKing.vengeflyKingRightMaxHpBeforeP5 = 430;
            Modules.BossChallenge.VengeflyKing.vengeflyKingSummonMaxHpBeforeP5 = 8;
            Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimitBeforeP5 = false;
            Modules.BossChallenge.VengeflyKing.vengeflyKingHasStoredStateBeforeP5 = false;
        }

        private static void ResetAdditionalGhostHelperDefaultsGlobal()
        {
            ResetMarmuHelperDefaults();
            ResetXeroHelperDefaults();
            ResetMarkothHelperDefaults();
            ResetGalienHelperDefaults();
            ResetGorbHelperDefaults();
            ResetElderHuHelperDefaults();
            ResetNoEyesHelperDefaults();
            ResetDungDefenderHelperDefaults();
            ResetWhiteDefenderHelperDefaults();
            ResetHiveKnightHelperDefaults();
            ResetBrokenVesselHelperDefaults();
            ResetLostKinHelperDefaults();
            ResetNoskHelperDefaults();
            ResetWingedNoskHelperDefaults();
            ResetUumuuHelperDefaults();
            ResetTraitorLordHelperDefaults();
            ResetTroupeMasterGrimmHelperDefaults();
            ResetNightmareKingGrimmHelperDefaults();
            ResetPureVesselHelperDefaults();
            ResetAbsoluteRadianceHelperDefaults();
            ResetPaintmasterSheoHelperDefaults();
            ResetSoulWarriorHelperDefaults();
            ResetNailsageSlyHelperDefaults();
            ResetSoulMasterHelperDefaults();
            ResetSoulTyrantHelperDefaults();
            ResetWatcherKnightHelperDefaults();
            ResetOroMatoHelperDefaults();
            ResetGodTamerHelperDefaults();
            ResetOblobblesHelperDefaults();
            ResetFalseKnightHelperDefaults();
            ResetFailedChampionHelperDefaults();
            ResetSisterOfBattleHelperDefaults();
            ResetMantisLordHelperDefaults();
            ResetFlukemarmHelperDefaults();
            ResetVengeflyKingDefaults();
        }

        private static void SetAdditionalGhostHelpersModulesEnabled(bool value)
        {
            SetModuleEnabled<Modules.BossChallenge.MarmuHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.XeroHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.MarkothHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.GalienHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.GorbHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.ElderHuHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.NoEyesHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.DungDefenderHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.WhiteDefenderHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.HiveKnightHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.BrokenVesselHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.LostKinHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.NoskHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.WingedNoskHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.UumuuHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.TraitorLordHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.TroupeMasterGrimmHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.NightmareKingGrimmHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.PureVesselHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.AbsoluteRadianceHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.PaintmasterSheoHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.SoulWarriorHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.NailsageSlyHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.SoulMasterHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.SoulTyrantHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.WatcherKnightHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.OroMatoHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.GodTamerHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.OblobblesHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.FalseKnightHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.FailedChampionHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.SisterOfBattleHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.MantisLordHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.FlukemarmHelper>(value);
            SetModuleEnabled<Modules.BossChallenge.VengeflyKing>(value);
        }

        private bool GetAdditionalGhostHelpersEnabled()
        {
            return GetMarmuHelperEnabled()
                || GetXeroHelperEnabled()
                || GetMarkothHelperEnabled()
                || GetGalienHelperEnabled()
                || GetGorbHelperEnabled()
                || GetElderHuHelperEnabled()
                || GetNoEyesHelperEnabled()
                || GetDungDefenderHelperEnabled()
                || GetWhiteDefenderHelperEnabled()
                || GetHiveKnightHelperEnabled()
                || GetBrokenVesselHelperEnabled()
                || GetLostKinHelperEnabled()
                || GetNoskHelperEnabled()
                || GetWingedNoskHelperEnabled()
                || GetUumuuHelperEnabled()
                || GetTraitorLordHelperEnabled()
                || GetTroupeMasterGrimmHelperEnabled()
                || GetNightmareKingGrimmHelperEnabled()
                || GetPureVesselHelperEnabled()
                || GetAbsoluteRadianceHelperEnabled()
                || GetPaintmasterSheoHelperEnabled()
                || GetSoulWarriorHelperEnabled()
                || GetNailsageSlyHelperEnabled()
                || GetSoulMasterHelperEnabled()
                || GetSoulTyrantHelperEnabled()
                || GetWatcherKnightHelperEnabled()
                || GetOroMatoHelperEnabled()
                || GetGodTamerHelperEnabled()
                || GetOblobblesHelperEnabled()
                || GetFalseKnightHelperEnabled()
                || GetFailedChampionHelperEnabled()
                || GetSisterOfBattleHelperEnabled()
                || GetMantisLordHelperEnabled()
                || GetFlukemarmHelperEnabled()
                || GetVengeflyKingEnabled()
                ;
        }

        private bool IsAnyAdditionalGhostHelperVisible()
        {
            return marmuHelperVisible
                || xeroHelperVisible
                || markothHelperVisible
                || galienHelperVisible
                || gorbHelperVisible
                || elderHuHelperVisible
                || noEyesHelperVisible
                || dungDefenderHelperVisible
                || whiteDefenderHelperVisible
                || hiveKnightHelperVisible
                || brokenVesselHelperVisible
                || lostKinHelperVisible
                || noskHelperVisible
                || wingedNoskHelperVisible
                || uumuuHelperVisible
                || traitorLordHelperVisible
                || troupeMasterGrimmHelperVisible
                || nightmareKingGrimmHelperVisible
                || pureVesselHelperVisible
                || absoluteRadianceHelperVisible
                || paintmasterSheoHelperVisible
                || soulWarriorHelperVisible
                || nailsageSlyHelperVisible
                || soulMasterHelperVisible
                || soulTyrantHelperVisible
                || watcherKnightHelperVisible
                || oroMatoHelperVisible
                || godTamerHelperVisible
                || oblobblesHelperVisible
                || falseKnightHelperVisible
                || failedChampionHelperVisible
                || sisterOfBattleHelperVisible
                || mantisLordHelperVisible
                || flukemarmHelperVisible
                || vengeflyKingVisible
                ;
        }

        private void SetAllAdditionalGhostHelpersVisible(bool value)
        {
            SetMarmuHelperVisible(value);
            SetXeroHelperVisible(value);
            SetMarkothHelperVisible(value);
            SetGalienHelperVisible(value);
            SetGorbHelperVisible(value);
            SetElderHuHelperVisible(value);
            SetNoEyesHelperVisible(value);
            SetDungDefenderHelperVisible(value);
            SetWhiteDefenderHelperVisible(value);
            SetHiveKnightHelperVisible(value);
            SetBrokenVesselHelperVisible(value);
            SetLostKinHelperVisible(value);
            SetNoskHelperVisible(value);
            SetWingedNoskHelperVisible(value);
            SetUumuuHelperVisible(value);
            SetTraitorLordHelperVisible(value);
            SetTroupeMasterGrimmHelperVisible(value);
            SetNightmareKingGrimmHelperVisible(value);
            SetPureVesselHelperVisible(value);
            SetAbsoluteRadianceHelperVisible(value);
            SetPaintmasterSheoHelperVisible(value);
            SetSoulWarriorHelperVisible(value);
            SetNailsageSlyHelperVisible(value);
            SetSoulMasterHelperVisible(value);
            SetSoulTyrantHelperVisible(value);
            SetWatcherKnightHelperVisible(value);
            SetOroMatoHelperVisible(value);
            SetGodTamerHelperVisible(value);
            SetOblobblesHelperVisible(value);
            SetFalseKnightHelperVisible(value);
            SetFailedChampionHelperVisible(value);
            SetSisterOfBattleHelperVisible(value);
            SetMantisLordHelperVisible(value);
            SetFlukemarmHelperVisible(value);
            SetVengeflyKingVisible(value);
        }

        private void SetAdditionalGhostHelpersEnabled(bool value)
        {
            SetMarmuHelperEnabled(value);
            SetXeroHelperEnabled(value);
            SetMarkothHelperEnabled(value);
            SetGalienHelperEnabled(value);
            SetGorbHelperEnabled(value);
            SetElderHuHelperEnabled(value);
            SetNoEyesHelperEnabled(value);
            SetDungDefenderHelperEnabled(value);
            SetWhiteDefenderHelperEnabled(value);
            SetHiveKnightHelperEnabled(value);
            SetBrokenVesselHelperEnabled(value);
            SetLostKinHelperEnabled(value);
            SetNoskHelperEnabled(value);
            SetWingedNoskHelperEnabled(value);
            SetUumuuHelperEnabled(value);
            SetTraitorLordHelperEnabled(value);
            SetTroupeMasterGrimmHelperEnabled(value);
            SetNightmareKingGrimmHelperEnabled(value);
            SetPureVesselHelperEnabled(value);
            SetAbsoluteRadianceHelperEnabled(value);
            SetPaintmasterSheoHelperEnabled(value);
            SetSoulWarriorHelperEnabled(value);
            SetNailsageSlyHelperEnabled(value);
            SetSoulMasterHelperEnabled(value);
            SetSoulTyrantHelperEnabled(value);
            SetWatcherKnightHelperEnabled(value);
            SetOroMatoHelperEnabled(value);
            SetGodTamerHelperEnabled(value);
            SetOblobblesHelperEnabled(value);
            SetFalseKnightHelperEnabled(value);
            SetFailedChampionHelperEnabled(value);
            SetSisterOfBattleHelperEnabled(value);
            SetMantisLordHelperEnabled(value);
            SetFlukemarmHelperEnabled(value);
            SetVengeflyKingEnabled(value);
        }

        private void RefreshAdditionalGhostHelpersUi()
        {
            RefreshMarmuHelperUi();
            RefreshXeroHelperUi();
            RefreshMarkothHelperUi();
            RefreshGalienHelperUi();
            RefreshGorbHelperUi();
            RefreshElderHuHelperUi();
            RefreshNoEyesHelperUi();
            RefreshDungDefenderHelperUi();
            RefreshWhiteDefenderHelperUi();
            RefreshHiveKnightHelperUi();
            RefreshBrokenVesselHelperUi();
            RefreshLostKinHelperUi();
            RefreshNoskHelperUi();
            RefreshWingedNoskHelperUi();
            RefreshUumuuHelperUi();
            RefreshTraitorLordHelperUi();
            RefreshTroupeMasterGrimmHelperUi();
            RefreshNightmareKingGrimmHelperUi();
            RefreshPureVesselHelperUi();
            RefreshAbsoluteRadianceHelperUi();
            RefreshPaintmasterSheoHelperUi();
            RefreshSoulWarriorHelperUi();
            RefreshNailsageSlyHelperUi();
            RefreshSoulMasterHelperUi();
            RefreshSoulTyrantHelperUi();
            RefreshWatcherKnightHelperUi();
            RefreshOroMatoHelperUi();
            RefreshGodTamerHelperUi();
            RefreshOblobblesHelperUi();
            RefreshFalseKnightHelperUi();
            RefreshFailedChampionHelperUi();
            RefreshSisterOfBattleHelperUi();
            RefreshMantisLordHelperUi();
            RefreshFlukemarmHelperUi();
            RefreshVengeflyKingUi();
        }

        private void DestroyAdditionalGhostHelperRoots()
        {
            DestroyRoot(ref marmuHelperRoot);
            DestroyRoot(ref xeroHelperRoot);
            DestroyRoot(ref markothHelperRoot);
            DestroyRoot(ref galienHelperRoot);
            DestroyRoot(ref gorbHelperRoot);
            DestroyRoot(ref elderHuHelperRoot);
            DestroyRoot(ref noEyesHelperRoot);
            DestroyRoot(ref dungDefenderHelperRoot);
            DestroyRoot(ref whiteDefenderHelperRoot);
            DestroyRoot(ref hiveKnightHelperRoot);
            DestroyRoot(ref brokenVesselHelperRoot);
            DestroyRoot(ref lostKinHelperRoot);
            DestroyRoot(ref noskHelperRoot);
            DestroyRoot(ref wingedNoskHelperRoot);
            DestroyRoot(ref uumuuHelperRoot);
            DestroyRoot(ref traitorLordHelperRoot);
            DestroyRoot(ref troupeMasterGrimmHelperRoot);
            DestroyRoot(ref nightmareKingGrimmHelperRoot);
            DestroyRoot(ref pureVesselHelperRoot);
            DestroyRoot(ref absoluteRadianceHelperRoot);
            DestroyRoot(ref paintmasterSheoHelperRoot);
            DestroyRoot(ref soulWarriorHelperRoot);
            DestroyRoot(ref nailsageSlyHelperRoot);
            DestroyRoot(ref soulMasterHelperRoot);
            DestroyRoot(ref soulTyrantHelperRoot);
            DestroyRoot(ref watcherKnightHelperRoot);
            DestroyRoot(ref oroMatoHelperRoot);
            DestroyRoot(ref godTamerHelperRoot);
            DestroyRoot(ref oblobblesHelperRoot);
            DestroyRoot(ref falseKnightHelperRoot);
            DestroyRoot(ref failedChampionHelperRoot);
            DestroyRoot(ref sisterOfBattleHelperRoot);
            DestroyRoot(ref mantisLordHelperRoot);
            DestroyRoot(ref flukemarmHelperRoot);
            DestroyRoot(ref vengeflyKingRoot);
        }

        private Module? GetMarmuHelperModule()
        {
            if (marmuHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.MarmuHelper), out marmuHelperModule);
            }

            return marmuHelperModule;
        }

        private bool GetMarmuHelperEnabled()
        {
            Module? module = GetMarmuHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetMarmuHelperEnabled(bool value)
        {
            Module? module = GetMarmuHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateMarmuHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetMarmuUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.MarmuHelper.marmuP5Hp)
            {
                Modules.BossChallenge.MarmuHelper.marmuUseMaxHp = true;
                RefreshMarmuHelperUi();
                return;
            }

            Modules.BossChallenge.MarmuHelper.marmuUseMaxHp = value;
            Modules.BossChallenge.MarmuHelper.ReapplyLiveSettings();
            RefreshMarmuHelperUi();
        }

        private void SetMarmuP5HpEnabled(bool value)
        {
            Modules.BossChallenge.MarmuHelper.SetP5HpEnabled(value);
            RefreshMarmuHelperUi();
        }

        private void SetMarmuMaxHp(int value)
        {
            if (Modules.BossChallenge.MarmuHelper.marmuP5Hp)
            {
                Modules.BossChallenge.MarmuHelper.marmuMaxHp = 416;
                RefreshMarmuHelperUi();
                return;
            }

            Modules.BossChallenge.MarmuHelper.marmuMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.MarmuHelper.marmuUseMaxHp)
            {
                Modules.BossChallenge.MarmuHelper.ApplyMarmuHealthIfPresent();
            }
        }

        private void RefreshMarmuHelperUi()
        {
            Module? module = GetMarmuHelperModule();
            UpdateToggleValue(marmuHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(marmuHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(marmuP5HpValue, Modules.BossChallenge.MarmuHelper.marmuP5Hp);
            UpdateToggleValue(marmuUseMaxHpValue, Modules.BossChallenge.MarmuHelper.marmuUseMaxHp);
            UpdateIntInputValue(marmuMaxHpField, Modules.BossChallenge.MarmuHelper.marmuMaxHp);

            UpdateMarmuHelperInteractivity();
        }

        private void UpdateMarmuHelperInteractivity()
        {
            SetContentInteractivity(marmuHelperContent, GetMarmuHelperEnabled(), "MarmuHelperEnableRow");
            if (!GetMarmuHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.MarmuHelper.marmuUseMaxHp;
            bool p5Hp = Modules.BossChallenge.MarmuHelper.marmuP5Hp;
            SetRowInteractivity(marmuHelperContent, "MarmuUseMaxHpRow", !p5Hp);
            SetRowInteractivity(marmuHelperContent, "MarmuMaxHpRow", useMaxHp && !p5Hp);
        }

        private void SetMarmuHelperVisible(bool value)
        {
            marmuHelperVisible = value;
            if (marmuHelperRoot != null)
            {
                marmuHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshMarmuHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetXeroHelperModule()
        {
            if (xeroHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.XeroHelper), out xeroHelperModule);
            }

            return xeroHelperModule;
        }

        private bool GetXeroHelperEnabled()
        {
            Module? module = GetXeroHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetXeroHelperEnabled(bool value)
        {
            Module? module = GetXeroHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateXeroHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetXeroUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.XeroHelper.xeroP5Hp)
            {
                Modules.BossChallenge.XeroHelper.xeroUseMaxHp = true;
                RefreshXeroHelperUi();
                return;
            }

            Modules.BossChallenge.XeroHelper.xeroUseMaxHp = value;
            Modules.BossChallenge.XeroHelper.xeroPhase2Hp = Mathf.Clamp(Modules.BossChallenge.XeroHelper.xeroPhase2Hp, 1, GetXeroPhase2MaxHp());
            Modules.BossChallenge.XeroHelper.ReapplyLiveSettings();
            RefreshXeroHelperUi();
        }

        private void SetXeroUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.XeroHelper.xeroP5Hp)
            {
                Modules.BossChallenge.XeroHelper.xeroUseCustomPhase = false;
                RefreshXeroHelperUi();
                return;
            }

            Modules.BossChallenge.XeroHelper.xeroPhase2Hp = Mathf.Clamp(Modules.BossChallenge.XeroHelper.xeroPhase2Hp, 1, GetXeroPhase2MaxHp());
            Modules.BossChallenge.XeroHelper.xeroUseCustomPhase = value;
            Modules.BossChallenge.XeroHelper.ReapplyLiveSettings();
            RefreshXeroHelperUi();
        }

        private void SetXeroP5HpEnabled(bool value)
        {
            Modules.BossChallenge.XeroHelper.SetP5HpEnabled(value);
            RefreshXeroHelperUi();
        }

        private void SetXeroMaxHp(int value)
        {
            if (Modules.BossChallenge.XeroHelper.xeroP5Hp)
            {
                Modules.BossChallenge.XeroHelper.xeroMaxHp = 650;
                RefreshXeroHelperUi();
                return;
            }

            Modules.BossChallenge.XeroHelper.xeroMaxHp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.XeroHelper.xeroPhase2Hp = Mathf.Clamp(Modules.BossChallenge.XeroHelper.xeroPhase2Hp, 1, GetXeroPhase2MaxHp());
            if (Modules.BossChallenge.XeroHelper.xeroUseMaxHp)
            {
                Modules.BossChallenge.XeroHelper.ApplyXeroHealthIfPresent();
            }

            if (Modules.BossChallenge.XeroHelper.xeroUseCustomPhase)
            {
                Modules.BossChallenge.XeroHelper.ReapplyLiveSettings();
            }

            RefreshXeroHelperUi();
        }

        private void SetXeroPhase2Hp(int value)
        {
            if (Modules.BossChallenge.XeroHelper.xeroP5Hp)
            {
                RefreshXeroHelperUi();
                return;
            }

            Modules.BossChallenge.XeroHelper.xeroPhase2Hp = Mathf.Clamp(value, 1, GetXeroPhase2MaxHp());

            if (Modules.BossChallenge.XeroHelper.xeroUseCustomPhase)
            {
                Modules.BossChallenge.XeroHelper.ReapplyLiveSettings();
            }

            RefreshXeroHelperUi();
        }

        private static int GetXeroPhase2MaxHp()
        {
            if (Modules.BossChallenge.XeroHelper.xeroP5Hp)
            {
                return 650;
            }

            if (Modules.BossChallenge.XeroHelper.xeroUseMaxHp)
            {
                return Mathf.Clamp(Modules.BossChallenge.XeroHelper.xeroMaxHp, 1, 999999);
            }

            return 900;
        }

        private void RefreshXeroHelperUi()
        {
            Module? module = GetXeroHelperModule();
            UpdateToggleValue(xeroHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(xeroHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(xeroP5HpValue, Modules.BossChallenge.XeroHelper.xeroP5Hp);
            UpdateToggleValue(xeroUseMaxHpValue, Modules.BossChallenge.XeroHelper.xeroUseMaxHp);
            UpdateToggleValue(xeroUseCustomPhaseValue, Modules.BossChallenge.XeroHelper.xeroUseCustomPhase);
            UpdateIntInputValue(xeroMaxHpField, Modules.BossChallenge.XeroHelper.xeroMaxHp);
            UpdateIntInputValue(xeroPhase2HpField, Modules.BossChallenge.XeroHelper.xeroPhase2Hp);

            UpdateXeroHelperInteractivity();
        }

        private void UpdateXeroHelperInteractivity()
        {
            SetContentInteractivity(xeroHelperContent, GetXeroHelperEnabled(), "XeroHelperEnableRow");
            if (!GetXeroHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.XeroHelper.xeroUseMaxHp;
            bool p5Hp = Modules.BossChallenge.XeroHelper.xeroP5Hp;
            bool useCustomPhase = Modules.BossChallenge.XeroHelper.xeroUseCustomPhase;
            SetRowInteractivity(xeroHelperContent, "XeroUseMaxHpRow", !p5Hp);
            SetRowInteractivity(xeroHelperContent, "XeroMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(xeroHelperContent, "XeroUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(xeroHelperContent, "XeroPhase2HpRow", useCustomPhase && !p5Hp);
        }

        private void SetXeroHelperVisible(bool value)
        {
            xeroHelperVisible = value;
            if (xeroHelperRoot != null)
            {
                xeroHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshXeroHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetMarkothHelperModule()
        {
            if (markothHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.MarkothHelper), out markothHelperModule);
            }

            return markothHelperModule;
        }

        private bool GetMarkothHelperEnabled()
        {
            Module? module = GetMarkothHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetMarkothHelperEnabled(bool value)
        {
            Module? module = GetMarkothHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateMarkothHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetMarkothUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.MarkothHelper.markothP5Hp)
            {
                Modules.BossChallenge.MarkothHelper.markothUseMaxHp = true;
                RefreshMarkothHelperUi();
                return;
            }

            Modules.BossChallenge.MarkothHelper.markothUseMaxHp = value;
            Modules.BossChallenge.MarkothHelper.markothPhase2Hp = Mathf.Clamp(Modules.BossChallenge.MarkothHelper.markothPhase2Hp, 1, GetMarkothPhase2MaxHp());
            Modules.BossChallenge.MarkothHelper.ReapplyLiveSettings();
            RefreshMarkothHelperUi();
        }

        private void SetMarkothUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.MarkothHelper.markothP5Hp)
            {
                Modules.BossChallenge.MarkothHelper.markothUseCustomPhase = false;
                RefreshMarkothHelperUi();
                return;
            }

            Modules.BossChallenge.MarkothHelper.markothPhase2Hp = Mathf.Clamp(Modules.BossChallenge.MarkothHelper.markothPhase2Hp, 1, GetMarkothPhase2MaxHp());
            Modules.BossChallenge.MarkothHelper.markothUseCustomPhase = value;
            Modules.BossChallenge.MarkothHelper.ReapplyLiveSettings();
            RefreshMarkothHelperUi();
        }

        private void SetMarkothP5HpEnabled(bool value)
        {
            Modules.BossChallenge.MarkothHelper.SetP5HpEnabled(value);
            RefreshMarkothHelperUi();
        }

        private void SetMarkothMaxHp(int value)
        {
            if (Modules.BossChallenge.MarkothHelper.markothP5Hp)
            {
                Modules.BossChallenge.MarkothHelper.markothMaxHp = 650;
                RefreshMarkothHelperUi();
                return;
            }

            Modules.BossChallenge.MarkothHelper.markothMaxHp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.MarkothHelper.markothPhase2Hp = Mathf.Clamp(Modules.BossChallenge.MarkothHelper.markothPhase2Hp, 1, GetMarkothPhase2MaxHp());
            if (Modules.BossChallenge.MarkothHelper.markothUseMaxHp)
            {
                Modules.BossChallenge.MarkothHelper.ApplyMarkothHealthIfPresent();
            }

            if (Modules.BossChallenge.MarkothHelper.markothUseCustomPhase)
            {
                Modules.BossChallenge.MarkothHelper.ReapplyLiveSettings();
            }

            RefreshMarkothHelperUi();
        }

        private void SetMarkothPhase2Hp(int value)
        {
            if (Modules.BossChallenge.MarkothHelper.markothP5Hp)
            {
                RefreshMarkothHelperUi();
                return;
            }

            Modules.BossChallenge.MarkothHelper.markothPhase2Hp = Mathf.Clamp(value, 1, GetMarkothPhase2MaxHp());

            if (Modules.BossChallenge.MarkothHelper.markothUseCustomPhase)
            {
                Modules.BossChallenge.MarkothHelper.ReapplyLiveSettings();
            }

            RefreshMarkothHelperUi();
        }

        private static int GetMarkothPhase2MaxHp()
        {
            if (Modules.BossChallenge.MarkothHelper.markothP5Hp)
            {
                return 650;
            }

            if (Modules.BossChallenge.MarkothHelper.markothUseMaxHp)
            {
                return Mathf.Clamp(Modules.BossChallenge.MarkothHelper.markothMaxHp, 1, 999999);
            }

            return 950;
        }

        private void RefreshMarkothHelperUi()
        {
            Module? module = GetMarkothHelperModule();
            UpdateToggleValue(markothHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(markothHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(markothP5HpValue, Modules.BossChallenge.MarkothHelper.markothP5Hp);
            UpdateToggleValue(markothUseMaxHpValue, Modules.BossChallenge.MarkothHelper.markothUseMaxHp);
            UpdateToggleValue(markothUseCustomPhaseValue, Modules.BossChallenge.MarkothHelper.markothUseCustomPhase);
            UpdateIntInputValue(markothMaxHpField, Modules.BossChallenge.MarkothHelper.markothMaxHp);
            UpdateIntInputValue(markothPhase2HpField, Modules.BossChallenge.MarkothHelper.markothPhase2Hp);

            UpdateMarkothHelperInteractivity();
        }

        private void UpdateMarkothHelperInteractivity()
        {
            SetContentInteractivity(markothHelperContent, GetMarkothHelperEnabled(), "MarkothHelperEnableRow");
            if (!GetMarkothHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.MarkothHelper.markothUseMaxHp;
            bool p5Hp = Modules.BossChallenge.MarkothHelper.markothP5Hp;
            bool useCustomPhase = Modules.BossChallenge.MarkothHelper.markothUseCustomPhase;
            SetRowInteractivity(markothHelperContent, "MarkothUseMaxHpRow", !p5Hp);
            SetRowInteractivity(markothHelperContent, "MarkothMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(markothHelperContent, "MarkothUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(markothHelperContent, "MarkothPhase2HpRow", useCustomPhase && !p5Hp);
        }

        private void SetMarkothHelperVisible(bool value)
        {
            markothHelperVisible = value;
            if (markothHelperRoot != null)
            {
                markothHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshMarkothHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetGalienHelperModule()
        {
            if (galienHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.GalienHelper), out galienHelperModule);
            }

            return galienHelperModule;
        }

        private bool GetGalienHelperEnabled()
        {
            Module? module = GetGalienHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetGalienHelperEnabled(bool value)
        {
            Module? module = GetGalienHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateGalienHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetGalienUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.GalienHelper.galienP5Hp)
            {
                Modules.BossChallenge.GalienHelper.galienUseMaxHp = true;
                RefreshGalienHelperUi();
                return;
            }

            Modules.BossChallenge.GalienHelper.galienUseMaxHp = value;
            NormalizeGalienPhaseThresholdInputs();
            Modules.BossChallenge.GalienHelper.ReapplyLiveSettings();
            RefreshGalienHelperUi();
        }

        private void SetGalienUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.GalienHelper.galienP5Hp)
            {
                Modules.BossChallenge.GalienHelper.galienUseCustomPhase = false;
                RefreshGalienHelperUi();
                return;
            }

            int phase2MaxHp = GetGalienPhase2MaxHp();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.GalienHelper.galienPhase2Hp, 2, phase2MaxHp);
            Modules.BossChallenge.GalienHelper.galienPhase2Hp = phase2Hp;
            Modules.BossChallenge.GalienHelper.galienPhase3Hp = Mathf.Clamp(Modules.BossChallenge.GalienHelper.galienPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
            Modules.BossChallenge.GalienHelper.galienUseCustomPhase = value;
            Modules.BossChallenge.GalienHelper.ReapplyLiveSettings();
            RefreshGalienHelperUi();
        }

        private void SetGalienP5HpEnabled(bool value)
        {
            Modules.BossChallenge.GalienHelper.SetP5HpEnabled(value);
            RefreshGalienHelperUi();
        }

        private void SetGalienMaxHp(int value)
        {
            if (Modules.BossChallenge.GalienHelper.galienP5Hp)
            {
                Modules.BossChallenge.GalienHelper.galienMaxHp = 650;
                RefreshGalienHelperUi();
                return;
            }

            Modules.BossChallenge.GalienHelper.galienMaxHp = Mathf.Clamp(value, 1, 999999);
            NormalizeGalienPhaseThresholdInputs();
            if (Modules.BossChallenge.GalienHelper.galienUseMaxHp)
            {
                Modules.BossChallenge.GalienHelper.ApplyGalienHealthIfPresent();
            }

            if (Modules.BossChallenge.GalienHelper.galienUseCustomPhase)
            {
                Modules.BossChallenge.GalienHelper.ReapplyLiveSettings();
            }
        }

        private void SetGalienPhase2Hp(int value)
        {
            if (Modules.BossChallenge.GalienHelper.galienP5Hp)
            {
                RefreshGalienHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(value, 2, GetGalienPhase2MaxHp());
            Modules.BossChallenge.GalienHelper.galienPhase2Hp = phase2Hp;
            Modules.BossChallenge.GalienHelper.galienPhase3Hp = Mathf.Clamp(Modules.BossChallenge.GalienHelper.galienPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.GalienHelper.galienUseCustomPhase)
            {
                Modules.BossChallenge.GalienHelper.ReapplyLiveSettings();
            }

            RefreshGalienHelperUi();
        }

        private void SetGalienPhase3Hp(int value)
        {
            if (Modules.BossChallenge.GalienHelper.galienP5Hp)
            {
                RefreshGalienHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.GalienHelper.galienPhase2Hp, 2, GetGalienPhase2MaxHp());
            Modules.BossChallenge.GalienHelper.galienPhase2Hp = phase2Hp;
            Modules.BossChallenge.GalienHelper.galienPhase3Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.GalienHelper.galienUseCustomPhase)
            {
                Modules.BossChallenge.GalienHelper.ReapplyLiveSettings();
            }

            RefreshGalienHelperUi();
        }

        private int GetGalienPhase2MaxHp()
        {
            return Mathf.Clamp(Modules.BossChallenge.GalienHelper.GetPhase2MaxHpForUi(), 2, 999999);
        }

        private void NormalizeGalienPhaseThresholdInputs()
        {
            int phase2MaxHp = GetGalienPhase2MaxHp();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.GalienHelper.galienPhase2Hp, 2, phase2MaxHp);
            Modules.BossChallenge.GalienHelper.galienPhase2Hp = phase2Hp;
            Modules.BossChallenge.GalienHelper.galienPhase3Hp = Mathf.Clamp(Modules.BossChallenge.GalienHelper.galienPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
        }

        private void RefreshGalienHelperUi()
        {
            Module? module = GetGalienHelperModule();
            UpdateToggleValue(galienHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(galienHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(galienP5HpValue, Modules.BossChallenge.GalienHelper.galienP5Hp);
            UpdateToggleValue(galienUseMaxHpValue, Modules.BossChallenge.GalienHelper.galienUseMaxHp);
            UpdateToggleValue(galienUseCustomPhaseValue, Modules.BossChallenge.GalienHelper.galienUseCustomPhase);
            UpdateIntInputValue(galienMaxHpField, Modules.BossChallenge.GalienHelper.galienMaxHp);
            UpdateIntInputValue(galienPhase2HpField, Modules.BossChallenge.GalienHelper.galienPhase2Hp);
            UpdateIntInputValue(galienPhase3HpField, Modules.BossChallenge.GalienHelper.galienPhase3Hp);

            UpdateGalienHelperInteractivity();
        }

        private void UpdateGalienHelperInteractivity()
        {
            SetContentInteractivity(galienHelperContent, GetGalienHelperEnabled(), "GalienHelperEnableRow");
            if (!GetGalienHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.GalienHelper.galienUseMaxHp;
            bool p5Hp = Modules.BossChallenge.GalienHelper.galienP5Hp;
            bool useCustomPhase = Modules.BossChallenge.GalienHelper.galienUseCustomPhase;
            SetRowInteractivity(galienHelperContent, "GalienUseMaxHpRow", !p5Hp);
            SetRowInteractivity(galienHelperContent, "GalienMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(galienHelperContent, "GalienUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(galienHelperContent, "GalienPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(galienHelperContent, "GalienPhase3HpRow", useCustomPhase && !p5Hp);
        }

        private void SetGalienHelperVisible(bool value)
        {
            galienHelperVisible = value;
            if (galienHelperRoot != null)
            {
                galienHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshGalienHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetGorbHelperModule()
        {
            if (gorbHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.GorbHelper), out gorbHelperModule);
            }

            return gorbHelperModule;
        }

        private bool GetGorbHelperEnabled()
        {
            Module? module = GetGorbHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetGorbHelperEnabled(bool value)
        {
            Module? module = GetGorbHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateGorbHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetGorbUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.GorbHelper.gorbP5Hp)
            {
                Modules.BossChallenge.GorbHelper.gorbUseMaxHp = true;
                RefreshGorbHelperUi();
                return;
            }

            Modules.BossChallenge.GorbHelper.gorbUseMaxHp = value;
            NormalizeGorbPhaseThresholdInputs();
            Modules.BossChallenge.GorbHelper.ReapplyLiveSettings();
            RefreshGorbHelperUi();
        }

        private void SetGorbUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.GorbHelper.gorbP5Hp)
            {
                Modules.BossChallenge.GorbHelper.gorbUseCustomPhase = false;
                RefreshGorbHelperUi();
                return;
            }

            int phase2MaxHp = GetGorbPhase2MaxHp();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.GorbHelper.gorbPhase2Hp, 2, phase2MaxHp);
            Modules.BossChallenge.GorbHelper.gorbPhase2Hp = phase2Hp;
            Modules.BossChallenge.GorbHelper.gorbPhase3Hp = Mathf.Clamp(Modules.BossChallenge.GorbHelper.gorbPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
            Modules.BossChallenge.GorbHelper.gorbUseCustomPhase = value;
            Modules.BossChallenge.GorbHelper.ReapplyLiveSettings();
            RefreshGorbHelperUi();
        }

        private void SetGorbP5HpEnabled(bool value)
        {
            Modules.BossChallenge.GorbHelper.SetP5HpEnabled(value);
            RefreshGorbHelperUi();
        }

        private void SetGorbMaxHp(int value)
        {
            if (Modules.BossChallenge.GorbHelper.gorbP5Hp)
            {
                Modules.BossChallenge.GorbHelper.gorbMaxHp = 650;
                RefreshGorbHelperUi();
                return;
            }

            Modules.BossChallenge.GorbHelper.gorbMaxHp = Mathf.Clamp(value, 1, 999999);
            NormalizeGorbPhaseThresholdInputs();
            if (Modules.BossChallenge.GorbHelper.gorbUseMaxHp)
            {
                Modules.BossChallenge.GorbHelper.ApplyGorbHealthIfPresent();
            }

            if (Modules.BossChallenge.GorbHelper.gorbUseCustomPhase)
            {
                Modules.BossChallenge.GorbHelper.ReapplyLiveSettings();
            }
        }

        private void SetGorbPhase2Hp(int value)
        {
            if (Modules.BossChallenge.GorbHelper.gorbP5Hp)
            {
                RefreshGorbHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(value, 2, GetGorbPhase2MaxHp());
            Modules.BossChallenge.GorbHelper.gorbPhase2Hp = phase2Hp;
            Modules.BossChallenge.GorbHelper.gorbPhase3Hp = Mathf.Clamp(Modules.BossChallenge.GorbHelper.gorbPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.GorbHelper.gorbUseCustomPhase)
            {
                Modules.BossChallenge.GorbHelper.ReapplyLiveSettings();
            }

            RefreshGorbHelperUi();
        }

        private void SetGorbPhase3Hp(int value)
        {
            if (Modules.BossChallenge.GorbHelper.gorbP5Hp)
            {
                RefreshGorbHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.GorbHelper.gorbPhase2Hp, 2, GetGorbPhase2MaxHp());
            Modules.BossChallenge.GorbHelper.gorbPhase2Hp = phase2Hp;
            Modules.BossChallenge.GorbHelper.gorbPhase3Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.GorbHelper.gorbUseCustomPhase)
            {
                Modules.BossChallenge.GorbHelper.ReapplyLiveSettings();
            }

            RefreshGorbHelperUi();
        }

        private int GetGorbPhase2MaxHp()
        {
            return Mathf.Clamp(Modules.BossChallenge.GorbHelper.GetPhase2MaxHpForUi(), 2, 999999);
        }

        private void NormalizeGorbPhaseThresholdInputs()
        {
            int phase2MaxHp = GetGorbPhase2MaxHp();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.GorbHelper.gorbPhase2Hp, 2, phase2MaxHp);
            Modules.BossChallenge.GorbHelper.gorbPhase2Hp = phase2Hp;
            Modules.BossChallenge.GorbHelper.gorbPhase3Hp = Mathf.Clamp(Modules.BossChallenge.GorbHelper.gorbPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
        }

        private void RefreshGorbHelperUi()
        {
            Module? module = GetGorbHelperModule();
            UpdateToggleValue(gorbHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(gorbHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(gorbP5HpValue, Modules.BossChallenge.GorbHelper.gorbP5Hp);
            UpdateToggleValue(gorbUseMaxHpValue, Modules.BossChallenge.GorbHelper.gorbUseMaxHp);
            UpdateToggleValue(gorbUseCustomPhaseValue, Modules.BossChallenge.GorbHelper.gorbUseCustomPhase);
            UpdateIntInputValue(gorbMaxHpField, Modules.BossChallenge.GorbHelper.gorbMaxHp);
            UpdateIntInputValue(gorbPhase2HpField, Modules.BossChallenge.GorbHelper.gorbPhase2Hp);
            UpdateIntInputValue(gorbPhase3HpField, Modules.BossChallenge.GorbHelper.gorbPhase3Hp);

            UpdateGorbHelperInteractivity();
        }

        private void UpdateGorbHelperInteractivity()
        {
            SetContentInteractivity(gorbHelperContent, GetGorbHelperEnabled(), "GorbHelperEnableRow");
            if (!GetGorbHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.GorbHelper.gorbUseMaxHp;
            bool p5Hp = Modules.BossChallenge.GorbHelper.gorbP5Hp;
            bool useCustomPhase = Modules.BossChallenge.GorbHelper.gorbUseCustomPhase;
            SetRowInteractivity(gorbHelperContent, "GorbUseMaxHpRow", !p5Hp);
            SetRowInteractivity(gorbHelperContent, "GorbMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(gorbHelperContent, "GorbUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(gorbHelperContent, "GorbPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(gorbHelperContent, "GorbPhase3HpRow", useCustomPhase && !p5Hp);
        }

        private void SetGorbHelperVisible(bool value)
        {
            gorbHelperVisible = value;
            if (gorbHelperRoot != null)
            {
                gorbHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshGorbHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetElderHuHelperModule()
        {
            if (elderHuHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.ElderHuHelper), out elderHuHelperModule);
            }

            return elderHuHelperModule;
        }

        private bool GetElderHuHelperEnabled()
        {
            Module? module = GetElderHuHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetElderHuHelperEnabled(bool value)
        {
            Module? module = GetElderHuHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateElderHuHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetElderHuUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.ElderHuHelper.elderHuP5Hp)
            {
                Modules.BossChallenge.ElderHuHelper.elderHuUseMaxHp = true;
                RefreshElderHuHelperUi();
                return;
            }

            Modules.BossChallenge.ElderHuHelper.elderHuUseMaxHp = value;
            Modules.BossChallenge.ElderHuHelper.ReapplyLiveSettings();
            RefreshElderHuHelperUi();
        }

        private void SetElderHuP5HpEnabled(bool value)
        {
            Modules.BossChallenge.ElderHuHelper.SetP5HpEnabled(value);
            RefreshElderHuHelperUi();
        }

        private void SetElderHuMaxHp(int value)
        {
            if (Modules.BossChallenge.ElderHuHelper.elderHuP5Hp)
            {
                Modules.BossChallenge.ElderHuHelper.elderHuMaxHp = 600;
                RefreshElderHuHelperUi();
                return;
            }

            Modules.BossChallenge.ElderHuHelper.elderHuMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.ElderHuHelper.elderHuUseMaxHp)
            {
                Modules.BossChallenge.ElderHuHelper.ApplyElderHuHealthIfPresent();
            }
        }

        private void RefreshElderHuHelperUi()
        {
            Module? module = GetElderHuHelperModule();
            UpdateToggleValue(elderHuHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(elderHuHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(elderHuP5HpValue, Modules.BossChallenge.ElderHuHelper.elderHuP5Hp);
            UpdateToggleValue(elderHuUseMaxHpValue, Modules.BossChallenge.ElderHuHelper.elderHuUseMaxHp);
            UpdateIntInputValue(elderHuMaxHpField, Modules.BossChallenge.ElderHuHelper.elderHuMaxHp);

            UpdateElderHuHelperInteractivity();
        }

        private void UpdateElderHuHelperInteractivity()
        {
            SetContentInteractivity(elderHuHelperContent, GetElderHuHelperEnabled(), "ElderHuHelperEnableRow");
            if (!GetElderHuHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.ElderHuHelper.elderHuUseMaxHp;
            bool p5Hp = Modules.BossChallenge.ElderHuHelper.elderHuP5Hp;
            SetRowInteractivity(elderHuHelperContent, "ElderHuUseMaxHpRow", !p5Hp);
            SetRowInteractivity(elderHuHelperContent, "ElderHuMaxHpRow", useMaxHp && !p5Hp);
        }

        private void SetElderHuHelperVisible(bool value)
        {
            elderHuHelperVisible = value;
            if (elderHuHelperRoot != null)
            {
                elderHuHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshElderHuHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetNoEyesHelperModule()
        {
            if (noEyesHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.NoEyesHelper), out noEyesHelperModule);
            }

            return noEyesHelperModule;
        }

        private bool GetNoEyesHelperEnabled()
        {
            Module? module = GetNoEyesHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetNoEyesHelperEnabled(bool value)
        {
            Module? module = GetNoEyesHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateNoEyesHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetNoEyesUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.NoEyesHelper.noEyesP5Hp)
            {
                Modules.BossChallenge.NoEyesHelper.noEyesUseMaxHp = true;
                RefreshNoEyesHelperUi();
                return;
            }

            Modules.BossChallenge.NoEyesHelper.noEyesUseMaxHp = value;
            NormalizeNoEyesPhaseThresholdInputs();
            Modules.BossChallenge.NoEyesHelper.ReapplyLiveSettings();
            RefreshNoEyesHelperUi();
        }

        private void SetNoEyesUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.NoEyesHelper.noEyesP5Hp)
            {
                Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase = false;
                RefreshNoEyesHelperUi();
                return;
            }

            int phase2MaxHp = GetNoEyesPhase2MaxHp();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp, 2, phase2MaxHp);
            Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp = phase2Hp;
            Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp = Mathf.Clamp(Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
            Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase = value;
            Modules.BossChallenge.NoEyesHelper.ReapplyLiveSettings();
            RefreshNoEyesHelperUi();
        }

        private void SetNoEyesP5HpEnabled(bool value)
        {
            Modules.BossChallenge.NoEyesHelper.SetP5HpEnabled(value);
            RefreshNoEyesHelperUi();
        }

        private void SetNoEyesMaxHp(int value)
        {
            if (Modules.BossChallenge.NoEyesHelper.noEyesP5Hp)
            {
                Modules.BossChallenge.NoEyesHelper.noEyesMaxHp = 570;
                RefreshNoEyesHelperUi();
                return;
            }

            Modules.BossChallenge.NoEyesHelper.noEyesMaxHp = Mathf.Clamp(value, 1, 999999);
            NormalizeNoEyesPhaseThresholdInputs();
            if (Modules.BossChallenge.NoEyesHelper.noEyesUseMaxHp || Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase)
            {
                Modules.BossChallenge.NoEyesHelper.ReapplyLiveSettings();
            }

            RefreshNoEyesHelperUi();
        }

        private void SetNoEyesPhase2Hp(int value)
        {
            if (Modules.BossChallenge.NoEyesHelper.noEyesP5Hp)
            {
                RefreshNoEyesHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(value, 2, GetNoEyesPhase2MaxHp());
            Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp = phase2Hp;
            Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp = Mathf.Clamp(Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase)
            {
                Modules.BossChallenge.NoEyesHelper.ReapplyLiveSettings();
            }

            RefreshNoEyesHelperUi();
        }

        private void SetNoEyesPhase3Hp(int value)
        {
            if (Modules.BossChallenge.NoEyesHelper.noEyesP5Hp)
            {
                RefreshNoEyesHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp, 2, GetNoEyesPhase2MaxHp());
            Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp = phase2Hp;
            Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase)
            {
                Modules.BossChallenge.NoEyesHelper.ReapplyLiveSettings();
            }

            RefreshNoEyesHelperUi();
        }

        private int GetNoEyesPhase2MaxHp()
        {
            return Mathf.Clamp(Modules.BossChallenge.NoEyesHelper.GetPhase2MaxHpForUi(), 2, 999999);
        }

        private void NormalizeNoEyesPhaseThresholdInputs()
        {
            int phase2MaxHp = GetNoEyesPhase2MaxHp();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp, 2, phase2MaxHp);
            Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp = phase2Hp;
            Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp = Mathf.Clamp(Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
        }

        private void RefreshNoEyesHelperUi()
        {
            Module? module = GetNoEyesHelperModule();
            UpdateToggleValue(noEyesHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(noEyesHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(noEyesP5HpValue, Modules.BossChallenge.NoEyesHelper.noEyesP5Hp);
            UpdateToggleValue(noEyesUseMaxHpValue, Modules.BossChallenge.NoEyesHelper.noEyesUseMaxHp);
            UpdateToggleValue(noEyesUseCustomPhaseValue, Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase);
            UpdateIntInputValue(noEyesMaxHpField, Modules.BossChallenge.NoEyesHelper.noEyesMaxHp);
            UpdateIntInputValue(noEyesPhase2HpField, Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp);
            UpdateIntInputValue(noEyesPhase3HpField, Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp);

            UpdateNoEyesHelperInteractivity();
        }

        private void UpdateNoEyesHelperInteractivity()
        {
            SetContentInteractivity(noEyesHelperContent, GetNoEyesHelperEnabled(), "NoEyesHelperEnableRow");
            if (!GetNoEyesHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.NoEyesHelper.noEyesUseMaxHp;
            bool p5Hp = Modules.BossChallenge.NoEyesHelper.noEyesP5Hp;
            bool useCustomPhase = Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase;
            SetRowInteractivity(noEyesHelperContent, "NoEyesUseMaxHpRow", !p5Hp);
            SetRowInteractivity(noEyesHelperContent, "NoEyesMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(noEyesHelperContent, "NoEyesUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(noEyesHelperContent, "NoEyesPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(noEyesHelperContent, "NoEyesPhase3HpRow", useCustomPhase && !p5Hp);
        }

        private void SetNoEyesHelperVisible(bool value)
        {
            noEyesHelperVisible = value;
            if (noEyesHelperRoot != null)
            {
                noEyesHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshNoEyesHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetDungDefenderHelperModule()
        {
            if (dungDefenderHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.DungDefenderHelper), out dungDefenderHelperModule);
            }

            return dungDefenderHelperModule;
        }

        private bool GetDungDefenderHelperEnabled()
        {
            Module? module = GetDungDefenderHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetDungDefenderHelperEnabled(bool value)
        {
            Module? module = GetDungDefenderHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateDungDefenderHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetDungDefenderUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.DungDefenderHelper.dungDefenderP5Hp)
            {
                Modules.BossChallenge.DungDefenderHelper.dungDefenderUseMaxHp = true;
                RefreshDungDefenderHelperUi();
                return;
            }

            Modules.BossChallenge.DungDefenderHelper.dungDefenderUseMaxHp = value;
            Modules.BossChallenge.DungDefenderHelper.ReapplyLiveSettings();
            RefreshDungDefenderHelperUi();
        }

        private void SetDungDefenderUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.DungDefenderHelper.dungDefenderP5Hp)
            {
                Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhase = false;
                RefreshDungDefenderHelperUi();
                return;
            }

            int maxPhase2Hp = Mathf.Max(1, Modules.BossChallenge.DungDefenderHelper.GetPhase2MaxHpForUi());
            Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2Hp = Mathf.Clamp(Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2Hp, 1, maxPhase2Hp);
            Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhase = value;
            Modules.BossChallenge.DungDefenderHelper.ReapplyLiveSettings();
            RefreshDungDefenderHelperUi();
        }

        private void SetDungDefenderP5HpEnabled(bool value)
        {
            Modules.BossChallenge.DungDefenderHelper.SetP5HpEnabled(value);
            RefreshDungDefenderHelperUi();
        }

        private void SetDungDefenderMaxHp(int value)
        {
            if (Modules.BossChallenge.DungDefenderHelper.dungDefenderP5Hp)
            {
                Modules.BossChallenge.DungDefenderHelper.dungDefenderMaxHp = 800;
                RefreshDungDefenderHelperUi();
                return;
            }

            Modules.BossChallenge.DungDefenderHelper.dungDefenderMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.DungDefenderHelper.dungDefenderUseMaxHp)
            {
                Modules.BossChallenge.DungDefenderHelper.ApplyDungDefenderHealthIfPresent();
            }
        }

        private void SetDungDefenderPhase2Hp(int value)
        {
            if (Modules.BossChallenge.DungDefenderHelper.dungDefenderP5Hp)
            {
                Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2Hp = 350;
                RefreshDungDefenderHelperUi();
                return;
            }

            int maxPhase2Hp = Mathf.Max(1, Modules.BossChallenge.DungDefenderHelper.GetPhase2MaxHpForUi());
            Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2Hp = Mathf.Clamp(value, 1, maxPhase2Hp);
            if (Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhase)
            {
                Modules.BossChallenge.DungDefenderHelper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshDungDefenderHelperUi();
        }

        private void RefreshDungDefenderHelperUi()
        {
            Module? module = GetDungDefenderHelperModule();
            UpdateToggleValue(dungDefenderHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(dungDefenderHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(dungDefenderP5HpValue, Modules.BossChallenge.DungDefenderHelper.dungDefenderP5Hp);
            UpdateToggleValue(dungDefenderUseMaxHpValue, Modules.BossChallenge.DungDefenderHelper.dungDefenderUseMaxHp);
            UpdateToggleValue(dungDefenderUseCustomPhaseValue, Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhase);
            UpdateIntInputValue(dungDefenderMaxHpField, Modules.BossChallenge.DungDefenderHelper.dungDefenderMaxHp);
            UpdateIntInputValue(dungDefenderPhase2HpField, Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2Hp);

            UpdateDungDefenderHelperInteractivity();
        }

        private void UpdateDungDefenderHelperInteractivity()
        {
            SetContentInteractivity(dungDefenderHelperContent, GetDungDefenderHelperEnabled(), "DungDefenderHelperEnableRow");
            if (!GetDungDefenderHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.DungDefenderHelper.dungDefenderUseMaxHp;
            bool p5Hp = Modules.BossChallenge.DungDefenderHelper.dungDefenderP5Hp;
            bool useCustomPhase = Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhase;
            SetRowInteractivity(dungDefenderHelperContent, "DungDefenderUseMaxHpRow", !p5Hp);
            SetRowInteractivity(dungDefenderHelperContent, "DungDefenderMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(dungDefenderHelperContent, "DungDefenderUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(dungDefenderHelperContent, "DungDefenderPhase2HpRow", useCustomPhase && !p5Hp);
        }

        private void SetDungDefenderHelperVisible(bool value)
        {
            dungDefenderHelperVisible = value;
            if (dungDefenderHelperRoot != null)
            {
                dungDefenderHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshDungDefenderHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetWhiteDefenderHelperModule()
        {
            if (whiteDefenderHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.WhiteDefenderHelper), out whiteDefenderHelperModule);
            }

            return whiteDefenderHelperModule;
        }

        private bool GetWhiteDefenderHelperEnabled()
        {
            Module? module = GetWhiteDefenderHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetWhiteDefenderHelperEnabled(bool value)
        {
            Module? module = GetWhiteDefenderHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateWhiteDefenderHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetWhiteDefenderUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderP5Hp)
            {
                Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseMaxHp = true;
                RefreshWhiteDefenderHelperUi();
                return;
            }

            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseMaxHp = value;
            Modules.BossChallenge.WhiteDefenderHelper.ReapplyLiveSettings();
            RefreshWhiteDefenderHelperUi();
        }

        private void SetWhiteDefenderUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderP5Hp)
            {
                Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseCustomPhase = false;
                RefreshWhiteDefenderHelperUi();
                return;
            }

            int maxPhase2Hp = Mathf.Max(1, Modules.BossChallenge.WhiteDefenderHelper.GetPhase2MaxHpForUi());
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderPhase2Hp = Mathf.Clamp(Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderPhase2Hp, 1, maxPhase2Hp);
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseCustomPhase = value;
            Modules.BossChallenge.WhiteDefenderHelper.ReapplyLiveSettings();
            RefreshWhiteDefenderHelperUi();
        }

        private void SetWhiteDefenderP5HpEnabled(bool value)
        {
            Modules.BossChallenge.WhiteDefenderHelper.SetP5HpEnabled(value);
            RefreshWhiteDefenderHelperUi();
        }

        private void SetWhiteDefenderMaxHp(int value)
        {
            if (Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderP5Hp)
            {
                Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderMaxHp = 1600;
                RefreshWhiteDefenderHelperUi();
                return;
            }

            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseMaxHp)
            {
                Modules.BossChallenge.WhiteDefenderHelper.ApplyWhiteDefenderHealthIfPresent();
            }
        }

        private void SetWhiteDefenderPhase2Hp(int value)
        {
            if (Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderP5Hp)
            {
                Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderPhase2Hp = 600;
                RefreshWhiteDefenderHelperUi();
                return;
            }

            int maxPhase2Hp = Mathf.Max(1, Modules.BossChallenge.WhiteDefenderHelper.GetPhase2MaxHpForUi());
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderPhase2Hp = Mathf.Clamp(value, 1, maxPhase2Hp);
            if (Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseCustomPhase)
            {
                Modules.BossChallenge.WhiteDefenderHelper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshWhiteDefenderHelperUi();
        }

        private void RefreshWhiteDefenderHelperUi()
        {
            Module? module = GetWhiteDefenderHelperModule();
            UpdateToggleValue(whiteDefenderHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(whiteDefenderHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(whiteDefenderP5HpValue, Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderP5Hp);
            UpdateToggleValue(whiteDefenderUseMaxHpValue, Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseMaxHp);
            UpdateToggleValue(whiteDefenderUseCustomPhaseValue, Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseCustomPhase);
            UpdateIntInputValue(whiteDefenderMaxHpField, Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderMaxHp);
            UpdateIntInputValue(whiteDefenderPhase2HpField, Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderPhase2Hp);

            UpdateWhiteDefenderHelperInteractivity();
        }

        private void UpdateWhiteDefenderHelperInteractivity()
        {
            SetContentInteractivity(whiteDefenderHelperContent, GetWhiteDefenderHelperEnabled(), "WhiteDefenderHelperEnableRow");
            if (!GetWhiteDefenderHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseMaxHp;
            bool p5Hp = Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderP5Hp;
            bool useCustomPhase = Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseCustomPhase;
            SetRowInteractivity(whiteDefenderHelperContent, "WhiteDefenderUseMaxHpRow", !p5Hp);
            SetRowInteractivity(whiteDefenderHelperContent, "WhiteDefenderMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(whiteDefenderHelperContent, "WhiteDefenderUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(whiteDefenderHelperContent, "WhiteDefenderPhase2HpRow", useCustomPhase && !p5Hp);
        }

        private void SetWhiteDefenderHelperVisible(bool value)
        {
            whiteDefenderHelperVisible = value;
            if (whiteDefenderHelperRoot != null)
            {
                whiteDefenderHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshWhiteDefenderHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetHiveKnightHelperModule()
        {
            if (hiveKnightHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.HiveKnightHelper), out hiveKnightHelperModule);
            }

            return hiveKnightHelperModule;
        }

        private bool GetHiveKnightHelperEnabled()
        {
            Module? module = GetHiveKnightHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetHiveKnightHelperEnabled(bool value)
        {
            Module? module = GetHiveKnightHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateHiveKnightHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetHiveKnightUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp)
            {
                Modules.BossChallenge.HiveKnightHelper.hiveKnightUseMaxHp = true;
                RefreshHiveKnightHelperUi();
                return;
            }

            Modules.BossChallenge.HiveKnightHelper.hiveKnightUseMaxHp = value;
            Modules.BossChallenge.HiveKnightHelper.ReapplyLiveSettings();
            RefreshHiveKnightHelperUi();
        }

        private void SetHiveKnightUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp)
            {
                Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase = false;
                RefreshHiveKnightHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase2Hp, 2, 1300);
            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase2Hp = phase2Hp;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase3Hp = Mathf.Clamp(Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
            Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase = value;
            Modules.BossChallenge.HiveKnightHelper.ReapplyLiveSettings();
            RefreshHiveKnightHelperUi();
        }

        private void SetHiveKnightP5HpEnabled(bool value)
        {
            Modules.BossChallenge.HiveKnightHelper.SetP5HpEnabled(value);
            RefreshHiveKnightHelperUi();
        }

        private void SetHiveKnightMaxHp(int value)
        {
            if (Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp)
            {
                Modules.BossChallenge.HiveKnightHelper.hiveKnightMaxHp = 850;
                RefreshHiveKnightHelperUi();
                return;
            }

            Modules.BossChallenge.HiveKnightHelper.hiveKnightMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.HiveKnightHelper.hiveKnightUseMaxHp)
            {
                Modules.BossChallenge.HiveKnightHelper.ApplyHiveKnightHealthIfPresent();
            }
        }

        private void SetHiveKnightPhase2Hp(int value)
        {
            if (Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp)
            {
                RefreshHiveKnightHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(value, 2, 1300);
            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase2Hp = phase2Hp;

            int maxPhase3Hp = Mathf.Max(1, phase2Hp - 1);
            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase3Hp = Mathf.Clamp(Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase3Hp, 1, maxPhase3Hp);

            if (Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase)
            {
                Modules.BossChallenge.HiveKnightHelper.ReapplyLiveSettings();
            }

            RefreshHiveKnightHelperUi();
        }

        private void SetHiveKnightPhase3Hp(int value)
        {
            if (Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp)
            {
                RefreshHiveKnightHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase2Hp, 2, 1300);
            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase2Hp = phase2Hp;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase3Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase)
            {
                Modules.BossChallenge.HiveKnightHelper.ReapplyLiveSettings();
            }

            RefreshHiveKnightHelperUi();
        }

        private void RefreshHiveKnightHelperUi()
        {
            Module? module = GetHiveKnightHelperModule();
            UpdateToggleValue(hiveKnightHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(hiveKnightHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(hiveKnightP5HpValue, Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp);
            UpdateToggleValue(hiveKnightUseMaxHpValue, Modules.BossChallenge.HiveKnightHelper.hiveKnightUseMaxHp);
            UpdateToggleValue(hiveKnightUseCustomPhaseValue, Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase);
            UpdateIntInputValue(hiveKnightMaxHpField, Modules.BossChallenge.HiveKnightHelper.hiveKnightMaxHp);
            UpdateIntInputValue(hiveKnightPhase2HpField, Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase2Hp);
            UpdateIntInputValue(hiveKnightPhase3HpField, Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase3Hp);

            UpdateHiveKnightHelperInteractivity();
        }

        private void UpdateHiveKnightHelperInteractivity()
        {
            SetContentInteractivity(hiveKnightHelperContent, GetHiveKnightHelperEnabled(), "HiveKnightHelperEnableRow");
            if (!GetHiveKnightHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.HiveKnightHelper.hiveKnightUseMaxHp;
            bool p5Hp = Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp;
            bool useCustomPhase = Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase;
            SetRowInteractivity(hiveKnightHelperContent, "HiveKnightUseMaxHpRow", !p5Hp);
            SetRowInteractivity(hiveKnightHelperContent, "HiveKnightMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(hiveKnightHelperContent, "HiveKnightUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(hiveKnightHelperContent, "HiveKnightPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(hiveKnightHelperContent, "HiveKnightPhase3HpRow", useCustomPhase && !p5Hp);
        }

        private void SetHiveKnightHelperVisible(bool value)
        {
            hiveKnightHelperVisible = value;
            if (hiveKnightHelperRoot != null)
            {
                hiveKnightHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshHiveKnightHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetBrokenVesselHelperModule()
        {
            if (brokenVesselHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.BrokenVesselHelper), out brokenVesselHelperModule);
            }

            return brokenVesselHelperModule;
        }

        private bool GetBrokenVesselHelperEnabled()
        {
            Module? module = GetBrokenVesselHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetBrokenVesselHelperEnabled(bool value)
        {
            Module? module = GetBrokenVesselHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateBrokenVesselHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetBrokenVesselUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseMaxHp = true;
                RefreshBrokenVesselHelperUi();
                return;
            }

            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseMaxHp = value;
            Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase = false;
                RefreshBrokenVesselHelperUi();
                return;
            }

            NormalizeBrokenVesselPhaseThresholds();
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase = value;
            Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselUseCustomSummonHpEnabled(bool value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonHp = false;
                RefreshBrokenVesselHelperUi();
                return;
            }

            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonHp = value;
            Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselUseCustomSummonLimitEnabled(bool value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonLimit = false;
                RefreshBrokenVesselHelperUi();
                return;
            }

            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonLimit = value;
            Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselP5HpEnabled(bool value)
        {
            Modules.BossChallenge.BrokenVesselHelper.SetP5HpEnabled(value);
            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselMaxHp(int value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselMaxHp = 700;
                RefreshBrokenVesselHelperUi();
                return;
            }

            Modules.BossChallenge.BrokenVesselHelper.brokenVesselMaxHp = Mathf.Clamp(value, 1, 999999);
            NormalizeBrokenVesselPhaseThresholds();
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseMaxHp
                || Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase)
            {
                Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            }

            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselPhase2Hp(int value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                RefreshBrokenVesselHelperUi();
                return;
            }

            int phase2Max = GetBrokenVesselPhase2Max();
            int phase2Hp = Mathf.Clamp(value, 4, phase2Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp = Mathf.Clamp(
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp,
                3,
                phase3Max);

            int phase4Max = Mathf.Max(2, Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp - 1);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp = Mathf.Clamp(
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp,
                2,
                phase4Max);

            int phase5Max = Mathf.Max(1, Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp - 1);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp = Mathf.Clamp(
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp,
                1,
                phase5Max);

            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase)
            {
                Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            }

            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselPhase3Hp(int value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                RefreshBrokenVesselHelperUi();
                return;
            }

            int phase2Max = GetBrokenVesselPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp, 4, phase2Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(value, 3, phase3Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp = phase3Hp;

            int phase4Max = Mathf.Max(2, phase3Hp - 1);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp = Mathf.Clamp(
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp,
                2,
                phase4Max);

            int phase5Max = Mathf.Max(1, Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp - 1);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp = Mathf.Clamp(
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp,
                1,
                phase5Max);

            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase)
            {
                Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            }

            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselPhase4Hp(int value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                RefreshBrokenVesselHelperUi();
                return;
            }

            int phase2Max = GetBrokenVesselPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp, 4, phase2Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp, 3, phase3Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp = phase3Hp;

            int phase4Max = Mathf.Max(2, phase3Hp - 1);
            int phase4Hp = Mathf.Clamp(value, 2, phase4Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp = phase4Hp;

            int phase5Max = Mathf.Max(1, phase4Hp - 1);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp = Mathf.Clamp(
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp,
                1,
                phase5Max);

            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase)
            {
                Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            }

            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselPhase5Hp(int value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                RefreshBrokenVesselHelperUi();
                return;
            }

            int phase2Max = GetBrokenVesselPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp, 4, phase2Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp, 3, phase3Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp = phase3Hp;

            int phase4Max = Mathf.Max(2, phase3Hp - 1);
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp, 2, phase4Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp = phase4Hp;

            int phase5Max = Mathf.Max(1, phase4Hp - 1);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp = Mathf.Clamp(value, 1, phase5Max);

            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase)
            {
                Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            }

            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselSummonHp(int value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                RefreshBrokenVesselHelperUi();
                return;
            }

            Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonHp)
            {
                Modules.BossChallenge.BrokenVesselHelper.ApplyBrokenVesselSummonHealthIfPresent();
            }

            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselSummonLimit(int value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                RefreshBrokenVesselHelperUi();
                return;
            }

            Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonLimit = Mathf.Clamp(value, 0, 999);
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonLimit)
            {
                Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            }

            RefreshBrokenVesselHelperUi();
        }

        private int GetBrokenVesselPhase2Max()
        {
            return Mathf.Max(4, Modules.BossChallenge.BrokenVesselHelper.brokenVesselMaxHp);
        }

        private void NormalizeBrokenVesselPhaseThresholds()
        {
            int phase2Max = GetBrokenVesselPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp, 4, phase2Max);
            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp, 3, phase3Max);
            int phase4Max = Mathf.Max(2, phase3Hp - 1);
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp, 2, phase4Max);
            int phase5Max = Mathf.Max(1, phase4Hp - 1);
            int phase5Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp, 1, phase5Max);

            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp = phase2Hp;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp = phase3Hp;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp = phase4Hp;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp = phase5Hp;
        }

        private void RefreshBrokenVesselHelperUi()
        {
            Module? module = GetBrokenVesselHelperModule();
            UpdateToggleValue(brokenVesselHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(brokenVesselHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(brokenVesselP5HpValue, Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp);
            UpdateToggleValue(brokenVesselUseMaxHpValue, Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseMaxHp);
            UpdateToggleValue(brokenVesselUseCustomPhaseValue, Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase);
            UpdateToggleValue(brokenVesselUseCustomSummonHpValue, Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonHp);
            UpdateToggleValue(brokenVesselUseCustomSummonLimitValue, Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonLimit);
            UpdateIntInputValue(brokenVesselMaxHpField, Modules.BossChallenge.BrokenVesselHelper.brokenVesselMaxHp);
            UpdateIntInputValue(brokenVesselPhase2HpField, Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp);
            UpdateIntInputValue(brokenVesselPhase3HpField, Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp);
            UpdateIntInputValue(brokenVesselPhase4HpField, Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp);
            UpdateIntInputValue(brokenVesselPhase5HpField, Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp);
            UpdateIntInputValue(brokenVesselSummonHpField, Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonHp);
            UpdateIntInputValue(brokenVesselSummonLimitField, Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonLimit);

            UpdateBrokenVesselHelperInteractivity();
        }

        private void UpdateBrokenVesselHelperInteractivity()
        {
            SetContentInteractivity(brokenVesselHelperContent, GetBrokenVesselHelperEnabled(), "BrokenVesselHelperEnableRow");
            if (!GetBrokenVesselHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseMaxHp;
            bool p5Hp = Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp;
            bool useCustomPhase = Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase;
            bool useCustomSummonHp = Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonHp;
            bool useCustomSummonLimit = Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonLimit;
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselUseMaxHpRow", !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselPhase3HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselPhase4HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselPhase5HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselUseCustomSummonHpRow", !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselSummonHpRow", useCustomSummonHp && !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselUseCustomSummonLimitRow", !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselSummonLimitRow", useCustomSummonLimit && !p5Hp);
        }

        private void SetBrokenVesselHelperVisible(bool value)
        {
            brokenVesselHelperVisible = value;
            if (brokenVesselHelperRoot != null)
            {
                brokenVesselHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshBrokenVesselHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetLostKinHelperModule()
        {
            if (lostKinHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.LostKinHelper), out lostKinHelperModule);
            }

            return lostKinHelperModule;
        }

        private bool GetLostKinHelperEnabled()
        {
            Module? module = GetLostKinHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetLostKinHelperEnabled(bool value)
        {
            Module? module = GetLostKinHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateLostKinHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetLostKinUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                Modules.BossChallenge.LostKinHelper.lostKinUseMaxHp = true;
                RefreshLostKinHelperUi();
                return;
            }

            Modules.BossChallenge.LostKinHelper.lostKinUseMaxHp = value;
            Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            RefreshLostKinHelperUi();
        }

        private void SetLostKinUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase = false;
                RefreshLostKinHelperUi();
                return;
            }

            NormalizeLostKinPhaseThresholds();
            Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase = value;
            Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            RefreshLostKinHelperUi();
        }

        private void SetLostKinUseCustomSummonHpEnabled(bool value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonHp = false;
                RefreshLostKinHelperUi();
                return;
            }

            Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonHp = value;
            Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            RefreshLostKinHelperUi();
        }

        private void SetLostKinUseCustomSummonLimitEnabled(bool value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonLimit = false;
                RefreshLostKinHelperUi();
                return;
            }

            Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonLimit = value;
            Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            RefreshLostKinHelperUi();
        }

        private void SetLostKinP5HpEnabled(bool value)
        {
            Modules.BossChallenge.LostKinHelper.SetP5HpEnabled(value);
            RefreshLostKinHelperUi();
        }

        private void SetLostKinMaxHp(int value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                Modules.BossChallenge.LostKinHelper.lostKinMaxHp = 1200;
                RefreshLostKinHelperUi();
                return;
            }

            Modules.BossChallenge.LostKinHelper.lostKinMaxHp = Mathf.Clamp(value, 1, 999999);
            NormalizeLostKinPhaseThresholds();
            if (Modules.BossChallenge.LostKinHelper.lostKinUseMaxHp
                || Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase)
            {
                Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            }

            RefreshLostKinHelperUi();
        }

        private void SetLostKinPhase2Hp(int value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                RefreshLostKinHelperUi();
                return;
            }

            int phase2Max = GetLostKinPhase2Max();
            int phase2Hp = Mathf.Clamp(value, 4, phase2Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp = Mathf.Clamp(
                Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp,
                3,
                phase3Max);

            int phase4Max = Mathf.Max(2, Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp - 1);
            Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp = Mathf.Clamp(
                Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp,
                2,
                phase4Max);

            int phase5Max = Mathf.Max(1, Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp - 1);
            Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp = Mathf.Clamp(
                Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp,
                1,
                phase5Max);

            if (Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase)
            {
                Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            }

            RefreshLostKinHelperUi();
        }

        private void SetLostKinPhase3Hp(int value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                RefreshLostKinHelperUi();
                return;
            }

            int phase2Max = GetLostKinPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp, 4, phase2Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(value, 3, phase3Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp = phase3Hp;

            int phase4Max = Mathf.Max(2, phase3Hp - 1);
            Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp = Mathf.Clamp(
                Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp,
                2,
                phase4Max);

            int phase5Max = Mathf.Max(1, Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp - 1);
            Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp = Mathf.Clamp(
                Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp,
                1,
                phase5Max);

            if (Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase)
            {
                Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            }

            RefreshLostKinHelperUi();
        }

        private void SetLostKinPhase4Hp(int value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                RefreshLostKinHelperUi();
                return;
            }

            int phase2Max = GetLostKinPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp, 4, phase2Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp, 3, phase3Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp = phase3Hp;

            int phase4Max = Mathf.Max(2, phase3Hp - 1);
            int phase4Hp = Mathf.Clamp(value, 2, phase4Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp = phase4Hp;

            int phase5Max = Mathf.Max(1, phase4Hp - 1);
            Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp = Mathf.Clamp(
                Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp,
                1,
                phase5Max);

            if (Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase)
            {
                Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            }

            RefreshLostKinHelperUi();
        }

        private void SetLostKinPhase5Hp(int value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                RefreshLostKinHelperUi();
                return;
            }

            int phase2Max = GetLostKinPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp, 4, phase2Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp, 3, phase3Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp = phase3Hp;

            int phase4Max = Mathf.Max(2, phase3Hp - 1);
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp, 2, phase4Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp = phase4Hp;

            int phase5Max = Mathf.Max(1, phase4Hp - 1);
            Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp = Mathf.Clamp(value, 1, phase5Max);

            if (Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase)
            {
                Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            }

            RefreshLostKinHelperUi();
        }

        private void SetLostKinSummonHp(int value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                RefreshLostKinHelperUi();
                return;
            }

            Modules.BossChallenge.LostKinHelper.lostKinSummonHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonHp)
            {
                Modules.BossChallenge.LostKinHelper.ApplyLostKinSummonHealthIfPresent();
            }

            RefreshLostKinHelperUi();
        }

        private void SetLostKinSummonLimit(int value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                RefreshLostKinHelperUi();
                return;
            }

            Modules.BossChallenge.LostKinHelper.lostKinSummonLimit = Mathf.Clamp(value, 0, 999);
            if (Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonLimit)
            {
                Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            }

            RefreshLostKinHelperUi();
        }

        private int GetLostKinPhase2Max()
        {
            return Mathf.Max(4, Modules.BossChallenge.LostKinHelper.lostKinMaxHp);
        }

        private void NormalizeLostKinPhaseThresholds()
        {
            int phase2Max = GetLostKinPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp, 4, phase2Max);
            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp, 3, phase3Max);
            int phase4Max = Mathf.Max(2, phase3Hp - 1);
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp, 2, phase4Max);
            int phase5Max = Mathf.Max(1, phase4Hp - 1);
            int phase5Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp, 1, phase5Max);

            Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp = phase2Hp;
            Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp = phase3Hp;
            Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp = phase4Hp;
            Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp = phase5Hp;
        }

        private void RefreshLostKinHelperUi()
        {
            Module? module = GetLostKinHelperModule();
            UpdateToggleValue(lostKinHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(lostKinHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(lostKinP5HpValue, Modules.BossChallenge.LostKinHelper.lostKinP5Hp);
            UpdateToggleValue(lostKinUseMaxHpValue, Modules.BossChallenge.LostKinHelper.lostKinUseMaxHp);
            UpdateToggleValue(lostKinUseCustomPhaseValue, Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase);
            UpdateToggleValue(lostKinUseCustomSummonHpValue, Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonHp);
            UpdateToggleValue(lostKinUseCustomSummonLimitValue, Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonLimit);
            UpdateIntInputValue(lostKinMaxHpField, Modules.BossChallenge.LostKinHelper.lostKinMaxHp);
            UpdateIntInputValue(lostKinPhase2HpField, Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp);
            UpdateIntInputValue(lostKinPhase3HpField, Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp);
            UpdateIntInputValue(lostKinPhase4HpField, Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp);
            UpdateIntInputValue(lostKinPhase5HpField, Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp);
            UpdateIntInputValue(lostKinSummonHpField, Modules.BossChallenge.LostKinHelper.lostKinSummonHp);
            UpdateIntInputValue(lostKinSummonLimitField, Modules.BossChallenge.LostKinHelper.lostKinSummonLimit);

            UpdateLostKinHelperInteractivity();
        }

        private void UpdateLostKinHelperInteractivity()
        {
            SetContentInteractivity(lostKinHelperContent, GetLostKinHelperEnabled(), "LostKinHelperEnableRow");
            if (!GetLostKinHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.LostKinHelper.lostKinUseMaxHp;
            bool p5Hp = Modules.BossChallenge.LostKinHelper.lostKinP5Hp;
            bool useCustomPhase = Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase;
            bool useCustomSummonHp = Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonHp;
            bool useCustomSummonLimit = Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonLimit;
            SetRowInteractivity(lostKinHelperContent, "LostKinUseMaxHpRow", !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinPhase3HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinPhase4HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinPhase5HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinUseCustomSummonHpRow", !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinSummonHpRow", useCustomSummonHp && !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinUseCustomSummonLimitRow", !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinSummonLimitRow", useCustomSummonLimit && !p5Hp);
        }

        private void SetLostKinHelperVisible(bool value)
        {
            lostKinHelperVisible = value;
            if (lostKinHelperRoot != null)
            {
                lostKinHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshLostKinHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetWingedNoskHelperModule()
        {
            if (wingedNoskHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.WingedNoskHelper), out wingedNoskHelperModule);
            }

            return wingedNoskHelperModule;
        }

        private bool GetWingedNoskHelperEnabled()
        {
            Module? module = GetWingedNoskHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetWingedNoskHelperEnabled(bool value)
        {
            Module? module = GetWingedNoskHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateWingedNoskHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetWingedNoskUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp)
            {
                Modules.BossChallenge.WingedNoskHelper.wingedNoskUseMaxHp = true;
                RefreshWingedNoskHelperUi();
                return;
            }

            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseMaxHp = value;
            Modules.BossChallenge.WingedNoskHelper.ReapplyLiveSettings();
            RefreshWingedNoskHelperUi();
        }

        private void SetWingedNoskP5HpEnabled(bool value)
        {
            Modules.BossChallenge.WingedNoskHelper.SetP5HpEnabled(value);
            RefreshWingedNoskHelperUi();
        }

        private void SetWingedNoskMaxHp(int value)
        {
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp)
            {
                Modules.BossChallenge.WingedNoskHelper.wingedNoskMaxHp = 750;
                Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2Hp = 375;
                RefreshWingedNoskHelperUi();
                return;
            }

            Modules.BossChallenge.WingedNoskHelper.wingedNoskMaxHp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2Hp = ClampWingedNoskPhase2HpForUi(Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2Hp);
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskUseMaxHp)
            {
                Modules.BossChallenge.WingedNoskHelper.ApplyWingedNoskHealthIfPresent();
            }

            Modules.BossChallenge.WingedNoskHelper.ApplyPhaseThresholdSettingsIfPresent();
            RefreshWingedNoskHelperUi();
        }

        private void SetWingedNoskUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp)
            {
                Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomPhase = false;
                RefreshWingedNoskHelperUi();
                return;
            }

            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomPhase = value;
            Modules.BossChallenge.WingedNoskHelper.ApplyPhaseThresholdSettingsIfPresent();
            RefreshWingedNoskHelperUi();
        }

        private void SetWingedNoskPhase2Hp(int value)
        {
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp)
            {
                Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2Hp = 375;
                RefreshWingedNoskHelperUi();
                return;
            }

            int clamped = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2Hp = ClampWingedNoskPhase2HpForUi(clamped);
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomPhase)
            {
                Modules.BossChallenge.WingedNoskHelper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshWingedNoskHelperUi();
        }

        private void SetWingedNoskUseCustomSummonHpEnabled(bool value)
        {
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp)
            {
                Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonHp = false;
                RefreshWingedNoskHelperUi();
                return;
            }

            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonHp = value;
            Modules.BossChallenge.WingedNoskHelper.ReapplyLiveSettings();
            RefreshWingedNoskHelperUi();
        }

        private void SetWingedNoskSummonHp(int value)
        {
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp)
            {
                Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonHp = 1;
                RefreshWingedNoskHelperUi();
                return;
            }

            Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonHp)
            {
                Modules.BossChallenge.WingedNoskHelper.ApplyWingedNoskSummonHealthIfPresent();
            }

            RefreshWingedNoskHelperUi();
        }

        private void SetWingedNoskUseCustomSummonLimitEnabled(bool value)
        {
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp)
            {
                Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonLimit = false;
                RefreshWingedNoskHelperUi();
                return;
            }

            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonLimit = value;
            Modules.BossChallenge.WingedNoskHelper.ReapplyLiveSettings();
            RefreshWingedNoskHelperUi();
        }

        private void SetWingedNoskSummonLimit(int value)
        {
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp)
            {
                Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonLimit = 5;
                RefreshWingedNoskHelperUi();
                return;
            }

            Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonLimit = Mathf.Clamp(value, 0, 999);
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonLimit)
            {
                Modules.BossChallenge.WingedNoskHelper.ApplySummonLimitSettingsIfPresent();
            }

            RefreshWingedNoskHelperUi();
        }

        private static int ClampWingedNoskPhase2HpForUi(int value)
        {
            int maxHp = Mathf.Clamp(Modules.BossChallenge.WingedNoskHelper.wingedNoskMaxHp, 1, 999999);
            return Mathf.Clamp(value, 1, maxHp);
        }

        private void RefreshWingedNoskHelperUi()
        {
            Module? module = GetWingedNoskHelperModule();
            UpdateToggleValue(wingedNoskHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(wingedNoskHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(wingedNoskP5HpValue, Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp);
            UpdateToggleValue(wingedNoskUseMaxHpValue, Modules.BossChallenge.WingedNoskHelper.wingedNoskUseMaxHp);
            UpdateToggleValue(wingedNoskUseCustomPhaseValue, Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomPhase);
            UpdateToggleValue(wingedNoskUseCustomSummonHpValue, Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonHp);
            UpdateToggleValue(wingedNoskUseCustomSummonLimitValue, Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonLimit);
            UpdateIntInputValue(wingedNoskMaxHpField, Modules.BossChallenge.WingedNoskHelper.wingedNoskMaxHp);
            UpdateIntInputValue(wingedNoskPhase2HpField, Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2Hp);
            UpdateIntInputValue(wingedNoskSummonHpField, Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonHp);
            UpdateIntInputValue(wingedNoskSummonLimitField, Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonLimit);

            UpdateWingedNoskHelperInteractivity();
        }

        private void UpdateWingedNoskHelperInteractivity()
        {
            SetContentInteractivity(wingedNoskHelperContent, GetWingedNoskHelperEnabled(), "WingedNoskHelperEnableRow");
            if (!GetWingedNoskHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.WingedNoskHelper.wingedNoskUseMaxHp;
            bool p5Hp = Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp;
            bool useCustomPhase = Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomPhase;
            bool useCustomSummonHp = Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonHp;
            bool useCustomSummonLimit = Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonLimit;
            SetRowInteractivity(wingedNoskHelperContent, "WingedNoskUseMaxHpRow", !p5Hp);
            SetRowInteractivity(wingedNoskHelperContent, "WingedNoskMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(wingedNoskHelperContent, "WingedNoskUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(wingedNoskHelperContent, "WingedNoskPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(wingedNoskHelperContent, "WingedNoskUseCustomSummonHpRow", !p5Hp);
            SetRowInteractivity(wingedNoskHelperContent, "WingedNoskSummonHpRow", useCustomSummonHp && !p5Hp);
            SetRowInteractivity(wingedNoskHelperContent, "WingedNoskUseCustomSummonLimitRow", !p5Hp);
            SetRowInteractivity(wingedNoskHelperContent, "WingedNoskSummonLimitRow", useCustomSummonLimit && !p5Hp);
        }

        private void SetWingedNoskHelperVisible(bool value)
        {
            wingedNoskHelperVisible = value;
            if (wingedNoskHelperRoot != null)
            {
                wingedNoskHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshWingedNoskHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetUumuuHelperModule()
        {
            if (uumuuHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.UumuuHelper), out uumuuHelperModule);
            }

            return uumuuHelperModule;
        }

        private bool GetUumuuHelperEnabled()
        {
            Module? module = GetUumuuHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetUumuuHelperEnabled(bool value)
        {
            Module? module = GetUumuuHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateUumuuHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetUumuuUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.UumuuHelper.uumuuP5Hp)
            {
                Modules.BossChallenge.UumuuHelper.uumuuUseMaxHp = true;
                RefreshUumuuHelperUi();
                return;
            }

            Modules.BossChallenge.UumuuHelper.uumuuUseMaxHp = value;
            Modules.BossChallenge.UumuuHelper.ReapplyLiveSettings();
            RefreshUumuuHelperUi();
        }

        private void SetUumuuUseCustomSummonHpEnabled(bool value)
        {
            if (Modules.BossChallenge.UumuuHelper.uumuuP5Hp)
            {
                Modules.BossChallenge.UumuuHelper.uumuuUseCustomSummonHp = false;
                RefreshUumuuHelperUi();
                return;
            }

            Modules.BossChallenge.UumuuHelper.uumuuUseCustomSummonHp = value;
            Modules.BossChallenge.UumuuHelper.ReapplyLiveSettings();
            RefreshUumuuHelperUi();
        }

        private void SetUumuuP5HpEnabled(bool value)
        {
            Modules.BossChallenge.UumuuHelper.SetP5HpEnabled(value);
            RefreshUumuuHelperUi();
        }

        private void SetUumuuMaxHp(int value)
        {
            if (Modules.BossChallenge.UumuuHelper.uumuuP5Hp)
            {
                Modules.BossChallenge.UumuuHelper.uumuuMaxHp = 350;
                RefreshUumuuHelperUi();
                return;
            }

            Modules.BossChallenge.UumuuHelper.uumuuMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.UumuuHelper.uumuuUseMaxHp)
            {
                Modules.BossChallenge.UumuuHelper.ApplyUumuuHealthIfPresent();
            }

            RefreshUumuuHelperUi();
        }

        private void SetUumuuSummonHp(int value)
        {
            if (Modules.BossChallenge.UumuuHelper.uumuuP5Hp)
            {
                Modules.BossChallenge.UumuuHelper.uumuuSummonHp = 1;
                RefreshUumuuHelperUi();
                return;
            }

            Modules.BossChallenge.UumuuHelper.uumuuSummonHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.UumuuHelper.uumuuUseCustomSummonHp)
            {
                Modules.BossChallenge.UumuuHelper.ApplyUumuuSummonHealthIfPresent();
            }

            RefreshUumuuHelperUi();
        }

        private void RefreshUumuuHelperUi()
        {
            Module? module = GetUumuuHelperModule();
            UpdateToggleValue(uumuuHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(uumuuHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(uumuuP5HpValue, Modules.BossChallenge.UumuuHelper.uumuuP5Hp);
            UpdateToggleValue(uumuuUseMaxHpValue, Modules.BossChallenge.UumuuHelper.uumuuUseMaxHp);
            UpdateToggleValue(uumuuUseCustomSummonHpValue, Modules.BossChallenge.UumuuHelper.uumuuUseCustomSummonHp);
            UpdateIntInputValue(uumuuMaxHpField, Modules.BossChallenge.UumuuHelper.uumuuMaxHp);
            UpdateIntInputValue(uumuuSummonHpField, Modules.BossChallenge.UumuuHelper.uumuuSummonHp);

            UpdateUumuuHelperInteractivity();
        }

        private void UpdateUumuuHelperInteractivity()
        {
            SetContentInteractivity(uumuuHelperContent, GetUumuuHelperEnabled(), "UumuuHelperEnableRow");
            if (!GetUumuuHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.UumuuHelper.uumuuUseMaxHp;
            bool p5Hp = Modules.BossChallenge.UumuuHelper.uumuuP5Hp;
            bool useCustomSummonHp = Modules.BossChallenge.UumuuHelper.uumuuUseCustomSummonHp;
            SetRowInteractivity(uumuuHelperContent, "UumuuUseMaxHpRow", !p5Hp);
            SetRowInteractivity(uumuuHelperContent, "UumuuMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(uumuuHelperContent, "UumuuUseCustomSummonHpRow", !p5Hp);
            SetRowInteractivity(uumuuHelperContent, "UumuuSummonHpRow", useCustomSummonHp && !p5Hp);
        }

        private void SetUumuuHelperVisible(bool value)
        {
            uumuuHelperVisible = value;
            if (uumuuHelperRoot != null)
            {
                uumuuHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshUumuuHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetNoskHelperModule()
        {
            if (noskHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.NoskHelper), out noskHelperModule);
            }

            return noskHelperModule;
        }

        private bool GetNoskHelperEnabled()
        {
            Module? module = GetNoskHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetNoskHelperEnabled(bool value)
        {
            Module? module = GetNoskHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateNoskHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetNoskUseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.NoskHelper.noskUseMaxHp = value;
            Modules.BossChallenge.NoskHelper.ReapplyLiveSettings();
            RefreshNoskHelperUi();
        }

        private void SetNoskUseCustomPhaseEnabled(bool value)
        {
            int maxPhase2Hp = GetNoskPhase2MaxHp();
            Modules.BossChallenge.NoskHelper.noskPhase2Hp =
                Mathf.Clamp(Modules.BossChallenge.NoskHelper.noskPhase2Hp, 1, maxPhase2Hp);
            Modules.BossChallenge.NoskHelper.noskUseCustomPhase = value;
            Modules.BossChallenge.NoskHelper.ReapplyLiveSettings();
            RefreshNoskHelperUi();
        }

        private void SetNoskMaxHp(int value)
        {
            Modules.BossChallenge.NoskHelper.noskMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.NoskHelper.noskUseMaxHp)
            {
                Modules.BossChallenge.NoskHelper.ApplyNoskHealthIfPresent();
            }

            int maxPhase2Hp = GetNoskPhase2MaxHp();
            Modules.BossChallenge.NoskHelper.noskPhase2Hp =
                Mathf.Clamp(Modules.BossChallenge.NoskHelper.noskPhase2Hp, 1, maxPhase2Hp);
            if (Modules.BossChallenge.NoskHelper.noskUseCustomPhase)
            {
                Modules.BossChallenge.NoskHelper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshNoskHelperUi();
        }

        private void SetNoskPhase2Hp(int value)
        {
            int maxPhase2Hp = GetNoskPhase2MaxHp();
            Modules.BossChallenge.NoskHelper.noskPhase2Hp = Mathf.Clamp(value, 1, maxPhase2Hp);
            if (Modules.BossChallenge.NoskHelper.noskUseCustomPhase)
            {
                Modules.BossChallenge.NoskHelper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshNoskHelperUi();
        }

        private int GetNoskPhase2MaxHp()
        {
            return Mathf.Max(1, Modules.BossChallenge.NoskHelper.GetPhase2MaxHpForUi());
        }

        private void RefreshNoskHelperUi()
        {
            Module? module = GetNoskHelperModule();
            UpdateToggleValue(noskHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(noskHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(noskUseMaxHpValue, Modules.BossChallenge.NoskHelper.noskUseMaxHp);
            UpdateToggleValue(noskUseCustomPhaseValue, Modules.BossChallenge.NoskHelper.noskUseCustomPhase);
            UpdateIntInputValue(noskMaxHpField, Modules.BossChallenge.NoskHelper.noskMaxHp);
            UpdateIntInputValue(noskPhase2HpField, Modules.BossChallenge.NoskHelper.noskPhase2Hp);

            UpdateNoskHelperInteractivity();
        }

        private void UpdateNoskHelperInteractivity()
        {
            SetContentInteractivity(noskHelperContent, GetNoskHelperEnabled(), "NoskHelperEnableRow");
            if (!GetNoskHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.NoskHelper.noskUseMaxHp;
            bool useCustomPhase = Modules.BossChallenge.NoskHelper.noskUseCustomPhase;
            SetRowInteractivity(noskHelperContent, "NoskMaxHpRow", useMaxHp);
            SetRowInteractivity(noskHelperContent, "NoskPhase2HpRow", useCustomPhase);
        }

        private void SetNoskHelperVisible(bool value)
        {
            noskHelperVisible = value;
            if (noskHelperRoot != null)
            {
                noskHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshNoskHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetTraitorLordHelperModule()
        {
            if (traitorLordHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.TraitorLordHelper), out traitorLordHelperModule);
            }

            return traitorLordHelperModule;
        }

        private bool GetTraitorLordHelperEnabled()
        {
            Module? module = GetTraitorLordHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetTraitorLordHelperEnabled(bool value)
        {
            Module? module = GetTraitorLordHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateTraitorLordHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetTraitorLordUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.TraitorLordHelper.traitorLordP5Hp)
            {
                Modules.BossChallenge.TraitorLordHelper.traitorLordUseMaxHp = true;
                RefreshTraitorLordHelperUi();
                return;
            }

            Modules.BossChallenge.TraitorLordHelper.traitorLordUseMaxHp = value;
            Modules.BossChallenge.TraitorLordHelper.ReapplyLiveSettings();
            RefreshTraitorLordHelperUi();
        }

        private void SetTraitorLordUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.TraitorLordHelper.traitorLordP5Hp)
            {
                Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhase = false;
                RefreshTraitorLordHelperUi();
                return;
            }

            int maxPhase2Hp = GetTraitorLordPhase2MaxHp();
            Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp =
                Mathf.Clamp(Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp, 1, maxPhase2Hp);
            Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhase = value;
            Modules.BossChallenge.TraitorLordHelper.ReapplyLiveSettings();
            RefreshTraitorLordHelperUi();
        }

        private void SetTraitorLordP5HpEnabled(bool value)
        {
            Modules.BossChallenge.TraitorLordHelper.SetP5HpEnabled(value);
            RefreshTraitorLordHelperUi();
        }

        private void SetTraitorLordMaxHp(int value)
        {
            if (Modules.BossChallenge.TraitorLordHelper.traitorLordP5Hp)
            {
                Modules.BossChallenge.TraitorLordHelper.traitorLordMaxHp = 800;
                RefreshTraitorLordHelperUi();
                return;
            }

            Modules.BossChallenge.TraitorLordHelper.traitorLordMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.TraitorLordHelper.traitorLordUseMaxHp)
            {
                Modules.BossChallenge.TraitorLordHelper.ApplyTraitorLordHealthIfPresent();
            }

            int maxPhase2Hp = GetTraitorLordPhase2MaxHp();
            Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp =
                Mathf.Clamp(Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp, 1, maxPhase2Hp);
            if (Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhase)
            {
                Modules.BossChallenge.TraitorLordHelper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshTraitorLordHelperUi();
        }

        private void SetTraitorLordPhase2Hp(int value)
        {
            if (Modules.BossChallenge.TraitorLordHelper.traitorLordP5Hp)
            {
                Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp = 500;
                RefreshTraitorLordHelperUi();
                return;
            }

            int maxPhase2Hp = GetTraitorLordPhase2MaxHp();
            Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp = Mathf.Clamp(value, 1, maxPhase2Hp);
            if (Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhase)
            {
                Modules.BossChallenge.TraitorLordHelper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshTraitorLordHelperUi();
        }

        private int GetTraitorLordPhase2MaxHp()
        {
            return Mathf.Max(1, Modules.BossChallenge.TraitorLordHelper.GetPhase2MaxHpForUi());
        }

        private void RefreshTraitorLordHelperUi()
        {
            Module? module = GetTraitorLordHelperModule();
            UpdateToggleValue(traitorLordHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(traitorLordHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(traitorLordP5HpValue, Modules.BossChallenge.TraitorLordHelper.traitorLordP5Hp);
            UpdateToggleValue(traitorLordUseMaxHpValue, Modules.BossChallenge.TraitorLordHelper.traitorLordUseMaxHp);
            UpdateToggleValue(traitorLordUseCustomPhaseValue, Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhase);
            UpdateIntInputValue(traitorLordMaxHpField, Modules.BossChallenge.TraitorLordHelper.traitorLordMaxHp);
            UpdateIntInputValue(traitorLordPhase2HpField, Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp);

            UpdateTraitorLordHelperInteractivity();
        }

        private void UpdateTraitorLordHelperInteractivity()
        {
            SetContentInteractivity(traitorLordHelperContent, GetTraitorLordHelperEnabled(), "TraitorLordHelperEnableRow");
            if (!GetTraitorLordHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.TraitorLordHelper.traitorLordUseMaxHp;
            bool p5Hp = Modules.BossChallenge.TraitorLordHelper.traitorLordP5Hp;
            bool useCustomPhase = Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhase;
            SetRowInteractivity(traitorLordHelperContent, "TraitorLordUseMaxHpRow", !p5Hp);
            SetRowInteractivity(traitorLordHelperContent, "TraitorLordMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(traitorLordHelperContent, "TraitorLordUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(traitorLordHelperContent, "TraitorLordPhase2HpRow", useCustomPhase && !p5Hp);
        }

        private void SetTraitorLordHelperVisible(bool value)
        {
            traitorLordHelperVisible = value;
            if (traitorLordHelperRoot != null)
            {
                traitorLordHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshTraitorLordHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetTroupeMasterGrimmHelperModule()
        {
            if (troupeMasterGrimmHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.TroupeMasterGrimmHelper), out troupeMasterGrimmHelperModule);
            }

            return troupeMasterGrimmHelperModule;
        }

        private bool GetTroupeMasterGrimmHelperEnabled()
        {
            Module? module = GetTroupeMasterGrimmHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetTroupeMasterGrimmHelperEnabled(bool value)
        {
            Module? module = GetTroupeMasterGrimmHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateTroupeMasterGrimmHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetTroupeMasterGrimmUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp)
            {
                Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseMaxHp = true;
                RefreshTroupeMasterGrimmHelperUi();
                return;
            }

            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseMaxHp = value;
            Modules.BossChallenge.TroupeMasterGrimmHelper.ReapplyLiveSettings();
            RefreshTroupeMasterGrimmHelperUi();
        }

        private void SetTroupeMasterGrimmUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp)
            {
                Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase = false;
                RefreshTroupeMasterGrimmHelperUi();
                return;
            }

            NormalizeTroupeMasterGrimmPhaseThresholds();
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase = value;
            Modules.BossChallenge.TroupeMasterGrimmHelper.ReapplyLiveSettings();
            RefreshTroupeMasterGrimmHelperUi();
        }

        private void SetTroupeMasterGrimmP5HpEnabled(bool value)
        {
            Modules.BossChallenge.TroupeMasterGrimmHelper.SetP5HpEnabled(value);
            RefreshTroupeMasterGrimmHelperUi();
        }

        private void SetTroupeMasterGrimmMaxHp(int value)
        {
            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp)
            {
                Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmMaxHp = 1000;
                RefreshTroupeMasterGrimmHelperUi();
                return;
            }

            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmMaxHp = Mathf.Clamp(value, 1, 999999);
            NormalizeTroupeMasterGrimmPhaseThresholds();
            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseMaxHp
                || Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase)
            {
                Modules.BossChallenge.TroupeMasterGrimmHelper.ReapplyLiveSettings();
            }

            RefreshTroupeMasterGrimmHelperUi();
        }

        private void SetTroupeMasterGrimmPhase2Hp(int value)
        {
            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp)
            {
                RefreshTroupeMasterGrimmHelperUi();
                return;
            }

            int phase2Max = GetTroupeMasterGrimmPhase2Max();
            int phase2Hp = Mathf.Clamp(value, 1, phase2Max);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(1, phase2Hp - 1);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp = Mathf.Clamp(
                Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp,
                1,
                phase3Max);

            int phase4Max = Mathf.Max(1, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp - 1);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp = Mathf.Clamp(
                Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp,
                1,
                phase4Max);

            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase)
            {
                Modules.BossChallenge.TroupeMasterGrimmHelper.ReapplyLiveSettings();
            }

            RefreshTroupeMasterGrimmHelperUi();
        }

        private void SetTroupeMasterGrimmPhase3Hp(int value)
        {
            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp)
            {
                RefreshTroupeMasterGrimmHelperUi();
                return;
            }

            int phase2Max = GetTroupeMasterGrimmPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp, 1, phase2Max);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(1, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(value, 1, phase3Max);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp = phase3Hp;

            int phase4Max = Mathf.Max(1, phase3Hp - 1);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp = Mathf.Clamp(
                Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp,
                1,
                phase4Max);

            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase)
            {
                Modules.BossChallenge.TroupeMasterGrimmHelper.ReapplyLiveSettings();
            }

            RefreshTroupeMasterGrimmHelperUi();
        }

        private void SetTroupeMasterGrimmPhase4Hp(int value)
        {
            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp)
            {
                RefreshTroupeMasterGrimmHelperUi();
                return;
            }

            int phase2Max = GetTroupeMasterGrimmPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp, 1, phase2Max);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(1, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp, 1, phase3Max);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp = phase3Hp;

            int phase4Max = Mathf.Max(1, phase3Hp - 1);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp = Mathf.Clamp(value, 1, phase4Max);

            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase)
            {
                Modules.BossChallenge.TroupeMasterGrimmHelper.ReapplyLiveSettings();
            }

            RefreshTroupeMasterGrimmHelperUi();
        }

        private int GetTroupeMasterGrimmPhase2Max()
        {
            return Mathf.Max(1, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmMaxHp);
        }

        private void NormalizeTroupeMasterGrimmPhaseThresholds()
        {
            int phase2Max = GetTroupeMasterGrimmPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp, 1, phase2Max);
            int phase3Max = Mathf.Max(1, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp, 1, phase3Max);
            int phase4Max = Mathf.Max(1, phase3Hp - 1);
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp, 1, phase4Max);

            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp = phase2Hp;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp = phase3Hp;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp = phase4Hp;
        }

        private void RefreshTroupeMasterGrimmHelperUi()
        {
            Module? module = GetTroupeMasterGrimmHelperModule();
            UpdateToggleValue(troupeMasterGrimmHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(troupeMasterGrimmHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(troupeMasterGrimmP5HpValue, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp);
            UpdateToggleValue(troupeMasterGrimmUseMaxHpValue, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseMaxHp);
            UpdateToggleValue(troupeMasterGrimmUseCustomPhaseValue, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase);
            UpdateIntInputValue(troupeMasterGrimmMaxHpField, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmMaxHp);
            UpdateIntInputValue(troupeMasterGrimmPhase2HpField, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp);
            UpdateIntInputValue(troupeMasterGrimmPhase3HpField, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp);
            UpdateIntInputValue(troupeMasterGrimmPhase4HpField, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp);

            UpdateTroupeMasterGrimmHelperInteractivity();
        }

        private void UpdateTroupeMasterGrimmHelperInteractivity()
        {
            SetContentInteractivity(troupeMasterGrimmHelperContent, GetTroupeMasterGrimmHelperEnabled(), "TroupeMasterGrimmHelperEnableRow");
            if (!GetTroupeMasterGrimmHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseMaxHp;
            bool p5Hp = Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp;
            bool useCustomPhase = Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase;
            SetRowInteractivity(troupeMasterGrimmHelperContent, "TroupeMasterGrimmUseMaxHpRow", !p5Hp);
            SetRowInteractivity(troupeMasterGrimmHelperContent, "TroupeMasterGrimmMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(troupeMasterGrimmHelperContent, "TroupeMasterGrimmUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(troupeMasterGrimmHelperContent, "TroupeMasterGrimmPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(troupeMasterGrimmHelperContent, "TroupeMasterGrimmPhase3HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(troupeMasterGrimmHelperContent, "TroupeMasterGrimmPhase4HpRow", useCustomPhase && !p5Hp);
        }

        private void SetTroupeMasterGrimmHelperVisible(bool value)
        {
            troupeMasterGrimmHelperVisible = value;
            if (troupeMasterGrimmHelperRoot != null)
            {
                troupeMasterGrimmHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshTroupeMasterGrimmHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetNightmareKingGrimmHelperModule()
        {
            if (nightmareKingGrimmHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.NightmareKingGrimmHelper), out nightmareKingGrimmHelperModule);
            }

            return nightmareKingGrimmHelperModule;
        }

        private bool GetNightmareKingGrimmHelperEnabled()
        {
            Module? module = GetNightmareKingGrimmHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetNightmareKingGrimmHelperEnabled(bool value)
        {
            Module? module = GetNightmareKingGrimmHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateNightmareKingGrimmHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetNightmareKingGrimmUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp)
            {
                Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseMaxHp = true;
                RefreshNightmareKingGrimmHelperUi();
                return;
            }

            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseMaxHp = value;
            Modules.BossChallenge.NightmareKingGrimmHelper.ReapplyLiveSettings();
            RefreshNightmareKingGrimmHelperUi();
        }

        private void SetNightmareKingGrimmUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp)
            {
                Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase = false;
                RefreshNightmareKingGrimmHelperUi();
                return;
            }

            int ragePhase1Hp = Mathf.Clamp(Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1Hp, 3, 1650);
            int ragePhase2Max = Mathf.Max(2, ragePhase1Hp - 1);
            int ragePhase2Hp = Mathf.Clamp(Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2Hp, 2, ragePhase2Max);
            int ragePhase3Max = Mathf.Max(1, ragePhase2Hp - 1);
            int ragePhase3Hp = Mathf.Clamp(Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3Hp, 1, ragePhase3Max);

            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1Hp = ragePhase1Hp;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2Hp = ragePhase2Hp;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3Hp = ragePhase3Hp;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase = value;
            Modules.BossChallenge.NightmareKingGrimmHelper.ReapplyLiveSettings();
            RefreshNightmareKingGrimmHelperUi();
        }

        private void SetNightmareKingGrimmP5HpEnabled(bool value)
        {
            Modules.BossChallenge.NightmareKingGrimmHelper.SetP5HpEnabled(value);
            RefreshNightmareKingGrimmHelperUi();
        }

        private void SetNightmareKingGrimmMaxHp(int value)
        {
            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp)
            {
                Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmMaxHp = 1250;
                RefreshNightmareKingGrimmHelperUi();
                return;
            }

            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseMaxHp
                || Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase)
            {
                Modules.BossChallenge.NightmareKingGrimmHelper.ReapplyLiveSettings();
            }

            RefreshNightmareKingGrimmHelperUi();
        }

        private void SetNightmareKingGrimmRagePhase1Hp(int value)
        {
            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp)
            {
                RefreshNightmareKingGrimmHelperUi();
                return;
            }

            int ragePhase1Hp = Mathf.Clamp(value, 3, 1650);
            int ragePhase2Max = Mathf.Max(2, ragePhase1Hp - 1);
            int ragePhase2Hp = Mathf.Clamp(Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2Hp, 2, ragePhase2Max);
            int ragePhase3Max = Mathf.Max(1, ragePhase2Hp - 1);
            int ragePhase3Hp = Mathf.Clamp(Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3Hp, 1, ragePhase3Max);

            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1Hp = ragePhase1Hp;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2Hp = ragePhase2Hp;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3Hp = ragePhase3Hp;

            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase)
            {
                Modules.BossChallenge.NightmareKingGrimmHelper.ReapplyLiveSettings();
            }

            RefreshNightmareKingGrimmHelperUi();
        }

        private void SetNightmareKingGrimmRagePhase2Hp(int value)
        {
            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp)
            {
                RefreshNightmareKingGrimmHelperUi();
                return;
            }

            int ragePhase1Hp = Mathf.Clamp(Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1Hp, 3, 1650);
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1Hp = ragePhase1Hp;
            int ragePhase2Max = Mathf.Max(2, ragePhase1Hp - 1);
            int ragePhase2Hp = Mathf.Clamp(value, 2, ragePhase2Max);
            int ragePhase3Max = Mathf.Max(1, ragePhase2Hp - 1);
            int ragePhase3Hp = Mathf.Clamp(Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3Hp, 1, ragePhase3Max);

            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2Hp = ragePhase2Hp;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3Hp = ragePhase3Hp;

            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase)
            {
                Modules.BossChallenge.NightmareKingGrimmHelper.ReapplyLiveSettings();
            }

            RefreshNightmareKingGrimmHelperUi();
        }

        private void SetNightmareKingGrimmRagePhase3Hp(int value)
        {
            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp)
            {
                RefreshNightmareKingGrimmHelperUi();
                return;
            }

            int ragePhase1Hp = Mathf.Clamp(Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1Hp, 3, 1650);
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1Hp = ragePhase1Hp;
            int ragePhase2Max = Mathf.Max(2, ragePhase1Hp - 1);
            int ragePhase2Hp = Mathf.Clamp(Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2Hp, 2, ragePhase2Max);
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2Hp = ragePhase2Hp;
            int ragePhase3Hp = Mathf.Clamp(value, 1, Mathf.Max(1, ragePhase2Hp - 1));
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3Hp = ragePhase3Hp;

            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase)
            {
                Modules.BossChallenge.NightmareKingGrimmHelper.ReapplyLiveSettings();
            }

            RefreshNightmareKingGrimmHelperUi();
        }

        private void RefreshNightmareKingGrimmHelperUi()
        {
            Module? module = GetNightmareKingGrimmHelperModule();
            UpdateToggleValue(nightmareKingGrimmHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(nightmareKingGrimmHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(nightmareKingGrimmP5HpValue, Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp);
            UpdateToggleValue(nightmareKingGrimmUseMaxHpValue, Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseMaxHp);
            UpdateToggleValue(nightmareKingGrimmUseCustomPhaseValue, Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase);
            UpdateIntInputValue(nightmareKingGrimmMaxHpField, Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmMaxHp);
            UpdateIntInputValue(nightmareKingGrimmRagePhase1HpField, Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1Hp);
            UpdateIntInputValue(nightmareKingGrimmRagePhase2HpField, Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2Hp);
            UpdateIntInputValue(nightmareKingGrimmRagePhase3HpField, Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3Hp);

            UpdateNightmareKingGrimmHelperInteractivity();
        }

        private void UpdateNightmareKingGrimmHelperInteractivity()
        {
            SetContentInteractivity(nightmareKingGrimmHelperContent, GetNightmareKingGrimmHelperEnabled(), "NightmareKingGrimmHelperEnableRow");
            if (!GetNightmareKingGrimmHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseMaxHp;
            bool p5Hp = Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp;
            bool useCustomPhase = Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase;
            SetRowInteractivity(nightmareKingGrimmHelperContent, "NightmareKingGrimmUseMaxHpRow", !p5Hp);
            SetRowInteractivity(nightmareKingGrimmHelperContent, "NightmareKingGrimmMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(nightmareKingGrimmHelperContent, "NightmareKingGrimmUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(nightmareKingGrimmHelperContent, "NightmareKingGrimmRagePhase1HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(nightmareKingGrimmHelperContent, "NightmareKingGrimmRagePhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(nightmareKingGrimmHelperContent, "NightmareKingGrimmRagePhase3HpRow", useCustomPhase && !p5Hp);
        }

        private void SetNightmareKingGrimmHelperVisible(bool value)
        {
            nightmareKingGrimmHelperVisible = value;
            if (nightmareKingGrimmHelperRoot != null)
            {
                nightmareKingGrimmHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshNightmareKingGrimmHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetPureVesselHelperModule()
        {
            if (pureVesselHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.PureVesselHelper), out pureVesselHelperModule);
            }

            return pureVesselHelperModule;
        }

        private bool GetPureVesselHelperEnabled()
        {
            Module? module = GetPureVesselHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetPureVesselHelperEnabled(bool value)
        {
            Module? module = GetPureVesselHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdatePureVesselHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetPureVesselUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp)
            {
                Modules.BossChallenge.PureVesselHelper.pureVesselUseMaxHp = true;
                RefreshPureVesselHelperUi();
                return;
            }

            Modules.BossChallenge.PureVesselHelper.pureVesselUseMaxHp = value;
            Modules.BossChallenge.PureVesselHelper.ReapplyLiveSettings();
            RefreshPureVesselHelperUi();
        }

        private void SetPureVesselUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp)
            {
                Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase = false;
                RefreshPureVesselHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.PureVesselHelper.pureVesselPhase2Hp, 2, 1850);
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase2Hp = phase2Hp;
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase3Hp = Mathf.Clamp(Modules.BossChallenge.PureVesselHelper.pureVesselPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
            Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase = value;
            Modules.BossChallenge.PureVesselHelper.ReapplyLiveSettings();
            RefreshPureVesselHelperUi();
        }

        private void SetPureVesselP5HpEnabled(bool value)
        {
            Modules.BossChallenge.PureVesselHelper.SetP5HpEnabled(value);
            RefreshPureVesselHelperUi();
        }

        private void SetPureVesselMaxHp(int value)
        {
            if (Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp)
            {
                Modules.BossChallenge.PureVesselHelper.pureVesselMaxHp = 1600;
                RefreshPureVesselHelperUi();
                return;
            }

            Modules.BossChallenge.PureVesselHelper.pureVesselMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.PureVesselHelper.pureVesselUseMaxHp)
            {
                Modules.BossChallenge.PureVesselHelper.ApplyPureVesselHealthIfPresent();
            }
        }

        private void SetPureVesselPhase2Hp(int value)
        {
            if (Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp)
            {
                RefreshPureVesselHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(value, 2, 1850);
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase2Hp = phase2Hp;
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase3Hp = Mathf.Clamp(Modules.BossChallenge.PureVesselHelper.pureVesselPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase)
            {
                Modules.BossChallenge.PureVesselHelper.ReapplyLiveSettings();
            }

            RefreshPureVesselHelperUi();
        }

        private void SetPureVesselPhase3Hp(int value)
        {
            if (Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp)
            {
                RefreshPureVesselHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.PureVesselHelper.pureVesselPhase2Hp, 2, 1850);
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase2Hp = phase2Hp;
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase3Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase)
            {
                Modules.BossChallenge.PureVesselHelper.ReapplyLiveSettings();
            }

            RefreshPureVesselHelperUi();
        }

        private void RefreshPureVesselHelperUi()
        {
            Module? module = GetPureVesselHelperModule();
            UpdateToggleValue(pureVesselHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(pureVesselHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(pureVesselP5HpValue, Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp);
            UpdateToggleValue(pureVesselUseMaxHpValue, Modules.BossChallenge.PureVesselHelper.pureVesselUseMaxHp);
            UpdateToggleValue(pureVesselUseCustomPhaseValue, Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase);
            UpdateIntInputValue(pureVesselMaxHpField, Modules.BossChallenge.PureVesselHelper.pureVesselMaxHp);
            UpdateIntInputValue(pureVesselPhase2HpField, Modules.BossChallenge.PureVesselHelper.pureVesselPhase2Hp);
            UpdateIntInputValue(pureVesselPhase3HpField, Modules.BossChallenge.PureVesselHelper.pureVesselPhase3Hp);

            UpdatePureVesselHelperInteractivity();
        }

        private void UpdatePureVesselHelperInteractivity()
        {
            SetContentInteractivity(pureVesselHelperContent, GetPureVesselHelperEnabled(), "PureVesselHelperEnableRow");
            if (!GetPureVesselHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.PureVesselHelper.pureVesselUseMaxHp;
            bool p5Hp = Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp;
            bool useCustomPhase = Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase;
            SetRowInteractivity(pureVesselHelperContent, "PureVesselUseMaxHpRow", !p5Hp);
            SetRowInteractivity(pureVesselHelperContent, "PureVesselMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(pureVesselHelperContent, "PureVesselUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(pureVesselHelperContent, "PureVesselPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(pureVesselHelperContent, "PureVesselPhase3HpRow", useCustomPhase && !p5Hp);
        }

        private void SetPureVesselHelperVisible(bool value)
        {
            pureVesselHelperVisible = value;
            if (pureVesselHelperRoot != null)
            {
                pureVesselHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshPureVesselHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetAbsoluteRadianceHelperModule()
        {
            if (absoluteRadianceHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.AbsoluteRadianceHelper), out absoluteRadianceHelperModule);
            }

            return absoluteRadianceHelperModule;
        }

        private bool GetAbsoluteRadianceHelperEnabled()
        {
            Module? module = GetAbsoluteRadianceHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetAbsoluteRadianceHelperEnabled(bool value)
        {
            Module? module = GetAbsoluteRadianceHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateAbsoluteRadianceHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetAbsoluteRadianceUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseMaxHp = true;
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseMaxHp = value;
            Modules.BossChallenge.AbsoluteRadianceHelper.ReapplyLiveSettings();
            RefreshAbsoluteRadianceHelperUi();
        }

        private void SetAbsoluteRadianceUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase = false;
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            int maxHp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp, 1, 999999);
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp, 1, maxHp);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp, 1, Mathf.Max(1, phase3Hp - 1));
            int phase5Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp, 1, Mathf.Max(1, phase4Hp - 1));
            int finalPhaseHp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHp, 1, 999999);

            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp = phase2Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp = phase3Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp = phase4Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp = phase5Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHp = finalPhaseHp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase = value;
            Modules.BossChallenge.AbsoluteRadianceHelper.ReapplyLiveSettings();
            RefreshAbsoluteRadianceHelperUi();
        }

        private void SetAbsoluteRadianceP5HpEnabled(bool value)
        {
            Modules.BossChallenge.AbsoluteRadianceHelper.SetP5HpEnabled(value);
            RefreshAbsoluteRadianceHelperUi();
        }

        private void SetAbsoluteRadianceMaxHp(int value)
        {
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp = 3000;
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase)
            {
                int maxHp = Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp;
                int phase2Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp, 1, maxHp);
                int phase3Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
                int phase4Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp, 1, Mathf.Max(1, phase3Hp - 1));
                int phase5Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp, 1, Mathf.Max(1, phase4Hp - 1));
                int finalPhaseHp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHp, 1, 999999);
                Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp = phase2Hp;
                Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp = phase3Hp;
                Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp = phase4Hp;
                Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp = phase5Hp;
                Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHp = finalPhaseHp;
                Modules.BossChallenge.AbsoluteRadianceHelper.ReapplyLiveSettings();
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseMaxHp)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.ApplyAbsoluteRadianceHealthIfPresent();
            }
        }

        private void SetAbsoluteRadiancePhase2Hp(int value)
        {
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp)
            {
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            int maxHp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp, 1, 999999);
            int phase2Hp = Mathf.Clamp(value, 1, maxHp);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp, 1, Mathf.Max(1, phase3Hp - 1));
            int phase5Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp, 1, Mathf.Max(1, phase4Hp - 1));

            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp = phase2Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp = phase3Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp = phase4Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp = phase5Hp;

            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.ReapplyLiveSettings();
            }

            RefreshAbsoluteRadianceHelperUi();
        }

        private void SetAbsoluteRadianceFinalPhaseHp(int value)
        {
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp)
            {
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHp = Mathf.Clamp(value, 1, 999999);

            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.ReapplyLiveSettings();
            }

            RefreshAbsoluteRadianceHelperUi();
        }

        private void SetAbsoluteRadiancePhase3Hp(int value)
        {
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp)
            {
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            int maxHp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp, 1, 999999);
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp, 1, maxHp);
            int phase3Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase2Hp - 1));
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp, 1, Mathf.Max(1, phase3Hp - 1));
            int phase5Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp, 1, Mathf.Max(1, phase4Hp - 1));

            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp = phase2Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp = phase3Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp = phase4Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp = phase5Hp;

            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.ReapplyLiveSettings();
            }

            RefreshAbsoluteRadianceHelperUi();
        }

        private void SetAbsoluteRadiancePhase4Hp(int value)
        {
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp)
            {
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            int maxHp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp, 1, 999999);
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp, 1, maxHp);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
            int phase4Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase3Hp - 1));
            int phase5Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp, 1, Mathf.Max(1, phase4Hp - 1));

            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp = phase2Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp = phase3Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp = phase4Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp = phase5Hp;

            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.ReapplyLiveSettings();
            }

            RefreshAbsoluteRadianceHelperUi();
        }

        private void SetAbsoluteRadiancePhase5Hp(int value)
        {
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp)
            {
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            int maxHp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp, 1, 999999);
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp, 1, maxHp);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp, 1, Mathf.Max(1, phase3Hp - 1));
            int phase5Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase4Hp - 1));

            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp = phase2Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp = phase3Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp = phase4Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp = phase5Hp;

            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.ReapplyLiveSettings();
            }

            RefreshAbsoluteRadianceHelperUi();
        }

        private void RefreshAbsoluteRadianceHelperUi()
        {
            Module? module = GetAbsoluteRadianceHelperModule();
            UpdateToggleValue(absoluteRadianceHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(absoluteRadianceHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(absoluteRadianceP5HpValue, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp);
            UpdateToggleValue(absoluteRadianceUseMaxHpValue, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseMaxHp);
            UpdateToggleValue(absoluteRadianceUseCustomPhaseValue, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase);
            UpdateIntInputValue(absoluteRadianceMaxHpField, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp);
            UpdateIntInputValue(absoluteRadiancePhase2HpField, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp);
            UpdateIntInputValue(absoluteRadiancePhase3HpField, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp);
            UpdateIntInputValue(absoluteRadiancePhase4HpField, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp);
            UpdateIntInputValue(absoluteRadiancePhase5HpField, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp);
            UpdateIntInputValue(absoluteRadianceFinalPhaseHpField, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHp);

            UpdateAbsoluteRadianceHelperInteractivity();
        }

        private void UpdateAbsoluteRadianceHelperInteractivity()
        {
            SetContentInteractivity(absoluteRadianceHelperContent, GetAbsoluteRadianceHelperEnabled(), "AbsoluteRadianceHelperEnableRow");
            if (!GetAbsoluteRadianceHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseMaxHp;
            bool p5Hp = Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp;
            bool useCustomPhase = Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase;
            SetRowInteractivity(absoluteRadianceHelperContent, "AbsoluteRadianceUseMaxHpRow", !p5Hp);
            SetRowInteractivity(absoluteRadianceHelperContent, "AbsoluteRadianceMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(absoluteRadianceHelperContent, "AbsoluteRadianceUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(absoluteRadianceHelperContent, "AbsoluteRadiancePhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(absoluteRadianceHelperContent, "AbsoluteRadiancePhase3HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(absoluteRadianceHelperContent, "AbsoluteRadiancePhase4HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(absoluteRadianceHelperContent, "AbsoluteRadiancePhase5HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(absoluteRadianceHelperContent, "AbsoluteRadianceFinalPhaseHpRow", useCustomPhase && !p5Hp);
        }

        private void SetAbsoluteRadianceHelperVisible(bool value)
        {
            absoluteRadianceHelperVisible = value;
            if (absoluteRadianceHelperRoot != null)
            {
                absoluteRadianceHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshAbsoluteRadianceHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetPaintmasterSheoHelperModule()
        {
            if (paintmasterSheoHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.PaintmasterSheoHelper), out paintmasterSheoHelperModule);
            }

            return paintmasterSheoHelperModule;
        }

        private bool GetPaintmasterSheoHelperEnabled()
        {
            Module? module = GetPaintmasterSheoHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetPaintmasterSheoHelperEnabled(bool value)
        {
            Module? module = GetPaintmasterSheoHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdatePaintmasterSheoHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetPaintmasterSheoUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoP5Hp)
            {
                Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoUseMaxHp = true;
                RefreshPaintmasterSheoHelperUi();
                return;
            }

            Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoUseMaxHp = value;
            Modules.BossChallenge.PaintmasterSheoHelper.ReapplyLiveSettings();
            RefreshPaintmasterSheoHelperUi();
        }

        private void SetPaintmasterSheoP5HpEnabled(bool value)
        {
            Modules.BossChallenge.PaintmasterSheoHelper.SetP5HpEnabled(value);
            RefreshPaintmasterSheoHelperUi();
        }

        private void SetPaintmasterSheoMaxHp(int value)
        {
            if (Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoP5Hp)
            {
                Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoMaxHp = 950;
                RefreshPaintmasterSheoHelperUi();
                return;
            }

            Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoUseMaxHp)
            {
                Modules.BossChallenge.PaintmasterSheoHelper.ApplyPaintmasterSheoHealthIfPresent();
            }
        }

        private void RefreshPaintmasterSheoHelperUi()
        {
            Module? module = GetPaintmasterSheoHelperModule();
            UpdateToggleValue(paintmasterSheoHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(paintmasterSheoHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(paintmasterSheoP5HpValue, Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoP5Hp);
            UpdateToggleValue(paintmasterSheoUseMaxHpValue, Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoUseMaxHp);
            UpdateIntInputValue(paintmasterSheoMaxHpField, Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoMaxHp);

            UpdatePaintmasterSheoHelperInteractivity();
        }

        private void UpdatePaintmasterSheoHelperInteractivity()
        {
            SetContentInteractivity(paintmasterSheoHelperContent, GetPaintmasterSheoHelperEnabled(), "PaintmasterSheoHelperEnableRow");
            if (!GetPaintmasterSheoHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoUseMaxHp;
            bool p5Hp = Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoP5Hp;
            SetRowInteractivity(paintmasterSheoHelperContent, "PaintmasterSheoUseMaxHpRow", !p5Hp);
            SetRowInteractivity(paintmasterSheoHelperContent, "PaintmasterSheoMaxHpRow", useMaxHp && !p5Hp);
        }

        private void SetPaintmasterSheoHelperVisible(bool value)
        {
            paintmasterSheoHelperVisible = value;
            if (paintmasterSheoHelperRoot != null)
            {
                paintmasterSheoHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshPaintmasterSheoHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetSoulWarriorHelperModule()
        {
            if (soulWarriorHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.SoulWarriorHelper), out soulWarriorHelperModule);
            }

            return soulWarriorHelperModule;
        }

        private bool GetSoulWarriorHelperEnabled()
        {
            Module? module = GetSoulWarriorHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetSoulWarriorHelperEnabled(bool value)
        {
            Module? module = GetSoulWarriorHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateSoulWarriorHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetSoulWarriorUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp)
            {
                Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseMaxHp = true;
                RefreshSoulWarriorHelperUi();
                return;
            }

            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseMaxHp = value;
            Modules.BossChallenge.SoulWarriorHelper.ReapplyLiveSettings();
            RefreshSoulWarriorHelperUi();
        }

        private void SetSoulWarriorP5HpEnabled(bool value)
        {
            Modules.BossChallenge.SoulWarriorHelper.SetP5HpEnabled(value);
            RefreshSoulWarriorHelperUi();
        }

        private void SetSoulWarriorMaxHp(int value)
        {
            if (Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp)
            {
                Modules.BossChallenge.SoulWarriorHelper.soulWarriorMaxHp = 750;
                RefreshSoulWarriorHelperUi();
                return;
            }

            Modules.BossChallenge.SoulWarriorHelper.soulWarriorMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseMaxHp)
            {
                Modules.BossChallenge.SoulWarriorHelper.ApplySoulWarriorHealthIfPresent();
            }
        }

        private void SetSoulWarriorUseCustomSummonHpEnabled(bool value)
        {
            if (Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp)
            {
                Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonHp = false;
                RefreshSoulWarriorHelperUi();
                return;
            }

            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonHp = value;
            Modules.BossChallenge.SoulWarriorHelper.ReapplyLiveSettings();
            RefreshSoulWarriorHelperUi();
        }

        private void SetSoulWarriorSummonHp(int value)
        {
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonHp
                && !Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp)
            {
                Modules.BossChallenge.SoulWarriorHelper.ApplySoulWarriorSummonHealthIfPresent();
            }
        }

        private void SetSoulWarriorUseCustomSummonLimitEnabled(bool value)
        {
            if (Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp)
            {
                Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonLimit = false;
                RefreshSoulWarriorHelperUi();
                return;
            }

            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonLimit = value;
            Modules.BossChallenge.SoulWarriorHelper.ReapplyLiveSettings();
            RefreshSoulWarriorHelperUi();
        }

        private void SetSoulWarriorSummonLimit(int value)
        {
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonLimit = Mathf.Clamp(value, 36, 999);
            if (Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonLimit
                && !Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp)
            {
                Modules.BossChallenge.SoulWarriorHelper.ApplySummonLimitSettingsIfPresent();
            }
        }

        private void RefreshSoulWarriorHelperUi()
        {
            Module? module = GetSoulWarriorHelperModule();
            UpdateToggleValue(soulWarriorHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(soulWarriorHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(soulWarriorP5HpValue, Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp);
            UpdateToggleValue(soulWarriorUseMaxHpValue, Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseMaxHp);
            UpdateToggleValue(soulWarriorUseCustomSummonHpValue, Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonHp);
            UpdateToggleValue(soulWarriorUseCustomSummonLimitValue, Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonLimit);
            UpdateIntInputValue(soulWarriorMaxHpField, Modules.BossChallenge.SoulWarriorHelper.soulWarriorMaxHp);
            UpdateIntInputValue(soulWarriorSummonHpField, Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonHp);
            UpdateIntInputValue(soulWarriorSummonLimitField, Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonLimit);

            UpdateSoulWarriorHelperInteractivity();
        }

        private void UpdateSoulWarriorHelperInteractivity()
        {
            SetContentInteractivity(soulWarriorHelperContent, GetSoulWarriorHelperEnabled(), "SoulWarriorHelperEnableRow");
            if (!GetSoulWarriorHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseMaxHp;
            bool p5Hp = Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp;
            bool useCustomSummonHp = Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonHp;
            bool useCustomSummonLimit = Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonLimit;
            SetRowInteractivity(soulWarriorHelperContent, "SoulWarriorUseMaxHpRow", !p5Hp);
            SetRowInteractivity(soulWarriorHelperContent, "SoulWarriorMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(soulWarriorHelperContent, "SoulWarriorUseCustomSummonHpRow", !p5Hp);
            SetRowInteractivity(soulWarriorHelperContent, "SoulWarriorSummonHpRow", useCustomSummonHp && !p5Hp);
            SetRowInteractivity(soulWarriorHelperContent, "SoulWarriorUseCustomSummonLimitRow", !p5Hp);
            SetRowInteractivity(soulWarriorHelperContent, "SoulWarriorSummonLimitRow", useCustomSummonLimit && !p5Hp);
        }

        private void SetSoulWarriorHelperVisible(bool value)
        {
            soulWarriorHelperVisible = value;
            if (soulWarriorHelperRoot != null)
            {
                soulWarriorHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshSoulWarriorHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetNailsageSlyHelperModule()
        {
            if (nailsageSlyHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.NailsageSlyHelper), out nailsageSlyHelperModule);
            }

            return nailsageSlyHelperModule;
        }

        private bool GetNailsageSlyHelperEnabled()
        {
            Module? module = GetNailsageSlyHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetNailsageSlyHelperEnabled(bool value)
        {
            Module? module = GetNailsageSlyHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateNailsageSlyHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetNailsageSlyP5HpEnabled(bool value)
        {
            Modules.BossChallenge.NailsageSlyHelper.SetP5HpEnabled(value);
            RefreshNailsageSlyHelperUi();
        }

        private void SetNailsageSlyPhase1Hp(int value)
        {
            if (Modules.BossChallenge.NailsageSlyHelper.nailsageSlyP5Hp)
            {
                Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase1Hp = 800;
                RefreshNailsageSlyHelperUi();
                return;
            }

            Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.NailsageSlyHelper.ReapplyLiveSettings();
        }

        private void SetNailsageSlyPhase2Hp(int value)
        {
            if (Modules.BossChallenge.NailsageSlyHelper.nailsageSlyP5Hp)
            {
                Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase2Hp = 250;
                RefreshNailsageSlyHelperUi();
                return;
            }

            Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.NailsageSlyHelper.ReapplyLiveSettings();
        }

        private void RefreshNailsageSlyHelperUi()
        {
            Module? module = GetNailsageSlyHelperModule();
            UpdateToggleValue(nailsageSlyHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(nailsageSlyHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(nailsageSlyP5HpValue, Modules.BossChallenge.NailsageSlyHelper.nailsageSlyP5Hp);
            UpdateIntInputValue(nailsageSlyPhase1HpField, Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase1Hp);
            UpdateIntInputValue(nailsageSlyPhase2HpField, Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase2Hp);

            UpdateNailsageSlyHelperInteractivity();
        }

        private void UpdateNailsageSlyHelperInteractivity()
        {
            SetContentInteractivity(nailsageSlyHelperContent, GetNailsageSlyHelperEnabled(), "NailsageSlyHelperEnableRow");
            if (!GetNailsageSlyHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.NailsageSlyHelper.nailsageSlyP5Hp;
            SetRowInteractivity(nailsageSlyHelperContent, "NailsageSlyPhase1HpRow", !p5Hp);
            SetRowInteractivity(nailsageSlyHelperContent, "NailsageSlyPhase2HpRow", !p5Hp);
        }

        private void SetNailsageSlyHelperVisible(bool value)
        {
            nailsageSlyHelperVisible = value;
            if (nailsageSlyHelperRoot != null)
            {
                nailsageSlyHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshNailsageSlyHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetSoulMasterHelperModule()
        {
            if (soulMasterHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.SoulMasterHelper), out soulMasterHelperModule);
            }

            return soulMasterHelperModule;
        }

        private bool GetSoulMasterHelperEnabled()
        {
            Module? module = GetSoulMasterHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetSoulMasterHelperEnabled(bool value)
        {
            Module? module = GetSoulMasterHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateSoulMasterHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetSoulMasterP5HpEnabled(bool value)
        {
            Modules.BossChallenge.SoulMasterHelper.SetP5HpEnabled(value);
            RefreshSoulMasterHelperUi();
        }

        private void SetSoulMasterPhase1Hp(int value)
        {
            if (Modules.BossChallenge.SoulMasterHelper.soulMasterP5Hp)
            {
                Modules.BossChallenge.SoulMasterHelper.soulMasterPhase1Hp = 600;
                RefreshSoulMasterHelperUi();
                return;
            }

            Modules.BossChallenge.SoulMasterHelper.soulMasterPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.SoulMasterHelper.ReapplyLiveSettings();
        }

        private void SetSoulMasterPhase2Hp(int value)
        {
            if (Modules.BossChallenge.SoulMasterHelper.soulMasterP5Hp)
            {
                Modules.BossChallenge.SoulMasterHelper.soulMasterPhase2Hp = 350;
                RefreshSoulMasterHelperUi();
                return;
            }

            Modules.BossChallenge.SoulMasterHelper.soulMasterPhase2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.SoulMasterHelper.ReapplyLiveSettings();
        }

        private void RefreshSoulMasterHelperUi()
        {
            Module? module = GetSoulMasterHelperModule();
            UpdateToggleValue(soulMasterHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(soulMasterHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(soulMasterP5HpValue, Modules.BossChallenge.SoulMasterHelper.soulMasterP5Hp);
            UpdateIntInputValue(soulMasterPhase1HpField, Modules.BossChallenge.SoulMasterHelper.soulMasterPhase1Hp);
            UpdateIntInputValue(soulMasterPhase2HpField, Modules.BossChallenge.SoulMasterHelper.soulMasterPhase2Hp);

            UpdateSoulMasterHelperInteractivity();
        }

        private void UpdateSoulMasterHelperInteractivity()
        {
            SetContentInteractivity(soulMasterHelperContent, GetSoulMasterHelperEnabled(), "SoulMasterHelperEnableRow");
            if (!GetSoulMasterHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.SoulMasterHelper.soulMasterP5Hp;
            SetRowInteractivity(soulMasterHelperContent, "SoulMasterPhase1HpRow", !p5Hp);
            SetRowInteractivity(soulMasterHelperContent, "SoulMasterPhase2HpRow", !p5Hp);
        }

        private void SetSoulMasterHelperVisible(bool value)
        {
            soulMasterHelperVisible = value;
            if (soulMasterHelperRoot != null)
            {
                soulMasterHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshSoulMasterHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetSoulTyrantHelperModule()
        {
            if (soulTyrantHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.SoulTyrantHelper), out soulTyrantHelperModule);
            }

            return soulTyrantHelperModule;
        }

        private bool GetSoulTyrantHelperEnabled()
        {
            Module? module = GetSoulTyrantHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetSoulTyrantHelperEnabled(bool value)
        {
            Module? module = GetSoulTyrantHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateSoulTyrantHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetSoulTyrantP5HpEnabled(bool value)
        {
            Modules.BossChallenge.SoulTyrantHelper.SetP5HpEnabled(value);
            RefreshSoulTyrantHelperUi();
        }

        private void SetSoulTyrantPhase1Hp(int value)
        {
            if (Modules.BossChallenge.SoulTyrantHelper.soulTyrantP5Hp)
            {
                Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase1Hp = 900;
                RefreshSoulTyrantHelperUi();
                return;
            }

            Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.SoulTyrantHelper.ReapplyLiveSettings();
        }

        private void SetSoulTyrantPhase2Hp(int value)
        {
            if (Modules.BossChallenge.SoulTyrantHelper.soulTyrantP5Hp)
            {
                Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase2Hp = 350;
                RefreshSoulTyrantHelperUi();
                return;
            }

            Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.SoulTyrantHelper.ReapplyLiveSettings();
        }

        private void RefreshSoulTyrantHelperUi()
        {
            Module? module = GetSoulTyrantHelperModule();
            UpdateToggleValue(soulTyrantHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(soulTyrantHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(soulTyrantP5HpValue, Modules.BossChallenge.SoulTyrantHelper.soulTyrantP5Hp);
            UpdateIntInputValue(soulTyrantPhase1HpField, Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase1Hp);
            UpdateIntInputValue(soulTyrantPhase2HpField, Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase2Hp);

            UpdateSoulTyrantHelperInteractivity();
        }

        private void UpdateSoulTyrantHelperInteractivity()
        {
            SetContentInteractivity(soulTyrantHelperContent, GetSoulTyrantHelperEnabled(), "SoulTyrantHelperEnableRow");
            if (!GetSoulTyrantHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.SoulTyrantHelper.soulTyrantP5Hp;
            SetRowInteractivity(soulTyrantHelperContent, "SoulTyrantPhase1HpRow", !p5Hp);
            SetRowInteractivity(soulTyrantHelperContent, "SoulTyrantPhase2HpRow", !p5Hp);
        }

        private void SetSoulTyrantHelperVisible(bool value)
        {
            soulTyrantHelperVisible = value;
            if (soulTyrantHelperRoot != null)
            {
                soulTyrantHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshSoulTyrantHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetWatcherKnightHelperModule()
        {
            if (watcherKnightHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.WatcherKnightHelper), out watcherKnightHelperModule);
            }

            return watcherKnightHelperModule;
        }

        private bool GetWatcherKnightHelperEnabled()
        {
            Module? module = GetWatcherKnightHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetWatcherKnightHelperEnabled(bool value)
        {
            Module? module = GetWatcherKnightHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateWatcherKnightHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetWatcherKnightP5HpEnabled(bool value)
        {
            Modules.BossChallenge.WatcherKnightHelper.SetP5HpEnabled(value);
            RefreshWatcherKnightHelperUi();
        }

        private void SetWatcherKnight1Hp(int value)
        {
            if (Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp)
            {
                Modules.BossChallenge.WatcherKnightHelper.watcherKnight1Hp = 350;
                RefreshWatcherKnightHelperUi();
                return;
            }

            Modules.BossChallenge.WatcherKnightHelper.watcherKnight1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.WatcherKnightHelper.ReapplyLiveSettings();
        }

        private void SetWatcherKnight2Hp(int value)
        {
            if (Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp)
            {
                Modules.BossChallenge.WatcherKnightHelper.watcherKnight2Hp = 350;
                RefreshWatcherKnightHelperUi();
                return;
            }

            Modules.BossChallenge.WatcherKnightHelper.watcherKnight2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.WatcherKnightHelper.ReapplyLiveSettings();
        }

        private void SetWatcherKnight3Hp(int value)
        {
            if (Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp)
            {
                Modules.BossChallenge.WatcherKnightHelper.watcherKnight3Hp = 350;
                RefreshWatcherKnightHelperUi();
                return;
            }

            Modules.BossChallenge.WatcherKnightHelper.watcherKnight3Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.WatcherKnightHelper.ReapplyLiveSettings();
        }

        private void SetWatcherKnight4Hp(int value)
        {
            if (Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp)
            {
                Modules.BossChallenge.WatcherKnightHelper.watcherKnight4Hp = 350;
                RefreshWatcherKnightHelperUi();
                return;
            }

            Modules.BossChallenge.WatcherKnightHelper.watcherKnight4Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.WatcherKnightHelper.ReapplyLiveSettings();
        }

        private void SetWatcherKnight5Hp(int value)
        {
            if (Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp)
            {
                Modules.BossChallenge.WatcherKnightHelper.watcherKnight5Hp = 350;
                RefreshWatcherKnightHelperUi();
                return;
            }

            Modules.BossChallenge.WatcherKnightHelper.watcherKnight5Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.WatcherKnightHelper.ReapplyLiveSettings();
        }

        private void SetWatcherKnight6Hp(int value)
        {
            if (Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp)
            {
                Modules.BossChallenge.WatcherKnightHelper.watcherKnight6Hp = 350;
                RefreshWatcherKnightHelperUi();
                return;
            }

            Modules.BossChallenge.WatcherKnightHelper.watcherKnight6Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.WatcherKnightHelper.ReapplyLiveSettings();
        }

        private void RefreshWatcherKnightHelperUi()
        {
            Module? module = GetWatcherKnightHelperModule();
            UpdateToggleValue(watcherKnightHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(watcherKnightHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(watcherKnightP5HpValue, Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp);
            UpdateIntInputValue(watcherKnight1HpField, Modules.BossChallenge.WatcherKnightHelper.watcherKnight1Hp);
            UpdateIntInputValue(watcherKnight2HpField, Modules.BossChallenge.WatcherKnightHelper.watcherKnight2Hp);
            UpdateIntInputValue(watcherKnight3HpField, Modules.BossChallenge.WatcherKnightHelper.watcherKnight3Hp);
            UpdateIntInputValue(watcherKnight4HpField, Modules.BossChallenge.WatcherKnightHelper.watcherKnight4Hp);
            UpdateIntInputValue(watcherKnight5HpField, Modules.BossChallenge.WatcherKnightHelper.watcherKnight5Hp);
            UpdateIntInputValue(watcherKnight6HpField, Modules.BossChallenge.WatcherKnightHelper.watcherKnight6Hp);

            UpdateWatcherKnightHelperInteractivity();
        }

        private void UpdateWatcherKnightHelperInteractivity()
        {
            SetContentInteractivity(watcherKnightHelperContent, GetWatcherKnightHelperEnabled(), "WatcherKnightHelperEnableRow");
            if (!GetWatcherKnightHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp;
            SetRowInteractivity(watcherKnightHelperContent, "WatcherKnight1HpRow", !p5Hp);
            SetRowInteractivity(watcherKnightHelperContent, "WatcherKnight2HpRow", !p5Hp);
            SetRowInteractivity(watcherKnightHelperContent, "WatcherKnight3HpRow", !p5Hp);
            SetRowInteractivity(watcherKnightHelperContent, "WatcherKnight4HpRow", !p5Hp);
            SetRowInteractivity(watcherKnightHelperContent, "WatcherKnight5HpRow", !p5Hp);
            SetRowInteractivity(watcherKnightHelperContent, "WatcherKnight6HpRow", !p5Hp);
        }

        private void SetWatcherKnightHelperVisible(bool value)
        {
            watcherKnightHelperVisible = value;
            if (watcherKnightHelperRoot != null)
            {
                watcherKnightHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshWatcherKnightHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetOroMatoHelperModule()
        {
            if (oroMatoHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.OroMatoHelper), out oroMatoHelperModule);
            }

            return oroMatoHelperModule;
        }

        private bool GetOroMatoHelperEnabled()
        {
            Module? module = GetOroMatoHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetOroMatoHelperEnabled(bool value)
        {
            Module? module = GetOroMatoHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateOroMatoHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetOroMatoP5HpEnabled(bool value)
        {
            Modules.BossChallenge.OroMatoHelper.SetP5HpEnabled(value);
            RefreshOroMatoHelperUi();
        }

        private void SetOroMatoOroPhase1Hp(int value)
        {
            if (Modules.BossChallenge.OroMatoHelper.oroMatoP5Hp)
            {
                Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase1Hp = 500;
                RefreshOroMatoHelperUi();
                return;
            }

            Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.OroMatoHelper.ReapplyLiveSettings();
        }

        private void SetOroMatoOroPhase2Hp(int value)
        {
            if (Modules.BossChallenge.OroMatoHelper.oroMatoP5Hp)
            {
                Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase2Hp = 600;
                RefreshOroMatoHelperUi();
                return;
            }

            Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.OroMatoHelper.ReapplyLiveSettings();
        }

        private void SetOroMatoMatoHp(int value)
        {
            if (Modules.BossChallenge.OroMatoHelper.oroMatoP5Hp)
            {
                Modules.BossChallenge.OroMatoHelper.oroMatoMatoHp = 1000;
                RefreshOroMatoHelperUi();
                return;
            }

            Modules.BossChallenge.OroMatoHelper.oroMatoMatoHp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.OroMatoHelper.ReapplyLiveSettings();
        }

        private void RefreshOroMatoHelperUi()
        {
            Module? module = GetOroMatoHelperModule();
            UpdateToggleValue(oroMatoHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(oroMatoHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(oroMatoP5HpValue, Modules.BossChallenge.OroMatoHelper.oroMatoP5Hp);
            UpdateIntInputValue(oroMatoOroPhase1HpField, Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase1Hp);
            UpdateIntInputValue(oroMatoOroPhase2HpField, Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase2Hp);
            UpdateIntInputValue(oroMatoMatoHpField, Modules.BossChallenge.OroMatoHelper.oroMatoMatoHp);

            UpdateOroMatoHelperInteractivity();
        }

        private void UpdateOroMatoHelperInteractivity()
        {
            SetContentInteractivity(oroMatoHelperContent, GetOroMatoHelperEnabled(), "OroMatoHelperEnableRow");
            if (!GetOroMatoHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.OroMatoHelper.oroMatoP5Hp;
            SetRowInteractivity(oroMatoHelperContent, "OroMatoOroPhase1HpRow", !p5Hp);
            SetRowInteractivity(oroMatoHelperContent, "OroMatoOroPhase2HpRow", !p5Hp);
            SetRowInteractivity(oroMatoHelperContent, "OroMatoMatoHpRow", !p5Hp);
        }

        private void SetOroMatoHelperVisible(bool value)
        {
            oroMatoHelperVisible = value;
            if (oroMatoHelperRoot != null)
            {
                oroMatoHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshOroMatoHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetGodTamerHelperModule()
        {
            if (godTamerHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.GodTamerHelper), out godTamerHelperModule);
            }

            return godTamerHelperModule;
        }

        private bool GetGodTamerHelperEnabled()
        {
            Module? module = GetGodTamerHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetGodTamerHelperEnabled(bool value)
        {
            Module? module = GetGodTamerHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateGodTamerHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetGodTamerP5HpEnabled(bool value)
        {
            Modules.BossChallenge.GodTamerHelper.SetP5HpEnabled(value);
            RefreshGodTamerHelperUi();
        }

        private void SetGodTamerLobsterHp(int value)
        {
            if (Modules.BossChallenge.GodTamerHelper.godTamerP5Hp)
            {
                Modules.BossChallenge.GodTamerHelper.godTamerLobsterHp = 750;
                RefreshGodTamerHelperUi();
                return;
            }

            Modules.BossChallenge.GodTamerHelper.godTamerLobsterHp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.GodTamerHelper.ReapplyLiveSettings();
        }

        private void SetGodTamerLancerHp(int value)
        {
            if (Modules.BossChallenge.GodTamerHelper.godTamerP5Hp)
            {
                Modules.BossChallenge.GodTamerHelper.godTamerLancerHp = 750;
                RefreshGodTamerHelperUi();
                return;
            }

            Modules.BossChallenge.GodTamerHelper.godTamerLancerHp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.GodTamerHelper.ReapplyLiveSettings();
        }

        private void RefreshGodTamerHelperUi()
        {
            Module? module = GetGodTamerHelperModule();
            UpdateToggleValue(godTamerHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(godTamerHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(godTamerP5HpValue, Modules.BossChallenge.GodTamerHelper.godTamerP5Hp);
            UpdateIntInputValue(godTamerLobsterHpField, Modules.BossChallenge.GodTamerHelper.godTamerLobsterHp);
            UpdateIntInputValue(godTamerLancerHpField, Modules.BossChallenge.GodTamerHelper.godTamerLancerHp);

            UpdateGodTamerHelperInteractivity();
        }

        private void UpdateGodTamerHelperInteractivity()
        {
            SetContentInteractivity(godTamerHelperContent, GetGodTamerHelperEnabled(), "GodTamerHelperEnableRow");
            if (!GetGodTamerHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.GodTamerHelper.godTamerP5Hp;
            SetRowInteractivity(godTamerHelperContent, "GodTamerLobsterHpRow", !p5Hp);
            SetRowInteractivity(godTamerHelperContent, "GodTamerLancerHpRow", !p5Hp);
        }

        private void SetGodTamerHelperVisible(bool value)
        {
            godTamerHelperVisible = value;
            if (godTamerHelperRoot != null)
            {
                godTamerHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshGodTamerHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetOblobblesHelperModule()
        {
            if (oblobblesHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.OblobblesHelper), out oblobblesHelperModule);
            }

            return oblobblesHelperModule;
        }

        private bool GetOblobblesHelperEnabled()
        {
            Module? module = GetOblobblesHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetOblobblesHelperEnabled(bool value)
        {
            Module? module = GetOblobblesHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateOblobblesHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetOblobblesP5HpEnabled(bool value)
        {
            Modules.BossChallenge.OblobblesHelper.SetP5HpEnabled(value);
            RefreshOblobblesHelperUi();
        }

        private void SetOblobblesLeftPhase1Hp(int value)
        {
            if (Modules.BossChallenge.OblobblesHelper.oblobblesP5Hp)
            {
                Modules.BossChallenge.OblobblesHelper.oblobblesLeftPhase1Hp = 450;
                RefreshOblobblesHelperUi();
                return;
            }

            Modules.BossChallenge.OblobblesHelper.oblobblesLeftPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.OblobblesHelper.ReapplyLiveSettings();
        }

        private void SetOblobblesRightPhase1Hp(int value)
        {
            if (Modules.BossChallenge.OblobblesHelper.oblobblesP5Hp)
            {
                Modules.BossChallenge.OblobblesHelper.oblobblesRightPhase1Hp = 450;
                RefreshOblobblesHelperUi();
                return;
            }

            Modules.BossChallenge.OblobblesHelper.oblobblesRightPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.OblobblesHelper.ReapplyLiveSettings();
        }

        private void SetOblobblesUsePhase2HpEnabled(bool value)
        {
            if (Modules.BossChallenge.OblobblesHelper.oblobblesP5Hp)
            {
                Modules.BossChallenge.OblobblesHelper.oblobblesUsePhase2Hp = false;
                RefreshOblobblesHelperUi();
                return;
            }

            Modules.BossChallenge.OblobblesHelper.oblobblesUsePhase2Hp = value;
            Modules.BossChallenge.OblobblesHelper.ReapplyLiveSettings();
            RefreshOblobblesHelperUi();
        }

        private void SetOblobblesPhase2Hp(int value)
        {
            if (Modules.BossChallenge.OblobblesHelper.oblobblesP5Hp)
            {
                RefreshOblobblesHelperUi();
                return;
            }

            Modules.BossChallenge.OblobblesHelper.oblobblesPhase2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.OblobblesHelper.ReapplyLiveSettings();
        }

        private void RefreshOblobblesHelperUi()
        {
            Module? module = GetOblobblesHelperModule();
            UpdateToggleValue(oblobblesHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(oblobblesHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(oblobblesP5HpValue, Modules.BossChallenge.OblobblesHelper.oblobblesP5Hp);
            UpdateToggleValue(oblobblesUsePhase2HpValue, Modules.BossChallenge.OblobblesHelper.oblobblesUsePhase2Hp);
            UpdateIntInputValue(oblobblesLeftPhase1HpField, Modules.BossChallenge.OblobblesHelper.oblobblesLeftPhase1Hp);
            UpdateIntInputValue(oblobblesRightPhase1HpField, Modules.BossChallenge.OblobblesHelper.oblobblesRightPhase1Hp);
            UpdateIntInputValue(oblobblesPhase2HpField, Modules.BossChallenge.OblobblesHelper.oblobblesPhase2Hp);

            UpdateOblobblesHelperInteractivity();
        }

        private void UpdateOblobblesHelperInteractivity()
        {
            SetContentInteractivity(oblobblesHelperContent, GetOblobblesHelperEnabled(), "OblobblesHelperEnableRow");
            if (!GetOblobblesHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.OblobblesHelper.oblobblesP5Hp;
            bool usePhase2Hp = Modules.BossChallenge.OblobblesHelper.oblobblesUsePhase2Hp;
            SetRowInteractivity(oblobblesHelperContent, "OblobblesLeftPhase1HpRow", !p5Hp);
            SetRowInteractivity(oblobblesHelperContent, "OblobblesRightPhase1HpRow", !p5Hp);
            SetRowInteractivity(oblobblesHelperContent, "OblobblesUsePhase2HpRow", !p5Hp);
            SetRowInteractivity(oblobblesHelperContent, "OblobblesPhase2HpRow", usePhase2Hp && !p5Hp);
        }

        private void SetOblobblesHelperVisible(bool value)
        {
            oblobblesHelperVisible = value;
            if (oblobblesHelperRoot != null)
            {
                oblobblesHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshOblobblesHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetFalseKnightHelperModule()
        {
            if (falseKnightHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.FalseKnightHelper), out falseKnightHelperModule);
            }

            return falseKnightHelperModule;
        }

        private bool GetFalseKnightHelperEnabled()
        {
            Module? module = GetFalseKnightHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetFalseKnightHelperEnabled(bool value)
        {
            Module? module = GetFalseKnightHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateFalseKnightHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetFalseKnightP5HpEnabled(bool value)
        {
            Modules.BossChallenge.FalseKnightHelper.SetP5HpEnabled(value);
            RefreshFalseKnightHelperUi();
        }

        private void SetFalseKnightArmorPhase1Hp(int value)
        {
            if (Modules.BossChallenge.FalseKnightHelper.falseKnightP5Hp)
            {
                Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase1Hp = 260;
                RefreshFalseKnightHelperUi();
                return;
            }

            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.FalseKnightHelper.ReapplyLiveSettings();
        }

        private void SetFalseKnightArmorPhase2Hp(int value)
        {
            if (Modules.BossChallenge.FalseKnightHelper.falseKnightP5Hp)
            {
                Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase2Hp = 260;
                RefreshFalseKnightHelperUi();
                return;
            }

            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.FalseKnightHelper.ReapplyLiveSettings();
        }

        private void SetFalseKnightArmorPhase3Hp(int value)
        {
            if (Modules.BossChallenge.FalseKnightHelper.falseKnightP5Hp)
            {
                Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase3Hp = 260;
                RefreshFalseKnightHelperUi();
                return;
            }

            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase3Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.FalseKnightHelper.ReapplyLiveSettings();
        }

        private void RefreshFalseKnightHelperUi()
        {
            Module? module = GetFalseKnightHelperModule();
            UpdateToggleValue(falseKnightHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(falseKnightHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(falseKnightP5HpValue, Modules.BossChallenge.FalseKnightHelper.falseKnightP5Hp);
            UpdateIntInputValue(falseKnightArmorPhase1HpField, Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase1Hp);
            UpdateIntInputValue(falseKnightArmorPhase2HpField, Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase2Hp);
            UpdateIntInputValue(falseKnightArmorPhase3HpField, Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase3Hp);

            UpdateFalseKnightHelperInteractivity();
        }

        private void UpdateFalseKnightHelperInteractivity()
        {
            SetContentInteractivity(falseKnightHelperContent, GetFalseKnightHelperEnabled(), "FalseKnightHelperEnableRow");
            if (!GetFalseKnightHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.FalseKnightHelper.falseKnightP5Hp;
            SetRowInteractivity(falseKnightHelperContent, "FalseKnightArmorPhase1HpRow", !p5Hp);
            SetRowInteractivity(falseKnightHelperContent, "FalseKnightArmorPhase2HpRow", !p5Hp);
            SetRowInteractivity(falseKnightHelperContent, "FalseKnightArmorPhase3HpRow", !p5Hp);
        }

        private void SetFalseKnightHelperVisible(bool value)
        {
            falseKnightHelperVisible = value;
            if (falseKnightHelperRoot != null)
            {
                falseKnightHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshFalseKnightHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetFailedChampionHelperModule()
        {
            if (failedChampionHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.FailedChampionHelper), out failedChampionHelperModule);
            }

            return failedChampionHelperModule;
        }

        private bool GetFailedChampionHelperEnabled()
        {
            Module? module = GetFailedChampionHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetFailedChampionHelperEnabled(bool value)
        {
            Module? module = GetFailedChampionHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateFailedChampionHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetFailedChampionP5HpEnabled(bool value)
        {
            Modules.BossChallenge.FailedChampionHelper.SetP5HpEnabled(value);
            RefreshFailedChampionHelperUi();
        }

        private void SetFailedChampionArmorPhase1Hp(int value)
        {
            if (Modules.BossChallenge.FailedChampionHelper.failedChampionP5Hp)
            {
                Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase1Hp = 360;
                RefreshFailedChampionHelperUi();
                return;
            }

            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.FailedChampionHelper.ReapplyLiveSettings();
        }

        private void SetFailedChampionArmorPhase2Hp(int value)
        {
            if (Modules.BossChallenge.FailedChampionHelper.failedChampionP5Hp)
            {
                Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase2Hp = 360;
                RefreshFailedChampionHelperUi();
                return;
            }

            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.FailedChampionHelper.ReapplyLiveSettings();
        }

        private void SetFailedChampionArmorPhase3Hp(int value)
        {
            if (Modules.BossChallenge.FailedChampionHelper.failedChampionP5Hp)
            {
                Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase3Hp = 360;
                RefreshFailedChampionHelperUi();
                return;
            }

            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase3Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.FailedChampionHelper.ReapplyLiveSettings();
        }

        private void RefreshFailedChampionHelperUi()
        {
            Module? module = GetFailedChampionHelperModule();
            UpdateToggleValue(failedChampionHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(failedChampionHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(failedChampionP5HpValue, Modules.BossChallenge.FailedChampionHelper.failedChampionP5Hp);
            UpdateIntInputValue(failedChampionArmorPhase1HpField, Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase1Hp);
            UpdateIntInputValue(failedChampionArmorPhase2HpField, Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase2Hp);
            UpdateIntInputValue(failedChampionArmorPhase3HpField, Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase3Hp);

            UpdateFailedChampionHelperInteractivity();
        }

        private void UpdateFailedChampionHelperInteractivity()
        {
            SetContentInteractivity(failedChampionHelperContent, GetFailedChampionHelperEnabled(), "FailedChampionHelperEnableRow");
            if (!GetFailedChampionHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.FailedChampionHelper.failedChampionP5Hp;
            SetRowInteractivity(failedChampionHelperContent, "FailedChampionArmorPhase1HpRow", !p5Hp);
            SetRowInteractivity(failedChampionHelperContent, "FailedChampionArmorPhase2HpRow", !p5Hp);
            SetRowInteractivity(failedChampionHelperContent, "FailedChampionArmorPhase3HpRow", !p5Hp);
        }

        private void SetFailedChampionHelperVisible(bool value)
        {
            failedChampionHelperVisible = value;
            if (failedChampionHelperRoot != null)
            {
                failedChampionHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshFailedChampionHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetSisterOfBattleHelperModule()
        {
            if (sisterOfBattleHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.SisterOfBattleHelper), out sisterOfBattleHelperModule);
            }

            return sisterOfBattleHelperModule;
        }

        private bool GetSisterOfBattleHelperEnabled()
        {
            Module? module = GetSisterOfBattleHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetSisterOfBattleHelperEnabled(bool value)
        {
            Module? module = GetSisterOfBattleHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateSisterOfBattleHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetSisterOfBattleP5HpEnabled(bool value)
        {
            Modules.BossChallenge.SisterOfBattleHelper.SetP5HpEnabled(value);
            RefreshSisterOfBattleHelperUi();
        }

        private void SetSisterOfBattlePhase1Hp(int value)
        {
            if (Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleP5Hp)
            {
                Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase1Hp = 500;
                RefreshSisterOfBattleHelperUi();
                return;
            }

            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.SisterOfBattleHelper.ReapplyLiveSettings();
        }

        private void SetSisterOfBattlePhase2S1Hp(int value)
        {
            if (Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleP5Hp)
            {
                Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S1Hp = 750;
                RefreshSisterOfBattleHelperUi();
                return;
            }

            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.SisterOfBattleHelper.ReapplyLiveSettings();
        }

        private void SetSisterOfBattlePhase2S2Hp(int value)
        {
            if (Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleP5Hp)
            {
                Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S2Hp = 750;
                RefreshSisterOfBattleHelperUi();
                return;
            }

            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.SisterOfBattleHelper.ReapplyLiveSettings();
        }

        private void SetSisterOfBattlePhase2S3Hp(int value)
        {
            if (Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleP5Hp)
            {
                Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S3Hp = 750;
                RefreshSisterOfBattleHelperUi();
                return;
            }

            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S3Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.SisterOfBattleHelper.ReapplyLiveSettings();
        }

        private void RefreshSisterOfBattleHelperUi()
        {
            Module? module = GetSisterOfBattleHelperModule();
            UpdateToggleValue(sisterOfBattleHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(sisterOfBattleHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(sisterOfBattleP5HpValue, Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleP5Hp);
            UpdateIntInputValue(sisterOfBattlePhase1HpField, Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase1Hp);
            UpdateIntInputValue(sisterOfBattlePhase2S1HpField, Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S1Hp);
            UpdateIntInputValue(sisterOfBattlePhase2S2HpField, Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S2Hp);
            UpdateIntInputValue(sisterOfBattlePhase2S3HpField, Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S3Hp);

            UpdateSisterOfBattleHelperInteractivity();
        }

        private void UpdateSisterOfBattleHelperInteractivity()
        {
            SetContentInteractivity(sisterOfBattleHelperContent, GetSisterOfBattleHelperEnabled(), "SisterOfBattleHelperEnableRow");
            if (!GetSisterOfBattleHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleP5Hp;
            SetRowInteractivity(sisterOfBattleHelperContent, "SisterOfBattlePhase1HpRow", !p5Hp);
            SetRowInteractivity(sisterOfBattleHelperContent, "SisterOfBattlePhase2S1HpRow", !p5Hp);
            SetRowInteractivity(sisterOfBattleHelperContent, "SisterOfBattlePhase2S2HpRow", !p5Hp);
            SetRowInteractivity(sisterOfBattleHelperContent, "SisterOfBattlePhase2S3HpRow", !p5Hp);
        }

        private void SetSisterOfBattleHelperVisible(bool value)
        {
            sisterOfBattleHelperVisible = value;
            if (sisterOfBattleHelperRoot != null)
            {
                sisterOfBattleHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshSisterOfBattleHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetMantisLordHelperModule()
        {
            if (mantisLordHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.MantisLordHelper), out mantisLordHelperModule);
            }

            return mantisLordHelperModule;
        }

        private bool GetMantisLordHelperEnabled()
        {
            Module? module = GetMantisLordHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetMantisLordHelperEnabled(bool value)
        {
            Module? module = GetMantisLordHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateMantisLordHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetMantisLordPhase1Hp(int value)
        {
            Modules.BossChallenge.MantisLordHelper.mantisLordPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.MantisLordHelper.ReapplyLiveSettings();
        }

        private void SetMantisLordPhase2S1Hp(int value)
        {
            Modules.BossChallenge.MantisLordHelper.mantisLordPhase2S1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.MantisLordHelper.ReapplyLiveSettings();
        }

        private void SetMantisLordPhase2S2Hp(int value)
        {
            Modules.BossChallenge.MantisLordHelper.mantisLordPhase2S2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.MantisLordHelper.ReapplyLiveSettings();
        }

        private void RefreshMantisLordHelperUi()
        {
            Module? module = GetMantisLordHelperModule();
            UpdateToggleValue(mantisLordHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(mantisLordHelperToggleIcon, module?.Enabled ?? false);
            UpdateIntInputValue(mantisLordPhase1HpField, Modules.BossChallenge.MantisLordHelper.mantisLordPhase1Hp);
            UpdateIntInputValue(mantisLordPhase2S1HpField, Modules.BossChallenge.MantisLordHelper.mantisLordPhase2S1Hp);
            UpdateIntInputValue(mantisLordPhase2S2HpField, Modules.BossChallenge.MantisLordHelper.mantisLordPhase2S2Hp);

            UpdateMantisLordHelperInteractivity();
        }

        private void UpdateMantisLordHelperInteractivity()
        {
            SetContentInteractivity(mantisLordHelperContent, GetMantisLordHelperEnabled(), "MantisLordHelperEnableRow");
            if (!GetMantisLordHelperEnabled())
            {
                return;
            }

            SetRowInteractivity(mantisLordHelperContent, "MantisLordPhase1HpRow", true);
            SetRowInteractivity(mantisLordHelperContent, "MantisLordPhase2S1HpRow", true);
            SetRowInteractivity(mantisLordHelperContent, "MantisLordPhase2S2HpRow", true);
        }

        private void SetMantisLordHelperVisible(bool value)
        {
            mantisLordHelperVisible = value;
            if (mantisLordHelperRoot != null)
            {
                mantisLordHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshMantisLordHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetFlukemarmHelperModule()
        {
            if (flukemarmHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.FlukemarmHelper), out flukemarmHelperModule);
            }

            return flukemarmHelperModule;
        }

        private bool GetFlukemarmHelperEnabled()
        {
            Module? module = GetFlukemarmHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetFlukemarmHelperEnabled(bool value)
        {
            Module? module = GetFlukemarmHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateFlukemarmHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetFlukemarmUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp)
            {
                Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHp = true;
                RefreshFlukemarmHelperUi();
                return;
            }

            Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHp = value;
            Modules.BossChallenge.FlukemarmHelper.ReapplyLiveSettings();
            RefreshFlukemarmHelperUi();
        }

        private void SetFlukemarmP5HpEnabled(bool value)
        {
            Modules.BossChallenge.FlukemarmHelper.SetP5HpEnabled(value);
            RefreshFlukemarmHelperUi();
        }

        private void SetFlukemarmUseCustomSummonLimitEnabled(bool value)
        {
            if (Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp)
            {
                Modules.BossChallenge.FlukemarmHelper.flukemarmUseCustomSummonLimit = false;
                RefreshFlukemarmHelperUi();
                return;
            }

            Modules.BossChallenge.FlukemarmHelper.flukemarmUseCustomSummonLimit = value;
            Modules.BossChallenge.FlukemarmHelper.ReapplyLiveSettings();
            RefreshFlukemarmHelperUi();
        }

        private void SetFlukemarmMaxHp(int value)
        {
            if (Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp)
            {
                Modules.BossChallenge.FlukemarmHelper.flukemarmMaxHp = 500;
                RefreshFlukemarmHelperUi();
                return;
            }

            Modules.BossChallenge.FlukemarmHelper.flukemarmMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHp)
            {
                Modules.BossChallenge.FlukemarmHelper.ApplyFlukemarmHealthIfPresent();
            }
        }

        private void SetFlukemarmFlyHp(int value)
        {
            if (Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp)
            {
                Modules.BossChallenge.FlukemarmHelper.flukemarmFlyHp = 35;
                RefreshFlukemarmHelperUi();
                return;
            }

            Modules.BossChallenge.FlukemarmHelper.flukemarmFlyHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHp)
            {
                Modules.BossChallenge.FlukemarmHelper.ApplyFlukemarmHealthIfPresent();
            }
        }

        private void SetFlukemarmSummonLimit(int value)
        {
            if (Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp)
            {
                Modules.BossChallenge.FlukemarmHelper.flukemarmSummonLimit = 6;
                RefreshFlukemarmHelperUi();
                return;
            }

            Modules.BossChallenge.FlukemarmHelper.flukemarmSummonLimit = Mathf.Clamp(value, 6, 999);
            if (Modules.BossChallenge.FlukemarmHelper.flukemarmUseCustomSummonLimit)
            {
                Modules.BossChallenge.FlukemarmHelper.ApplySummonLimitSettingsIfPresent();
            }
        }

        private void RefreshFlukemarmHelperUi()
        {
            Module? module = GetFlukemarmHelperModule();
            UpdateToggleValue(flukemarmHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(flukemarmHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(flukemarmP5HpValue, Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp);
            UpdateToggleValue(flukemarmUseMaxHpValue, Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHp);
            UpdateToggleValue(flukemarmUseCustomSummonLimitValue, Modules.BossChallenge.FlukemarmHelper.flukemarmUseCustomSummonLimit);
            UpdateIntInputValue(flukemarmMaxHpField, Modules.BossChallenge.FlukemarmHelper.flukemarmMaxHp);
            UpdateIntInputValue(flukemarmFlyHpField, Modules.BossChallenge.FlukemarmHelper.flukemarmFlyHp);
            UpdateIntInputValue(flukemarmSummonLimitField, Modules.BossChallenge.FlukemarmHelper.flukemarmSummonLimit);

            UpdateFlukemarmHelperInteractivity();
        }

        private void UpdateFlukemarmHelperInteractivity()
        {
            SetContentInteractivity(flukemarmHelperContent, GetFlukemarmHelperEnabled(), "FlukemarmHelperEnableRow");
            if (!GetFlukemarmHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHp;
            bool p5Hp = Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp;
            bool useCustomSummonLimit = Modules.BossChallenge.FlukemarmHelper.flukemarmUseCustomSummonLimit;
            SetRowInteractivity(flukemarmHelperContent, "FlukemarmUseMaxHpRow", !p5Hp);
            SetRowInteractivity(flukemarmHelperContent, "FlukemarmMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(flukemarmHelperContent, "FlukemarmFlyHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(flukemarmHelperContent, "FlukemarmUseCustomSummonLimitRow", !p5Hp);
            SetRowInteractivity(flukemarmHelperContent, "FlukemarmSummonLimitRow", useCustomSummonLimit && !p5Hp);
        }

        private void SetFlukemarmHelperVisible(bool value)
        {
            flukemarmHelperVisible = value;
            if (flukemarmHelperRoot != null)
            {
                flukemarmHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshFlukemarmHelperUi();
            }

            UpdateUiState();
        }

        private Module? GetVengeflyKingModule()
        {
            if (vengeflyKingModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.VengeflyKing), out vengeflyKingModule);
            }

            return vengeflyKingModule;
        }

        private bool GetVengeflyKingEnabled()
        {
            Module? module = GetVengeflyKingModule();
            return module?.Enabled ?? false;
        }

        private void SetVengeflyKingEnabled(bool value)
        {
            Module? module = GetVengeflyKingModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateVengeflyKingInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetVengeflyKingUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp = true;
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp = value;
            Modules.BossChallenge.VengeflyKing.ReapplyLiveSettings();
            RefreshVengeflyKingUi();
        }

        private void SetVengeflyKingP5HpEnabled(bool value)
        {
            Modules.BossChallenge.VengeflyKing.SetP5HpEnabled(value);
            RefreshVengeflyKingUi();
        }

        private void SetVengeflyKingUseCustomSummonLimitEnabled(bool value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit = false;
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit = value;
            Modules.BossChallenge.VengeflyKing.ReapplyLiveSettings();
            RefreshVengeflyKingUi();
        }

        private void SetVengeflyKingLeftMaxHp(int value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                Modules.BossChallenge.VengeflyKing.vengeflyKingLeftMaxHp = 450;
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingLeftMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp)
            {
                Modules.BossChallenge.VengeflyKing.ApplyVengeflyHealthIfPresent();
            }
        }

        private void SetVengeflyKingRightMaxHp(int value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                Modules.BossChallenge.VengeflyKing.vengeflyKingRightMaxHp = 190;
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingRightMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp)
            {
                Modules.BossChallenge.VengeflyKing.ApplyVengeflyHealthIfPresent();
            }
        }

        private void SetVengeflyKingSummonMaxHp(int value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                Modules.BossChallenge.VengeflyKing.vengeflyKingSummonMaxHp = 8;
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingSummonMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp)
            {
                Modules.BossChallenge.VengeflyKing.ApplyVengeflyHealthIfPresent();
            }
        }

        private void SetVengeflyKingLeftSummonLimit(int value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonLimit = 4;
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonLimit = Mathf.Clamp(value, 0, 999);
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit)
            {
                Modules.BossChallenge.VengeflyKing.ApplySummonLimitSettingsIfPresent();
            }
        }

        private void SetVengeflyKingRightSummonLimit(int value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonLimit = 4;
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonLimit = Mathf.Clamp(value, 0, 999);
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit)
            {
                Modules.BossChallenge.VengeflyKing.ApplySummonLimitSettingsIfPresent();
            }
        }

        private void SetVengeflyKingLeftSummonAttackLimit(int value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonAttackLimit = Mathf.Clamp(value, 0, 999);
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit)
            {
                Modules.BossChallenge.VengeflyKing.ApplySummonLimitSettingsIfPresent();
            }
        }

        private void SetVengeflyKingRightSummonAttackLimit(int value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonAttackLimit = Mathf.Clamp(value, 0, 999);
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit)
            {
                Modules.BossChallenge.VengeflyKing.ApplySummonLimitSettingsIfPresent();
            }
        }

        private void RefreshVengeflyKingUi()
        {
            Module? module = GetVengeflyKingModule();
            UpdateToggleValue(vengeflyKingToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(vengeflyKingToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(vengeflyKingP5HpValue, Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp);
            UpdateToggleValue(vengeflyKingUseMaxHpValue, Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp);
            UpdateToggleValue(vengeflyKingUseCustomSummonLimitValue, Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit);
            UpdateIntInputValue(vengeflyKingLeftMaxHpField, Modules.BossChallenge.VengeflyKing.vengeflyKingLeftMaxHp);
            UpdateIntInputValue(vengeflyKingRightMaxHpField, Modules.BossChallenge.VengeflyKing.vengeflyKingRightMaxHp);
            UpdateIntInputValue(vengeflyKingSummonMaxHpField, Modules.BossChallenge.VengeflyKing.vengeflyKingSummonMaxHp);
            UpdateIntInputValue(vengeflyKingLeftSummonLimitField, Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonLimit);
            UpdateIntInputValue(vengeflyKingRightSummonLimitField, Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonLimit);
            UpdateIntInputValue(vengeflyKingLeftSummonAttackLimitField, Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonAttackLimit);
            UpdateIntInputValue(vengeflyKingRightSummonAttackLimitField, Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonAttackLimit);

            UpdateVengeflyKingInteractivity();
        }

        private void UpdateVengeflyKingInteractivity()
        {
            SetContentInteractivity(vengeflyKingContent, GetVengeflyKingEnabled(), "VengeflyKingEnableRow");
            if (!GetVengeflyKingEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp;
            bool p5Hp = Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp;
            bool useCustomSummonLimit = Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit;
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingUseMaxHpRow", !p5Hp);
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingLeftMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingRightMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingSummonMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingUseCustomSummonLimitRow", !p5Hp);
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingLeftSummonLimitRow", useCustomSummonLimit && !p5Hp);
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingRightSummonLimitRow", useCustomSummonLimit && !p5Hp);
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingLeftSummonAttackLimitRow", useCustomSummonLimit && !p5Hp);
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingRightSummonAttackLimitRow", useCustomSummonLimit && !p5Hp);
        }

        private void SetVengeflyKingVisible(bool value)
        {
            vengeflyKingVisible = value;
            if (vengeflyKingRoot != null)
            {
                vengeflyKingRoot.SetActive(value);
            }

            if (value)
            {
                RefreshVengeflyKingUi();
            }

            UpdateUiState();
        }

        private void OpenAdditionalGhostHelperOverlay(Action<bool> setVisible)
        {
            returnToBossManipulateOnClose = true;
            SetBossManipulateVisible(false);
            setVisible(true);
        }

        private void CloseAdditionalGhostHelperOverlay(Action<bool> setVisible)
        {
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            setVisible(false);
        }

        private void OnBossManipulateMarmuClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetMarmuHelperVisible);
        }

        private void OnMarmuHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetMarmuHelperVisible);
        }

        private void OnMarmuHelperResetDefaultsClicked()
        {
            ResetMarmuHelperDefaults();
            SetMarmuHelperEnabled(false);
            Modules.BossChallenge.MarmuHelper.RestoreVanillaHealthIfPresent();
            RefreshMarmuHelperUi();
        }

        private void OnBossManipulateXeroClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetXeroHelperVisible);
        }

        private void OnXeroHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetXeroHelperVisible);
        }

        private void OnXeroHelperResetDefaultsClicked()
        {
            ResetXeroHelperDefaults();
            SetXeroHelperEnabled(false);
            Modules.BossChallenge.XeroHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.XeroHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshXeroHelperUi();
        }

        private void OnBossManipulateMarkothClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetMarkothHelperVisible);
        }

        private void OnMarkothHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetMarkothHelperVisible);
        }

        private void OnMarkothHelperResetDefaultsClicked()
        {
            ResetMarkothHelperDefaults();
            SetMarkothHelperEnabled(false);
            Modules.BossChallenge.MarkothHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.MarkothHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshMarkothHelperUi();
        }

        private void OnBossManipulateGalienClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetGalienHelperVisible);
        }

        private void OnGalienHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetGalienHelperVisible);
        }

        private void OnGalienHelperResetDefaultsClicked()
        {
            ResetGalienHelperDefaults();
            SetGalienHelperEnabled(false);
            Modules.BossChallenge.GalienHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.GalienHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshGalienHelperUi();
        }

        private void OnBossManipulateGorbClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetGorbHelperVisible);
        }

        private void OnGorbHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetGorbHelperVisible);
        }

        private void OnGorbHelperResetDefaultsClicked()
        {
            ResetGorbHelperDefaults();
            SetGorbHelperEnabled(false);
            Modules.BossChallenge.GorbHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.GorbHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshGorbHelperUi();
        }

        private void OnBossManipulateElderHuClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetElderHuHelperVisible);
        }

        private void OnElderHuHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetElderHuHelperVisible);
        }

        private void OnElderHuHelperResetDefaultsClicked()
        {
            ResetElderHuHelperDefaults();
            SetElderHuHelperEnabled(false);
            Modules.BossChallenge.ElderHuHelper.RestoreVanillaHealthIfPresent();
            RefreshElderHuHelperUi();
        }

        private void OnBossManipulateNoEyesClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetNoEyesHelperVisible);
        }

        private void OnNoEyesHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetNoEyesHelperVisible);
        }

        private void OnNoEyesHelperResetDefaultsClicked()
        {
            ResetNoEyesHelperDefaults();
            SetNoEyesHelperEnabled(false);
            Modules.BossChallenge.NoEyesHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.NoEyesHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshNoEyesHelperUi();
        }

        private void OnBossManipulateDungDefenderClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetDungDefenderHelperVisible);
        }

        private void OnDungDefenderHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetDungDefenderHelperVisible);
        }

        private void OnDungDefenderHelperResetDefaultsClicked()
        {
            ResetDungDefenderHelperDefaults();
            SetDungDefenderHelperEnabled(false);
            Modules.BossChallenge.DungDefenderHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.DungDefenderHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshDungDefenderHelperUi();
        }

        private void OnBossManipulateWhiteDefenderClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetWhiteDefenderHelperVisible);
        }

        private void OnWhiteDefenderHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetWhiteDefenderHelperVisible);
        }

        private void OnWhiteDefenderHelperResetDefaultsClicked()
        {
            ResetWhiteDefenderHelperDefaults();
            SetWhiteDefenderHelperEnabled(false);
            Modules.BossChallenge.WhiteDefenderHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.WhiteDefenderHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshWhiteDefenderHelperUi();
        }

        private void OnBossManipulateHiveKnightClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetHiveKnightHelperVisible);
        }

        private void OnHiveKnightHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetHiveKnightHelperVisible);
        }

        private void OnHiveKnightHelperResetDefaultsClicked()
        {
            ResetHiveKnightHelperDefaults();
            SetHiveKnightHelperEnabled(false);
            Modules.BossChallenge.HiveKnightHelper.RestoreVanillaHealthIfPresent();
            RefreshHiveKnightHelperUi();
        }

        private void OnBossManipulateBrokenVesselClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetBrokenVesselHelperVisible);
        }

        private void OnBrokenVesselHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetBrokenVesselHelperVisible);
        }

        private void OnBrokenVesselHelperResetDefaultsClicked()
        {
            ResetBrokenVesselHelperDefaults();
            SetBrokenVesselHelperEnabled(false);
            Modules.BossChallenge.BrokenVesselHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.BrokenVesselHelper.RestoreVanillaSummonHealthIfPresent();
            Modules.BossChallenge.BrokenVesselHelper.RestoreVanillaSummonLimitsIfPresent();
            Modules.BossChallenge.BrokenVesselHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshBrokenVesselHelperUi();
        }

        private void OnBossManipulateLostKinClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetLostKinHelperVisible);
        }

        private void OnLostKinHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetLostKinHelperVisible);
        }

        private void OnLostKinHelperResetDefaultsClicked()
        {
            ResetLostKinHelperDefaults();
            SetLostKinHelperEnabled(false);
            Modules.BossChallenge.LostKinHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.LostKinHelper.RestoreVanillaSummonHealthIfPresent();
            Modules.BossChallenge.LostKinHelper.RestoreVanillaSummonLimitsIfPresent();
            Modules.BossChallenge.LostKinHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshLostKinHelperUi();
        }

        private void OnBossManipulateNoskClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetNoskHelperVisible);
        }

        private void OnNoskHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetNoskHelperVisible);
        }

        private void OnNoskHelperResetDefaultsClicked()
        {
            ResetNoskHelperDefaults();
            SetNoskHelperEnabled(false);
            Modules.BossChallenge.NoskHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.NoskHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshNoskHelperUi();
        }

        private void OnBossManipulateWingedNoskClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetWingedNoskHelperVisible);
        }

        private void OnWingedNoskHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetWingedNoskHelperVisible);
        }

        private void OnWingedNoskHelperResetDefaultsClicked()
        {
            ResetWingedNoskHelperDefaults();
            SetWingedNoskHelperEnabled(false);
            Modules.BossChallenge.WingedNoskHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.WingedNoskHelper.RestoreVanillaSummonHealthIfPresent();
            Modules.BossChallenge.WingedNoskHelper.RestoreVanillaSummonLimitsIfPresent();
            Modules.BossChallenge.WingedNoskHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshWingedNoskHelperUi();
        }

        private void OnBossManipulateUumuuClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetUumuuHelperVisible);
        }

        private void OnUumuuHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetUumuuHelperVisible);
        }

        private void OnUumuuHelperResetDefaultsClicked()
        {
            ResetUumuuHelperDefaults();
            SetUumuuHelperEnabled(false);
            Modules.BossChallenge.UumuuHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.UumuuHelper.RestoreVanillaSummonHealthIfPresent();
            RefreshUumuuHelperUi();
        }

        private void OnBossManipulateTraitorLordClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetTraitorLordHelperVisible);
        }

        private void OnTraitorLordHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetTraitorLordHelperVisible);
        }

        private void OnTraitorLordHelperResetDefaultsClicked()
        {
            ResetTraitorLordHelperDefaults();
            SetTraitorLordHelperEnabled(false);
            Modules.BossChallenge.TraitorLordHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.TraitorLordHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshTraitorLordHelperUi();
        }

        private void OnBossManipulateTroupeMasterGrimmClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetTroupeMasterGrimmHelperVisible);
        }

        private void OnTroupeMasterGrimmHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetTroupeMasterGrimmHelperVisible);
        }

        private void OnTroupeMasterGrimmHelperResetDefaultsClicked()
        {
            ResetTroupeMasterGrimmHelperDefaults();
            SetTroupeMasterGrimmHelperEnabled(false);
            Modules.BossChallenge.TroupeMasterGrimmHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.TroupeMasterGrimmHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshTroupeMasterGrimmHelperUi();
        }

        private void OnBossManipulateNightmareKingGrimmClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetNightmareKingGrimmHelperVisible);
        }

        private void OnNightmareKingGrimmHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetNightmareKingGrimmHelperVisible);
        }

        private void OnNightmareKingGrimmHelperResetDefaultsClicked()
        {
            ResetNightmareKingGrimmHelperDefaults();
            SetNightmareKingGrimmHelperEnabled(false);
            Modules.BossChallenge.NightmareKingGrimmHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.NightmareKingGrimmHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshNightmareKingGrimmHelperUi();
        }

        private void OnBossManipulatePureVesselClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetPureVesselHelperVisible);
        }

        private void OnPureVesselHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetPureVesselHelperVisible);
        }

        private void OnPureVesselHelperResetDefaultsClicked()
        {
            ResetPureVesselHelperDefaults();
            SetPureVesselHelperEnabled(false);
            Modules.BossChallenge.PureVesselHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.PureVesselHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshPureVesselHelperUi();
        }

        private void OnBossManipulateAbsoluteRadianceClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetAbsoluteRadianceHelperVisible);
        }

        private void OnAbsoluteRadianceHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetAbsoluteRadianceHelperVisible);
        }

        private void OnAbsoluteRadianceHelperResetDefaultsClicked()
        {
            ResetAbsoluteRadianceHelperDefaults();
            SetAbsoluteRadianceHelperEnabled(false);
            Modules.BossChallenge.AbsoluteRadianceHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.AbsoluteRadianceHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshAbsoluteRadianceHelperUi();
        }

        private void OnBossManipulatePaintmasterSheoClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetPaintmasterSheoHelperVisible);
        }

        private void OnPaintmasterSheoHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetPaintmasterSheoHelperVisible);
        }

        private void OnPaintmasterSheoHelperResetDefaultsClicked()
        {
            ResetPaintmasterSheoHelperDefaults();
            SetPaintmasterSheoHelperEnabled(false);
            Modules.BossChallenge.PaintmasterSheoHelper.RestoreVanillaHealthIfPresent();
            RefreshPaintmasterSheoHelperUi();
        }

        private void OnBossManipulateSoulWarriorClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetSoulWarriorHelperVisible);
        }

        private void OnSoulWarriorHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetSoulWarriorHelperVisible);
        }

        private void OnSoulWarriorHelperResetDefaultsClicked()
        {
            ResetSoulWarriorHelperDefaults();
            SetSoulWarriorHelperEnabled(false);
            Modules.BossChallenge.SoulWarriorHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.SoulWarriorHelper.RestoreVanillaSummonHealthIfPresent();
            Modules.BossChallenge.SoulWarriorHelper.RestoreVanillaSummonLimitsIfPresent();
            RefreshSoulWarriorHelperUi();
        }

        private void OnBossManipulateNailsageSlyClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetNailsageSlyHelperVisible);
        }

        private void OnNailsageSlyHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetNailsageSlyHelperVisible);
        }

        private void OnNailsageSlyHelperResetDefaultsClicked()
        {
            ResetNailsageSlyHelperDefaults();
            SetNailsageSlyHelperEnabled(false);
            Modules.BossChallenge.NailsageSlyHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.NailsageSlyHelper.RestoreVanillaPhase2HpIfPresent();
            RefreshNailsageSlyHelperUi();
        }

        private void OnBossManipulateSoulMasterClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetSoulMasterHelperVisible);
        }

        private void OnSoulMasterHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetSoulMasterHelperVisible);
        }

        private void OnSoulMasterHelperResetDefaultsClicked()
        {
            ResetSoulMasterHelperDefaults();
            SetSoulMasterHelperEnabled(false);
            Modules.BossChallenge.SoulMasterHelper.RestoreVanillaHealthIfPresent();
            RefreshSoulMasterHelperUi();
        }

        private void OnBossManipulateSoulTyrantClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetSoulTyrantHelperVisible);
        }

        private void OnSoulTyrantHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetSoulTyrantHelperVisible);
        }

        private void OnSoulTyrantHelperResetDefaultsClicked()
        {
            ResetSoulTyrantHelperDefaults();
            SetSoulTyrantHelperEnabled(false);
            Modules.BossChallenge.SoulTyrantHelper.RestoreVanillaHealthIfPresent();
            RefreshSoulTyrantHelperUi();
        }

        private void OnBossManipulateWatcherKnightClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetWatcherKnightHelperVisible);
        }

        private void OnWatcherKnightHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetWatcherKnightHelperVisible);
        }

        private void OnWatcherKnightHelperResetDefaultsClicked()
        {
            ResetWatcherKnightHelperDefaults();
            SetWatcherKnightHelperEnabled(false);
            Modules.BossChallenge.WatcherKnightHelper.RestoreVanillaHealthIfPresent();
            RefreshWatcherKnightHelperUi();
        }

        private void OnBossManipulateOroMatoClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetOroMatoHelperVisible);
        }

        private void OnOroMatoHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetOroMatoHelperVisible);
        }

        private void OnOroMatoHelperResetDefaultsClicked()
        {
            ResetOroMatoHelperDefaults();
            SetOroMatoHelperEnabled(false);
            Modules.BossChallenge.OroMatoHelper.RestoreVanillaHealthIfPresent();
            RefreshOroMatoHelperUi();
        }

        private void OnBossManipulateGodTamerClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetGodTamerHelperVisible);
        }

        private void OnGodTamerHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetGodTamerHelperVisible);
        }

        private void OnGodTamerHelperResetDefaultsClicked()
        {
            ResetGodTamerHelperDefaults();
            SetGodTamerHelperEnabled(false);
            Modules.BossChallenge.GodTamerHelper.RestoreVanillaHealthIfPresent();
            RefreshGodTamerHelperUi();
        }

        private void OnBossManipulateOblobblesClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetOblobblesHelperVisible);
        }

        private void OnOblobblesHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetOblobblesHelperVisible);
        }

        private void OnOblobblesHelperResetDefaultsClicked()
        {
            ResetOblobblesHelperDefaults();
            SetOblobblesHelperEnabled(false);
            Modules.BossChallenge.OblobblesHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.OblobblesHelper.RestoreVanillaPhase2SettingsIfPresent();
            RefreshOblobblesHelperUi();
        }

        private void OnBossManipulateFalseKnightClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetFalseKnightHelperVisible);
        }

        private void OnFalseKnightHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetFalseKnightHelperVisible);
        }

        private void OnFalseKnightHelperResetDefaultsClicked()
        {
            ResetFalseKnightHelperDefaults();
            SetFalseKnightHelperEnabled(false);
            Modules.BossChallenge.FalseKnightHelper.RestoreVanillaHealthIfPresent();
            RefreshFalseKnightHelperUi();
        }

        private void OnBossManipulateFailedChampionClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetFailedChampionHelperVisible);
        }

        private void OnFailedChampionHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetFailedChampionHelperVisible);
        }

        private void OnFailedChampionHelperResetDefaultsClicked()
        {
            ResetFailedChampionHelperDefaults();
            SetFailedChampionHelperEnabled(false);
            Modules.BossChallenge.FailedChampionHelper.RestoreVanillaHealthIfPresent();
            RefreshFailedChampionHelperUi();
        }

        private void OnBossManipulateFlukemarmClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetFlukemarmHelperVisible);
        }

        private void OnFlukemarmHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetFlukemarmHelperVisible);
        }

        private void OnFlukemarmHelperResetDefaultsClicked()
        {
            ResetFlukemarmHelperDefaults();
            SetFlukemarmHelperEnabled(false);
            Modules.BossChallenge.FlukemarmHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.FlukemarmHelper.RestoreVanillaSummonLimitsIfPresent();
            RefreshFlukemarmHelperUi();
        }

        private void OnBossManipulateVengeflyKingClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetVengeflyKingVisible);
        }

        private void OnVengeflyKingBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetVengeflyKingVisible);
        }

        private void OnVengeflyKingResetDefaultsClicked()
        {
            ResetVengeflyKingDefaults();
            SetVengeflyKingEnabled(false);
            Modules.BossChallenge.VengeflyKing.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.VengeflyKing.RestoreVanillaSummonLimitsIfPresent();
            RefreshVengeflyKingUi();
        }

        private void OnBossManipulateSisterOfBattleClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetSisterOfBattleHelperVisible);
        }

        private void OnSisterOfBattleHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetSisterOfBattleHelperVisible);
        }

        private void OnSisterOfBattleHelperResetDefaultsClicked()
        {
            ResetSisterOfBattleHelperDefaults();
            SetSisterOfBattleHelperEnabled(false);
            Modules.BossChallenge.SisterOfBattleHelper.RestoreVanillaHealthIfPresent();
            RefreshSisterOfBattleHelperUi();
        }

        private void OnBossManipulateMantisLordClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetMantisLordHelperVisible);
        }

        private void OnMantisLordHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetMantisLordHelperVisible);
        }

        private void OnMantisLordHelperResetDefaultsClicked()
        {
            ResetMantisLordHelperDefaults();
            SetMantisLordHelperEnabled(false);
            Modules.BossChallenge.MantisLordHelper.RestoreVanillaHealthIfPresent();
            RefreshMantisLordHelperUi();
        }

        private void AppendAdditionalGhostHelperRows(RectTransform content, ref float rowY, ref float lastY)
        {
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateMarmuRow", "Modules/MarmuHelper".Localize(), rowY, OnBossManipulateMarmuClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateXeroRow", "Modules/XeroHelper".Localize(), rowY, OnBossManipulateXeroClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateMarkothRow", "Modules/MarkothHelper".Localize(), rowY, OnBossManipulateMarkothClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateGalienRow", "Modules/GalienHelper".Localize(), rowY, OnBossManipulateGalienClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateGorbRow", "Modules/GorbHelper".Localize(), rowY, OnBossManipulateGorbClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateElderHuRow", "Modules/ElderHuHelper".Localize(), rowY, OnBossManipulateElderHuClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateNoEyesRow", "Modules/NoEyesHelper".Localize(), rowY, OnBossManipulateNoEyesClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateDungDefenderRow", "Modules/DungDefenderHelper".Localize(), rowY, OnBossManipulateDungDefenderClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateWhiteDefenderRow", "Modules/WhiteDefenderHelper".Localize(), rowY, OnBossManipulateWhiteDefenderClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateHiveKnightRow", "Modules/HiveKnightHelper".Localize(), rowY, OnBossManipulateHiveKnightClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateBrokenVesselRow", "Modules/BrokenVesselHelper".Localize(), rowY, OnBossManipulateBrokenVesselClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateLostKinRow", "Modules/LostKinHelper".Localize(), rowY, OnBossManipulateLostKinClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateNoskRow", "Modules/NoskHelper".Localize(), rowY, OnBossManipulateNoskClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateWingedNoskRow", "Modules/WingedNoskHelper".Localize(), rowY, OnBossManipulateWingedNoskClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateUumuuRow", "Modules/UumuuHelper".Localize(), rowY, OnBossManipulateUumuuClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateTraitorLordRow", "Modules/TraitorLordHelper".Localize(), rowY, OnBossManipulateTraitorLordClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateTroupeMasterGrimmRow", "Modules/TroupeMasterGrimmHelper".Localize(), rowY, OnBossManipulateTroupeMasterGrimmClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateNightmareKingGrimmRow", "Modules/NightmareKingGrimmHelper".Localize(), rowY, OnBossManipulateNightmareKingGrimmClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulatePureVesselRow", "Modules/PureVesselHelper".Localize(), rowY, OnBossManipulatePureVesselClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateAbsoluteRadianceRow", "Modules/AbsoluteRadianceHelper".Localize(), rowY, OnBossManipulateAbsoluteRadianceClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulatePaintmasterSheoRow", "Modules/PaintmasterSheoHelper".Localize(), rowY, OnBossManipulatePaintmasterSheoClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateSoulWarriorRow", "Modules/SoulWarriorHelper".Localize(), rowY, OnBossManipulateSoulWarriorClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateNailsageSlyRow", "Modules/NailsageSlyHelper".Localize(), rowY, OnBossManipulateNailsageSlyClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateSoulMasterRow", "Modules/SoulMasterHelper".Localize(), rowY, OnBossManipulateSoulMasterClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateSoulTyrantRow", "Modules/SoulTyrantHelper".Localize(), rowY, OnBossManipulateSoulTyrantClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateWatcherKnightRow", "Modules/WatcherKnightHelper".Localize(), rowY, OnBossManipulateWatcherKnightClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateOroMatoRow", "Modules/OroMatoHelper".Localize(), rowY, OnBossManipulateOroMatoClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateGodTamerRow", "Modules/GodTamerHelper".Localize(), rowY, OnBossManipulateGodTamerClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateOblobblesRow", "Modules/OblobblesHelper".Localize(), rowY, OnBossManipulateOblobblesClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateFalseKnightRow", "Modules/FalseKnightHelper".Localize(), rowY, OnBossManipulateFalseKnightClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateFailedChampionRow", "Modules/FailedChampionHelper".Localize(), rowY, OnBossManipulateFailedChampionClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateFlukemarmRow", "Modules/FlukemarmHelper".Localize(), rowY, OnBossManipulateFlukemarmClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateVengeflyKingRow", "Modules/VengeflyKing".Localize(), rowY, OnBossManipulateVengeflyKingClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateSisterOfBattleRow", "Modules/SisterOfBattleHelper".Localize(), rowY, OnBossManipulateSisterOfBattleClicked);
            lastY = rowY;
            rowY += BossManipulateRowSpacing;
            CreateButtonRow(content, "BossManipulateMantisLordRow", "Modules/MantisLordHelper".Localize(), rowY, OnBossManipulateMantisLordClicked);
            lastY = rowY;
        }

        private void BuildAdditionalGhostHelpersOverlayUi()
        {
            BuildMarmuHelperOverlayUi();
            BuildXeroHelperOverlayUi();
            BuildMarkothHelperOverlayUi();
            BuildGalienHelperOverlayUi();
            BuildGorbHelperOverlayUi();
            BuildElderHuHelperOverlayUi();
            BuildNoEyesHelperOverlayUi();
            BuildDungDefenderHelperOverlayUi();
            BuildWhiteDefenderHelperOverlayUi();
            BuildHiveKnightHelperOverlayUi();
            BuildBrokenVesselHelperOverlayUi();
            BuildLostKinHelperOverlayUi();
            BuildNoskHelperOverlayUi();
            BuildWingedNoskHelperOverlayUi();
            BuildUumuuHelperOverlayUi();
            BuildTraitorLordHelperOverlayUi();
            BuildTroupeMasterGrimmHelperOverlayUi();
            BuildNightmareKingGrimmHelperOverlayUi();
            BuildPureVesselHelperOverlayUi();
            BuildAbsoluteRadianceHelperOverlayUi();
            BuildPaintmasterSheoHelperOverlayUi();
            BuildSoulWarriorHelperOverlayUi();
            BuildNailsageSlyHelperOverlayUi();
            BuildSoulMasterHelperOverlayUi();
            BuildSoulTyrantHelperOverlayUi();
            BuildWatcherKnightHelperOverlayUi();
            BuildOroMatoHelperOverlayUi();
            BuildGodTamerHelperOverlayUi();
            BuildOblobblesHelperOverlayUi();
            BuildFalseKnightHelperOverlayUi();
            BuildFailedChampionHelperOverlayUi();
            BuildFlukemarmHelperOverlayUi();
            BuildVengeflyKingOverlayUi();
            BuildSisterOfBattleHelperOverlayUi();
            BuildMantisLordHelperOverlayUi();
        }

        private void BuildStandardGhostHelperOverlayUi(
            ref GameObject? helperRoot,
            ref RectTransform? helperContent,
            string helperKey,
            int canvasSortOrder,
            float panelHeight,
            float rowSpacing,
            Func<bool> getEnabled,
            Action<bool> setEnabled,
            Func<bool> getP5Hp,
            Action<bool> setP5Hp,
            Func<bool> getUseMaxHp,
            Action<bool> setUseMaxHp,
            Func<int> getMaxHp,
            Action<int> setMaxHp,
            Action onResetClicked,
            Action onBackClicked,
            out Text? toggleValue,
            out Image? toggleIcon,
            out Text? p5HpValue,
            out Text? useMaxHpValue,
            out InputField? maxHpField)
        {
            helperRoot = new GameObject($"{helperKey}OverlayCanvas");
            helperRoot.transform.SetParent(transform, false);

            Canvas canvas = helperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = canvasSortOrder;

            CanvasScaler scaler = helperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            helperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = helperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(helperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject($"{helperKey}Panel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, panelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", $"Modules/{helperKey}".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            helperContent = content;

            string rowKey = helperKey.EndsWith("Helper", StringComparison.Ordinal)
                ? helperKey.Substring(0, helperKey.Length - "Helper".Length)
                : helperKey;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                $"{helperKey}EnableRow",
                $"Settings/{helperKey}/Enable".Localize(),
                rowY,
                getEnabled,
                setEnabled,
                out toggleValue,
                out toggleIcon
            );

            lastY = rowY;
            rowY += rowSpacing;
            CreateToggleRow(
                content,
                $"{rowKey}P5HpRow",
                $"Settings/{helperKey}/P5HP".Localize(),
                rowY,
                getP5Hp,
                setP5Hp,
                out p5HpValue
            );

            lastY = rowY;
            rowY += rowSpacing;
            CreateToggleRow(
                content,
                $"{rowKey}UseMaxHpRow",
                $"Settings/{helperKey}/UseMaxHP".Localize(),
                rowY,
                getUseMaxHp,
                setUseMaxHp,
                out useMaxHpValue
            );

            lastY = rowY;
            rowY += rowSpacing;
            CreateAdjustInputRow(
                content,
                $"{rowKey}MaxHpRow",
                $"Settings/{helperKey}/MaxHP".Localize(),
                rowY,
                getMaxHp,
                setMaxHp,
                1,
                999999,
                10,
                out maxHpField
            );

            lastY = rowY;
            rowY += rowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, $"{helperKey}ResetRow", $"Settings/{helperKey}/Reset".Localize(), resetY, onResetClicked);
            CreateButtonRow(panel.transform, $"{helperKey}BackRow", "Back", backY, onBackClicked);
        }


        private void BuildMarmuHelperOverlayUi()
        {
            BuildStandardGhostHelperOverlayUi(
                ref marmuHelperRoot,
                ref marmuHelperContent,
                "MarmuHelper",
                MarmuHelperCanvasSortOrder,
                MarmuHelperPanelHeight,
                MarmuHelperRowSpacing,
                GetMarmuHelperEnabled,
                SetMarmuHelperEnabled,
                () => Modules.BossChallenge.MarmuHelper.marmuP5Hp,
                SetMarmuP5HpEnabled,
                () => Modules.BossChallenge.MarmuHelper.marmuUseMaxHp,
                SetMarmuUseMaxHpEnabled,
                () => Modules.BossChallenge.MarmuHelper.marmuMaxHp,
                SetMarmuMaxHp,
                OnMarmuHelperResetDefaultsClicked,
                OnMarmuHelperBackClicked,
                out marmuHelperToggleValue,
                out marmuHelperToggleIcon,
                out marmuP5HpValue,
                out marmuUseMaxHpValue,
                out marmuMaxHpField);
        }

        private void BuildXeroHelperOverlayUi()
        {
            xeroHelperRoot = new GameObject("XeroHelperOverlayCanvas");
            xeroHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = xeroHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = XeroHelperCanvasSortOrder;

            CanvasScaler scaler = xeroHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            xeroHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = xeroHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(xeroHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("XeroHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, XeroHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/XeroHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = XeroHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            xeroHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "XeroHelperEnableRow",
                "Settings/XeroHelper/Enable".Localize(),
                rowY,
                GetXeroHelperEnabled,
                SetXeroHelperEnabled,
                out xeroHelperToggleValue,
                out xeroHelperToggleIcon
            );

            lastY = rowY;
            rowY += XeroHelperRowSpacing;
            CreateToggleRow(
                content,
                "XeroP5HpRow",
                "Settings/XeroHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.XeroHelper.xeroP5Hp,
                SetXeroP5HpEnabled,
                out xeroP5HpValue
            );

            lastY = rowY;
            rowY += XeroHelperRowSpacing;
            CreateToggleRow(
                content,
                "XeroUseMaxHpRow",
                "Settings/XeroHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.XeroHelper.xeroUseMaxHp,
                SetXeroUseMaxHpEnabled,
                out xeroUseMaxHpValue
            );

            lastY = rowY;
            rowY += XeroHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "XeroMaxHpRow",
                "Settings/XeroHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.XeroHelper.xeroMaxHp,
                SetXeroMaxHp,
                1,
                999999,
                10,
                out xeroMaxHpField
            );

            lastY = rowY;
            rowY += XeroHelperRowSpacing;
            CreateToggleRow(
                content,
                "XeroUseCustomPhaseRow",
                "Settings/XeroHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.XeroHelper.xeroUseCustomPhase,
                SetXeroUseCustomPhaseEnabled,
                out xeroUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += XeroHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "XeroPhase2HpRow",
                "Settings/XeroHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.XeroHelper.xeroPhase2Hp,
                SetXeroPhase2Hp,
                1,
                999999,
                10,
                out xeroPhase2HpField
            );

            lastY = rowY;
            rowY += XeroHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "XeroHelperResetRow", "Settings/XeroHelper/Reset".Localize(), resetY, OnXeroHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "XeroHelperBackRow", "Back", backY, OnXeroHelperBackClicked);
        }

        private void BuildMarkothHelperOverlayUi()
        {
            markothHelperRoot = new GameObject("MarkothHelperOverlayCanvas");
            markothHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = markothHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = MarkothHelperCanvasSortOrder;

            CanvasScaler scaler = markothHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            markothHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = markothHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(markothHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("MarkothHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, MarkothHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/MarkothHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(MarkothHelperPanelHeight);
            float backY = GetFixedBackY(MarkothHelperPanelHeight);
            float topOffset = GetScrollTopOffset(MarkothHelperPanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(MarkothHelperPanelHeight, resetY);
            float viewHeight = Mathf.Max(0f, MarkothHelperPanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, MarkothHelperPanelHeight, topOffset, bottomOffset);
            markothHelperContent = content;

            float rowY = GetRowStartY(MarkothHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "MarkothHelperEnableRow",
                "Settings/MarkothHelper/Enable".Localize(),
                rowY,
                GetMarkothHelperEnabled,
                SetMarkothHelperEnabled,
                out markothHelperToggleValue,
                out markothHelperToggleIcon
            );

            lastY = rowY;
            rowY += MarkothHelperRowSpacing;
            CreateToggleRow(
                content,
                "MarkothP5HpRow",
                "Settings/MarkothHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.MarkothHelper.markothP5Hp,
                SetMarkothP5HpEnabled,
                out markothP5HpValue
            );

            lastY = rowY;
            rowY += MarkothHelperRowSpacing;
            CreateToggleRow(
                content,
                "MarkothUseMaxHpRow",
                "Settings/MarkothHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.MarkothHelper.markothUseMaxHp,
                SetMarkothUseMaxHpEnabled,
                out markothUseMaxHpValue
            );

            lastY = rowY;
            rowY += MarkothHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MarkothMaxHpRow",
                "Settings/MarkothHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.MarkothHelper.markothMaxHp,
                SetMarkothMaxHp,
                1,
                999999,
                10,
                out markothMaxHpField
            );

            lastY = rowY;
            rowY += MarkothHelperRowSpacing;
            CreateToggleRow(
                content,
                "MarkothUseCustomPhaseRow",
                "Settings/MarkothHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.MarkothHelper.markothUseCustomPhase,
                SetMarkothUseCustomPhaseEnabled,
                out markothUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += MarkothHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MarkothPhase2HpRow",
                "Settings/MarkothHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.MarkothHelper.markothPhase2Hp,
                SetMarkothPhase2Hp,
                1,
                999999,
                10,
                out markothPhase2HpField
            );

            lastY = rowY;
            rowY += MarkothHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "MarkothHelperResetRow", "Settings/MarkothHelper/Reset".Localize(), resetY, OnMarkothHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "MarkothHelperBackRow", "Back", backY, OnMarkothHelperBackClicked);
        }

        private void BuildGalienHelperOverlayUi()
        {
            galienHelperRoot = new GameObject("GalienHelperOverlayCanvas");
            galienHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = galienHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = GalienHelperCanvasSortOrder;

            CanvasScaler scaler = galienHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            galienHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = galienHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(galienHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("GalienHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, GalienHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/GalienHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = GalienHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            galienHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "GalienHelperEnableRow",
                "Settings/GalienHelper/Enable".Localize(),
                rowY,
                GetGalienHelperEnabled,
                SetGalienHelperEnabled,
                out galienHelperToggleValue,
                out galienHelperToggleIcon
            );

            lastY = rowY;
            rowY += GalienHelperRowSpacing;
            CreateToggleRow(
                content,
                "GalienP5HpRow",
                "Settings/GalienHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.GalienHelper.galienP5Hp,
                SetGalienP5HpEnabled,
                out galienP5HpValue
            );

            lastY = rowY;
            rowY += GalienHelperRowSpacing;
            CreateToggleRow(
                content,
                "GalienUseMaxHpRow",
                "Settings/GalienHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.GalienHelper.galienUseMaxHp,
                SetGalienUseMaxHpEnabled,
                out galienUseMaxHpValue
            );

            lastY = rowY;
            rowY += GalienHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GalienMaxHpRow",
                "Settings/GalienHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.GalienHelper.galienMaxHp,
                SetGalienMaxHp,
                1,
                999999,
                10,
                out galienMaxHpField
            );

            lastY = rowY;
            rowY += GalienHelperRowSpacing;
            CreateToggleRow(
                content,
                "GalienUseCustomPhaseRow",
                "Settings/GalienHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.GalienHelper.galienUseCustomPhase,
                SetGalienUseCustomPhaseEnabled,
                out galienUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += GalienHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GalienPhase2HpRow",
                "Settings/GalienHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.GalienHelper.galienPhase2Hp,
                SetGalienPhase2Hp,
                2,
                999999,
                10,
                out galienPhase2HpField
            );

            lastY = rowY;
            rowY += GalienHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GalienPhase3HpRow",
                "Settings/GalienHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.GalienHelper.galienPhase3Hp,
                SetGalienPhase3Hp,
                1,
                999999,
                10,
                out galienPhase3HpField
            );

            lastY = rowY;
            rowY += GalienHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "GalienHelperResetRow", "Settings/GalienHelper/Reset".Localize(), resetY, OnGalienHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "GalienHelperBackRow", "Back", backY, OnGalienHelperBackClicked);
        }

        private void BuildGorbHelperOverlayUi()
        {
            gorbHelperRoot = new GameObject("GorbHelperOverlayCanvas");
            gorbHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = gorbHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = GorbHelperCanvasSortOrder;

            CanvasScaler scaler = gorbHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            gorbHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = gorbHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(gorbHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("GorbHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, GorbHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/GorbHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(GorbHelperPanelHeight);
            float backY = GetFixedBackY(GorbHelperPanelHeight);
            float topOffset = GetScrollTopOffset(GorbHelperPanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(GorbHelperPanelHeight, resetY);
            float viewHeight = Mathf.Max(0f, GorbHelperPanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, GorbHelperPanelHeight, topOffset, bottomOffset);
            gorbHelperContent = content;

            float rowY = GetRowStartY(GorbHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "GorbHelperEnableRow",
                "Settings/GorbHelper/Enable".Localize(),
                rowY,
                GetGorbHelperEnabled,
                SetGorbHelperEnabled,
                out gorbHelperToggleValue,
                out gorbHelperToggleIcon
            );

            lastY = rowY;
            rowY += GorbHelperRowSpacing;
            CreateToggleRow(
                content,
                "GorbP5HpRow",
                "Settings/GorbHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.GorbHelper.gorbP5Hp,
                SetGorbP5HpEnabled,
                out gorbP5HpValue
            );

            lastY = rowY;
            rowY += GorbHelperRowSpacing;
            CreateToggleRow(
                content,
                "GorbUseMaxHpRow",
                "Settings/GorbHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.GorbHelper.gorbUseMaxHp,
                SetGorbUseMaxHpEnabled,
                out gorbUseMaxHpValue
            );

            lastY = rowY;
            rowY += GorbHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GorbMaxHpRow",
                "Settings/GorbHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.GorbHelper.gorbMaxHp,
                SetGorbMaxHp,
                1,
                999999,
                10,
                out gorbMaxHpField
            );

            lastY = rowY;
            rowY += GorbHelperRowSpacing;
            CreateToggleRow(
                content,
                "GorbUseCustomPhaseRow",
                "Settings/GorbHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.GorbHelper.gorbUseCustomPhase,
                SetGorbUseCustomPhaseEnabled,
                out gorbUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += GorbHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GorbPhase2HpRow",
                "Settings/GorbHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.GorbHelper.gorbPhase2Hp,
                SetGorbPhase2Hp,
                2,
                999999,
                10,
                out gorbPhase2HpField
            );

            lastY = rowY;
            rowY += GorbHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GorbPhase3HpRow",
                "Settings/GorbHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.GorbHelper.gorbPhase3Hp,
                SetGorbPhase3Hp,
                1,
                999999,
                10,
                out gorbPhase3HpField
            );

            lastY = rowY;
            rowY += GorbHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "GorbHelperResetRow", "Settings/GorbHelper/Reset".Localize(), resetY, OnGorbHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "GorbHelperBackRow", "Back", backY, OnGorbHelperBackClicked);
        }

        private void BuildElderHuHelperOverlayUi()
        {
            BuildStandardGhostHelperOverlayUi(
                ref elderHuHelperRoot,
                ref elderHuHelperContent,
                "ElderHuHelper",
                ElderHuHelperCanvasSortOrder,
                ElderHuHelperPanelHeight,
                ElderHuHelperRowSpacing,
                GetElderHuHelperEnabled,
                SetElderHuHelperEnabled,
                () => Modules.BossChallenge.ElderHuHelper.elderHuP5Hp,
                SetElderHuP5HpEnabled,
                () => Modules.BossChallenge.ElderHuHelper.elderHuUseMaxHp,
                SetElderHuUseMaxHpEnabled,
                () => Modules.BossChallenge.ElderHuHelper.elderHuMaxHp,
                SetElderHuMaxHp,
                OnElderHuHelperResetDefaultsClicked,
                OnElderHuHelperBackClicked,
                out elderHuHelperToggleValue,
                out elderHuHelperToggleIcon,
                out elderHuP5HpValue,
                out elderHuUseMaxHpValue,
                out elderHuMaxHpField);
        }

        private void BuildNoEyesHelperOverlayUi()
        {
            noEyesHelperRoot = new GameObject("NoEyesHelperOverlayCanvas");
            noEyesHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = noEyesHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = NoEyesHelperCanvasSortOrder;

            CanvasScaler scaler = noEyesHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            noEyesHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = noEyesHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(noEyesHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("NoEyesHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, NoEyesHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/NoEyesHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = NoEyesHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            noEyesHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "NoEyesHelperEnableRow",
                "Settings/NoEyesHelper/Enable".Localize(),
                rowY,
                GetNoEyesHelperEnabled,
                SetNoEyesHelperEnabled,
                out noEyesHelperToggleValue,
                out noEyesHelperToggleIcon
            );

            lastY = rowY;
            rowY += NoEyesHelperRowSpacing;
            CreateToggleRow(
                content,
                "NoEyesP5HpRow",
                "Settings/NoEyesHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NoEyesHelper.noEyesP5Hp,
                SetNoEyesP5HpEnabled,
                out noEyesP5HpValue
            );

            lastY = rowY;
            rowY += NoEyesHelperRowSpacing;
            CreateToggleRow(
                content,
                "NoEyesUseMaxHpRow",
                "Settings/NoEyesHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.NoEyesHelper.noEyesUseMaxHp,
                SetNoEyesUseMaxHpEnabled,
                out noEyesUseMaxHpValue
            );

            lastY = rowY;
            rowY += NoEyesHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoEyesMaxHpRow",
                "Settings/NoEyesHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.NoEyesHelper.noEyesMaxHp,
                SetNoEyesMaxHp,
                1,
                999999,
                10,
                out noEyesMaxHpField
            );

            lastY = rowY;
            rowY += NoEyesHelperRowSpacing;
            CreateToggleRow(
                content,
                "NoEyesUseCustomPhaseRow",
                "Settings/NoEyesHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase,
                SetNoEyesUseCustomPhaseEnabled,
                out noEyesUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += NoEyesHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoEyesPhase2HpRow",
                "Settings/NoEyesHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp,
                SetNoEyesPhase2Hp,
                2,
                999999,
                10,
                out noEyesPhase2HpField
            );

            lastY = rowY;
            rowY += NoEyesHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoEyesPhase3HpRow",
                "Settings/NoEyesHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp,
                SetNoEyesPhase3Hp,
                1,
                999999,
                10,
                out noEyesPhase3HpField
            );

            lastY = rowY;
            rowY += NoEyesHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "NoEyesHelperResetRow", "Settings/NoEyesHelper/Reset".Localize(), resetY, OnNoEyesHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "NoEyesHelperBackRow", "Back", backY, OnNoEyesHelperBackClicked);
        }

        private void BuildDungDefenderHelperOverlayUi()
        {
            dungDefenderHelperRoot = new GameObject("DungDefenderHelperOverlayCanvas");
            dungDefenderHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = dungDefenderHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = DungDefenderHelperCanvasSortOrder;

            CanvasScaler scaler = dungDefenderHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            dungDefenderHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = dungDefenderHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(dungDefenderHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("DungDefenderHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, DungDefenderHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/DungDefenderHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = DungDefenderHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            dungDefenderHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "DungDefenderHelperEnableRow",
                "Settings/DungDefenderHelper/Enable".Localize(),
                rowY,
                GetDungDefenderHelperEnabled,
                SetDungDefenderHelperEnabled,
                out dungDefenderHelperToggleValue,
                out dungDefenderHelperToggleIcon
            );

            lastY = rowY;
            rowY += DungDefenderHelperRowSpacing;
            CreateToggleRow(
                content,
                "DungDefenderP5HpRow",
                "Settings/DungDefenderHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.DungDefenderHelper.dungDefenderP5Hp,
                SetDungDefenderP5HpEnabled,
                out dungDefenderP5HpValue
            );

            lastY = rowY;
            rowY += DungDefenderHelperRowSpacing;
            CreateToggleRow(
                content,
                "DungDefenderUseMaxHpRow",
                "Settings/DungDefenderHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.DungDefenderHelper.dungDefenderUseMaxHp,
                SetDungDefenderUseMaxHpEnabled,
                out dungDefenderUseMaxHpValue
            );

            lastY = rowY;
            rowY += DungDefenderHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "DungDefenderMaxHpRow",
                "Settings/DungDefenderHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.DungDefenderHelper.dungDefenderMaxHp,
                SetDungDefenderMaxHp,
                1,
                999999,
                10,
                out dungDefenderMaxHpField
            );

            lastY = rowY;
            rowY += DungDefenderHelperRowSpacing;
            CreateToggleRow(
                content,
                "DungDefenderUseCustomPhaseRow",
                "Settings/DungDefenderHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhase,
                SetDungDefenderUseCustomPhaseEnabled,
                out dungDefenderUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += DungDefenderHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "DungDefenderPhase2HpRow",
                "Settings/DungDefenderHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2Hp,
                SetDungDefenderPhase2Hp,
                1,
                999999,
                10,
                out dungDefenderPhase2HpField
            );

            lastY = rowY;
            rowY += DungDefenderHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "DungDefenderHelperResetRow", "Settings/DungDefenderHelper/Reset".Localize(), resetY, OnDungDefenderHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "DungDefenderHelperBackRow", "Back", backY, OnDungDefenderHelperBackClicked);
        }

        private void BuildWhiteDefenderHelperOverlayUi()
        {
            whiteDefenderHelperRoot = new GameObject("WhiteDefenderHelperOverlayCanvas");
            whiteDefenderHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = whiteDefenderHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = WhiteDefenderHelperCanvasSortOrder;

            CanvasScaler scaler = whiteDefenderHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            whiteDefenderHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = whiteDefenderHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(whiteDefenderHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("WhiteDefenderHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, WhiteDefenderHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/WhiteDefenderHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = WhiteDefenderHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            whiteDefenderHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "WhiteDefenderHelperEnableRow",
                "Settings/WhiteDefenderHelper/Enable".Localize(),
                rowY,
                GetWhiteDefenderHelperEnabled,
                SetWhiteDefenderHelperEnabled,
                out whiteDefenderHelperToggleValue,
                out whiteDefenderHelperToggleIcon
            );

            lastY = rowY;
            rowY += WhiteDefenderHelperRowSpacing;
            CreateToggleRow(
                content,
                "WhiteDefenderP5HpRow",
                "Settings/WhiteDefenderHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderP5Hp,
                SetWhiteDefenderP5HpEnabled,
                out whiteDefenderP5HpValue
            );

            lastY = rowY;
            rowY += WhiteDefenderHelperRowSpacing;
            CreateToggleRow(
                content,
                "WhiteDefenderUseMaxHpRow",
                "Settings/WhiteDefenderHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseMaxHp,
                SetWhiteDefenderUseMaxHpEnabled,
                out whiteDefenderUseMaxHpValue
            );

            lastY = rowY;
            rowY += WhiteDefenderHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WhiteDefenderMaxHpRow",
                "Settings/WhiteDefenderHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderMaxHp,
                SetWhiteDefenderMaxHp,
                1,
                999999,
                10,
                out whiteDefenderMaxHpField
            );

            lastY = rowY;
            rowY += WhiteDefenderHelperRowSpacing;
            CreateToggleRow(
                content,
                "WhiteDefenderUseCustomPhaseRow",
                "Settings/WhiteDefenderHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseCustomPhase,
                SetWhiteDefenderUseCustomPhaseEnabled,
                out whiteDefenderUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += WhiteDefenderHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WhiteDefenderPhase2HpRow",
                "Settings/WhiteDefenderHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderPhase2Hp,
                SetWhiteDefenderPhase2Hp,
                1,
                999999,
                10,
                out whiteDefenderPhase2HpField
            );

            lastY = rowY;
            rowY += WhiteDefenderHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "WhiteDefenderHelperResetRow", "Settings/WhiteDefenderHelper/Reset".Localize(), resetY, OnWhiteDefenderHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "WhiteDefenderHelperBackRow", "Back", backY, OnWhiteDefenderHelperBackClicked);
        }

        private void BuildHiveKnightHelperOverlayUi()
        {
            hiveKnightHelperRoot = new GameObject("HiveKnightHelperOverlayCanvas");
            hiveKnightHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = hiveKnightHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = HiveKnightHelperCanvasSortOrder;

            CanvasScaler scaler = hiveKnightHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            hiveKnightHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = hiveKnightHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(hiveKnightHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("HiveKnightHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, HiveKnightHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/HiveKnightHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = HiveKnightHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            hiveKnightHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "HiveKnightHelperEnableRow",
                "Settings/HiveKnightHelper/Enable".Localize(),
                rowY,
                GetHiveKnightHelperEnabled,
                SetHiveKnightHelperEnabled,
                out hiveKnightHelperToggleValue,
                out hiveKnightHelperToggleIcon
            );

            lastY = rowY;
            rowY += HiveKnightHelperRowSpacing;
            CreateToggleRow(
                content,
                "HiveKnightP5HpRow",
                "Settings/HiveKnightHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp,
                SetHiveKnightP5HpEnabled,
                out hiveKnightP5HpValue
            );

            lastY = rowY;
            rowY += HiveKnightHelperRowSpacing;
            CreateToggleRow(
                content,
                "HiveKnightUseMaxHpRow",
                "Settings/HiveKnightHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.HiveKnightHelper.hiveKnightUseMaxHp,
                SetHiveKnightUseMaxHpEnabled,
                out hiveKnightUseMaxHpValue
            );

            lastY = rowY;
            rowY += HiveKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "HiveKnightMaxHpRow",
                "Settings/HiveKnightHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.HiveKnightHelper.hiveKnightMaxHp,
                SetHiveKnightMaxHp,
                1,
                999999,
                10,
                out hiveKnightMaxHpField
            );

            lastY = rowY;
            rowY += HiveKnightHelperRowSpacing;
            CreateToggleRow(
                content,
                "HiveKnightUseCustomPhaseRow",
                "Settings/HiveKnightHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase,
                SetHiveKnightUseCustomPhaseEnabled,
                out hiveKnightUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += HiveKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "HiveKnightPhase2HpRow",
                "Settings/HiveKnightHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase2Hp,
                SetHiveKnightPhase2Hp,
                2,
                1300,
                10,
                out hiveKnightPhase2HpField
            );

            lastY = rowY;
            rowY += HiveKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "HiveKnightPhase3HpRow",
                "Settings/HiveKnightHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase3Hp,
                SetHiveKnightPhase3Hp,
                1,
                1299,
                10,
                out hiveKnightPhase3HpField
            );

            lastY = rowY;
            rowY += HiveKnightHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "HiveKnightHelperResetRow", "Settings/HiveKnightHelper/Reset".Localize(), resetY, OnHiveKnightHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "HiveKnightHelperBackRow", "Back", backY, OnHiveKnightHelperBackClicked);
        }

        private void BuildBrokenVesselHelperOverlayUi()
        {
            brokenVesselHelperRoot = new GameObject("BrokenVesselHelperOverlayCanvas");
            brokenVesselHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = brokenVesselHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = BrokenVesselHelperCanvasSortOrder;

            CanvasScaler scaler = brokenVesselHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            brokenVesselHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = brokenVesselHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(brokenVesselHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("BrokenVesselHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, BrokenVesselHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/BrokenVesselHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = BrokenVesselHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            brokenVesselHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "BrokenVesselHelperEnableRow",
                "Settings/BrokenVesselHelper/Enable".Localize(),
                rowY,
                GetBrokenVesselHelperEnabled,
                SetBrokenVesselHelperEnabled,
                out brokenVesselHelperToggleValue,
                out brokenVesselHelperToggleIcon
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateToggleRow(
                content,
                "BrokenVesselP5HpRow",
                "Settings/BrokenVesselHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp,
                SetBrokenVesselP5HpEnabled,
                out brokenVesselP5HpValue
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateToggleRow(
                content,
                "BrokenVesselUseMaxHpRow",
                "Settings/BrokenVesselHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseMaxHp,
                SetBrokenVesselUseMaxHpEnabled,
                out brokenVesselUseMaxHpValue
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "BrokenVesselMaxHpRow",
                "Settings/BrokenVesselHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselMaxHp,
                SetBrokenVesselMaxHp,
                1,
                999999,
                10,
                out brokenVesselMaxHpField
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateToggleRow(
                content,
                "BrokenVesselUseCustomPhaseRow",
                "Settings/BrokenVesselHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase,
                SetBrokenVesselUseCustomPhaseEnabled,
                out brokenVesselUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "BrokenVesselPhase2HpRow",
                "Settings/BrokenVesselHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp,
                SetBrokenVesselPhase2Hp,
                4,
                999999,
                10,
                out brokenVesselPhase2HpField
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "BrokenVesselPhase3HpRow",
                "Settings/BrokenVesselHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp,
                SetBrokenVesselPhase3Hp,
                3,
                999998,
                10,
                out brokenVesselPhase3HpField
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "BrokenVesselPhase4HpRow",
                "Settings/BrokenVesselHelper/Phase4HP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp,
                SetBrokenVesselPhase4Hp,
                2,
                999997,
                10,
                out brokenVesselPhase4HpField
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "BrokenVesselPhase5HpRow",
                "Settings/BrokenVesselHelper/Phase5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp,
                SetBrokenVesselPhase5Hp,
                1,
                999996,
                10,
                out brokenVesselPhase5HpField
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateToggleRow(
                content,
                "BrokenVesselUseCustomSummonHpRow",
                "Settings/BrokenVesselHelper/UseCustomSummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonHp,
                SetBrokenVesselUseCustomSummonHpEnabled,
                out brokenVesselUseCustomSummonHpValue
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "BrokenVesselSummonHpRow",
                "Settings/BrokenVesselHelper/SummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonHp,
                SetBrokenVesselSummonHp,
                1,
                999999,
                10,
                out brokenVesselSummonHpField
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateToggleRow(
                content,
                "BrokenVesselUseCustomSummonLimitRow",
                "Settings/BrokenVesselHelper/UseCustomSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonLimit,
                SetBrokenVesselUseCustomSummonLimitEnabled,
                out brokenVesselUseCustomSummonLimitValue
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "BrokenVesselSummonLimitRow",
                "Settings/BrokenVesselHelper/SummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonLimit,
                SetBrokenVesselSummonLimit,
                0,
                999,
                1,
                out brokenVesselSummonLimitField
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "BrokenVesselHelperResetRow", "Settings/BrokenVesselHelper/Reset".Localize(), resetY, OnBrokenVesselHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "BrokenVesselHelperBackRow", "Back", backY, OnBrokenVesselHelperBackClicked);
        }

        private void BuildLostKinHelperOverlayUi()
        {
            lostKinHelperRoot = new GameObject("LostKinHelperOverlayCanvas");
            lostKinHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = lostKinHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = LostKinHelperCanvasSortOrder;

            CanvasScaler scaler = lostKinHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            lostKinHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = lostKinHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(lostKinHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("LostKinHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, LostKinHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/LostKinHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = LostKinHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            lostKinHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "LostKinHelperEnableRow",
                "Settings/LostKinHelper/Enable".Localize(),
                rowY,
                GetLostKinHelperEnabled,
                SetLostKinHelperEnabled,
                out lostKinHelperToggleValue,
                out lostKinHelperToggleIcon
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateToggleRow(
                content,
                "LostKinP5HpRow",
                "Settings/LostKinHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinP5Hp,
                SetLostKinP5HpEnabled,
                out lostKinP5HpValue
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateToggleRow(
                content,
                "LostKinUseMaxHpRow",
                "Settings/LostKinHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinUseMaxHp,
                SetLostKinUseMaxHpEnabled,
                out lostKinUseMaxHpValue
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "LostKinMaxHpRow",
                "Settings/LostKinHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinMaxHp,
                SetLostKinMaxHp,
                1,
                999999,
                10,
                out lostKinMaxHpField
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateToggleRow(
                content,
                "LostKinUseCustomPhaseRow",
                "Settings/LostKinHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase,
                SetLostKinUseCustomPhaseEnabled,
                out lostKinUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "LostKinPhase2HpRow",
                "Settings/LostKinHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp,
                SetLostKinPhase2Hp,
                4,
                999999,
                10,
                out lostKinPhase2HpField
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "LostKinPhase3HpRow",
                "Settings/LostKinHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp,
                SetLostKinPhase3Hp,
                3,
                999998,
                10,
                out lostKinPhase3HpField
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "LostKinPhase4HpRow",
                "Settings/LostKinHelper/Phase4HP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp,
                SetLostKinPhase4Hp,
                2,
                999997,
                10,
                out lostKinPhase4HpField
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "LostKinPhase5HpRow",
                "Settings/LostKinHelper/Phase5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp,
                SetLostKinPhase5Hp,
                1,
                999996,
                10,
                out lostKinPhase5HpField
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateToggleRow(
                content,
                "LostKinUseCustomSummonHpRow",
                "Settings/LostKinHelper/UseCustomSummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonHp,
                SetLostKinUseCustomSummonHpEnabled,
                out lostKinUseCustomSummonHpValue
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "LostKinSummonHpRow",
                "Settings/LostKinHelper/SummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinSummonHp,
                SetLostKinSummonHp,
                1,
                999999,
                10,
                out lostKinSummonHpField
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateToggleRow(
                content,
                "LostKinUseCustomSummonLimitRow",
                "Settings/LostKinHelper/UseCustomSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonLimit,
                SetLostKinUseCustomSummonLimitEnabled,
                out lostKinUseCustomSummonLimitValue
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "LostKinSummonLimitRow",
                "Settings/LostKinHelper/SummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinSummonLimit,
                SetLostKinSummonLimit,
                0,
                999,
                1,
                out lostKinSummonLimitField
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "LostKinHelperResetRow", "Settings/LostKinHelper/Reset".Localize(), resetY, OnLostKinHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "LostKinHelperBackRow", "Back", backY, OnLostKinHelperBackClicked);
        }

        private void BuildWingedNoskHelperOverlayUi()
        {
            wingedNoskHelperRoot = new GameObject("WingedNoskHelperOverlayCanvas");
            wingedNoskHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = wingedNoskHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = WingedNoskHelperCanvasSortOrder;

            CanvasScaler scaler = wingedNoskHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            wingedNoskHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = wingedNoskHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(wingedNoskHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("WingedNoskHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, WingedNoskHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/WingedNoskHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = WingedNoskHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            wingedNoskHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "WingedNoskHelperEnableRow",
                "Settings/WingedNoskHelper/Enable".Localize(),
                rowY,
                GetWingedNoskHelperEnabled,
                SetWingedNoskHelperEnabled,
                out wingedNoskHelperToggleValue,
                out wingedNoskHelperToggleIcon
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateToggleRow(
                content,
                "WingedNoskP5HpRow",
                "Settings/WingedNoskHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp,
                SetWingedNoskP5HpEnabled,
                out wingedNoskP5HpValue
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateToggleRow(
                content,
                "WingedNoskUseMaxHpRow",
                "Settings/WingedNoskHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskUseMaxHp,
                SetWingedNoskUseMaxHpEnabled,
                out wingedNoskUseMaxHpValue
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WingedNoskMaxHpRow",
                "Settings/WingedNoskHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskMaxHp,
                SetWingedNoskMaxHp,
                1,
                999999,
                10,
                out wingedNoskMaxHpField
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateToggleRow(
                content,
                "WingedNoskUseCustomPhaseRow",
                "Settings/WingedNoskHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomPhase,
                SetWingedNoskUseCustomPhaseEnabled,
                out wingedNoskUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WingedNoskPhase2HpRow",
                "Settings/WingedNoskHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2Hp,
                SetWingedNoskPhase2Hp,
                1,
                999999,
                10,
                out wingedNoskPhase2HpField
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateToggleRow(
                content,
                "WingedNoskUseCustomSummonHpRow",
                "Settings/WingedNoskHelper/UseCustomSummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonHp,
                SetWingedNoskUseCustomSummonHpEnabled,
                out wingedNoskUseCustomSummonHpValue
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WingedNoskSummonHpRow",
                "Settings/WingedNoskHelper/SummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonHp,
                SetWingedNoskSummonHp,
                1,
                999999,
                1,
                out wingedNoskSummonHpField
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateToggleRow(
                content,
                "WingedNoskUseCustomSummonLimitRow",
                "Settings/WingedNoskHelper/UseCustomSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonLimit,
                SetWingedNoskUseCustomSummonLimitEnabled,
                out wingedNoskUseCustomSummonLimitValue
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WingedNoskSummonLimitRow",
                "Settings/WingedNoskHelper/SummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonLimit,
                SetWingedNoskSummonLimit,
                0,
                999,
                1,
                out wingedNoskSummonLimitField
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "WingedNoskHelperResetRow", "Settings/WingedNoskHelper/Reset".Localize(), resetY, OnWingedNoskHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "WingedNoskHelperBackRow", "Back", backY, OnWingedNoskHelperBackClicked);
        }

        private void BuildUumuuHelperOverlayUi()
        {
            uumuuHelperRoot = new GameObject("UumuuHelperOverlayCanvas");
            uumuuHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = uumuuHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = UumuuHelperCanvasSortOrder;

            CanvasScaler scaler = uumuuHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            uumuuHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = uumuuHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(uumuuHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("UumuuHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, UumuuHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/UumuuHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = UumuuHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            uumuuHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "UumuuHelperEnableRow",
                "Settings/UumuuHelper/Enable".Localize(),
                rowY,
                GetUumuuHelperEnabled,
                SetUumuuHelperEnabled,
                out uumuuHelperToggleValue,
                out uumuuHelperToggleIcon
            );

            lastY = rowY;
            rowY += UumuuHelperRowSpacing;
            CreateToggleRow(
                content,
                "UumuuP5HpRow",
                "Settings/UumuuHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.UumuuHelper.uumuuP5Hp,
                SetUumuuP5HpEnabled,
                out uumuuP5HpValue
            );

            lastY = rowY;
            rowY += UumuuHelperRowSpacing;
            CreateToggleRow(
                content,
                "UumuuUseMaxHpRow",
                "Settings/UumuuHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.UumuuHelper.uumuuUseMaxHp,
                SetUumuuUseMaxHpEnabled,
                out uumuuUseMaxHpValue
            );

            lastY = rowY;
            rowY += UumuuHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "UumuuMaxHpRow",
                "Settings/UumuuHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.UumuuHelper.uumuuMaxHp,
                SetUumuuMaxHp,
                1,
                999999,
                10,
                out uumuuMaxHpField
            );

            lastY = rowY;
            rowY += UumuuHelperRowSpacing;
            CreateToggleRow(
                content,
                "UumuuUseCustomSummonHpRow",
                "Settings/UumuuHelper/UseCustomSummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.UumuuHelper.uumuuUseCustomSummonHp,
                SetUumuuUseCustomSummonHpEnabled,
                out uumuuUseCustomSummonHpValue
            );

            lastY = rowY;
            rowY += UumuuHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "UumuuSummonHpRow",
                "Settings/UumuuHelper/SummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.UumuuHelper.uumuuSummonHp,
                SetUumuuSummonHp,
                1,
                999999,
                1,
                out uumuuSummonHpField
            );

            lastY = rowY;
            rowY += UumuuHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "UumuuHelperResetRow", "Settings/UumuuHelper/Reset".Localize(), resetY, OnUumuuHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "UumuuHelperBackRow", "Back", backY, OnUumuuHelperBackClicked);
        }

        private void BuildNoskHelperOverlayUi()
        {
            noskHelperRoot = new GameObject("NoskHelperOverlayCanvas");
            noskHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = noskHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = NoskHelperCanvasSortOrder;

            CanvasScaler scaler = noskHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            noskHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = noskHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(noskHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("NoskHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, NoskHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/NoskHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = NoskHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            noskHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "NoskHelperEnableRow",
                "Settings/NoskHelper/Enable".Localize(),
                rowY,
                GetNoskHelperEnabled,
                SetNoskHelperEnabled,
                out noskHelperToggleValue,
                out noskHelperToggleIcon
            );

            lastY = rowY;
            rowY += NoskHelperRowSpacing;
            CreateToggleRow(
                content,
                "NoskUseMaxHpRow",
                "Settings/NoskHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.NoskHelper.noskUseMaxHp,
                SetNoskUseMaxHpEnabled,
                out noskUseMaxHpValue
            );

            lastY = rowY;
            rowY += NoskHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoskMaxHpRow",
                "Settings/NoskHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.NoskHelper.noskMaxHp,
                SetNoskMaxHp,
                1,
                999999,
                10,
                out noskMaxHpField
            );

            lastY = rowY;
            rowY += NoskHelperRowSpacing;
            CreateToggleRow(
                content,
                "NoskUseCustomPhaseRow",
                "Settings/NoskHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.NoskHelper.noskUseCustomPhase,
                SetNoskUseCustomPhaseEnabled,
                out noskUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += NoskHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoskPhase2HpRow",
                "Settings/NoskHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NoskHelper.noskPhase2Hp,
                SetNoskPhase2Hp,
                1,
                999999,
                10,
                out noskPhase2HpField
            );

            lastY = rowY;
            rowY += NoskHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "NoskHelperResetRow", "Settings/NoskHelper/Reset".Localize(), resetY, OnNoskHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "NoskHelperBackRow", "Back", backY, OnNoskHelperBackClicked);
        }

        private void BuildTraitorLordHelperOverlayUi()
        {
            traitorLordHelperRoot = new GameObject("TraitorLordHelperOverlayCanvas");
            traitorLordHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = traitorLordHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = TraitorLordHelperCanvasSortOrder;

            CanvasScaler scaler = traitorLordHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            traitorLordHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = traitorLordHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(traitorLordHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("TraitorLordHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, TraitorLordHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/TraitorLordHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = TraitorLordHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            traitorLordHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "TraitorLordHelperEnableRow",
                "Settings/TraitorLordHelper/Enable".Localize(),
                rowY,
                GetTraitorLordHelperEnabled,
                SetTraitorLordHelperEnabled,
                out traitorLordHelperToggleValue,
                out traitorLordHelperToggleIcon
            );

            lastY = rowY;
            rowY += TraitorLordHelperRowSpacing;
            CreateToggleRow(
                content,
                "TraitorLordP5HpRow",
                "Settings/TraitorLordHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.TraitorLordHelper.traitorLordP5Hp,
                SetTraitorLordP5HpEnabled,
                out traitorLordP5HpValue
            );

            lastY = rowY;
            rowY += TraitorLordHelperRowSpacing;
            CreateToggleRow(
                content,
                "TraitorLordUseMaxHpRow",
                "Settings/TraitorLordHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.TraitorLordHelper.traitorLordUseMaxHp,
                SetTraitorLordUseMaxHpEnabled,
                out traitorLordUseMaxHpValue
            );

            lastY = rowY;
            rowY += TraitorLordHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "TraitorLordMaxHpRow",
                "Settings/TraitorLordHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.TraitorLordHelper.traitorLordMaxHp,
                SetTraitorLordMaxHp,
                1,
                999999,
                10,
                out traitorLordMaxHpField
            );

            lastY = rowY;
            rowY += TraitorLordHelperRowSpacing;
            CreateToggleRow(
                content,
                "TraitorLordUseCustomPhaseRow",
                "Settings/TraitorLordHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhase,
                SetTraitorLordUseCustomPhaseEnabled,
                out traitorLordUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += TraitorLordHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "TraitorLordPhase2HpRow",
                "Settings/TraitorLordHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp,
                SetTraitorLordPhase2Hp,
                1,
                999999,
                10,
                out traitorLordPhase2HpField
            );

            lastY = rowY;
            rowY += TraitorLordHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "TraitorLordHelperResetRow", "Settings/TraitorLordHelper/Reset".Localize(), resetY, OnTraitorLordHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "TraitorLordHelperBackRow", "Back", backY, OnTraitorLordHelperBackClicked);
        }

        private void BuildTroupeMasterGrimmHelperOverlayUi()
        {
            troupeMasterGrimmHelperRoot = new GameObject("TroupeMasterGrimmHelperOverlayCanvas");
            troupeMasterGrimmHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = troupeMasterGrimmHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = TroupeMasterGrimmHelperCanvasSortOrder;

            CanvasScaler scaler = troupeMasterGrimmHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            troupeMasterGrimmHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = troupeMasterGrimmHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(troupeMasterGrimmHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("TroupeMasterGrimmHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, TroupeMasterGrimmHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/TroupeMasterGrimmHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = TroupeMasterGrimmHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            troupeMasterGrimmHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "TroupeMasterGrimmHelperEnableRow",
                "Settings/TroupeMasterGrimmHelper/Enable".Localize(),
                rowY,
                GetTroupeMasterGrimmHelperEnabled,
                SetTroupeMasterGrimmHelperEnabled,
                out troupeMasterGrimmHelperToggleValue,
                out troupeMasterGrimmHelperToggleIcon
            );

            lastY = rowY;
            rowY += TroupeMasterGrimmHelperRowSpacing;
            CreateToggleRow(
                content,
                "TroupeMasterGrimmP5HpRow",
                "Settings/TroupeMasterGrimmHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp,
                SetTroupeMasterGrimmP5HpEnabled,
                out troupeMasterGrimmP5HpValue
            );

            lastY = rowY;
            rowY += TroupeMasterGrimmHelperRowSpacing;
            CreateToggleRow(
                content,
                "TroupeMasterGrimmUseMaxHpRow",
                "Settings/TroupeMasterGrimmHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseMaxHp,
                SetTroupeMasterGrimmUseMaxHpEnabled,
                out troupeMasterGrimmUseMaxHpValue
            );

            lastY = rowY;
            rowY += TroupeMasterGrimmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "TroupeMasterGrimmMaxHpRow",
                "Settings/TroupeMasterGrimmHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmMaxHp,
                SetTroupeMasterGrimmMaxHp,
                1,
                999999,
                10,
                out troupeMasterGrimmMaxHpField
            );

            lastY = rowY;
            rowY += TroupeMasterGrimmHelperRowSpacing;
            CreateToggleRow(
                content,
                "TroupeMasterGrimmUseCustomPhaseRow",
                "Settings/TroupeMasterGrimmHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase,
                SetTroupeMasterGrimmUseCustomPhaseEnabled,
                out troupeMasterGrimmUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += TroupeMasterGrimmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "TroupeMasterGrimmPhase2HpRow",
                "Settings/TroupeMasterGrimmHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp,
                SetTroupeMasterGrimmPhase2Hp,
                1,
                999999,
                10,
                out troupeMasterGrimmPhase2HpField
            );

            lastY = rowY;
            rowY += TroupeMasterGrimmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "TroupeMasterGrimmPhase3HpRow",
                "Settings/TroupeMasterGrimmHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp,
                SetTroupeMasterGrimmPhase3Hp,
                1,
                999998,
                10,
                out troupeMasterGrimmPhase3HpField
            );

            lastY = rowY;
            rowY += TroupeMasterGrimmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "TroupeMasterGrimmPhase4HpRow",
                "Settings/TroupeMasterGrimmHelper/Phase4HP".Localize(),
                rowY,
                () => Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp,
                SetTroupeMasterGrimmPhase4Hp,
                1,
                999997,
                10,
                out troupeMasterGrimmPhase4HpField
            );

            lastY = rowY;
            rowY += TroupeMasterGrimmHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "TroupeMasterGrimmHelperResetRow", "Settings/TroupeMasterGrimmHelper/Reset".Localize(), resetY, OnTroupeMasterGrimmHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "TroupeMasterGrimmHelperBackRow", "Back", backY, OnTroupeMasterGrimmHelperBackClicked);
        }

        private void BuildNightmareKingGrimmHelperOverlayUi()
        {
            nightmareKingGrimmHelperRoot = new GameObject("NightmareKingGrimmHelperOverlayCanvas");
            nightmareKingGrimmHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = nightmareKingGrimmHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = NightmareKingGrimmHelperCanvasSortOrder;

            CanvasScaler scaler = nightmareKingGrimmHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            nightmareKingGrimmHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = nightmareKingGrimmHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(nightmareKingGrimmHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("NightmareKingGrimmHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, NightmareKingGrimmHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/NightmareKingGrimmHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = NightmareKingGrimmHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            nightmareKingGrimmHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "NightmareKingGrimmHelperEnableRow",
                "Settings/NightmareKingGrimmHelper/Enable".Localize(),
                rowY,
                GetNightmareKingGrimmHelperEnabled,
                SetNightmareKingGrimmHelperEnabled,
                out nightmareKingGrimmHelperToggleValue,
                out nightmareKingGrimmHelperToggleIcon
            );

            lastY = rowY;
            rowY += NightmareKingGrimmHelperRowSpacing;
            CreateToggleRow(
                content,
                "NightmareKingGrimmP5HpRow",
                "Settings/NightmareKingGrimmHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp,
                SetNightmareKingGrimmP5HpEnabled,
                out nightmareKingGrimmP5HpValue
            );

            lastY = rowY;
            rowY += NightmareKingGrimmHelperRowSpacing;
            CreateToggleRow(
                content,
                "NightmareKingGrimmUseMaxHpRow",
                "Settings/NightmareKingGrimmHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseMaxHp,
                SetNightmareKingGrimmUseMaxHpEnabled,
                out nightmareKingGrimmUseMaxHpValue
            );

            lastY = rowY;
            rowY += NightmareKingGrimmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NightmareKingGrimmMaxHpRow",
                "Settings/NightmareKingGrimmHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmMaxHp,
                SetNightmareKingGrimmMaxHp,
                1,
                999999,
                10,
                out nightmareKingGrimmMaxHpField
            );

            lastY = rowY;
            rowY += NightmareKingGrimmHelperRowSpacing;
            CreateToggleRow(
                content,
                "NightmareKingGrimmUseCustomPhaseRow",
                "Settings/NightmareKingGrimmHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase,
                SetNightmareKingGrimmUseCustomPhaseEnabled,
                out nightmareKingGrimmUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += NightmareKingGrimmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NightmareKingGrimmRagePhase1HpRow",
                "Settings/NightmareKingGrimmHelper/RagePhase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1Hp,
                SetNightmareKingGrimmRagePhase1Hp,
                3,
                1650,
                10,
                out nightmareKingGrimmRagePhase1HpField
            );

            lastY = rowY;
            rowY += NightmareKingGrimmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NightmareKingGrimmRagePhase2HpRow",
                "Settings/NightmareKingGrimmHelper/RagePhase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2Hp,
                SetNightmareKingGrimmRagePhase2Hp,
                2,
                1649,
                10,
                out nightmareKingGrimmRagePhase2HpField
            );

            lastY = rowY;
            rowY += NightmareKingGrimmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NightmareKingGrimmRagePhase3HpRow",
                "Settings/NightmareKingGrimmHelper/RagePhase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3Hp,
                SetNightmareKingGrimmRagePhase3Hp,
                1,
                1648,
                10,
                out nightmareKingGrimmRagePhase3HpField
            );

            lastY = rowY;
            rowY += NightmareKingGrimmHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "NightmareKingGrimmHelperResetRow", "Settings/NightmareKingGrimmHelper/Reset".Localize(), resetY, OnNightmareKingGrimmHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "NightmareKingGrimmHelperBackRow", "Back", backY, OnNightmareKingGrimmHelperBackClicked);
        }

        private void BuildPureVesselHelperOverlayUi()
        {
            pureVesselHelperRoot = new GameObject("PureVesselHelperOverlayCanvas");
            pureVesselHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = pureVesselHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = PureVesselHelperCanvasSortOrder;

            CanvasScaler scaler = pureVesselHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            pureVesselHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = pureVesselHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(pureVesselHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("PureVesselHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PureVesselHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/PureVesselHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = PureVesselHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            pureVesselHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "PureVesselHelperEnableRow",
                "Settings/PureVesselHelper/Enable".Localize(),
                rowY,
                GetPureVesselHelperEnabled,
                SetPureVesselHelperEnabled,
                out pureVesselHelperToggleValue,
                out pureVesselHelperToggleIcon
            );

            lastY = rowY;
            rowY += PureVesselHelperRowSpacing;
            CreateToggleRow(
                content,
                "PureVesselP5HpRow",
                "Settings/PureVesselHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp,
                SetPureVesselP5HpEnabled,
                out pureVesselP5HpValue
            );

            lastY = rowY;
            rowY += PureVesselHelperRowSpacing;
            CreateToggleRow(
                content,
                "PureVesselUseMaxHpRow",
                "Settings/PureVesselHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.PureVesselHelper.pureVesselUseMaxHp,
                SetPureVesselUseMaxHpEnabled,
                out pureVesselUseMaxHpValue
            );

            lastY = rowY;
            rowY += PureVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "PureVesselMaxHpRow",
                "Settings/PureVesselHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.PureVesselHelper.pureVesselMaxHp,
                SetPureVesselMaxHp,
                1,
                999999,
                10,
                out pureVesselMaxHpField
            );

            lastY = rowY;
            rowY += PureVesselHelperRowSpacing;
            CreateToggleRow(
                content,
                "PureVesselUseCustomPhaseRow",
                "Settings/PureVesselHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase,
                SetPureVesselUseCustomPhaseEnabled,
                out pureVesselUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += PureVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "PureVesselPhase2HpRow",
                "Settings/PureVesselHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.PureVesselHelper.pureVesselPhase2Hp,
                SetPureVesselPhase2Hp,
                2,
                1850,
                10,
                out pureVesselPhase2HpField
            );

            lastY = rowY;
            rowY += PureVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "PureVesselPhase3HpRow",
                "Settings/PureVesselHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.PureVesselHelper.pureVesselPhase3Hp,
                SetPureVesselPhase3Hp,
                1,
                1849,
                10,
                out pureVesselPhase3HpField
            );

            lastY = rowY;
            rowY += PureVesselHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "PureVesselHelperResetRow", "Settings/PureVesselHelper/Reset".Localize(), resetY, OnPureVesselHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "PureVesselHelperBackRow", "Back", backY, OnPureVesselHelperBackClicked);
        }

        private void BuildAbsoluteRadianceHelperOverlayUi()
        {
            absoluteRadianceHelperRoot = new GameObject("AbsoluteRadianceHelperOverlayCanvas");
            absoluteRadianceHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = absoluteRadianceHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = AbsoluteRadianceHelperCanvasSortOrder;

            CanvasScaler scaler = absoluteRadianceHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            absoluteRadianceHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = absoluteRadianceHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(absoluteRadianceHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("AbsoluteRadianceHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, AbsoluteRadianceHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/AbsoluteRadianceHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = AbsoluteRadianceHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            absoluteRadianceHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "AbsoluteRadianceHelperEnableRow",
                "Settings/AbsoluteRadianceHelper/Enable".Localize(),
                rowY,
                GetAbsoluteRadianceHelperEnabled,
                SetAbsoluteRadianceHelperEnabled,
                out absoluteRadianceHelperToggleValue,
                out absoluteRadianceHelperToggleIcon
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateToggleRow(
                content,
                "AbsoluteRadianceP5HpRow",
                "Settings/AbsoluteRadianceHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp,
                SetAbsoluteRadianceP5HpEnabled,
                out absoluteRadianceP5HpValue
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateToggleRow(
                content,
                "AbsoluteRadianceUseMaxHpRow",
                "Settings/AbsoluteRadianceHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseMaxHp,
                SetAbsoluteRadianceUseMaxHpEnabled,
                out absoluteRadianceUseMaxHpValue
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "AbsoluteRadianceMaxHpRow",
                "Settings/AbsoluteRadianceHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp,
                SetAbsoluteRadianceMaxHp,
                1,
                999999,
                10,
                out absoluteRadianceMaxHpField
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateToggleRow(
                content,
                "AbsoluteRadianceUseCustomPhaseRow",
                "Settings/AbsoluteRadianceHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase,
                SetAbsoluteRadianceUseCustomPhaseEnabled,
                out absoluteRadianceUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "AbsoluteRadiancePhase2HpRow",
                "Settings/AbsoluteRadianceHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp,
                SetAbsoluteRadiancePhase2Hp,
                1,
                999999,
                10,
                out absoluteRadiancePhase2HpField
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "AbsoluteRadiancePhase3HpRow",
                "Settings/AbsoluteRadianceHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp,
                SetAbsoluteRadiancePhase3Hp,
                1,
                999999,
                10,
                out absoluteRadiancePhase3HpField
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "AbsoluteRadiancePhase4HpRow",
                "Settings/AbsoluteRadianceHelper/Phase4HP".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp,
                SetAbsoluteRadiancePhase4Hp,
                1,
                999999,
                10,
                out absoluteRadiancePhase4HpField
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "AbsoluteRadiancePhase5HpRow",
                "Settings/AbsoluteRadianceHelper/Phase5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp,
                SetAbsoluteRadiancePhase5Hp,
                1,
                999999,
                10,
                out absoluteRadiancePhase5HpField
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "AbsoluteRadianceFinalPhaseHpRow",
                "Settings/AbsoluteRadianceHelper/FinalPhaseHP".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHp,
                SetAbsoluteRadianceFinalPhaseHp,
                1,
                999999,
                10,
                out absoluteRadianceFinalPhaseHpField
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "AbsoluteRadianceHelperResetRow", "Settings/AbsoluteRadianceHelper/Reset".Localize(), resetY, OnAbsoluteRadianceHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "AbsoluteRadianceHelperBackRow", "Back", backY, OnAbsoluteRadianceHelperBackClicked);
        }

        private void BuildPaintmasterSheoHelperOverlayUi()
        {
            BuildStandardGhostHelperOverlayUi(
                ref paintmasterSheoHelperRoot,
                ref paintmasterSheoHelperContent,
                "PaintmasterSheoHelper",
                PaintmasterSheoHelperCanvasSortOrder,
                PaintmasterSheoHelperPanelHeight,
                PaintmasterSheoHelperRowSpacing,
                GetPaintmasterSheoHelperEnabled,
                SetPaintmasterSheoHelperEnabled,
                () => Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoP5Hp,
                SetPaintmasterSheoP5HpEnabled,
                () => Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoUseMaxHp,
                SetPaintmasterSheoUseMaxHpEnabled,
                () => Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoMaxHp,
                SetPaintmasterSheoMaxHp,
                OnPaintmasterSheoHelperResetDefaultsClicked,
                OnPaintmasterSheoHelperBackClicked,
                out paintmasterSheoHelperToggleValue,
                out paintmasterSheoHelperToggleIcon,
                out paintmasterSheoP5HpValue,
                out paintmasterSheoUseMaxHpValue,
                out paintmasterSheoMaxHpField);
        }

        private void BuildSoulWarriorHelperOverlayUi()
        {
            soulWarriorHelperRoot = new GameObject("SoulWarriorHelperOverlayCanvas");
            soulWarriorHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = soulWarriorHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = SoulWarriorHelperCanvasSortOrder;

            CanvasScaler scaler = soulWarriorHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            soulWarriorHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = soulWarriorHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(soulWarriorHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("SoulWarriorHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, SoulWarriorHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/SoulWarriorHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(SoulWarriorHelperPanelHeight);
            float backY = GetFixedBackY(SoulWarriorHelperPanelHeight);
            float topOffset = GetScrollTopOffset(SoulWarriorHelperPanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(SoulWarriorHelperPanelHeight, resetY);
            float viewHeight = Mathf.Max(0f, SoulWarriorHelperPanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, SoulWarriorHelperPanelHeight, topOffset, bottomOffset);
            soulWarriorHelperContent = content;

            float rowY = GetRowStartY(SoulWarriorHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "SoulWarriorHelperEnableRow",
                "Settings/SoulWarriorHelper/Enable".Localize(),
                rowY,
                GetSoulWarriorHelperEnabled,
                SetSoulWarriorHelperEnabled,
                out soulWarriorHelperToggleValue,
                out soulWarriorHelperToggleIcon
            );

            lastY = rowY;
            rowY += SoulWarriorHelperRowSpacing;
            CreateToggleRow(
                content,
                "SoulWarriorP5HpRow",
                "Settings/SoulWarriorHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp,
                SetSoulWarriorP5HpEnabled,
                out soulWarriorP5HpValue
            );

            lastY = rowY;
            rowY += SoulWarriorHelperRowSpacing;
            CreateToggleRow(
                content,
                "SoulWarriorUseMaxHpRow",
                "Settings/SoulWarriorHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseMaxHp,
                SetSoulWarriorUseMaxHpEnabled,
                out soulWarriorUseMaxHpValue
            );

            lastY = rowY;
            rowY += SoulWarriorHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SoulWarriorMaxHpRow",
                "Settings/SoulWarriorHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulWarriorHelper.soulWarriorMaxHp,
                SetSoulWarriorMaxHp,
                1,
                999999,
                10,
                out soulWarriorMaxHpField
            );

            lastY = rowY;
            rowY += SoulWarriorHelperRowSpacing;
            CreateToggleRow(
                content,
                "SoulWarriorUseCustomSummonHpRow",
                "Settings/SoulWarriorHelper/UseCustomSummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonHp,
                SetSoulWarriorUseCustomSummonHpEnabled,
                out soulWarriorUseCustomSummonHpValue
            );

            lastY = rowY;
            rowY += SoulWarriorHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SoulWarriorSummonHpRow",
                "Settings/SoulWarriorHelper/SummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonHp,
                SetSoulWarriorSummonHp,
                1,
                999999,
                10,
                out soulWarriorSummonHpField
            );

            lastY = rowY;
            rowY += SoulWarriorHelperRowSpacing;
            CreateToggleRow(
                content,
                "SoulWarriorUseCustomSummonLimitRow",
                "Settings/SoulWarriorHelper/UseCustomSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonLimit,
                SetSoulWarriorUseCustomSummonLimitEnabled,
                out soulWarriorUseCustomSummonLimitValue
            );

            lastY = rowY;
            rowY += SoulWarriorHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SoulWarriorSummonLimitRow",
                "Settings/SoulWarriorHelper/SummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonLimit,
                SetSoulWarriorSummonLimit,
                36,
                999,
                1,
                out soulWarriorSummonLimitField
            );

            lastY = rowY;
            rowY += SoulWarriorHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "SoulWarriorHelperResetRow", "Settings/SoulWarriorHelper/Reset".Localize(), resetY, OnSoulWarriorHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "SoulWarriorHelperBackRow", "Back", backY, OnSoulWarriorHelperBackClicked);
        }

        private void BuildNailsageSlyHelperOverlayUi()
        {
            nailsageSlyHelperRoot = new GameObject("NailsageSlyHelperOverlayCanvas");
            nailsageSlyHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = nailsageSlyHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = NailsageSlyHelperCanvasSortOrder;

            CanvasScaler scaler = nailsageSlyHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            nailsageSlyHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = nailsageSlyHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(nailsageSlyHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("NailsageSlyHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, NailsageSlyHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/NailsageSlyHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(NailsageSlyHelperPanelHeight);
            float backY = GetFixedBackY(NailsageSlyHelperPanelHeight);
            float topOffset = GetScrollTopOffset(NailsageSlyHelperPanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(NailsageSlyHelperPanelHeight, resetY);
            float viewHeight = Mathf.Max(0f, NailsageSlyHelperPanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, NailsageSlyHelperPanelHeight, topOffset, bottomOffset);
            nailsageSlyHelperContent = content;

            float rowY = GetRowStartY(NailsageSlyHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "NailsageSlyHelperEnableRow",
                "Settings/NailsageSlyHelper/Enable".Localize(),
                rowY,
                GetNailsageSlyHelperEnabled,
                SetNailsageSlyHelperEnabled,
                out nailsageSlyHelperToggleValue,
                out nailsageSlyHelperToggleIcon
            );

            lastY = rowY;
            rowY += NailsageSlyHelperRowSpacing;
            CreateToggleRow(
                content,
                "NailsageSlyP5HpRow",
                "Settings/NailsageSlyHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NailsageSlyHelper.nailsageSlyP5Hp,
                SetNailsageSlyP5HpEnabled,
                out nailsageSlyP5HpValue
            );

            lastY = rowY;
            rowY += NailsageSlyHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NailsageSlyPhase1HpRow",
                "Settings/NailsageSlyHelper/Phase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase1Hp,
                SetNailsageSlyPhase1Hp,
                1,
                999999,
                10,
                out nailsageSlyPhase1HpField
            );

            lastY = rowY;
            rowY += NailsageSlyHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NailsageSlyPhase2HpRow",
                "Settings/NailsageSlyHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase2Hp,
                SetNailsageSlyPhase2Hp,
                1,
                999999,
                10,
                out nailsageSlyPhase2HpField
            );

            lastY = rowY;
            rowY += NailsageSlyHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "NailsageSlyHelperResetRow", "Settings/NailsageSlyHelper/Reset".Localize(), resetY, OnNailsageSlyHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "NailsageSlyHelperBackRow", "Back", backY, OnNailsageSlyHelperBackClicked);
        }

        private void BuildSoulMasterHelperOverlayUi()
        {
            soulMasterHelperRoot = new GameObject("SoulMasterHelperOverlayCanvas");
            soulMasterHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = soulMasterHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = SoulMasterHelperCanvasSortOrder;

            CanvasScaler scaler = soulMasterHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            soulMasterHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = soulMasterHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(soulMasterHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("SoulMasterHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, SoulMasterHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/SoulMasterHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(SoulMasterHelperPanelHeight);
            float backY = GetFixedBackY(SoulMasterHelperPanelHeight);
            float topOffset = GetScrollTopOffset(SoulMasterHelperPanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(SoulMasterHelperPanelHeight, resetY);
            float viewHeight = Mathf.Max(0f, SoulMasterHelperPanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, SoulMasterHelperPanelHeight, topOffset, bottomOffset);
            soulMasterHelperContent = content;

            float rowY = GetRowStartY(SoulMasterHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "SoulMasterHelperEnableRow",
                "Settings/SoulMasterHelper/Enable".Localize(),
                rowY,
                GetSoulMasterHelperEnabled,
                SetSoulMasterHelperEnabled,
                out soulMasterHelperToggleValue,
                out soulMasterHelperToggleIcon
            );

            lastY = rowY;
            rowY += SoulMasterHelperRowSpacing;
            CreateToggleRow(
                content,
                "SoulMasterP5HpRow",
                "Settings/SoulMasterHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulMasterHelper.soulMasterP5Hp,
                SetSoulMasterP5HpEnabled,
                out soulMasterP5HpValue
            );

            lastY = rowY;
            rowY += SoulMasterHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SoulMasterPhase1HpRow",
                "Settings/SoulMasterHelper/Phase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulMasterHelper.soulMasterPhase1Hp,
                SetSoulMasterPhase1Hp,
                1,
                999999,
                10,
                out soulMasterPhase1HpField
            );

            lastY = rowY;
            rowY += SoulMasterHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SoulMasterPhase2HpRow",
                "Settings/SoulMasterHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulMasterHelper.soulMasterPhase2Hp,
                SetSoulMasterPhase2Hp,
                1,
                999999,
                10,
                out soulMasterPhase2HpField
            );

            lastY = rowY;
            rowY += SoulMasterHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "SoulMasterHelperResetRow", "Settings/SoulMasterHelper/Reset".Localize(), resetY, OnSoulMasterHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "SoulMasterHelperBackRow", "Back", backY, OnSoulMasterHelperBackClicked);
        }

        private void BuildSoulTyrantHelperOverlayUi()
        {
            soulTyrantHelperRoot = new GameObject("SoulTyrantHelperOverlayCanvas");
            soulTyrantHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = soulTyrantHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = SoulTyrantHelperCanvasSortOrder;

            CanvasScaler scaler = soulTyrantHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            soulTyrantHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = soulTyrantHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(soulTyrantHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("SoulTyrantHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, SoulTyrantHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/SoulTyrantHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(SoulTyrantHelperPanelHeight);
            float backY = GetFixedBackY(SoulTyrantHelperPanelHeight);
            float topOffset = GetScrollTopOffset(SoulTyrantHelperPanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(SoulTyrantHelperPanelHeight, resetY);
            float viewHeight = Mathf.Max(0f, SoulTyrantHelperPanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, SoulTyrantHelperPanelHeight, topOffset, bottomOffset);
            soulTyrantHelperContent = content;

            float rowY = GetRowStartY(SoulTyrantHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "SoulTyrantHelperEnableRow",
                "Settings/SoulTyrantHelper/Enable".Localize(),
                rowY,
                GetSoulTyrantHelperEnabled,
                SetSoulTyrantHelperEnabled,
                out soulTyrantHelperToggleValue,
                out soulTyrantHelperToggleIcon
            );

            lastY = rowY;
            rowY += SoulTyrantHelperRowSpacing;
            CreateToggleRow(
                content,
                "SoulTyrantP5HpRow",
                "Settings/SoulTyrantHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulTyrantHelper.soulTyrantP5Hp,
                SetSoulTyrantP5HpEnabled,
                out soulTyrantP5HpValue
            );

            lastY = rowY;
            rowY += SoulTyrantHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SoulTyrantPhase1HpRow",
                "Settings/SoulTyrantHelper/Phase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase1Hp,
                SetSoulTyrantPhase1Hp,
                1,
                999999,
                10,
                out soulTyrantPhase1HpField
            );

            lastY = rowY;
            rowY += SoulTyrantHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SoulTyrantPhase2HpRow",
                "Settings/SoulTyrantHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase2Hp,
                SetSoulTyrantPhase2Hp,
                1,
                999999,
                10,
                out soulTyrantPhase2HpField
            );

            lastY = rowY;
            rowY += SoulTyrantHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "SoulTyrantHelperResetRow", "Settings/SoulTyrantHelper/Reset".Localize(), resetY, OnSoulTyrantHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "SoulTyrantHelperBackRow", "Back", backY, OnSoulTyrantHelperBackClicked);
        }

        private void BuildWatcherKnightHelperOverlayUi()
        {
            watcherKnightHelperRoot = new GameObject("WatcherKnightHelperOverlayCanvas");
            watcherKnightHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = watcherKnightHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = WatcherKnightHelperCanvasSortOrder;

            CanvasScaler scaler = watcherKnightHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            watcherKnightHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = watcherKnightHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(watcherKnightHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("WatcherKnightHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, WatcherKnightHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/WatcherKnightHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(WatcherKnightHelperPanelHeight);
            float backY = GetFixedBackY(WatcherKnightHelperPanelHeight);
            float topOffset = GetScrollTopOffset(WatcherKnightHelperPanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(WatcherKnightHelperPanelHeight, resetY);
            float viewHeight = Mathf.Max(0f, WatcherKnightHelperPanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, WatcherKnightHelperPanelHeight, topOffset, bottomOffset);
            watcherKnightHelperContent = content;

            float rowY = GetRowStartY(WatcherKnightHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "WatcherKnightHelperEnableRow",
                "Settings/WatcherKnightHelper/Enable".Localize(),
                rowY,
                GetWatcherKnightHelperEnabled,
                SetWatcherKnightHelperEnabled,
                out watcherKnightHelperToggleValue,
                out watcherKnightHelperToggleIcon
            );

            lastY = rowY;
            rowY += WatcherKnightHelperRowSpacing;
            CreateToggleRow(
                content,
                "WatcherKnightP5HpRow",
                "Settings/WatcherKnightHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp,
                SetWatcherKnightP5HpEnabled,
                out watcherKnightP5HpValue
            );

            lastY = rowY;
            rowY += WatcherKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WatcherKnight1HpRow",
                "Settings/WatcherKnightHelper/Knight1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WatcherKnightHelper.watcherKnight1Hp,
                SetWatcherKnight1Hp,
                1,
                999999,
                10,
                out watcherKnight1HpField
            );

            lastY = rowY;
            rowY += WatcherKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WatcherKnight2HpRow",
                "Settings/WatcherKnightHelper/Knight2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WatcherKnightHelper.watcherKnight2Hp,
                SetWatcherKnight2Hp,
                1,
                999999,
                10,
                out watcherKnight2HpField
            );

            lastY = rowY;
            rowY += WatcherKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WatcherKnight3HpRow",
                "Settings/WatcherKnightHelper/Knight3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WatcherKnightHelper.watcherKnight3Hp,
                SetWatcherKnight3Hp,
                1,
                999999,
                10,
                out watcherKnight3HpField
            );

            lastY = rowY;
            rowY += WatcherKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WatcherKnight4HpRow",
                "Settings/WatcherKnightHelper/Knight4HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WatcherKnightHelper.watcherKnight4Hp,
                SetWatcherKnight4Hp,
                1,
                999999,
                10,
                out watcherKnight4HpField
            );

            lastY = rowY;
            rowY += WatcherKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WatcherKnight5HpRow",
                "Settings/WatcherKnightHelper/Knight5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WatcherKnightHelper.watcherKnight5Hp,
                SetWatcherKnight5Hp,
                1,
                999999,
                10,
                out watcherKnight5HpField
            );

            lastY = rowY;
            rowY += WatcherKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WatcherKnight6HpRow",
                "Settings/WatcherKnightHelper/Knight6HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WatcherKnightHelper.watcherKnight6Hp,
                SetWatcherKnight6Hp,
                1,
                999999,
                10,
                out watcherKnight6HpField
            );

            lastY = rowY;
            rowY += WatcherKnightHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "WatcherKnightHelperResetRow", "Settings/WatcherKnightHelper/Reset".Localize(), resetY, OnWatcherKnightHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "WatcherKnightHelperBackRow", "Back", backY, OnWatcherKnightHelperBackClicked);
        }

        private void BuildOroMatoHelperOverlayUi()
        {
            oroMatoHelperRoot = new GameObject("OroMatoHelperOverlayCanvas");
            oroMatoHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = oroMatoHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = OroMatoHelperCanvasSortOrder;

            CanvasScaler scaler = oroMatoHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            oroMatoHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = oroMatoHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(oroMatoHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("OroMatoHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, OroMatoHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/OroMatoHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(OroMatoHelperPanelHeight);
            float backY = GetFixedBackY(OroMatoHelperPanelHeight);
            float topOffset = GetScrollTopOffset(OroMatoHelperPanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(OroMatoHelperPanelHeight, resetY);
            float viewHeight = Mathf.Max(0f, OroMatoHelperPanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, OroMatoHelperPanelHeight, topOffset, bottomOffset);
            oroMatoHelperContent = content;

            float rowY = GetRowStartY(OroMatoHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "OroMatoHelperEnableRow",
                "Settings/OroMatoHelper/Enable".Localize(),
                rowY,
                GetOroMatoHelperEnabled,
                SetOroMatoHelperEnabled,
                out oroMatoHelperToggleValue,
                out oroMatoHelperToggleIcon
            );

            lastY = rowY;
            rowY += OroMatoHelperRowSpacing;
            CreateToggleRow(
                content,
                "OroMatoP5HpRow",
                "Settings/OroMatoHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.OroMatoHelper.oroMatoP5Hp,
                SetOroMatoP5HpEnabled,
                out oroMatoP5HpValue
            );

            lastY = rowY;
            rowY += OroMatoHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "OroMatoOroPhase1HpRow",
                "Settings/OroMatoHelper/OroPhase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase1Hp,
                SetOroMatoOroPhase1Hp,
                1,
                999999,
                10,
                out oroMatoOroPhase1HpField
            );

            lastY = rowY;
            rowY += OroMatoHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "OroMatoOroPhase2HpRow",
                "Settings/OroMatoHelper/OroPhase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase2Hp,
                SetOroMatoOroPhase2Hp,
                1,
                999999,
                10,
                out oroMatoOroPhase2HpField
            );

            lastY = rowY;
            rowY += OroMatoHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "OroMatoMatoHpRow",
                "Settings/OroMatoHelper/MatoHP".Localize(),
                rowY,
                () => Modules.BossChallenge.OroMatoHelper.oroMatoMatoHp,
                SetOroMatoMatoHp,
                1,
                999999,
                10,
                out oroMatoMatoHpField
            );

            lastY = rowY;
            rowY += OroMatoHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "OroMatoHelperResetRow", "Settings/OroMatoHelper/Reset".Localize(), resetY, OnOroMatoHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "OroMatoHelperBackRow", "Back", backY, OnOroMatoHelperBackClicked);
        }

        private void BuildGodTamerHelperOverlayUi()
        {
            godTamerHelperRoot = new GameObject("GodTamerHelperOverlayCanvas");
            godTamerHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = godTamerHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = GodTamerHelperCanvasSortOrder;

            CanvasScaler scaler = godTamerHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            godTamerHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = godTamerHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(godTamerHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("GodTamerHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, GodTamerHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/GodTamerHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(GodTamerHelperPanelHeight);
            float backY = GetFixedBackY(GodTamerHelperPanelHeight);
            float topOffset = GetScrollTopOffset(GodTamerHelperPanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(GodTamerHelperPanelHeight, resetY);
            float viewHeight = Mathf.Max(0f, GodTamerHelperPanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, GodTamerHelperPanelHeight, topOffset, bottomOffset);
            godTamerHelperContent = content;

            float rowY = GetRowStartY(GodTamerHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "GodTamerHelperEnableRow",
                "Settings/GodTamerHelper/Enable".Localize(),
                rowY,
                GetGodTamerHelperEnabled,
                SetGodTamerHelperEnabled,
                out godTamerHelperToggleValue,
                out godTamerHelperToggleIcon
            );

            lastY = rowY;
            rowY += GodTamerHelperRowSpacing;
            CreateToggleRow(
                content,
                "GodTamerP5HpRow",
                "Settings/GodTamerHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.GodTamerHelper.godTamerP5Hp,
                SetGodTamerP5HpEnabled,
                out godTamerP5HpValue
            );

            lastY = rowY;
            rowY += GodTamerHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GodTamerLobsterHpRow",
                "Settings/GodTamerHelper/LobsterHP".Localize(),
                rowY,
                () => Modules.BossChallenge.GodTamerHelper.godTamerLobsterHp,
                SetGodTamerLobsterHp,
                1,
                999999,
                10,
                out godTamerLobsterHpField
            );

            lastY = rowY;
            rowY += GodTamerHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GodTamerLancerHpRow",
                "Settings/GodTamerHelper/LancerHP".Localize(),
                rowY,
                () => Modules.BossChallenge.GodTamerHelper.godTamerLancerHp,
                SetGodTamerLancerHp,
                1,
                999999,
                10,
                out godTamerLancerHpField
            );

            lastY = rowY;
            rowY += GodTamerHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "GodTamerHelperResetRow", "Settings/GodTamerHelper/Reset".Localize(), resetY, OnGodTamerHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "GodTamerHelperBackRow", "Back", backY, OnGodTamerHelperBackClicked);
        }

        private void BuildOblobblesHelperOverlayUi()
        {
            oblobblesHelperRoot = new GameObject("OblobblesHelperOverlayCanvas");
            oblobblesHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = oblobblesHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = OblobblesHelperCanvasSortOrder;

            CanvasScaler scaler = oblobblesHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            oblobblesHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = oblobblesHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(oblobblesHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("OblobblesHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, OblobblesHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/OblobblesHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(OblobblesHelperPanelHeight);
            float backY = GetFixedBackY(OblobblesHelperPanelHeight);
            float topOffset = GetScrollTopOffset(OblobblesHelperPanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(OblobblesHelperPanelHeight, resetY);
            float viewHeight = Mathf.Max(0f, OblobblesHelperPanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, OblobblesHelperPanelHeight, topOffset, bottomOffset);
            oblobblesHelperContent = content;

            float rowY = GetRowStartY(OblobblesHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "OblobblesHelperEnableRow",
                "Settings/OblobblesHelper/Enable".Localize(),
                rowY,
                GetOblobblesHelperEnabled,
                SetOblobblesHelperEnabled,
                out oblobblesHelperToggleValue,
                out oblobblesHelperToggleIcon
            );

            lastY = rowY;
            rowY += OblobblesHelperRowSpacing;
            CreateToggleRow(
                content,
                "OblobblesP5HpRow",
                "Settings/OblobblesHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.OblobblesHelper.oblobblesP5Hp,
                SetOblobblesP5HpEnabled,
                out oblobblesP5HpValue
            );

            lastY = rowY;
            rowY += OblobblesHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "OblobblesLeftPhase1HpRow",
                "Settings/OblobblesHelper/LeftPhase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.OblobblesHelper.oblobblesLeftPhase1Hp,
                SetOblobblesLeftPhase1Hp,
                1,
                999999,
                10,
                out oblobblesLeftPhase1HpField
            );

            lastY = rowY;
            rowY += OblobblesHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "OblobblesRightPhase1HpRow",
                "Settings/OblobblesHelper/RightPhase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.OblobblesHelper.oblobblesRightPhase1Hp,
                SetOblobblesRightPhase1Hp,
                1,
                999999,
                10,
                out oblobblesRightPhase1HpField
            );

            lastY = rowY;
            rowY += OblobblesHelperRowSpacing;
            CreateToggleRow(
                content,
                "OblobblesUsePhase2HpRow",
                "Settings/OblobblesHelper/UsePhase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.OblobblesHelper.oblobblesUsePhase2Hp,
                SetOblobblesUsePhase2HpEnabled,
                out oblobblesUsePhase2HpValue
            );

            lastY = rowY;
            rowY += OblobblesHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "OblobblesPhase2HpRow",
                "Settings/OblobblesHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.OblobblesHelper.oblobblesPhase2Hp,
                SetOblobblesPhase2Hp,
                1,
                999999,
                10,
                out oblobblesPhase2HpField
            );

            lastY = rowY;
            rowY += OblobblesHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "OblobblesHelperResetRow", "Settings/OblobblesHelper/Reset".Localize(), resetY, OnOblobblesHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "OblobblesHelperBackRow", "Back", backY, OnOblobblesHelperBackClicked);
        }

        private void BuildFalseKnightHelperOverlayUi()
        {
            falseKnightHelperRoot = new GameObject("FalseKnightHelperOverlayCanvas");
            falseKnightHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = falseKnightHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = FalseKnightHelperCanvasSortOrder;

            CanvasScaler scaler = falseKnightHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            falseKnightHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = falseKnightHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(falseKnightHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("FalseKnightHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, FalseKnightHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/FalseKnightHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(FalseKnightHelperPanelHeight);
            float backY = GetFixedBackY(FalseKnightHelperPanelHeight);
            float topOffset = GetScrollTopOffset(FalseKnightHelperPanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(FalseKnightHelperPanelHeight, resetY);
            float viewHeight = Mathf.Max(0f, FalseKnightHelperPanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, FalseKnightHelperPanelHeight, topOffset, bottomOffset);
            falseKnightHelperContent = content;

            float rowY = GetRowStartY(FalseKnightHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "FalseKnightHelperEnableRow",
                "Settings/FalseKnightHelper/Enable".Localize(),
                rowY,
                GetFalseKnightHelperEnabled,
                SetFalseKnightHelperEnabled,
                out falseKnightHelperToggleValue,
                out falseKnightHelperToggleIcon
            );

            lastY = rowY;
            rowY += FalseKnightHelperRowSpacing;
            CreateToggleRow(
                content,
                "FalseKnightP5HpRow",
                "Settings/FalseKnightHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FalseKnightHelper.falseKnightP5Hp,
                SetFalseKnightP5HpEnabled,
                out falseKnightP5HpValue
            );

            lastY = rowY;
            rowY += FalseKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FalseKnightArmorPhase1HpRow",
                "Settings/FalseKnightHelper/ArmorPhase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase1Hp,
                SetFalseKnightArmorPhase1Hp,
                1,
                999999,
                10,
                out falseKnightArmorPhase1HpField
            );

            lastY = rowY;
            rowY += FalseKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FalseKnightArmorPhase2HpRow",
                "Settings/FalseKnightHelper/ArmorPhase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase2Hp,
                SetFalseKnightArmorPhase2Hp,
                1,
                999999,
                10,
                out falseKnightArmorPhase2HpField
            );

            lastY = rowY;
            rowY += FalseKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FalseKnightArmorPhase3HpRow",
                "Settings/FalseKnightHelper/ArmorPhase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase3Hp,
                SetFalseKnightArmorPhase3Hp,
                1,
                999999,
                10,
                out falseKnightArmorPhase3HpField
            );

            lastY = rowY;
            rowY += FalseKnightHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "FalseKnightHelperResetRow", "Settings/FalseKnightHelper/Reset".Localize(), resetY, OnFalseKnightHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "FalseKnightHelperBackRow", "Back", backY, OnFalseKnightHelperBackClicked);
        }

        private void BuildFailedChampionHelperOverlayUi()
        {
            failedChampionHelperRoot = new GameObject("FailedChampionHelperOverlayCanvas");
            failedChampionHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = failedChampionHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = FailedChampionHelperCanvasSortOrder;

            CanvasScaler scaler = failedChampionHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            failedChampionHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = failedChampionHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(failedChampionHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("FailedChampionHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, FailedChampionHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/FailedChampionHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(FailedChampionHelperPanelHeight);
            float backY = GetFixedBackY(FailedChampionHelperPanelHeight);
            float topOffset = GetScrollTopOffset(FailedChampionHelperPanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(FailedChampionHelperPanelHeight, resetY);
            float viewHeight = Mathf.Max(0f, FailedChampionHelperPanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, FailedChampionHelperPanelHeight, topOffset, bottomOffset);
            failedChampionHelperContent = content;

            float rowY = GetRowStartY(FailedChampionHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "FailedChampionHelperEnableRow",
                "Settings/FailedChampionHelper/Enable".Localize(),
                rowY,
                GetFailedChampionHelperEnabled,
                SetFailedChampionHelperEnabled,
                out failedChampionHelperToggleValue,
                out failedChampionHelperToggleIcon
            );

            lastY = rowY;
            rowY += FailedChampionHelperRowSpacing;
            CreateToggleRow(
                content,
                "FailedChampionP5HpRow",
                "Settings/FailedChampionHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FailedChampionHelper.failedChampionP5Hp,
                SetFailedChampionP5HpEnabled,
                out failedChampionP5HpValue
            );

            lastY = rowY;
            rowY += FailedChampionHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FailedChampionArmorPhase1HpRow",
                "Settings/FailedChampionHelper/ArmorPhase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase1Hp,
                SetFailedChampionArmorPhase1Hp,
                1,
                999999,
                10,
                out failedChampionArmorPhase1HpField
            );

            lastY = rowY;
            rowY += FailedChampionHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FailedChampionArmorPhase2HpRow",
                "Settings/FailedChampionHelper/ArmorPhase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase2Hp,
                SetFailedChampionArmorPhase2Hp,
                1,
                999999,
                10,
                out failedChampionArmorPhase2HpField
            );

            lastY = rowY;
            rowY += FailedChampionHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FailedChampionArmorPhase3HpRow",
                "Settings/FailedChampionHelper/ArmorPhase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase3Hp,
                SetFailedChampionArmorPhase3Hp,
                1,
                999999,
                10,
                out failedChampionArmorPhase3HpField
            );

            lastY = rowY;
            rowY += FailedChampionHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "FailedChampionHelperResetRow", "Settings/FailedChampionHelper/Reset".Localize(), resetY, OnFailedChampionHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "FailedChampionHelperBackRow", "Back", backY, OnFailedChampionHelperBackClicked);
        }

        private void BuildFlukemarmHelperOverlayUi()
        {
            flukemarmHelperRoot = new GameObject("FlukemarmHelperOverlayCanvas");
            flukemarmHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = flukemarmHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = FlukemarmHelperCanvasSortOrder;

            CanvasScaler scaler = flukemarmHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            flukemarmHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = flukemarmHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(flukemarmHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("FlukemarmHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, FlukemarmHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/FlukemarmHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(FlukemarmHelperPanelHeight);
            float backY = GetFixedBackY(FlukemarmHelperPanelHeight);
            float topOffset = GetScrollTopOffset(FlukemarmHelperPanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(FlukemarmHelperPanelHeight, resetY);
            float viewHeight = Mathf.Max(0f, FlukemarmHelperPanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, FlukemarmHelperPanelHeight, topOffset, bottomOffset);
            flukemarmHelperContent = content;

            float rowY = GetRowStartY(FlukemarmHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "FlukemarmHelperEnableRow",
                "Settings/FlukemarmHelper/Enable".Localize(),
                rowY,
                GetFlukemarmHelperEnabled,
                SetFlukemarmHelperEnabled,
                out flukemarmHelperToggleValue,
                out flukemarmHelperToggleIcon
            );

            lastY = rowY;
            rowY += FlukemarmHelperRowSpacing;
            CreateToggleRow(
                content,
                "FlukemarmP5HpRow",
                "Settings/FlukemarmHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp,
                SetFlukemarmP5HpEnabled,
                out flukemarmP5HpValue
            );

            lastY = rowY;
            rowY += FlukemarmHelperRowSpacing;
            CreateToggleRow(
                content,
                "FlukemarmUseMaxHpRow",
                "Settings/FlukemarmHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHp,
                SetFlukemarmUseMaxHpEnabled,
                out flukemarmUseMaxHpValue
            );

            lastY = rowY;
            rowY += FlukemarmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FlukemarmMaxHpRow",
                "Settings/FlukemarmHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.FlukemarmHelper.flukemarmMaxHp,
                SetFlukemarmMaxHp,
                1,
                999999,
                10,
                out flukemarmMaxHpField
            );

            lastY = rowY;
            rowY += FlukemarmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FlukemarmFlyHpRow",
                "Settings/FlukemarmHelper/FlukeFlyHP".Localize(),
                rowY,
                () => Modules.BossChallenge.FlukemarmHelper.flukemarmFlyHp,
                SetFlukemarmFlyHp,
                1,
                999999,
                1,
                out flukemarmFlyHpField
            );

            lastY = rowY;
            rowY += FlukemarmHelperRowSpacing;
            CreateToggleRow(
                content,
                "FlukemarmUseCustomSummonLimitRow",
                "Settings/FlukemarmHelper/UseCustomSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.FlukemarmHelper.flukemarmUseCustomSummonLimit,
                SetFlukemarmUseCustomSummonLimitEnabled,
                out flukemarmUseCustomSummonLimitValue
            );

            lastY = rowY;
            rowY += FlukemarmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FlukemarmSummonLimitRow",
                "Settings/FlukemarmHelper/SummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.FlukemarmHelper.flukemarmSummonLimit,
                SetFlukemarmSummonLimit,
                6,
                999,
                1,
                out flukemarmSummonLimitField
            );

            lastY = rowY;
            rowY += FlukemarmHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "FlukemarmHelperResetRow", "Settings/FlukemarmHelper/Reset".Localize(), resetY, OnFlukemarmHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "FlukemarmHelperBackRow", "Back", backY, OnFlukemarmHelperBackClicked);
        }

        private void BuildVengeflyKingOverlayUi()
        {
            vengeflyKingRoot = new GameObject("VengeflyKingOverlayCanvas");
            vengeflyKingRoot.transform.SetParent(transform, false);

            Canvas canvas = vengeflyKingRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = VengeflyKingCanvasSortOrder;

            CanvasScaler scaler = vengeflyKingRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            vengeflyKingRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = vengeflyKingRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(vengeflyKingRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("VengeflyKingPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, VengeflyKingPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/VengeflyKing".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(VengeflyKingPanelHeight);
            float backY = GetFixedBackY(VengeflyKingPanelHeight);
            float topOffset = GetScrollTopOffset(VengeflyKingPanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(VengeflyKingPanelHeight, resetY);
            float viewHeight = Mathf.Max(0f, VengeflyKingPanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, VengeflyKingPanelHeight, topOffset, bottomOffset);
            vengeflyKingContent = content;

            float rowY = GetRowStartY(VengeflyKingPanelHeight, RowStartY, topOffset);
            float lastY = rowY;

            CreateToggleRowWithIcon(
                content,
                "VengeflyKingEnableRow",
                "Settings/VengeflyKing/Enable".Localize(),
                rowY,
                GetVengeflyKingEnabled,
                SetVengeflyKingEnabled,
                out vengeflyKingToggleValue,
                out vengeflyKingToggleIcon
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateToggleRow(
                content,
                "VengeflyKingP5HpRow",
                "Settings/VengeflyKing/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp,
                SetVengeflyKingP5HpEnabled,
                out vengeflyKingP5HpValue
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateToggleRow(
                content,
                "VengeflyKingUseMaxHpRow",
                "Settings/VengeflyKing/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp,
                SetVengeflyKingUseMaxHpEnabled,
                out vengeflyKingUseMaxHpValue
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateAdjustInputRow(
                content,
                "VengeflyKingLeftMaxHpRow",
                "Settings/VengeflyKing/LeftMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingLeftMaxHp,
                SetVengeflyKingLeftMaxHp,
                1,
                999999,
                10,
                out vengeflyKingLeftMaxHpField
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateAdjustInputRow(
                content,
                "VengeflyKingRightMaxHpRow",
                "Settings/VengeflyKing/RightMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingRightMaxHp,
                SetVengeflyKingRightMaxHp,
                1,
                999999,
                10,
                out vengeflyKingRightMaxHpField
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateAdjustInputRow(
                content,
                "VengeflyKingSummonMaxHpRow",
                "Settings/VengeflyKing/VengeflyHP".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingSummonMaxHp,
                SetVengeflyKingSummonMaxHp,
                1,
                999999,
                1,
                out vengeflyKingSummonMaxHpField
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateToggleRow(
                content,
                "VengeflyKingUseCustomSummonLimitRow",
                "Settings/VengeflyKing/UseCustomSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit,
                SetVengeflyKingUseCustomSummonLimitEnabled,
                out vengeflyKingUseCustomSummonLimitValue
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateAdjustInputRow(
                content,
                "VengeflyKingLeftSummonLimitRow",
                "Settings/VengeflyKing/LeftSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonLimit,
                SetVengeflyKingLeftSummonLimit,
                0,
                999,
                1,
                out vengeflyKingLeftSummonLimitField
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateAdjustInputRow(
                content,
                "VengeflyKingRightSummonLimitRow",
                "Settings/VengeflyKing/RightSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonLimit,
                SetVengeflyKingRightSummonLimit,
                0,
                999,
                1,
                out vengeflyKingRightSummonLimitField
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateAdjustInputRow(
                content,
                "VengeflyKingLeftSummonAttackLimitRow",
                "Settings/VengeflyKing/LeftSummonAttackLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonAttackLimit,
                SetVengeflyKingLeftSummonAttackLimit,
                0,
                999,
                1,
                out vengeflyKingLeftSummonAttackLimitField
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateAdjustInputRow(
                content,
                "VengeflyKingRightSummonAttackLimitRow",
                "Settings/VengeflyKing/RightSummonAttackLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonAttackLimit,
                SetVengeflyKingRightSummonAttackLimit,
                0,
                999,
                1,
                out vengeflyKingRightSummonAttackLimitField
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "VengeflyKingResetRow", "Settings/VengeflyKing/Reset".Localize(), resetY, OnVengeflyKingResetDefaultsClicked);
            CreateButtonRow(panel.transform, "VengeflyKingBackRow", "Back", backY, OnVengeflyKingBackClicked);
        }

        private void BuildSisterOfBattleHelperOverlayUi()
        {
            sisterOfBattleHelperRoot = new GameObject("SisterOfBattleHelperOverlayCanvas");
            sisterOfBattleHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = sisterOfBattleHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = SisterOfBattleHelperCanvasSortOrder;

            CanvasScaler scaler = sisterOfBattleHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            sisterOfBattleHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = sisterOfBattleHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(sisterOfBattleHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("SisterOfBattleHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, SisterOfBattleHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/SisterOfBattleHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(SisterOfBattleHelperPanelHeight);
            float backY = GetFixedBackY(SisterOfBattleHelperPanelHeight);
            float topOffset = GetScrollTopOffset(SisterOfBattleHelperPanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(SisterOfBattleHelperPanelHeight, resetY);
            float viewHeight = Mathf.Max(0f, SisterOfBattleHelperPanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, SisterOfBattleHelperPanelHeight, topOffset, bottomOffset);
            sisterOfBattleHelperContent = content;

            float rowY = GetRowStartY(SisterOfBattleHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "SisterOfBattleHelperEnableRow",
                "Settings/SisterOfBattleHelper/Enable".Localize(),
                rowY,
                GetSisterOfBattleHelperEnabled,
                SetSisterOfBattleHelperEnabled,
                out sisterOfBattleHelperToggleValue,
                out sisterOfBattleHelperToggleIcon
            );

            lastY = rowY;
            rowY += SisterOfBattleHelperRowSpacing;
            CreateToggleRow(
                content,
                "SisterOfBattleP5HpRow",
                "Settings/SisterOfBattleHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleP5Hp,
                SetSisterOfBattleP5HpEnabled,
                out sisterOfBattleP5HpValue
            );

            lastY = rowY;
            rowY += SisterOfBattleHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SisterOfBattlePhase1HpRow",
                "Settings/SisterOfBattleHelper/Phase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase1Hp,
                SetSisterOfBattlePhase1Hp,
                1,
                999999,
                10,
                out sisterOfBattlePhase1HpField
            );

            lastY = rowY;
            rowY += SisterOfBattleHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SisterOfBattlePhase2S1HpRow",
                "Settings/SisterOfBattleHelper/Phase2S1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S1Hp,
                SetSisterOfBattlePhase2S1Hp,
                1,
                999999,
                10,
                out sisterOfBattlePhase2S1HpField
            );

            lastY = rowY;
            rowY += SisterOfBattleHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SisterOfBattlePhase2S2HpRow",
                "Settings/SisterOfBattleHelper/Phase2S2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S2Hp,
                SetSisterOfBattlePhase2S2Hp,
                1,
                999999,
                10,
                out sisterOfBattlePhase2S2HpField
            );

            lastY = rowY;
            rowY += SisterOfBattleHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SisterOfBattlePhase2S3HpRow",
                "Settings/SisterOfBattleHelper/Phase2S3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S3Hp,
                SetSisterOfBattlePhase2S3Hp,
                1,
                999999,
                10,
                out sisterOfBattlePhase2S3HpField
            );

            lastY = rowY;
            rowY += SisterOfBattleHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "SisterOfBattleHelperResetRow", "Settings/SisterOfBattleHelper/Reset".Localize(), resetY, OnSisterOfBattleHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "SisterOfBattleHelperBackRow", "Back", backY, OnSisterOfBattleHelperBackClicked);
        }

        private void BuildMantisLordHelperOverlayUi()
        {
            mantisLordHelperRoot = new GameObject("MantisLordHelperOverlayCanvas");
            mantisLordHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = mantisLordHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = MantisLordHelperCanvasSortOrder;

            CanvasScaler scaler = mantisLordHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            mantisLordHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = mantisLordHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(mantisLordHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("MantisLordHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, MantisLordHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/MantisLordHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float resetY = GetFixedResetY(MantisLordHelperPanelHeight);
            float backY = GetFixedBackY(MantisLordHelperPanelHeight);
            float topOffset = GetScrollTopOffset(MantisLordHelperPanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(MantisLordHelperPanelHeight, resetY);
            float viewHeight = Mathf.Max(0f, MantisLordHelperPanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, MantisLordHelperPanelHeight, topOffset, bottomOffset);
            mantisLordHelperContent = content;

            float rowY = GetRowStartY(MantisLordHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "MantisLordHelperEnableRow",
                "Settings/MantisLordHelper/Enable".Localize(),
                rowY,
                GetMantisLordHelperEnabled,
                SetMantisLordHelperEnabled,
                out mantisLordHelperToggleValue,
                out mantisLordHelperToggleIcon
            );

            lastY = rowY;
            rowY += MantisLordHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MantisLordPhase1HpRow",
                "Settings/MantisLordHelper/Phase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.MantisLordHelper.mantisLordPhase1Hp,
                SetMantisLordPhase1Hp,
                1,
                999999,
                10,
                out mantisLordPhase1HpField
            );

            lastY = rowY;
            rowY += MantisLordHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MantisLordPhase2S1HpRow",
                "Settings/MantisLordHelper/Phase2S1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.MantisLordHelper.mantisLordPhase2S1Hp,
                SetMantisLordPhase2S1Hp,
                1,
                999999,
                10,
                out mantisLordPhase2S1HpField
            );

            lastY = rowY;
            rowY += MantisLordHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MantisLordPhase2S2HpRow",
                "Settings/MantisLordHelper/Phase2S2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.MantisLordHelper.mantisLordPhase2S2Hp,
                SetMantisLordPhase2S2Hp,
                1,
                999999,
                10,
                out mantisLordPhase2S2HpField
            );

            lastY = rowY;
            rowY += MantisLordHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "MantisLordHelperResetRow", "Settings/MantisLordHelper/Reset".Localize(), resetY, OnMantisLordHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "MantisLordHelperBackRow", "Back", backY, OnMantisLordHelperBackClicked);
        }

    }
}
