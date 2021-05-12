using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Avali
{
	// Token: 0x02000037 RID: 55
	public class CompProperties_TextThing : CompProperties
	{
		// Token: 0x06000122 RID: 290 RVA: 0x0000A710 File Offset: 0x00008910
		public CompProperties_TextThing()
		{
			this.compClass = typeof(CompTextThing);
		}

		// Token: 0x040000F5 RID: 245
		public string translatedTexPath;

		// Token: 0x040000F6 RID: 246
		public JobDef useJob;

		// Token: 0x040000F7 RID: 247
		public string useLabel;

		// Token: 0x040000F8 RID: 248
		public SkillDef workSkill;

		// Token: 0x040000F9 RID: 249
		public int minSkillLevel;

		// Token: 0x040000FA RID: 250
		public ThingDef workTable;

		// Token: 0x040000FB RID: 251
		public int workLeft = -1;

		// Token: 0x040000FC RID: 252
		public bool showAuthor = true;

		// Token: 0x040000FD RID: 253
		public bool showTranslator = true;

		// Token: 0x040000FE RID: 254
		public bool showWorkLeft = true;

		// Token: 0x040000FF RID: 255
		public float translatedMarketValue;

		// Token: 0x04000100 RID: 256
		public float defaultMarketValue;

		// Token: 0x04000101 RID: 257
		public string labelTranslated = "";

		// Token: 0x04000102 RID: 258
		public string descriptionNotTranslated = "";

		// Token: 0x04000103 RID: 259
		public string descriptionTranslated = "";

		// Token: 0x04000104 RID: 260
		public string translatedText = "";

		// Token: 0x04000105 RID: 261
		public string author;

		// Token: 0x04000106 RID: 262
		public TaleDef taleWhenTranslated;

		// Token: 0x04000107 RID: 263
		public ThoughtDef thoughtWhenTranslated;

		// Token: 0x04000108 RID: 264
		public Vector2 translationTabWinSize = Vector2.zero;
	}
}
