using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x0200006F RID: 111
	public class JobDriver_GotoForLovinAvali : JobDriver
	{
		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600022B RID: 555 RVA: 0x00003039 File Offset: 0x00001239
		private Pawn partner
		{
			get
			{
				return (Pawn)((Thing)this.job.GetTarget(this.partnerInd));
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600022C RID: 556 RVA: 0x00003056 File Offset: 0x00001256
		private Building bed
		{
			get
			{
				return (Building)((Thing)this.job.GetTarget(this.bedInd));
			}
		}

		// Token: 0x0600022D RID: 557 RVA: 0x00002187 File Offset: 0x00000387
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		// Token: 0x0600022E RID: 558 RVA: 0x00011D4C File Offset: 0x0000FF4C
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(this.partnerInd);
			this.FailOn(() => !this.partner.health.capacities.CanBeAwake);
			yield return Toils_Goto.GotoThing(this.partnerInd, PathEndMode.Touch);
			Room pawnRoom = this.pawn.GetRoom(RegionType.Set_Passable);
			List<Pawn> pawnsForLovin = this.pawn.FindPawnsForLovinInRoom(pawnRoom, false);
			if (pawnsForLovin != null)
			{
				bool rand = Rand.Bool;
				Toil wait = Toils_General.Wait(1, TargetIndex.None);
				wait.tickAction = delegate()
				{
					this.pawn.GainComfortFromCellIfPossible(false);
					if (this.partner.CurJobDef != JobDefOfAvali.LovinAvali && this.partner.CurJobDef != JobDefOfAvali.LovinAvaliPartner)
					{
						this.ReadyForNextToil();
						if (rand)
						{
							JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.EndJob, 1f, null);
							return;
						}
					}
					else
					{
						if (rand)
						{
							this.pawn.rotationTracker.Face(this.partner.DrawPos);
							JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.None, 1f, null);
							return;
						}
						if (this.pawn.IsHashIntervalTick(100))
						{
							this.pawn.rotationTracker.FaceCell(this.pawn.RandomAdjacentCell8Way());
						}
					}
				};
				wait.socialMode = RandomSocialMode.SuperActive;
				if (this.partner.CurJobDef == JobDefOfAvali.LovinAvali || this.partner.CurJobDef == JobDefOfAvali.LovinAvaliPartner)
				{
					if (rand)
					{
						base.ReportStringProcessed("Watching".Translate());
					}
					else
					{
						base.ReportStringProcessed("Waiting".Translate());
					}
					yield return wait;
				}
				Job newJob = new Job(JobDefOfAvali.LovinAvali, this.partner, this.bed);
				this.pawn.jobs.StartJob(newJob, JobCondition.Succeeded, null, false, true, null, null, false, false);
			}
			yield break;
		}

		// Token: 0x040001F7 RID: 503
		private const int ticksBeforeMote = 200;

		// Token: 0x040001F8 RID: 504
		private TargetIndex partnerInd = TargetIndex.A;

		// Token: 0x040001F9 RID: 505
		private TargetIndex bedInd = TargetIndex.B;
	}
}
