// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Rush.Objects;

namespace osu.Game.Rulesets.Rush.Edit.Blueprints
{
    public class RushPlacementBlueprint<T> : PlacementBlueprint
        where T : RushHitObject
    {
        protected new T HitObject => (T)base.HitObject;

        public RushPlacementBlueprint(HitObject hitObject)
            : base(hitObject)
        {
        }
    }
}
