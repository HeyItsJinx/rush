// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Types;
using osuTK;

namespace osu.Game.Rulesets.Rush.Objects
{
    public class LanedHit : RushHitObject, IHasPosition
    {
        public readonly Bindable<LanedHitLane> LaneBindable = new Bindable<LanedHitLane>();

        public virtual LanedHitLane Lane
        {
            get => LaneBindable.Value;
            set => LaneBindable.Value = value;
        }

        public override Judgement CreateJudgement() => new RushJudgement();

        public override float X => 0;
        public override float Y => Lane == LanedHitLane.Air ? 0 : 400;
        public override Vector2 Position => new Vector2(X, Y);
    }
}
