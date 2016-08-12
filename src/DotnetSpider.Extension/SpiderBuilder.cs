#if Test
using Newtonsoft.Json;
#endif

namespace DotnetSpider.Extension
{
	public abstract class EntitySpiderBuilder
	{
		protected abstract EntitySpider GetEntitySpider();

		public void Run(params string[] args)
		{
			var spider = GetEntitySpider();
			if (spider != null)
			{
#if Test
	// ת��JSON��ת����SpiderContext, ���ڲ���JsonSpiderContext�Ƿ�����
			string json = JsonConvert.SerializeObject(GetSpiderContext());
			ModelSpider spider = new ModelSpider(JsonConvert.DeserializeObject<JsonSpiderContext>(json).ToRuntimeContext());
#elif Publish
				//ModelSpider spider = new ModelSpider(context) {AfterSpiderFinished = AfterSpiderFinished};
#endif
				spider.Run(args);
			}
		}
	}
}