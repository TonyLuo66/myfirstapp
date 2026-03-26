using Dapper;
using Microsoft.AspNetCore.Http;
using MyFirstApp.Business.Interfaces;
using MyFirstApp.Business.Models.Dtos;
using MyFirstApp.Business.Models.Queries;
using MyFirstApp.Common.Constants;
using MyFirstApp.Common.Exceptions;
using MyFirstApp.Common.Models;
using MyFirstApp.Infrastructure.Data;

namespace MyFirstApp.Infrastructure.Repositories;

/// <summary>
/// Dapper query repository for application profiles.
/// </summary>
public sealed class ApplicationProfileQueryRepository : IApplicationProfileQueryRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationProfileQueryRepository"/> class.
    /// </summary>
    /// <param name="connectionFactory">The SQL connection factory.</param>
    public ApplicationProfileQueryRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc />
    public async Task<ApplicationProfileDto?> GetByKeyAsync(string profileKey, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                profile_key AS ProfileKey,
                display_name AS DisplayName,
                owner_team AS OwnerTeam,
                environment AS Environment,
                is_active AS IsActive,
                updated_at AS UpdatedAt
            FROM dbo.app_profiles
            WHERE profile_key = @ProfileKey;
            """;

        try
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<ApplicationProfileDto>(new CommandDefinition(sql, new { ProfileKey = profileKey }, cancellationToken: cancellationToken));
        }
        catch (Exception exception)
        {
            throw new AppException(StatusCodes.Status500InternalServerError, ReturnCodeConstants.DatabaseError, "讀取應用資料失敗。", exception);
        }
    }

    /// <inheritdoc />
    public async Task<PagedResult<ApplicationProfileDto>> SearchAsync(ApplicationProfileQueryDto query, CancellationToken cancellationToken)
    {
        const string dataSql = """
            SELECT
                profile_key AS ProfileKey,
                display_name AS DisplayName,
                owner_team AS OwnerTeam,
                environment AS Environment,
                is_active AS IsActive,
                updated_at AS UpdatedAt
            FROM dbo.app_profiles
            WHERE (@Keyword IS NULL OR profile_key LIKE @KeywordLike OR display_name LIKE @KeywordLike)
              AND (@IsActive IS NULL OR is_active = @IsActive)
            ORDER BY profile_key
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        const string countSql = """
            SELECT COUNT(1)
            FROM dbo.app_profiles
            WHERE (@Keyword IS NULL OR profile_key LIKE @KeywordLike OR display_name LIKE @KeywordLike)
              AND (@IsActive IS NULL OR is_active = @IsActive);
            """;

        string? keyword = string.IsNullOrWhiteSpace(query.Keyword) ? null : query.Keyword.Trim();
        object parameters = new
        {
            Keyword = keyword,
            KeywordLike = keyword is null ? null : $"%{keyword}%",
            query.IsActive,
            query.PageSize,
            Offset = (query.PageNumber - 1) * query.PageSize
        };

        try
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            var items = (await connection.QueryAsync<ApplicationProfileDto>(new CommandDefinition(dataSql, parameters, cancellationToken: cancellationToken))).ToList();
            int totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

            return new PagedResult<ApplicationProfileDto>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                Items = items
            };
        }
        catch (Exception exception)
        {
            throw new AppException(StatusCodes.Status500InternalServerError, ReturnCodeConstants.DatabaseError, "查詢應用資料失敗。", exception);
        }
    }
}
