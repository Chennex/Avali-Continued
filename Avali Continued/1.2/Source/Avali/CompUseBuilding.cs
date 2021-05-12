using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000034 RID: 52
	public class CompUseBuilding : CompMannable
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000115 RID: 277 RVA: 0x000029CD File Offset: 0x00000BCD
		public new CompProperties_UseBuilding Props
		{
			get
			{
				return (CompProperties_UseBuilding)this.props;
			}
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000A4BC File Offset: 0x000086BC
		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn pawn)
		{
			if (!pawn.Drafted && pawn.RaceProps.ToolUser && pawn.CanReserveAndReach(this.parent, PathEndMode.InteractionCell, Danger.Deadly, 1, -1, null, false) && (this.Props.workType == WorkTags.None || pawn.story == null || pawn.story.DisabledWorkTagsBackstoryAndTraits != this.Props.workType))
			{
				FloatMenuOption opt = new FloatMenuOption(this.Props.floatMenuText.Translate(this.parent.LabelShort), delegate()
				{
					Job job = new Job(this.Props.useJob, this.parent, this.parent.InteractionCell, this.parent);
					pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
				}, MenuOptionPriority.Default, null, null, 0f, null, null);
				yield return opt;
			}
			yield break;
		}
	}
}
