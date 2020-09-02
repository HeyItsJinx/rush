// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Rush.Edit.Blueprints.Components;
using osu.Game.Rulesets.Rush.Objects;

namespace osu.Game.Rulesets.Rush.Edit.Blueprints
{
    public class MinionPlacementBlueprint : LanedPlacementBlueprint<Minion>
    {
        protected override CompositeDrawable CreatePiece() => new EditMinionPiece();

        public MinionPlacementBlueprint()
            : base(new Minion())
        {
        }
    }
}
