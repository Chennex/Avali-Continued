using System;
using Verse;

namespace Avali
{
	// Token: 0x02000033 RID: 51
	public class CompProperties_UseBuilding : CompProperties
	{
		// Token: 0x06000114 RID: 276 RVA: 0x000029B5 File Offset: 0x00000BB5
		public CompProperties_UseBuilding()
		{
			this.compClass = typeof(CompUseBuilding);
		}

		// Token: 0x040000E8 RID: 232
		public WorkTags workType;

		// Token: 0x040000E9 RID: 233
		public JobDef useJob;

		// Token: 0x040000EA RID: 234
		public string floatMenuText;
	}
}
