using System;

namespace Core
{
	public interface IArticleRepository
	{
		Article GetCatalogArticle(string c10);
		Article GetPreOrderArticle(string c10);
	}

	public class ArticleRepository :IArticleRepository
	{
		public Article GetCatalogArticle(string c10)
		{
			return new Article () {
				Code10=c10,
				EAN="AVCDEF",
				MFC="MFC"
			};
		}

		public Article GetPreOrderArticle(string c10)
		{
			return 
				string.IsNullOrEmpty(c10)
				? null
				: new Article () {Code10=c10};
		}
	}
}