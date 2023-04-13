using GameFellowship.Data.Database;
using GameFellowship.Data.FormModels;
using GameFellowship.Pages;
using Microsoft.EntityFrameworkCore;

namespace GameFellowship.Data.Services;

public class PostService : IPostService
{
    public static string DefaultConnectionSigns => "++";

    private readonly IDbContextFactory<GameFellowshipDb> _dbContextFactory;

    public PostService(IDbContextFactory<GameFellowshipDb> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<bool> CreatePostAsync(PostModel model, int userId)
	{
		if (userId <= 0) return false;

        using var dbContext = _dbContextFactory.CreateDbContext();

		var resultUser = await dbContext.Users
										.Where(user => userId == user.Id)
										.FirstOrDefaultAsync();
		if (resultUser is null) return false;

		var resultGame = await dbContext.Games
										.Where(game => game.Name == model.GameName)
										.FirstOrDefaultAsync();
		if (resultGame is null) return false;

		Post post = new()
        {
			LastUpdate = DateTime.Now.ToUniversalTime(),
			MatchType = model.MatchType,
			Requirements = string.Join(DefaultConnectionSigns, model.Requirements),
			Description = model.Description,
			TotalPeople = model.TotalPeople,
			CurrentPeople = model.CurrentPeople,
			PlayNow = model.PlayNow,
			StartDate = model.PlayNow ? null : model.StartDate.ToUniversalTime(),
			EndDate = model.PlayNow ? null : model.EndDate.ToUniversalTime(),
			AudioChat = model.AudioChat,
			AudioPlatform = model.AudioChat ? null : model.AudioPlatform,
			AudioLink = model.AudioChat ? null : model.AudioLink,
			Game = resultGame,
			Creator = resultUser,
            JoinedUsers = new List<User> { resultUser }
			// FIXME: Empty Conversations ? Will it work ?
		};
		await dbContext.Posts.AddAsync(post);
		await dbContext.SaveChangesAsync();

		return true;
	}

	public async Task<bool> DeletePostAsync(int postId)
	{
        using var dbContext = _dbContextFactory.CreateDbContext();
        var resultPost = await dbContext.Posts
                                        .Where(post => post.Id == postId)
                                        .FirstOrDefaultAsync();
        if (resultPost is null) return false;

		dbContext.Posts.Remove(resultPost);
		await dbContext.SaveChangesAsync();

		return true;
    }

	public async Task<bool> AddCurrentUserAsync(int postId, int userId)
	{
		using var dbContext = _dbContextFactory.CreateDbContext();
		var resultPost = await dbContext.Posts
										.Where(post => post.Id == postId)
										.Include(post => post.JoinedUsers)
										.FirstOrDefaultAsync();
		if (resultPost is null) return false;

		var resultUser = await dbContext.Users
										.Where(user => user.Id == userId)
										.FirstOrDefaultAsync();
		if (resultUser is null || resultPost.JoinedUsers.Contains(resultUser))
		{
			return false;
		}

		resultPost.JoinedUsers.Add(resultUser);
		resultPost.CurrentPeople++;

		await dbContext.SaveChangesAsync();

		return true;
	}

	public async Task<bool> AddConversationAsync(ConversationModel model)
	{
		using var dbContext = _dbContextFactory.CreateDbContext();
		var resultPost = await dbContext.Posts
										.Where(post => post.Id == model.PostId)
										.FirstOrDefaultAsync();
		if (resultPost is null) return false;

		var resultUser = await dbContext.Users
										.Where(user => user.Id == model.CreatorId)
										.FirstOrDefaultAsync();
		if (resultUser is null) return false;

		Conversation newConversation = new()
		{
			SendTime = model.SendTime,
			Context = model.Context,
			Post = resultPost,
			Creator = resultUser
		};
		await dbContext.Conversations.AddAsync(newConversation);
		await dbContext.SaveChangesAsync();

		return true;
	}

	public async Task<bool> DeleteCurrentUserAsync(int postId, int userId)
	{
        using var dbContext = _dbContextFactory.CreateDbContext();
        var resultPost = await dbContext.Posts
                                        .Where(post => post.Id == postId)
                                        .Include(post => post.JoinedUsers)
                                        .FirstOrDefaultAsync();
        if (resultPost is null) return false;

        var resultUser = await dbContext.Users
                                        .Where(user => user.Id == userId)
                                        .FirstOrDefaultAsync();
        if (resultUser is null || !resultPost.JoinedUsers.Contains(resultUser))
        {
            return false;
        }

        if (!resultPost.JoinedUsers.Remove(resultUser))
		{
			return false;
		}
		resultPost.CurrentPeople--;

		await dbContext.SaveChangesAsync();

        return true;
	}

	public async Task<Post?> GetPostAsync(int postId)
	{
        using var dbContext = _dbContextFactory.CreateDbContext();
        var resultPost = await dbContext.Posts
                                        .Where(post => post.Id == postId)
                                        .FirstOrDefaultAsync();

        return resultPost;
	}

	public async Task<Post[]> GetPostsAsync(string gameName)
	{
        using var dbContext = _dbContextFactory.CreateDbContext();
        var resultPost = await dbContext.Posts
										.Include(post => post.Game)
                                        .Where(post => post.Game.Name == gameName)
										.Include(post => post.Creator)
                                        .ToArrayAsync();

		return resultPost;
    }

	public async Task<Post[]> GetPostsAsync(IEnumerable<int> postIDs)
	{
        using var dbContext = _dbContextFactory.CreateDbContext();
        var resultPost = await dbContext.Posts
                                        .Where(post => postIDs.Contains(post.Id))
                                        .ToArrayAsync();

        return resultPost;
    }

	// TODO: Try add group by count
	public async Task<string[]> GetMatchTypesAsync(int count, string? gameName = null)
	{
        using var dbContext = _dbContextFactory.CreateDbContext();

		string[]? resultPost;

        if (string.IsNullOrWhiteSpace(gameName))
		{
			resultPost = await dbContext.Posts
										.GroupBy(post => post.MatchType)
										.Select(group => new { Type = group.Key, Count = group.Count() })
										.OrderBy(group => group.Count)
										.Select(group => group.Type)
										.TakeLast(count)
										.ToArrayAsync();
		}
		else
		{
            resultPost = await dbContext.Posts
										// FIXME: .Include(post => post.Game)
										.Where(post => post.Game.Name == gameName)
                                        .GroupBy(post => post.MatchType)
                                        .Select(group => new { Type = group.Key, Count = group.Count() })
                                        .OrderBy(group => group.Count)
                                        .Select(group => group.Type)
                                        .TakeLast(count)
                                        .ToArrayAsync();
        }

		return resultPost;
    }

    public async Task<int[]> GetJoinedUserIds(int postId)
	{
        using var dbContext = _dbContextFactory.CreateDbContext();
		var resultPost = await dbContext.Posts
										.Where(post => post.Id == postId)
										.Include(post => post.JoinedUsers)
										.FirstOrDefaultAsync();
		return resultPost?.JoinedUsers?.Select(user => user.Id).ToArray()
			?? Array.Empty<int>();
    }

	public async Task<Conversation[]> GetConversations(int postId)
	{
        using var dbContext = _dbContextFactory.CreateDbContext();
        var resultPost = await dbContext.Posts
                                        .Where(post => post.Id == postId)
                                        .Include(post => post.Conversations)
                                        .FirstOrDefaultAsync();

        return resultPost?.Conversations?.ToArray() ?? Array.Empty<Conversation>();
    }

    public async Task<string[]> GetAudioPlatformsAsync(int count)
	{
        using var dbContext = _dbContextFactory.CreateDbContext();
        var resultPost = await dbContext.Posts
										.Where(post => !string.IsNullOrWhiteSpace(post.AudioPlatform))
                                        .GroupBy(post => post.AudioPlatform)
                                        .Select(group => new { Type = group.Key, Count = group.Count() })
                                        .OrderBy(group => group.Count)
                                        .Select(group => group.Type)
                                        .TakeLast(count)
                                        .ToArrayAsync();

		return resultPost!;
	}
}