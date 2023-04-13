﻿namespace GameFellowship.Data.Services;

public interface ILoginService
{
    string LocalStorageKey => "user";
    string DefaultConnectionSign => "++";

    Task<(bool, int)> UserHasLoginAsync(string? userLoginInfo);

    Task<(bool, string)> UserLoginAsync(string userName, string password);
	Task<bool> UserLogoutAsync(string? userLoginInfo);
}