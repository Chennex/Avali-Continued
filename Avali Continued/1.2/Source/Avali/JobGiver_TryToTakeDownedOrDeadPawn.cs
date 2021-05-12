using System;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x0200001C RID: 28
	public class JobGiver_TryToTakeDownedOrDeadPawn : ThinkNode_JobGiver
	{
		// Token: 0x0600007E RID: 126 RVA: 0x00005F14 File Offset: 0x00004114
		protected override Job TryGiveJob(Pawn pawn)
		{
			if (pawn.IsColonist || pawn.MentalState == null)
			{
				return null;
			}
			if (pawn.MentalState.def == null || pawn.MentalState.def != MentalStateDefOf.PanicFlee)
			{
				return null;
			}
			IntVec3 c;
			if (!RCellFinder.TryFindBestExitSpot(pawn, out c, TraverseMode.ByPawn))
			{
				return null;
			}
			Hediff_AvaliBiology avaliBiologyHediff = pawn.health.hediffSet.GetHediffs<Hediff_AvaliBiology>().First<Hediff_AvaliBiology>();
			Predicate<Thing> validator;
			Pawn pawn2;
			if (avaliBiologyHediff != null)
			{
				validator = delegate(Thing t)
				{
					Pawn pawn3 = t as Pawn;
					return pawn3.RaceProps.Humanlike && pawn3.PawnListed(avaliBiologyHediff.packPawns) && pawn3.Downed && pawn.CanReserve(pawn3, 1, -1, null, false);
				};
				pawn2 = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), this.maxSearchDistance, validator, null, 0, -1, false, RegionType.Set_Passable, false);
				if (pawn2 != null)
				{
					return new Job(JobDefOfAvali.TakeDownedOrDeadPawn)
					{
						targetA = pawn2,
						targetB = c,
						count = 1
					};
				}
			}
			validator = delegate(Thing t)
			{
				Pawn pawn3 = t as Pawn;
				return pawn3.RaceProps.Humanlike && pawn3.Faction == pawn.Faction && pawn3.Downed && pawn.CanReserve(pawn3, 1, -1, null, false);
			};
			pawn2 = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), this.maxSearchDistance, validator, null, 0, -1, false, RegionType.Set_Passable, false);
			if (pawn2 != null)
			{
				return new Job(JobDefOfAvali.TakeDownedOrDeadPawn)
				{
					targetA = pawn2,
					targetB = c,
					count = 1
				};
			}
			validator = delegate(Thing t)
			{
				Corpse corpse2 = t as Corpse;
				return corpse2 != null && corpse2.InnerPawn != null && corpse2.InnerPawn.RaceProps.Humanlike && corpse2.InnerPawn.PawnListed(avaliBiologyHediff.packPawns) && pawn.CanReserve(corpse2, 1, -1, null, false);
			};
			Corpse corpse = (Corpse)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Corpse), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), this.maxSearchDistance, validator, null, 0, -1, false, RegionType.Set_Passable, false);
			if (corpse != null)
			{
				Job job = new Job(JobDefOfAvali.TakeDownedOrDeadPawn);
				job.targetA = corpse;
				job.targetB = c;
				job.count = 1;
			}
			return null;
		}

		// Token: 0x04000060 RID: 96
		public float maxSearchDistance = 15f;
	}
}
