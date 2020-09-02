// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Rush.Edit.Blueprints;

namespace osu.Game.Rulesets.Rush.Edit
{
    public class DualHitCompositionTool : HitObjectCompositionTool
    {
        public DualHitCompositionTool()
            : base("Dual Hit")
        {
        }

        public override PlacementBlueprint CreatePlacementBlueprint() => new DualHitPlacementBlueprint();
    }
}
