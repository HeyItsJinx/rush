// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.Rush.Edit
{
    public class RushHitObjectComposer : HitObjectComposer<RushHitObject>
    {
        public RushHitObjectComposer(RushRuleset ruleset)
            : base(ruleset)
        {
        }

        protected override IReadOnlyList<HitObjectCompositionTool> CompositionTools => new HitObjectCompositionTool[]
        {
            // new HitCompositionTool(),
            // new DrumRollCompositionTool(),
            // new SwellCompositionTool()
        };

        protected override ComposeBlueprintContainer CreateBlueprintContainer(IEnumerable<DrawableHitObject> hitObjects)
            => new RushBlueprintContainer(hitObjects);
    }
}
