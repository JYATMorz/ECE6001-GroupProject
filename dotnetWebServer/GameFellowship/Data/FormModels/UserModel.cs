using GameFellowship.Data.Services;

namespace GameFellowship.Data.FormModels;

public class UserModel
{
	private readonly IUserService userService;

	private const int _userNameMaxLength = 12;
	private const int _userPasswordMinLength = 6;
	private const int _userPasswordMaxLength = 20;

	private string _userName = string.Empty;
	[UserNameValidator(_userNameMaxLength)]
	public string UserName
	{
		get => _userName;
		set
		{
			_userName = value.Trim();
			OnUserNameChange();
		}
	}
	[UserExistValidator("�û���")]
	public bool UserNameExisted { get; private set; } = false;
	[UserValidCheckValidator]
	public bool ValidUserName => !string.IsNullOrWhiteSpace(UserName) && UserName.Length <= _userNameMaxLength && !UserNameExisted;

	private string _userEmail = string.Empty;
	private string _passwordRepeat = string.Empty;

	[UserEmailValidator]
	public string UserEmail
	{
		get => _userEmail;
		set
		{
			_userEmail = value.Trim();
			OnUserEmailChange();
		}
	}
	[UserExistValidator("����")]
	public bool UserEmailExisted { get; private set; } = false;

	[UserPasswordValidator(_userPasswordMinLength, _userPasswordMaxLength)]
	public string UserPassword { get; set; } = string.Empty;
	public bool PasswordModified { get; set; } = false;
	public string PasswordRepeat
	{
		get => _passwordRepeat;
		set
		{
			_passwordRepeat = value.Trim();
			if (!PasswordModified) PasswordModified = true;
		}
	}
	[UserValidCheckValidator]
	public bool ValidPassword => !string.IsNullOrWhiteSpace(UserPassword) && UserPassword == PasswordRepeat;

	public string UserIconURI { get; set; } = string.Empty;

	public UserModel(IUserService service)
	{
		userService = service;
	}

	private async void OnUserNameChange()
	{
		if (!string.IsNullOrWhiteSpace(UserName))
		{
			UserNameExisted = await userService.HasUserAsync(UserName);
		}
		else
		{
			UserNameExisted = false;
		}
	}

	private async void OnUserEmailChange()
	{
		if (!string.IsNullOrWhiteSpace(UserEmail))
		{
			UserEmailExisted = await userService.HasEmailAsync(UserEmail);
		}
		else
		{
			UserEmailExisted = false;
		}
	}
}