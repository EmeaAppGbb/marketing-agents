using System.Net;
using FluentAssertions;
using MarketingAgents.Api.Data;
using MarketingAgents.Api.Models;
using Microsoft.Azure.Cosmos;
using Moq;
using Xunit;

namespace MarketingAgents.Api.Tests.Data;

public class CampaignRepositoryTests
{
    private readonly Mock<CosmosClient> _mockCosmosClient;
    private readonly Mock<Container> _mockContainer;
    private readonly Mock<IVersionRepository> _mockVersionRepository;
    private readonly Mock<IAuditReportRepository> _mockAuditReportRepository;
    private readonly CampaignRepository _repository;
    private const string DatabaseName = "testdb";

    public CampaignRepositoryTests()
    {
        _mockCosmosClient = new Mock<CosmosClient>();
        _mockContainer = new Mock<Container>();
        _mockVersionRepository = new Mock<IVersionRepository>();
        _mockAuditReportRepository = new Mock<IAuditReportRepository>();

        _mockCosmosClient
            .Setup(x => x.GetContainer(DatabaseName, "Campaigns"))
            .Returns(_mockContainer.Object);

        _repository = new CampaignRepository(
            _mockCosmosClient.Object,
            DatabaseName,
            _mockVersionRepository.Object,
            _mockAuditReportRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateCampaign_WhenValidCampaignProvided()
    {
        // Arrange
        var campaign = CreateTestCampaign();
        var mockResponse = new Mock<ItemResponse<Campaign>>();
        mockResponse.Setup(x => x.Resource).Returns(campaign);

        _mockContainer
            .Setup(x => x.CreateItemAsync(
                It.IsAny<Campaign>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        // Act
        var result = await _repository.CreateAsync(campaign);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(campaign.Id);
        result.Name.Should().Be(campaign.Name);

        _mockContainer.Verify(
            x => x.CreateItemAsync(
                It.Is<Campaign>(c => c.Id == campaign.Id),
                It.Is<PartitionKey>(pk => pk.ToString().Contains(campaign.Id)),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCampaign_WhenCampaignExists()
    {
        // Arrange
        var campaign = CreateTestCampaign();
        var mockResponse = new Mock<ItemResponse<Campaign>>();
        mockResponse.Setup(x => x.Resource).Returns(campaign);

        _mockContainer
            .Setup(x => x.ReadItemAsync<Campaign>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        // Act
        var result = await _repository.GetByIdAsync(campaign.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(campaign.Id);
        result.Name.Should().Be(campaign.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCampaignNotFound()
    {
        // Arrange
        _mockContainer
            .Setup(x => x.ReadItemAsync<Campaign>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CosmosException("Not found", HttpStatusCode.NotFound, 0, string.Empty, 0));

        // Act
        var result = await _repository.GetByIdAsync("nonexistent-id");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCampaign_WhenValidCampaignProvided()
    {
        // Arrange
        var campaign = CreateTestCampaign();
        var updatedCampaign = campaign with
        {
            Name = "Updated Campaign",
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        var mockResponse = new Mock<ItemResponse<Campaign>>();
        mockResponse.Setup(x => x.Resource).Returns(updatedCampaign);

        _mockContainer
            .Setup(x => x.ReplaceItemAsync(
                It.IsAny<Campaign>(),
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        // Act
        var result = await _repository.UpdateAsync(updatedCampaign);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Campaign");
        result.Id.Should().Be(campaign.Id);

        _mockContainer.Verify(
            x => x.ReplaceItemAsync(
                It.Is<Campaign>(c => c.Name == "Updated Campaign"),
                updatedCampaign.Id,
                It.Is<PartitionKey>(pk => pk.ToString().Contains(updatedCampaign.PartitionKey)),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteCampaign_WhenCampaignExists()
    {
        // Arrange
        var campaign = CreateTestCampaign();
        var mockGetResponse = new Mock<ItemResponse<Campaign>>();
        mockGetResponse.Setup(x => x.Resource).Returns(campaign);

        var deletedCampaign = campaign with { IsDeleted = true, UpdatedAt = DateTimeOffset.UtcNow };
        var mockUpdateResponse = new Mock<ItemResponse<Campaign>>();
        mockUpdateResponse.Setup(x => x.Resource).Returns(deletedCampaign);

        _mockContainer
            .Setup(x => x.ReadItemAsync<Campaign>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockGetResponse.Object);

        _mockContainer
            .Setup(x => x.ReplaceItemAsync(
                It.IsAny<Campaign>(),
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockUpdateResponse.Object);

        // Act
        await _repository.DeleteAsync(campaign.Id);

        // Assert
        _mockContainer.Verify(
            x => x.ReplaceItemAsync(
                It.Is<Campaign>(c => c.IsDeleted),
                campaign.Id,
                It.Is<PartitionKey>(pk => pk.ToString().Contains(campaign.Id)),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDoNothing_WhenCampaignNotFound()
    {
        // Arrange
        _mockContainer
            .Setup(x => x.ReadItemAsync<Campaign>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CosmosException("Not found", HttpStatusCode.NotFound, 0, string.Empty, 0));

        // Act
        await _repository.DeleteAsync("nonexistent-id");

        // Assert
        _mockContainer.Verify(
            x => x.ReplaceItemAsync(
                It.IsAny<Campaign>(),
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetCampaignSnapshotAsync_ShouldReturnNull_WhenCampaignNotFound()
    {
        // Arrange
        _mockContainer
            .Setup(x => x.ReadItemAsync<Campaign>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CosmosException("Not found", HttpStatusCode.NotFound, 0, string.Empty, 0));

        // Act
        var result = await _repository.GetCampaignSnapshotAsync("nonexistent-id");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCampaignSnapshotAsync_ShouldReturnCampaignOnly_WhenNoActiveVersions()
    {
        // Arrange
        var campaign = CreateTestCampaign() with { ActiveVersionIds = null };
        var mockResponse = new Mock<ItemResponse<Campaign>>();
        mockResponse.Setup(x => x.Resource).Returns(campaign);

        _mockContainer
            .Setup(x => x.ReadItemAsync<Campaign>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        // Act
        var result = await _repository.GetCampaignSnapshotAsync(campaign.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Campaign.Should().Be(campaign);
        result.ActiveVersions.Should().BeEmpty();
        result.AuditReports.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCampaignSnapshotAsync_ShouldIncludeActiveVersionsAndAudits_WhenPresent()
    {
        // Arrange
        var campaign = CreateTestCampaign() with
        {
            ActiveVersionIds = new Dictionary<ArtifactType, string>
            {
                { ArtifactType.Copy, "version-1" },
                { ArtifactType.ShortCopy, "version-2" },
            },
        };

        var mockResponse = new Mock<ItemResponse<Campaign>>();
        mockResponse.Setup(x => x.Resource).Returns(campaign);

        _mockContainer
            .Setup(x => x.ReadItemAsync<Campaign>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        var version1 = new ArtifactVersion
        {
            Id = "version-1",
            ArtifactId = "artifact-1",
            CampaignId = campaign.Id,
            VersionNumber = 1,
            Content = "Test copy content",
            CreatedAt = DateTimeOffset.UtcNow,
            Status = ArtifactVersionStatus.Generated,
            AuditReportId = "audit-1",
        };

        var version2 = new ArtifactVersion
        {
            Id = "version-2",
            ArtifactId = "artifact-2",
            CampaignId = campaign.Id,
            VersionNumber = 1,
            Content = "Test short copy",
            CreatedAt = DateTimeOffset.UtcNow,
            Status = ArtifactVersionStatus.Generated,
        };

        var auditReport = new AuditReport
        {
            Id = "audit-1",
            VersionId = "version-1",
            CampaignId = campaign.Id,
            OverallStatus = AuditStatus.Pass,
            CategoryScores = new Dictionary<string, AuditScore>(),
            FlaggedItems = [],
            Recommendations = [],
            CreatedAt = DateTimeOffset.UtcNow,
        };

        _mockVersionRepository
            .Setup(x => x.GetByIdAsync("version-1", campaign.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(version1);

        _mockVersionRepository
            .Setup(x => x.GetByIdAsync("version-2", campaign.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(version2);

        _mockAuditReportRepository
            .Setup(x => x.GetByVersionIdAsync("version-1", campaign.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(auditReport);

        // Act
        var result = await _repository.GetCampaignSnapshotAsync(campaign.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Campaign.Should().Be(campaign);
        result.ActiveVersions.Should().HaveCount(2);
        result.ActiveVersions[ArtifactType.Copy].Should().Be(version1);
        result.ActiveVersions[ArtifactType.ShortCopy].Should().Be(version2);
        result.AuditReports.Should().ContainKey("version-1");
        result.AuditReports["version-1"].Should().Be(auditReport);
    }

    private static Campaign CreateTestCampaign()
    {
        return new Campaign
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Campaign",
            Brief = new CampaignBrief
            {
                Objective = "Test objective",
                TargetAudience = "Test audience",
                ProductDetails = "Test product",
            },
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            Status = CampaignStatus.Draft,
            IsDeleted = false,
        };
    }
}
