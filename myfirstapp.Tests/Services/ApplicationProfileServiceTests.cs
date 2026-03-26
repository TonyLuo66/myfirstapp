using Microsoft.AspNetCore.Http;
using MyFirstApp.Business.Interfaces;
using MyFirstApp.Business.Models.Commands;
using MyFirstApp.Business.Models.Dtos;
using MyFirstApp.Business.Models.Queries;
using MyFirstApp.Business.Services;
using MyFirstApp.Common.Constants;
using MyFirstApp.Common.Exceptions;
using MyFirstApp.Common.Models;

namespace MyFirstApp.Tests.Services;

public sealed class ApplicationProfileServiceTests
{
    [Fact]
    public async Task CreateAsync_TrimsFieldsBeforePersistingAndReloading()
    {
        FakeQueryRepository queryRepository = new();
        FakeCommandRepository commandRepository = new();
        ApplicationProfileService service = new(queryRepository, commandRepository);

        ApplicationProfileDto expectedProfile = new()
        {
            ProfileKey = "crm-api",
            DisplayName = "CRM API",
            OwnerTeam = "CRM Team",
            Environment = "Production",
            IsActive = true,
            UpdatedAt = new DateTime(2026, 3, 26, 0, 0, 0, DateTimeKind.Utc)
        };

        queryRepository.ProfileToReturn = expectedProfile;

        ApplicationProfileDto result = await service.CreateAsync(
            new CreateApplicationProfileCommand
            {
                ProfileKey = "  crm-api  ",
                DisplayName = "  CRM API  ",
                OwnerTeam = "  CRM Team  ",
                Environment = "  Production  ",
                IsActive = true
            },
            CancellationToken.None);

        Assert.Same(expectedProfile, result);
        Assert.NotNull(commandRepository.CreateCommand);
        Assert.Equal("crm-api", commandRepository.CreateCommand.ProfileKey);
        Assert.Equal("CRM API", commandRepository.CreateCommand.DisplayName);
        Assert.Equal("CRM Team", commandRepository.CreateCommand.OwnerTeam);
        Assert.Equal("Production", commandRepository.CreateCommand.Environment);
        Assert.Equal("crm-api", queryRepository.LastProfileKey);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFoundForMissingRecord()
    {
        FakeQueryRepository queryRepository = new();
        FakeCommandRepository commandRepository = new()
        {
            UpdateResult = 0
        };
        ApplicationProfileService service = new(queryRepository, commandRepository);

        AppException exception = await Assert.ThrowsAsync<AppException>(() => service.UpdateAsync(
            new UpdateApplicationProfileCommand
            {
                ProfileKey = " missing-profile ",
                DisplayName = "CRM API",
                OwnerTeam = "CRM Team",
                Environment = "Production",
                IsActive = true
            },
            CancellationToken.None));

        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        Assert.Equal(ReturnCodeConstants.RecordNotFound, exception.ReturnCode);
        Assert.Equal("查無對應資料: missing-profile", exception.Message);
    }

    [Fact]
    public async Task SearchAsync_ForwardsQueryToRepository()
    {
        FakeQueryRepository queryRepository = new();
        FakeCommandRepository commandRepository = new();
        ApplicationProfileService service = new(queryRepository, commandRepository);
        ApplicationProfileQueryDto query = new()
        {
            Keyword = "portal",
            PageNumber = 2,
            PageSize = 5
        };

        await service.SearchAsync(query, CancellationToken.None);

        Assert.Same(query, queryRepository.LastQuery);
    }

    private sealed class FakeQueryRepository : IApplicationProfileQueryRepository
    {
        public ApplicationProfileDto? ProfileToReturn { get; set; }

        public string? LastProfileKey { get; private set; }

        public ApplicationProfileQueryDto? LastQuery { get; private set; }

        public Task<ApplicationProfileDto?> GetByKeyAsync(string profileKey, CancellationToken cancellationToken)
        {
            LastProfileKey = profileKey;
            return Task.FromResult(ProfileToReturn);
        }

        public Task<PagedResult<ApplicationProfileDto>> SearchAsync(ApplicationProfileQueryDto query, CancellationToken cancellationToken)
        {
            LastQuery = query;
            return Task.FromResult(new PagedResult<ApplicationProfileDto>
            {
                Items = []
            });
        }
    }

    private sealed class FakeCommandRepository : IApplicationProfileCommandRepository
    {
        public CreateApplicationProfileCommand? CreateCommand { get; private set; }

        public UpdateApplicationProfileCommand? UpdateCommand { get; private set; }

        public int UpdateResult { get; set; } = 1;

        public Task CreateAsync(CreateApplicationProfileCommand command, CancellationToken cancellationToken)
        {
            CreateCommand = command;
            return Task.CompletedTask;
        }

        public Task<int> UpdateAsync(UpdateApplicationProfileCommand command, CancellationToken cancellationToken)
        {
            UpdateCommand = command;
            return Task.FromResult(UpdateResult);
        }
    }
}
