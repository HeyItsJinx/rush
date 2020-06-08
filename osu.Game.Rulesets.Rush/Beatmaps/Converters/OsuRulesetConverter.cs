// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Rush.Objects;
using osuTK;

namespace osu.Game.Rulesets.Rush.Beatmaps.Converters
{
    internal class OsuRulesetConverter : RulesetConverter
    {
        private const float half_height = 200f;

        public OsuRulesetConverter(IBeatmap original)
            : base(original)
        {
        }

        protected override LanedHitLane? LaneForHitObject(HitObject hitObject) =>
            hitObject is IHasYPosition hasYPosition ? (LanedHitLane?)(hasYPosition.Y < half_height ? LanedHitLane.Air : LanedHitLane.Ground) : null;

        protected override HitObjectFlags FlagsForHitObject(HitObject hitObject, IBeatmap beatmap)
        {
            if (hitObject is IHasDuration && !(hitObject is IHasDistance))
                return HitObjectFlags.ForceMiniboss;

            HitObjectFlags flags = HitObjectFlags.None;

            // sliders should force a notesheet to start or end
            if (hitObject is IHasDuration hasDuration && hasDuration.Duration >= MIN_SHEET_LENGTH)
                flags |= HitObjectFlags.ForceStartNotesheet | HitObjectFlags.ForceEndNotesheet;

            var positionData = hitObject as IHasPosition;
            var newCombo = (hitObject as IHasCombo)?.NewCombo ?? false;

            float positionSeparation = ((positionData?.Position ?? Vector2.Zero) - (State.PreviousPosition ?? Vector2.Zero)).Length;
            double timeSeparation = hitObject.StartTime - State.PreviousEndTime;

            if (timeSeparation <= 80)
            {
                // more than 187 bpm
                flags |= newCombo ? HitObjectFlags.ForceNotSameLane : HitObjectFlags.ForceSameLane;
                flags |= HitObjectFlags.AllowSawbladeAdd;
            }
            else if (timeSeparation <= 105)
            {
                // more than 140 bpm
                flags |= newCombo ? HitObjectFlags.ForceNotSameLane : HitObjectFlags.SuggestNotSameLane;
                // flags |= HitObjectFlags.LowProbability;
                flags |= HitObjectFlags.AllowSawbladeAdd;
                flags |= HitObjectFlags.ForceEndNotesheet;
            }
            else if (timeSeparation <= 125)
            {
                // more than 120 bpm
                flags |= newCombo ? HitObjectFlags.ForceNotSameLane : HitObjectFlags.SuggestNotSameLane;
                flags |= HitObjectFlags.AllowSawbladeAdd;
                flags |= HitObjectFlags.ForceEndNotesheet;
            }
            else if (timeSeparation <= 135 && positionSeparation < 20)
            {
                // more than 111 bpm stream
                flags |= newCombo ? HitObjectFlags.ForceNotSameLane : HitObjectFlags.ForceSameLane;
                flags |= HitObjectFlags.AllowSawbladeAdd;
                flags |= HitObjectFlags.ForceEndNotesheet;
            }
            else
            {
                flags |= newCombo ? HitObjectFlags.ForceNotSameLane : HitObjectFlags.ForceSameLane;
                // flags |= HitObjectFlags.LowProbability;
                flags |= HitObjectFlags.AllowDoubleHit;
                flags |= HitObjectFlags.AllowSawbladeAdd;
                flags |= HitObjectFlags.ForceEndNotesheet;
                // flags |= HitObjectFlags.AllowSawbladeReplace; FIXME: for now, always add rather than replace
            }

            // new combo should never be low probability
            if (newCombo) flags &= ~HitObjectFlags.LowProbability;

            // new combo should force note sheets to end
            if (newCombo) flags |= HitObjectFlags.ForceEndNotesheet;

            return flags;
        }
    }
}
