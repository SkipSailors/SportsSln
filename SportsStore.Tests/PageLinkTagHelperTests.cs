namespace SportsStore.Tests;

using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.ViewModels;
using NSubstitute;

[TestClass]
public class PageLinkTagHelperTests
{
    [TestMethod]
    public void Can_Generate_Page_Links()
    {
        IUrlHelper urlHelper = Substitute.For<IUrlHelper>();
        urlHelper
            .Action(Arg.Any<UrlActionContext>())
            .Returns("Test/Page1", "Test/Page2", "Test/Page3");
        IUrlHelperFactory urlHelperFactory = Substitute.For<IUrlHelperFactory>();
        urlHelperFactory.GetUrlHelper(Arg.Any<ActionContext>()).Returns(urlHelper);
        ViewContext viewContext = new();
        PageLinkTagHelper helper = new(urlHelperFactory)
        {
            PageModel = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            },
            ViewContext = viewContext,
            PageAction = "Test"
        };
        TagHelperContext ctx = new(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            "");
        TagHelperContent content = Substitute.For<TagHelperContent>();
        TagHelperOutput output = new(
            "div",
            new TagHelperAttributeList(),
            (cache, encoder) => Task.FromResult(content));
        helper.Process(ctx, output);
        Assert.AreEqual(@"<a href=""Test/Page1"">1</a>" +
                        @"<a href=""Test/Page2"">2</a>" +
                        @"<a href=""Test/Page3"">3</a>", output.Content.GetContent());
    }
}