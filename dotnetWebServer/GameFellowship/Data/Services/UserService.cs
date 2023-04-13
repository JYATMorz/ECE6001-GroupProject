using GameFellowship.Data.Database;
using GameFellowship.Data.FormModels;
using Microsoft.EntityFrameworkCore;

namespace GameFellowship.Data.Services;

public class UserService : IUserService
{
	public string DefaultUserIconUri { get; } = "images/UserIcons/50913860_p9.jpg";
	public string DefaultUserIconFolder { get; } = "UserIcons";
	public string DefaultUserName { get; } = "匿名";

	private readonly IDbContextFactory<GameFellowshipDb> _dbContextFactory;

	public UserService(IDbContextFactory<GameFellowshipDb> dbContextFactory)
	{
		_dbContextFactory = dbContextFactory;
	}

	public async Task<bool> CreateUserAsync(UserModel model)
	{
		if (await HasUserAsync(model.UserName))
		{
			return false;
		}
		if (!string.IsNullOrWhiteSpace(model.UserEmail) && await HasEmailAsync(model.UserEmail))
		{
			return false;
		}

		using var dbContext = _dbContextFactory.CreateDbContext();
		User newUser = new()
		{
			Password = model.UserPassword,
			Email = model.UserEmail,
			IconURI = model.UserIconURI
		};

        dbContext.Users.Add(newUser);
		await dbContext.SaveChangesAsync();

		return true;
	}

	public async Task<bool> AddFollowedGameAsync(int userId, int gameId)
	{
        using var dbContext = _dbContextFactory.CreateDbContext();
		var resultUser = await dbContext.Users
										.Where(user => user.Id == userId)
										.Include(user => user.FollowedGames)
										.FirstOrDefaultAsync();
		if (resultUser is null) return false;

		var resultGame = resultUser.FollowedGames
								   .Where(game => game.Id == gameId)
								   .FirstOrDefault();
		if (resultGame is null) return false;

		if (resultUser.FollowedGames.Contains(resultGame))
		{
			return false;
		}
		resultUser.FollowedGames.Add(resultGame);
		resultGame.Followers++;

		await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AddJoinedPostAsync(int userId, int postId)
	{
        using var dbContext = _dbContextFactory.CreateDbContext();
        var resultUser = await dbContext.Users
										.Where(user => user.Id == userId)
										.Include(user => user.JoinedPosts)
										.FirstOrDefaultAsync();
        if (resultUser is null) return false;

        var resultPost = resultUser.JoinedPosts
                             .Where(post => post.Id == postId)
							 .FirstOrDefault();
		if (resultPost is null || resultPost.CurrentPeople >= resultPost.TotalPeople)
		{
            return false;
        }

		if (resultUser.JoinedPosts.Contains(resultPost))
		{
			return false;
		}
		resultUser.JoinedPosts.Add(resultPost);
		resultPost.CurrentPeople++;

        await dbContext.SaveChangesAsync();

        return true;
    }

	public async Task<bool> AddCreatedPostAsync(int userId, int postId)
	{
        using var dbContext = _dbContextFactory.CreateDbContext();
        var resultUser = await dbContext.Users
                                        .Where(user => user.Id == userId)
                                        .Include(user => user.CreatedPosts)
                                        .FirstOrDefaultAsync();
        if (resultUser is null) return false;

        var resultPost = resultUser.JoinedPosts
                             .Where(post => post.Id == postId)
                             .FirstOrDefault();
		if (resultPost is null || resultUser.CreatedPosts.Contains(resultPost))
		{
			return false;
		}

		resultUser.CreatedPosts.Add(resultPost);
		await dbContext.SaveChangesAsync();

		return true;
    }

    public async Task<bool> DeleteFollowedGameAsync(int userId, int gameId)
	{
		using var dbContext = _dbContextFactory.CreateDbContext();
		var resultUser = await dbContext.Users
										.Where(user => userId == user.Id)
										.Include(user => user.FollowedGames)
										.FirstOrDefaultAsync();
		if (resultUser is null) return false;

		var resultGame = resultUser.FollowedGames
								   .Where(game => game.Id == gameId)
								   .FirstOrDefault();
		if (resultGame is null) return false;

		if (!resultUser.FollowedGames.Remove(resultGame))
		{
			return false;
		}
		resultGame.Followers--;

		await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteJoinedPostAsync(int userId, int postId)
	{
		using var dbContext = _dbContextFactory.CreateDbContext();
		var resultUser = await dbContext.Users
										.Where(user => userId == user.Id)
										.Include(user => user.JoinedPosts)
										.FirstOrDefaultAsync();
		if (resultUser is null) return false;

		var resultPost = resultUser.JoinedPosts
								   .Where(post => post.Id == postId)
								   .FirstOrDefault();
		if (resultPost is null) return false;

		if (!resultUser.JoinedPosts.Remove(resultPost))
		{
			return false;
		}
		resultPost.CurrentPeople--;

		await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteCreatedPostAsync(int userId, int postId)
	{
		using var dbContext = _dbContextFactory.CreateDbContext();
		var result = await dbContext.Users
									.Where(user => userId == user.Id)
									.Include(user => user.CreatedPosts)
									.FirstOrDefaultAsync();
        if (result is null) return false;

        var resultPost = result.CreatedPosts
							   .Where(post => post.Id == postId)
							   .FirstOrDefault();
        if (resultPost is null) return false;

		if (!result.CreatedPosts.Remove(resultPost))
		{
			return false;
		}

        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<string> GetUserNameAsync(int userId)
	{
		using var dbContext = _dbContextFactory.CreateDbContext();
		var resultUser = await dbContext.Users
										.Where(user => userId == user.Id)
										.FirstOrDefaultAsync();

		return resultUser?.Name ?? DefaultUserName;
	}

	public async Task<string> GetUserIconUriAsync(int userId)
	{
		using var dbContext = _dbContextFactory.CreateDbContext();
		var resultUser = await dbContext.Users
										.Where(user => userId == user.Id)
										.FirstOrDefaultAsync();

		return resultUser?.IconURI ?? DefaultUserIconUri;
	}

	public async Task<string[]> GetUserFollowedGameNamesAsync(int userId)
	{
		using var dbContext = _dbContextFactory.CreateDbContext();
		var resultGames = await dbContext.Users
										 .Where(user => userId == user.Id)
										 .Include(user => user.FollowedGames)
										 .FirstOrDefaultAsync();

		return resultGames?.FollowedGames?.Select(game => game.Name).ToArray()
			?? Array.Empty<string>();
	}

	public async Task<Post[]> GetUserJoinedPostsAsync(int userId)
	{
		using var dbContext = _dbContextFactory.CreateDbContext();
		var result = await dbContext.Users
									.Where(user => userId == user.Id)
									.Include(user => user.JoinedPosts)
									.ThenInclude(post => post.Game)
									.FirstOrDefaultAsync();

		return result?.JoinedPosts?.ToArray() ?? Array.Empty<Post>();
	}

	public async Task<(string, string)> GetUserNameIconPairAsync(int userId)
	{
		using var dbContext = _dbContextFactory.CreateDbContext();
		var resultUser = await dbContext.Users
										.Where(user => userId == user.Id)
										.FirstOrDefaultAsync();

		return (resultUser?.Name ?? string.Empty, resultUser?.IconURI ?? string.Empty);
	}

	public async Task<Dictionary<string, string>> GetUserGroupNameIconPairAsync(IEnumerable<int> userIds)
	{
		using var dbContext = _dbContextFactory.CreateDbContext();
		var resultUsers = await dbContext.Users
										 .Where(user => userIds.Contains(user.Id))
										 .ToDictionaryAsync(user => user.Name, user => user.IconURI);

		return resultUsers;
	}

    public async Task<bool> HasUserAsync(int userId)
	{
        using var dbContext = _dbContextFactory.CreateDbContext();
		bool result = await dbContext.Users
									 .Where(user => user.Id == userId)
									 .AnyAsync();

        return result;
	}

	public async Task<bool> HasUserAsync(string userName)
	{
        using var dbContext = _dbContextFactory.CreateDbContext();
		bool result = await dbContext.Users
									 .Where(user => userName.Equals(user.Name))
									 .AnyAsync();

        return result;
	}

	public async Task<bool> HasEmailAsync(string email)
	{
        using var dbContext = _dbContextFactory.CreateDbContext();
		bool result = await dbContext.Users
									 .AnyAsync(user => email.Equals(user.Email));

        return result;
	}
}