using FluentAssertions;
using HAcgReader.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    /// 测试 <see cref="MainWindowModel"/>
    /// </summary>
    [TestMethod]
    public void Test()
    {
        var eventRaised = new Dictionary<string, int>();

        var model = new MainWindowModel();
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
    /// 在没有事件监听器的情况下测试 <see cref="MainWindowModel"/>
    /// </summary>
    [TestMethod]
    public void TestWithoutEventListener()
    {
        var model = new MainWindowModel();

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
}
