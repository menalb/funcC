using System;
using Core;

namespace Runner
{
	class MainClass
	{
		public static void Main (string[] args)
		{			
			IArticleNotifierCore anCore= new ArticleNotifierCore(new ArticleRepository());
			anCore.OnArticleFound(a=>
				{
					string.Format
					("Article: C10: {0}, EAN: {1}, MFC: {2}",
						a.Code10,a.EAN,a.MFC);
				})
				.OnArticleNotFound(
					()=>Notify("Article Not Found!!!"))
				.ProcessMessage("12345678AB");
		}

		private static void Notify(string message)
		{
			Console.WriteLine(message);
		}
	}
}