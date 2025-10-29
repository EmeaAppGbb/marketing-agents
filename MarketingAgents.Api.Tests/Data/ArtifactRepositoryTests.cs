using System.Net;
using FluentAssertions;
using MarketingAgents.Api.Data;
using MarketingAgents.Api.Models;
using Microsoft.Azure.Cosmos;
using Moq;
using Xunit;

namespace MarketingAgents.Api.Tests.Data;

public class ArtifactRepositoryTests
{
    private readonly Mock<CosmosClient> _mockCosmosClient;
    private readonly Mock<Container> _mockContainer;
    private readonly ArtifactRepository _repository;
    private const string DatabaseName = "testdb";

    public ArtifactRepositoryTests()
    {
        _mockCosmosClient = new Mock<CosmosClient>();
        _mockContainer = new Mock<Container>();

        _mockCosmosClient
            .Setup(x => x.GetContainer(DatabaseName, "Artifacts"))
            .Returns(_mockContainer.Object);

        _repository = new ArtifactRepository(_mockCosmosClient.Object, DatabaseName);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateArtifact_WhenValidArtifactProvided()
    {
        // Arrange
        var artifact = CreateTestArtifact();
        var mockResponse = new Mock<ItemResponse<Artifact>>();
        mockResponse.Setup(x => x.Resource).Returns(artifact);

        _mockContainer
            .Setup(x => x.CreateItemAsync(
                It.IsAny<Artifact>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        // Act
        var result = await _repository.CreateAsync(artifact);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(artifact.Id);
        result.Type.Should().Be(artifact.Type);

        _mockContainer.Verify(
            x => x.CreateItemAsync(
                It.Is<Artifact>(a => a.Id == artifact.Id),
                It.Is<PartitionKey>(pk => pk.ToString().Contains(artifact.CampaignId)),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnArtifact_WhenArtifactExists()
    {
        // Arrange
        var artifact = CreateTestArtifact();
        var mockResponse = new Mock<ItemResponse<Artifact>>();
        mockResponse.Setup(x => x.Resource).Returns(artifact);

        _mockContainer
            .Setup(x => x.ReadItemAsync<Artifact>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        // Act
        var result = await _repository.GetByIdAsync(artifact.Id, artifact.CampaignId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(artifact.Id);
        result.Type.Should().Be(artifact.Type);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenArtifactNotFound()
    {
        // Arrange
        _mockContainer
            .Setup(x => x.ReadItemAsync<Artifact>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CosmosException("Not found", HttpStatusCode.NotFound, 0, string.Empty, 0));

        // Act
        var result = await _repository.GetByIdAsync("nonexistent-id", "campaign-id");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCampaignIdAsync_ShouldReturnArtifacts_WhenArtifactsExist()
    {
        // Arrange
        var campaignId = "campaign-1";
        var artifacts = new List<Artifact>
        {
            CreateTestArtifact(campaignId, ArtifactType.Copy),
            CreateTestArtifact(campaignId, ArtifactType.ShortCopy),
            CreateTestArtifact(campaignId, ArtifactType.VisualConcept),
        };

        var mockIterator = MockFeedIterator(artifacts);

        _mockContainer
            .Setup(x => x.GetItemQueryIterator<Artifact>(
                It.IsAny<QueryDefinition>(),
                It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>()))
            .Returns(mockIterator.Object);

        // Act
        var result = await _repository.GetByCampaignIdAsync(campaignId);

        // Assert
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(a => a.CampaignId.Should().Be(campaignId));
    }

    [Fact]
    public async Task GetByCampaignIdAsync_ShouldReturnEmpty_WhenNoArtifactsExist()
    {
        // Arrange
        var mockIterator = MockFeedIterator(new List<Artifact>());

        _mockContainer
            .Setup(x => x.GetItemQueryIterator<Artifact>(
                It.IsAny<QueryDefinition>(),
                It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>()))
            .Returns(mockIterator.Object);

        // Act
        var result = await _repository.GetByCampaignIdAsync("campaign-1");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetVersionHistoryAsync_ShouldReturnVersions_WhenVersionsExist()
    {
        // Arrange
        var campaignId = "campaign-1";
        var artifactId = "artifact-1";
        var versions = new List<ArtifactVersion>
        {
            new ArtifactVersion
            {
                Id = "version-1",
                ArtifactId = artifactId,
                CampaignId = campaignId,
                VersionNumber = 1,
                Content = "Version 1 content",
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-2),
                Status = ArtifactVersionStatus.Generated,
            },
            new ArtifactVersion
            {
                Id = "version-2",
                ArtifactId = artifactId,
                CampaignId = campaignId,
                VersionNumber = 2,
                Content = "Version 2 content",
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
                Status = ArtifactVersionStatus.Generated,
            },
        };

        var mockIterator = MockVersionIterator(versions);

        _mockContainer
            .Setup(x => x.GetItemQueryIterator<ArtifactVersion>(
                It.IsAny<QueryDefinition>(),
                It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>()))
            .Returns(mockIterator.Object);

        // Act
        var result = await _repository.GetVersionHistoryAsync(artifactId, campaignId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(v =>
        {
            v.ArtifactId.Should().Be(artifactId);
            v.CampaignId.Should().Be(campaignId);
        });
    }

    private static Artifact CreateTestArtifact(string? campaignId = null, ArtifactType? type = null)
    {
        return new Artifact
        {
            Id = Guid.NewGuid().ToString(),
            CampaignId = campaignId ?? Guid.NewGuid().ToString(),
            Type = type ?? ArtifactType.Copy,
            CreatedAt = DateTimeOffset.UtcNow,
            VersionIds = ["version-1"],
        };
    }

    private static Mock<FeedIterator<Artifact>> MockFeedIterator(List<Artifact> items)
    {
        var mockIterator = new Mock<FeedIterator<Artifact>>();
        var queue = new Queue<List<Artifact>>(new[] { items });

        mockIterator
            .Setup(x => x.HasMoreResults)
            .Returns(() => queue.Count > 0);

        mockIterator
            .Setup(x => x.ReadNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var batch = queue.Dequeue();
                var mockResponse = new Mock<FeedResponse<Artifact>>();
                mockResponse.Setup(x => x.GetEnumerator()).Returns(batch.GetEnumerator());
                return mockResponse.Object;
            });

        return mockIterator;
    }

    private static Mock<FeedIterator<ArtifactVersion>> MockVersionIterator(List<ArtifactVersion> items)
    {
        var mockIterator = new Mock<FeedIterator<ArtifactVersion>>();
        var queue = new Queue<List<ArtifactVersion>>(new[] { items });

        mockIterator
            .Setup(x => x.HasMoreResults)
            .Returns(() => queue.Count > 0);

        mockIterator
            .Setup(x => x.ReadNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var batch = queue.Dequeue();
                var mockResponse = new Mock<FeedResponse<ArtifactVersion>>();
                mockResponse.Setup(x => x.GetEnumerator()).Returns(batch.GetEnumerator());
                return mockResponse.Object;
            });

        return mockIterator;
    }
}
