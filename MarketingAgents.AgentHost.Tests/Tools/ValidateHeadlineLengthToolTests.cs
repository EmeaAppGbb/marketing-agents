// Copyright (c) Marketing Agents. All rights reserved.

using MarketingAgents.AgentHost.Tools;

namespace MarketingAgents.AgentHost.Tests.Tools;

/// <summary>
/// Unit tests for ValidateHeadlineLengthTool.
/// </summary>
public sealed class ValidateHeadlineLengthToolTests
{
    [Theory]
    [InlineData("Short", 5)]
    [InlineData("This is a test headline", 23)]
    [InlineData("Perfect headline length for SEO and engagement!", 47)]
    public void ValidateHeadlineLength_WithVariousLengths_ReturnsAppropriateGuidance(string headline, int expectedLength)
    {
        // Act
        var result = ValidateHeadlineLengthTool.ValidateHeadlineLength(headline);

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
        result.Should().Contain(expectedLength.ToString());
    }

    [Fact]
    public void ValidateHeadlineLength_WithOptimalLength_ReturnsOptimalGuidance()
    {
        // Arrange
        var headline = "This headline is exactly fifty-five characters long!!";

        // Act
        var result = ValidateHeadlineLengthTool.ValidateHeadlineLength(headline);

        // Assert
        result.Should().Contain("optimal");
        result.Should().Contain("53");
    }

    [Fact]
    public void ValidateHeadlineLength_WithTooShortHeadline_ReturnsExpandGuidance()
    {
        // Arrange
        var headline = "Too short";

        // Act
        var result = ValidateHeadlineLengthTool.ValidateHeadlineLength(headline);

        // Assert
        result.Should().Contain("too short");
        result.Should().Contain("expanding");
    }

    [Fact]
    public void ValidateHeadlineLength_WithTooLongHeadline_ReturnsShortenGuidance()
    {
        // Arrange
        var headline = new string('x', 75);

        // Act
        var result = ValidateHeadlineLengthTool.ValidateHeadlineLength(headline);

        // Assert
        result.Should().Contain("too long");
        result.Should().Contain("75");
    }

    [Fact]
    public void ValidateHeadlineLength_WithNullHeadline_ThrowsArgumentException()
    {
        // Act
        var act = () => ValidateHeadlineLengthTool.ValidateHeadlineLength(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ValidateHeadlineLength_WithEmptyHeadline_ThrowsArgumentException()
    {
        // Act
        var act = () => ValidateHeadlineLengthTool.ValidateHeadlineLength(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
