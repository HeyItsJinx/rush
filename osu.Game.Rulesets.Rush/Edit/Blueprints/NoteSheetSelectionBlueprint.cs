// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Rush.Objects.Drawables;

namespace osu.Game.Rulesets.Rush.Edit.Blueprints
{
    public class NoteSheetSelectionBlueprint : RushSelectionBlueprint
    {
        public new DrawableNoteSheet DrawableObject => (DrawableNoteSheet)base.DrawableObject;

        public NoteSheetSelectionBlueprint(DrawableNoteSheet noteSheet)
            : base(noteSheet)
        {
        }
    }
}
