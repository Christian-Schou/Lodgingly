using Lodgingly.Framework.Application.Messaging.Commands;

namespace Lodgingly.Module.Users.Application.Users.UpdateUser;

public sealed record UpdateUserCommand(Guid UserId, string FirstName, string LastName) : ICommand;