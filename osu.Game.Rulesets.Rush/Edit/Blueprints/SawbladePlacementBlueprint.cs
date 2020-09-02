// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Rush.Objects;

namespace osu.Game.Rulesets.Rush.Edit.Blueprints
{
    public class SawbladePlacementBlueprint : RushPlacementBlueprint<Sawblade>
    {
        public SawbladePlacementBlueprint()
            : base(new Sawblade())
        {
        }
    }
}
