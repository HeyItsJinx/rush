// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Rush.Objects;
using osuTK;

namespace osu.Game.Rulesets.Rush.Beatmaps
{
    public abstract class RulesetConverter : IRulesetConverter
    {
        protected const double SKIP_PROBABILITY = 0.1;
        protected const double SAWBLADE_PROBABILITY = 0.1;
        protected const double ORB_PROBABILITY = 0.2;
        protected const double SUGGEST_PROBABILITY = 0.1;
        protected const double NOTESHEET_START_PROBABILITY = 0.5;
        protected const double NOTESHEET_END_PROBABILITY = 0.2;
        protected const double NOTESHEET_DUAL_PROBABILITY = 0.1;
        protected const double KIAI_MULTIPLIER = 4;

        protected const double SAWBLADE_SAME_LANE_SAFETY_TIME = 90;
        protected const double SAWBLADE_FALL_SAFETY_NEAR_TIME = 80;
        protected const double SAWBLADE_FALL_SAFETY_FAR_TIME = 600;
        protected const double MIN_SAWBLADE_TIME = 500;
        protected const double MIN_HEART_TIME = 30000;
        protected const double MIN_ORB_TIME = 500;
        protected const double MAX_SHEET_LENGTH = 2000;
        protected const double MIN_SHEET_LENGTH = 120;
        protected const double MIN_REPEAT_TIME = 100;

        protected ConversionState State = new ConversionState();

        protected RulesetConverter(IBeatmap original)
        {
            var firstObject = original.HitObjects.FirstOrDefault()?.StartTime ?? 0;
            State.NextHeartTime = firstObject + MIN_HEART_TIME;
            State.NextDualOrbTime = firstObject + MIN_ORB_TIME;
            State.NextSawbladeTime = firstObject + MIN_SAWBLADE_TIME;
        }

        public IEnumerable<RushHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap)
        {
            HitObjectFlags flags = FlagsForHitObject(original, beatmap);

            // if no flags, completely skip this object
            if (flags == HitObjectFlags.None)
            {
                State.Update(null, null, HitObjectFlags.None);
                yield break;
            }

            // create a miniboss if required
            if (flags.HasFlag(HitObjectFlags.ForceMiniboss))
            {
                yield return CreateMiniBoss(original);

                State.Update(original, null, flags);

                yield break;
            }

            Random random = new Random((int)original.StartTime);
            LanedHitLane? lane = null;
            var kiaiMultiplier = original.Kiai ? KIAI_MULTIPLIER : 1;

            // try to get a lane from the force flags
            if (flags.HasFlag(HitObjectFlags.ForceSameLane) || flags.HasFlag(HitObjectFlags.SuggestSameLane) && random.NextDouble() < SUGGEST_PROBABILITY)
                lane = State.PreviousLane;
            else if (flags.HasFlag(HitObjectFlags.ForceNotSameLane) || flags.HasFlag(HitObjectFlags.SuggestNotSameLane) && random.NextDouble() < SUGGEST_PROBABILITY)
                lane = State.PreviousLane?.Opposite();

            // get the lane from the object
            lane ??= LaneForHitObject(original);

            // if we should end a sheet, try to
            if (State.CurrentNoteSheets.Count > 0 && (flags.HasFlag(HitObjectFlags.ForceEndNotesheet) || flags.HasFlag(HitObjectFlags.SuggestEndNotesheet) && random.NextDouble() < NOTESHEET_END_PROBABILITY))
            {
                // TODO: for now we'll end both sheets where they are and ignore snapping logic
                State.CurrentNoteSheets.Clear();
            }

            // if we should start a notesheet...
            if (flags.HasFlag(HitObjectFlags.ForceStartNotesheet) || flags.HasFlag(HitObjectFlags.SuggestStartNotesheet) && random.NextDouble() < NOTESHEET_START_PROBABILITY)
            {
                // TODO: for now, end all existing sheets
                State.CurrentNoteSheets.Clear();

                // use the suggested lane or randomly select one
                LanedHitLane sheetLane = lane ?? (random.NextDouble() < 0.5 ? LanedHitLane.Ground : LanedHitLane.Air);

                // create a sheet
                NoteSheet sheet = State.CurrentNoteSheets[sheetLane] = CreateNoteSheet(original, sheetLane, original.Samples);
                LanedHitLane otherLane = sheetLane.Opposite();

                // FIXME: surely this is bad, altering the hit object after it's been returned???
                if (sheet != null)
                    yield return sheet;

                // for sliders with repeats, add extra objects to the lane without a sheet
                if (original is IHasRepeats hasRepeats && hasRepeats.RepeatCount > 0)
                {
                    var duration = original.GetEndTime() - original.StartTime;
                    var repeatDuration = duration / hasRepeats.SpanCount();
                    var skip = 1;

                    // Currently an issue where an odd number of repeats (span count) will skip
                    // the final minion if repeats are too short. Not sure what to do here since
                    // it doesn't make rhythmic sense to add an extra hit object.
                    // Examples:
                    //   *-*-*-*-* becomes *---*---* (good)
                    //   *-*-*-*   becomes *---*-- (looks bad) instead of *---*-* (rhythmically worse)
                    while (repeatDuration < MIN_REPEAT_TIME)
                    {
                        repeatDuration *= 2;
                        skip *= 2;
                    }

                    var repeatCurrent = original.StartTime;
                    var index = -1;

                    foreach (var nodeSample in hasRepeats.NodeSamples)
                    {
                        index++;

                        if (index % skip != 0)
                            continue;

                        yield return CreateNormalHit(original, otherLane, nodeSample, repeatCurrent);

                        repeatCurrent += repeatDuration;
                    }
                }
                // otherwise we have a chance to make a dual sheet
                else if (random.NextDouble() < NOTESHEET_DUAL_PROBABILITY)
                {
                    State.CurrentNoteSheets[otherLane] = CreateNoteSheet(original, otherLane, null);
                    yield return State.CurrentNoteSheets[otherLane];
                }

                State.Update(null, sheetLane, flags);
                yield break;
            }

            // if either of the sheets are too long, end them where they are
            if (State.CurrentNoteSheets.ContainsKey(LanedHitLane.Air) && State.CurrentNoteSheets[LanedHitLane.Air].Duration >= MAX_SHEET_LENGTH)
                State.CurrentNoteSheets.Remove(LanedHitLane.Air);

            if (State.CurrentNoteSheets.ContainsKey(LanedHitLane.Ground) && State.CurrentNoteSheets[LanedHitLane.Ground].Duration >= MAX_SHEET_LENGTH)
                State.CurrentNoteSheets.Remove(LanedHitLane.Ground);

            // if it's low probability, potentially skip this object
            if (flags.HasFlag(HitObjectFlags.LowProbability) && random.NextDouble() < SKIP_PROBABILITY)
            {
                State.Update(null, lane ?? State.PreviousLane, flags);
                yield break;
            }

            // if not too close to a sawblade, allow adding a double hit
            if (original.StartTime - State.LastSawbladeTime >= SAWBLADE_SAME_LANE_SAFETY_TIME
                && flags.HasFlag(HitObjectFlags.AllowDoubleHit)
                && original.StartTime >= State.NextDualOrbTime
                && random.NextDouble() < ORB_PROBABILITY)
            {
                State.NextDualOrbTime = original.StartTime + MIN_ORB_TIME;
                yield return CreateDualHit(original);

                State.Update(original, null, flags);
                yield break;
            }

            // if we still haven't selected a lane at this point, pick a random one
            var finalLane = lane ?? (random.NextDouble() < 0.5 ? LanedHitLane.Ground : LanedHitLane.Air);

            // check if a lane is blocked by a notesheet
            LanedHitLane? blockedLane = State.CurrentNoteSheets.ContainsKey(LanedHitLane.Air)
                ? LanedHitLane.Air
                : State.CurrentNoteSheets.ContainsKey(LanedHitLane.Ground)
                    ? LanedHitLane.Ground
                    : (LanedHitLane?)null;

            if (blockedLane != null && finalLane == blockedLane)
                finalLane = blockedLane.Value.Opposite();

            var timeSinceLastSawblade = original.StartTime - State.LastSawbladeTime;
            var tooCloseToLastSawblade = lane == State.LastSawbladeLane && timeSinceLastSawblade < SAWBLADE_SAME_LANE_SAFETY_TIME;

            bool sawbladeAdded = false;

            // if we are allowed to add or replace a sawblade, potentially do it
            if ((flags & HitObjectFlags.AllowSawbladeAddOrReplace) != 0
                && original.StartTime >= State.NextSawbladeTime
                && kiaiMultiplier * random.NextDouble() < SAWBLADE_PROBABILITY)
            {
                // the sawblade will always appear in the opposite lane to where the player is expected to hit
                var sawbladeLane = finalLane.Opposite();

                // if the sawblade time is less than twice the minimum gap, it must be in the opposite lane to its previous one
                if (original.StartTime - State.NextSawbladeTime < 2 * MIN_SAWBLADE_TIME)
                    sawbladeLane = State.LastSawbladeLane?.Opposite() ?? LanedHitLane.Ground;

                // if the new sawblade is too close to the previous hit in the same lane, skip it
                var tooCloseToSameLane = State.PreviousLane == null || State.PreviousLane == sawbladeLane && original.StartTime - State.PreviousStartTime < SAWBLADE_SAME_LANE_SAFETY_TIME;

                // if a ground sawblade is too far from the previous hit in the air lane, skip it (as the player may not have time to jump upon landing)
                var canFallOntoSawblade = State.PreviousLane == LanedHitLane.Air && sawbladeLane == LanedHitLane.Ground && original.StartTime - State.PreviousStartTime > SAWBLADE_FALL_SAFETY_NEAR_TIME
                                          && original.StartTime - State.PreviousStartTime < SAWBLADE_FALL_SAFETY_FAR_TIME;

                // air sawblades may only appear in a kiai section, and not too close to a hit in the same lane (or laneless)
                // also need to account for a gap where the player may fall onto the blade
                if (sawbladeLane != blockedLane
                    && !tooCloseToSameLane
                    // && !canFallOntoSawblade FIXME: ignore for now since we're only adding sawblades, not replacing
                    && (sawbladeLane == LanedHitLane.Ground || original.Kiai))
                {
                    sawbladeAdded = true;
                    State.LastSawbladeLane = sawbladeLane;
                    State.LastSawbladeTime = original.StartTime;
                    State.NextSawbladeTime = original.StartTime + MIN_SAWBLADE_TIME;

                    // add a sawblade
                    yield return CreateSawblade(original, sawbladeLane);

                    // absolutely need to make sure that we never try to add a hit to the same lane as the sawblade that was just added
                    finalLane = sawbladeLane.Opposite();
                }
            }

            // we can add a regular hit if:
            //   we didn't add a sawblade, or
            //   we added a sawblade and are allowed to replace the hit entirely, or
            //   we added a sawblade that was in the opposite lane
            if (finalLane != blockedLane && !tooCloseToLastSawblade && (!sawbladeAdded || !flags.HasFlag(HitObjectFlags.AllowSawbladeReplace)))
                yield return CreateNormalHit(original, finalLane);

            State.Update(original, finalLane, flags);
        }

        protected LanedHit CreateNormalHit(HitObject original, LanedHitLane lane, IList<HitSampleInfo> samples = null, double? time = null)
        {
            time ??= original.StartTime;
            samples ??= original.Samples;

            // if it's time to add a heart, we must do so
            if (time >= State.NextHeartTime)
            {
                State.NextHeartTime += MIN_HEART_TIME;

                return new Heart
                {
                    StartTime = time.Value,
                    Samples = samples,
                    Lane = lane,
                };
            }

            return new Minion
            {
                StartTime = time.Value,
                Samples = samples,
                Lane = lane,
            };
        }

        protected MiniBoss CreateMiniBoss(HitObject original) =>
            new MiniBoss
            {
                StartTime = original.StartTime,
                EndTime = original.GetEndTime(),
                Samples = original.Samples
            };

        protected NoteSheet CreateNoteSheet(HitObject original, LanedHitLane lane, IList<HitSampleInfo> samples) =>
            new NoteSheet
            {
                StartTime = original.StartTime,
                EndTime = original.GetEndTime(),
                Samples = samples ?? new List<HitSampleInfo>(),
                Lane = lane
            };

        protected DualHit CreateDualHit(HitObject original) =>
            new DualHit
            {
                StartTime = original.StartTime,
                Samples = original.Samples
            };

        protected Sawblade CreateSawblade(HitObject original, LanedHitLane lane) =>
            new Sawblade
            {
                StartTime = original.StartTime,
                Lane = lane
            };

        protected abstract LanedHitLane? LaneForHitObject(HitObject hitObject);

        protected abstract HitObjectFlags FlagsForHitObject(HitObject hitObject, IBeatmap beatmap);

        [Flags]
        public enum HitObjectFlags
        {
            None = 0,

            /// <summary>
            /// Ensures that the next object will be in the same lane.
            /// </summary>
            ForceSameLane = 1 << 0,

            /// <summary>
            /// Ensures that the next object will not be in the same lane.
            /// </summary>
            ForceNotSameLane = 1 << 1,

            /// <summary>
            /// Indicates that the next object should consider staying in the same lane.
            /// </summary>
            SuggestSameLane = 1 << 2,

            /// <summary>
            /// Indicates that the next object should consider changing lanes.
            /// </summary>
            SuggestNotSameLane = 1 << 3,

            /// <summary>
            /// Indicates that the next object will be less likely to appear.
            /// </summary>
            LowProbability = 1 << 4,

            /// <summary>
            /// Indicates that the next object may be replaced with double hit.
            /// </summary>
            AllowDoubleHit = 1 << 5,

            /// <summary>
            /// Indicates that the next object may be completely replaced with a sawblade in the opposite lane.
            /// </summary>
            AllowSawbladeReplace = 1 << 6,

            /// <summary>
            /// Indicates that the next object may additionally add a sawblade to the opposite lane.
            /// </summary>
            AllowSawbladeAdd = 1 << 7,

            ForceStartNotesheet = 1 << 8,
            ForceEndNotesheet = 1 << 9,
            SuggestStartNotesheet = 1 << 10,
            SuggestEndNotesheet = 1 << 11,

            ForceMiniboss = 1 << 12,

            AllowSawbladeAddOrReplace = AllowSawbladeAdd | AllowSawbladeReplace,
        }

        public class ConversionState
        {
            #region Previous HitObject

            public Vector2? PreviousPosition;
            public double? PreviousDuration;
            public double? PreviousDistance;
            public double PreviousStartTime;
            public double PreviousEndTime;
            public bool PreviousKiai;
            public LanedHitLane? PreviousLane;
            public HitObjectFlags PreviousFlags;

            #endregion

            #region Tracking Last/Next Objects

            public double LastSawbladeTime;
            public LanedHitLane? LastSawbladeLane;
            public double NextHeartTime;
            public double NextDualOrbTime;
            public double NextSawbladeTime;

            #endregion

            #region Tracking Current Objects

            public readonly Dictionary<LanedHitLane, NoteSheet> CurrentNoteSheets = new Dictionary<LanedHitLane, NoteSheet>();

            #endregion

            public void Update(HitObject hitObject, LanedHitLane? lane, HitObjectFlags flags)
            {
                if (hitObject != null)
                {
                    var x = (hitObject as IHasXPosition)?.X;
                    var y = (hitObject as IHasYPosition)?.Y;

                    PreviousPosition = x == null && y == null ? (Vector2?)null : new Vector2(x ?? 0, y ?? 0);
                    PreviousDuration = (hitObject as IHasDuration)?.Duration;
                    PreviousDistance = (hitObject as IHasDistance)?.Distance;

                    PreviousStartTime = hitObject.StartTime;
                    PreviousEndTime = hitObject.GetEndTime();

                    PreviousKiai = hitObject.Kiai;
                }

                PreviousFlags = flags;
                PreviousLane = lane;
            }
        }
    }
}
