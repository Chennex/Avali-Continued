using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000049 RID: 73
	public class JobDriver_SingsAlone : JobDriver
	{
		// Token: 0x06000187 RID: 391 RVA: 0x00002C1E File Offset: 0x00000E1E
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, true);
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000E2C4 File Offset: 0x0000C4C4
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
			yield return new Toil
			{
				initAction = delegate()
				{
					this.faceDir = ((!this.job.def.faceDir.IsValid) ? Rot4.Random : this.job.def.faceDir);
				},
				tickAction = delegate()
				{
					this.pawn.rotationTracker.FaceCell(this.pawn.Position + this.faceDir.FacingCell);
					this.pawn.GainComfortFromCellIfPossible(false);
					JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.EndJob, 1f, null);
					if (this.pawn.IsHashIntervalTick(100))
					{
						MoteMaker.ThrowMetaIcon(this.pawn.Position, this.pawn.Map, ThingDefOfAvali.Mote_Note);
					}
				},
				handlingFacing = true,
				defaultCompleteMode = ToilCompleteMode.Delay,
				defaultDuration = this.job.def.joyDuration
			};
			yield break;
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000E2E4 File Offset: 0x0000C4E4
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<Rot4>(ref this.faceDir, "faceDir", default(Rot4), false);
		}

		// Token: 0x04000154 RID: 340
		private Rot4 faceDir;
	}
}
