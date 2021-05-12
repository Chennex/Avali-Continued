using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x0200004E RID: 78
	public class JobDriver_SingsSocial : JobDriver
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060001A7 RID: 423 RVA: 0x0000E9E4 File Offset: 0x0000CBE4
		private Thing GatherSpotParent
		{
			get
			{
				return this.job.GetTarget(TargetIndex.A).Thing;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x0000EA08 File Offset: 0x0000CC08
		private bool HasChair
		{
			get
			{
				return this.job.GetTarget(TargetIndex.B).HasThing;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060001A9 RID: 425 RVA: 0x0000EA2C File Offset: 0x0000CC2C
		private bool HasDrink
		{
			get
			{
				return this.job.GetTarget(TargetIndex.C).HasThing;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060001AA RID: 426 RVA: 0x0000EA50 File Offset: 0x0000CC50
		private IntVec3 ClosestGatherSpotParentCell
		{
			get
			{
				return this.GatherSpotParent.OccupiedRect().ClosestCellTo(this.pawn.Position);
			}
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000EA7C File Offset: 0x0000CC7C
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.job.GetTarget(TargetIndex.B), this.job, 1, -1, null, true) && (!this.HasDrink || this.pawn.Reserve(this.job.GetTarget(TargetIndex.C), this.job, 1, -1, null, true));
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000EADC File Offset: 0x0000CCDC
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.EndOnDespawnedOrNull(TargetIndex.A, JobCondition.Incompletable);
			if (this.HasChair)
			{
				this.EndOnDespawnedOrNull(TargetIndex.B, JobCondition.Incompletable);
			}
			if (this.HasDrink)
			{
				this.FailOnDestroyedNullOrForbidden(TargetIndex.C);
				yield return Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.OnCell).FailOnSomeonePhysicallyInteracting(TargetIndex.C);
				yield return Toils_Haul.StartCarryThing(TargetIndex.C, false, false, false);
			}
			yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
			Toil chew = new Toil();
			chew.tickAction = delegate()
			{
				this.pawn.rotationTracker.FaceCell(this.ClosestGatherSpotParentCell);
				this.pawn.GainComfortFromCellIfPossible(false);
				JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.GoToNextToil, 1f, null);
				if (this.pawn.IsHashIntervalTick(100))
				{
					MoteMaker.ThrowMetaIcon(this.pawn.Position, this.pawn.Map, ThingDefOfAvali.Mote_Note);
				}
			};
			chew.handlingFacing = true;
			chew.defaultCompleteMode = ToilCompleteMode.Delay;
			chew.defaultDuration = this.job.def.joyDuration;
			chew.AddFinishAction(delegate
			{
				JoyUtility.TryGainRecRoomThought(this.pawn);
			});
			chew.socialMode = RandomSocialMode.SuperActive;
			Toils_Ingest.AddIngestionEffects(chew, this.pawn, TargetIndex.C, TargetIndex.None);
			yield return chew;
			if (this.HasDrink)
			{
				yield return Toils_Ingest.FinalizeIngest(this.pawn, TargetIndex.C);
			}
			yield break;
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000EAFC File Offset: 0x0000CCFC
		public override bool ModifyCarriedThingDrawPos(ref Vector3 drawPos, ref bool behind, ref bool flip)
		{
			IntVec3 closestGatherSpotParentCell = this.ClosestGatherSpotParentCell;
			return JobDriver_Ingest.ModifyCarriedThingDrawPosWorker(ref drawPos, ref behind, ref flip, closestGatherSpotParentCell, this.pawn);
		}

		// Token: 0x04000164 RID: 356
		private const TargetIndex GatherSpotParentInd = TargetIndex.A;

		// Token: 0x04000165 RID: 357
		private const TargetIndex ChairOrSpotInd = TargetIndex.B;

		// Token: 0x04000166 RID: 358
		private const TargetIndex OptionalIngestibleInd = TargetIndex.C;
	}
}
