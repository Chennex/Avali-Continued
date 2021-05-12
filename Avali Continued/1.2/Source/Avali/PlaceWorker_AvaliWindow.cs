using System;
using Verse;

namespace Avali
{
	// Token: 0x0200001E RID: 30
	public class PlaceWorker_AvaliWindow : PlaceWorker
	{
		// Token: 0x06000084 RID: 132 RVA: 0x0000626C File Offset: 0x0000446C
		public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 center, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			if (rot == Rot4.West || rot == Rot4.East)
			{
				this.c1 = center + new IntVec3(1, 0, 0);
				this.c2 = center - new IntVec3(1, 0, 0);
			}
			else
			{
				this.c1 = center + new IntVec3(0, 0, 1);
				this.c2 = center - new IntVec3(0, 0, 1);
			}
			if (AvaliUtility.BuildingInPosition(map, this.c1, def) != null && AvaliUtility.BuildingInPosition(map, this.c2, def) != null)
			{
				return true;
			}
			return true;
		}

		// Token: 0x04000063 RID: 99
		private IntVec3 c1;

		// Token: 0x04000064 RID: 100
		private IntVec3 c2;
	}
}
