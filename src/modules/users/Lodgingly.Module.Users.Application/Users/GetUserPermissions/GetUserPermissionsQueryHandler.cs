using System.Data.Common;
using Dapper;
using Lodgingly.Framework.Application.Authroization;
using Lodgingly.Framework.Application.Data;
using Lodgingly.Framework.Application.Messaging.Queries;
using Lodgingly.Framework.Domain.Results;
using Lodgingly.Module.Users.Domain.Users;

namespace Lodgingly.Module.Users.Application.Users.GetUserPermissions;

internal sealed class GetUserPermissionsQueryHandler(IDatabaseConnectionFactory dbConnectionFactory)
: IQueryHandler<GetUserPermissionsQuery, PermissionsResponse>
{
    public async Task<Result<PermissionsResponse>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        
        const string sql =
            $$"""
              SELECT DISTINCT
                  u.id AS {nameof(UserPermission.UserId)},
                  rp.permission_code AS {nameof(UserPermission.Permission)}
              FROM users.users u
              JOIN users.user_roles ur ON ur.user_id = u.id
              JOIN users.role_permissions rp ON rp.role_name = ur.role_name
              WHERE u.identity_id = @IdentityId
              """;

        List<UserPermission> permissions = (await connection.QueryAsync<UserPermission>(sql, request)).AsList();

        return !permissions.Any()
            ? Result.Failure<PermissionsResponse>(UserErrors.NotFound(request.IdentityId)) 
            : new PermissionsResponse(permissions[0].UserId, permissions.Select(p => p.Permission).ToHashSet());
    }

    internal sealed class UserPermission
    {
        internal Guid UserId { get; init; }
        
        internal string Permission { get; init; }
    }
}