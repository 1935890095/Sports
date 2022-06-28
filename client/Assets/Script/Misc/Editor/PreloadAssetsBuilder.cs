namespace ZF.Misc.Editor
{
	public class PreloadAssetsBuilder : AssetBuilder
	{
		public PreloadAssetsBuilder()
		{
			this.outputPath = "Assets/StreamingAssets/res/preload";
			this.compress = true;
			this.bundleVariant = "preload";
		}
	}
}