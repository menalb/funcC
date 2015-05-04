using NUnit.Framework;
using Rhino.Mocks;
using System;
using Core;

namespace Tests
{
	[TestFixture ()]
	public class when_an_article_is_provided : with_an_article_pending_changes_notifier
	{
		[Test]
		public void it_should_always_check_if_is_a_preOrder_article()
		{
			_articleRepository.AssertWasCalled(_=>_.GetPreOrderArticle(_code10));
		}
	}

	[TestFixture ()]
	public class when_a_preOrder_article_is_provided : with_an_article_pending_changes_notifier
	{
		public override void Given ()
		{
			base.Given ();
			_articleRepository.Expect (_ => _.GetPreOrderArticle (Arg<String>.Is.Anything)).Return (
				new Article
				{
					Code10=_code10
				}
			);
		}

		[Test]
		public void it_should_get_its_information_from_the_preOrder()
		{
			_articleRepository.AssertWasCalled(_=>_.GetPreOrderArticle(_code10));
		}

		[Test]
		public void it_should_search_in_the_catalog()
		{
			_articleRepository.AssertWasNotCalled(_=>_.GetCatalogArticle(_code10));
		}
	}

	[TestFixture ()]
	public class when_a_no_preOrder_article_is_provided : with_an_article_pending_changes_notifier
	{
		public override void Given ()
		{
			base.Given ();
			_articleRepository.Expect (_ => _.GetPreOrderArticle (Arg<String>.Is.Anything)).Return (null);
			_articleRepository.Expect (_ => _.GetCatalogArticle (Arg<String>.Is.Anything)).Return (new Article());
		}

		[Test]
		public void it_should_get_its_informations_from_the_catalog()
		{
			_articleRepository.AssertWasCalled(_=>_.GetCatalogArticle(_code10));
		}
	}

	[TestFixture ()]
	public class when_a_preOrder_or_not_article_is_provided : with_an_article_pending_changes_notifier
	{
		
	}

	[TestFixture ()]
	public class and_the_article_has_been_found : when_a_preOrder_or_not_article_is_provided
	{
		private bool _onArticleFoundWasCalled=false;
		private bool _onArticleNotFoundWasCalled=false;
		private Article _article;

		public override void Given ()
		{
			base.Given ();
			_articleRepository.Expect (_ => _.GetPreOrderArticle (Arg<String>.Is.Anything)).Return (null);
			_articleRepository.Expect (_ => _.GetCatalogArticle (Arg<String>.Is.Anything)).Return (
				new Article
				{
					Code10=_code10,
					EAN="EAN",
					MFC="MFC"
				}
			);
		}

		public override void When()
		{
			this._sut.OnArticleFound(a=>
				{
					_onArticleFoundWasCalled=true;
					_article=a;
				}).ProcessMessage(_code10);
		}

		[Test]
		public void it_should_invoke_the_onSucces_action()
		{
			Assert.IsTrue (_onArticleFoundWasCalled);
			Assert.IsFalse (_onArticleNotFoundWasCalled);
		}

		[Test]
		public void it_should_return_the_article_by_the_onSuccess_action()
		{
			Assert.AreEqual (_article.Code10, _code10);
			Assert.AreEqual (_article.EAN, "EAN");
			Assert.AreEqual (_article.MFC, "MFC");

		}
	}

	[TestFixture ()]
	public class and_the_article_has_not_been_found : when_a_preOrder_or_not_article_is_provided
	{
		private bool _onArticleFoundWasCalled=false;
		private bool _onArticleNotFoundWasCalled=false;

		public override void Given ()
		{
			base.Given ();
			_articleRepository.Expect (_ => _.GetPreOrderArticle (Arg<String>.Is.Anything)).Return (null);
			_articleRepository.Expect (_ => _.GetCatalogArticle (Arg<String>.Is.Anything)).Return (null);
		}

		public override void When()
		{
			this._sut.OnArticleNotFound(()=>{_onArticleNotFoundWasCalled=true;}).ProcessMessage(_code10);
		}

		[Test]
		public void it_should_invoke_the_onSucces_action()
		{
			Assert.IsFalse (_onArticleFoundWasCalled);
			Assert.IsTrue (_onArticleNotFoundWasCalled);
		}
	}

	#region [spec]

	public abstract class with_an_article_pending_changes_notifier : SpecificationContext
	{
	    protected string _code10="12345678AB";
		protected ArticleNotifierCore _sut;
		protected IArticleRepository _articleRepository;

		public override void Given()
		{
			_articleRepository = MockRepository.GenerateMock<IArticleRepository> ();
			this._sut = new ArticleNotifierCore (_articleRepository);
		}

		public override void When()
		{
			this._sut.OnArticleFound(a=>{}).OnArticleNotFound(()=>{}).ProcessMessage(_code10);
		}
	}

	public abstract class SpecificationContext
	{
		[SetUp]
		public void Init()
		{
			this.Given();
			this.When();
		}

		public virtual void Given() { }
		public virtual void When() { }
	}

	#endregion
}