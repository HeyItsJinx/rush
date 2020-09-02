// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Rush.Edit.Blueprints;
using osu.Game.Rulesets.Rush.Objects;

namespace osu.Game.Rulesets.Rush.Edit
{
    public class SawbladeCompositionTool : HitObjectCompositionTool
    {
        public SawbladeCompositionTool()
            : base(nameof(Sawblade))
        {
        }

        public override PlacementBlueprint CreatePlacementBlueprint() => new SawbladePlacementBlueprint();
    }
}
