// Copyright (c) Marketing Agents. All rights reserved.

using MarketingAgents.Api.Models;

namespace MarketingAgents.Api.Data;

/// <summary>
/// Repository interface for artifact version operations.
/// </summary>
public interface IVersionRepository
{
    /// <summary>
    /// Creates a new artifact version.
    /// </summary>
    /// <param name="version">The version to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created version.</returns>
    Task<ArtifactVersion> CreateAsync(ArtifactVersion version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a version by its ID.
    /// </summary>
    /// <param name="versionId">The version ID.</param>
    /// <param name="campaignId">The campaign ID (partition key).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The version if found; otherwise, null.</returns>
    Task<ArtifactVersion?> GetByIdAsync(string versionId, string campaignId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latest version for an artifact.
    /// </summary>
    /// <param name="artifactId">The artifact ID.</param>
    /// <param name="campaignId">The campaign ID (partition key).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The latest version if found; otherwise, null.</returns>
    Task<ArtifactVersion?> GetLatestByArtifactIdAsync(string artifactId, string campaignId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Archives a version by updating its status.
    /// </summary>
    /// <param name="versionId">The version ID.</param>
    /// <param name="campaignId">The campaign ID (partition key).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ArchiveAsync(string versionId, string campaignId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing version.
    /// </summary>
    /// <param name="version">The version to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated version.</returns>
    Task<ArtifactVersion> UpdateAsync(ArtifactVersion version, CancellationToken cancellationToken = default);
}
