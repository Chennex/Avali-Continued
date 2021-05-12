using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000062 RID: 98
	public class JoyGiver_InteractBuildingStanding : JoyGiver_InteractBuilding
	{
		// Token: 0x060001F8 RID: 504 RVA: 0x000105C0 File Offset: 0x0000E7C0
		protected override Job TryGivePlayJob(Pawn pawn, Thing thing)
		{
			if (!thing.InteractionCell.Impassable(thing.Map) && !thing.IsForbidden(pawn) && !thing.InteractionCell.IsForbidden(pawn) && !pawn.Map.pawnDestinationReservationManager.IsReserved(thing.InteractionCell))
			{
				return new Job(this.def.jobDef, thing, thing.InteractionCell);
			}
			return null;
		}
	}
}
