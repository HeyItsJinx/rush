// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Rush.Objects;

namespace osu.Game.Rulesets.Rush.Beatmaps.Converters
{
    internal class TaikoRulesetConverter : RulesetConverter
    {
        public const int RULESET_ID = 3;

        public TaikoRulesetConverter(IBeatmap original)
            : base(original)
        {
        }

        protected override LanedHitLane? LaneForHitObject(HitObject hitObject) => isRim(hitObject) ? LanedHitLane.Air : LanedHitLane.Ground;

        protected override HitObjectFlags FlagsForHitObject(HitObject hitObject, IBeatmap beatmap)
        {
            switch (hitObject)
            {
                case IHasDistance _:
                    return HitObjectFlags.ForceStartNotesheet;

                case IHasDuration _:
                    return HitObjectFlags.ForceMiniboss;

                default:
                    return isStrong(hitObject) ? HitObjectFlags.ForceDoubleHit : isRim(hitObject) ? HitObjectFlags.ForceAir : HitObjectFlags.ForceGround;
            }
        }

        private bool isStrong(HitObject hitObject) => hitObject.Samples.Any(s => s.Name == HitSampleInfo.HIT_FINISH);
        private bool isRim(HitObject hitObject) => hitObject.Samples.Any(s => s.Name == HitSampleInfo.HIT_CLAP || s.Name == HitSampleInfo.HIT_WHISTLE);
    }
}
