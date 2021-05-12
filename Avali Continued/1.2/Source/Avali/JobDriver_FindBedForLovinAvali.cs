using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000008 RID: 8
	public class JobDriver_FindBedForLovinAvali : JobDriver
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600001B RID: 27 RVA: 0x000020F5 File Offset: 0x000002F5
		private Pawn partner
		{
			get
			{
				return (Pawn)((Thing)this.job.GetTarget(this.PartnerInd));
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002112 File Offset: 0x00000312
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.partner, this.job, 1, -1, null, true);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00003904 File Offset: 0x00001B04
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(this.PartnerInd);
			this.FailOn(() => !this.partner.health.capacities.CanBeAwake);
			Building firstBuilding = this.partner.Position.GetFirstBuilding(base.Map);
			if (firstBuilding == null || firstBuilding.GetStatValue(StatDefOf.Comfort, true) < 0.5f)
			{
				Room room = this.pawn.GetRoom(RegionType.Set_Passable);
				if (room != null)
				{
					AvaliUtility.FindAllThingsOnMapAtRange(this.pawn, null, typeof(Pawn), room.ContainedAndAdjacentThings, 20f, 9999, true, true);
				}
			}
			yield break;
		}

		// Token: 0x04000016 RID: 22
		private const int ticksBeforeMote = 200;

		// Token: 0x04000017 RID: 23
		private TargetIndex PartnerInd = TargetIndex.A;
	}
}
