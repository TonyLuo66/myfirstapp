using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using MyFirstApp.Business.Interfaces;
using MyFirstApp.Business.Models.Commands;
using MyFirstApp.Common.Constants;
using MyFirstApp.Common.Exceptions;
using MyFirstApp.Infrastructure.Data;

namespace MyFirstApp.Infrastructure.Repositories;

/// <summary>
/// Dapper command repository for application profiles.
/// </summary>
public sealed class ApplicationProfileCommandRepository : IApplicationProfileCommandRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationProfileCommandRepository"/> class.
    /// </summary>
    /// <param name="connectionFactory">The SQL connection factory.</param>
    public ApplicationProfileCommandRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc />
    public async Task CreateAsync(CreateApplicationProfileCommand command, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO dbo.app_profiles (profile_key, display_name, owner_team, environment, is_active, updated_at)
            VALUES (@ProfileKey, @DisplayName, @OwnerTeam, @Environment, @IsActive, SYSUTCDATETIME());
            """;

        try
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            await connection.ExecuteAsync(new CommandDefinition(sql, command, cancellationToken: cancellationToken));
        }
        catch (SqlException exception) when (exception.Number is 2627 or 2601)
        {
            throw new AppException(StatusCodes.Status409Conflict, ReturnCodeConstants.RecordAlreadyExists, $"資料已存在: {command.ProfileKey}", exception);
        }
        catch (Exception exception)
        {
            throw new AppException(StatusCodes.Status500InternalServerError, ReturnCodeConstants.DatabaseError, "新增應用資料失敗。", exception);
        }
    }

    /// <inheritdoc />
    public async Task<int> UpdateAsync(UpdateApplicationProfileCommand command, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE dbo.app_profiles
            SET
                display_name = @DisplayName,
                owner_team = @OwnerTeam,
                environment = @Environment,
                is_active = @IsActive,
                updated_at = SYSUTCDATETIME()
            WHERE profile_key = @ProfileKey;
            """;

        try
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            return await connection.ExecuteAsync(new CommandDefinition(sql, command, cancellationToken: cancellationToken));
        }
        catch (Exception exception)
        {
            throw new AppException(StatusCodes.Status500InternalServerError, ReturnCodeConstants.DatabaseError, "更新應用資料失敗。", exception);
        }
    }
}