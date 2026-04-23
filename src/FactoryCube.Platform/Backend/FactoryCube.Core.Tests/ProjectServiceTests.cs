using FactoryCube.Core.Application.DTOs;
using FactoryCube.Core.Application.Services;
using FactoryCube.Core.Domain.Entities;
using FactoryCube.Core.Domain.Interfaces;
using Moq;
using Xunit;

namespace FactoryCube.Core.Tests;

public class ProjectServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Return_ProjectDto()
    {
        var mockRepo = new Mock<IProjectRepository>();
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>())).ReturnsAsync((Project p, CancellationToken _) => p);
        mockRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var svc = new ProjectService(mockRepo.Object);
        var result = await svc.CreateAsync(new CreateProjectRequest("Test", null, "PLC"), "admin");

        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
        Assert.Equal("PLC", result.EquipmentType);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        var mockRepo = new Mock<IProjectRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Project?)null);

        var svc = new ProjectService(mockRepo.Object);
        var result = await svc.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }
}
