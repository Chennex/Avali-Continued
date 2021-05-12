using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x0200005D RID: 93
	public class JobDriver_UseRunningTrack : JobDriver
	{
		// Token: 0x060001E5 RID: 485 RVA: 0x0000E4C8 File Offset: 0x0000C6C8
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.job.targetA, this.job, this.job.def.joyMaxParticipants, 0, null, true) && this.pawn.Reserve(this.job.targetB, this.job, 1, -1, null, true);
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x00010280 File Offset: 0x0000E480
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.EndOnDespawnedOrNull(TargetIndex.A, JobCondition.Incompletable);
			this.EndOnDespawnedOrNull(TargetIndex.B, JobCondition.Incompletable);
			this.FailOnForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.OnCell);
			float statValue = base.TargetThingA.GetStatValue(StatDefOf.JoyGainFactor, true);
			Building building = (Building)this.pawn.CurJob.targetA.Thing;
			Toil use = new Toil();
			use.tickAction = delegate()
			{
				this.pawn.rotationTracker.FaceCell(this.TargetA.Cell);
				this.pawn.GainComfortFromCellIfPossible(false);
				building.GetComp<CompMannable>().ManForATick(this.pawn);
				if (this.TargetC.IsValid)
				{
					JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.None, statValue, null);
					return;
				}
				JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.EndJob, statValue, null);
			};
			if (!base.TargetC.IsValid)
			{
				use.defaultCompleteMode = ToilCompleteMode.Delay;
				use.defaultDuration = this.job.def.joyDuration;
			}
			else
			{
				use.defaultCompleteMode = ToilCompleteMode.Never;
			}
			use.handlingFacing = true;
			use.AddFinishAction(delegate
			{
				JoyUtility.TryGainRecRoomThought(this.pawn);
			});
			yield return use;
			yield break;
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000E548 File Offset: 0x0000C748
		public override object[] TaleParameters()
		{
			return new object[]
			{
				this.pawn,
				base.TargetA.Thing.def
			};
		}
	}
}
