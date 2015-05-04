using System;

namespace Core
{
	public interface IArticleNotifierCore
	{
		void ProcessMessage(string code10);

		IArticleNotifierCore OnArticleFound(Action<Article> onSuccess);
		IArticleNotifierCore OnArticleNotFound(Action onFail);
	}

	public class ArticleNotifierCore : IArticleNotifierCore
	{
		private readonly IArticleRepository _articleRepository;
		private Action<Article> _onArticleFound;
		private Action _onArticleNotFound;

		public IArticleNotifierCore OnArticleFound (Action<Article> onArticleNotFound)
		{
			_onArticleFound = onArticleNotFound;
			return this;
		}

		public IArticleNotifierCore OnArticleNotFound (Action onArticleNotFound)
		{
			_onArticleNotFound = onArticleNotFound;
			return this;
		}

		public ArticleNotifierCore(IArticleRepository articleRepository)
		{
			_articleRepository = articleRepository;
		}

		public void ProcessMessage(string code10)
		{
			TryToGetPreOrderArticle (
				code10,
				a => _onArticleFound (a),
				() => TryToGetCatalogArticle(
					code10,
					a => _onArticleFound (a),
					()=>_onArticleNotFound ())
			);
		}

		private void TryToGetPreOrderArticle(string code10,Action<Article> preOrderArticleFound,Action preOrderArticleNotFound)
		{
			var article=_articleRepository.GetPreOrderArticle (code10);
			if (article != null && preOrderArticleFound!=null)
				preOrderArticleFound (article);
			if (article == null && preOrderArticleNotFound!=null)
				preOrderArticleNotFound ();
		}

		private void TryToGetCatalogArticle(string code10,Action<Article> catalogArticleFound,Action catalogArticleNotFound)
		{
			var article=_articleRepository.GetCatalogArticle (code10);
			if (article != null && catalogArticleFound!=null)
				catalogArticleFound (article);
			if (article == null && catalogArticleNotFound!=null)
				catalogArticleNotFound ();
		}
	}
}