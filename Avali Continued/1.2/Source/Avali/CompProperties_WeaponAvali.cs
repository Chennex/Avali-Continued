using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Avali
{
	// Token: 0x02000011 RID: 17
	public class CompProperties_WeaponAvali : CompProperties
	{
		// Token: 0x0600004E RID: 78 RVA: 0x000022B7 File Offset: 0x000004B7
		public CompProperties_WeaponAvali()
		{
			this.compClass = typeof(CompRangedWeaponAvali);
		}

		// Token: 0x04000031 RID: 49
		public JobDef useJob;

		// Token: 0x04000032 RID: 50
		public string useLabel;

		// Token: 0x04000033 RID: 51
		public ThingDef workTable;

		// Token: 0x04000034 RID: 52
		public SkillDef hackWorkSkill;

		// Token: 0x04000035 RID: 53
		public int hackMinSkillLevel;

		// Token: 0x04000036 RID: 54
		public int workLeft;

		// Token: 0x04000037 RID: 55
		public List<ThingDef> requredFacilities = new List<ThingDef>();
	}
}
