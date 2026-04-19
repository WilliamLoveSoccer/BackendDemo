using System.Linq.Expressions;
using BackendDemo.Data;
using BackendDemo.DTOs;
using BackendDemo.Managers;
using BackendDemo.Models;
using BackendDemo.Repositories;
using Moq;
using NUnit.Framework;

namespace BackendDemo.Tests;

[TestFixture]
public class AlertManagerTests
{
    private Mock<IAlertRepository> _repositoryMock = null!;
    private Mock<IGroupRepository> _groupRepositoryMock = null!;
    private Mock<IAppDbContext> _dbMock = null!;
    private AlertManager _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IAlertRepository>();
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _dbMock = new Mock<IAppDbContext>();
        _sut = new AlertManager(_repositoryMock.Object, _groupRepositoryMock.Object, _dbMock.Object);
    }

    #region CreateAlertAsync

    [Test]
    public async Task CreateAlertAsync_MapsRequestFieldsToAlert()
    {
        var request = new CreateAlertRequest
        {
            Title = "Test Title",
            Body = "Test Body",
            CreatedBy = "user1",
            GroupIds = [1, 2],
        };

        Alert? capturedAlert = null;
        _groupRepositoryMock.Setup(r => r.GetMissingGroupIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([]);
        _repositoryMock
            .Setup(r => r.AddAlertAsync(It.IsAny<Alert>()))
            .Callback<Alert>(a => capturedAlert = a)
            .Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.AddAlertGroupsAsync(It.IsAny<IEnumerable<AlertGroup>>())).Returns(Task.CompletedTask);
        _dbMock.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        await _sut.CreateAlertAsync(request);

        Assert.That(capturedAlert, Is.Not.Null);
        Assert.That(capturedAlert!.Title, Is.EqualTo("Test Title"));
        Assert.That(capturedAlert.Body, Is.EqualTo("Test Body"));
        Assert.That(capturedAlert.CreatedBy, Is.EqualTo("user1"));
    }

    [Test]
    public async Task CreateAlertAsync_SetsCreatedAtToApproximatelyUtcNow()
    {
        var request = new CreateAlertRequest { Title = "T", GroupIds = [1] };
        var before = DateTime.UtcNow;

        Alert? capturedAlert = null;
        _groupRepositoryMock.Setup(r => r.GetMissingGroupIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([]);
        _repositoryMock
            .Setup(r => r.AddAlertAsync(It.IsAny<Alert>()))
            .Callback<Alert>(a => capturedAlert = a)
            .Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.AddAlertGroupsAsync(It.IsAny<IEnumerable<AlertGroup>>())).Returns(Task.CompletedTask);
        _dbMock.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        await _sut.CreateAlertAsync(request);
        var after = DateTime.UtcNow;

        Assert.That(capturedAlert!.CreatedAt, Is.GreaterThanOrEqualTo(before));
        Assert.That(capturedAlert.CreatedAt, Is.LessThanOrEqualTo(after));
    }

    [Test]
    public async Task CreateAlertAsync_CreatesOneAlertGroupPerGroupId()
    {
        var request = new CreateAlertRequest { Title = "T", GroupIds = [10, 20, 30] };

        IEnumerable<AlertGroup>? capturedGroups = null;
        _groupRepositoryMock.Setup(r => r.GetMissingGroupIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([]);
        _repositoryMock.Setup(r => r.AddAlertAsync(It.IsAny<Alert>())).Returns(Task.CompletedTask);
        _repositoryMock
            .Setup(r => r.AddAlertGroupsAsync(It.IsAny<IEnumerable<AlertGroup>>()))
            .Callback<IEnumerable<AlertGroup>>(g => capturedGroups = g)
            .Returns(Task.CompletedTask);
        _dbMock.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        await _sut.CreateAlertAsync(request);

        Assert.That(capturedGroups, Is.Not.Null);
        Assert.That(capturedGroups!.Select(g => g.GroupId), Is.EquivalentTo(new[] { 10, 20, 30 }));
    }

    [Test]
    public async Task CreateAlertAsync_AlertGroupsReferenceCreatedAlert()
    {
        var request = new CreateAlertRequest { Title = "T", GroupIds = [5] };

        Alert? capturedAlert = null;
        IEnumerable<AlertGroup>? capturedGroups = null;
        _groupRepositoryMock.Setup(r => r.GetMissingGroupIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([]);
        _repositoryMock
            .Setup(r => r.AddAlertAsync(It.IsAny<Alert>()))
            .Callback<Alert>(a => capturedAlert = a)
            .Returns(Task.CompletedTask);
        _repositoryMock
            .Setup(r => r.AddAlertGroupsAsync(It.IsAny<IEnumerable<AlertGroup>>()))
            .Callback<IEnumerable<AlertGroup>>(g => capturedGroups = g.ToList())
            .Returns(Task.CompletedTask);
        _dbMock.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        await _sut.CreateAlertAsync(request);

        foreach (var group in capturedGroups!)
            Assert.That(group.Alert, Is.SameAs(capturedAlert));
    }

    [Test]
    public async Task CreateAlertAsync_CallsSaveChangesAsync()
    {
        var request = new CreateAlertRequest { Title = "T", GroupIds = [1] };
        _groupRepositoryMock.Setup(r => r.GetMissingGroupIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([]);
        _repositoryMock.Setup(r => r.AddAlertAsync(It.IsAny<Alert>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.AddAlertGroupsAsync(It.IsAny<IEnumerable<AlertGroup>>())).Returns(Task.CompletedTask);
        _dbMock.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        await _sut.CreateAlertAsync(request);

        _dbMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task CreateAlertAsync_ReturnsCreatedAlert()
    {
        var request = new CreateAlertRequest
        {
            Title = "My Alert",
            Body = "Body text",
            CreatedBy = "bob",
            GroupIds = [1],
        };
        _groupRepositoryMock.Setup(r => r.GetMissingGroupIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([]);
        _repositoryMock.Setup(r => r.AddAlertAsync(It.IsAny<Alert>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.AddAlertGroupsAsync(It.IsAny<IEnumerable<AlertGroup>>())).Returns(Task.CompletedTask);
        _dbMock.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        var result = await _sut.CreateAlertAsync(request);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Title, Is.EqualTo("My Alert"));
        Assert.That(result.Body, Is.EqualTo("Body text"));
        Assert.That(result.CreatedBy, Is.EqualTo("bob"));
    }

    [Test]
    public void CreateAlertAsync_ThrowsApplicationException_WhenGroupIdsDoNotExist()
    {
        var request = new CreateAlertRequest { Title = "T", GroupIds = [1, 2, 3] };
        _groupRepositoryMock
            .Setup(r => r.GetMissingGroupIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync([2, 3]);

        var ex = Assert.ThrowsAsync<ApplicationException>(() => _sut.CreateAlertAsync(request));

        Assert.That(ex!.Message, Does.Contain("2"));
        Assert.That(ex.Message, Does.Contain("3"));
    }

    #endregion

    #region GetAlertStatusAsync

    [Test]
    public async Task GetAlertStatusAsync_ReturnsNull_WhenAlertDoesNotExist()
    {
        _repositoryMock.Setup(r => r.AlertExistsAsync(99)).ReturnsAsync(false);

        var result = await _sut.GetAlertStatusAsync(99);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAlertStatusAsync_ReturnsCorrectStatusCounts()
    {
        _repositoryMock.Setup(r => r.AlertExistsAsync(1)).ReturnsAsync(true);
        _repositoryMock
            .Setup(r => r.GetDeliveryLogsAsync(It.IsAny<Expression<Func<DeliveryLog, bool>>>()))
            .ReturnsAsync(new List<DeliveryLog>
            {
                new() { UserId = 1, Status = DeliveryStatus.Sent },
                new() { UserId = 2, Status = DeliveryStatus.Sent },
                new() { UserId = 3, Status = DeliveryStatus.Failed },
                new() { UserId = 4, Status = DeliveryStatus.Pending },
                new() { UserId = 5, Status = DeliveryStatus.Pending },
            });

        var result = await _sut.GetAlertStatusAsync(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.SentCount, Is.EqualTo(2));
        Assert.That(result.FailedCount, Is.EqualTo(1));
        Assert.That(result.PendingCount, Is.EqualTo(2));
        Assert.That(result.TotalRecipients, Is.EqualTo(5));
    }

    [Test]
    public async Task GetAlertStatusAsync_TotalRecipients_CountsDistinctUserIds()
    {
        _repositoryMock.Setup(r => r.AlertExistsAsync(1)).ReturnsAsync(true);
        _repositoryMock
            .Setup(r => r.GetDeliveryLogsAsync(It.IsAny<Expression<Func<DeliveryLog, bool>>>()))
            .ReturnsAsync(new List<DeliveryLog>
            {
                new() { UserId = 1, Status = DeliveryStatus.Sent },
                new() { UserId = 1, Status = DeliveryStatus.Sent },
                new() { UserId = 2, Status = DeliveryStatus.Failed },
            });

        var result = await _sut.GetAlertStatusAsync(1);

        Assert.That(result!.TotalRecipients, Is.EqualTo(2));
    }

    [Test]
    public async Task GetAlertStatusAsync_ReturnsZeroCounts_WhenNoDeliveryLogs()
    {
        _repositoryMock.Setup(r => r.AlertExistsAsync(1)).ReturnsAsync(true);
        _repositoryMock
            .Setup(r => r.GetDeliveryLogsAsync(It.IsAny<Expression<Func<DeliveryLog, bool>>>()))
            .ReturnsAsync(new List<DeliveryLog>());

        var result = await _sut.GetAlertStatusAsync(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.TotalRecipients, Is.EqualTo(0));
        Assert.That(result.SentCount, Is.EqualTo(0));
        Assert.That(result.FailedCount, Is.EqualTo(0));
        Assert.That(result.PendingCount, Is.EqualTo(0));
    }

    [Test]
    public async Task GetAlertStatusAsync_DoesNotQueryDeliveryLogs_WhenAlertDoesNotExist()
    {
        _repositoryMock.Setup(r => r.AlertExistsAsync(99)).ReturnsAsync(false);

        await _sut.GetAlertStatusAsync(99);

        _repositoryMock.Verify(
            r => r.GetDeliveryLogsAsync(It.IsAny<Expression<Func<DeliveryLog, bool>>>()),
            Times.Never);
    }

    #endregion

    #region GetAlertsAsync

    [Test]
    public async Task GetAlertsAsync_PassesCorrectParametersToRepository()
    {
        _repositoryMock
            .Setup(r => r.GetPagenatedAlertsAsync(2, 10))
            .ReturnsAsync((Enumerable.Empty<AlertListItem>(), 0));

        await _sut.GetAlertsAsync(2, 10);

        _repositoryMock.Verify(r => r.GetPagenatedAlertsAsync(2, 10), Times.Once);
    }

    [Test]
    public async Task GetAlertsAsync_ReturnsPagedResultWithRepositoryData()
    {
        var items = new List<AlertListItem>
        {
            new() { Id = 1, Title = "Alert A" },
            new() { Id = 2, Title = "Alert B" },
        };
        _repositoryMock
            .Setup(r => r.GetPagenatedAlertsAsync(1, 5))
            .ReturnsAsync((items, 42));

        var result = await _sut.GetAlertsAsync(1, 5);

        Assert.That(result.Items, Is.EquivalentTo(items));
        Assert.That(result.Total, Is.EqualTo(42));
        Assert.That(result.Page, Is.EqualTo(1));
        Assert.That(result.PageSize, Is.EqualTo(5));
    }

    #endregion
}
