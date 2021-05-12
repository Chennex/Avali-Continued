using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000074 RID: 116
	public class JobGiver_GotoForLovinAvali : ThinkNode_JobGiver
	{
		// Token: 0x06000250 RID: 592 RVA: 0x000126B8 File Offset: 0x000108B8
		protected override Job TryGiveJob(Pawn pawn)
		{
			if (pawn.def != ThingDefOfAvali.Avali)
			{
				return null;
			}
			if (Find.TickManager.TicksGame < pawn.mindState.canLovinTick)
			{
				return null;
			}
			Predicate<Thing> validator = delegate(Thing t)
			{
				Pawn pawn3 = t as Pawn;
				return !pawn3.Downed && !pawn3.IsForbidden(pawn) && !pawn3.health.HasHediffsNeedingTend(false) && pawn3.InBed() && !pawn.health.HasHediffsNeedingTend(false) && !pawn.Drafted && LovePartnerRelationUtility.LovePartnerRelationExists(pawn, pawn3) && Find.TickManager.TicksGame >= pawn3.mindState.canLovinTick && pawn3.RaceProps.Humanlike == pawn.RaceProps.Humanlike;
			};
			Pawn pawn2 = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(pawn.def), PathEndMode.Touch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 30f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
			if (pawn2 == null)
			{
				return null;
			}
			Building_Bed building_Bed = RestUtility.FindBedFor(pawn2);
			if (building_Bed != null)
			{
				if (pawn2.CurrentBed() != building_Bed)
				{
					return null;
				}
				if (!building_Bed.Medical)
				{
					return new Job(JobDefOfAvali.GotoForLovinAvali, pawn2, building_Bed);
				}
			}
			return null;
		}
	}
}
