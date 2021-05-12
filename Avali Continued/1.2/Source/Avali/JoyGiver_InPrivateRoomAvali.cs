using System;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000060 RID: 96
	public class JoyGiver_InPrivateRoomAvali : JoyGiver
	{
		// Token: 0x060001F4 RID: 500 RVA: 0x00010528 File Offset: 0x0000E728
		public override Job TryGiveJob(Pawn pawn)
		{
			if (!pawn.health.hediffSet.HasHediff(HediffDefOfAvali.AvaliBiology, false))
			{
				return null;
			}
			if (pawn.ownership == null)
			{
				return null;
			}
			Room ownedRoom = pawn.ownership.OwnedRoom;
			if (ownedRoom == null)
			{
				return null;
			}
			IntVec3 c2;
			if (!(from c in ownedRoom.Cells
			where c.Standable(pawn.Map) && !c.IsForbidden(pawn) && pawn.CanReserveAndReach(c, PathEndMode.OnCell, Danger.None, 1, -1, null, false)
			select c).TryRandomElement(out c2))
			{
				return null;
			}
			return new Job(this.def.jobDef, c2);
		}
	}
}
