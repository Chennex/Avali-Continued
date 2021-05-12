using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x0200000A RID: 10
	public class JobDriver_FindPartnerForLovinAvali : JobDriver
	{
		// Token: 0x06000028 RID: 40 RVA: 0x00002187 File Offset: 0x00000387
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00003A4C File Offset: 0x00001C4C
		protected override IEnumerable<Toil> MakeNewToils()
		{
			Room room = this.pawn.GetRoom(RegionType.Set_Passable);
			List<Pawn> list = this.pawn.FindPawnsForLovinInRoom(room, false);
			if (list != null)
			{
				List<Pawn> list2 = null;
				for (int i = 0; i < list.Count; i++)
				{
					Pawn pawn = list[i];
					if (!pawn.CurJobDef.isIdle || pawn.CurJobDef.joyGainRate > 0f)
					{
						list2.Add(pawn);
					}
				}
				if (list2 == null)
				{
					for (int j = 0; j < list.Count; j++)
					{
						Pawn pawn2 = list[j];
						if (pawn2.CurJobDef == JobDefOfAvali.LovinAvali || pawn2.CurJobDef == JobDefOfAvali.LovinAvaliPartner)
						{
							list2.Add(pawn2);
						}
					}
				}
				if (list2 != null)
				{
					Pawn bestPartnerForLovin = this.GetBestPartnerForLovin(list2);
					if (bestPartnerForLovin != null)
					{
						Building building = RestUtility.FindBedFor(bestPartnerForLovin);
						if (building == null || !this.pawn.CanReserveAndReach(building, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, false))
						{
							building = RestUtility.FindBedFor(this.pawn);
							if (building == null || !this.pawn.CanReserveAndReach(building, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, false))
							{
								building = null;
								List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
								float num = 0f;
								for (int k = 0; k < containedAndAdjacentThings.Count; k++)
								{
									Building building2 = containedAndAdjacentThings[k] as Building;
									if (building2 != null)
									{
										float statValue = building2.GetStatValue(StatDefOf.Comfort, true);
										if (statValue > num && this.pawn.CanReserveAndReach(building2, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, false))
										{
											building = building2;
											num = statValue;
										}
									}
								}
							}
						}
						Job newJob = new Job(JobDefOfAvali.LovinAvali, bestPartnerForLovin, building);
						this.pawn.jobs.StartJob(newJob, JobCondition.Succeeded, null, false, true, null, null, false, false);
					}
				}
			}
			yield break;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00003A6C File Offset: 0x00001C6C
		public Pawn GetBestPartnerForLovin(List<Pawn> candidates)
		{
			if (candidates.Count == 0)
			{
				return null;
			}
			Pawn pawn = null;
			for (int i = 0; i < candidates.Count; i++)
			{
				Pawn pawn2 = candidates[i];
				if (this.pawn.HaveLoveRelation(pawn2))
				{
					pawn = pawn2;
					break;
				}
			}
			if (pawn == null)
			{
				int num = 0;
				int num2 = 0;
				for (int j = 0; j < candidates.Count; j++)
				{
					Pawn pawn3 = candidates[j];
					int num3 = this.pawn.relations.OpinionOf(pawn3);
					int num4 = pawn3.relations.OpinionOf(this.pawn);
					if (num3 > num && num4 > num2)
					{
						num = num3;
						num2 = num4;
						pawn = pawn3;
					}
				}
			}
			return pawn;
		}
	}
}
