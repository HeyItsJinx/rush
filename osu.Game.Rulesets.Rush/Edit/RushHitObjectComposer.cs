// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.Rush.Edit
{
    public class RushHitObjectComposer : HitObjectComposer<RushHitObject>
    {
        private DrawableRushEditRuleset drawableRuleset;

        public new RushPlayfield Playfield => drawableRuleset.Playfield;

        public IScrollingInfo ScrollingInfo => drawableRuleset.ScrollingInfo;

        public RushHitObjectComposer(RushRuleset ruleset)
            : base(ruleset)
        {
        }

        protected override IReadOnlyList<HitObjectCompositionTool> CompositionTools => new HitObjectCompositionTool[]
        {
            new MinionCompositionTool(),
            new NoteSheetCompositionTool(),
            new DualHitCompositionTool(),
            new MiniBossCompositionTool(),
            new SawbladeCompositionTool(),
            new HeartCompositionTool(),
        };

        protected override DrawableRuleset<RushHitObject> CreateDrawableRuleset(Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
        {
            drawableRuleset = new DrawableRushEditRuleset(ruleset, beatmap, mods);

            return drawableRuleset;
        }

        protected override ComposeBlueprintContainer CreateBlueprintContainer(IEnumerable<DrawableHitObject> hitObjects)
            => new RushBlueprintContainer(hitObjects);
    }
}
