// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Rush.Scoring;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Rush.Objects
{
    public class RushHitObject : HitObject, IHasPosition
    {
        public override Judgement CreateJudgement() => new RushJudgement();

        protected override HitWindows CreateHitWindows() => new RushHitWindows();

        public virtual float X => 0;
        public virtual float Y => 0;
        public virtual Vector2 Position => Vector2.Zero;
    }
}
