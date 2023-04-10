﻿namespace GameFellowship.Data.Services;

public class LoginService : ILoginService
{
	private readonly IUserService _userService;
	private HashSet<int> _loginUsers = new();

	public string SessionStorageKey { get; } = "userid";

	public LoginService(IUserService service)
	{
		_userService = service;
	}

	public bool UserHasLogin(int userId)
	{
        return _loginUsers.Contains(userId);
	}

	public async Task<(bool, int)> UserLoginAsync(string userName, string password)
	{
		(bool correct, int userId) = await _userService.CheckUserPasswordAsync(userName, password);

		if (!correct)
		{
			return (false, -1);
		}

		if (!_loginUsers.Add(userId))
		{
			// duplicate user login
			return (true, userId);
		}

		return (true, userId);
	}

	public bool UserLogout(int userId)
	{
        return _loginUsers.Remove(userId);
	}
}