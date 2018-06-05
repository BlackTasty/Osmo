using Osmo.Core.Objects;
using Osmo.Core.Reader;
using System.Collections.Generic;

namespace Osmo.Core
{
    static class FixedValues
    {
        internal static void InitializeReader()
        {
            if (readerInterface == null)
                readerInterface = new SkinElementReader(Properties.Resources.SkinningInterface);
            if (readerStandard == null)
                readerStandard = new SkinElementReader(Properties.Resources.SkinningStandard);
            if (readerCatch == null)
                readerCatch = new SkinElementReader(Properties.Resources.SkinningCatch);
            if (readerMania == null)
                readerMania = new SkinElementReader(Properties.Resources.SkinningMania);
            if (readerTaiko == null)
                readerTaiko = new SkinElementReader(Properties.Resources.SkinningTaiko);
            if (readerSounds == null)
                readerSounds = new SkinSoundReader(Properties.Resources.SkinningSounds);
        }

        internal const int EDITOR_INDEX = 2;
        internal const int CONFIG_INDEX = 4;
        
        internal static SkinElementReader readerInterface;
        internal static SkinElementReader readerStandard;
        internal static SkinElementReader readerCatch;
        internal static SkinElementReader readerMania;
        internal static SkinElementReader readerTaiko;
        internal static SkinSoundReader readerSounds;

        internal static readonly List<CompletionData> skinIniGeneralCompletionData =
            new List<CompletionData>() {
            new CompletionData("Name", "What is the name of this skin?"),
            new CompletionData("Author", "Who is the author of this skin?"),
            new CompletionData("Version", "How should the skin behave?\nVisit https://osu.ppy.sh/help/wiki/Skinning/skin.ini for more details about versions."),
            new CompletionData("AnimationFramerate", "How many frames should be displayed by the animations that depend on this value in one second?"),
            new CompletionData("AllowSliderBallTint", "Should the slider combo colour tint the slider ball?"),
            new CompletionData("ComboBurstRandom", "Should combobursts be shown in a random order?"),
            new CompletionData("CursorCentre", "Should the cursor have an origin at the centre of the image?"),
            new CompletionData("CursorExpand", "Should the cursor expand when clicked?"),
            new CompletionData("CursorRotate", "Should the cursor sprite rotate constantly?"),
            new CompletionData("CursorTrailRotate", "Should the cursor sprite rotate constantly?"),
            new CompletionData("CustomComboBurstSounds", "On which combo counts should the comboburst sounds be played?"),
            new CompletionData("HitCircleOverlayAboveNumber", "Should the hitcircleoverlay be drawn above the numbers?"),
            new CompletionData("LayeredHitSounds", "Should the hitnormal sounds always be played?"),
            new CompletionData("SliderBallFlip", "If the sliderball is reversed, should the sliderball sprite flip horizontally?"),
            new CompletionData("SliderBallFrames", "How many frames do you have for the sliderball animation?"),
            new CompletionData("SliderStyle", "What style should the sliders use?"),
            new CompletionData("SpinnerFadePlayfield", "Should the spinner add black bars during spins?"),
            new CompletionData("SpinnerFrequencyModulate", "Should the spinnerspin sound pitch up the longer the spinner goes?"),
            new CompletionData("SpinnerNoBlink", "Should the highest bar of the metre stay visible all the time?") };

        internal static readonly List<CompletionData> skinIniColoursCompletionData =
            new List<CompletionData>() {
            new CompletionData("Combo1", "What colour is used for the last combo?"),
            new CompletionData("Combo2", "What colour is used for the first combo?"),
            new CompletionData("Combo3", "What colour is used for the second combo?"),
            new CompletionData("Combo4", "What colour is used for the third combo?"),
            new CompletionData("Combo5", "What colour is used for the fourth combo?"),
            new CompletionData("Combo6", "What colour is used for the fifth combo?"),
            new CompletionData("Combo7", "What colour is used for the sixth combo?"),
            new CompletionData("Combo8", "What colour is used for the seventh combo?"),
            new CompletionData("InputOverlayText", "What colour should the numbers on the input keys be tinted in?"),
            new CompletionData("MenuGlow", "What colour should the spectrum bars in the main menu be coloured in?"),
            new CompletionData("SliderBall", "What colour should the default sliderball be coloured in?"),
            new CompletionData("SliderBorder", "What colour should be used for the sliderborders?"),
            new CompletionData("SliderTrackOverride", "What colour should all sliderbodies be coloured in?"),
            new CompletionData("SongSelectActiveText", "What colour should the text of the active panel be tinted in?"),
            new CompletionData("SongSelectInactiveText", "What colour should the text of the inactive panels be tinted in?"),
            new CompletionData("SpinnerBackground", "What colour should be added to the spinner-background?"),
            new CompletionData("StarBreakAdditive", "What colour should be added to star2 during breaks?") };

        internal static readonly List<CompletionData> skinIniFontsCompletionData =
            new List<CompletionData>() {
            new CompletionData("HitCirclePrefix","What prefix is used for the hitcircle numbers?"),
            new CompletionData("HitCircleOverlap","By how many pixels should the hitcircle numbers overlap?"),
            new CompletionData("ScorePrefix","What prefix is used for the score numbers?"),
            new CompletionData("ScoreOverlap","By how many pixels should the score numbers overlap?"),
            new CompletionData("ComboPrefix","What prefix is used for the combo numbers?"),
            new CompletionData("ComboOverlap","By how many pixels should the combo numbers overlap?") };

        internal static readonly List<CompletionData> skinIniCTBCompletionData =
            new List<CompletionData>() {
            new CompletionData("HyperDash","What colour should be used for the dash?"),
            new CompletionData("HyperDashFruit","What colour should be used for the fruits?"),
            new CompletionData("HyperDashAfterImage","What colour should be used for the after images?") };

        internal static readonly List<CompletionData> skinIniManiaCompletionData =
            new List<CompletionData>() {
            new CompletionData("Keys","What keycount are these settings for?"),
            new CompletionData("ColumnStart","Where does the left column start?"),
            new CompletionData("ColumnRight","Up to which point can columns be drawn?"),
            new CompletionData("ColumnSpacing","What is the distance between all columns individually?"),
            new CompletionData("ColumnWidth","What widths do all columns have individually?"),
            new CompletionData("ColumnLineWidth","How thick are the column seperators individually?"),
            new CompletionData("BarlineHeight","How thick is the barline?"),
            new CompletionData("LightingNWidth","Which widths should LightingN use for all columns individually?"),
            new CompletionData("LightingLWidth","Which widths should LightingL use for all columns individually?"),
            new CompletionData("WidthForNoteHeightScale","Which height should all notes have if columns have individual widths?"),
            new CompletionData("HitPosition","On which height should the judgement line be drawn at?"),
            new CompletionData("LightPosition","On which height should the stage lights be drawn at?"),
            new CompletionData("ScorePosition","On which height should the hitbursts appear at?"),
            new CompletionData("ComboPosition","On which height should the combo counter appear at?"),
            new CompletionData("JudgementLine","Should an additional line be drawn above the StageHint?"),
            new CompletionData("LightFramePerSecond","May be obsolete."),
            new CompletionData("SpecialStyle","What SpecialStyle is used for this keycount if available?"),
            new CompletionData("ComboBurstStyle","On what side should the comboburst appear?"),
            new CompletionData("SplitStages","Should the stage be split into 2 stages?"),
            new CompletionData("StageSeparation","What distance should the 2 stages have when splitted?"),
            new CompletionData("SeparateScore","Should the hitburst only be shown on the stage it was scored on?"),
            new CompletionData("KeysUnderNotes","Should the keys be covered by notes when passing them?"),
            new CompletionData("UpsideDown","Should the stage always be upside down?"),
            new CompletionData("KeyFlipWhenUpsideDown","Should all of the keys be flipped when the stage is flipped?"),
            new CompletionData("KeyFlipWhenUpsideDown#","Should the specified column's key be flipped when the stage is flipped?"),
            new CompletionData("NoteFlipWhenUpsideDown","Should all of the notes be flipped when the stage is flipped?"),
            new CompletionData("KeyFlipWhenUpsideDown#D","Should the column's pressed key be flipped when the stage is flipped?"),
            new CompletionData("NoteFlipWhenUpsideDown#","Should the column's note be flipped when the stage is flipped?"),
            new CompletionData("NoteFlipWhenUpsideDown#H","Should the column's hold note head be flipped when the stage is flipped?"),
            new CompletionData("NoteFlipWhenUpsideDown#L","Should the column's hold note body be flipped when the stage is flipped?"),
            new CompletionData("NoteFlipWhenUpsideDown#T","Should the column's hold note tail be flipped when the stage is flipped?"),
            new CompletionData("NoteBodyStyle","What style should be used for all hold note bodies?"),
            new CompletionData("NoteBodyStyle#","What style should be used for all hold note bodies?"),
            new CompletionData("Colour#","What colour should be used for the column's lane?"),
            new CompletionData("ColourLight#","What colour should be used for the column's lighting?"),
            new CompletionData("ColourColumnLine","What colour should be used for the column lines?"),
            new CompletionData("ColourBarline","What colour should be used for the bar seperator?"),
            new CompletionData("ColourJudgementLine","What colour should be used for the timing line?"),
            new CompletionData("ColourKeyWarning","What colour should be used for the keybinding reminders?"),
            new CompletionData("ColourHold","What colour should be used for the combo counter during holds?"),
            new CompletionData("ColourBreak","What colour should be used for the combo counter when it breaks?"),
            new CompletionData("KeyImage#","What is the name of the column's unpressed key image?"),
            new CompletionData("KeyImage#D","What is the name of the column's pressed key image?"),
            new CompletionData("NoteImage#","What is the name of the column's note image?"),
            new CompletionData("NoteImage#H","What is the name of the column's hold note head image?"),
            new CompletionData("NoteImage#L","What is the name of the column's hold note body image?"),
            new CompletionData("NoteImage#T","What is the name of the column's hold note tail image?"),
            new CompletionData("StageLeft","What is the name of the left stage image?"),
            new CompletionData("StageRight","What is the name of the right stage image?"),
            new CompletionData("StageBottom","What is the name of the bottom stage image?"),
            new CompletionData("StageHint","What is the name of the stage hint image?"),
            new CompletionData("StageLight","What is the name of the stage light image?"),
            new CompletionData("LightingN","What is the name of the note lighting image?"),
            new CompletionData("LightingL","What is the name of the hold note lighting image?"),
            new CompletionData("WarningArrow","What is the name of the warning arrow image?"),
            new CompletionData("Hit0","What is the name of the hit0 image?"),
            new CompletionData("Hit50","What is the name of the hit50 image?"),
            new CompletionData("Hit100","What is the name of the hit100 image?"),
            new CompletionData("Hit200","What is the name of the hit200 image?"),
            new CompletionData("Hit300","What is the name of the hit300 image?"),
            new CompletionData("Hit300g","What is the name of the hit300g image?") };
    }
}
