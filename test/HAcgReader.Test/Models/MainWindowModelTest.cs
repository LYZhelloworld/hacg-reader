using FluentAssertions;
using HAcgReader.Models;
using HAcgReader.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace HAcgReader.Test.Models;

/// <summary>
/// 测试 <see cref="MainWindowModel"/>
/// </summary>
[TestClass]
[ExcludeFromCodeCoverage]
public class MainWindowModelTest
{
    /// <summary>
    /// 测试用 <see cref="ArticleModel"/>
    /// </summary>
    private static readonly ArticleModel s_testArticle = new()
    {
        Title = "test",
        Link = "https://example.com",
        CommentLink = "https://example.com",
        MagnetLinks = new string[] { "a", "b" },
        Creator = "creator",
        PubDate = System.DateTime.Now,
        Categories = new string[] { "cat1", "cat2" },
        CommentCount = 50,
    };

    /// <summary>
    /// 测试 <see cref="MainWindowModel(string)"/>
    /// </summary>
    [TestMethod]
    public void TestConstructor()
    {
        using var model = new MainWindowModel("example.com");
        model.Should().NotBeNull();
    }

    /// <summary>
    /// 测试 <see cref="MainWindowModel.AddArticles(ArticleModel[])"/>
    /// </summary>
    [TestMethod]
    public void Test()
    {
        var eventRaised = new Dictionary<string, int>();

        using var model = new MainWindowModel(
            rssFeedService: Mock.Of<IRssFeedService>(),
            pageAnalyzerService: Mock.Of<IPageAnalyzerService>());
        model.PropertyChanged += (_, args) =>
        {
            var propName = args.PropertyName!;
            if (!eventRaised.ContainsKey(propName))
            {
                eventRaised[propName] = 0;
            }

            eventRaised[propName]++;
        };

        model.Articles.Should().BeEmpty();

        model.AddArticles(s_testArticle);
        eventRaised["Articles"].Should().Be(1);
        model.Articles.Should().HaveCount(1);

        model.SelectedIndex.Should().Be(-1);
        model.ShowDetails.Should().BeFalse();
        model.DetailVisibility.Should().Be(System.Windows.Visibility.Hidden);
        model.SelectedArticle.Title.Should().BeEmpty();

        model.SelectedIndex = 0;
        model.ShowDetails.Should().BeTrue();
        model.DetailVisibility.Should().Be(System.Windows.Visibility.Visible);
        model.SelectedArticle.Title.Should().Be("test");
        eventRaised[nameof(model.SelectedIndex)].Should().Be(1);
        eventRaised[nameof(model.DetailVisibility)].Should().Be(1);
        eventRaised[nameof(model.SelectedArticle)].Should().Be(1);
    }

    /// <summary>
    /// 测试 <see cref="MainWindowModel.AddArticles(ArticleModel[])"/> 在没有事件监听器时的情况
    /// </summary>
    [TestMethod]
    public void TestWithoutEventListener()
    {
        using var model = new MainWindowModel(
            rssFeedService: Mock.Of<IRssFeedService>(),
            pageAnalyzerService: Mock.Of<IPageAnalyzerService>());

        model.Articles.Should().BeEmpty();

        model.AddArticles(s_testArticle);
        model.Articles.Should().HaveCount(1);

        model.SelectedIndex.Should().Be(-1);
        model.ShowDetails.Should().BeFalse();
        model.DetailVisibility.Should().Be(System.Windows.Visibility.Hidden);
        model.SelectedArticle.Title.Should().BeEmpty();

        model.SelectedIndex = 0;
        model.ShowDetails.Should().BeTrue();
        model.DetailVisibility.Should().Be(System.Windows.Visibility.Visible);
        model.SelectedArticle.Title.Should().Be("test");
    }

    /// <summary>
    /// 测试 <see cref="MainWindowModel.FetchNewArticlesAsync"/>
    /// </summary>
    [TestMethod]
    public void TestFetchNewArticlesAsync()
    {
        var rssFeedService = new Mock<IRssFeedService>();
        rssFeedService.Setup(x => x.FetchNextAsync()).ReturnsAsync(new ArticleModel[]
        {
            new() { Title = "title 1" },
            new() { Title = "title 2" },
        });

        var pageAnalyzerService = new Mock<IPageAnalyzerService>();
        pageAnalyzerService.Setup(x => x.AnalyzeAsync(new() { Title = "title 1" }))
            .ReturnsAsync(new ArticleModel() { Title = "title 1", MagnetLinks = new string[] { "link 1" } });
        pageAnalyzerService.Setup(x => x.AnalyzeAsync(new() { Title = "title 2" }))
            .ReturnsAsync(new ArticleModel() { Title = "title 2", MagnetLinks = new string[] { "link 2" } });

        using var model = new MainWindowModel(
            rssFeedService: rssFeedService.Object,
            pageAnalyzerService: pageAnalyzerService.Object);

        model.FetchNewArticlesAsync().Wait();

        model.Articles.Should().BeEquivalentTo(new ArticleModel[]
        {
            new() { Title = "title 1", MagnetLinks = new string[] { "link 1" } },
            new() { Title = "title 2", MagnetLinks = new string[] { "link 2" } },
        });
    }
}
