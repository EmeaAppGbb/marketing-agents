// Copyright (c) Marketing Agents. All rights reserved.

using MarketingAgents.AgentHost.Tools;

namespace MarketingAgents.AgentHost.Tests.Tools;

/// <summary>
/// Unit tests for AnalyzeToneAlignmentTool.
/// </summary>
public sealed class AnalyzeToneAlignmentToolTests
{
    [Theory]
    [InlineData("We deliver strategic solutions to optimize and enhance your business performance with proven expertise.", "professional")]
    [InlineData("You'll love our fun and exciting new product! It's amazing and awesome!", "playful")]
    [InlineData("We understand your needs and care about helping you. Together, we can support your journey.", "empathetic")]
    public void AnalyzeToneAlignment_WithMatchingTone_ReturnsPositiveAssessment(string copyText, string targetTone)
    {
        // Act
        var result = AnalyzeToneAlignmentTool.AnalyzeToneAlignment(copyText, targetTone);

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
        result.Should().Contain("Tone Adherence");
        result.Should().MatchRegex("(Excellent|Good|Moderate)");
    }

    [Fact]
    public void AnalyzeToneAlignment_WithProfessionalTone_IdentifiesKeywords()
    {
        // Arrange
        var copyText = "Our strategic solution delivers proven expertise to optimize your business.";
        var targetTone = "professional";

        // Act
        var result = AnalyzeToneAlignmentTool.AnalyzeToneAlignment(copyText, targetTone);

        // Assert
        result.Should().Contain("strategic");
        result.Should().Contain("solution");
        result.Should().Contain("expertise");
    }

    [Fact]
    public void AnalyzeToneAlignment_WithPlayfulTone_IdentifiesKeywords()
    {
        // Arrange
        var copyText = "You'll love our fun and exciting product! It's amazing!";
        var targetTone = "playful";

        // Act
        var result = AnalyzeToneAlignmentTool.AnalyzeToneAlignment(copyText, targetTone);

        // Assert
        result.Should().Contain("fun");
        result.Should().Contain("exciting");
        result.Should().Contain("love");
    }

    [Fact]
    public void AnalyzeToneAlignment_WithMismatchedTone_ReturnsWeakAssessment()
    {
        // Arrange
        var copyText = "This is a simple sentence without specific tone markers.";
        var targetTone = "professional";

        // Act
        var result = AnalyzeToneAlignmentTool.AnalyzeToneAlignment(copyText, targetTone);

        // Assert
        result.Should().Contain("Weak");
    }

    [Fact]
    public void AnalyzeToneAlignment_WithUnknownTone_ReturnsErrorMessage()
    {
        // Arrange
        var copyText = "Some text here";
        var targetTone = "unknown-tone";

        // Act
        var result = AnalyzeToneAlignmentTool.AnalyzeToneAlignment(copyText, targetTone);

        // Assert
        result.Should().Contain("Unknown tone");
        result.Should().Contain("Supported tones");
    }

    [Fact]
    public void AnalyzeToneAlignment_WithNullCopyText_ThrowsArgumentException()
    {
        // Act
        var act = () => AnalyzeToneAlignmentTool.AnalyzeToneAlignment(null!, "professional");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AnalyzeToneAlignment_WithNullTone_ThrowsArgumentException()
    {
        // Act
        var act = () => AnalyzeToneAlignmentTool.AnalyzeToneAlignment("Some text", null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("PROFESSIONAL")]
    [InlineData("Professional")]
    [InlineData("professional")]
    [InlineData(" professional ")]
    public void AnalyzeToneAlignment_WithVariousCasing_WorksCorrectly(string targetTone)
    {
        // Arrange
        var copyText = "Strategic solution to deliver expertise";

        // Act
        var result = AnalyzeToneAlignmentTool.AnalyzeToneAlignment(copyText, targetTone);

        // Assert
        result.Should().Contain("Tone Adherence");
        result.Should().NotContain("Unknown tone");
    }
}
