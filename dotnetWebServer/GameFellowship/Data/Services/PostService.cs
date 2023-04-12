using GameFellowship.Data.FormModels;

namespace GameFellowship.Data.Services;

public class PostService : IPostService
{
	private readonly IUserService _userService;

	private List<PostTemp> _posts = new() {
		new PostTemp(
			new DateTime(2023, 2, 2, 2, 2, 2),
			"Minecraft", "种地",
			new string[] {"铁盔甲", "钻石剑", "自带干粮"},
			"种土豆谢谢茄子",
			5, 1, new int[] {1,2,3,5,6},
			new DateTime(2023,3,21), new DateTime(2023,12,29),
			"Kook", "https://www.baidu.com",
			new ConversationTemp[] {
				new(1, "Test, Test, \r\n, Long Context Test1", new DateTime(2023, 1, 11, 11, 11, 11)),
				new(2, "Test, Test, \r\n, Long Context Test1", new DateTime(2023, 2, 22, 22, 22, 22))
			}
		),
		new PostTemp(
			new DateTime(2023, 1, 1, 1, 1, 1),
			"Minecraft", "守村庄",
			new string[] {"钻石甲", "钻石剑", "弩"},
			"救救救救救救救救救救救",
			5, 1, new int[] {1,2,3,5}
		),
		new PostTemp(
			new DateTime(2023, 2, 2, 2, 2, 2),
			"Destiny 2", "Raid",
			new string[] {"1810", "星火术", "速刷"},
			"来打过的谢谢",
			6, 4, new int[] {4,5},
			null, null, null, null,
			new ConversationTemp[] {
				new(4, "Test, Test, \r\n, Long Context Test1", new DateTime(2022, 1, 11, 11, 11, 11)),
				new(5, "Test, Test, \r\n, Long Context Test1", new DateTime(2022, 2, 22, 22, 22, 22))
			}
		),
	};

	public PostService(IUserService userService)
	{
		_userService = userService;
	}

	public Task<(bool, int)> CreateNewPostAsync(PostModel model, int userID)
	{
		if (userID <= 0) return Task.FromResult((false, -1));

		PostTemp newPost = new(model, userID, DateTime.Now);
		_posts.Add(newPost);

		return Task.FromResult((true, newPost.PostID));
	}

	public Task<bool> AddNewCurrentUserAsync(int postID, int userID)
	{
		var resultPost =
			from post in _posts
			where post.PostID == postID
			select post;

		if (!resultPost.Any())
		{
            return Task.FromResult(false);
        }

		if (resultPost.First().CurrentUserIDs.Add(userID))
		{
			++resultPost.First().CurrentPeople;
		}

		return Task.FromResult(true);
	}

	public Task<bool> AddNewConversationAsync(int postID, ConversationTemp conversation)
	{
		var resultPost =
			from post in _posts
			where post.PostID == postID
			select post;

		if (!resultPost.Any())
			return Task.FromResult(false);

		resultPost.First().Conversations.Add(conversation);
        resultPost.First().LastUpdate = conversation.SendTime;

		return Task.FromResult(true);
	}

	public Task<bool> DeleteCurrentUserAsync(int postID, int userID)
	{
		var resultPost =
			from post in _posts
			where post.PostID == postID
			select post;

		if (!resultPost.Any())
			return Task.FromResult(false);

		if (!resultPost.First().CurrentUserIDs.Remove(userID))
			return Task.FromResult(false);

		if (--resultPost.First().CurrentPeople <= 0)
		{
			_userService.DeleteCreatePostAsync(resultPost.First().CreatorID, postID);
			_posts.Remove(resultPost.First());
		}

        return Task.FromResult(true);
	}

	public Task<PostTemp> GetPostAsync(int postID)
	{
		var resultPost =
			from post in _posts
			where post.PostID == postID
			select post;

		if (!resultPost.Any())
			return Task.FromResult(new PostTemp());

		return Task.FromResult(resultPost.First());
	}

	public Task<PostTemp[]> GetPostsAsync(string gameName)
	{
		var resultPosts =
			from post in _posts
			where post.GameName == gameName
			select post;

		if (!resultPosts.Any())
		{
			return Task.FromResult(Array.Empty<PostTemp>());
		}

		return Task.FromResult(resultPosts.ToArray());
	}

	public Task<PostTemp[]> GetPostsAsync(IEnumerable<int> postIDs)
	{
		if (!postIDs.Any())
		{
			return Task.FromResult(Array.Empty<PostTemp>());
		}

		var resultPosts =
			from post in _posts
			where postIDs.Contains(post.PostID)
			select post;

		if (!resultPosts.Any())
		{
			return Task.FromResult(Array.Empty<PostTemp>());
		}

		return Task.FromResult(resultPosts.ToArray());
	}

	// TODO: Try add group by count
	public Task<string[]> GetMatchTypesAsync(int count, string? gameName = null)
	{
		IEnumerable<string> resultMatchTypes;

		if (string.IsNullOrWhiteSpace(gameName))
		{
			resultMatchTypes = (
				from post in _posts
				select post.MatchType
				).Take(count);
		}
		else
		{
			resultMatchTypes = (
				from post in _posts
				where post.GameName.ToLower() == gameName.ToLower()
				select post.MatchType
			).Take(count);
		}

		if (!resultMatchTypes.Any())
		{
			return Task.FromResult(Array.Empty<string>());
		}

		return Task.FromResult(resultMatchTypes.ToArray());
	}

	public Task<string[]> GetAudioPlatformsAsync(int count)
	{
		var resultPlatforms = (
			from post in _posts
			where post.AudioChat
			select post.AudioPlatform
			).Take(count);

		if (!resultPlatforms.Any())
		{
			return Task.FromResult(Array.Empty<string>());
		}

		return Task.FromResult(resultPlatforms.ToArray());
	}
}